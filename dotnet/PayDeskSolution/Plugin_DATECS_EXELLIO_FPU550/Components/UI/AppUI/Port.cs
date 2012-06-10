using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using PluginModule.Components.Objects;
using System.Collections;

namespace DATECS_EXELLIO_FPU550.UI.AppUI
{
    public partial class Port : UserControl
    {
        private ComPort _port;

        public Port(ref ComPort _drvcport)
        {
            InitializeComponent();

            // Initialize communication port
            _port = _drvcport;
            InitializePortConfiguration(_port.PortConfig);
        }

        private void InitializePortConfiguration(Hashtable pc)
        {
            //Add all ports of this computer
            comboBox2.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());

            //Port
            if (comboBox2.Items.Contains(pc["PORT"].ToString()))
                comboBox2.SelectedItem = pc["PORT"].ToString();
            else
                if (comboBox2.Items.Count != 0)
                    comboBox2.SelectedIndex = 0;
            //Rate
            comboBox3.SelectedItem = pc["RATE"].ToString();
            //DataBits
            comboBox7.SelectedItem = pc["DBITS"].ToString();
            //Patity
            comboBox5.SelectedItem = pc["PARITY"].ToString();
            //StopBits
            comboBox6.SelectedItem = pc["SBITS"].ToString();
            //Read Timeout
            if ((uint)pc["RT"] == UInt32.MaxValue)
                textBox1.Text = "-1";
            else
                textBox1.Text = pc["RT"].ToString();
            //Read Multipier
            if ((uint)pc["RM"] == UInt32.MaxValue)
                textBox2.Text = "-1";
            else
                textBox2.Text = pc["RM"].ToString();
            //Read Constant
            if ((uint)pc["RC"] == UInt32.MaxValue)
                textBox3.Text = "-1";
            else
                textBox3.Text = pc["RC"].ToString();
            //Write Multiplier
            if ((uint)pc["WM"] == UInt32.MaxValue)
                textBox4.Text = "-1";
            else
                textBox4.Text = pc["WM"].ToString();
            //Write Constant
            if ((uint)pc["WC"] == UInt32.MaxValue)
                textBox5.Text = "-1";
            else
                textBox5.Text = pc["WC"].ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Hashtable _pcfg = _port.PortConfig;
            _pcfg["PORT"] = comboBox2.SelectedItem.ToString();
            _pcfg["RATE"] = comboBox3.SelectedItem.ToString();
            _pcfg["PARITY"] = comboBox5.SelectedItem.ToString();
            _pcfg["DBITS"] = comboBox7.SelectedItem.ToString();
            _pcfg["SBITS"] = comboBox6.SelectedItem.ToString();
            _pcfg["RT"] = textBox1.Text;
            _pcfg["RM"] = textBox2.Text;
            _pcfg["RC"] = textBox3.Text;
            _pcfg["WM"] = textBox4.Text;
            _pcfg["WC"] = textBox5.Text;
            _port.PortConfig = _pcfg;

            _port.SavePortConfig();
        }
    }
}
