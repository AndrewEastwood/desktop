using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using mdcore;
using mdcore.Config;
using mdcore.Lib;

namespace PayDesk.Components.UI
{
    public partial class uiWndSettings : Form
    {
        private int i = 0;

        public uiWndSettings()
        {
            InitializeComponent();
        }
        private void Settings_Load(object sender, EventArgs e)
        {
            //#TAX
            //LoadAppTaxRates
            dataGridView2.Rows.Clear();
            if (AppConfig.TAX_AppTaxChar != null)
                for (i = 0; i < AppConfig.TAX_AppTaxChar.Length; i++)
                {
                    dataGridView2.Rows.Add(
                        AppConfig.TAX_AppTaxChar[i],
                        AppConfig.TAX_AppTaxRates[i],
                        AppConfig.TAX_AppTaxDisc[i]);
                }
            //LoadComparableTaxTable
            DataGridViewRow dwRow = null;
            DataGridViewComboBoxCell cbCell = null;
            DataGridViewTextBoxCell tbCell = null;
            for (i = 0; i < AppConfig.TAX_MarketColumn.Length; i++)
            {
                cbCell = new DataGridViewComboBoxCell();
                cbCell.ValueType = typeof(char);
                cbCell.MaxDropDownItems = 8;
                cbCell.Value = ' ';

                tbCell = new DataGridViewTextBoxCell();
                tbCell.ValueType = typeof(char);
                tbCell.Value = AppConfig.TAX_MarketColumn[i];

                dwRow = new DataGridViewRow();
                dwRow.Cells.Add(cbCell);
                dwRow.Cells.Add(tbCell);

                dataGridView1.Rows.Add(dwRow);

                cbCell.Dispose();
                tbCell.Dispose();
                dwRow.Dispose();
            }
            dataGridView1.ReadOnly = false;
            dataGridView1.Columns[1].ReadOnly = true;
            UpdateTaxValues();
            //RestoreComparableValues
            try
            {
                if (AppConfig.TAX_AppColumn != null)
                    for (i = 0; i < AppConfig.TAX_AppColumn.Length; i++)
                        dataGridView1["pd", i].Value = AppConfig.TAX_AppColumn[i];
            }
            catch { }

            //#COMMON
            //PayDesk
            sys_gen_kasaNom.Value = AppConfig.APP_PayDesk;
            //RefreshRate
            numericUpDown5.Value = AppConfig.APP_RefreshRate / 1000;
            //SubUnit
            sys_gen_pidrozd.Value = AppConfig.APP_SubUnit;
            //DefaultCountOfArticle
            textBox3.Text = AppConfig.APP_StartTotal.ToString();
            //SubUnitName
            textBox5.Text = AppConfig.APP_SubUnitName;
            //Customer's ID  
            textBox1.Text = AppConfig.APP_ClientID;
            //WeightType
            comboBox1.SelectedIndex = AppConfig.APP_WeightType;
            //DefaultSearchType
            comboBox2.SelectedIndex = AppConfig.APP_SearchType;
            //Search Types Access
            // --> realized in Event (comboBox2_SelctedIndexChanged)

            //#PATH
            //Folder of Exchange
            phExTbBox.Text = AppConfig.Path_Exchnage;
            //Folder of Cheques
            phCheqTbBox.Text = AppConfig.Path_Cheques;
            //Folder of Bills
            phBillTbBox.Text = AppConfig.Path_Bills;
            //Folder of Articles
            phArtTBox.Text = AppConfig.Path_Articles;

            //#ADDITIONAL
            //Timeout request of refresh
            numericUpDown6.Value = AppConfig.APP_RefreshTimeout;
            //ChequeName
            maskedTextBox1.Text = AppConfig.APP_ChequeName;
            //Invent auto save
            numericUpDown3.Value = AppConfig.APP_InvAutoSave;
            //Language
            comboBox5.SelectedItem = AppConfig.APP_Language;
            //Total digits after point of money
            numericUpDown2.Value = AppConfig.APP_MoneyDecimals;
            //Total digits after point of dose
            numericUpDown4.Value = AppConfig.APP_DoseDecimals;
            //Clear temp on exit
            checkBox6.Checked = AppConfig.APP_ClearTEMPonExit;
            //Show info on indicator
            checkBox7.Checked = AppConfig.APP_ShowInfoOnIndicator;
            //Invent window
            checkBox4.Checked = AppConfig.APP_ShowInventWindow;
            //One copy
            checkBox5.Checked = AppConfig.APP_AllowOneCopy;
            //barcode scanner
            scaner_cBox_useProgramMode.Checked = AppConfig.APP_ScannerUseProgamMode;
            scanner_char_frequency.Value = (decimal)AppConfig.APP_ScannerCharReadFrequency;
            system_cBox_buyerBCReader.SelectedIndex = AppConfig.APP_BuyerBarCodeSource;
            system_num_buyerBCMinLen.Value = (decimal)AppConfig.APP_BuyerBarCodeMinLen;

            // PAGE: Content
            // -TabControl: Main
            // --TabPage: Common
            this.content_general_checkBox_showPromptMsgOnIllegal.Checked = AppConfig.Content_Common_PromptMsgOnIllegal;
            // --TabPage: Cheques
            // Add total
            switch (AppConfig.Content_Cheques_AddTotal)
            {
                case "none": this.content_order_radioButton_addQuantity1.Checked = true; break;
                case "type1": this.content_order_radioButton_addQuantity2.Checked = true; break;
                case "type2": this.content_order_radioButton_addQuantity3.Checked = true; break;
            }
            // --TabPage: Bills
            this.checkBox_pBills_DeleteBillAtEnd.Checked = AppConfig.Content_Bills_KeepAliveAfterCheque;
            // --TabPage: Articles
            this.checkBox_pArticles_KeepOriginData.Checked = AppConfig.Content_Articles_KeepDataAfterImport;

            //Navigator
            treeView1.Select();
            treeView1.SelectedNode = treeView1.Nodes[0];
        }

