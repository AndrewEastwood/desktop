using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using driver.Components;
using driver.Components.UI;
using driver.Config;
using driver.Lib;
using components.Shared.Interfaces;
using components.Shared.Attributes;
using components.Components.MMessageBox;
//0using mdcore;
//0using winapi;
//0using mdcore.Config;
//0using mdcore.Components;
//0using ;
//0using mdcore.Components.UI;

namespace PayDesk.Components.UI
{
    /// <summary>
    /// Service UI
    /// Peroform configuration of FP, Users and User's Schemas
    /// </summary>
    public partial class uiWndService : Form
    {
        // Private fileds
        private System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binF = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

        // Constructor
        public uiWndService()
        {
            InitializeComponent();
        }

        // Events
        private void Service_Load(object sender, EventArgs e)
        {
            LoadService();
            LoadUsersList();
            LoadSchemesList();
        }//ok
        private void Service_FormClosing(object sender, FormClosingEventArgs e)
        {
            tabPage8.Controls.Clear();
            pluginPanel.Controls.Clear();

            DialogResult = DialogResult.Retry;
        }
        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabControl2.TabPages[1].Hide();
            if (tabControl2.SelectedIndex == 1)
            {
                uiWndAdmin adm = new uiWndAdmin();
                adm.StartPosition = FormStartPosition.CenterParent;
                if (DialogResult.OK != adm.ShowDialog())
                    tabControl2.SelectTab(0);
                else
                {
                    maskedTextBox1.Text = ConfigManager.Instance.CommonConfiguration.APP_Admin;
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
            pluginPanel.Controls.Clear();
            tabPage8.Controls.Clear();

            if (comboBox4.SelectedItem != null && comboBox4.SelectedItem.ToString() != string.Empty)
            {
                Program.AppPlugins.SetActive(PluginType.FPDriver, comboBox4.SelectedItem.ToString());

                if (Program.AppPlugins.IsActive(PluginType.FPDriver))
                {
                    IFPDriver drv = Program.AppPlugins.GetActive<IFPDriver>();

                    pluginPanel.Controls.Add(drv.PortUI);
                    pluginPanel.Controls[drv.PortUI.Name].Dock = DockStyle.Fill;
                    tabPage8.Controls.Add(drv.DriverUI);
                    tabPage8.Controls[drv.DriverUI.Name].Dock = DockStyle.Fill;
                }
            }
            else
                Program.AppPlugins.SetActive(PluginType.FPDriver, "");

        }//save service settings
        private void LoadService()
        {
            comboBox4.Items.Add(string.Empty);
            comboBox4.Items.AddRange(Program.AppPlugins.GetNames(PluginType.FPDriver));

            // Restore last saved module
            try
            {
                comboBox4.SelectedItem = Program.AppPlugins.GetActive<IFPDriver>().Name;
                button6.PerformClick();
            }
            catch { }
        }
        #endregion
        #region Users TabPage
        #region Users buttons Events
        //private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        //{
        //    try
        //    {
        //        if (File.Exists(ConfigManager.Instance.CommonConfiguration.Path_Users + "\\" + "user" + (e.Node.Index + 1).ToString() + ".usr"))
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
                if (File.Exists(ConfigManager.Instance.CommonConfiguration.Path_Users + "\\" + "user" + (listBox1.SelectedIndex + 1).ToString() + ".usr"))
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
            File.Delete(ConfigManager.Instance.CommonConfiguration.Path_Users + "\\" + "user" + (listBox1.SelectedIndex + 1).ToString() + ".usr");
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
                    ConfigManager.Instance.CommonConfiguration.APP_Admin = maskedTextBox3.Text;
                    maskedTextBox1.Text = ConfigManager.Instance.CommonConfiguration.APP_Admin;
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
            string[] sMas = Directory.GetFiles(ConfigManager.Instance.CommonConfiguration.Path_Schemes + "\\", "*.mst");
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
            if (!Directory.Exists(ConfigManager.Instance.CommonConfiguration.Path_Users))
                Directory.CreateDirectory(ConfigManager.Instance.CommonConfiguration.Path_Users);

            for (byte i = 0; i < listBox1.Items.Count; i++)
            {
                if (File.Exists(ConfigManager.Instance.CommonConfiguration.Path_Users + "\\" +"user" + (i + 1).ToString() + ".usr"))
                {
                    UserConfig.LoadData(ConfigManager.Instance.CommonConfiguration.Path_Users + "\\" + "user" + (i + 1).ToString() + ".usr");

                    //treeView1.Nodes[i].ForeColor = Color.Green;
                    //treeView1.Nodes[i].NodeFont = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
                    listBox1.Items[i] = UserConfig.UserID + " *";
                }
            }
        }
        /// <summary>
        /// Завантаження інформації користувача
        /// </summary>
        /// <param name="id">Код користувача</param>
        private void LoadUserInfo(int id)
        {
            UserConfig.LoadData(ConfigManager.Instance.CommonConfiguration.Path_Users + "\\" + "user" + (id + 1) + ".usr");

            //Update components;
            uIDTBox.Text = UserConfig.UserID;
            userLTbox.Text = UserConfig.UserLogin;
            userPTbox.Text = UserConfig.UserPassword;
            userLFPTbox.Text = UserConfig.UserFpLogin;
            userPFPTbox.Text = UserConfig.UserFpPassword;  
            checkBox1.Checked = UserConfig.AdminState;
            for (byte i = 0; i < UserSchema.ITEMS_COUNT; i++)
                userProperties.SetItemChecked((int)i, UserConfig.Properties[i]);

            label5.Text = "Параметри : \"" + UserConfig.UserID + "\"  Схема : \"" + UserConfig.SchemaName + "\"";
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

            UserConfig.UserID = uIDTBox.Text;
            UserConfig.UserLogin = userLTbox.Text;
            UserConfig.UserPassword = userPTbox.Text;
            UserConfig.UserFpLogin = userLFPTbox.Text;
            UserConfig.UserFpPassword = userPFPTbox.Text; 
            UserConfig.AdminState = checkBox1.Checked;
            for (byte i = 0; i < UserSchema.ITEMS_COUNT; i++)
                UserConfig.Properties[i] = userProperties.GetItemChecked((int)i);

            UserConfig.SchemaName = RecognizeSchema(UserConfig.Properties);
            UserConfig.SaveData(ConfigManager.Instance.CommonConfiguration.Path_Users + "\\" + "user" + (id + 1) + ".usr");

            label5.Text = "Параметри : \"" + UserConfig.UserID + "\"  Схема : \"" + UserConfig.SchemaName + "\"";
            //treeView1.Nodes[id].ForeColor = Color.Green;
            //treeView1.Nodes[id].NodeFont = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            listBox1.Items[id] = UserConfig.UserID + " *";
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

            string[] sMas = Directory.GetFiles(ConfigManager.Instance.CommonConfiguration.Path_Schemes + "\\", "*.mst");
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
                for (j = 0; j < UserSchema.ITEMS_COUNT - 1; j++)
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
            FileMgrLib.ClearCheckedBox(ref userProperties);
            label5.Text = "Касир не зареєстрований";
            checkBox1.Checked = false;
            userLTbox.Text = (id + 1).ToString();
            userPTbox.Text = "";
            uIDTBox.Text = "";
            userLFPTbox.Clear();
            userPFPTbox.Clear();
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

            FileMgrLib.ClearCheckedBox(ref checkedListBox1);

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
                File.Delete(ConfigManager.Instance.CommonConfiguration.Path_Schemes + "\\" + oldName + ".mst");
            }

