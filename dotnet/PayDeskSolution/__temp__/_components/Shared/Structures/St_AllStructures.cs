using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace components.Shared.Structures
{
    [Serializable]
    public struct FILETIME
    {
        public UInt32 dwLowDateTime;
        public UInt32 dwHightDateTime;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SYSTEMTIME
    {
        public short wYear;
        public short wMonth;
        public short wDayOfWeek;
        public short wDay;
        public short wHour;
        public short wMinute;
        public short wSecond;
        public short wMilliseconds;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct DCB
    {
        //public Int32 DCBlength; // sizeof(DCB)
        //public Int32 BaudRate; // current baud rate
        //public Int32 PackedValues;
        //public Int32 fBinary; // binary mode, no EOF check 
        //public Int32 fParity; // enable parity checking 
        //public Int32 fOutxCtsFlow; // CTS output flow control 
        //public Int32 fOutxDsrFlow; // DSR output flow control 
        //public Int32 fDtrControl; // DTR flow control type 
        //public Int32 fDsrSensitivity; // DSR sensitivity 
        //public Int32 fTXContinueOnXoff; // XOFF continues Tx 
        //public Int32 fOutX; // XON/XOFF out flow control 
        //public Int32 fInX; // XON/XOFF in flow control 
        //public Int32 fErrorChar; // enable error replacement 
        //public Int32 fNull; // enable null stripping 
        //public Int32 fRtsControl; // RTS flow control 
        //public Int32 fAbortOnError; // abort reads/writes on error 
        //public Int32 fDummy2; // reserved
        //public Int16 wReserved; // not currently used
        //public Int16 XonLim; // transmit XON threshold
        //public Int16 XoffLim; // transmit XOFF threshold
        //public Byte ByteSize; // number of bits/byte, 4-8
        //public Byte Parity; // 0-4=no,odd,even,mark,space
        //public Byte StopBits; // 0,1,2 = 1, 1.5, 2
        //public Byte XonChar; // Tx and Rx XON character
        //public Byte XoffChar; // Tx and Rx XOFF character
        //public Byte ErrorChar; // error replacement character
        //public Byte EofChar; // end of input character
        //public Byte EvtChar; // received event character
        //public Int16 wReserved1; // reserved; do not use
        public Int32 DCBlength;
        public Int32 BaudRate;

        public Int32 Flags;
        //public Int32 fBinary;// = 1;
        //public Int32 fParity;// = 1;
        //public Int32 fOutxCtsFlow;// = 1;
        //public Int32 fOutxDsrFlow;// = 1;
        //public Int32 fDtrControl;// = 2;
        //public Int32 fDsrSensitivity;// = 1;
        //public Int32 fTXContinueOnXoff;// = 1;
        //public Int32 fOutX;// = 1;
        //public Int32 fInX;// = 1;
        //public Int32 fErrorChar;// = 1;
        //public Int32 fNull;// = 1;
        //public Int32 fRtsControl;// = 2;
        //public Int32 fAbortOnError;// = 1;
        //public Int32 fDummy2;// = 17;

        public Int16 wReserved;
        public Int16 XonLim;
        public Int16 XoffLim;
        public Byte ByteSize;
        public Byte Parity;
        public Byte StopBits;
        public char XonChar;
        public char XoffChar;
        public char ErrorChar;
        public char EofChar;
        public char EvtChar;
        public Int16 wReserved1;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct COMMTIMEOUTS
    {
        public UInt32 ReadIntervalTimeout;
        public UInt32 ReadTotalTimeoutMultiplier;
        public UInt32 ReadTotalTimeoutConstant;
        public UInt32 WriteTotalTimeoutMultiplier;
        public UInt32 WriteTotalTimeoutConstant;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct COMMSTAT
    {
        public UInt32 bitfield;
        public UInt32 cbInQue;
        public UInt32 cbOutQue;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct OVERLAPPED
    {
        public UIntPtr internalLow;
        public UIntPtr internalHigh;
        public UInt32 offset;
        public UInt32 offsetHigh;
        public IntPtr hEvent;
    }

    // GUI


    public enum CbtHookAction : int
    {
        HCBT_MOVESIZE = 0,
        HCBT_MINMAX = 1,
        HCBT_QS = 2,
        HCBT_CREATEWND = 3,
        HCBT_DESTROYWND = 4,
        HCBT_ACTIVATE = 5,
        HCBT_CLICKSKIPPED = 6,
        HCBT_KEYSKIPPED = 7,
        HCBT_SYSCOMMAND = 8,
        HCBT_SETFOCUS = 9
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CWPRETSTRUCT
    {
        public IntPtr lResult;
        public IntPtr lParam;
        public IntPtr wParam;
        public uint message;
        public IntPtr hwnd;
    }

    // E:GUI

}
