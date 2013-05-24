using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Xml;
using System.Runtime.InteropServices;
using System.Collections;
using components.Shared.Enums;
using components.Components.WinApi;
using components.Shared.Structures;

namespace components.Components.SerialPort
{
    public class Com_SerialPort : IDisposable
    {
        // Config file name
        private string CFG_FILE_NAME = "Port.xml";

        // Params
        private int portIndex;
        private BaudRate baudRate;
        private Parity parity;
        private ByteSize dataBits;
        private StopBits stopBits;
        private IntPtr handle;
        private bool isOpen;
        private UInt32 ReadIntervalTimeout;
        private UInt32 ReadTotalTimeoutMultiplier;
        private UInt32 ReadTotalTimeoutConstant;
        private UInt32 WriteTotalTimeoutMultiplier;
        private UInt32 WriteTotalTimeoutConstant;
        private object tag = "-";
        //
        private Hashtable _pcfg;

        /* CONSTRUCTOR */

        public Com_SerialPort()
        {
            portIndex = 1;
            baudRate = BaudRate.Baud_9600;
            parity = Parity.None;
            dataBits = ByteSize.Eight;
            stopBits = StopBits.One;
            handle = IntPtr.Zero;
            ReadIntervalTimeout = Com_WinApi.MAXDWORD;

            _pcfg = new Hashtable();

            _pcfg.Add("PORT", this.portIndex);
            _pcfg.Add("RATE", (int)this.baudRate);
            _pcfg.Add("PARITY", (byte)this.parity);
            _pcfg.Add("DBITS", (byte)this.dataBits);
            _pcfg.Add("SBITS", (int)this.stopBits);
            _pcfg.Add("RT", this.ReadIntervalTimeout);
            _pcfg.Add("RM", this.ReadTotalTimeoutMultiplier);
            _pcfg.Add("RC", this.ReadTotalTimeoutConstant);
            _pcfg.Add("WM", this.WriteTotalTimeoutMultiplier);
            _pcfg.Add("WC", this.WriteTotalTimeoutConstant);
        }

        /* DESTRUCTOR */

        ~Com_SerialPort()
        {
            Dispose(false);
        }

        /* PUBLIC MEMBERS */

        private static Dictionary<string, Com_SerialPort> _portCollection = new Dictionary<string, Com_SerialPort>();
        public static Com_SerialPort GetPort(string instanceName)
        {
            if(_portCollection.ContainsKey(instanceName))
                return _portCollection[instanceName];

            _portCollection[instanceName] = new Com_SerialPort();

            return _portCollection[instanceName];
        }

        public static Com_SerialPort GetAndConfigurePort(string instanceName, Hashtable configuration)
        {
            Com_SerialPort port = GetPort(instanceName);
            port.PortConfig = configuration;
            port.Tag = instanceName;
            return port;
        }

        public static void ClosePort(string instanceName)
        {
            if (_portCollection.ContainsKey(instanceName))
            {
                try
                {
                    _portCollection[instanceName].Close();
                }
                catch { }
                _portCollection.Remove(instanceName);
            }
        }

        public static void CloseAllPorts(bool clear)
        {
            foreach (KeyValuePair<string, Com_SerialPort> port in _portCollection)
            {
                port.Value.Close();
                if (clear)
                    _portCollection.Remove(port.Key);
            }
        }

        

