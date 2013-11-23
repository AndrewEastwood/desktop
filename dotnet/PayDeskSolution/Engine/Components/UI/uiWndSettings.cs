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
//0using mdcore;
//0using mdcore.Config;
//0using ;
using System.Collections;
using driver.Config;
using driver.Lib;
using driver.Common;

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
            // profile tax
            //tax_profile_groupBox.Visible = tax_profile_bottomSpacer.Visible = ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles;

            //#COMMON
            //PayDesk
            sys_gen_kasaNom.Value = ConfigManager.Instance.CommonConfiguration.APP_PayDesk;
            //RefreshRate
            system_common_numericUpDown_refreshRate.Value = ConfigManager.Instance.CommonConfiguration.APP_RefreshRate / 1000;
            //SubUnit
            sys_gen_pidrozd.Value = ConfigManager.Instance.CommonConfiguration.APP_SubUnit;
            //DefaultCountOfArticle
            system_common_textBox_startupQuantity.Text = ConfigManager.Instance.CommonConfiguration.APP_StartTotal.ToString();
            //SubUnitName
            system_common_textBox_subunitName.Text = ConfigManager.Instance.CommonConfiguration.APP_SubUnitName;
            //Customer's ID  
            system_common_textBox_defaultcustomerID.Text = ConfigManager.Instance.CommonConfiguration.APP_ClientID;
            //WeightType
            system_common_comboBox_weightType.SelectedIndex = ConfigManager.Instance.CommonConfiguration.APP_WeightType;
            //DefaultSearchType
            system_common_comboBox_searchType.SelectedIndex = ConfigManager.Instance.CommonConfiguration.APP_SearchType;
            //Search Types Access
            // --> realized in Event (comboBox2_SelctedIndexChanged)

            //#PATH
            this.System_Path_txbox_folderArticles.Text = ConfigManager.Instance.CommonConfiguration.Path_Articles;
            this.System_Path_txbox_folderCheques.Text = ConfigManager.Instance.CommonConfiguration.Path_Cheques;
            this.System_Path_txbox_folderTemp.Text = ConfigManager.Instance.CommonConfiguration.Path_Temp;
            this.System_Path_txbox_folderBills.Text = ConfigManager.Instance.CommonConfiguration.Path_Bills;
            this.System_Path_txbox_folderUserSchemas.Text = ConfigManager.Instance.CommonConfiguration.Path_Schemes;
            // this.System_Path_txbox_folderExchange.Text = ConfigManager.Instance.CommonConfiguration.Path_Exchnage;
            this.System_Path_txbox_folderUsers.Text = ConfigManager.Instance.CommonConfiguration.Path_Users;
            this.System_Path_txbox_folderTemplates.Text = ConfigManager.Instance.CommonConfiguration.Path_Templates;
            this.System_Path_txbox_folderPlugins.Text = ConfigManager.Instance.CommonConfiguration.Path_Plugins;
            this.System_Path_txbox_folderReports.Text = ConfigManager.Instance.CommonConfiguration.Path_Reports;

            //#ADDITIONAL
            //Timeout request of refresh
            numericUpDown6.Value = ConfigManager.Instance.CommonConfiguration.APP_RefreshTimeout;
            //ChequeName
            maskedTextBox1.Text = ConfigManager.Instance.CommonConfiguration.APP_ChequeName;
            //Invent auto save
            numericUpDown3.Value = ConfigManager.Instance.CommonConfiguration.APP_InvAutoSave;
            //Language
            comboBox5.SelectedItem = ConfigManager.Instance.CommonConfiguration.APP_Language;
            //Total digits after point of money
            numericUpDown2.Value = ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals;
            //Total digits after point of dose
            numericUpDown4.Value = ConfigManager.Instance.CommonConfiguration.APP_DoseDecimals;
            //Clear temp on exit
            checkBox6.Checked = ConfigManager.Instance.CommonConfiguration.APP_ClearTEMPonExit;
            //Show info on indicator
            checkBox7.Checked = ConfigManager.Instance.CommonConfiguration.APP_ShowInfoOnIndicator;
            //Invent window
            checkBox4.Checked = ConfigManager.Instance.CommonConfiguration.APP_ShowInventWindow;
            //One copy
            checkBox5.Checked = ConfigManager.Instance.CommonConfiguration.APP_AllowOneCopy;
            //Barcode
            scaner_cBox_useProgramMode.Checked = ConfigManager.Instance.CommonConfiguration.APP_ScannerUseProgamMode;
            scanner_char_frequency.Value = (decimal)ConfigManager.Instance.CommonConfiguration.APP_ScannerCharReadFrequency;
            system_cBox_buyerBCReader.SelectedIndex = ConfigManager.Instance.CommonConfiguration.APP_BuyerBarCodeSource;
            system_num_buyerBCMinLen.Value = (decimal)ConfigManager.Instance.CommonConfiguration.APP_BuyerBarCodeMinLen;

            // PAGE: Content
            // -TabControl: Main
            // --TabPage: Common
            this.content_general_checkBox_showPromptMsgOnIllegal.Checked = ConfigManager.Instance.CommonConfiguration.Content_Common_PromptMsgOnIllegal;
            this.content_general_numeric_printDelay.Value = ConfigManager.Instance.CommonConfiguration.Content_Common_PrinterDelaySec;
            // --TabPage: Cheques
            // Add total
            switch (ConfigManager.Instance.CommonConfiguration.Content_Cheques_AddTotal)
            {
                case "none": this.content_order_radioButton_addQuantity1.Checked = true; break;
                case "type1": this.content_order_radioButton_addQuantity2.Checked = true; break;
                case "type2": this.content_order_radioButton_addQuantity3.Checked = true; break;
            }
            // split articles to different cheques
            // removed this.content_chq_chBox_useSeparateCheque.Checked = ConfigManager.Instance.CommonConfiguration.Content_Cheques_UseSeparateCheque;
            // removed this.content_chq_textBox_separatedArticleMaskById.Text = ConfigManager.Instance.CommonConfiguration.Content_Cheques_SeparatedArticleMaskById;
            // use custom client's card
            this.content_chq_chBox_useCustomClientCardBC.Checked = ConfigManager.Instance.CommonConfiguration.Content_Cheques_UseCustomClientCardBC;
            this.content_chq_textBox_customClientCardBC.Text = ConfigManager.Instance.CommonConfiguration.Content_Cheques_CustomClientCardBC;
            // use secure backup
            this.content_chq_chBox_addCopyToArchive.Checked = ConfigManager.Instance.CommonConfiguration.Content_Cheques_AddCopyToArchive;
            //Client Card
            this.content_chq_textBox_customClientCardBC.Text = ConfigManager.Instance.CommonConfiguration.Content_Cheques_CustomClientCardBC;
            // --TabPage: Bills
            this.checkBox_pBills_DeleteBillAtEnd.Checked = ConfigManager.Instance.CommonConfiguration.Content_Bills_KeepAliveAfterCheque;
            this.content_bill_chBox_addCopyToArchive.Checked = ConfigManager.Instance.CommonConfiguration.Content_Bills_AddCopyToArchive;
            this.checkBox_pBills_ShowBillSumColumn.Checked = ConfigManager.Instance.CommonConfiguration.Content_Bills_ShowBillSumColumn;
            this.checkBox_pBills_ShowBillTotalSum.Checked = ConfigManager.Instance.CommonConfiguration.Content_Bills_ShowBillTotalSum;
            // --TabPage: Articles
            this.checkBox_pArticles_KeepOriginData.Checked = ConfigManager.Instance.CommonConfiguration.Content_Articles_KeepDataAfterImport;

            // Profiles
            this.profiles_chq_chBox_useProfiles.Checked = ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles;

            Hashtable ht = new Hashtable();
            if (ConfigManager.Instance.CommonConfiguration.PROFILES_Items == null)
                ConfigManager.Instance.CommonConfiguration.PROFILES_Items = new Hashtable();
            Hashtable profiles = new Hashtable();
            foreach (DictionaryEntry di in ConfigManager.Instance.CommonConfiguration.PROFILES_Items)
            {
                try
                {
                    ht = (Hashtable)di.Value;

                    // data fill
                    wndSettings.uc_firmEntry fe = new wndSettings.uc_firmEntry(di.Key, ht);

                    // style
                    fe.BackColor = Color.White;
                    fe.Dock = DockStyle.Fill;

                    // adding new tab
                    TabPage tp = new TabPage();
                    tp.ImageIndex = 0;
                    tp.Controls.Add(fe);
                    profiles_tab_profiles.TabPages.Add(tp);
                    profiles_tab_profiles.SelectedTab = tp;

                    profiles[di.Key] = ht["NAME"];
                }
                catch { }
            }
            profiles_tab_profiles.Visible = !(profiles_tab_profiles.TabPages.Count == 0);
            // profiles legal item
            //profiles["-"] = "-";
            BindingSource bs = new BindingSource();
            BindingSource bs_legal = new BindingSource();
            profiles[CoreConst.KEY_DEFAULT_PROFILE_ID] = "-";
            bs.DataSource = profiles;
            bs_legal.DataSource = profiles;
            this.tax_profile_cBox.DataSource = bs;
            this.tax_profile_cBox.DisplayMember = "Value";

            this.profiles_cBox_legalProfile.DataSource = bs_legal;
            this.profiles_cBox_legalProfile.DisplayMember = "Value";
            try
            {
                foreach (DictionaryEntry di in this.profiles_cBox_legalProfile.Items)
                {
                    if (ConfigManager.Instance.CommonConfiguration.PROFILES_LegalProgileID.Equals(di.Key))
                    {
                        this.profiles_cBox_legalProfile.SelectedItem = di;
                        break;
                    }
                }
            }
            catch { }
            
            // tax

            this.tax_profile_cBox.SelectedIndex = 0;
            // Settings_Tax_Grid(CoreConst.KEY_DEFAULT_PROFILE_ID);
            
            //Navigator
            treeView1.Select();
            treeView1.SelectedNode = treeView1.Nodes[0];
        }

        //Navigator
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            panelProfiles.Visible = false;
            PanelPDV.Visible = false;
            PanelSystem.Visible = false;
            PanelStyle.Visible = false;
            panelContent.Visible = false;

            switch (e.Node.Level * 10 + e.Node.Index)
            {
                case 00: { PanelPDV.Visible = true; break; }
                case 01: { PanelSystem.Visible = true; break; }
                case 02: { panelProfiles.Visible = true; break; }
                case 03: { PanelStyle.Visible = true; break; }
                case 04: { panelContent.Visible = true; break; }
            }
        }     

        //Pages
        #region Taxs Rates
        private void button12_Click(object sender, EventArgs e)
        {
            dataGridView_tax_userdefined.Rows.Add(new object[] { "", "0.0", true });
            dataGridView_tax_userdefined.Rows[dataGridView_tax_userdefined.RowCount - 1].Selected = true;
            dataGridView_tax_userdefined.CurrentCell = dataGridView_tax_userdefined["taxChar", dataGridView_tax_userdefined.RowCount - 1];
            dataGridView_tax_userdefined.BeginEdit(true);
        }//ADD TAX
        private void button14_Click(object sender, EventArgs e)
        {
            if (dataGridView_tax_userdefined.CurrentRow != null)
                dataGridView_tax_userdefined.Rows.Remove(dataGridView_tax_userdefined.CurrentRow);
            UpdateTaxValues();
        }//Delete TAX
        private void dataGridView2_CellEndEdit(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            if (dataGridView_tax_userdefined[e.ColumnIndex, e.RowIndex].Value.ToString() == "")
                dataGridView_tax_userdefined.BeginEdit(true);
            else
                UpdateTaxValues();
        }
        private void tax_profile_cBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string pk = CoreConst.KEY_DEFAULT_PROFILE_ID;
            try
            {
                pk = ((DictionaryEntry)tax_profile_cBox.SelectedItem).Key.ToString();
            }
            catch { }
            Settings_Tax_Grid(pk);
        }
        
        #endregion

        #region System
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Search Types Access
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
                checkedListBox1.SetItemChecked(i, ConfigManager.Instance.CommonConfiguration.APP_SrchTypesAccess[i]);
            checkedListBox1.SetItemChecked(system_common_comboBox_searchType.SelectedIndex, true);
        }
        #endregion

        #region Style
        //General
        private void ColorList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (Colors_ListBox.IndexFromPoint(e.Location) >= 0)
            {
                switch (Colors_ListBox.SelectedIndex)
                {
                    case 0: colorDialog1.Color = ConfigManager.Instance.CommonConfiguration.STYLE_BackgroundInfPan; break;
                    case 1: colorDialog1.Color = ConfigManager.Instance.CommonConfiguration.STYLE_BackgroundAddPan; break;
                    case 2: colorDialog1.Color = ConfigManager.Instance.CommonConfiguration.STYLE_BackgroundSumRest; break;
                    case 3: colorDialog1.Color = ConfigManager.Instance.CommonConfiguration.STYLE_BackgroundAChqTbl; break;
                    case 4: colorDialog1.Color = ConfigManager.Instance.CommonConfiguration.STYLE_BackgroundNAChqTbl; break;
                    case 5: colorDialog1.Color = ConfigManager.Instance.CommonConfiguration.STYLE_BackgroundArtTbl; break;
                    case 6: colorDialog1.Color = ConfigManager.Instance.CommonConfiguration.STYLE_BackgroundStatPan; break;
                }
                colorDialog1.ShowDialog();
                switch (Colors_ListBox.SelectedIndex)
                {
                    case 0: ConfigManager.Instance.CommonConfiguration.STYLE_BackgroundInfPan = colorDialog1.Color; break;
                    case 1: ConfigManager.Instance.CommonConfiguration.STYLE_BackgroundAddPan = colorDialog1.Color; break;
                    case 2: ConfigManager.Instance.CommonConfiguration.STYLE_BackgroundSumRest = colorDialog1.Color; break;
                    case 3: ConfigManager.Instance.CommonConfiguration.STYLE_BackgroundAChqTbl = colorDialog1.Color; break;
                    case 4: ConfigManager.Instance.CommonConfiguration.STYLE_BackgroundNAChqTbl = colorDialog1.Color; break;
                    case 5: ConfigManager.Instance.CommonConfiguration.STYLE_BackgroundArtTbl = colorDialog1.Color; break;
                    case 6: ConfigManager.Instance.CommonConfiguration.STYLE_BackgroundStatPan = colorDialog1.Color; break;
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
                        fontDialog1.Font = ConfigManager.Instance.CommonConfiguration.STYLE_SumFont;
                        fontDialog1.Color = ConfigManager.Instance.CommonConfiguration.STYLE_SumFontColor; 
                        break;
                    case 1:
                        fontDialog1.Font = ConfigManager.Instance.CommonConfiguration.STYLE_RestFont;
                        fontDialog1.Color = ConfigManager.Instance.CommonConfiguration.STYLE_RestFontColor;
                        break;
                    case 2:
                        fontDialog1.Font = ConfigManager.Instance.CommonConfiguration.STYLE_ArticlesFont;
                        fontDialog1.Color = ConfigManager.Instance.CommonConfiguration.STYLE_ArticlesFontColor;
                        break;
                    case 3:
                        fontDialog1.Font = ConfigManager.Instance.CommonConfiguration.STYLE_ChequeFont;
                        fontDialog1.Color = ConfigManager.Instance.CommonConfiguration.STYLE_ChequeFontColor;
                        break;
                    case 4:
                        fontDialog1.Font = ConfigManager.Instance.CommonConfiguration.STYLE_StatusFont;
                        fontDialog1.Color = ConfigManager.Instance.CommonConfiguration.STYLE_StatusFontColor;
                        break;
                    case 5:
                        fontDialog1.Font = ConfigManager.Instance.CommonConfiguration.STYLE_AddInformerFont;
                        fontDialog1.Color = ConfigManager.Instance.CommonConfiguration.STYLE_AddInformerFontColor;
                        break;
                    case 6:
                        fontDialog1.Font = ConfigManager.Instance.CommonConfiguration.STYLE_ChqInformerFont;
                        fontDialog1.Color = ConfigManager.Instance.CommonConfiguration.STYLE_ChqInformerFontColor;
                        break;
                    case 7:
                        fontDialog1.Font = ConfigManager.Instance.CommonConfiguration.STYLE_AppInformerFont;
                        fontDialog1.Color = ConfigManager.Instance.CommonConfiguration.STYLE_AppInformerFontColor;
                        break;
                    case 8:
                        fontDialog1.Font = ConfigManager.Instance.CommonConfiguration.STYLE_BillWindow;
                        break;
                    case 9:
                        fontDialog1.Font = ConfigManager.Instance.CommonConfiguration.STYLE_BillWindowEntry;
                        break;
                    case 10:
                        fontDialog1.Font = ConfigManager.Instance.CommonConfiguration.STYLE_BillWindowEntryItems;
                        break;
                    case 11:
                        fontDialog1.Font = ConfigManager.Instance.CommonConfiguration.skin_sensor_fontsize;
                        break;
                }
                fontDialog1.ShowDialog();
                switch (Fonts_ListBox.SelectedIndex)
                {
                    case 0:
                        ConfigManager.Instance.CommonConfiguration.STYLE_SumFont = fontDialog1.Font;
                        ConfigManager.Instance.CommonConfiguration.STYLE_SumFontColor = fontDialog1.Color;
                        break;
                    case 1:
                        ConfigManager.Instance.CommonConfiguration.STYLE_RestFont = fontDialog1.Font;
                        ConfigManager.Instance.CommonConfiguration.STYLE_RestFontColor = fontDialog1.Color;
                        break;
                    case 2:
                        ConfigManager.Instance.CommonConfiguration.STYLE_ArticlesFont = fontDialog1.Font;
                        ConfigManager.Instance.CommonConfiguration.STYLE_ArticlesFontColor = fontDialog1.Color;
                        break;
                    case 3:
                        ConfigManager.Instance.CommonConfiguration.STYLE_ChequeFont = fontDialog1.Font;
                        ConfigManager.Instance.CommonConfiguration.STYLE_ChequeFontColor = fontDialog1.Color;
                        break;
                    case 4:
                        ConfigManager.Instance.CommonConfiguration.STYLE_StatusFont = fontDialog1.Font;
                        ConfigManager.Instance.CommonConfiguration.STYLE_StatusFontColor = fontDialog1.Color;
                        break;
                    case 5:
                        ConfigManager.Instance.CommonConfiguration.STYLE_AddInformerFont = fontDialog1.Font;
                        ConfigManager.Instance.CommonConfiguration.STYLE_AddInformerFontColor = fontDialog1.Color;
                        break;
                    case 6:
                        ConfigManager.Instance.CommonConfiguration.STYLE_ChqInformerFont = fontDialog1.Font;
                        ConfigManager.Instance.CommonConfiguration.STYLE_ChqInformerFontColor = fontDialog1.Color;
                        break;
                    case 7:
                        ConfigManager.Instance.CommonConfiguration.STYLE_AppInformerFont = fontDialog1.Font;
                        ConfigManager.Instance.CommonConfiguration.STYLE_AppInformerFontColor = fontDialog1.Color;
                        break;
                    case 8:
                        ConfigManager.Instance.CommonConfiguration.STYLE_BillWindow = fontDialog1.Font;
                        break;
                    case 9:
                        ConfigManager.Instance.CommonConfiguration.STYLE_BillWindowEntry = fontDialog1.Font;
                        break;
                    case 10:
                        ConfigManager.Instance.CommonConfiguration.STYLE_BillWindowEntryItems = fontDialog1.Font;
                        break;
                    case 11:
                        ConfigManager.Instance.CommonConfiguration.skin_sensor_fontsize = fontDialog1.Font;
                        break;
                }
            }
        }
        private void Misc_ListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            wndSettings.uiWndSettingsNumberRequest bR = new wndSettings.uiWndSettingsNumberRequest();
            //wndSettings.uiWndSettingsValueSelector vS = new wndSettings.uiWndSettingsValueSelector();
            
            if (Misc_ListBox.IndexFromPoint(e.Location) >= 0)
            {
                switch (Misc_ListBox.SelectedIndex)
                {
                    case 0: bR.Value = ConfigManager.Instance.CommonConfiguration.STYLE_Misc_ChequeRowHeight; break;
                    case 1: bR.Value = ConfigManager.Instance.CommonConfiguration.STYLE_Misc_ArticleRowHeight; break;
                    case 2: bR.Value = ConfigManager.Instance.CommonConfiguration.STYLE_Misc_BillItemsRowHeight; break;
                    case 3: bR.Value = ConfigManager.Instance.CommonConfiguration.STYLE_Misc_BillItemProductsRowHeight; break;
                    case 4:
                        //vS.addValues(new object[] { 50, 100 });
                        bR.Value = ConfigManager.Instance.CommonConfiguration.skin_sensor_com_size_cheque; break;
                    case 5:
                        //vS.addValues(new object[] { 50, 100 });
                        bR.Value = ConfigManager.Instance.CommonConfiguration.skin_sensor_com_size_art; break;
                }
                if (bR.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    switch (Misc_ListBox.SelectedIndex)
                    {
                        case 0: ConfigManager.Instance.CommonConfiguration.STYLE_Misc_ChequeRowHeight = bR.Value; break;
                        case 1: ConfigManager.Instance.CommonConfiguration.STYLE_Misc_ArticleRowHeight = bR.Value; break;
                        case 2: ConfigManager.Instance.CommonConfiguration.STYLE_Misc_BillItemsRowHeight = bR.Value; break;
                        case 3: ConfigManager.Instance.CommonConfiguration.STYLE_Misc_BillItemProductsRowHeight = bR.Value; break;
                        case 4: ConfigManager.Instance.CommonConfiguration.skin_sensor_com_size_cheque = bR.Value; break;
                        case 5: ConfigManager.Instance.CommonConfiguration.skin_sensor_com_size_art = bR.Value; break;
                    }

            }

            bR.Dispose();
        }
        #endregion

        // Content
        /*
         * General Event Handlers
         */
        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            switch (((Control)sender).Name)
            {
                case "content_chq_chBox_useSeparateCheque":
                    {
                        this.content_chq_textBox_separatedArticleMaskById.Enabled = this.content_chq_chBox_useSeparateCheque.Checked;
                        break;
                    }
                case "content_chq_chBox_useCustomClientCardBC":
                    {
                        this.content_chq_textBox_customClientCardBC.Enabled = this.content_chq_chBox_useCustomClientCardBC.Checked;
                        break;
                    }
                case "profiles_chq_chBox_useProfiles":
                    {
                        // profile tax
                        //tax_profile_groupBox.Visible = tax_profile_bottomSpacer.Visible = this.profiles_chq_chBox_useProfiles.Checked;
                        break;
                    }
            }
        }
        private void button_Click(object sender, EventArgs e)
        {
            Control linkedCtrl = new Control();
            switch (((Control)sender).Name)
            {
                case "System_Path_btn_folderArticlesBrowse":
                    {
                        folderBrowserDialog1.Description = "Вкажіть шлях до каталогу товарів";
                        linkedCtrl = this.System_Path_txbox_folderArticles;
                        break;
                    }
                case "System_Path_btn_folderChequesBrowse":
                    {
                        folderBrowserDialog1.Description = "Вкажіть шлях до каталогу чеків";
                        linkedCtrl = this.System_Path_txbox_folderCheques;
                        break;
                    }
                case "System_Path_btn_folderTempBrowse":
                    {
                        folderBrowserDialog1.Description = "Вкажіть шлях до тимчасової папки";
                        linkedCtrl = this.System_Path_txbox_folderTemp;
                        break;
                    }
                case "System_Path_btn_folderBillsBrowse":
                    {
                        folderBrowserDialog1.Description = "Вкажіть шлях до каталогу рахунків";
                        linkedCtrl = this.System_Path_txbox_folderBills;
                        break;
                    }
                case "System_Path_btn_folderUserSchemasBrowse":
                    {
                        folderBrowserDialog1.Description = "Вкажіть шлях до каталогу схем касирів";
                        linkedCtrl = this.System_Path_txbox_folderUserSchemas;
                        break;
                    }
                //case "System_Path_btn_folderExchangeBrowse":
                //    {
                //        folderBrowserDialog1.Description = "Вкажіть шлях до каталогу обміну";
                //        linkedCtrl = this.System_Path_txbox_folderExchange;
                //        break;
                //    }
                case "System_Path_btn_folderUsersBrowse":
                    {
                        folderBrowserDialog1.Description = "Вкажіть шлях до каталогу користувачів";
                        linkedCtrl = this.System_Path_txbox_folderUsers;
                        break;
                    }
                case "System_Path_btn_folderTemplatesBrowse":
                    {
                        folderBrowserDialog1.Description = "Вкажіть шлях до каталогу шаблонів";
                        linkedCtrl = this.System_Path_txbox_folderTemplates;
                        break;
                    }
                case "System_Path_btn_folderPluginsBrowse":
                    {
                        folderBrowserDialog1.Description = "Вкажіть шлях до каталогу додатків";
                        linkedCtrl = this.System_Path_txbox_folderPlugins;
                        break;
                    }
                case "System_Path_btn_folderReportsBrowse":
                    {
                        folderBrowserDialog1.Description = "Вкажіть шлях до каталогу звітів";
                        linkedCtrl = this.System_Path_txbox_folderReports;
                        break;
                    }
                    // Profiles
                case "profiles_btn_create":
                    {
                        TabPage tp = new TabPage();
                        tp.ImageIndex = 0;
                        wndSettings.uc_firmEntry fe = new wndSettings.uc_firmEntry();
                        fe.BackColor = Color.White;
                        fe.Dock = DockStyle.Fill;
                        tp.Controls.Add(fe);
                        profiles_tab_profiles.TabPages.Add(tp);
                        profiles_tab_profiles.SelectedTab = tp;

                        profiles_tab_profiles.Visible = !(profiles_tab_profiles.TabPages.Count == 0);
                        break;
                    }
                case "profiles_btn_refresh":
                    {
                        Hashtable profiles = new Hashtable();
                        BindingSource bs = new BindingSource();

                        for (int i = 0; i < profiles_tab_profiles.TabPages.Count; i++)
                        {
                            wndSettings.uc_firmEntry fe = (wndSettings.uc_firmEntry)profiles_tab_profiles.TabPages[i].Controls[0];
                            profiles[fe.Profile_ID] = fe.Profile_Name;
                        }

                        profiles[CoreConst.KEY_DEFAULT_PROFILE_ID] = "-";
                        bs.DataSource = profiles;
                        this.profiles_cBox_legalProfile.DataSource = bs;
                        this.profiles_cBox_legalProfile.DisplayMember = "Value";

                        break;
                    }
                case "tax_profiles_btn_refresh":
                    {
                        Hashtable profiles = new Hashtable();
                        BindingSource bs = new BindingSource();

                        for (int i = 0; i < profiles_tab_profiles.TabPages.Count; i++)
                        {
                            wndSettings.uc_firmEntry fe = (wndSettings.uc_firmEntry)profiles_tab_profiles.TabPages[i].Controls[0];
                            profiles[fe.Profile_ID] = fe.Profile_Name;
                        }

                        profiles[CoreConst.KEY_DEFAULT_PROFILE_ID] = "-";
                        bs.DataSource = profiles;
                        this.tax_profile_cBox.DataSource = bs;
                        this.tax_profile_cBox.DisplayMember = "Value";

                        break;
                    }
                case "tax_profile_btnSave":
                    {
                        //object pId = this.tax_profile_cBox.SelectedValue;

                        string pk = CoreConst.KEY_DEFAULT_PROFILE_ID;
                        try
                        {
                            pk = ((DictionaryEntry)tax_profile_cBox.SelectedItem).Key.ToString();
                        }
                        catch { }

                        Settings_Tax_Grid_Save(pk);


                        /*
                        Hashtable compatibleTaxGrid = ConfigManager.Instance.CommonConfiguration.TAX_Compatibility_Template;
                        Hashtable userTaxDefined = new Hashtable();

                        // fill tax compatibility with selected values
                        for (int i = 0; i < dataGridView_tax_compatibility.RowCount; i++)
                            compatibleTaxGrid[dataGridView_tax_compatibility["mkt", i].Value] = dataGridView_tax_compatibility["pd", i].Value;

                        // user defined tax
                        for (int i = 0; i < dataGridView_tax_userdefined.RowCount; i++)
                            userTaxDefined[dataGridView_tax_userdefined["taxChar", i].Value] = string.Format("{0};{1}", dataGridView_tax_userdefined["taxRate",i].Value, dataGridView_tax_userdefined["taxDisc",i].Value);

                        // saving
                        ConfigManager.Instance.CommonConfiguration.TAX_Compatibility[pId] = compatibleTaxGrid;

                        */

                        break;
                    }
            }

            try
            {
                folderBrowserDialog1.SelectedPath = ((TextBox)linkedCtrl).Text;
                folderBrowserDialog1.ShowDialog();
                ((TextBox)linkedCtrl).Text = folderBrowserDialog1.SelectedPath;
            }
            catch { }
        }

        private void Settings_KeyDown(object sender, KeyEventArgs e)
        {
            KeyEventArgs esc = new KeyEventArgs(Keys.Escape);

            if (e.KeyValue == esc.KeyValue)
            {
                Close();
                return;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                /*
                //#TAX
                //SaveAppTaxRates(dataGridView2);
                char[] tChars = new char[0];
                double[] tRates = new double[0];
                bool[] tDisc = new bool[0];

                char curr_tChar = ' ';
                double curr_tRate = 0.0;
                bool curr_tDisc = true;

                bool error = false;

                for (i = 0; i < dataGridView_tax_userdefined.RowCount; i++)
                {
                    try
                    {
                        curr_tChar = dataGridView_tax_userdefined.Rows[i].Cells["taxChar"].Value.ToString()[0];
                        curr_tRate = MathLib.GetDouble(dataGridView_tax_userdefined.Rows[i].Cells["taxRate"].Value.ToString());
                        curr_tDisc = Convert.ToBoolean(dataGridView_tax_userdefined.Rows[i].Cells["taxDisc"].Value);

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
                    ConfigManager.Instance.CommonConfiguration.TAX_AppTaxChar = tChars;
                    ConfigManager.Instance.CommonConfiguration.TAX_AppTaxRates = tRates;
                    ConfigManager.Instance.CommonConfiguration.TAX_AppTaxDisc = tDisc;
                }

                //SaveComparableTaxTable(dataGridView1);
                if (ConfigManager.Instance.CommonConfiguration.TAX_AppColumn == null)
                    ConfigManager.Instance.CommonConfiguration.TAX_AppColumn = new char[dataGridView_tax_compatibility.RowCount];

                for (i = 0; i < dataGridView_tax_compatibility.RowCount; i++)
                    ConfigManager.Instance.CommonConfiguration.TAX_AppColumn[i] = char.Parse(dataGridView_tax_compatibility.Rows[i].Cells[0].Value.ToString());

                */



                //#COMMON
                //PayDesk
                ConfigManager.Instance.CommonConfiguration.APP_PayDesk = Convert.ToByte(sys_gen_kasaNom.Value);
                //RefreshRate
                ConfigManager.Instance.CommonConfiguration.APP_RefreshRate = Convert.ToInt32(system_common_numericUpDown_refreshRate.Value) * 1000;
                //SubUnit
                ConfigManager.Instance.CommonConfiguration.APP_SubUnit = Convert.ToByte(sys_gen_pidrozd.Value);
                //DefaultCountOfArticle
                ConfigManager.Instance.CommonConfiguration.APP_StartTotal = Convert.ToDouble(system_common_textBox_startupQuantity.Text);
                //SubUnitName
                ConfigManager.Instance.CommonConfiguration.APP_SubUnitName = system_common_textBox_subunitName.Text;
                //Customer's ID  
                ConfigManager.Instance.CommonConfiguration.APP_ClientID = system_common_textBox_defaultcustomerID.Text;
                //WeightType
                ConfigManager.Instance.CommonConfiguration.APP_WeightType = system_common_comboBox_weightType.SelectedIndex;
                //DefaultSearchType
                ConfigManager.Instance.CommonConfiguration.APP_SearchType = system_common_comboBox_searchType.SelectedIndex;
                //Search Types Access
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    ConfigManager.Instance.CommonConfiguration.APP_SrchTypesAccess[i] = checkedListBox1.GetItemChecked(i);
                ConfigManager.Instance.CommonConfiguration.APP_SrchTypesAccess[system_common_comboBox_searchType.SelectedIndex] = true;

                //#PATH
                ConfigManager.Instance.CommonConfiguration.Path_Articles = this.System_Path_txbox_folderArticles.Text;
                ConfigManager.Instance.CommonConfiguration.Path_Cheques = this.System_Path_txbox_folderCheques.Text;
                ConfigManager.Instance.CommonConfiguration.Path_Temp = this.System_Path_txbox_folderTemp.Text;
                ConfigManager.Instance.CommonConfiguration.Path_Bills = this.System_Path_txbox_folderBills.Text;
                ConfigManager.Instance.CommonConfiguration.Path_Schemes = this.System_Path_txbox_folderUserSchemas.Text;
                //ConfigManager.Instance.CommonConfiguration.Path_Exchnage = this.System_Path_txbox_folderExchange.Text;
                ConfigManager.Instance.CommonConfiguration.Path_Users = this.System_Path_txbox_folderUsers.Text;
                ConfigManager.Instance.CommonConfiguration.Path_Templates = this.System_Path_txbox_folderTemplates.Text;
                ConfigManager.Instance.CommonConfiguration.Path_Plugins = this.System_Path_txbox_folderPlugins.Text;
                ConfigManager.Instance.CommonConfiguration.Path_Reports = this.System_Path_txbox_folderReports.Text;

                //#ADDITIONAL
                //Timeout request of refresh
                ConfigManager.Instance.CommonConfiguration.APP_RefreshTimeout = Convert.ToInt32(numericUpDown6.Value);
                //ChequeName
                ConfigManager.Instance.CommonConfiguration.APP_ChequeName = maskedTextBox1.Text;
                //Invent save delay
                ConfigManager.Instance.CommonConfiguration.APP_InvAutoSave = Convert.ToByte(numericUpDown3.Value);
                //Language
                ConfigManager.Instance.CommonConfiguration.APP_Language = comboBox5.Text.ToString();
                //Total digits after point of money
                ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals = Convert.ToByte(numericUpDown2.Value);
                //Total digits after point of dose
                ConfigManager.Instance.CommonConfiguration.APP_DoseDecimals = Convert.ToByte(numericUpDown4.Value);
                //clear temp on exit
                ConfigManager.Instance.CommonConfiguration.APP_ClearTEMPonExit = checkBox6.Checked;
                //Invent window
                ConfigManager.Instance.CommonConfiguration.APP_ShowInventWindow = checkBox4.Checked;
                //Show info on indicator
                ConfigManager.Instance.CommonConfiguration.APP_ShowInfoOnIndicator = checkBox7.Checked;
                //One copy
                ConfigManager.Instance.CommonConfiguration.APP_AllowOneCopy = checkBox5.Checked;

                //Rounding start dose
                ConfigManager.Instance.CommonConfiguration.APP_StartTotal = Math.Round(ConfigManager.Instance.CommonConfiguration.APP_StartTotal, ConfigManager.Instance.CommonConfiguration.APP_DoseDecimals, MidpointRounding.AwayFromZero);
                //Barcode
                ConfigManager.Instance.CommonConfiguration.APP_ScannerUseProgamMode = scaner_cBox_useProgramMode.Checked;
                ConfigManager.Instance.CommonConfiguration.APP_ScannerCharReadFrequency = (int)scanner_char_frequency.Value;
                ConfigManager.Instance.CommonConfiguration.APP_BuyerBarCodeSource = system_cBox_buyerBCReader.SelectedIndex;
                ConfigManager.Instance.CommonConfiguration.APP_BuyerBarCodeMinLen = (int)system_num_buyerBCMinLen.Value;

                // PAGE: Content
                // -TabControl: Main
                // --TabPage: Common
                ConfigManager.Instance.CommonConfiguration.Content_Common_PromptMsgOnIllegal = this.content_general_checkBox_showPromptMsgOnIllegal.Checked;
                ConfigManager.Instance.CommonConfiguration.Content_Common_PrinterDelaySec = (int)this.content_general_numeric_printDelay.Value;
                // Client Card
                ConfigManager.Instance.CommonConfiguration.Content_Cheques_CustomClientCardBC = System_Path_txbox_folderUsers.Text;
                // --TabPage: Cheques
                // removed ConfigManager.Instance.CommonConfiguration.Content_Cheques_UseSeparateCheque = this.content_chq_chBox_useSeparateCheque.Checked;
                // removed ConfigManager.Instance.CommonConfiguration.Content_Cheques_SeparatedArticleMaskById = this.content_chq_textBox_separatedArticleMaskById.Text;
                // Add total
                if (content_order_radioButton_addQuantity1.Checked)
                    ConfigManager.Instance.CommonConfiguration.Content_Cheques_AddTotal = "none";
                if (content_order_radioButton_addQuantity2.Checked)
                    ConfigManager.Instance.CommonConfiguration.Content_Cheques_AddTotal = "type1";
                if (content_order_radioButton_addQuantity3.Checked)
                    ConfigManager.Instance.CommonConfiguration.Content_Cheques_AddTotal = "type2";
                // split articles to different cheques
                ConfigManager.Instance.CommonConfiguration.Content_Cheques_UseSeparateCheque = this.content_chq_chBox_useSeparateCheque.Checked;
                ConfigManager.Instance.CommonConfiguration.Content_Cheques_SeparatedArticleMaskById = this.content_chq_textBox_separatedArticleMaskById.Text;
                // use custom client's card
                ConfigManager.Instance.CommonConfiguration.Content_Cheques_UseCustomClientCardBC = this.content_chq_chBox_useCustomClientCardBC.Checked;
                ConfigManager.Instance.CommonConfiguration.Content_Cheques_CustomClientCardBC = this.content_chq_textBox_customClientCardBC.Text;
                // use secure backup
                ConfigManager.Instance.CommonConfiguration.Content_Cheques_AddCopyToArchive = this.content_chq_chBox_addCopyToArchive.Checked;
                // --TabPage: Bills
                ConfigManager.Instance.CommonConfiguration.Content_Bills_KeepAliveAfterCheque = this.checkBox_pBills_DeleteBillAtEnd.Checked;
                ConfigManager.Instance.CommonConfiguration.Content_Bills_AddCopyToArchive = this.content_bill_chBox_addCopyToArchive.Checked;
                ConfigManager.Instance.CommonConfiguration.Content_Bills_ShowBillSumColumn = this.checkBox_pBills_ShowBillSumColumn.Checked;
                ConfigManager.Instance.CommonConfiguration.Content_Bills_ShowBillTotalSum = this.checkBox_pBills_ShowBillTotalSum.Checked;
                // --TabPage: Articles
                ConfigManager.Instance.CommonConfiguration.Content_Articles_KeepDataAfterImport = this.checkBox_pArticles_KeepOriginData.Checked;


                // Profiles
                ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles = this.profiles_chq_chBox_useProfiles.Checked;
                Hashtable ht = new Hashtable();
                ConfigManager.Instance.CommonConfiguration.PROFILES_Items.Clear();
                //ConfigManager.Instance.CommonConfiguration.PROFILES_updateDateTime.Clear();
                for (int i = 0; i < profiles_tab_profiles.TabPages.Count; i++)
                {
                    wndSettings.uc_firmEntry fe = (wndSettings.uc_firmEntry)profiles_tab_profiles.TabPages[i].Controls[0];
                    ht.Add("NAME", fe.Profile_Name);
                    //ht.Add("SOURCE", fe.Profile_Source);
                    ht.Add("OUTPUT", fe.Profile_Output);
                    ht.Add("FILTER", fe.Profile_Filter);
                    ht.Add("SUBUNIT", fe.Profile_SubUnit);
                    ConfigManager.Instance.CommonConfiguration.PROFILES_Items.Add(fe.Profile_ID, ht);
                    //ConfigManager.Instance.CommonConfiguration.PROFILES_updateDateTime.Add(fe.Profile_ID, new DateTime[3]);
                    ht = new Hashtable();
                }

                ConfigManager.Instance.CommonConfiguration.PROFILES_LegalProgileID = ((DictionaryEntry)this.profiles_cBox_legalProfile.SelectedItem).Key;

                /*
                if (Program.MainArgs.ContainsKey("-c") && Program.MainArgs["-c"] != null)
                    ConfigManager.Instance.CommonConfiguration.SaveData(Program.MainArgs["-c"].ToString());
                else*/


                ConfigManager.SaveConfiguration();
            }
            catch { return; }
            DialogResult = DialogResult.OK;
        }

        //Methods
        private void UpdateTaxValues()
        {
            int j = 0;
            char[] taxs = new char[dataGridView_tax_userdefined.RowCount];
            ((DataGridViewComboBoxColumn)dataGridView_tax_compatibility.Columns["pd"]).Items.Clear();
            for (i = 0; i < taxs.Length; i++)
                try
                {
                    taxs[i] = char.Parse(dataGridView_tax_userdefined["taxChar", i].Value.ToString());
                    ((DataGridViewComboBoxColumn)dataGridView_tax_compatibility.Columns["pd"]).Items.Add(taxs[i]);
                }
                catch { taxs[i] = ' '; }

            for (i = 0; i < dataGridView_tax_compatibility.RowCount; i++)
            {
                ((DataGridViewComboBoxCell)dataGridView_tax_compatibility["pd", i]).Items.Clear();

                if (taxs.Length != 0)
                {

                    for (j = 0; j < taxs.Length; j++)
                        ((DataGridViewComboBoxCell)dataGridView_tax_compatibility["pd", i]).Items.Add(taxs[j]);

                    if (!((DataGridViewComboBoxCell)dataGridView_tax_compatibility["pd", i]).Items.Contains(dataGridView_tax_compatibility["pd", i].Value))
                        dataGridView_tax_compatibility["pd", i].Value = taxs[0];
                }
                else
                    dataGridView_tax_compatibility["pd", i].Value = '\0';
            }
        }
        private void Settings_Tax_Grid(object profile)
        {

            if (ConfigManager.Instance.CommonConfiguration.TAX_Compatibility == null)
                ConfigManager.Instance.CommonConfiguration.TAX_Compatibility = new Hashtable();

            if (ConfigManager.Instance.CommonConfiguration.TAX_DefinedRates == null)
                ConfigManager.Instance.CommonConfiguration.TAX_DefinedRates = new Hashtable();
            
            // check for default
            if (!ConfigManager.Instance.CommonConfiguration.TAX_Compatibility.ContainsKey(profile))
            {
                ConfigManager.Instance.CommonConfiguration.TAX_Compatibility[profile] = ConfigManager.Instance.CommonConfiguration.TAX_Compatibility_Template;
                ConfigManager.Instance.CommonConfiguration.TAX_DefinedRates[profile] = new Hashtable();
            }

            // get profile config
            Hashtable compatibleGrid = (Hashtable)ConfigManager.Instance.CommonConfiguration.TAX_Compatibility[profile];
            Hashtable definedGrid = (Hashtable)ConfigManager.Instance.CommonConfiguration.TAX_DefinedRates[profile];

            
            // load defined tax rates
            dataGridView_tax_userdefined.Rows.Clear();
            List<string> definedParameters = new List<string>();
            if (definedGrid != null)
            {
                SortedDictionary<string, string> sortedDefinedGrid = new SortedDictionary<string, string>();
                foreach (DictionaryEntry de in definedGrid)
                    sortedDefinedGrid[(string)de.Key] = de.Value.ToString();

                foreach (KeyValuePair<string, string> de in sortedDefinedGrid)
                {
                    definedParameters.Clear();
                    definedParameters.Add(de.Key.ToString());
                    definedParameters.AddRange(de.Value.ToString().Split(';'));
                    //definedParameters = de.Value.ToString().Split(';');
                    dataGridView_tax_userdefined.Rows.Add(definedParameters.ToArray());
                }
            }

            // load compatible grid
            dataGridView_tax_compatibility.Rows.Clear();
            SortedDictionary<char, char> sortedCompatibleGrid = new SortedDictionary<char, char>();
            if (compatibleGrid != null)
            {
                DataGridViewRow dwRow = null;
                DataGridViewComboBoxCell cbCell = null;
                DataGridViewTextBoxCell tbCell = null;

                foreach (DictionaryEntry de in compatibleGrid)
                    sortedCompatibleGrid[(char)de.Key] = (char)de.Value;

                foreach (KeyValuePair<char, char> de in sortedCompatibleGrid)
                {
                    cbCell = new DataGridViewComboBoxCell();
                    cbCell.ValueType = typeof(char);
                    cbCell.MaxDropDownItems = 8;
                    cbCell.Value = de.Value;

                    tbCell = new DataGridViewTextBoxCell();
                    tbCell.ValueType = typeof(char);
                    tbCell.Value = de.Key;

                    dwRow = new DataGridViewRow();
                    dwRow.Cells.Add(cbCell);
                    dwRow.Cells.Add(tbCell);

                    dataGridView_tax_compatibility.Rows.Add(dwRow);

                    cbCell.Dispose();
                    tbCell.Dispose();
                    dwRow.Dispose();
                }
                dataGridView_tax_compatibility.ReadOnly = false;
                dataGridView_tax_compatibility.Columns["mkt"].ReadOnly = true;
                dataGridView_tax_compatibility.Columns["mkt"].SortMode = DataGridViewColumnSortMode.Automatic;
                dataGridView_tax_compatibility.Sort(dataGridView_tax_compatibility.Columns["mkt"], ListSortDirection.Ascending);
            }

            UpdateTaxValues();

            try
            {
                if (compatibleGrid != null)
                {
                    int rowIndex = 0;
                    foreach (KeyValuePair<char, char> de in sortedCompatibleGrid)
                        dataGridView_tax_compatibility["pd", rowIndex++].Value = de.Value;
                }
            }
            catch { }

            //LoadAppTaxRates
            /*
            dataGridView_tax_userdefined.Rows.Clear();
            if (ConfigManager.Instance.CommonConfiguration.TAX_AppTaxChar != null)
                for (i = 0; i < ConfigManager.Instance.CommonConfiguration.TAX_AppTaxChar.Length; i++)
                {
                    dataGridView_tax_userdefined.Rows.Add(
                        ConfigManager.Instance.CommonConfiguration.TAX_AppTaxChar[i],
                        ConfigManager.Instance.CommonConfiguration.TAX_AppTaxRates[i],
                        ConfigManager.Instance.CommonConfiguration.TAX_AppTaxDisc[i]);
                }
            */
            //LoadComparableTaxTable
            /*
            DataGridViewRow dwRow = null;
            DataGridViewComboBoxCell cbCell = null;
            DataGridViewTextBoxCell tbCell = null;
            for (i = 0; i < ConfigManager.Instance.CommonConfiguration.TAX_MarketColumn.Length; i++)
            {
                cbCell = new DataGridViewComboBoxCell();
                cbCell.ValueType = typeof(char);
                cbCell.MaxDropDownItems = 8;
                cbCell.Value = ' ';

                tbCell = new DataGridViewTextBoxCell();
                tbCell.ValueType = typeof(char);
                tbCell.Value = ConfigManager.Instance.CommonConfiguration.TAX_MarketColumn[i];

                dwRow = new DataGridViewRow();
                dwRow.Cells.Add(cbCell);
                dwRow.Cells.Add(tbCell);

                dataGridView_tax_compatibility.Rows.Add(dwRow);

                cbCell.Dispose();
                tbCell.Dispose();
                dwRow.Dispose();
            }
            dataGridView_tax_compatibility.ReadOnly = false;
            dataGridView_tax_compatibility.Columns[1].ReadOnly = true;
            UpdateTaxValues();
            */
            //RestoreComparableValues
            /*
            try
            {
                if (ConfigManager.Instance.CommonConfiguration.TAX_AppColumn != null)
                    for (i = 0; i < ConfigManager.Instance.CommonConfiguration.TAX_AppColumn.Length; i++)
                        dataGridView_tax_compatibility["pd", i].Value = ConfigManager.Instance.CommonConfiguration.TAX_AppColumn[i];
            }
            catch { }
            */
        }
        private void Settings_Tax_Grid_Save(object profile)
        {
            if (ConfigManager.Instance.CommonConfiguration.TAX_Compatibility == null)
                ConfigManager.Instance.CommonConfiguration.TAX_Compatibility = new Hashtable();

            if (ConfigManager.Instance.CommonConfiguration.TAX_DefinedRates == null)
                ConfigManager.Instance.CommonConfiguration.TAX_DefinedRates = new Hashtable();
            
            // check for default
            if (!ConfigManager.Instance.CommonConfiguration.TAX_Compatibility.ContainsKey(profile))
            {
                ConfigManager.Instance.CommonConfiguration.TAX_Compatibility[profile] = ConfigManager.Instance.CommonConfiguration.TAX_Compatibility_Template;
                ConfigManager.Instance.CommonConfiguration.TAX_DefinedRates[profile] = new Hashtable();
            }

            Hashtable compatibleGrid = new Hashtable();
            Hashtable definedGrid = new Hashtable();

            // save compatible grid
            foreach (DataGridViewRow de in dataGridView_tax_compatibility.Rows)
            {
                compatibleGrid[de.Cells["mkt"].Value] = de.Cells["pd"].Value;
            }

            // save defined grid
            foreach (DataGridViewRow de in dataGridView_tax_userdefined.Rows)
            {
                definedGrid[de.Cells["taxChar"].Value] = string.Format("{0};{1}", de.Cells["taxRate"].Value, de.Cells["taxDisc"].Value);
            }

            //clean
            //ConfigManager.Instance.CommonConfiguration.TAX_Compatibility.Remove("-");
            //ConfigManager.Instance.CommonConfiguration.TAX_DefinedRates.Remove("-");

            // get profile config
            ConfigManager.Instance.CommonConfiguration.TAX_Compatibility[profile] = compatibleGrid;
            ConfigManager.Instance.CommonConfiguration.TAX_DefinedRates[profile] = definedGrid;
        }

        private void profiles_tab_profiles_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Rectangle r = profiles_tab_profiles.GetTabRect(profiles_tab_profiles.SelectedIndex);

            if (e.X - r.X < 20 && e.Y - r.Y < 20)
            {
                if (MessageBox.Show("Видалити профіль [" + profiles_tab_profiles.SelectedTab.Text + "] ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    profiles_tab_profiles.TabPages.Remove(profiles_tab_profiles.SelectedTab);
                    profiles_tab_profiles.Visible = !(profiles_tab_profiles.TabPages.Count == 0);
                }
            }
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            //.CoreLib.WriteLog(e.Exception, "dataGridView1_DataError");
        }

        private void System_Path_btn_resetPaths_Click(object sender, EventArgs e)
        {
            // initial path values
            ConfigManager.Instance.CommonConfiguration.Path_Config = Application.StartupPath + @"\config";
            this.System_Path_txbox_folderArticles.Text = Application.StartupPath + @"\articles";
            this.System_Path_txbox_folderCheques.Text = Application.StartupPath + @"\cheques";
            this.System_Path_txbox_folderTemp.Text = Application.StartupPath + @"\temp";
            this.System_Path_txbox_folderBills.Text = Application.StartupPath + @"\bills";
            this.System_Path_txbox_folderUserSchemas.Text = Application.StartupPath + @"\schemes";
            this.System_Path_txbox_folderUsers.Text = Application.StartupPath + @"\users";
            this.System_Path_txbox_folderTemplates.Text = Application.StartupPath + @"\templates";
            ConfigManager.Instance.CommonConfiguration.Path_Rules = Application.StartupPath + @"\Rules.ini";
            ConfigManager.Instance.CommonConfiguration.Path_Units = Application.StartupPath + @"\Units.ini";
            this.System_Path_txbox_folderPlugins.Text = Application.StartupPath + @"\plugins";
            this.System_Path_txbox_folderReports.Text = Application.StartupPath + @"\reports";
            ConfigManager.Instance.CommonConfiguration.Path_FeedInbox = Application.StartupPath + @"\feed_inbox";
        }

    }
}