            FileStream fs = new FileStream(ConfigManager.Instance.CommonConfiguration.Path_Schemes + "\\" + sh.SchemaName + ".mst", FileMode.Create, FileAccess.Write);
            sh.SchemaTable = new bool[checkedListBox1.Items.Count];
            for (int i = 0; i < UserSchema.ITEMS_COUNT; i++)
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

            FileMgrLib.ClearCheckedBox(ref checkedListBox1);
            AutoEnableToolBar();

            isNewSch = false;
        }//ok
        private void cancelToolStripButton1_Click(object sender, EventArgs e)
        {
            FileMgrLib.ClearCheckedBox(ref checkedListBox1);
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

            string[] sMas = Directory.GetFiles(ConfigManager.Instance.CommonConfiguration.Path_Schemes + "\\", "*.mst");
            FileStream fs = null;
            for (int i = 0; i < sMas.Length; i++)
            {
                fs = new FileStream(sMas[i], FileMode.Open);
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binF = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                sch = (UserSchema)binF.Deserialize(fs);
                if (sch.SchemaName == toolStripComboBox1.SelectedItem.ToString())
                {
                    for (int j = 0; j < UserSchema.ITEMS_COUNT; j++)
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
                File.Delete(ConfigManager.Instance.CommonConfiguration.Path_Schemes + "\\" + shName + ".mst");
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
            if (!Directory.Exists(ConfigManager.Instance.CommonConfiguration.Path_Schemes))
                Directory.CreateDirectory(ConfigManager.Instance.CommonConfiguration.Path_Schemes);

            UserSchema sch = new UserSchema();
            checkedListBox1.Items.AddRange(sch.SchemaItems);
            userProperties.Items.AddRange(sch.SchemaItems);
            
            string[] sMas = Directory.GetFiles(ConfigManager.Instance.CommonConfiguration.Path_Schemes + "\\", "*.mst");

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

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}