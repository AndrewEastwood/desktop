using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using driver.Lib;
using driver.Config;
using driver.Components;
using driver.Components.Objects;
using components.Components.MMessageBox;
using components.Public;
using System.Collections;

namespace driver.Components.UI
{
    public partial class Request : FormEx
    {
        private double newTotal;
        private double newPrice;
        private DataRow dRow;
        private double currTotal;
        private double addTotal;
        private bool locked;
        private double articlePackage;
        //Objects.ComPort comP;
        //private ApplicationConfiguration appConfiguration;

        public Request(DataRow dRow, double tot)
        {
            InitializeComponent();

            //appConfiguration = appConfig;

            // restore position
            /*try
            {
                this.Location = ((Point)ConfigManager.Instance.CommonConfiguration.WP_ALL["Q_REQUEST"]);
                this.StartPosition = FormStartPosition.Manual;
            }
            catch
            {
                if (ConfigManager.Instance.CommonConfiguration.WP_ALL == null)
                    ConfigManager.Instance.CommonConfiguration.WP_ALL = new System.Collections.Hashtable();
                // saving position
                ConfigManager.Instance.CommonConfiguration.WP_ALL["Q_REQUEST"] = this.Location;
            }*/

            double.TryParse(dRow["PACK"].ToString(), out this.articlePackage);
            if (this.articlePackage == 0)
                this.articlePackage = 1;
            this.currTotal = tot;

            // window description
            Text = dRow["DESC"].ToString();

            // general quantity
            this.textBox1.Text = this.currTotal.ToString();
            
            // additional quantity
            if (driver.Config.ConfigManager.Instance.CommonConfiguration.Content_Cheques_AddTotal == "none")
                this.textBox2.Visible = this.label4.Visible = false;
                
            /*

            startTotal = -1;
            if (tot < 0)
                textBox1.Text = dRow["TOT"].ToString();
            else
            {
                textBox1.Text = tot.ToString();
                startTotal = AppFunc.GetDouble(dRow["TOT"]);
            }*/

            bool showGetScaleButton = false;
            for (byte i = 0; i < ((string[])driver.Config.ConfigManager.Instance.CommonConfiguration.APP_UnitFilter[0]).Length; i++)
                if (dRow["UNIT"].ToString() == ((string[])driver.Config.ConfigManager.Instance.CommonConfiguration.APP_UnitFilter[0])[i] && ((bool[])driver.Config.ConfigManager.Instance.CommonConfiguration.APP_UnitFilter[2])[i])
                {
                    showGetScaleButton = true;
                    break;
                }

            label_scale_separator.Visible = showGetScaleButton;
            button_scale_value.Visible = showGetScaleButton;
            textBox1.Enabled = textBox2.Enabled = !showGetScaleButton;


            // price list
            comboBox1.Items.Add(dRow["PRICE"]);
            if (UserConfig.Properties[1])
                comboBox1.Items.AddRange(new object[]{
                    dRow["PR1"],
                    dRow["PR2"],
                    dRow["PR3"]});

            if (UserConfig.Properties[2])
            {
                comboBox1.DropDownStyle = ComboBoxStyle.DropDown;
                if (!comboBox1.Items.Contains(dRow["ORIGPRICE"]))
                    comboBox1.Items.Add(dRow["ORIGPRICE"]);
            }
            else
                comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;

            comboBox1.SelectedIndex = 0;

            // helper
            textBox3.Text = "1 " + dRow["UNIT"].ToString() + " = " + this.articlePackage.ToString();

            /*
            this.flowLayoutPanel1.AutoSize = false;
            this.Height = this.flowLayoutPanel1.Height;
            this.flowLayoutPanel1.Height = 0;
            this.flowLayoutPanel1.Width = 0;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink; */
            // old 17-08-2011 ***  this.Width = this.panel1.Width;
            //this.Height;
            this.panel1.Height = 0;
            this.panel2.Height = 0;
            //this.panel2.Refresh();
            //this.button_scale_value.Visible = true;
            //this.AddProductBtn.Visible = true;
            //this.Width = this.panel1.Width; this.Height = panel1.Height;
            this.Height = 0;
            this.Update();
            // current article data row
            this.dRow = dRow;
            //this.TopMost = true;
            //this.comP = new Objects.ComPort();
            //this.comP.LoadPortConfig("ScalePort.xml");

            global::components.Components.SerialPort.Com_SerialPort.GetAndConfigurePort("scales", (Hashtable)ApplicationConfiguration.Instance["serialPortConnect.additionalDevices.scale"]);
            
        }

