// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
/*=============================================================================
**
** Class: SerialStream
**
** Purpose: Class for enabling low-level sync and async control over a serial 
**          : communications resource.
**
** Date: August, 2002
**
=============================================================================*/

using System;
using System.IO;
using System.Text;
using System.ComponentModel;
using System.Resources;
using System.Runtime;
using System.Security;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections;
using System.Data;
using Microsoft.Win32;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using System.Runtime.CompilerServices;



// Notes about the SerialStream:
//    * The stream is always opened via the SerialStream constructor.
//    * The handleProtector guarantees ownership of the file handle, so that it may not be 
//    * unnaturally closed by another process or thread.  Thus, since all properties are available
//    * only when the object exists, the object's properties can be queried only when the SerialStream
//  * object is instantiated (i.e. "open").  
//    * Handles to serial communications resources here always: 
//    * 1) own the handle
//  * 2) are opened for asynchronous operation
//  * 3) set access at the level of FileAccess.ReadWrite
//  * 4) Allow for reading AND writing
//  * 5) Disallow seeking, since they encapsulate a file of type FILE_TYPE_CHAR

namespace mdcore
{

    public delegate void SerialEventHandler(object source, SerialEventArgs e);
    internal delegate int WaitEventCallback();
    internal class SerialStream : Stream
    {

        // members supporting properties exposed to SerialPort
        private string portName;
        private byte parityReplace = (byte)'?';
        private bool dtrEnable;
        private bool rtsEnable;
        private bool inBreak = false;                // port is initially in non-break state
        private Handshake handshake;

        // The internal C# representations of Win32 structures necessary for communication
        // hold most of the internal "fields" maintaining information about the port.  
        private UnsafeNativeMethods.DCB dcb;
        private UnsafeNativeMethods.COMMTIMEOUTS commTimeouts;
        private UnsafeNativeMethods.COMSTAT comStat;
        private UnsafeNativeMethods.COMMPROP commProp;

        // internal-use members
        private const long dsrTimeout = 0L;
        private const int maxDataBits = 8;
        private const int minDataBits = 5;
        private HandleProtector _handleProtector;  // See the HandleProtector class.
        internal bool lastOpTimedOut = false;    // Read returns without error on timeout, so this is internally required to determine timeout.
        private byte[] tempBuf;                    // used to avoid multiple array allocations in ReadByte() 

        // callback-related members, following MSDN's Asyncrhonous Programming Design Pattern.
        private WaitEventCallback myWaitCommCallback;
        private AsyncCallback myAsyncCallback;
        private Object state;

        // called whenever any async i/o operation completes.
        private unsafe static readonly IOCompletionCallback IOCallback = new IOCompletionCallback(SerialStream.AsyncFSCallback);

        // three different events, also wrapped by SerialPort.
        internal event SerialEventHandler ReceivedEvent;    // called when one character is received.
        internal event SerialEventHandler PinChangedEvent; // called when any of the pin/ring-related triggers occurs
        internal event SerialEventHandler ErrorEvent;        // called when any runtime error occurs on the port (frame, overrun, parity, etc.)


        // ----SECTION: inherited properties from Stream class ------------* 

        // These six properites are required for SerialStream to inherit from the abstract Stream class.
        // Note four of them are always true or false, and two of them throw exceptions, so these 
        // are not usefully queried by applications which know they have a SerialStream, etc...
        public override bool CanRead
        {
            get { return (!_handleProtector.IsClosed); }
        }

        public override bool CanSeek
        {
            get { return false; }
        }


        public override bool CanWrite
        {
            get { return (!_handleProtector.IsClosed); }
        }

        public override long Length
        {
            get { throw new NotSupportedException(InternalResources.GetResourceString("NotSupported_UnseekableStream")); }
        }


        public override long Position
        {
            get { throw new NotSupportedException(InternalResources.GetResourceString("NotSupported_UnseekableStream")); }
            set { throw new NotSupportedException(InternalResources.GetResourceString("NotSupported_UnseekableStream")); }
        }

        // ----- new get-set properties -----------------*

        // Standard port properties, also called from SerialPort
        // BaudRate may not be settable to an arbitrary integer between dwMinBaud and dwMaxBaud,
        // and is limited only by the serial driver.  Typically about twelve values such
        // as Winbase.h's CBR_110 through CBR_256000 are used.
        internal int BaudRate
        {
            get { return (int)dcb.BaudRate; }
            set
            {
                if (value <= 0 || (value > commProp.dwMaxBaud && commProp.dwMaxBaud > 0))
                {
                    // if no upper bound on baud rate imposed by serial driver, note that argument must be positive
                    if (commProp.dwMaxBaud == 0)
                    {
                        throw new ArgumentOutOfRangeException("baudRate",
                            InternalResources.GetResourceString("ArgumentOutOfRange_NeedPosNum"));
                    }
                    else
                    {
                        // otherwise, we can present the bounds on the baud rate for this driver
                        throw new ArgumentOutOfRangeException("baudRate",
                            InternalResources.GetResourceString("ArgumentOutOfRange_Bounds_Lower_Upper", 0, commProp.dwMaxBaud));
                    }
                }
                // Set only if it's different.  Rollback to previous values if setting fails.
                //  This pattern occurs through most of the other properties in this class.
                if (value != dcb.BaudRate)
                {
                    int baudRateOld = (int)dcb.BaudRate;
                    dcb.BaudRate = (uint)value;

                    if (UnsafeNativeMethods.SetCommState(_handleProtector.Handle, ref dcb) == false)
                    {
                        dcb.BaudRate = (uint)baudRateOld;
                        InternalResources.WinIOError();
                    }
                }
            }
        }

        internal int DataBits
        {
            get { return (int)dcb.ByteSize; }
            set
            {
                if (value < minDataBits || value > maxDataBits)
                {
                    throw new ArgumentOutOfRangeException("dataBits",
                        InternalResources.GetResourceString("ArgumentOutOfRange_Bounds_Lower_Upper", minDataBits, maxDataBits));
                }
                if (value != dcb.ByteSize)
                {
                    byte byteSizeOld = dcb.ByteSize;
                    dcb.ByteSize = (byte)value;

                    if (UnsafeNativeMethods.SetCommState(_handleProtector.Handle, ref dcb) == false)
                    {
                        dcb.ByteSize = byteSizeOld;
                        InternalResources.WinIOError();
                    }
                }
            }
        }


        internal bool DiscardNull
        {
            get { return (GetDcbFlag(NativeMethods.FNULL) == 1); }
            set
            {
                int fNullFlag = GetDcbFlag(NativeMethods.FNULL);
                if (value == true && fNullFlag == 0 || value == false && fNullFlag == 1)
                {
                    int fNullOld = fNullFlag;
                    SetDcbFlag(NativeMethods.FNULL, value ? 1 : 0);

                    if (UnsafeNativeMethods.SetCommState(_handleProtector.Handle, ref dcb) == false)
                    {
                        SetDcbFlag(NativeMethods.FNULL, fNullOld);
                        InternalResources.WinIOError();
                    }
                }
            }
        }

        internal bool DtrEnable
        {
            get { return dtrEnable; }
            set
            {
                if (value != dtrEnable)
                {
                    bool dtrEnableOld = dtrEnable;
                    int fDtrControlOld = GetDcbFlag(NativeMethods.FDTRCONTROL);

                    dtrEnable = value;
                    SetDcbFlag(NativeMethods.FDTRCONTROL, dtrEnable ? 1 : 0);

                    if (UnsafeNativeMethods.SetCommState(_handleProtector.Handle, ref dcb) == false)
                    {
                        dtrEnable = dtrEnableOld;
                        SetDcbFlag(NativeMethods.FDTRCONTROL, fDtrControlOld);
                        InternalResources.WinIOError();
                    }
                }
            }
        }

