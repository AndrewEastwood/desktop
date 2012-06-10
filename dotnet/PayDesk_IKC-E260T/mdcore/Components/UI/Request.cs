using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using mdcore.Lib;
using mdcore.Config;
using mdcore.Components;

namespace mdcore.Components.UI
{
    public partial class Request : Form
    {
        private double newTotal;
        private double newPrice;
        private DataRow dRow;
        private double currTotal;
        private double addTotal;
        private bool locked;
        private double articlePackage;
        Objects.ComPort comP;

        public Request(DataRow dRow, double tot)
        {
            InitializeComponent();
            double.TryParse(dRow["PACK"].ToString(), out this.articlePackage);
            if (this.articlePackage == 0)
                this.articlePackage = 1;
            this.currTotal = tot;

            // window description
            Text = dRow["DESC"].ToString();

            // general quantity
            this.textBox1.Text = this.currTotal.ToString();

            // additional quantity
            if (AppConfig.Content_Cheques_AddTotal == "none")
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
            for (byte i = 0; i < ((string[])AppConfig.APP_UnitFilter[0]).Length; i++)
                if (dRow["UNIT"].ToString() == ((string[])AppConfig.APP_UnitFilter[0])[i] && ((bool[])AppConfig.APP_UnitFilter[2])[i])
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

            //this.flowLayoutPanel1.Height = 0;
            //this.flowLayoutPanel1.Width = 0;
            //this.AutoSize = true;
            this.Height = 0;
            this.Width = this.panel1.Width;
            this.Update();

            // current article data row
            this.dRow = dRow;

            this.comP = new Objects.ComPort();
            this.comP.LoadPortConfig("ScalePort.xml");
        }

        ~Request()
        {
            if (this.comP != null)
            {
                this.comP.Close();
                this.comP.Dispose();
            }
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
                newTotal = CoreLib.GetDouble(textBox1.Text.Trim());
                newPrice = CoreLib.GetDouble(comboBox1.Text.Trim());

                newTotal = CoreLib.GetRoundedDose(newTotal);
                newPrice = CoreLib.GetRoundedMoney(newPrice);

                if (newTotal <= 0 || newTotal > 100000 )
                {
                    textBox1.Focus();
                    textBox1.SelectAll();
                    MMessageBox.Show("Помилкове значення кількості");
                    return;
                }

                if (newPrice <= 0)
                {
                    textBox1.Focus();
                    textBox1.SelectAll();
                    MMessageBox.Show("Ціна не може бути відємною");
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
            if (this.ShowDialog() != DialogResult.OK)
                return false;
            HUpdateRowSourceEndHook();
            return true;
        }

        public bool UpdateRowSource(IWin32Window owner)
        {
            if (this.ShowDialog(owner) != DialogResult.OK)
                return false;
            HUpdateRowSourceEndHook();
            return true;
        }

        public bool UpdateRowSource(Control owner)
        {
            if (this.ShowDialog(owner) != DialogResult.OK)
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
            dRow["SUM"] = dRow["ASUM"] = CoreLib.GetRoundedMoney(newTotal * newPrice);
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
            }
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (locked) { locked = false; return; }


            switch (((TextBox)sender).Tag.ToString())
            {
                case "main":
                    {
                        this.currTotal = CoreLib.GetDouble(textBox1.Text);
                        if (AppConfig.Content_Cheques_AddTotal == "type1")
                        {
                            this.addTotal = CoreLib.GetRoundedDose(this.currTotal * this.articlePackage);
                        }
                        if (AppConfig.Content_Cheques_AddTotal == "type2")
                        {
                            this.addTotal = CoreLib.GetRoundedDose(this.currTotal / this.articlePackage);
                        }
                        locked = true;
                        textBox2.Text = this.addTotal.ToString();
                        locked = false;
                        break;

                    }
                case "add":
                    {
                        this.addTotal = CoreLib.GetDouble(textBox2.Text);
                        if (AppConfig.Content_Cheques_AddTotal == "type1")
                        {
                            this.currTotal = CoreLib.GetRoundedDose(this.addTotal / this.articlePackage);
                        }
                        if (AppConfig.Content_Cheques_AddTotal == "type2")
                        {
                            this.currTotal = CoreLib.GetRoundedDose(this.addTotal * this.articlePackage);
                        }
                        locked = true;
                        textBox1.Text = this.currTotal.ToString();
                        locked = false;
                        break;
                    }
            }
        }