        ~Request()
        {
            global::components.Components.SerialPort.Com_SerialPort.ClosePort("scales");
            /*if (this.comP != null)
            {
                this.comP.Close();
                this.comP.Dispose();
            }*/
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                if (UserConfig.Properties[2])
                    comboBox1.DropDownStyle = ComboBoxStyle.DropDown;
            }
            else
                comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void AddProductBtn_Click(object sender, EventArgs e)
        {
            try
            {
                newTotal = MathLib.GetDouble(textBox1.Text);
                newPrice = MathLib.GetDouble(comboBox1.Text);

                newTotal = MathLib.GetRoundedDose(newTotal);
                newPrice = MathLib.GetRoundedMoney(newPrice);

                if (newTotal <= 0 || newTotal > 100000 )
                {
                    textBox1.Focus();
                    textBox1.SelectAll();
                    MMessageBoxEx.Show(this, "Помилкове значення кількості", Application.ProductName);
                    return;
                }

                if (newPrice <= 0)
                {
                    textBox1.Focus();
                    textBox1.SelectAll();
                    MMessageBoxEx.Show(this, "Ціна не може бути відємною", Application.ProductName);
                    return;
                }

                if (UserConfig.Properties[8] && !UserConfig.Properties[1] && !UserConfig.Properties[2])
                    newPrice = CoreLib.AutomaticPrice(newTotal, dRow);

            }
            catch { return; }

            DialogResult = DialogResult.OK;
            Close();
        }

        public DataRow dataRow { get { return this.dRow; } }

        public bool UpdateRowSource()
        {
            this.Enabled = false;
            Application.DoEvents();
            this.Enabled = true;
            System.Windows.Forms.SendKeys.Flush();
            if (this.ShowDialog() != DialogResult.OK)
                return false;
            HUpdateRowSourceEndHook();
            return true;
        }

        public bool UpdateRowSource(IWin32Window owner)
        {
            this.Enabled = false;
            Application.DoEvents();
            this.Enabled = true;
            System.Windows.Forms.SendKeys.Flush();
            if (this.ShowDialog(owner) != DialogResult.OK)
                return false;
            HUpdateRowSourceEndHook();
            return true;
        }

        public bool UpdateRowSource(Control owner)
        {
            this.Enabled = false;
            Application.DoEvents();
            this.Enabled = true;
            System.Windows.Forms.SendKeys.Flush();
            if (this.ShowDialogEx(owner) != DialogResult.OK)
                return false;
            HUpdateRowSourceEndHook();
            return true;
        }

        public bool UpdateRowSource(Control ctrlOwner, IWin32Window winOwner)
        {
            this.Enabled = false;
            Application.DoEvents();
            this.Enabled = true;
            System.Windows.Forms.SendKeys.Flush();
            //this.Parent = Form.FromHandle(winOwner.Handle);
            if (this.ShowDialogEx(ctrlOwner) != DialogResult.OK)
                return false;
            HUpdateRowSourceEndHook();
            return true;
        }

        private void HUpdateRowSourceEndHook()
        {
           /* int pack = 1;
            int.TryParse(dRow["PACK"].ToString(), out pack);
            if (pack == 0) pack = 1;*/
            dRow["PRICE"] = newPrice;
            /*
            if (AppConfig.APP_AddTotal == "type1")
                newTotal = AppFunc.GetRoundedDose(newTotal / pack);
            if (AppConfig.APP_AddTotal == "type2")
                newTotal = AppFunc.GetRoundedDose(newTotal * pack);
            */
            dRow["TOT"] = newTotal;
            dRow["SUM"] = dRow["ASUM"] = MathLib.GetRoundedMoney(newTotal * newPrice);
        }