        internal Handshake Handshake
        {
            get { return handshake; }
            set
            {

                if (value < Handshake.None || value > Handshake.RequestToSendXOnXOff)
                    throw new ArgumentOutOfRangeException("handshake", InternalResources.GetResourceString("ArgumentOutOfRange_Enum"));

                if (value != handshake)
                {
                    // in the DCB, handshake affects the fRtsControl, fOutxCtsFlow, and fInX, fOutX fields,
                    // so we must save everything in that closure before making any changes.
                    Handshake handshakeOld = handshake;
                    int fInOutXOld = GetDcbFlag(NativeMethods.FINX);
                    int fOutxCtsFlowOld = GetDcbFlag(NativeMethods.FOUTXCTSFLOW);
                    int fRtsControlOld = GetDcbFlag(NativeMethods.FRTSCONTROL);

                    handshake = value;
                    int fInXOutXFlag = (handshake == Handshake.XOnXOff || handshake == Handshake.RequestToSendXOnXOff) ? 1 : 0;
                    SetDcbFlag(NativeMethods.FINX, fInXOutXFlag);
                    SetDcbFlag(NativeMethods.FOUTX, fInXOutXFlag);

                    SetDcbFlag(NativeMethods.FOUTXCTSFLOW, (handshake == Handshake.RequestToSend ||
                        handshake == Handshake.RequestToSendXOnXOff) ? 1 : 0);

                    // handshake and rtsEnable properties are necessary and sufficient to determining 
                    // the fRtsControl field of the DCB.
                    if ((handshake == Handshake.RequestToSend ||
                        handshake == Handshake.RequestToSendXOnXOff))
                    {
                        SetDcbFlag(NativeMethods.FRTSCONTROL, NativeMethods.RTS_CONTROL_HANDSHAKE);
                    }
                    else if (rtsEnable)
                    {
                        SetDcbFlag(NativeMethods.FRTSCONTROL, NativeMethods.RTS_CONTROL_ENABLE);
                    }
                    else
                    {
                        SetDcbFlag(NativeMethods.FRTSCONTROL, NativeMethods.RTS_CONTROL_DISABLE);
                    }

                    if (UnsafeNativeMethods.SetCommState(_handleProtector.Handle, ref dcb) == false)
                    {
                        handshake = handshakeOld;
                        SetDcbFlag(NativeMethods.FINX, fInOutXOld);
                        SetDcbFlag(NativeMethods.FOUTX, fInOutXOld);
                        SetDcbFlag(NativeMethods.FOUTXCTSFLOW, fOutxCtsFlowOld);
                        SetDcbFlag(NativeMethods.FRTSCONTROL, fRtsControlOld);
                        InternalResources.WinIOError();
                    }

                }
            }
        }

        internal Parity Parity
        {
            get { return (Parity)dcb.Parity; }
            set
            {
                if (value < Parity.None || value > Parity.Space)
                    throw new ArgumentOutOfRangeException("parity", InternalResources.GetResourceString("ArgumentOutOfRange_Enum")); ;

                if ((byte)value != dcb.Parity)
                {
                    byte parityOld = dcb.Parity;

                    // in the DCB structure, the parity setting also potentially effects:
                    // fParity, fErrorChar, ErrorChar
                    // so these must be saved as well.
                    int fParityOld = GetDcbFlag(NativeMethods.FPARITY);
                    byte ErrorCharOld = dcb.ErrorChar;
                    int fErrorCharOld = GetDcbFlag(NativeMethods.FPARITY);
                    dcb.Parity = (byte)value;

                    int parityFlag = (dcb.Parity == (byte)Parity.None) ? 1 : 0;
                    SetDcbFlag(NativeMethods.FPARITY, parityFlag);
                    if (parityFlag == 1)
                    {
                        SetDcbFlag(NativeMethods.FERRORCHAR, (parityReplace != '\0') ? 1 : 0);
                        dcb.ErrorChar = parityReplace;
                    }
                    else
                    {
                        SetDcbFlag(NativeMethods.FERRORCHAR, 0);
                        dcb.ErrorChar = (byte)'\0';
                    }
                    if (UnsafeNativeMethods.SetCommState(_handleProtector.Handle, ref dcb) == false)
                    {
                        dcb.Parity = parityOld;
                        SetDcbFlag(NativeMethods.FPARITY, fParityOld);

                        dcb.ErrorChar = ErrorCharOld;
                        SetDcbFlag(NativeMethods.FERRORCHAR, fErrorCharOld);

                        InternalResources.WinIOError();
                    }
                }
            }
        }

        // ParityReplace is the eight-bit character which replaces any bytes which 
        // ParityReplace affects the equivalent field in the DCB structure: ErrorChar, and 
        // the DCB flag fErrorChar.
        internal byte ParityReplace
        {
            get { return parityReplace; }
            set
            {
                if (value != parityReplace)
                {
                    byte parityReplaceOld = parityReplace;
                    byte errorCharOld = dcb.ErrorChar;
                    int fErrorCharOld = GetDcbFlag(NativeMethods.FERRORCHAR);

                    parityReplace = value;
                    if (GetDcbFlag(NativeMethods.FPARITY) == 1)
                    {
                        SetDcbFlag(NativeMethods.FERRORCHAR, (parityReplace != '\0') ? 1 : 0);
                        dcb.ErrorChar = parityReplace;
                    }
                    else
                    {
                        SetDcbFlag(NativeMethods.FERRORCHAR, 0);
                        dcb.ErrorChar = (byte)'\0';
                    }


                    if (UnsafeNativeMethods.SetCommState(_handleProtector.Handle, ref dcb) == false)
                    {
                        parityReplace = parityReplaceOld;
                        SetDcbFlag(NativeMethods.FERRORCHAR, fErrorCharOld);
                        dcb.ErrorChar = errorCharOld;
                        InternalResources.WinIOError();
                    }
                }
            }
        }