        //Navigator
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            PanelPDV.Visible = false;
            PanelSystem.Visible = false;
            PanelStyle.Visible = false;
            panelContent.Visible = false;

            switch (e.Node.Index)
            {
                case 0: { PanelPDV.Visible = true; break; }
                case 1: { PanelSystem.Visible = true; break; }
                case 2: { PanelStyle.Visible = true; break; }
                case 3: { panelContent.Visible = true; break; }
            }
        }     

        //Pages
        #region Taxs Rates
        private void button12_Click(object sender, EventArgs e)
        {
            dataGridView2.Rows.Add(new object[] { "", "0.0", true });
            dataGridView2.Rows[dataGridView2.RowCount - 1].Selected = true;
            dataGridView2.CurrentCell = dataGridView2["taxChar", dataGridView2.RowCount - 1];
            dataGridView2.BeginEdit(true);
        }//ADD TAX
        private void button14_Click(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentRow != null)
                dataGridView2.Rows.Remove(dataGridView2.CurrentRow);
            UpdateTaxValues();
        }//Delete TAX
        private void dataGridView2_CellEndEdit(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            if (dataGridView2[e.ColumnIndex, e.RowIndex].Value.ToString() == "")
                dataGridView2.BeginEdit(true);
            else
                UpdateTaxValues();
        }
        #endregion

        #region System
         private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Search Types Access
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
                checkedListBox1.SetItemChecked(i, AppConfig.APP_SrchTypesAccess[i]);
            checkedListBox1.SetItemChecked(comboBox2.SelectedIndex, true);
        }
        //Path
        private void phExBtn_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Description = "Вкажіть шлях до каталогу, звідки буде оновлюватись інформація про товари";
            folderBrowserDialog1.SelectedPath = phExTbBox.Text;
            folderBrowserDialog1.ShowDialog();
            phExTbBox.Text = folderBrowserDialog1.SelectedPath;
        }//Folder of Exchange
        private void phCheqBtn_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Description = "Вкажіть шлях до каталогу, де будуть зберігатись чеки";
            folderBrowserDialog1.SelectedPath = phCheqTbBox.Text;
            folderBrowserDialog1.ShowDialog();
            phCheqTbBox.Text = folderBrowserDialog1.SelectedPath;
        }//Folder of Cheques
        private void phBilBtn_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Description = "Вкажіть шлях до каталогу, де будуть зберігатись рахунки";
            folderBrowserDialog1.SelectedPath = phBillTbBox.Text;
            folderBrowserDialog1.ShowDialog();
            phBillTbBox.Text = folderBrowserDialog1.SelectedPath;
        }//Folder of Bills
        private void phArtBtn_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Description = "Вкажіть шлях до каталогу, де будуть зберігатись товари";
            folderBrowserDialog1.SelectedPath = phArtTBox.Text;
            folderBrowserDialog1.ShowDialog();
            phArtTBox.Text = folderBrowserDialog1.SelectedPath;

        }//Folder of Articles
        #endregion

        #region Style
        //General
        private void ColorList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (Colors_ListBox.IndexFromPoint(e.Location) >= 0)
            {
                switch (Colors_ListBox.SelectedIndex)
                {
                    case 0: colorDialog1.Color = AppConfig.STYLE_BackgroundInfPan; break;
                    case 1: colorDialog1.Color = AppConfig.STYLE_BackgroundAddPan; break;
                    case 2: colorDialog1.Color = AppConfig.STYLE_BackgroundSumRest; break;
                    case 3: colorDialog1.Color = AppConfig.STYLE_BackgroundAChqTbl; break;
                    case 4: colorDialog1.Color = AppConfig.STYLE_BackgroundNAChqTbl; break;
                    case 5: colorDialog1.Color = AppConfig.STYLE_BackgroundArtTbl; break;
                    case 6: colorDialog1.Color = AppConfig.STYLE_BackgroundStatPan; break;
                }
                colorDialog1.ShowDialog();
                switch (Colors_ListBox.SelectedIndex)
                {
                    case 0: AppConfig.STYLE_BackgroundInfPan = colorDialog1.Color; break;
                    case 1: AppConfig.STYLE_BackgroundAddPan = colorDialog1.Color; break;
                    case 2: AppConfig.STYLE_BackgroundSumRest = colorDialog1.Color; break;
                    case 3: AppConfig.STYLE_BackgroundAChqTbl = colorDialog1.Color; break;
                    case 4: AppConfig.STYLE_BackgroundNAChqTbl = colorDialog1.Color; break;
                    case 5: AppConfig.STYLE_BackgroundArtTbl = colorDialog1.Color; break;
                    case 6: AppConfig.STYLE_BackgroundStatPan = colorDialog1.Color; break;
                }
            }
        }
        private void FontList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (Fonts_ListBox.IndexFromPoint(e.Location) >= 0)
            {
                switch (Fonts_ListBox.SelectedIndex)
                {
                    case 0: 
                        fontDialog1.Font = AppConfig.STYLE_SumFont;
                        fontDialog1.Color = AppConfig.STYLE_SumFontColor; 
                        break;
                    case 1:
                        fontDialog1.Font = AppConfig.STYLE_RestFont;
                        fontDialog1.Color = AppConfig.STYLE_RestFontColor;
                        break;
                    case 2:
                        fontDialog1.Font = AppConfig.STYLE_ArticlesFont;
                        fontDialog1.Color = AppConfig.STYLE_ArticlesFontColor;
                        break;
                    case 3:
                        fontDialog1.Font = AppConfig.STYLE_ChequeFont;
                        fontDialog1.Color = AppConfig.STYLE_ChequeFontColor;
                        break;
                    case 4:
                        fontDialog1.Font = AppConfig.STYLE_StatusFont;
                        fontDialog1.Color = AppConfig.STYLE_StatusFontColor;
                        break;
                    case 5:
                        fontDialog1.Font = AppConfig.STYLE_AddInformerFont;
                        fontDialog1.Color = AppConfig.STYLE_AddInformerFontColor;
                        break;
                    case 6:
                        fontDialog1.Font = AppConfig.STYLE_ChqInformerFont;
                        fontDialog1.Color = AppConfig.STYLE_ChqInformerFontColor;
                        break;
                    case 7:
                        fontDialog1.Font = AppConfig.STYLE_AppInformerFont;
                        fontDialog1.Color = AppConfig.STYLE_AppInformerFontColor;
                        break;
                    case 8:
                        fontDialog1.Font = AppConfig.STYLE_BillWindow;
                        break;
                    case 9:
                        fontDialog1.Font = AppConfig.STYLE_BillWindowEntry;
                        break;
                    case 10:
                        fontDialog1.Font = AppConfig.STYLE_BillWindowEntryItems;
                        break;
                }
                fontDialog1.ShowDialog();
                switch (Fonts_ListBox.SelectedIndex)
                {
                    case 0:
                        AppConfig.STYLE_SumFont = fontDialog1.Font;
                        AppConfig.STYLE_SumFontColor = fontDialog1.Color;
                        break;
                    case 1:
                        AppConfig.STYLE_RestFont = fontDialog1.Font;
                        AppConfig.STYLE_RestFontColor = fontDialog1.Color;
                        break;
                    case 2:
                        AppConfig.STYLE_ArticlesFont = fontDialog1.Font;
                        AppConfig.STYLE_ArticlesFontColor = fontDialog1.Color;
                        break;
                    case 3:
                        AppConfig.STYLE_ChequeFont = fontDialog1.Font;
                        AppConfig.STYLE_ChequeFontColor = fontDialog1.Color;
                        break;
                    case 4:
                        AppConfig.STYLE_StatusFont = fontDialog1.Font;
                        AppConfig.STYLE_StatusFontColor = fontDialog1.Color;
                        break;
                    case 5:
                        AppConfig.STYLE_AddInformerFont = fontDialog1.Font;
                        AppConfig.STYLE_AddInformerFontColor = fontDialog1.Color;
                        break;
                    case 6:
                        AppConfig.STYLE_ChqInformerFont = fontDialog1.Font;
                        AppConfig.STYLE_ChqInformerFontColor = fontDialog1.Color;
                        break;
                    case 7:
                        AppConfig.STYLE_AppInformerFont = fontDialog1.Font;
                        AppConfig.STYLE_AppInformerFontColor = fontDialog1.Color;
                        break;
                    case 8:
                        AppConfig.STYLE_BillWindow = fontDialog1.Font;
                        break;
                    case 9:
                        AppConfig.STYLE_BillWindowEntry = fontDialog1.Font;
                        break;
                    case 10:
                        AppConfig.STYLE_BillWindowEntryItems = fontDialog1.Font;
                        break;
                }
            }
        }
        private void Misc_ListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            wndSettings.uiWndSettingsNumberRequest bR = new wndSettings.uiWndSettingsNumberRequest();
            if (Misc_ListBox.IndexFromPoint(e.Location) >= 0)
            {
                switch (Misc_ListBox.SelectedIndex)
                {
                    case 0: bR.Value = AppConfig.STYLE_Misc_ChequeRowHeight; break;
                    case 1: bR.Value = AppConfig.STYLE_Misc_ArticleRowHeight; break;
                }
                if (bR.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    switch (Misc_ListBox.SelectedIndex)
                    {
                        case 0: AppConfig.STYLE_Misc_ChequeRowHeight = bR.Value; break;
                        case 1: AppConfig.STYLE_Misc_ArticleRowHeight = bR.Value; break;
                    }
            }

            bR.Dispose();
        }
        #endregion

        private void Settings_KeyDown(object sender, KeyEventArgs e)
        {
            KeyEventArgs esc = new KeyEventArgs(Keys.Escape);

            if (e.KeyValue == esc.KeyValue)
            {
                Close();
                return;
            }
        }
        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                //#TAX
                //SaveAppTaxRates(dataGridView2);
                char[] tChars = new char[0];
                double[] tRates = new double[0];
                bool[] tDisc = new bool[0];

                char curr_tChar = ' ';
                double curr_tRate = 0.0;
                bool curr_tDisc = true;

                bool error = false;

                for (i = 0; i < dataGridView2.RowCount; i++)
                {
                    try
                    {
                        curr_tChar = dataGridView2.Rows[i].Cells["taxChar"].Value.ToString()[0];
                        curr_tRate = CoreLib.GetDouble(dataGridView2.Rows[i].Cells["taxRate"].Value.ToString());
                        curr_tDisc = Convert.ToBoolean(dataGridView2.Rows[i].Cells["taxDisc"].Value);

                        Array.Resize<char>(ref tChars, tChars.Length + 1);
                        Array.Resize<double>(ref tRates, tRates.Length + 1);
                        Array.Resize<bool>(ref tDisc, tDisc.Length + 1);

                        tChars[tChars.Length - 1] = curr_tChar;
                        tRates[tRates.Length - 1] = curr_tRate;
                        tDisc[tDisc.Length - 1] = curr_tDisc;
                    }
                    catch { error = true; break; }
                }

                if (!error)
                {
                    AppConfig.TAX_AppTaxChar = tChars;
                    AppConfig.TAX_AppTaxRates = tRates;
                    AppConfig.TAX_AppTaxDisc = tDisc;
                }

                //SaveComparableTaxTable(dataGridView1);
                if (AppConfig.TAX_AppColumn == null)
                    AppConfig.TAX_AppColumn = new char[dataGridView1.RowCount];

                for (i = 0; i < dataGridView1.RowCount; i++)
                    AppConfig.TAX_AppColumn[i] = char.Parse(dataGridView1.Rows[i].Cells[0].Value.ToString());

                //#COMMON
                //PayDesk
                AppConfig.APP_PayDesk = Convert.ToByte(sys_gen_kasaNom.Value);
                //RefreshRate
                AppConfig.APP_RefreshRate = Convert.ToInt32(numericUpDown5.Value) * 1000;
                //SubUnit
                AppConfig.APP_SubUnit = Convert.ToByte(sys_gen_pidrozd.Value);
                //DefaultCountOfArticle
                AppConfig.APP_StartTotal = Convert.ToDouble(textBox3.Text);
                //SubUnitName
                AppConfig.APP_SubUnitName = textBox5.Text;
                //Customer's ID  
                AppConfig.APP_ClientID = textBox1.Text;
                //WeightType
                AppConfig.APP_WeightType = comboBox1.SelectedIndex;
                //DefaultSearchType
                AppConfig.APP_SearchType = comboBox2.SelectedIndex;
                //Search Types Access
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    AppConfig.APP_SrchTypesAccess[i] = checkedListBox1.GetItemChecked(i);
                AppConfig.APP_SrchTypesAccess[comboBox2.SelectedIndex] = true;

                //#PATH
                //Folder of Exchange
                AppConfig.Path_Exchnage = phExTbBox.Text;
                //Folder of Cheques
                AppConfig.Path_Cheques = phCheqTbBox.Text;
                //Folder of Bills
                AppConfig.Path_Bills = phBillTbBox.Text;
                //Folder of Articles
                AppConfig.Path_Articles = phArtTBox.Text;

                //#ADDITIONAL
                //Timeout request of refresh
                AppConfig.APP_RefreshTimeout = Convert.ToInt32(numericUpDown6.Value);
                //ChequeName
                AppConfig.APP_ChequeName = maskedTextBox1.Text;
                //Invent save delay
                AppConfig.APP_InvAutoSave = Convert.ToByte(numericUpDown3.Value);
                //Language
                AppConfig.APP_Language = comboBox5.Text.ToString();
                //Total digits after point of money
                AppConfig.APP_MoneyDecimals = Convert.ToByte(numericUpDown2.Value);
                //Total digits after point of dose
                AppConfig.APP_DoseDecimals = Convert.ToByte(numericUpDown4.Value);
                //clear temp on exit
                AppConfig.APP_ClearTEMPonExit = checkBox6.Checked;
                //Invent window
                AppConfig.APP_ShowInventWindow = checkBox4.Checked;
                //Show info on indicator
                AppConfig.APP_ShowInfoOnIndicator = checkBox7.Checked;
                //One copy
                AppConfig.APP_AllowOneCopy = checkBox5.Checked;
                // scanner
                AppConfig.APP_ScannerUseProgamMode = scaner_cBox_useProgramMode.Checked;
                AppConfig.APP_ScannerCharReadFrequency = (int)scanner_char_frequency.Value;
                AppConfig.APP_BuyerBarCodeSource = system_cBox_buyerBCReader.SelectedIndex;
                AppConfig.APP_BuyerBarCodeMinLen = (int)system_num_buyerBCMinLen.Value;

                //Rounding start dose
                AppConfig.APP_StartTotal = Math.Round(AppConfig.APP_StartTotal, AppConfig.APP_DoseDecimals, MidpointRounding.AwayFromZero);

                // PAGE: Content
                // -TabControl: Main
                // --TabPage: Common
                AppConfig.Content_Common_PromptMsgOnIllegal = this.content_general_checkBox_showPromptMsgOnIllegal.Checked;
                // --TabPage: Cheques
                // Add total
                if (content_order_radioButton_addQuantity1.Checked)
                    AppConfig.Content_Cheques_AddTotal = "none";
                if (content_order_radioButton_addQuantity2.Checked)
                    AppConfig.Content_Cheques_AddTotal = "type1";
                if (content_order_radioButton_addQuantity3.Checked)
                    AppConfig.Content_Cheques_AddTotal = "type2";
                // --TabPage: Bills
                AppConfig.Content_Bills_KeepAliveAfterCheque = this.checkBox_pBills_DeleteBillAtEnd.Checked;
                // --TabPage: Articles
                AppConfig.Content_Articles_KeepDataAfterImport = this.checkBox_pArticles_KeepOriginData.Checked;

                /*
                if (Program.MainArgs.ContainsKey("-c") && Program.MainArgs["-c"] != null)
                    AppConfig.SaveData(Program.MainArgs["-c"].ToString());
                else*/
                    AppConfig.SaveData();
            }
            catch { return; }
            DialogResult = DialogResult.OK;
        }

        //Methods
        private void UpdateTaxValues()
        {
            int j = 0;
            char[] taxs = new char[dataGridView2.RowCount];
            ((DataGridViewComboBoxColumn)dataGridView1.Columns["pd"]).Items.Clear();
            for (i = 0; i < taxs.Length; i++)
                try
                {
                    taxs[i] = char.Parse(dataGridView2["taxChar", i].Value.ToString());
                    ((DataGridViewComboBoxColumn)dataGridView1.Columns["pd"]).Items.Add(taxs[i]);
                }
                catch { taxs[i] = ' '; }

            for (i = 0; i < dataGridView1.RowCount; i++)
            {
                ((DataGridViewComboBoxCell)dataGridView1["pd", i]).Items.Clear();

                if (taxs.Length != 0)
                {

                    for (j = 0; j < taxs.Length; j++)
                        ((DataGridViewComboBoxCell)dataGridView1["pd", i]).Items.Add(taxs[j]);

                    if (!((DataGridViewComboBoxCell)dataGridView1["pd", i]).Items.Contains(dataGridView1["pd", i].Value))
                        dataGridView1["pd", i].Value = taxs[0];
                }
                else
                    dataGridView1["pd", i].Value = '\0';
            }
        }



    }
}