        private void Request_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
            {
                Close();
                return;
            }
            if (e.KeyValue == new KeyEventArgs(Keys.F9).KeyValue)
            {
                this.button_scale_value.PerformClick();
                this.AddProductBtn.Select();
                this.AddProductBtn.Focus();
            }
            if (e.KeyValue == new KeyEventArgs(Keys.Enter).KeyValue)
            {
                this.AddProductBtn.PerformClick();
            }
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (locked) { locked = false; return; }

            switch (((TextBox)sender).Tag.ToString())
            {
                case "main":
                    {
                        this.currTotal = MathLib.GetDouble(textBox1.Text);
                        if (driver.Config.ConfigManager.Instance.CommonConfiguration.Content_Cheques_AddTotal == "type1")
                        {
                            this.addTotal = MathLib.GetRoundedDose(this.currTotal * this.articlePackage);
                        }
                        if (driver.Config.ConfigManager.Instance.CommonConfiguration.Content_Cheques_AddTotal == "type2")
                        {
                            this.addTotal = MathLib.GetRoundedDose(this.currTotal / this.articlePackage);
                        }
                        locked = true;
                        textBox2.Text = this.addTotal.ToString();
                        break;

                    }
                case "add":
                    {
                        this.addTotal = MathLib.GetDouble(textBox2.Text);
                        if (driver.Config.ConfigManager.Instance.CommonConfiguration.Content_Cheques_AddTotal == "type1")
                        {
                            this.currTotal = MathLib.GetRoundedDose(this.addTotal / this.articlePackage);
                        }
                        if (driver.Config.ConfigManager.Instance.CommonConfiguration.Content_Cheques_AddTotal == "type2")
                        {
                            this.currTotal = MathLib.GetRoundedDose(this.addTotal * this.articlePackage);
                        }
                        locked = true;
                        textBox1.Text = this.currTotal.ToString();
                        break;
                    }
            }
        }

