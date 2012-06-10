using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace FPService
{
    public partial class FPService
    {
        private const byte totProtocols = 1;
        private UserControl _panel;
        private ComPort _comPort1;
        private Dictionary<string, ComPort> _comPortItems; // owner || port
        private string[][] _pFunc;

        //Protocols
        private Protocol.Protocol[] _protocols = new global::FPService.Protocol.Protocol[totProtocols];
        private byte protocolIndex = 0;

        //constructor
        public FPService()
        {
            Environment.CurrentDirectory = System.Windows.Forms.Application.StartupPath;
            _comPort1 = new ComPort();
            _comPortItems = new Dictionary<string, global::FPService.ComPort>();

            //LoadProtocols;
            _protocols[0] = new global::FPService.Protocol.IKSE260T.IKSE260T();//IKC-E260T
            //_protocols[1] = new global::FPService.Protocol.Maria301MT.Maria301MT();//Maria 301
            Load();
        }

        private void NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                CallFunction(_comPort1.Tag.ToString(), e.Node.Name, e.Node.Text, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, _comPort1.Tag.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Methods
        /// <summary>
        /// Виклик методу з драйверу ФП
        /// </summary>
        /// <param name="fp">Назва ФП</param>
        /// <param name="method">Назва методу</param>
        /// <param name="description">Опис методу</param>
        /// <param name="param">Параметри методу</param>
        /// <returns>Результат виконання функції ФП</returns>
        public object[] CallFunction(string fp, string method, string description, object[] param)
        {
            object[] retValue = new object[0];

            if (_comPort1.IsOpen)
                _comPort1.Close();

            _comPort1.Open();

            switch (fp)
            {
                case "IKC-E260T":
                    {
                        retValue = _protocols[0].CallFunction(method, description, this._comPort1, param);
                        break;
                    }
            }

            _comPort1.Close();

            return retValue;
        }
        /// <summary>
        /// Виклик методу з драйверу ФП
        /// </summary>
        /// <param name="method">Назва методу</param>
        /// <param name="description">Опис методу</param>
        /// <param name="param">Параметри методу</param>
        /// <returns>Результат виконання функції ФП</returns>
        public object[] CallFunction(string method, string description, object[] param)
        {
            return CallFunction(_comPort1.Tag.ToString(), method, description, param);
        }
        /// <summary>
        /// Виклик методу з драйверу ФП
        /// </summary>
        /// <param name="method">Назва методу</param>
        /// <param name="param">Параметри методу</param>
        /// <returns>Результат виконання функції ФП</returns>
        public object[] CallFunction(string method, object[] param)
        {
            return CallFunction(_comPort1.Tag.ToString(), method, string.Empty, param);
        }

        private void SetProtocol()
        {
            //init default values
            _panel = new UserControl();
            _pFunc = new string[2][] { new string[0], new string[0] };

            for (protocolIndex = 0; protocolIndex < _protocols.Length; protocolIndex++)
            {
                if (_protocols[protocolIndex].Protocol_Name == _comPort1.Tag.ToString())
                {
                    _panel = _protocols[protocolIndex].FP_Panel;
                    ((TreeView)_panel.Controls["functionsTree"]).NodeMouseDoubleClick -= this.NodeMouseDoubleClick;
                    ((TreeView)_panel.Controls["functionsTree"]).NodeMouseDoubleClick += this.NodeMouseDoubleClick;
                    _pFunc = _protocols[protocolIndex].ProtocolPublicFunctions;
                    break;
                }
            }

            if (protocolIndex == _protocols.Length)
                protocolIndex = byte.MaxValue;
        }
        public void Save()
        {
            Environment.CurrentDirectory = System.Windows.Forms.Application.StartupPath;
            _comPort1.SavePortConfig();
            SetProtocol();
            //_comPort1.SavePortConfig();
        }
        public void Load()
        {
            Environment.CurrentDirectory = System.Windows.Forms.Application.StartupPath;
            _comPort1.LoadPortConfig();
            SetProtocol();
        }

        //Properties
        public string[] Protocols
        {
            get
            {
                string[] names = new string[_protocols.Length + 1];
                names[0] = "-";
                for (int i = 0; i < _protocols.Length; i++)
                    names[i + 1] = ((Protocol.Protocol)_protocols[i]).Protocol_Name;

                return names;
            }
        }
        public bool UseEKKR
        {
            get
            {
                return _comPort1.Tag.ToString() != "-";
            }
        }
        public ComPort ComPort
        {
            get { return _comPort1; }
            set { _comPort1 = value; }
        }
        public UserControl FP_Panel
        {
            get { return _panel; }
        }
        public string[][] PublicFunctions
        {
            get { return _pFunc; }
        }
    }
}