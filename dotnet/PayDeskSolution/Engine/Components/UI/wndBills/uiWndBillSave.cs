using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//0using mdcore;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
//0using ;
using System.Collections;
using driver.Lib;
using driver.Components.UI;
using components.Components.MMessageBox;
using driver.Config;
using components.Public;

namespace PayDesk.Components.UI.wndBills
{
    public partial class uiWndBillSave : Form
    {
        //таблиця рахунку  
        private DataTable dtBill;
        private Dictionary<string, object> billInfoStructure;
        //номер рахунку
        private string billNo;
        //Якщо true то рахунок є новим інакше рахунок вже був збережений
        private bool isNewBill;
        private bool needCleanup;
        private bool updateComment;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="dTable">Таблиця рахунку</param>
        /// 
        public uiWndBillSave(DataTable dTable)
        {
            InitializeComponent();

            // restore position
            try
            {
                this.Location = ((Point)ConfigManager.Instance.CommonConfiguration.WP_ALL["BILL_CMT"]);
                this.StartPosition = FormStartPosition.Manual;
            }
            catch
            {
                if (ConfigManager.Instance.CommonConfiguration.WP_ALL == null)
                    ConfigManager.Instance.CommonConfiguration.WP_ALL = new System.Collections.Hashtable();
                // saving position
                ConfigManager.Instance.CommonConfiguration.WP_ALL["BILL_CMT"] = this.Location;
            }

            isNewBill = !dTable.ExtendedProperties.Contains("BILL") || dTable.ExtendedProperties["BILL"] == null;
            if (isNewBill)
                billNo = DataWorkBill.GetNextBillID();
            else
            {
                billNo = ((Dictionary<string , object>)dTable.ExtendedProperties["BILL"])["BILL_NO"].ToString();
                richTextBox1.Text = ((Dictionary<string, object>)dTable.ExtendedProperties["BILL"])["COMMENT"].ToString();
            }
            this.dtBill = dTable.Copy();
            this.needCleanup = false;
            Text += " " + billNo.ToString();

            /* adding templates */

            Hashtable configDinningRoom = ApplicationConfiguration.Instance.GetValueByKey<Hashtable>("dinningRoom");

            // 17-08-2011 *** Hashtable configDinningRoom = (Hashtable)Program.axCfg["dinningRoom"];
            this.flowLayoutPanel_top.Height = 0;
            this.flowLayoutPanel_top.Update();
            this.flowLayoutPanel_top.Controls.Clear();
            try
            {
                string pattern = configDinningRoom["patternBarOrderComment"].ToString();
                string[] sections = pattern.Split(new char[] { ' ' });

                foreach (string s in sections)
                {
                    try
                    {
                        ComboBox comboBox1 = new ComboBox();
                        comboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                        comboBox1.FormattingEnabled = true;
                        comboBox1.Location = new System.Drawing.Point(10, 20);
                        comboBox1.Margin = new System.Windows.Forms.Padding(20, 20, 5, 10);
                        comboBox1.Name = "comboBox_" + s;
                        comboBox1.Size = new System.Drawing.Size(258, 33); // with button width 215
                        comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
                        comboBox1.Sorted = true;
                        comboBox1.MaxDropDownItems = 8;
                        comboBox1.IntegralHeight = false;
                        comboBox1.KeyDown += new KeyEventHandler(comboBox_KeyDown);
                        /*
                        Button buttonAdd1 = new Button();
                        buttonAdd1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                        buttonAdd1.Name = "buttonAdd_" + s;
                        buttonAdd1.Text = "+";
                        buttonAdd1.Margin = new System.Windows.Forms.Padding(5, 20, 20, 10);
                        buttonAdd1.Size = new System.Drawing.Size(33, 33);
                        buttonAdd1.Click += new EventHandler(buttonAdd1_Click);
                        buttonAdd1.Tag = comboBox1.Name;
                        */
                        foreach (DictionaryEntry section in (Hashtable)configDinningRoom[s])
                        {
                            try
                            {
                                if (section.Key.ToString() == "items")
                                {
                                    foreach (DictionaryEntry singleItem in (Hashtable)section.Value)
                                        comboBox1.Items.Add(singleItem.Value);
                                    continue;
                                }
                                if (section.Key.ToString() == "active")
                                {
                                    comboBox1.Visible = bool.Parse(section.Value.ToString());
                                }
                            }
                            catch { }
                        }

                        if (comboBox1.Visible)
                        {
                            comboBox1.TabIndex = this.flowLayoutPanel_top.Controls.Count + 1;
                            this.flowLayoutPanel_top.Controls.Add(comboBox1);
                            //this.flowLayoutPanel_top.Controls.Add(buttonAdd1); // uncomment if button + is necessary
                            if (comboBox1.Items.Count != 0)
                                comboBox1.SelectedIndex = 0;
                            //comboBox1.SelectedIndexChanged += new EventHandler(comboBox1_SelectedIndexChanged);
                        }
                    }
                    catch { }
                }
            }
            catch { }

            /* in progress*/
            if (!isNewBill)
            {
                if (richTextBox1.Text != string.Empty)
                {
                    string[] items = richTextBox1.Text.Split(new char[] { ' ' });
                    if (this.flowLayoutPanel_top.Controls.Count != 0)
                    {
                        int idx = 0;
                        foreach (Control ct in this.flowLayoutPanel_top.Controls)
                        {
                            try
                            {
                                ComboBox cBox = (ComboBox)ct;
                                cBox.Text = items[idx].Replace("%20", " ");
                                richTextBox1.Text = richTextBox1.Text.Replace(items[idx], "").Trim();
                                idx++;
                            }
                            catch { }
                        }
                    }
                    richTextBox1.Text = richTextBox1.Text.Trim();
                }
            }

            /* control update */
            // 17-08-2011 if (!Program.axCfg.GetValueByPath<bool>("dinningRoom.useAdditionalComment"))
            if (!ApplicationConfiguration.Instance.GetValueByPath<bool>("dinningRoom.useAdditionalComment"))
            {
                this.label1.Visible = false;
                this.richTextBox1.Visible = false;
                this.button_clear.Visible = false;
                this.Height = 0;
            }

            this.button_save.TabIndex = this.flowLayoutPanel_top.Controls.Count + 1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dTable"></param>
        /// <param name="clearInfo"></param>
        public uiWndBillSave(DataTable dTable, bool clearInfo)
            : this(dTable)
        {
            this.needCleanup = clearInfo;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dTable"></param>
        /// <param name="billInfo"></param>
        public uiWndBillSave(DataTable dTable, Dictionary<string, object> billInfo)
            : this(dTable)
        {
            this.billInfoStructure = billInfo;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dTable"></param>
        /// <param name="clearInfo"></param>
        /// <param name="billInfo"></param>
        public uiWndBillSave(DataTable dTable, bool clearInfo, Dictionary<string ,object> billInfo)
            : this(dTable, clearInfo)
        {
            this.billInfoStructure = billInfo;
        }

        private void uiWndBillSave_Load(object sender, EventArgs e)
        {
            if (!isNewBill && !this.updateComment)
                button_save.PerformClick();
            else
            {
                this.Visible = true;
                this.Opacity = 1.0;

                /* activating first control */
                if (this.flowLayoutPanel_top.Controls.Count > 0)
                {
                    //ComboBox c = (ComboBox)this.flowLayoutPanel_top.Controls[0];
                    this.flowLayoutPanel_top.Controls[0].Focus();
                    this.flowLayoutPanel_top.Controls[0].Select();
                    this.Update();
                    //c.TabIndex = 1;
                }
            }

        }
        /// <summary>
        /// Виконує збереження рахунку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            string templateComment = string.Empty;
            /* getting all values from template controls */
            foreach (Control ct in flowLayoutPanel_top.Controls)
            {
                try
                {
                    ComboBox cTplBox = (ComboBox)ct;
                    templateComment += cTplBox.Text.Replace(" ", "%20") + " ";
                }
                catch { }

            }

            if (templateComment != string.Empty)
                templateComment = templateComment.Trim();

            if (richTextBox1.Text != string.Empty)
                templateComment += " " + richTextBox1.Text;

            //string templateComment = richTextBox1.Text.Trim();

            if (templateComment == string.Empty)
            {
                MMessageBox.Show("Введіть коментар рахунку", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (DataWorkBill.SaveBill(isNewBill, billNo, templateComment, ref this.dtBill))
            {/*
                if (this.needCleanup)
                    dtBill.ExtendedProperties.Clear();*/
                DialogResult = DialogResult.OK;
            }

            Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.richTextBox1.Text = string.Empty;
        }
        private void comboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Down || e.KeyData == Keys.Up)
            {
                ComboBox cb = (ComboBox)sender;
                cb.DroppedDown = true;
            }
            /*
            if (e.KeyData == Keys.Enter)
                this.button_save.PerformClick();*/
        }

        /// <summary>
        /// Обробник клавіатури
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BillRequets_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
            {
                Close();
            }/*
            if (e.KeyValue == new KeyEventArgs(Keys.Enter).KeyValue)
            {
                button1.PerformClick();
            }*/
        }
    
        /* Properties */
        /// <summary>
        /// 
        /// </summary>
        public string GetNewBillNo { get { return this.billNo.PadLeft(5, '0'); } }
        /// <summary>
        /// 
        /// </summary>
        public bool IsNewBill { get { return this.isNewBill; } }
        /// <summary>
        /// Saved bill object
        /// </summary>
        public DataTable SavedBill { get { return this.dtBill; } }
        public object SavedBillInfoStructure { get { return this.dtBill.ExtendedProperties; } }
        public bool UpdateComment { get { return this.updateComment; } set { this.updateComment = value; } }

        private void uiWndBillSave_FormClosing(object sender, FormClosingEventArgs e)
        {
            // saving position
            ConfigManager.Instance.CommonConfiguration.WP_ALL["BILL_CMT"] = this.Location;
        }



    }
}