        private void button_scale_value_Click(object sender, EventArgs e)
        {
            global::components.Components.SerialPort.Com_SerialPort comP = global::components.Components.SerialPort.Com_SerialPort.GetPort("scales");

            // get scale value or
            try
            {
                if (!comP.IsOpen)
                {
                    bool openRez = comP.Open();
                    if (!openRez)
                    {
                        MMessageBox.Show("Невдається відкрити СОМ-порт.", Application.ProductName);
                        this.AddProductBtn.Select();
                        this.AddProductBtn.Focus();
                        return;
                    }
                    /*
                    this.comP.PortMask((ushort)(winapi.Components.dwEvtMask.EV_RXCHAR | winapi.Components.dwEvtMask.EV_RXFLAG
                        | winapi.Components.dwEvtMask.EV_CTS | winapi.Components.dwEvtMask.EV_DSR | winapi.Components.dwEvtMask.EV_RLSD 
                        | winapi.Components.dwEvtMask.EV_BREAK | winapi.Components.dwEvtMask.EV_ERR | winapi.Components.dwEvtMask.EV_RING
                        | winapi.Components.dwEvtMask.EV_RXFLAG | winapi.Components.dwEvtMask.EV_TXEMPTY));*/
                }

                // get scale weight
                byte[] receivedAnswer = new byte[512];
                byte[] receivedWeight = new byte[512];
                uint countAnswer = 0;
                uint countWeight = 0;
                List<byte> cleanReceivedData = new List<byte>();

                byte[] buffer = new byte[512];
                int attempts = 2;
                int attemptsRead = 3;
                bool weightReceived = false;

                //WinAPI.OutputDebugString("R");
                // send BNQ (05)
                comP.Write(new byte[] { 0x05 });

                do
                {
                    Array.Clear(buffer, 0, buffer.Length);
                    if (comP.Read(ref buffer, out countAnswer))
                    {

                        switch (buffer[0])
                        {
                            // ACK (OK)
                            case 0x06:
                                {
                                    Array.Clear(buffer, 0, buffer.Length);
                                    cleanReceivedData.Clear();

                                    // send DC1
                                    comP.Write(new byte[] { 0x11 });
                                    System.Threading.Thread.Sleep(500);
                                    do
                                    {
                                        if (comP.Read(ref buffer, out countWeight))
                                        {
                                            if (cleanReceivedData.Count == 1 && cleanReceivedData[0] == 0x15)
                                                break;

                                            for (int i = 0; i < countWeight; i++)
                                                cleanReceivedData.Add(buffer[i]);

                                            // attempt ot find SDH, STX
                                            for (int i = 0; i < cleanReceivedData.Count; i++)
                                            {
                                                // SDH + next STX 
                                                if (cleanReceivedData[i] == 0x02 && cleanReceivedData[i - 1] == 0x01)
                                                {
                                                    // get status
                                                    //bool unstable = false;
                                                    //bool minus = false;
                                                    if (cleanReceivedData[i + 1] == 'U')
                                                    {
                                                        throw new Exception("Вага не зафіксована. Після стабілізації ваги повторіть команду");
                                                        //if (attemptsRead == 1)
                                                        //    MMessageBox.Show("Вага не зафіксована.", Application.ProductName);
                                                        //unstable = true;
                                                        //break;
                                                    }

                                                    // get sign
                                                    if (cleanReceivedData[i + 2] == '-')
                                                    {
                                                        throw new Exception("Відємне значення ваги");
                                                        // minus = true;
                                                        //if (attemptsRead == 1)
                                                        //    MMessageBox.Show("Відємне значення ваги.", Application.ProductName);
                                                        //break;
                                                    }
                                                    if (cleanReceivedData[i + 2] == 'F')
                                                    {
                                                        throw new Exception("Вага перевантажена");
                                                        //minus = true;
                                                        //if (attemptsRead == 1)
                                                        //    MMessageBox.Show("Вага перевантажена.", Application.ProductName);
                                                        //break;
                                                    }

                                                    // get weight
                                                    //if (unstable)
                                                    //    MMessageBox.Show("Вага не зафіксована.", Application.ProductName);

                                                    //if (minus)
                                                    //    MMessageBox.Show("Відємне значення ваги.", Application.ProductName);

                                                    //if (!unstable && !minus)
                                                    //{
                                                        byte[] __weightValue = new byte[6];
                                                        Array.Copy(cleanReceivedData.ToArray(), (i + 3), __weightValue, 0, __weightValue.Length);

                                                        textBox1.Text = Encoding.Default.GetString(__weightValue);
                                                        weightReceived = true;
                                                    //}

                                                    break;
                                                } // answer ok
                                            } // loop for detecting answer


                                        } // port reading

                                        attemptsRead--;


                                    } while (!weightReceived && attemptsRead > 0);

                                    break;
                                }

                            case 0x15:
                                {
                                    if (attempts == 1)
                                        throw new Exception("Вага недоступна");
                                        // MMessageBox.Show("Вага недоступна.", Application.ProductName);
                                    break;
                                }
                        }

                    }

                    attempts--;
                    if (attempts != 0)
                        System.Threading.Thread.Sleep(100);

                } while (!weightReceived && attempts > 0);

            }
            catch (Exception ex)
            {
                // show exception
                driver.Lib.CoreLib.WriteLog(ex, "handler button_scale_value_Click");
                MMessageBox.Show(ex.Message, Application.ProductName,  MessageBoxButtons.OK, MessageBoxIcon.Error);
                // MMessageBox.Show("Сталася помилка під час отримання значення ваги\r\nПеревірте справність та наявність підключення ваги до компютера\r\nПісля виключеня несправностей повторіть дію ще раз.", Application.ProductName);
            }

            comP.PortClear();

            this.AddProductBtn.Select();
            this.AddProductBtn.Focus();
        }

        private void Request_FormClosed(object sender, FormClosedEventArgs e)
        {
            //this.comP.Close();
            //this.comP.Dispose();
            global::components.Components.SerialPort.Com_SerialPort.ClosePort("scales");
        }

        private void Request_Load(object sender, EventArgs e)
        {/*
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Height = 0;
            this.panel2.Height = 0;
            this.AddProductBtn.Padding = new Padding(10);
            this.panel2.Refresh();
            this.button_scale_value.Visible = true;
            this.AddProductBtn.Visible = true;
            //this.Width = this.panel1.Width; this.Height = panel1.Height;
            this.Height = 0;
            this.Update();*/
        }
    }
}