        // Timeouts are considered to be TOTAL time for the Read/Write operation and to be in milliseconds.  
        // Timeouts are translated into DCB structure as follows:
        // Desired timeout        =>    ReadTotalTimeoutConstant    ReadTotalTimeoutMultiplier    ReadIntervalTimeout            
        //  0                                    0                            0                MAXDWORD
        //    0 < n < infinity                    n                        MAXDWORD            MAXDWORD
        // infinity                                Int32.MaxValue            MAXDWORD            MAXDWORD
        //
        // rationale for "infinity": There does not exist in the COMMTIMEOUTS structure a way to
        // *wait indefinitely for any byte, return when found*.  Instead, if we set ReadTimeout
        // to infinity, SerialStream's EndRead loops if Int32.MaxValue mills have elapsed
        // without a byte received.  Note that this is approximately 24 days, so essentially
        // most practical purposes effectively equate 24 days with an infinite amount of time
        // on a serial port connection.        
        internal int ReadTimeout
        {
            get
            {
                int constant = commTimeouts.ReadTotalTimeoutConstant;

                // any time user sets timeout to Int32.MaxValue, timeout is assumed
                // to be infinite, and not 2^31 - 1 milliseconds ~= 24 days

                if (constant == Int32.MaxValue) return SerialPort.InfiniteTimeout;
                else return constant;


            }
            set
            {
                if (value < 0 && value != SerialPort.InfiniteTimeout)
                    throw new ArgumentOutOfRangeException("readTimeout", InternalResources.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
                int oldReadConstant = commTimeouts.ReadTotalTimeoutConstant;
                int oldReadInterval = commTimeouts.ReadIntervalTimeout;
                int oldReadMultipler = commTimeouts.ReadTotalTimeoutMultiplier;

                if (value == 0)
                {
                    commTimeouts.ReadTotalTimeoutConstant = commTimeouts.ReadTotalTimeoutMultiplier = 0;
                    commTimeouts.ReadIntervalTimeout = NativeMethods.MAXDWORD;
                }
                else if (value == SerialPort.InfiniteTimeout)
                {

                    commTimeouts.ReadTotalTimeoutConstant = Int32.MaxValue;
                    commTimeouts.ReadTotalTimeoutMultiplier = commTimeouts.ReadIntervalTimeout = NativeMethods.MAXDWORD;
                }
                else
                {
                    commTimeouts.ReadTotalTimeoutConstant = value;
                    commTimeouts.ReadTotalTimeoutMultiplier = commTimeouts.ReadIntervalTimeout = NativeMethods.MAXDWORD;
                }
                if (UnsafeNativeMethods.SetCommTimeouts(_handleProtector.Handle, ref commTimeouts) == false)
                {
                    commTimeouts.ReadTotalTimeoutConstant = oldReadConstant;
                    commTimeouts.ReadTotalTimeoutMultiplier = oldReadMultipler;
                    commTimeouts.ReadIntervalTimeout = oldReadInterval;
                    InternalResources.WinIOError();
                }
            }
        }

        internal bool RtsEnable
        {
            get { return rtsEnable; }
            set
            {
                if (value != rtsEnable)
                {
                    bool rtsEnableOld = rtsEnable;
                    int fRtsControlOld = GetDcbFlag(NativeMethods.FRTSCONTROL);

                    rtsEnable = value;
                    if ((handshake == Handshake.RequestToSend ||
                        handshake == Handshake.RequestToSendXOnXOff))
                    {
                        SetDcbFlag(NativeMethods.FRTSCONTROL, NativeMethods.RTS_CONTROL_HANDSHAKE);
                    }
                    else if (rtsEnable)
                    {
                        SetDcbFlag(NativeMethods.FRTSCONTROL, NativeMethods.RTS_CONTROL_ENABLE);
                    }
                    else
                    {
                        SetDcbFlag(NativeMethods.FRTSCONTROL, NativeMethods.RTS_CONTROL_DISABLE);
                    }

                    if (UnsafeNativeMethods.SetCommState(_handleProtector.Handle, ref dcb) == false)
                    {
                        rtsEnable = rtsEnableOld;
                        SetDcbFlag(NativeMethods.FRTSCONTROL, fRtsControlOld);
                        InternalResources.WinIOError();
                    }
                }
            }
        }

        // StopBits represented in C# as StopBits enum type and in Win32 as an integer 1, 2, or 3.
        internal StopBits StopBits
        {
            get
            {
                switch (dcb.StopBits)
                {
                    case NativeMethods.ONESTOPBIT:
                        return StopBits.One;
                    case NativeMethods.ONE5STOPBITS:
                        return StopBits.OnePointFive;
                    case NativeMethods.TWOSTOPBITS:
                        return StopBits.Two;
                    default:
                        Debug.Assert(true, "Invalid Stopbits value " + dcb.StopBits);
                        return StopBits.One;

                }
            }
            set
            {
                if (value < StopBits.One || value > StopBits.OnePointFive)
                    throw new ArgumentOutOfRangeException("stopBits", InternalResources.GetResourceString("ArgumentOutOfRange_Enum"));

                byte nativeValue = 0;
                if (value == StopBits.One) nativeValue = (byte)NativeMethods.ONESTOPBIT;
                else if (value == StopBits.OnePointFive) nativeValue = (byte)NativeMethods.ONE5STOPBITS;
                else if (value == StopBits.Two) nativeValue = (byte)NativeMethods.TWOSTOPBITS;
                else Debug.Assert(true, "Invalid Stopbits value " + value);


                if (nativeValue != dcb.StopBits)
                {
                    byte stopBitsOld = dcb.StopBits;

                    if (UnsafeNativeMethods.SetCommState(_handleProtector.Handle, ref dcb) == false)
                    {
                        dcb.StopBits = stopBitsOld;
                        InternalResources.WinIOError();
                    }
                }
            }
        }

        // note: WriteTimeout must be either SerialPort.InfiniteTimeout or POSITIVE.
        // a timeout of zero implies that every Write call throws an exception.
        internal int WriteTimeout
        {
            get
            {
                int timeout = commTimeouts.WriteTotalTimeoutConstant;
                return (timeout == 0) ? SerialPort.InfiniteTimeout : timeout;
            }
            set
            {
                if (value <= 0 && value != SerialPort.InfiniteTimeout)
                    throw new ArgumentOutOfRangeException("WriteTimeout", InternalResources.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
                int oldWriteConstant = commTimeouts.WriteTotalTimeoutConstant;
                commTimeouts.WriteTotalTimeoutConstant = ((value == SerialPort.InfiniteTimeout) ? 0 : value);

                if (UnsafeNativeMethods.SetCommTimeouts(_handleProtector.Handle, ref commTimeouts) == false)
                {
                    commTimeouts.WriteTotalTimeoutConstant = oldWriteConstant;
                    InternalResources.WinIOError();
                }
            }
        }


        // CDHolding, CtsHolding, DsrHolding query the current state of each of the carrier, the CTS pin,
        // and the DSR pin, respectively. Read-only.
        // All will throw exceptions if the port is not open.
        internal bool CDHolding
        {
            get
            {
                int pinStatus = 0;
                if (UnsafeNativeMethods.GetCommModemStatus(_handleProtector.Handle, ref pinStatus) == false)
                    InternalResources.WinIOError();

                return (NativeMethods.MS_RLSD_ON & pinStatus) != 0;
            }
        }


        internal bool CtsHolding
        {
            get
            {
                int pinStatus = 0;
                if (UnsafeNativeMethods.GetCommModemStatus(_handleProtector.Handle, ref pinStatus) == false)
                    InternalResources.WinIOError();
                return (NativeMethods.MS_CTS_ON & pinStatus) != 0;
            }

        }

        internal bool DsrHolding
        {
            get
            {
                int pinStatus = 0;
                if (UnsafeNativeMethods.GetCommModemStatus(_handleProtector.Handle, ref pinStatus) == false)
                    InternalResources.WinIOError();

                return (NativeMethods.MS_DSR_ON & pinStatus) != 0;
            }
        }


        // Fills comStat structure from an unmanaged function 
        // to determine the number of bytes waiting in the serial driver's internal receive buffer.
        internal int InBufferBytes
        {
            get
            {
                int errorCode = 0; // "ref" arguments need to have values, as opposed to "out" arguments 
                if (UnsafeNativeMethods.ClearCommError(_handleProtector.Handle, ref errorCode, ref comStat) == false)
                {
                    InternalResources.WinIOError();
                }
                return (int)comStat.cbInQue;
            }
        }

        // Fills comStat structure from an unmanaged function 
        // to determine the number of bytes waiting in the serial driver's internal transmit buffer.
        internal int OutBufferBytes
        {
            get
            {
                int errorCode = 0; // "ref" arguments need to be set before method invocation, as opposed to "out" arguments 
                if (UnsafeNativeMethods.ClearCommError(_handleProtector.Handle, ref errorCode, ref comStat) == false)
                    InternalResources.WinIOError();
                return (int)comStat.cbOutQue;

            }
        }

        // -----------SECTION: constructor --------------------------*

        // this method is used by SerialPort upon SerialStream's creation
        internal SerialStream(string resource, int baudRate, Parity parity, int dataBits, StopBits stopBits, int readTimeout, int writeTimeout, Handshake handshake,
            bool dtrEnable, bool rtsEnable, bool discardNull, byte parityReplace)
        {

            //Error checking done in SerialPort.

            IntPtr tempHandle = UnsafeNativeMethods.CreateFile("\\\\.\\" + resource,
                NativeMethods.GENERIC_READ | NativeMethods.GENERIC_WRITE,
                0,    // comm devices must be opened w/exclusive-access
                NativeMethods.NULL, // no security attributes
                UnsafeNativeMethods.OPEN_EXISTING, // comm devices must use OPEN_EXISTING
                UnsafeNativeMethods.FILE_FLAG_OVERLAPPED |
                UnsafeNativeMethods.FILE_ATTRIBUTE_NORMAL,    // async I/O
                NativeMethods.NULL  // hTemplate must be NULL for comm devices
                );

            if (tempHandle == NativeMethods.INVALID_HANDLE_VALUE)
            {
                int errorCode = Marshal.GetLastWin32Error();
                switch (errorCode)
                {
                    case NativeMethods.ERROR_FILE_NOT_FOUND:
                        throw new FileNotFoundException("ERROR_FILE_NOT_FOUND", InternalResources.GetResourceString("IO.FileNotFound_FileName", resource));

                    case NativeMethods.ERROR_ACCESS_DENIED:
                        throw new UnauthorizedAccessException(InternalResources.GetResourceString("UnauthorizedAccess_IODenied_Path", resource));
                    default:
                        InternalResources.WinIOError();
                        break;
                }
            }

            if (UnsafeNativeMethods.GetFileType(tempHandle) != UnsafeNativeMethods.FILE_TYPE_CHAR)
                throw new ArgumentException("INVALID_RESOURCE_FILE", InternalResources.GetResourceString("Arg_InvalidResourceFile"));

            _handleProtector = new __HandleProtector(tempHandle, true);

            // set properties of the stream that exist as members in SerialStream
            this.portName = resource;
            this.handshake = handshake;
            this.dtrEnable = dtrEnable;
            this.rtsEnable = rtsEnable;
            this.parityReplace = parityReplace;

            tempBuf = new byte[1];            // used in ReadByte()

            // fill COMMPROPERTIES struct, which has our maximum allowed baud rate
            commProp = new UnsafeNativeMethods.COMMPROP();
            if (UnsafeNativeMethods.GetCommProperties(_handleProtector.Handle, ref commProp) == false)
            {
                UnsafeNativeMethods.CloseHandle(_handleProtector.Handle);
                InternalResources.WinIOError();
            }
            if (baudRate > commProp.dwMaxBaud)
                throw new ArgumentOutOfRangeException("baudRate", "Requested baud greater than maximum for this device driver = " + commProp.dwMaxBaud);

            comStat = new UnsafeNativeMethods.COMSTAT();
            // create internal DCB structure, initialize according to Platform SDK 
            // standard: ms-help://MS.MSNDNQTR.2002APR.1003/hardware/commun_965u.htm
            dcb = new UnsafeNativeMethods.DCB();

            // set constant properties of the DCB
            InitializeDCB(baudRate, parity, dataBits, stopBits, discardNull);

            // set timeout defaults
            commTimeouts.ReadIntervalTimeout = (readTimeout == SerialPort.InfiniteTimeout) ? 0 : NativeMethods.MAXDWORD;
            commTimeouts.ReadTotalTimeoutMultiplier = (readTimeout > 0 && readTimeout != SerialPort.InfiniteTimeout)
                                                            ? NativeMethods.MAXDWORD : 0;
            commTimeouts.ReadTotalTimeoutConstant = (readTimeout > 0 && readTimeout != SerialPort.InfiniteTimeout) ?
                                                            readTimeout : 0;
            commTimeouts.WriteTotalTimeoutMultiplier = 0;
            commTimeouts.WriteTotalTimeoutConstant = ((writeTimeout == SerialPort.InfiniteTimeout) ?
                                                            0 : writeTimeout);
            // note - we cannot have a meaningful conception of a 0 write timeout, since every write takes at 
            // least 0 mills.

            // set unmanaged timeout structure
            if (UnsafeNativeMethods.SetCommTimeouts(_handleProtector.Handle, ref commTimeouts) == false)
            {
                UnsafeNativeMethods.CloseHandle(_handleProtector.Handle);
                InternalResources.WinIOError();
            }

            if (!ThreadPool.BindHandle(_handleProtector.Handle))
            {
                throw new IOException(InternalResources.GetResourceString("IO.IO_BindHandleFailed"));
            }

            // prep. for starting event cycle.
            myWaitCommCallback = new WaitEventCallback(WaitForCommEvent);
            myAsyncCallback = new AsyncCallback(EndWaitForCommEvent);
            state = null;    // no need for new object, since we never use it.

            IAsyncResult ar = myWaitCommCallback.BeginInvoke(myAsyncCallback, state);
        }

        ~SerialStream()
        {
            if (_handleProtector != null)
            {
                Dispose(false);
            }
        }

        protected virtual void Dispose(bool disposing)
        {

            // Nothing will be done differently based on whether we are 
            // disposing vs. finalizing.

            // Signal the other side that we're closing.  Should do regardless of whether we've called
            // Close() or not Dispose() 
            if (_handleProtector != null)
            {
                if (!_handleProtector.IsClosed)
                {

                    if (!UnsafeNativeMethods.EscapeCommFunction(_handleProtector.Handle, NativeMethods.CLRDTR))
                    {
                        // should not happen
                        InternalResources.WinIOError();
                    }
                    Flush();
                    _handleProtector.Close();
                }
            }


        }

        // -----SECTION: all public methods ------------------*

        // User-accessible async read method.  Returns AsyncSerialStream_AsyncResult : IAsyncResult
        public override IAsyncResult BeginRead(byte[] array, int offset, int numBytes, AsyncCallback userCallback, object stateObject)
        {
            return BeginRead(array, offset, numBytes, userCallback, stateObject, ReadTimeout);
        }


        internal IAsyncResult BeginRead(byte[] array, int offset, int numBytes,
            AsyncCallback userCallback, object stateObject, int timeout)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", InternalResources.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            if (numBytes < 0)
                throw new ArgumentOutOfRangeException("numBytes", InternalResources.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            if (array.Length - offset < numBytes)
                throw new ArgumentException(InternalResources.GetResourceString("Argument_InvalidOffLen"));
            if (_handleProtector.IsClosed) InternalResources.FileNotOpen();
            return BeginReadCore(array, offset, numBytes, userCallback, stateObject, 0, timeout);
        }

        // User-accessible async write method.  Returns AsyncSerialStream_AsyncResult : IAsyncResult
        // Throws an exception if port is in break state.
        public override IAsyncResult BeginWrite(byte[] array, int offset, int numBytes,
            AsyncCallback userCallback, object stateObject)
        {
            return BeginWrite(array, offset, numBytes, userCallback, stateObject, WriteTimeout);
        }

        internal IAsyncResult BeginWrite(byte[] array, int offset, int numBytes,
            AsyncCallback userCallback, object stateObject, int timeout)
        {
            if (inBreak)
                throw new InvalidOperationException("BeginWrite in break");
            if (array == null)
                throw new ArgumentNullException("array");
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", InternalResources.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            if (numBytes < 0)
                throw new ArgumentOutOfRangeException("numBytes", InternalResources.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            if (array.Length - offset < numBytes)
                throw new ArgumentException(InternalResources.GetResourceString("Argument_InvalidOffLen"));

            if (_handleProtector.IsClosed) InternalResources.FileNotOpen();
            return BeginWriteCore(array, offset, numBytes, userCallback, stateObject, timeout);
        }

        // Equivalent to MSComm's Break = false
        internal void ClearBreak()
        {
            if (UnsafeNativeMethods.ClearCommBreak(_handleProtector.Handle) == false)
                InternalResources.WinIOError();
            inBreak = false;
        }

        // handle protector itself is closed in the Dispose() method, since we can Dispose() without calling
        // Close() in some cases.
        public override void Close()
        {
            if (_handleProtector.IsClosed) InternalResources.FileNotOpen();
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Uses Win32 method to dump out the receive buffer; analagous to MSComm's "InBufferCount = 0"
        internal void DiscardInBuffer()
        {

            if (UnsafeNativeMethods.PurgeComm(_handleProtector.Handle, NativeMethods.PURGE_RXCLEAR) == false)
                InternalResources.WinIOError();
        }

        // Uses Win32 method to dump out the xmit buffer; analagous to MSComm's "OutBufferCount = 0"
        internal void DiscardOutBuffer()
        {
            if (UnsafeNativeMethods.PurgeComm(_handleProtector.Handle, NativeMethods.PURGE_TXCLEAR) == false)
                InternalResources.WinIOError();
        }

        // Async companion to BeginRead.  
        // Note, assumed IAsyncResult argument is of derived type AsyncSerialStream_AsyncResult,
        // and throws an exception if untrue.
        public unsafe override int EndRead(IAsyncResult asyncResult)
        {
            if (_handleProtector.IsClosed) InternalResources.FileNotOpen();
            if (asyncResult == null)
                throw new ArgumentNullException("asyncResult");

            AsyncSerialStream_AsyncResult afsar = asyncResult as AsyncSerialStream_AsyncResult;
            if (afsar == null || afsar._isWrite)
                InternalResources.WrongAsyncResult();

            // This sidesteps race conditions, avoids memory corruption after freeing the
            // NativeOverlapped class or GCHandle twice.  
            if (1 == Interlocked.CompareExchange(ref afsar._EndXxxCalled, 1, 0))
                InternalResources.EndReadCalledTwice();


            WaitHandle wh = afsar.AsyncWaitHandle;
            if (wh != null)
            {

                if (!afsar.IsCompleted)
                {
                    do
                    {
                        // Since ReadFile() drops after a timeout (set above with SetCommTimeouts())
                        // without a guarantee of setting the error property to indicate this, calculate
                        // actual time elapsed here.
                        // The granularity of the system is such that border cases may go either way,
                        // but Windows makes no guarantees anyway.
                        int beginTicks = SafeNativeMethods.GetTickCount();
                        wh.WaitOne();
                        // There's a subtle race condition here.  In AsyncFSCallback,
                        // I must signal the WaitHandle then set _isComplete to be true,
                        // to avoid closing the WaitHandle before AsyncFSCallback has
                        // signalled it.  But with that behavior and the optimization
                        // to call WaitOne only when IsCompleted is false, it's possible
                        // to return from this method before IsCompleted is set to true.
                        // This is currently completely harmless, so the most efficient
                        // solution of just setting the field seems like the right thing
                        // to do.     
                        int currentTimeout = ReadTimeout;
                        int endTicks = SafeNativeMethods.GetTickCount();
                        if (endTicks - beginTicks >= currentTimeout && currentTimeout != SerialPort.InfiniteTimeout)
                            lastOpTimedOut = true;
                        else
                            lastOpTimedOut = false;
                    } while (afsar._numBytes == 0 && ReadTimeout == SerialPort.InfiniteTimeout);

                    afsar._isComplete = true;
                }
                wh.Close();
            }

            // Free memory, GC handles.
            NativeOverlapped* overlappedPtr = afsar._overlapped;
            if (overlappedPtr != null)
                Overlapped.Free(overlappedPtr);
            afsar.UnpinBuffer();

            // Check for non-timeout errors during the read.
            if (afsar._errorCode != 0)
                InternalResources.WinIOError(afsar._errorCode, portName);

            // return to old timeout.  Note that this class is not thread-safe, and the timeout of read operation A 
            // is actually the minimum timeout of any read operation plus the time from A's invocation that B set a minimum timeout.
            // NOT THREAD SAFE. 
            ReadTimeout = afsar._oldTimeout;
            return afsar._numBytes + afsar._numBufferedBytes;
        }

        // Async companion to BeginWrite.  
        // Note, assumed IAsyncResult argument is of derived type AsyncSerialStream_AsyncResult,
        // and throws an exception if untrue.
        // Also fails if called in port's break state.
        public unsafe override void EndWrite(IAsyncResult asyncResult)
        {
            if (_handleProtector.IsClosed) InternalResources.FileNotOpen();
            if (inBreak)
                throw new InvalidOperationException("EndWrite in break");
            if (asyncResult == null)
                throw new ArgumentNullException("asyncResult");

            AsyncSerialStream_AsyncResult afsar = asyncResult as AsyncSerialStream_AsyncResult;
            if (afsar == null || !afsar._isWrite)
                InternalResources.WrongAsyncResult();

            // This sidesteps race conditions, avoids memory corruption after freeing the
            // NativeOverlapped class or GCHandle twice.  
            if (1 == Interlocked.CompareExchange(ref afsar._EndXxxCalled, 1, 0))
                InternalResources.EndWriteCalledTwice();

            WaitHandle wh = afsar.AsyncWaitHandle;
            if (wh != null)
            {
                if (!afsar.IsCompleted)
                {
                    int beginTicks = SafeNativeMethods.GetTickCount();
                    // Since WriteFile() drops after a timeout (set above with SetCommTimeouts())
                    // without a guarantee of setting the error property to indicate this, calculate
                    // actual time elapsed here.
                    // The granularity of the system is such that border cases may go either way,
                    // but Windows makes no guarantees anyway.  Plus, write timeouts should be extremely rare. 

                    wh.WaitOne();
                    // There's a subtle race condition here.  In AsyncFSCallback,
                    // I must signal the WaitHandle then set _isComplete to be true,
                    // to avoid closing the WaitHandle before AsyncFSCallback has
                    // signalled it.  But with that behavior and the optimization
                    // to call WaitOne only when IsCompleted is false, it's possible
                    // to return from this method before IsCompleted is set to true.
                    // This is currently completely harmless, so the most efficient
                    // solution of just setting the field seems like the right thing
                    // to do.    
                    int currentTimeout = WriteTimeout;
                    int endTicks = SafeNativeMethods.GetTickCount();
                    if (endTicks - beginTicks >= currentTimeout && currentTimeout != SerialPort.InfiniteTimeout)
                        throw new TimeoutException("Write Timed Out: " + (endTicks - beginTicks) + " > " + currentTimeout);
                    afsar._isComplete = true;
                }
                wh.Close();
            }

            // Free memory, GC handles.
            NativeOverlapped* overlappedPtr = afsar._overlapped;
            if (overlappedPtr != null)
                Overlapped.Free(overlappedPtr);
            afsar.UnpinBuffer();

            // Now check for any error during the write.
            if (afsar._errorCode != 0)
                InternalResources.WinIOError(afsar._errorCode, portName);

            // return to old timeout. See read timeout commentary on race conditions.

            WriteTimeout = afsar._oldTimeout;
            // Number of bytes written is afsar._numBytes + afsar._numBufferedBytes.
            return;
        }

        // Flush dumps the contents of the serial driver's internal read and write buffers.
        // We actually expose the functionality for each, but fulfilling Stream's contract
        // requires a Flush() method.  Fails if handle closed.
        // Note: Serial driver's write buffer is *already* attempting to write it, so we can only wait until it finishes.
        public override void Flush()
        {
            if (_handleProtector.Handle == NativeMethods.NULL) throw new InvalidOperationException("Flush - Stream not open!");
            DiscardInBuffer();
            DiscardOutBuffer();
            return;
        }

        // Blocking read operation, returning the number of bytes read from the stream.  

        public override int Read([In, Out] byte[] array, int offset, int count)
        {
            return Read(array, offset, count, ReadTimeout);
        }

        internal int Read([In, Out] byte[] array, int offset, int count, int timeout)
        {
            if (array == null)
                throw new ArgumentNullException("array", InternalResources.GetResourceString("ArgumentNull_Buffer"));
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", InternalResources.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", InternalResources.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            if (array.Length - offset < count)
                throw new ArgumentException(InternalResources.GetResourceString("Argument_InvalidOffLen"));
            if (count == 0) return 0; // return immediately if no bytes requested; no need for overhead.

            Debug.Assert(timeout == SerialPort.InfiniteTimeout || timeout >= 0, "Serial Stream Read - called with timeout " + timeout);

            // Check to see we have no handle-related error, since the port's always supposed to be open.
            if (_handleProtector.IsClosed) InternalResources.FileNotOpen();

            IAsyncResult result = BeginReadCore(array, offset, count, null, null, 0, timeout);
            return EndRead(result);

        }


        public override int ReadByte()
        {
            return ReadByte(ReadTimeout);
        }

        internal int ReadByte(int timeout)
        {
            if (_handleProtector.IsClosed) InternalResources.FileNotOpen();

            IAsyncResult result = BeginReadCore(tempBuf, 0, 1, null, null, 0, timeout);
            int res = EndRead(result);

            if (lastOpTimedOut)
                return -1;
            else
                return tempBuf[0];
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException(InternalResources.GetResourceString("NotSupported_UnseekableStream"));
        }

        // Equivalent to MSComm's Break = true        
        internal void SetBreak()
        {

            if (UnsafeNativeMethods.SetCommBreak(_handleProtector.Handle) == false)
                InternalResources.WinIOError();
            inBreak = true;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException(InternalResources.GetResourceString("NotSupported_UnseekableStream"));
        }


        public override void Write(byte[] array, int offset, int count)
        {
            Write(array, offset, count, WriteTimeout);
        }
        internal void Write(byte[] array, int offset, int count, int timeout)
        {

            if (inBreak)
                throw new InvalidOperationException("Write in break");
            if (array == null)
                throw new ArgumentNullException("write buffer", InternalResources.GetResourceString("ArgumentNull_Array"));
            if (offset < 0)
                throw new ArgumentOutOfRangeException("write offset", InternalResources.GetResourceString("ArgumentOutOfRange_NeedPosNum"));
            if (count < 0)
                throw new ArgumentOutOfRangeException("write count", InternalResources.GetResourceString("ArgumentOutOfRange_NeedPosNum"));
            if (count == 0) return;    // no need to expend overhead in creating asyncResult, etc.                
            if (array.Length - offset < count)
                throw new ArgumentException("write count", InternalResources.GetResourceString("ArgumentOutOfRange_OffsetOut"));
            Debug.Assert(timeout == SerialPort.InfiniteTimeout || timeout >= 0, "Serial Stream Write - write timeout is " + timeout);

            // check for open handle, though the port is always supposed to be open
            if (_handleProtector.IsClosed) InternalResources.FileNotOpen();

            IAsyncResult result = BeginWriteCore(array, offset, count, null, null, timeout);
            EndWrite(result);

            return;

        }

        // use default timeout as argument to WriteByte override with timeout arg
        public override void WriteByte(byte value)
        {
            WriteByte(value, WriteTimeout);
        }

        internal void WriteByte(byte value, int timeout)
        {
            if (inBreak)
                throw new InvalidOperationException("WriteByte in break");

            if (_handleProtector.IsClosed) InternalResources.FileNotOpen();
            tempBuf[0] = value;
            IAsyncResult ar = BeginWriteCore(tempBuf, 0, 1, null, null, timeout);
            EndWrite(ar);

            return;
        }



        // --------SUBSECTION: internal-use methods ----------------------*
        // ------ internal DCB-supporting methods ------- * 

        // Initializes unmananged DCB struct, to be called after opening communications resource. 
        // assumes we have already: baudRate, parity, dataBits, stopBits
        // should only be called in SerialStream(...) 
        private void InitializeDCB(int baudRate, Parity parity, int dataBits, StopBits stopBits, bool discardNull)
        {

            // first get the current dcb structure setup
            if (UnsafeNativeMethods.GetCommState(_handleProtector.Handle, ref dcb) == false)
            {
                InternalResources.WinIOError();
            }
            dcb.DCBlength = (uint)System.Runtime.InteropServices.Marshal.SizeOf(dcb);

            // set parameterized properties
            dcb.BaudRate = (uint)baudRate;
            dcb.ByteSize = (byte)dataBits;


            switch (stopBits)
            {
                case StopBits.One:
                    dcb.StopBits = NativeMethods.ONESTOPBIT;
                    break;
                case StopBits.OnePointFive:
                    dcb.StopBits = NativeMethods.ONE5STOPBITS;
                    break;
                case StopBits.Two:
                    dcb.StopBits = NativeMethods.TWOSTOPBITS;
                    break;
                default:
                    Debug.Assert(true, "TBD");
                    break;
            }

            dcb.Parity = (byte)parity;
            // SetDcbFlag, GetDcbFlag expose access to each of the relevant bits of the 32-bit integer 
            // storing all flags of the DCB.  C# provides no direct means of manipulating bit fields, so
            // this is the solution.
            SetDcbFlag(NativeMethods.FPARITY, ((parity == Parity.None) ? 0 : 1));

            SetDcbFlag(NativeMethods.FBINARY, 1);    // always true for communications resources

            // set DCB fields implied by default and the arguments given.
            // Boolean fields in C# must become 1, 0 to properly set the bit flags in the unmanaged DCB struct    

            SetDcbFlag(NativeMethods.FOUTXCTSFLOW, ((handshake == Handshake.RequestToSend ||
                handshake == Handshake.RequestToSendXOnXOff) ? 1 : 0));
            SetDcbFlag(NativeMethods.FOUTXDSRFLOW, (dsrTimeout != 0L) ? 1 : 0);
            SetDcbFlag(NativeMethods.FDTRCONTROL, (dtrEnable) ? NativeMethods.DTR_CONTROL_ENABLE : NativeMethods.DTR_CONTROL_DISABLE);
            SetDcbFlag(NativeMethods.FDSRSENSITIVITY, 0); // this should remain off 
            SetDcbFlag(NativeMethods.FINX, (handshake == Handshake.XOnXOff || handshake == Handshake.RequestToSendXOnXOff) ? 1 : 0);
            SetDcbFlag(NativeMethods.FOUTX, (handshake == Handshake.XOnXOff || handshake == Handshake.RequestToSendXOnXOff) ? 1 : 0);

            // if no parity, we have no error character (i.e. ErrorChar = '\0' or null character)
            if (parity != Parity.None)
            {
                SetDcbFlag(NativeMethods.FERRORCHAR, (parityReplace != '\0') ? 1 : 0);
                dcb.ErrorChar = parityReplace;
            }
            else
            {
                SetDcbFlag(NativeMethods.FERRORCHAR, 0);
                dcb.ErrorChar = (byte)'\0';
            }

            // this method only runs once in the constructor, so we only have the default value to use.
            // Later the user may change this via the NullDiscard property.
            SetDcbFlag(NativeMethods.FNULL, discardNull ? 1 : 0);


            // Setting RTS control, which is RTS_CONTROL_HANDSHAKE if RTS / RTS-XOnXOff handshaking
            // used, RTS_ENABLE (RTS pin used during operation) if rtsEnable true but XOnXoff / No handshaking
            // used, and disabled otherwise.
            if ((handshake == Handshake.RequestToSend ||
                handshake == Handshake.RequestToSendXOnXOff))
            {
                SetDcbFlag(NativeMethods.FRTSCONTROL, NativeMethods.RTS_CONTROL_HANDSHAKE);
            }
            else if (rtsEnable)
            {
                SetDcbFlag(NativeMethods.FRTSCONTROL, NativeMethods.RTS_CONTROL_ENABLE);
            }
            else
            {
                SetDcbFlag(NativeMethods.FRTSCONTROL, NativeMethods.RTS_CONTROL_DISABLE);
            }

            dcb.XonChar = NativeMethods.DEFAULTXONCHAR;                // may be exposed later but for now, constant
            dcb.XoffChar = NativeMethods.DEFAULTXOFFCHAR;

            // minimum number of bytes allowed in each buffer before flow control activated
            // heuristically, this has been set at 1/4 of the buffer size
            dcb.XonLim = dcb.XoffLim = (ushort)(commProp.dwCurrentRxQueue / 4);

            dcb.EofChar = NativeMethods.EOFCHAR;

            //OLD MSCOMM: dcb.EvtChar = (byte) 0;
            // now changed to make use of RxFlag WaitCommEvent event => EofReceived WaitForCommEvent event
            dcb.EvtChar = NativeMethods.EOFCHAR;

            // set DCB structure
            if (UnsafeNativeMethods.SetCommState(_handleProtector.Handle, ref dcb) == false)
            {
                InternalResources.WinIOError();
            }
        }

        // Here we provide a method for getting the flags of the Device Control Block structure dcb
        // associated with each instance of SerialStream, i.e. this method gets myStream.dcb.Flags
        // Flags are any of the constants in NativeMethods such as FBINARY, FDTRCONTROL, etc.
        internal int GetDcbFlag(int whichFlag)
        {
            uint mask;

            Debug.Assert(whichFlag >= NativeMethods.FBINARY && whichFlag <= NativeMethods.FDUMMY2, "GetDcbFlag needs to fit into enum!");

            if (whichFlag == NativeMethods.FDTRCONTROL || whichFlag == NativeMethods.FRTSCONTROL)
            {
                mask = 0x3;
            }
            else if (whichFlag == NativeMethods.FDUMMY2)
            {
                mask = 0x1FFFF;
            }
            else
            {
                mask = 0x1;
            }
            uint result = dcb.Flags & (mask << whichFlag);
            return (int)(result >> whichFlag);
        }

        // Since C# applications have to provide a workaround for accessing and setting bitfields in unmanaged code,
        // here we provide methods for getting and setting the Flags field of the Device Control Block structure dcb
        // associated with each instance of SerialStream, i.e. this method sets myStream.dcb.Flags
        // Flags are any of the constants in NativeMethods such as FBINARY, FDTRCONTROL, etc.
        internal void SetDcbFlag(int whichFlag, int setting)
        {
            uint mask;
            setting = setting << whichFlag;

            Debug.Assert(whichFlag >= NativeMethods.FBINARY && whichFlag <= NativeMethods.FDUMMY2, "SetDcbFlag needs to fit into enum!");

            if (whichFlag == NativeMethods.FDTRCONTROL || whichFlag == NativeMethods.FRTSCONTROL)
            {
                mask = 0x3;
            }
            else if (whichFlag == NativeMethods.FDUMMY2)
            {
                mask = 0x1FFFF;
            }
            else
            {
                mask = 0x1;
            }

            // clear the region
            dcb.Flags &= ~(mask << whichFlag);

            // set the region
            dcb.Flags |= ((uint)setting);
        }

        // ----SUBSECTION: internal methods supporting public read/write methods-------*

        unsafe private AsyncSerialStream_AsyncResult BeginReadCore(byte[] array, int offset, int numBytes, AsyncCallback userCallback, Object stateObject, int numBufferedBytesRead, int timeout)
        {

            // Create and store async stream class library specific data in the 
            // async result
            AsyncSerialStream_AsyncResult asyncResult = new AsyncSerialStream_AsyncResult();
            asyncResult._userCallback = userCallback;
            asyncResult._userStateObject = stateObject;
            asyncResult._isWrite = false;

            asyncResult._oldTimeout = ReadTimeout; // store "old" timeout
            ReadTimeout = timeout;    // enforce timeouts in COMMTIMEOUTS structure

            // Must set this here to ensure all the state on the IAsyncResult 
            // object is set before we call ReadFile, which gives the OS an
            // opportunity to run our callback (including the user callback &
            // the call to EndRead) before ReadFile has returned.  
            asyncResult._numBufferedBytes = numBufferedBytesRead;

            // For Synchronous IO, I could go with either a callback and using
            // the managed Monitor class, or I could create a handle and wait on it.
            ManualResetEvent waitHandle = new ManualResetEvent(false);
            asyncResult._waitHandle = waitHandle;

            // Create a managed overlapped class
            // We will set the file offsets later
            Overlapped overlapped = new Overlapped(0, 0, 0, asyncResult);

            // Pack the Overlapped class, and store it in the async result
            NativeOverlapped* intOverlapped = overlapped.Pack(IOCallback);
            asyncResult._overlapped = intOverlapped;

            // Keep the array in one location in memory until the OS writes the
            // relevant data into the array.  Free GCHandle later.
            asyncResult.PinBuffer(array);

            // queue an async ReadFile operation and pass in a packed overlapped
            //int r = ReadFile(_handle, array, numBytes, null, intOverlapped);
            int hr = 0;
            int r = ReadFileNative(_handleProtector, array, offset, numBytes,
             intOverlapped, out hr);

            // ReadFile, the OS version, will return 0 on failure.  But
            // my ReadFileNative wrapper returns -1.  My wrapper will return
            // the following:
            // On error, r==-1.
            // On async requests that are still pending, r==-1 w/ hr==ERROR_IO_PENDING
            // on async requests that completed sequentially, r==0
            // Note that you will NEVER RELIABLY be able to get the number of bytes
            // read back from this call when using overlapped structures!  You must
            // not pass in a non-null lpNumBytesRead to ReadFile when using 
            // overlapped structures! 
            if (r == -1)
            {
                if (hr != NativeMethods.ERROR_IO_PENDING)
                {
                    if (hr == NativeMethods.ERROR_HANDLE_EOF)
                        InternalResources.EndOfFile();
                    else
                        InternalResources.WinIOError(hr, String.Empty);
                }
            }

            return asyncResult;
        }

        unsafe private AsyncSerialStream_AsyncResult BeginWriteCore(byte[] array, int offset, int numBytes, AsyncCallback userCallback, Object stateObject, int timeout)
        {

            // Create and store async stream class library specific data in the 
            // async result
            AsyncSerialStream_AsyncResult asyncResult = new AsyncSerialStream_AsyncResult();
            asyncResult._userCallback = userCallback;
            asyncResult._userStateObject = stateObject;
            asyncResult._isWrite = true;

            asyncResult._oldTimeout = WriteTimeout;        // set "old" timeout
            WriteTimeout = timeout;

            // For Synchronous IO, I could go with either a callback and using
            // the managed Monitor class, or I could create a handle and wait on it.
            ManualResetEvent waitHandle = new ManualResetEvent(false);
            asyncResult._waitHandle = waitHandle;

            // Create a managed overlapped class
            // We will set the file offsets later
            Overlapped overlapped = new Overlapped(0, 0, 0, asyncResult);

            // Pack the Overlapped class, and store it in the async result
            NativeOverlapped* intOverlapped = overlapped.Pack(IOCallback);
            asyncResult._overlapped = intOverlapped;


            // Keep the array in one location in memory until the OS reads the
            // relevant data from the array.  Free GCHandle later.
            asyncResult.PinBuffer(array);

            int hr = 0;
            // queue an async WriteFile operation and pass in a packed overlapped
            int r = WriteFileNative(_handleProtector, array, offset, numBytes, intOverlapped, out hr);

            // WriteFile, the OS version, will return 0 on failure.  But
            // my WriteFileNative wrapper returns -1.  My wrapper will return
            // the following:
            // On error, r==-1.
            // On async requests that are still pending, r==-1 w/ hr==ERROR_IO_PENDING
            // On async requests that completed sequentially, r==0
            // Note that you will NEVER RELIABLY be able to get the number of bytes
            // written back from this call when using overlapped IO!  You must
            // not pass in a non-null lpNumBytesWritten to WriteFile when using 
            // overlapped structures! 
            if (r == -1)
            {
                if (hr != NativeMethods.ERROR_IO_PENDING)
                {

                    if (hr == NativeMethods.ERROR_HANDLE_EOF)
                        InternalResources.EndOfFile();
                    else
                        InternalResources.WinIOError(hr, String.Empty);
                }
            }
            return asyncResult;
        }


        // Internal method, wrapping the PInvoke to ReadFile().
        internal unsafe int ReadFileNative(HandleProtector hp, byte[] bytes, int offset, int count, NativeOverlapped* overlapped, out int hr)
        {

            // Don't corrupt memory when multiple threads are erroneously writing
            // to this stream simultaneously.
            if (bytes.Length - offset < count)
                throw new IndexOutOfRangeException(InternalResources.GetResourceString("IndexOutOfRange_IORaceCondition"));

            // You can't use the fixed statement on an array of length 0.
            if (bytes.Length == 0)
            {
                hr = 0;
                return 0;
            }

            int r = 0;
            int numBytesRead = 0;

            bool incremented = false;
            try
            {
                if (hp.TryAddRef(ref incremented))
                {
                    fixed (byte* p = bytes)
                    {
                        r = UnsafeNativeMethods.ReadFile(hp.Handle, p + offset, count, NativeMethods.NULL, overlapped);
                    }
                }
                else
                    hr = NativeMethods.ERROR_INVALID_HANDLE;  // Handle was closed.
            }
            finally
            {
                if (incremented) hp.Release();
            }

            if (r == 0)
            {
                hr = Marshal.GetLastWin32Error();

                // Note: we should never silently swallow an error here without some
                // extra work.  We must make sure that BeginReadCore won't return an 
                // IAsyncResult that will cause EndRead to block, since the OS won't
                // call AsyncFSCallback for us.  

                // For invalid handles, detect the error and mark our handle
                // as closed to give slightly better error messages.  Also
                // help ensure we avoid handle recycling bugs.
                if (hr == NativeMethods.ERROR_INVALID_HANDLE)
                    _handleProtector.ForciblyMarkAsClosed();

                return -1;
            }
            else
                hr = 0;
            return numBytesRead;
        }

        internal unsafe int WriteFileNative(HandleProtector hp, byte[] bytes, int offset, int count, NativeOverlapped* overlapped, out int hr)
        {

            // Don't corrupt memory when multiple threads are erroneously writing
            // to this stream simultaneously.  (Note that the OS is reading from
            // the array we pass to WriteFile, but if we read beyond the end and
            // that memory isn't allocated, we could get an AV.)
            if (bytes.Length - offset < count)
                throw new IndexOutOfRangeException(InternalResources.GetResourceString("IndexOutOfRange_IORaceCondition"));

            // You can't use the fixed statement on an array of length 0.
            if (bytes.Length == 0)
            {
                hr = 0;
                return 0;
            }

            int numBytesWritten = 0;
            int r = 0;

            bool incremented = false;
            try
            {
                if (hp.TryAddRef(ref incremented))
                {
                    fixed (byte* p = bytes)
                    {
                        r = UnsafeNativeMethods.WriteFile(hp.Handle, p + offset, count, NativeMethods.NULL, overlapped);
                    }
                }
                else
                    hr = NativeMethods.ERROR_INVALID_HANDLE;  // Handle was closed.
            }
            finally
            {
                if (incremented) hp.Release();
            }

            if (r == 0)
            {
                hr = Marshal.GetLastWin32Error();
                // Note: we should never silently swallow an error here without some
                // extra work.  We must make sure that BeginWriteCore won't return an 
                // IAsyncResult that will cause EndWrite to block, since the OS won't
                // call AsyncFSCallback for us.  

                // For invalid handles, detect the error and mark our handle
                // as closed to give slightly better error messages.  Also
                // help ensure we avoid handle recycling bugs.
                if (hr == NativeMethods.ERROR_INVALID_HANDLE)
                    _handleProtector.ForciblyMarkAsClosed();

                return -1;
            }
            else
                hr = 0;
            return numBytesWritten;
        }

        // ----SUBSECTION: internal methods supporting events/async operation------*

        // This is the blocking method that waits for an event to occur.  It wraps the SDK's WaitCommEvent function.
        private unsafe int WaitForCommEvent()
        {
            int eventsOccurred = 0;
            // monitor all events except TXEMPTY
            UnsafeNativeMethods.SetCommMask(_handleProtector.Handle, NativeMethods.ALL_EVENTS);

            AsyncSerialStream_AsyncResult asyncResult = new AsyncSerialStream_AsyncResult();
            asyncResult._userCallback = null;
            asyncResult._userStateObject = null;
            asyncResult._isWrite = false;
            asyncResult._oldTimeout = -1;
            asyncResult._numBufferedBytes = 0;
            ManualResetEvent waitHandle = new ManualResetEvent(false);
            asyncResult._waitHandle = waitHandle;

            Overlapped overlapped = new Overlapped(0, 0, 0, asyncResult);

            // Pack the Overlapped class, and store it in the async result
            NativeOverlapped* intOverlapped = overlapped.Pack(IOCallback);

            if (UnsafeNativeMethods.WaitCommEvent(_handleProtector.Handle, ref eventsOccurred, intOverlapped) == false)
            {
                int hr = Marshal.GetLastWin32Error();
                if (hr == NativeMethods.ERROR_IO_PENDING)
                {
                    int temp = UnsafeNativeMethods.WaitForSingleObject(waitHandle.Handle, -1);
                    if (temp == 0) // no error
                        return eventsOccurred;
                    else
                        InternalResources.WinIOError();
                }
                else
                    InternalResources.WinIOError();

            }
            return eventsOccurred;
        }

        // This is the cleanup method called when WaitCommEvent() has returned.
        [OneWayAttribute()]
        private void EndWaitForCommEvent(IAsyncResult ar)
        {

            int errorEvents = (int)(SerialEvents.Frame | SerialEvents.Overrun
                | SerialEvents.RxOver | SerialEvents.RxParity | SerialEvents.TxFull);
            int receivedEvents = (int)(SerialEvents.ReceivedChars | SerialEvents.EofReceived);
            int pinChangedEvents = (int)(SerialEvents.Break | SerialEvents.CDChanged | SerialEvents.CtsChanged |
                SerialEvents.Ring | SerialEvents.DsrChanged);

            // Extract the delegate from the AsynchResult.
            WaitEventCallback myWaitCommCallback =
                (WaitEventCallback)((AsyncResult)ar).AsyncDelegate;
            // Obtain the result. 
            // The EV_CTS, EV_RLSD, EV_DSR, EV_BREAK, EV_RING, EV_RXCHAR match up with SerialEvents constants
            int nativeEvents = myWaitCommCallback.EndInvoke(ar);
            int errors = 0;
            int events = 0;

            if ((nativeEvents & NativeMethods.EV_ERR) != 0)
            {
                if (UnsafeNativeMethods.ClearCommError(_handleProtector.Handle, ref errors, ref comStat) == false)
                {
                    InternalResources.WinIOError();
                }

                if ((errors & NativeMethods.CE_RXOVER) != 0) events |= (int)SerialEvents.RxOver;
                if ((errors & NativeMethods.CE_OVERRUN) != 0) events |= (int)SerialEvents.Overrun;
                if ((errors & NativeMethods.CE_PARITY) != 0) events |= (int)SerialEvents.RxParity;
                if ((errors & NativeMethods.CE_FRAME) != 0) events |= (int)SerialEvents.Frame;
                if ((errors & NativeMethods.CE_TXFULL) != 0) events |= (int)SerialEvents.TxFull;
                if ((errors & NativeMethods.CE_BREAK) != 0) events |= (int)SerialEvents.Break;

            }
            if ((nativeEvents & NativeMethods.EV_RXCHAR) != 0) events |= (int)SerialEvents.ReceivedChars;
            if ((nativeEvents & NativeMethods.EV_RXFLAG) != 0) events |= (int)SerialEvents.EofReceived;
            if ((nativeEvents & NativeMethods.EV_CTS) != 0) events |= (int)SerialEvents.CtsChanged;
            if ((nativeEvents & NativeMethods.EV_DSR) != 0) events |= (int)SerialEvents.DsrChanged;
            if ((nativeEvents & NativeMethods.EV_RLSD) != 0) events |= (int)SerialEvents.CDChanged;
            if ((nativeEvents & NativeMethods.EV_RING) != 0) events |= (int)SerialEvents.Ring;
            if ((nativeEvents & NativeMethods.EV_BREAK) != 0) events |= (int)SerialEvents.Break;


            // errors are corresponding SerialEvents shifted right 8 bits 
            if ((events & errorEvents) != 0)
                ErrorEvent(this, new SerialEventArgs((SerialEvents)(events & errorEvents)));
            if ((events & pinChangedEvents) != 0)
                PinChangedEvent(this, new SerialEventArgs((SerialEvents)(events & pinChangedEvents)));
            if ((events & receivedEvents) != 0)
                ReceivedEvent(this, new SerialEventArgs((SerialEvents)(events & receivedEvents)));

            myWaitCommCallback.BeginInvoke(myAsyncCallback, state); // start it over again.    
        }


        // This is a the callback prompted when a thread completes any async I/O operation.  
        unsafe private static void AsyncFSCallback(uint errorCode, uint numBytes, NativeOverlapped* pOverlapped)
        {
            // Unpack overlapped
            Overlapped overlapped = Overlapped.Unpack(pOverlapped);

            // Extract async the result from overlapped structure
            AsyncSerialStream_AsyncResult asyncResult =
                (AsyncSerialStream_AsyncResult)overlapped.AsyncResult;
            asyncResult._numBytes = (int)numBytes;

            asyncResult._errorCode = (int)errorCode;

            // Pull waitHandle (here a ManualResetEvent) out of the asyncResult
            WaitHandle wh = asyncResult._waitHandle;
            if (wh != null)
            {
                bool r = NativeMethods.SetEvent(wh.Handle);
                if (!r) InternalResources.WinIOError();
            }
            // Set IsCompleted after WaitHandle signaled in case SetEvent fails.
            asyncResult._isComplete = true;

            // Call the user-provided callback.  Note that it can and often should
            // call EndRead or EndWrite.  There's no reason to use an async 
            // delegate here - we're already on a threadpool thread.  
            // Note the IAsyncResult's completedSynchronously property must return
            // false here, saying the user callback was called on another thread.

            asyncResult._completedSynchronously = false;
            AsyncCallback userCallback = asyncResult._userCallback;
            if (userCallback != null)
                userCallback(asyncResult);
        }


        // ----SECTION: internal classes --------*

        // This is an internal object implementing IAsyncResult with fields
        // for all of the relevant data necessary to complete the IO operation.
        // This is used by AsyncFSCallback and all async methods.
        unsafe internal class AsyncSerialStream_AsyncResult : IAsyncResult
        {
            // User code callback
            internal AsyncCallback _userCallback;

            internal Object _userStateObject;
            internal WaitHandle _waitHandle;
            internal GCHandle _bufferHandle;  // GCHandle to pin byte[].

            internal bool _isWrite;     // Whether this is a read or a write
            internal bool _isComplete;
            internal bool _completedSynchronously;  // Which thread called callback
            internal bool _bufferIsPinned;   // Whether our _bufferHandle is valid.
            internal int _EndXxxCalled;   // Whether we've called EndXxx already.
            internal int _numBytes;     // number of bytes read OR written
            internal int _numBufferedBytes;
            internal int _errorCode;
            internal NativeOverlapped* _overlapped;

            internal int _oldTimeout;    // added to preserve default timeouts on calls to specific timeouts
            /// <include file='doc\ModFileStream.uex' path='docs/doc[@for="ModFileStream.AsyncSerialStream_AsyncResult.AsyncState"]/*' />
            public virtual Object AsyncState
            {
                get { return _userStateObject; }
            }

            /// <include file='doc\ModFileStream.uex' path='docs/doc[@for="ModFileStream.AsyncSerialStream_AsyncResult.IsCompleted"]/*' />
            public bool IsCompleted
            {
                get { return _isComplete; }
                set { _isComplete = value; }
            }

            /// <include file='doc\ModFileStream.uex' path='docs/doc[@for="ModFileStream.AsyncSerialStream_AsyncResult.AsyncWaitHandle"]/*' />
            public WaitHandle AsyncWaitHandle
            {
                get { return _waitHandle; }
            }

            // Returns true iff the user callback was called by the thread that 
            // called BeginRead or BeginWrite.  If we use an async delegate or
            // threadpool thread internally, this will be false.  This is used
            // by code to determine whether a successive call to BeginRead needs 
            // to be done on their main thread or in their callback to avoid a
            // stack overflow on many reads or writes.
            public bool CompletedSynchronously
            {
                get { return _completedSynchronously; }
            }

            internal static AsyncSerialStream_AsyncResult CreateBufferedReadResult(int numBufferedBytes, AsyncCallback userCallback, Object userStateObject)
            {
                AsyncSerialStream_AsyncResult asyncResult = new AsyncSerialStream_AsyncResult();
                asyncResult._userCallback = userCallback;
                asyncResult._userStateObject = userStateObject;
                asyncResult._isComplete = true;
                asyncResult._isWrite = false;
                asyncResult._numBufferedBytes = numBufferedBytes;
                return asyncResult;
            }

            // An internal convenience method
            internal void CallUserCallback()
            {

                if (_userCallback != null)
                {
                    // Call user's callback on a threadpool thread.  
                    // we must set this to false, since this happens on another thread.
                    _completedSynchronously = false;
                    _userCallback.BeginInvoke(this, null, null);

                    // note that we needn't call EndInvoke, since the IAsyncResult finalizer does cleanup.
                }
            }

            // Methods to ensure that if we somehow return from Win32 ReadFile() or WriteFile() 
            // and fill the buffer at different times, the GC doesn't move said buffer.
            internal void PinBuffer(byte[] buffer)
            {
                _bufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                _bufferIsPinned = true;
            }

            internal void UnpinBuffer()
            {
                if (_bufferIsPinned)
                {
                    _bufferHandle.Free();
                    _bufferIsPinned = false;
                }
            }
        }

        // internal class used to ensure safety in using unmanaged handles.
        private sealed class __HandleProtector : HandleProtector
        {
            private bool _ownsHandle;

            internal __HandleProtector(IntPtr handle, bool ownsHandle)
                : base(handle)
            {
                _ownsHandle = ownsHandle;
            }

            protected internal override void FreeHandle(IntPtr handle)
            {
                if (_ownsHandle)
                    UnsafeNativeMethods.CloseHandle(handle);
            }
        }


    }
}