        /// <summary>
        /// Perform disposing element
        /// </summary>
        /// <param name="disposing"></param>
        public void Dispose(bool disposing)
        {
            GC.SuppressFinalize(this);
            Dispose();
        }
        /// <summary>
        /// Perform saving СОМ-port settings
        /// </summary>
        public void SavePortConfig()
        {
            XmlTextWriter xtw = null;
            try
            {
                xtw = new XmlTextWriter(CFG_FILE_NAME, Encoding.Default);
                xtw.Formatting = Formatting.Indented;
                xtw.Indentation = 4;
                xtw.WriteStartDocument();
                xtw.WriteStartElement("CommunicationPort");
                this.SaveMembers(xtw);
                xtw.WriteEndElement();
            }
            finally
            {
                if (xtw != null)
                {
                    xtw.Flush();
                    xtw.Close();
                }
            }
        }
        /// <summary>
        /// Завантаження і застосування параметрів СОМ-порту
        /// </summary>
        public void LoadPortConfig()
        {
            FileInfo fi = new FileInfo(CFG_FILE_NAME);
            if (!fi.Exists)
                this.SavePortConfig();

            if (isOpen)
            {
                System.Windows.Forms.MessageBox.Show("Неможливо завантажити конфігурацію порту\r\nколи порт є відкритий",
                    "COM порт", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                return;
            }
            
            XmlTextReader xtr = null;
            try
            {
                xtr = new XmlTextReader(CFG_FILE_NAME);
                xtr.WhitespaceHandling = WhitespaceHandling.None;
                while (xtr.Read())
                {
                    this.LoadMembers(xtr);
                }
            }
            finally
            {
                if (xtr != null) xtr.Close();
            }
        }
        public void LoadPortConfig(string path)
        {
            try
            {
                CFG_FILE_NAME = path;
                LoadPortConfig();
            }
            catch { }
        }
        public void LoadPortConfig(Hashtable config)
        {

        }
        public bool Available(int portIndex)
        {
            string portName = "COM" + portIndex.ToString() + ":";
            IntPtr tmphPort = Com_WinApi.CreateFileA(portName,
                (UInt32)dwDesiredAccess.GENERIC_READ, 0, IntPtr.Zero,
                (UInt32)dwCreationDisposion.OPEN_EXISTING, 0x0, IntPtr.Zero);
            if (tmphPort == (IntPtr)Com_WinApi.INVALID_HANDLE_VALUE)
                return false;
            else
                Com_WinApi.CloseHandle(tmphPort);

            return true;
        }
        /// <summary>
        /// Відкриття СОМ-порту
        /// </summary>
        /// <param name="portIndex">Номер порту</param>
        /// <returns>Якщо true то СОМ-порт відкритий успішно</returns>
        public bool Open(int portIndex)
        {
            string portName = "COM" + portIndex.ToString() + ":";
            handle = Com_WinApi.CreateFileA(portName, (UInt32)(dwDesiredAccess.GENERIC_READ | dwDesiredAccess.GENERIC_WRITE),
                0, IntPtr.Zero, (UInt32)dwCreationDisposion.OPEN_EXISTING, (UInt32)dwFileFlags.FILE_FLAG_OVERLAPPED, IntPtr.Zero);

            if (handle == (IntPtr)Com_WinApi.INVALID_HANDLE_VALUE)
                return false;

            DCB _dcb = new DCB();
            Com_WinApi.GetCommState(handle, ref _dcb);
            //Setup dcb
            _dcb.BaudRate = (int)this.baudRate;
            //Clear RtsControl
            _dcb.Flags &= 0x7FFFCFFF;
            //Clear DsrControl
            _dcb.Flags &= 0x7FFFFFCF;
            //Set fBinary to 1
            _dcb.Flags |= 0x00000001;
            //Set fParity to 1
            _dcb.Flags |= 0x00000002;
            _dcb.ByteSize = (byte)this.dataBits;
            _dcb.Parity = (byte)this.parity;
            _dcb.StopBits = (byte)this.stopBits;

            //Handflow
            _dcb.XonLim = 2048;
            _dcb.XoffLim = 512;
            _dcb.XonChar = (char)0x11;
            _dcb.XoffChar = (char)0x13;


            _dcb.XonLim = 0;
            _dcb.XoffLim = 16384;

            if (!Com_WinApi.SetCommState(handle, ref _dcb))
            {
                System.Windows.Forms.MessageBox.Show("Задані неправильні параметри порту", "COM Порт", 
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
            COMMTIMEOUTS _tOut = new COMMTIMEOUTS();
            Com_WinApi.GetCommTimeouts(handle, out _tOut);

            //Setup timeouts
            _tOut.ReadIntervalTimeout = ReadIntervalTimeout;
            _tOut.ReadTotalTimeoutConstant = ReadTotalTimeoutConstant;
            _tOut.ReadTotalTimeoutMultiplier = ReadTotalTimeoutMultiplier;
            _tOut.WriteTotalTimeoutConstant = WriteTotalTimeoutConstant;
            _tOut.WriteTotalTimeoutMultiplier = WriteTotalTimeoutMultiplier;

            if (!Com_WinApi.SetCommTimeouts(handle, ref _tOut))
            {
                System.Windows.Forms.MessageBox.Show("Задані неправильні таймаути порту", "COM Порт",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }

            Com_WinApi.PurgeComm(handle, Com_WinApi.PURGE_TXCLEAR);
            Com_WinApi.PurgeComm(handle, Com_WinApi.PURGE_RXCLEAR);

            isOpen = true;
            return true;
        }
        /// <summary>
        /// Відкриття СОМ-порту
        /// </summary>
        /// <param name="portName">Назва порту</param>
        /// <returns>Якщо true то СОМ-порт відкритий успішно</returns>
        public bool Open(string portName)
        {
            if (!isOpen)
            {
                string pIdx = string.Empty;
                for (byte i = 0; i < portName.Length; i++)
                    if (Char.IsDigit(portName[i]))
                        pIdx += portName[i];

                return Open(int.Parse(pIdx));
            }
            else
                return ReOpen();
        }
        /// <summary>
        /// Відкриття порту за раніше вказани індекстом СОМ-порту
        /// </summary>
        /// <returns>Якщо true то СОМ-порт відкритий успішно</returns>
        public bool Open()
        {
            if (!isOpen)
                return Open(portIndex);
            else
                return ReOpen();
        }
        /// <summary>
        /// Перевідкриття СОМ-порту
        /// </summary>
        /// <returns>Якщо true то СОМ-порт перевідкриття СОИ-порту відбулося успішно</returns>
        public bool ReOpen()
        {
            bool fRez = false;

            if (isOpen)
            {
                PortClear();
                fRez = Com_WinApi.CloseHandle(handle);
            }

            isOpen = false;
            if (fRez)
                isOpen = Open(portIndex);

            return isOpen;
        }
        /// <summary>
        /// Читання даних з СОМ-порту
        /// </summary>
        /// <param name="buffer">Буфер в який буде відбуватися читання інформації</param>
        /// <param name="count">Кількість прочитаних байтів</param>
        /// <returns>Якщо true то читання з СОМ-порту відбулося успішно</returns>
        public bool Read(ref byte[] buffer, out uint count)
        {
            count = 0;
            OVERLAPPED OLRead = new OVERLAPPED();
            bool fOK = false;
            OLRead.hEvent = Com_WinApi.CreateEventA(IntPtr.Zero, true, false, null);

            if (OLRead.hEvent != null)
            {
                fOK = Com_WinApi.ReadFile(handle, buffer, (uint)buffer.Length, out count, ref OLRead);
                if (!fOK)
                {
                    if (Marshal.GetLastWin32Error() == Com_WinApi.ERROR_IO_PENDING)
                    {
                        fOK = (Com_WinApi.WaitForSingleObject(OLRead.hEvent, 5000) == Com_WinApi.WAIT_OBJECT_0) &&
                            Com_WinApi.GetOverlappedResult(handle, ref OLRead, ref count, false);
                    }
                }
                Com_WinApi.CloseHandle(OLRead.hEvent);
                if (!fOK)
                    PortClear();
            }
            else
                Marshal.GetLastWin32Error();

            return fOK;
        }
        /// <summary>
        /// Відправка інформації на СОМ-порт
        /// </summary>
        /// <param name="SendArr">Інформація</param>
        /// <returns>Якщо true то відправка інформації на СОМ-порт відбулася успішно</returns>
        public bool Write(byte[] SendArr)
        {
            if (handle == (IntPtr)Com_WinApi.INVALID_HANDLE_VALUE) return false;
            if (SendArr == null) return false;
            if (SendArr.Length == 0) return true;

            uint BytesWritten = 0;
            OVERLAPPED OverlappedWrite = new OVERLAPPED();
            bool fOK = false;
            int lastError = 0;
            OverlappedWrite.hEvent = Com_WinApi.CreateEventA(IntPtr.Zero, true, false, null);

            components.Components.WinApi.Com_WinApi.OutputDebugString("COM-PORT WRITE: Step 1");
            if (OverlappedWrite.hEvent != null)
            {
                components.Components.WinApi.Com_WinApi.OutputDebugString("COM-PORT WRITE: Step 2");
                fOK = Com_WinApi.WriteFile(handle, SendArr, (uint)SendArr.Length, out BytesWritten, ref OverlappedWrite);
                components.Components.WinApi.Com_WinApi.OutputDebugString("COM-PORT WRITE: Step 3: " + fOK.ToString());
                components.Components.WinApi.Com_WinApi.OutputDebugString("" + Marshal.GetLastWin32Error());

                if (!fOK && Marshal.GetLastWin32Error() == Com_WinApi.ERROR_IO_PENDING)
                {
                    components.Components.WinApi.Com_WinApi.OutputDebugString("COM-PORT WRITE: Step 4: " + fOK.ToString());
                    components.Components.WinApi.Com_WinApi.OutputDebugString("" + Marshal.GetLastWin32Error());
                    fOK = (Com_WinApi.WaitForSingleObject(OverlappedWrite.hEvent, 1000) == Com_WinApi.WAIT_OBJECT_0) &&
                        Com_WinApi.GetOverlappedResult(handle, ref OverlappedWrite, ref BytesWritten, false);
                    components.Components.WinApi.Com_WinApi.OutputDebugString("COM-PORT WRITE: Step 5: " + fOK.ToString());
                    components.Components.WinApi.Com_WinApi.OutputDebugString("" + Marshal.GetLastWin32Error());
                }

                if (fOK)
                {
                    fOK = (BytesWritten == SendArr.Length);
                    components.Components.WinApi.Com_WinApi.OutputDebugString("COM-PORT WRITE: Step 6: " + fOK.ToString());
                    components.Components.WinApi.Com_WinApi.OutputDebugString("" + Marshal.GetLastWin32Error());
                }

                if (!fOK)
                {
                    PortClear();
                    components.Components.WinApi.Com_WinApi.OutputDebugString("COM-PORT WRITE: Step 7: " + fOK.ToString());
                    components.Components.WinApi.Com_WinApi.OutputDebugString("" + Marshal.GetLastWin32Error());
                }
                Com_WinApi.CloseHandle(OverlappedWrite.hEvent);
                components.Components.WinApi.Com_WinApi.OutputDebugString("COM-PORT WRITE: Step 8: " + fOK.ToString());
                components.Components.WinApi.Com_WinApi.OutputDebugString("" + Marshal.GetLastWin32Error());
            }
            else
                lastError = Marshal.GetLastWin32Error();

            if (lastError != 0)
                components.Components.WinApi.Com_WinApi.OutputDebugString("Marshal.GetLastWin32Error() returned " + lastError);

            components.Components.WinApi.Com_WinApi.OutputDebugString("" + Marshal.GetLastWin32Error());
            return fOK;
        }
        public bool PortClear()
        {
            Marshal.GetLastWin32Error();
            GC.SuppressFinalize(this);

            if (!Com_WinApi.PurgeComm(handle, (uint)(Com_WinApi.PURGE_TXCLEAR | Com_WinApi.PURGE_RXCLEAR)))
                return false;

            Marshal.GetLastWin32Error();

            return true;
        }
        public bool PortAbort()
        {
            Marshal.GetLastWin32Error();
            GC.SuppressFinalize(this);

            if (!Com_WinApi.PurgeComm(handle, (uint)(Com_WinApi.PURGE_TXABORT | Com_WinApi.PURGE_RXABORT)))
                return false;

            Marshal.GetLastWin32Error();

            return true;
        }
        /// <summary>
        /// Закриття СОМ-порту
        /// </summary>
        /// <returns>Якщо true то СОМ-порт закритий успішно</returns>
        public bool Close()
        {
            isOpen = !Com_WinApi.CloseHandle(handle);

            if (!isOpen)
                handle = IntPtr.Zero;
            if (handle == IntPtr.Zero)
                isOpen = false;

            return !isOpen;
        }

        /* PROPERTIES */

        public int PortIndex { get { return portIndex; } set { portIndex = value; } }
        public string PortName
        {
            get { return "COM" + portIndex; }
            set
            {
                string pIdx = string.Empty;
                for (byte i = 0; i < value.Length; i++)
                    if (Char.IsDigit(value[i]))
                        pIdx += value[i];

                portIndex = int.Parse(pIdx);
            }
        }
        public BaudRate BaudRate { get { return baudRate; } set { baudRate = value; } }
        public Parity Parity { get { return parity; } set { parity = value; } }
        public ByteSize ByteSize { get { return dataBits; } set { dataBits = value; } }
        public StopBits StopBits { get { return stopBits; } set { stopBits = value; } }
        public UInt32 RIntervalTimeout { get { return ReadIntervalTimeout; } set { ReadIntervalTimeout = value; } }
        public UInt32 RTotalTimeoutMultiplier { get { return ReadTotalTimeoutMultiplier; } set { ReadTotalTimeoutMultiplier = value; } }
        public UInt32 RTotalTimeoutConstant { get { return ReadTotalTimeoutConstant; } set { ReadTotalTimeoutConstant = value; } }
        public UInt32 WTotalTimeoutMultiplier { get { return WriteTotalTimeoutMultiplier; } set { WriteTotalTimeoutMultiplier = value; } }
        public UInt32 WTotalTimeoutConstant { get { return WriteTotalTimeoutConstant; } set { WriteTotalTimeoutConstant = value; } }
        public bool IsOpen { get { return isOpen; } }
        public object Tag { get { return tag; } set { tag = value; } }
        public Hashtable PortConfig
        {
            get
            {
                _pcfg["PORT"] = "COM" + this.portIndex;
                _pcfg["RATE"] = (int)this.baudRate;
                _pcfg["PARITY"] = (byte)this.parity;
                _pcfg["DBITS"] = (byte)this.dataBits;
                _pcfg["SBITS"] = (byte)this.stopBits;
                _pcfg["RT"] = this.ReadIntervalTimeout;
                _pcfg["RM"] = this.ReadTotalTimeoutMultiplier;
                _pcfg["RC"] = this.ReadTotalTimeoutConstant;
                _pcfg["WM"] = this.WriteTotalTimeoutMultiplier;
                _pcfg["WC"] = this.WriteTotalTimeoutConstant;

                return _pcfg;
            }
            set
            {
                _pcfg = value;

                try
                {
                    this.portIndex = int.Parse(_pcfg["PORT"].ToString().Substring(3));
                    this.baudRate = (BaudRate)Enum.Parse(typeof(BaudRate), _pcfg["RATE"].ToString());
                    this.parity = (Parity)Enum.Parse(typeof(Parity), _pcfg["PARITY"].ToString());
                    this.dataBits = (ByteSize)Enum.Parse(typeof(ByteSize), _pcfg["DBITS"].ToString());
                    this.stopBits = (StopBits)Enum.Parse(typeof(StopBits), _pcfg["SBITS"].ToString());
                    if (_pcfg["RT"].ToString() == "-1")
                        _pcfg["RT"] = UInt32.MaxValue;
                    this.ReadIntervalTimeout = UInt32.Parse(_pcfg["RT"].ToString());
                    if (_pcfg["RM"].ToString() == "-1")
                        _pcfg["RM"] = UInt32.MaxValue;
                    this.ReadTotalTimeoutMultiplier = UInt32.Parse(_pcfg["RM"].ToString());
                    if (_pcfg["RC"].ToString() == "-1")
                        _pcfg["RC"] = UInt32.MaxValue;
                    this.ReadTotalTimeoutConstant = UInt32.Parse(_pcfg["RC"].ToString());
                    if (_pcfg["WM"].ToString() == "-1")
                        _pcfg["WM"] = UInt32.MaxValue;
                    this.WriteTotalTimeoutMultiplier = UInt32.Parse(_pcfg["WM"].ToString());
                    if (_pcfg["WC"].ToString() == "-1")
                        _pcfg["WC"] = UInt32.MaxValue;
                    this.WriteTotalTimeoutConstant = UInt32.Parse(_pcfg["WC"].ToString());
                }
                catch { }
            }
        }

        /* PRIVATE MEMBERS  */

        private void LoadMembers(XmlTextReader xtr)
        {
            if (xtr.NodeType == XmlNodeType.Element)
            {
                switch (xtr.Name)
                {
                    case "PortIndex":
                        xtr.Read();
                        if (xtr.NodeType == XmlNodeType.Text)
                            this.portIndex = int.Parse(xtr.Value);
                        break;
                    case "BaudRate":
                        xtr.Read();
                        if (xtr.NodeType == XmlNodeType.Text)
                            this.baudRate = (BaudRate)int.Parse(xtr.Value);
                        break;
                    case "Parity":
                        xtr.Read();
                        if (xtr.NodeType == XmlNodeType.Text)
                            this.parity = (Parity)byte.Parse(xtr.Value);
                        break;
                    case "DataBits":
                        xtr.Read();
                        if (xtr.NodeType == XmlNodeType.Text)
                            this.dataBits = (ByteSize)byte.Parse(xtr.Value);
                        break;
                    case "StopBits":
                        xtr.Read();
                        if (xtr.NodeType == XmlNodeType.Text)
                            this.stopBits = (StopBits)byte.Parse(xtr.Value);
                        break;
                    case "ReadIntervalTimeout":
                        xtr.Read();
                        if (xtr.NodeType == XmlNodeType.Text)
                            this.ReadIntervalTimeout = UInt32.Parse(xtr.Value);
                        break;
                    case "ReadTotalTimeoutMultiplier":
                        xtr.Read();
                        if (xtr.NodeType == XmlNodeType.Text)
                            this.ReadTotalTimeoutMultiplier = UInt32.Parse(xtr.Value);
                        break;
                    case "ReadTotalTimeoutConstant":
                        xtr.Read();
                        if (xtr.NodeType == XmlNodeType.Text)
                            this.ReadTotalTimeoutConstant = UInt32.Parse(xtr.Value);
                        break;
                    case "WriteTotalTimeoutMultiplier":
                        xtr.Read();
                        if (xtr.NodeType == XmlNodeType.Text)
                            this.WriteTotalTimeoutMultiplier = UInt32.Parse(xtr.Value);
                        break;
                    case "WriteTotalTimeoutConstant":
                        xtr.Read();
                        if (xtr.NodeType == XmlNodeType.Text)
                            this.WriteTotalTimeoutConstant = UInt32.Parse(xtr.Value);
                        break;
                    case "Tag":
                        xtr.Read();
                        if (xtr.NodeType == XmlNodeType.Text)
                            this.tag = xtr.Value;
                        break;
                }
            }
        }
        private void SaveMembers(XmlTextWriter xtw)
        {
            xtw.WriteStartElement("ComPortSettings");
            xtw.WriteElementString("PortIndex", this.portIndex.ToString());
            xtw.WriteElementString("BaudRate", ((int)this.baudRate).ToString());
            xtw.WriteElementString("Parity", ((byte)this.parity).ToString());
            xtw.WriteElementString("DataBits", ((byte)this.dataBits).ToString());
            xtw.WriteElementString("StopBits", ((int)this.stopBits).ToString());
            xtw.WriteElementString("ReadIntervalTimeout", (this.ReadIntervalTimeout).ToString());
            xtw.WriteElementString("ReadTotalTimeoutMultiplier", (this.ReadTotalTimeoutMultiplier).ToString());
            xtw.WriteElementString("ReadTotalTimeoutConstant", (this.ReadTotalTimeoutConstant).ToString());
            xtw.WriteElementString("WriteTotalTimeoutMultiplier", (this.WriteTotalTimeoutMultiplier).ToString());
            xtw.WriteElementString("WriteTotalTimeoutConstant", (this.WriteTotalTimeoutConstant).ToString());
            xtw.WriteElementString("Tag", ((object)this.tag).ToString());
        }

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }

    //public static class WinAPI
    //{
    //    //Constants for errors:
    //    public static UInt32 ERROR_FILE_NOT_FOUND = 2;
    //    public static UInt32 ERROR_INVALID_NAME = 123;
    //    public static UInt32 ERROR_ACCESS_DENIED = 5;
    //    public static UInt32 ERROR_INVALID_HANDLE = 6;
    //    public static UInt32 ERROR_IO_PENDING = 997;
    //    public static UInt32 ERROR_HANDLE_EOF = 38;

    //    public static UInt32 WAIT_OBJECT_0 = 0x00000000;
    //    public static UInt32 WAIT_ABANDONED = 0x00000080;
    //    public static UInt32 WAIT_ABANDONED_0 = 0x00000080;
    //    //Constants for return value:
    //    public static Int32 INVALID_HANDLE_VALUE = -1;

    //    //Constants for dwFlagsAndAttributes:
    //    public static UInt32 FILE_FLAG_OVERLAPPED = 0x40000000;

    //    //Constants for dwCreationDisposition:
    //    public static UInt32 OPEN_EXISTING = 3;

    //    //Constants for dwDesiredAccess:
    //    public static UInt32 GENERIC_READ = 0x80000000;
    //    public static UInt32 GENERIC_WRITE = 0x40000000;

    //    // Constants for dwEvtMask:
    //    public static UInt32 EV_RXCHAR = 0x0001;
    //    public static UInt32 EV_RXFLAG = 0x0002;
    //    public static UInt32 EV_TXEMPTY = 0x0004;
    //    public static Int32 EV_CTS = 0x0008;
    //    public static UInt32 EV_DSR = 0x0010;
    //    public static UInt32 EV_RLSD = 0x0020;
    //    public static UInt32 EV_BREAK = 0x0040;
    //    public static UInt32 EV_ERR = 0x0080;
    //    public static UInt32 EV_RING = 0x0100;
    //    public static UInt32 EV_PERR = 0x0200;
    //    public static UInt32 EV_RX80FULL = 0x0400;
    //    public static UInt32 EV_EVENT1 = 0x0800;
    //    public static UInt32 EV_EVENT2 = 0x1000;
    //    // Added to enable use of "return immediately" timeout.
    //    public static UInt32 MAXDWORD = 0xffffffff;
    //    //Purge
    //    public static UInt16 PURGE_TXABORT = 0x0001; // Kill the pending/current writes to the comm port.
    //    public static UInt16 PURGE_RXABORT = 0x0002; // Kill the pending/current reads to the comm port.
    //    public static UInt16 PURGE_TXCLEAR = 0x0004; // Kill the transmit queue if there.
    //    public static UInt16 PURGE_RXCLEAR = 0x0008; // Kill the typeahead buffer if there.

    //    const string KernelDll = "kernel32.dll";
    //    [DllImport(KernelDll, SetLastError = true)]
    //    internal static extern IntPtr CreateFile(String lpFileName, UInt32 dwDesiredAccess, UInt32 dwShareMode, IntPtr lpSecurityAttributes, UInt32 dwCreationDisposition, UInt32 dwFlagsAndAttributes, IntPtr hTemplateFile);
    //    [DllImport(KernelDll, SetLastError = true)]
    //    internal static extern Boolean WriteFile(IntPtr fFile, Byte[] lpBuffer, UInt32 nNumberOfBytesToWrite, out UInt32 lpNumberOfBytesWritten, ref OVERLAPPED lpOverlapped);
    //    [DllImport(KernelDll, SetLastError = true)]
    //    internal static extern Boolean ReadFile(IntPtr hFile, Byte[] lpBuffer, UInt32 nNumberOfBytesToRead, out UInt32 nNumberOfBytesRead, ref OVERLAPPED lpOverlapped);
    //    [DllImport(KernelDll)]
    //    internal static extern IntPtr CreateEvent(IntPtr lpEventAttributes, Boolean bManualReset, Boolean bInitialState, String lpName);
    //    [DllImport(KernelDll, SetLastError = true)]
    //    internal static extern bool WaitCommEvent(IntPtr hFile, ref int lpEvtMask, ref OVERLAPPED lpOverlapped);

    //    [DllImport(KernelDll, SetLastError = true)]
    //    internal static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);
        
    //    [DllImport(KernelDll, SetLastError = true)]
    //    internal static extern bool ClearCommError(IntPtr hFile, ref int lpErrors, ref COMMSTAT lpCommStat);
        
    //    [DllImport(KernelDll, SetLastError = true)]
    //    internal static extern Boolean GetOverlappedResult(IntPtr hFile, ref OVERLAPPED lpOverlapped, ref UInt32 nNumberOfBytesTransferred, Boolean bWait);
    //    [DllImport(KernelDll)]
    //    internal static extern Boolean CloseHandle(IntPtr hObject);
    //    [DllImport(KernelDll)]
    //    internal static extern Boolean CancelIo(IntPtr hFile);
    //    [DllImport(KernelDll)]
    //    internal static extern Boolean SetCommMask(IntPtr hFile, UInt32 dwEvtMask);
    //    [DllImport(KernelDll)]
    //    internal static extern Boolean GetCommState(IntPtr hFile, ref DCB lpDCB);
    //    [DllImport(KernelDll)]
    //    internal static extern Boolean GetCommTimeouts(IntPtr hFile, out COMMTIMEOUTS lpCommTimeouts);
    //    [DllImport(KernelDll)]
    //    internal static extern Boolean BuildCommDCBAndTimeouts(String lpDef, ref DCB lpDCB, ref COMMTIMEOUTS lpCommTimeouts);
    //    [DllImport(KernelDll)]
    //    internal static extern Boolean SetCommState(IntPtr hFile, [In] ref DCB lpDCB);
    //    [DllImport(KernelDll)]
    //    internal static extern Boolean SetCommTimeouts(IntPtr hFile, [In] ref COMMTIMEOUTS lpCommTimeouts);
    //    [DllImport(KernelDll)]
    //    internal static extern Boolean SetupComm(IntPtr hFile, UInt32 dwInQueue, UInt32 dwOutQueue);
    //    [DllImport(KernelDll, SetLastError = true)]
    //    internal static extern bool PurgeComm(IntPtr hFile, uint dwFlags);
    //    [DllImport(KernelDll)]
    //    internal static extern void OutputDebugString([MarshalAs(UnmanagedType.LPTStr)]string lpOutputString);
    //}

    //[StructLayout(LayoutKind.Sequential)]
    //internal struct DCB
    //{
    //    //internal Int32 DCBlength; // sizeof(DCB)
    //    //internal Int32 BaudRate; // current baud rate
    //    //internal Int32 PackedValues;
    //    //internal Int32 fBinary; // binary mode, no EOF check 
    //    //internal Int32 fParity; // enable parity checking 
    //    //internal Int32 fOutxCtsFlow; // CTS output flow control 
    //    //internal Int32 fOutxDsrFlow; // DSR output flow control 
    //    //internal Int32 fDtrControl; // DTR flow control type 
    //    //internal Int32 fDsrSensitivity; // DSR sensitivity 
    //    //internal Int32 fTXContinueOnXoff; // XOFF continues Tx 
    //    //internal Int32 fOutX; // XON/XOFF out flow control 
    //    //internal Int32 fInX; // XON/XOFF in flow control 
    //    //internal Int32 fErrorChar; // enable error replacement 
    //    //internal Int32 fNull; // enable null stripping 
    //    //internal Int32 fRtsControl; // RTS flow control 
    //    //internal Int32 fAbortOnError; // abort reads/writes on error 
    //    //internal Int32 fDummy2; // reserved
    //    //internal Int16 wReserved; // not currently used
    //    //internal Int16 XonLim; // transmit XON threshold
    //    //internal Int16 XoffLim; // transmit XOFF threshold
    //    //internal Byte ByteSize; // number of bits/byte, 4-8
    //    //internal Byte Parity; // 0-4=no,odd,even,mark,space
    //    //internal Byte StopBits; // 0,1,2 = 1, 1.5, 2
    //    //internal Byte XonChar; // Tx and Rx XON character
    //    //internal Byte XoffChar; // Tx and Rx XOFF character
    //    //internal Byte ErrorChar; // error replacement character
    //    //internal Byte EofChar; // end of input character
    //    //internal Byte EvtChar; // received event character
    //    //internal Int16 wReserved1; // reserved; do not use
    //    internal Int32 DCBlength;
    //    internal Int32 BaudRate;

    //    internal Int32 Flags;
    //    //internal Int32 fBinary;// = 1;
    //    //internal Int32 fParity;// = 1;
    //    //internal Int32 fOutxCtsFlow;// = 1;
    //    //internal Int32 fOutxDsrFlow;// = 1;
    //    //internal Int32 fDtrControl;// = 2;
    //    //internal Int32 fDsrSensitivity;// = 1;
    //    //internal Int32 fTXContinueOnXoff;// = 1;
    //    //internal Int32 fOutX;// = 1;
    //    //internal Int32 fInX;// = 1;
    //    //internal Int32 fErrorChar;// = 1;
    //    //internal Int32 fNull;// = 1;
    //    //internal Int32 fRtsControl;// = 2;
    //    //internal Int32 fAbortOnError;// = 1;
    //    //internal Int32 fDummy2;// = 17;

    //    internal Int16 wReserved;
    //    internal Int16 XonLim;
    //    internal Int16 XoffLim;
    //    internal Byte ByteSize;
    //    internal Byte Parity;
    //    internal Byte StopBits;
    //    internal char XonChar;
    //    internal char XoffChar;
    //    internal char ErrorChar;
    //    internal char EofChar;
    //    internal char EvtChar;
    //    internal Int16 wReserved1;
    //}
    //[StructLayout(LayoutKind.Sequential)]
    //internal struct COMMTIMEOUTS
    //{
    //    internal UInt32 ReadIntervalTimeout;
    //    internal UInt32 ReadTotalTimeoutMultiplier;
    //    internal UInt32 ReadTotalTimeoutConstant;
    //    internal UInt32 WriteTotalTimeoutMultiplier;
    //    internal UInt32 WriteTotalTimeoutConstant;
    //}
    //[StructLayout(LayoutKind.Sequential)]
    //internal struct COMMSTAT
    //{
    //    internal UInt32 bitfield;
    //    internal UInt32 cbInQue;
    //    internal UInt32 cbOutQue;
    //}
    //[StructLayout(LayoutKind.Sequential)]
    //internal struct OVERLAPPED
    //{
    //    internal UIntPtr internalLow;
    //    internal UIntPtr internalHigh;
    //    internal UInt32 offset;
    //    internal UInt32 offsetHigh;
    //    internal IntPtr hEvent;
    //}
    ///// 
    ///// Число информационных бит в байте.
    ///// 
    //public enum ByteSize : byte
    //{
    //    Five = 5,
    //    Six = 6,
    //    Seven = 7,
    //    Eight = 8
    //}
    ///// 
    ///// Скорости передачи данных.
    /////
    //public enum BaudRate : int
    //{
    //    Baud_110 = 110,
    //    Baud_300 = 300,
    //    Baud_600 = 600,
    //    Baud_1200 = 1200,
    //    Baud_2400 = 2400,
    //    Baud_4800 = 4800,
    //    Baud_9600 = 9600,
    //    Baud_14400 = 14400,
    //    Baud_19200 = 19200,
    //    Baud_38400 = 38400,
    //    Baud_56000 = 56000,
    //    Baud_57600 = 57600,
    //    Baud_115200 = 115200,
    //    Baud_128000 = 128000,
    //    Baud_256000 = 256000,
    //}
    ///// 
    ///// Установки четности.
    ///// 
    //public enum Parity : byte
    //{
    //    /// 
    //    /// Без бита четности.
    //    /// 
    //    None = 0,
    //    /// 
    //    /// Дополнение до нечетности.
    //    /// 
    //    Odd = 1,
    //    /// 
    //    /// Дополнение до четности.
    //    /// 
    //    Even = 2,
    //    /// 
    //    /// Бит четности всегда 1.
    //    /// 
    //    Mark = 3,
    //    /// 
    //    /// Бит четности всегда 0.
    //    /// 
    //    Space = 4
    //}
    ///// 
    ///// Количество стоповых бит
    ///// 
    //public enum StopBits : byte
    //{
    //    /// 
    //    /// Один стоповый бит
    //    /// 
    //    One = 0,
    //    /// 
    //    /// Полтора стоповых бита
    //    /// 
    //    OnePointFive = 1,
    //    /// 
    //    /// Два стоповых бита
    //    /// 
    //    Two = 2
    //}
}
