using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace PayDesk.Components.UI.wndAdditional
{
    public partial class uiWndAdditionalPortCommands : Form
    {

        private FPService.ComPort port;


        public uiWndAdditionalPortCommands()
        {
            InitializeComponent();

            port = new FPService.ComPort();
        }

        ~uiWndAdditionalPortCommands()
        {
            this.port.Close();
        }

        /* Events */

        private void uiWndAdditionalPortCommands_Load(object sender, EventArgs e)
        {
            System.IO.FileStream fs = System.IO.File.Open("termal.txt", System.IO.FileMode.OpenOrCreate);
            System.IO.StreamReader sr = new System.IO.StreamReader(fs, Encoding.Default);

            Hashtable configCommands = new Hashtable();
            Hashtable portConfig = new Hashtable();
            string tln = string.Empty;
            bool useCommands = false;

            while ((tln = sr.ReadLine()) != null)
            {
                if (tln == ";")
                {
                    useCommands = true;
                    continue;
                }

                if (useCommands)
                    configCommands[tln.Split('=')[1].Trim()] = tln.Split('=')[0].Trim();
                else
                    portConfig[tln.Split('=')[0].Trim()] = tln.Split('=')[1].Trim();

            }

            fs.Close();
            fs.Dispose();
            
            BindingSource bs = new BindingSource();
            bs.DataSource = configCommands;
           
            this.lBox_main_commands.DisplayMember = "Value";
            this.lBox_main_commands.DataSource = bs;
            //this.lBox_main_commands.Sorted = true;


            port.PortConfig = portConfig;
            port.Open();

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