        private void button_scale_value_Click(object sender, EventArgs e)
        {
            // get scale value or
            try
            {
                if (!this.comP.IsOpen)
                {
                    bool openRez = this.comP.Open();
                    if (!openRez)
                    {
                        MMessageBox.Show("Невдається відкрити СОМ-порт.", Application.ProductName);
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
                int attempts = 30;
                int attemptsRead = 30;
                bool weightReceived = false;

                //WinAPI.OutputDebugString("R");
                // send BNQ (05)
                this.comP.Write(new byte[] { 0x05 });
                

                do
                {
                    Array.Clear(buffer, 0, buffer.Length);
                    if (this.comP.Read(ref buffer, out countAnswer))
                    {

                        switch (buffer[0])
                        {
                            // ACK (OK)
                            case 0x06:
                                {
                                    Array.Clear(buffer, 0, buffer.Length);
                                    cleanReceivedData.Clear();

                                    // send DC1
                                    this.comP.Write(new byte[] { 0x11 });
                                    System.Threading.Thread.Sleep(500);
                                    do
                                    {
                                        if (this.comP.Read(ref buffer, out countWeight))
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
                                                    bool unstable = false;
                                                    bool minus = false;
                                                    if (cleanReceivedData[i + 1] == 'U')
                                                    {
                                                        if (attemptsRead == 1)
                                                            MMessageBox.Show("Вага не зафіксована.", Application.ProductName);
                                                        unstable = true;
                                                        break;
                                                    }

                                                    // get sign
                                                    if (cleanReceivedData[i + 2] == '-')
                                                    {
                                                        minus = true;
                                                        if (attemptsRead == 1)
                                                            MMessageBox.Show("Відємне значення ваги.", Application.ProductName);
                                                        break;
                                                    }
                                                    if (cleanReceivedData[i + 2] == 'F')
                                                    {
                                                        minus = true;
                                                        if (attemptsRead == 1)
                                                            MMessageBox.Show("Вага перевантажена.", Application.ProductName);
                                                        break;
                                                    }

                                                    // get weight
                                                    if (unstable)
                                                        MMessageBox.Show("Вага не зафіксована.", Application.ProductName);

                                                    if (minus)
                                                        MMessageBox.Show("Відємне значення ваги.", Application.ProductName);

                                                    if (!unstable && !minus)
                                                    {
                                                        byte[] __weightValue = new byte[6];
                                                        Array.Copy(cleanReceivedData.ToArray(), (i + 3), __weightValue, 0, __weightValue.Length);

                                                        textBox1.Text = Encoding.Default.GetString(__weightValue);
                                                        weightReceived = true;
                                                    }

                                                    break;
                                                } // answer ok
                                            } // loof for detecting answer


                                        } // port reading

                                        attemptsRead--;


                                    } while (!weightReceived && attemptsRead > 0);

                                    break;
                                }

                            case 0x15:
                                {
                                    if (attempts == 1)
                                        MMessageBox.Show("Вага недоступна.", Application.ProductName);
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
                mdcore.Lib.CoreLib.WriteLog(ex, "handler button_scale_value_Click");
                MMessageBox.Show("Сталася помилка під час отримання значення ваги\r\nПеревірте справність та наявність підключення ваги до компютера\r\nПісля виключеня несправностей повторіть дію ще раз.", Application.ProductName);
            }

            this.comP.PortClear();
        }

        private void Request_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.comP.Close();
            this.comP.Dispose();
        }
    }
}