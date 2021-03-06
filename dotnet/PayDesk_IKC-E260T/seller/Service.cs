﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using mdcore;
using winapi;

namespace PayDesk
{
    public partial class Service : Form
    {
        private System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binF = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

        //constructor
        public Service()
        {
            InitializeComponent();
        }

        //Events
        private void Service_Load(object sender, EventArgs e)
        {
            LoadService();
            LoadUsersList();
            LoadSchemesList();
        }//ok
        private void Service_FormClosing(object sender, FormClosingEventArgs e)
        {
            tabPage8.Controls.Clear();
            DialogResult = DialogResult.Retry;
        }
        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabControl2.TabPages[1].Hide();
            if (tabControl2.SelectedIndex == 1)
            {
                Admin adm = new Admin();
                adm.StartPosition = FormStartPosition.CenterParent;
                if (DialogResult.OK != adm.ShowDialog())
                    tabControl2.SelectTab(0);
                else
                {
                    maskedTextBox1.Text = AppConfig.APP_Admin;
                    tabControl2.SelectTab(1);
                    tabControl2.TabPages[1].Show();
                    listBox1.Select();
                    listBox1.SelectedIndex = 0;
                    listBox1.Invalidate(true);
                    //NodePerformClick(treeView1.Nodes[0]);
                }
                adm.Dispose();
            }
        }//main tab control
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }//ok

        #region Service TabPage
        private void button6_Click(object sender, EventArgs e)
        {
            Program.Service.ComPort.Tag = comboBox4.Text;
            Program.Service.ComPort.PortName = comboBox2.Text;//port name
            //baud
            switch (comboBox3.SelectedItem.ToString())
            {
                case "Baud_110": Program.Service.ComPort.BaudRate = BaudRate.Baud_110; break;
                case "Baud_300": Program.Service.ComPort.BaudRate = BaudRate.Baud_300; break;
                case "Baud_600": Program.Service.ComPort.BaudRate = BaudRate.Baud_600; break;
                case "Baud_1200": Program.Service.ComPort.BaudRate = BaudRate.Baud_1200; break;
                case "Baud_2400": Program.Service.ComPort.BaudRate = BaudRate.Baud_2400; break;
                case "Baud_4800": Program.Service.ComPort.BaudRate = BaudRate.Baud_4800; break;
                case "Baud_9600": Program.Service.ComPort.BaudRate = BaudRate.Baud_9600; break;
                case "Baud_14400": Program.Service.ComPort.BaudRate = BaudRate.Baud_14400; break;
                case "Baud_19200": Program.Service.ComPort.BaudRate = BaudRate.Baud_19200; break;
                case "Baud_38400": Program.Service.ComPort.BaudRate = BaudRate.Baud_38400; break;
                case "Baud_56000": Program.Service.ComPort.BaudRate = BaudRate.Baud_56000; break;
                case "Baud_57600": Program.Service.ComPort.BaudRate = BaudRate.Baud_57600; break;
                case "Baud_115200": Program.Service.ComPort.BaudRate = BaudRate.Baud_115200; break;
                case "Baud_128000": Program.Service.ComPort.BaudRate = BaudRate.Baud_128000; break;
                case "Baud_256000": Program.Service.ComPort.BaudRate = BaudRate.Baud_256000; break;
            }
            //bits
            switch (comboBox7.SelectedItem.ToString())
            {
                case "Five": Program.Service.ComPort.ByteSize = ByteSize.Five; break;
                case "Six": Program.Service.ComPort.ByteSize = ByteSize.Six; break;
                case "Seven": Program.Service.ComPort.ByteSize = ByteSize.Seven; break;
                case "Eight": Program.Service.ComPort.ByteSize = ByteSize.Eight; break;
            }
            //Parity
            switch (comboBox5.SelectedItem.ToString())
            {
                case "Even": Program.Service.ComPort.Parity = Parity.Even; break;
                case "Mark": Program.Service.ComPort.Parity = Parity.Mark; break;
                case "None": Program.Service.ComPort.Parity = Parity.None; break;
                case "Odd": Program.Service.ComPort.Parity = Parity.Odd; break;
                case "Space": Program.Service.ComPort.Parity = Parity.Space; break;
            }
            //StopBits
            switch (comboBox6.SelectedItem.ToString())
            {
                case "One": Program.Service.ComPort.StopBits = StopBits.One; break;
                case "OnePointFive": Program.Service.ComPort.StopBits = StopBits.OnePointFive; break;
                case "Two": Program.Service.ComPort.StopBits = StopBits.Two; break;
            }

            //timeouts
            try
            {
                if (textBox1.Text == "-1")
                    Program.Service.ComPort.RIntervalTimeout = API.MAXDWORD;
                else
                    Program.Service.ComPort.RIntervalTimeout = UInt32.Parse(textBox1.Text);
                if (textBox2.Text == "-1")
                    Program.Service.ComPort.RTotalTimeoutMultiplier = UInt32.MaxValue;
                else
                    Program.Service.ComPort.RTotalTimeoutMultiplier = UInt32.Parse(textBox2.Text);
                if (textBox3.Text == "-1")
                    Program.Service.ComPort.RTotalTimeoutConstant = UInt32.MaxValue;
                else
                    Program.Service.ComPort.RTotalTimeoutConstant = UInt32.Parse(textBox3.Text);
                if (textBox4.Text == "-1")
                    Program.Service.ComPort.WTotalTimeoutMultiplier = UInt32.MaxValue;
                else
                    Program.Service.ComPort.WTotalTimeoutMultiplier = UInt32.Parse(textBox4.Text);
                if (textBox5.Text == "-1")
                    Program.Service.ComPort.WTotalTimeoutConstant = UInt32.MaxValue;
                else
                    Program.Service.ComPort.WTotalTimeoutConstant = UInt32.Parse(textBox5.Text);
            }
            catch { return; }

            Program.Service.Save();
            ReloadProtocol();
        }//save service settings
        private void LoadService()
        {
            comboBox2.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());//ports
            if (comboBox2.Items.Count == 0)
            {
                comboBox4.SelectedItem = "-";
                ReloadProtocol();
                return;
            }
            if (comboBox2.Items.Contains(Program.Service.ComPort.PortName))
                comboBox2.SelectedItem = Program.Service.ComPort.PortName;
            else
                comboBox2.SelectedIndex = 0;

            comboBox4.Items.AddRange(Program.Service.Protocols);
            comboBox4.SelectedItem = Program.Service.ComPort.Tag.ToString();//Protocol

            comboBox3.SelectedItem = Program.Service.ComPort.BaudRate.ToString();//Baud
            comboBox7.SelectedItem = Program.Service.ComPort.ByteSize.ToString();//Bits
            comboBox5.SelectedItem = Program.Service.ComPort.Parity.ToString();//Patity
            comboBox6.SelectedItem = Program.Service.ComPort.StopBits.ToString();//StopBits

            if (Program.Service.ComPort.RIntervalTimeout == UInt32.MaxValue)
                textBox1.Text = "-1";
            else
                textBox1.Text = Program.Service.ComPort.RIntervalTimeout.ToString();

            if (Program.Service.ComPort.RTotalTimeoutMultiplier == UInt32.MaxValue)
                textBox2.Text = "-1";
            else
                textBox2.Text = Program.Service.ComPort.RTotalTimeoutMultiplier.ToString();

            if (Program.Service.ComPort.RTotalTimeoutConstant == UInt32.MaxValue)
                textBox3.Text = "-1";
            else
                textBox3.Text = Program.Service.ComPort.RTotalTimeoutConstant.ToString();

            if (Program.Service.ComPort.WTotalTimeoutMultiplier == UInt32.MaxValue)
                textBox4.Text = "-1";
            else
                textBox4.Text = Program.Service.ComPort.WTotalTimeoutMultiplier.ToString();

            if (Program.Service.ComPort.WTotalTimeoutConstant == UInt32.MaxValue)
                textBox5.Text = "-1";
            else
                textBox5.Text = Program.Service.ComPort.WTotalTimeoutConstant.ToString();

            ReloadProtocol();
        }
        private void ReloadProtocol()
        {
            tabPage8.Controls.Clear();
            comboBox4.SelectedItem = Program.Service.ComPort.Tag.ToString();

            if (Program.Service.UseEKKR)
            {
                tabPage8.Show();
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                comboBox7.Enabled = true;
                comboBox5.Enabled = true;
                comboBox6.Enabled = true;
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                textBox4.Enabled = true;
                textBox5.Enabled = true;

                Program.Service.FP_Panel.Dock = DockStyle.Fill;
                tabPage8.Controls.Add(Program.Service.FP_Panel);
            }
            else
            {
                tabPage8.Hide();
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
                comboBox7.Enabled = false;
                comboBox5.Enabled = false;
                comboBox6.Enabled = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                textBox4.Enabled = false;
                textBox5.Enabled = false;
            }
        }
        #endregion
        #region Users TabPage
        #region Users buttons Events
        //private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        //{
        //    try
        //    {
        //        if (File.Exists(AppConfig.Path_Users + "\\" + "user" + (e.Node.Index + 1).ToString() + ".usr"))
        //        {
        //            LoadUserInfo(e.Node.Index);
        //            button2.Enabled = true;
        //        }
        //        else
        //        {
        //            ClearUserPage(e.Node.Index);
        //            button2.Enabled = false;
        //        }
        //    }
        //    catch { }
        //}
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(AppConfig.Path_Users + "\\" + "user" + (listBox1.SelectedIndex + 1).ToString() + ".usr"))
                {
                    LoadUserInfo(listBox1.SelectedIndex);
                    button2.Enabled = true;
                }
                else
                {
                    ClearUserPage(listBox1.SelectedIndex);
                    button2.Enabled = false;
                }
            }
            catch { }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            SaveUser(listBox1.SelectedIndex);
        }//save
        private void button2_Click(object sender, EventArgs e)
        {
            File.Delete(AppConfig.Path_Users + "\\" + "user" + (listBox1.SelectedIndex + 1).ToString() + ".usr");
            //ClearUserPage(treeView1.SelectedNode.Index);
            ClearUserPage(listBox1.SelectedIndex);

            //treeView1.SelectedNode.ForeColor = Color.Red;
            //treeView1.SelectedNode.NodeFont = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic);
            listBox1.Items[listBox1.SelectedIndex] = "Касир " + (listBox1.SelectedIndex + 1).ToString();
            listBox1.Invalidate(true);
            //NodePerformClick(treeView1.SelectedNode);
        }//delete
        //Amin
        private void maskedTextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Enter).KeyValue)
                maskedTextBox3.Select();

        }
        private void maskedTextBox3_TextChanged(object sender, EventArgs e)
        {
            if (maskedTextBox2.Text != maskedTextBox3.Text)
                maskedTextBox3.BackColor = Color.Pink;
            else
            {
                if (maskedTextBox3.BackColor == Color.LightGreen)
                    return;

                maskedTextBox3.BackColor = Color.LightGreen;
                if (DialogResult.Yes == MMessageBox.Show(this, "Змінити пароль адміністратора", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                {
                    AppConfig.APP_Admin = maskedTextBox3.Text;
                    maskedTextBox1.Text = AppConfig.APP_Admin;
                }
                maskedTextBox3.Text = "";
                maskedTextBox2.Text = "";
                maskedTextBox2.Focus();
                maskedTextBox3.BackColor = Color.FromKnownColor(KnownColor.Window);
            }
        }//ok
        //Use schema
        private void button5_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
                return;

            UserSchema sch = new UserSchema();
            string[] sMas = Directory.GetFiles(AppConfig.Path_Schemes + "\\", "*.mst");
            FileStream fs = null;

            for (int i = 0; i < sMas.Length; i++)
            {
                fs = new FileStream(sMas[i], FileMode.Open);
                sch = (UserSchema)binF.Deserialize(fs);
                fs.Close();
                fs.Dispose();

                if (sch.SchemaName == comboBox1.Text)
                {
                    for (int j = 0; j < sch.SchemaTable.Length; j++)
                        userProperties.SetItemChecked(j, sch.SchemaTable[j]);

                    //SaveUser(treeView1.SelectedNode.Index);
                    SaveUser(listBox1.SelectedIndex);
                }
            }
        }//ok
        #endregion

        //Methods for administration of users
        /// <summary>
        /// Завантаження списку користувачів
        /// </summary>
        private void LoadUsersList()
        {
            if (!Directory.Exists(AppConfig.Path_Users))
                Directory.CreateDirectory(AppConfig.Path_Users);

            for (byte i = 0; i < listBox1.Items.Count; i++)
            {
                if (File.Exists(AppConfig.Path_Users + "\\" +"user" + (i + 1).ToString() + ".usr"))
                {
                    UserStruct.LoadData(AppConfig.Path_Users + "\\" + "user" + (i + 1).ToString() + ".usr");

                    //treeView1.Nodes[i].ForeColor = Color.Green;
                    //treeView1.Nodes[i].NodeFont = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
                    listBox1.Items[i] = UserStruct.UserID + " *";
                }
            }
        }
        /// <summary>
        /// Завантаження інформації користувача
        /// </summary>
        /// <param name="id">Код користувача</param>
        private void LoadUserInfo(int id)
        {
            UserStruct.LoadData(AppConfig.Path_Users + "\\" + "user" + (id + 1) + ".usr");

            //Update components;
            uIDTBox.Text = UserStruct.UserID;
            userLTbox.Text = UserStruct.UserLogin;
            userPTbox.Text = UserStruct.UserPassword;

            checkBox1.Checked = UserStruct.AdminState;
            for (byte i = 0; i < UserSchema.ItemsCount; i++)
                userProperties.SetItemChecked((int)i, UserStruct.Properties[i]);

            label5.Text = "Параметри : \"" + UserStruct.UserID + "\"  Схема : \"" + UserStruct.SchemaName + "\"";
        }//ok
        /// <summary>
        /// Збереження параметрів користувача
        /// </summary>
        /// <param name="id">Код користувача</param>
        private void SaveUser(int id)
        {
            if (uIDTBox.Text == "")
                uIDTBox.Text = "Касир " + (id + 1);

            if (userLTbox.Text == "")
                userLTbox.Text = (id + 1).ToString();

            if (userPTbox.Text == "")
                userPTbox.Text = (id + 1).ToString();

            UserStruct.UserID = uIDTBox.Text;
            UserStruct.UserLogin = userLTbox.Text;
            UserStruct.UserPassword = userPTbox.Text;
            UserStruct.UserFLogin = (byte)id;

            UserStruct.AdminState = checkBox1.Checked;
            for (byte i = 0; i < UserSchema.ItemsCount; i++)
                UserStruct.Properties[i] = userProperties.GetItemChecked((int)i);

            UserStruct.SchemaName = RecognizeSchema(UserStruct.Properties);

            UserStruct.SaveData(AppConfig.Path_Users + "\\" + "user" + (id + 1) + ".usr");

            label5.Text = "Параметри : \"" + UserStruct.UserID + "\"  Схема : \"" + UserStruct.SchemaName + "\"";
            //treeView1.Nodes[id].ForeColor = Color.Green;
            //treeView1.Nodes[id].NodeFont = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            listBox1.Items[id] = UserStruct.UserID + " *";
            listBox1.Invalidate(true);
            //NodePerformClick(treeView1.Nodes[id]);
        }//ok
        /// <summary>
        /// Перевірка параметрів користувача з існуючими схемами
        /// </summary>
        /// <param name="schema">Список існуючих схем</param>
        /// <returns>Назва схеми, яка співападає з параметрами користувачів</returns>
        private string RecognizeSchema(bool[] schema)
        {
            UserSchema sch = new UserSchema();
            int j = 0;

            string[] sMas = Directory.GetFiles(AppConfig.Path_Schemes + "\\", "*.mst");
            bool match = true;
            FileStream fs = null;
            for (int i = 0; i < sMas.Length; i++)
            {
                fs = new FileStream(sMas[i], FileMode.Open);
                try
                {
                    sch = (UserSchema)binF.Deserialize(fs);
                }
                catch { }
                fs.Close();
                fs.Dispose();
                for (j = 0; j < UserSchema.ItemsCount - 1; j++)
                    if (schema[j] != sch.SchemaTable[j])
                    {
                        match = false;
                        break;
                    }


                if (match)
                    return sch.SchemaName;
                else
                    match = true;
            }

            return "";
        }
        /// <summary>
        /// Очищення параметрів користувача
        /// </summary>
        /// <param name="id">Код користувача</param>
        private void ClearUserPage(int id)
        {
            ClearCheckedBox(ref userProperties);
            label5.Text = "Касир не зареєстрований";
            checkBox1.Checked = false;
            userLTbox.Text = (id + 1).ToString();
            userPTbox.Text = "";
            uIDTBox.Text = "";
        }
        #endregion
        #region Schema TabPage
        private bool isNewSch = false;
        #region Schemes toolbar events
        private void newToolStripButton1_Click(object sender, EventArgs e)
        {
            ToolBarItemsEnable(false);

            saveToolStripButton1.Enabled = true;
            cancelToolStripButton1.Enabled = true;
            toolStripTextBox1.Enabled = true;
            checkedListBox1.Enabled = true;

            ClearCheckedBox(ref checkedListBox1);

            toolStripTextBox1.Enabled = true;
            toolStripTextBox1.Text = GetNextSchemaName(toolStripComboBox1.Items);
            toolStripTextBox1.Focus();
            toolStripTextBox1.SelectAll();
            isNewSch = true;
        }//ok
        private void saveToolStripButton1_Click(object sender, EventArgs e)
        {
            UserSchema sh = new UserSchema();
            string oldName = string.Empty;

            if (toolStripComboBox1.Items.Count != 0)
                oldName = toolStripComboBox1.SelectedItem.ToString();
            sh.SchemaName = toolStripTextBox1.Text;

            if (!isNewSch && oldName != sh.SchemaName)
            {
                DialogResult rez = MMessageBox.Show("Обновити схему \"" + oldName + "\" ?", "Сервіс", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (rez == DialogResult.No)
                {
                    toolStripTextBox1.Focus();
                    toolStripTextBox1.SelectAll();
                    return;
                }
                File.Delete(AppConfig.Path_Schemes + "\\" + oldName + ".mst");
            }

            FileStream fs = new FileStream(AppConfig.Path_Schemes + "\\" + sh.SchemaName + ".mst", FileMode.Create, FileAccess.Write);
            sh.SchemaTable = new bool[checkedListBox1.Items.Count];
            for (int i = 0; i < UserSchema.ItemsCount; i++)
                sh.SchemaTable[i] = checkedListBox1.GetItemChecked(i);
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binF = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            binF.Serialize(fs, sh);
            fs.Close();


            if (isNewSch)
            {
                toolStripComboBox1.Items.Add(sh.SchemaName);
                toolStripComboBox1.SelectedItem = sh.SchemaName;
                comboBox1.Items.Add(sh.SchemaName);
            }
            else if (sh.SchemaName != oldName)
            {
                toolStripComboBox1.Items.RemoveAt(toolStripComboBox1.Items.IndexOf(oldName));
                toolStripComboBox1.Items.Add(sh.SchemaName);

                comboBox1.Items.RemoveAt(comboBox1.Items.IndexOf(oldName));
                comboBox1.Items.Add(sh.SchemaName);

                toolStripComboBox1.SelectedItem = sh.SchemaName;
                comboBox1.SelectedIndex = 0;
            }

            toolStripTextBox1.Text = "";
            toolStripTextBox1.Enabled = false;

            ClearCheckedBox(ref checkedListBox1);
            AutoEnableToolBar();

            isNewSch = false;
        }//ok
        private void cancelToolStripButton1_Click(object sender, EventArgs e)
        {
            ClearCheckedBox(ref checkedListBox1);
            toolStripTextBox1.Text = string.Empty;
            toolStripTextBox1.Enabled = false;
            AutoEnableToolBar();

        }//ok
        private void editToolStripButton5_Click(object sender, EventArgs e)
        {
            
            ToolBarItemsEnable(false);

            saveToolStripButton1.Enabled = true;
            cancelToolStripButton1.Enabled = true;
            toolStripTextBox1.Enabled = true;

            UserSchema sch = new UserSchema();

            string[] sMas = Directory.GetFiles(AppConfig.Path_Schemes + "\\", "*.mst");
            FileStream fs = null;
            for (int i = 0; i < sMas.Length; i++)
            {
                fs = new FileStream(sMas[i], FileMode.Open);
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binF = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                sch = (UserSchema)binF.Deserialize(fs);
                if (sch.SchemaName == toolStripComboBox1.SelectedItem.ToString())
                {
                    for (int j = 0; j < UserSchema.ItemsCount; j++)
                        checkedListBox1.SetItemChecked(j, sch.SchemaTable[j]);

                    fs.Close();
                    break;
                }
                fs.Close();
            }

            toolStripTextBox1.Text = sch.SchemaName;
            checkedListBox1.Enabled = true;
        }//ok)
        private void deleteToolStripButton3_Click(object sender, EventArgs e)
        {
            string shName = toolStripComboBox1.Text;
            if (DialogResult.Yes == MMessageBox.Show("Видалити \"" + shName + "\" схему", "Сервіс", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                File.Delete(AppConfig.Path_Schemes + "\\" + shName + ".mst");
                toolStripComboBox1.Items.Remove(shName);
                comboBox1.Items.Remove(shName);
                AutoEnableToolBar();

                if (toolStripComboBox1.Items.Count != 0)
                    toolStripComboBox1.SelectedIndex = 0;
            }

        }//ok) 
        #endregion
        //Methods for administration of schemas
        private void LoadSchemesList()
        {
            if (!Directory.Exists(AppConfig.Path_Schemes))
                Directory.CreateDirectory(AppConfig.Path_Schemes);

            UserSchema sch = new UserSchema();
            checkedListBox1.Items.AddRange(sch.SchemaItems);
            userProperties.Items.AddRange(sch.SchemaItems);

            string[] sMas = Directory.GetFiles(AppConfig.Path_Schemes + "\\", "*.mst");

            for (int i = 0; i < sMas.Length; i++)
            {
                try
                {
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binF = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    sch = (UserSchema)binF.Deserialize(new FileStream(sMas[i], FileMode.Open));
                    toolStripComboBox1.Items.Add(sch.SchemaName);
                    comboBox1.Items.Add(sch.SchemaName);
                }
                catch { }
            }
            if (toolStripComboBox1.Items.Count != 0)
            {
                toolStripComboBox1.SelectedIndex = 0;
                comboBox1.SelectedIndex = 0;
            }

            AutoEnableToolBar();
        }
        private string GetNextSchemaName(ComboBox.ObjectCollection items)
        {
            string name = string.Empty;
            int idx = 0;
            string tx = "NovaShema";

            for (int i = 0; i < items.Count; i++)
                if (items[i].ToString() == tx)
                {
                    idx++;
                    tx = "NovaShema" + idx.ToString();
                    i = -1;
                }

            name = "NovaShema";
            if (idx != 0)
                name += idx.ToString();

            return name;
        }
        private void AutoEnableToolBar()
        {
            newToolStripButton1.Enabled = true;
            saveToolStripButton1.Enabled = false;
            cancelToolStripButton1.Enabled = false;

            bool vis = false;
            if (toolStripComboBox1.Items.Count != 0)
                vis = true;

            toolStripComboBox1.Enabled = vis;
            editToolStripButton5.Enabled = vis;
            deleteToolStripButton3.Enabled = vis;
            checkedListBox1.Enabled = false;

            //control from UserPage
            button5.Enabled = vis;
            comboBox1.Enabled = vis;
        }
        private void ToolBarItemsEnable(bool vis)
        {
            newToolStripButton1.Enabled = vis;
            saveToolStripButton1.Enabled = vis;
            editToolStripButton5.Enabled = vis;
            toolStripTextBox1.Enabled = vis;
            toolStripComboBox1.Enabled = vis;
            deleteToolStripButton3.Enabled = vis;
        }
        #endregion

        //My Methods
        #region Common Methods (User+Schema)
        private void ClearCheckedBox(ref CheckedListBox checkedListBox1)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
                checkedListBox1.SetItemChecked(i, false);
            checkedListBox1.SetItemChecked(checkedListBox1.Items.Count - 1, false);
        }//ok
        #endregion
    }
}