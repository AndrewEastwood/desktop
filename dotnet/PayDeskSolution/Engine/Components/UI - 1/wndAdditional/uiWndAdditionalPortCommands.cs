using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using components.Components.SerialPort;
using components.Public;

namespace PayDesk.Components.UI.wndAdditional
{
    public partial class uiWndAdditionalPortCommands : Form
    {

        private Com_SerialPort port;


        public uiWndAdditionalPortCommands()
        {
            InitializeComponent();

            port = new Com_SerialPort();
        }

        ~uiWndAdditionalPortCommands()
        {
            this.port.Close();
        }

        /* Events */

        private void uiWndAdditionalPortCommands_Load(object sender, EventArgs e)
        {
            //ApplicationConfiguration.Instance.
            //Program.axCfg.BindConfigData();
            
            // 17/08/2011 ---  Hashtable configCommands = (Hashtable)Program.axCfg["additionalCommands"];

            Hashtable configCommands = ApplicationConfiguration.Instance.GetValueByKey<Hashtable>("additionalCommands");
            Hashtable listData = null;

            try
            {

                listData = (Hashtable)configCommands["commandList"];
            }
            catch { }

            Hashtable _commandsSource = new Hashtable();
            Dictionary<string, string> s = new Dictionary<string, string>();

            if (listData != null)
                foreach (DictionaryEntry ht in (Hashtable)listData["items"])
                {
                    _commandsSource[((Hashtable)ht.Value)["command"]] = ((Hashtable)ht.Value)["title"];

                    s[((Hashtable)ht.Value)["command"].ToString()] = ((Hashtable)ht.Value)["title"].ToString();
                    //ht.Value
                }


            BindingSource bs = new BindingSource();
            bs.DataSource = _commandsSource;
           
            this.lBox_main_commands.DisplayMember = "Value";
            this.lBox_main_commands.DataSource = bs;
            //this.lBox_main_commands.Sorted = true;

            try
            {
                port.PortConfig = (Hashtable)ApplicationConfiguration.Instance.GetValueByPath<Hashtable>("additionalCommands.portConfiguration");
                port.Open();
            }
            catch { }

        }

        private void uiWndAdditionalPortCommands_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.port.Close();
        }

        private void uiWndAdditionalPortCommands_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
            {
                Close();
            }
            if (e.KeyValue == new KeyEventArgs(Keys.Enter).KeyValue)
            {
                this.port.Write(this.PortCommandNative);
            }
        }

        private void btn_main_ok_Click(object sender, EventArgs e)
        {

            this.port.Write(this.PortCommandNative);
            //this.port.Close();
            //this.DialogResult = System.Windows.Forms.DialogResult.OK;
            //this.Close();
        }

        public string PortCommand
        {
            get
            {
                string cmd = string.Empty;
                cmd = ((DictionaryEntry)this.lBox_main_commands.SelectedValue).Key.ToString();
                return cmd;
            }
        }

        public byte[] PortCommandNative
        {
            get
            {
                string cmd = this.PortCommand;

                string[] clean_cmd = cmd.Replace("x0", string.Empty).Split(' ');
                List<byte> cm = new List<byte>(); 
                foreach (string item in clean_cmd)
                {
                    cm.Add(Convert.ToByte(item, 16));
                }
                
                return cm.ToArray();
            }
        }



    }
}
