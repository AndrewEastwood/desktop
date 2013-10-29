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
        // test
        private System.IO.Ports.SerialPort _port;

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
            // util profile2
            _port.PortName = config["Port"].ToString();
            _port.BaudRate = int.Parse(config["BaudRate"].ToString());
            _port.Parity = (System.IO.Ports.Parity)int.Parse(config["Parity"].ToString());
            _port.DataBits = int.Parse(config["DataBits"].ToString());
            _port.StopBits = (System.IO.Ports.StopBits)int.Parse(config["StopBits"].ToString());
            _port.ReadTimeout = int.Parse(config["ReadTimeout"].ToString());
            _port.WriteTimeout = int.Parse(config["WriteTimeout"].ToString());
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
            string portName = "COM" + portIndex.ToString();

            try
            {
                _port = new System.IO.Ports.SerialPort(portName, (int)BaudRate);

                _port.WriteTimeout = (int)WriteTotalTimeoutConstant;
                _port.ReadTimeout = (int)ReadTotalTimeoutConstant;
                
                //_port.Parity = (System.IO.Ports.Parity)Parity;
                //_port.StopBits = (System.IO.Ports.StopBits)StopBits;
                //_port.DataBits = (int)ByteSize;

                _port.Open();

                isOpen = _port.IsOpen;
            }
            catch { }

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

            bool fOK = false;
            count = 0;
            int attempts = 5;
            int i = 0;

            do
            {
                count += (uint)_port.BytesToRead;
                if (count > 0 && buffer.Length > count)
                {
                    for (; i < count; i++)
                        buffer[i] = (byte)_port.ReadByte();
                    fOK = true;
                    //break;
                }
                else
                    System.Threading.Thread.Sleep(100);
            } while (--attempts > 0);

            //try
            //{
            //    count = (uint)_port.BytesToRead;
            //    if (count > 0)
            //    {
            //        for (int i = 0; i < count; i++)
            //            buffer[i] = (byte)_port.ReadByte();
            //        fOK = true;
            //    }

            //}
            //catch { }

            return fOK;
        }
        /// <summary>
        /// Відправка інформації на СОМ-порт
        /// </summary>
        /// <param name="SendArr">Інформація</param>
        /// <returns>Якщо true то відправка інформації на СОМ-порт відбулася успішно</returns>
        public bool Write(byte[] SendArr)
        {
            bool fOK = false;

            _port.Write(SendArr, 0, SendArr.Length);

            int attempts = 5;
            do
            {
                if (_port.BytesToWrite > 0)
                    System.Threading.Thread.Sleep(100);
                else
                {
                    fOK = true;
                    break;
                }
            } while (--attempts < 0);

            return fOK;
        }
        public bool PortClear()
        {
            return PortAbort();
        }
        public bool PortAbort()
        {
            Marshal.GetLastWin32Error();
            GC.SuppressFinalize(this);

            try
            {
                _port.DiscardOutBuffer();
                _port.DiscardInBuffer();
            }
            catch { }

            Marshal.GetLastWin32Error();

            return true;
        }
        /// <summary>
        /// Закриття СОМ-порту
        /// </summary>
        /// <returns>Якщо true то СОМ-порт закритий успішно</returns>
        public bool Close()
        {
            if (_port != null && _port.IsOpen)
                _port.Close();
            isOpen = false;
            return true;
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

        // return control ui
        // interact with controls (save/load conifg)
        // update port config and init port


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

}
