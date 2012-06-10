using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace driver.Config
{
    public class ConfigManager
    {
        private static ConfigManager instance;
        private string configName;
        private Dictionary<string, AppConfig> profiles;
        private AppConfig commonConfig;

        public static ConfigManager getInstance()
        {
            if (instance == null)
                instance = new ConfigManager();
            return instance;
        }

        public ConfigManager()
        {
            this.commonConfig = new AppConfig();
            this.configName = "Config";
            this.profiles = new Dictionary<string, AppConfig>();
            //this.LoadData();
            // if profiles are empty
            //this.AddNewProfile();
        }

        // will add en empty profile
        public void AddNewProfile()
        {
            AppConfig ac = new AppConfig();
            this.profiles.Add(ac.p_key, ac);
        }
        // will add a specific profile
        public void AddNewProfile(string profileName, string profileKey)
        {
            AppConfig ac = new AppConfig();
            ac.p_name = profileName;
            ac.p_key = profileKey;
            this.profiles.Add(profileKey, ac);
        }

        public bool DeleteProfile(string profileKey)
        {
            bool rez = true;
            try
            {
                if (this.profiles.Count == 0)
                    throw new Exception("Could not remove all profiles. One profile entry has to exist.");
                this.profiles.Remove(profileKey);
            }
            catch (Exception ex)
            {
                Lib.CoreLib.WriteLog(ex, "DeleteProfile(" + profileKey + ")");
                rez = false;
            }
            return rez;
        }

        public bool CloneExisted(string newProfileKey, string existedProfileKey)
        {
            bool rez = true;
            try
            {
                this.profiles.Add(newProfileKey, this.profiles[existedProfileKey]);
            }
            catch (Exception ex)
            {
                Lib.CoreLib.WriteLog(ex, "CloneExisted(" + newProfileKey + "," + existedProfileKey + ")");
                rez = false;
            }
            return rez;
        }

        public bool SaveData()
        {
            bool saveOK = true;
            BinaryFormatter binF = new BinaryFormatter();
            using (FileStream stream = new FileStream(Application.StartupPath + "\\" + this.configName + ".cfg", FileMode.Create, FileAccess.Write))
            {
                try
                {
                    binF.Serialize(stream, new object[] { this.commonConfig, this.profiles });
                }
                catch (Exception ex)
                {
                    saveOK = false;
                    Lib.CoreLib.WriteLog(ex, "LoadData");
                }
            }
            return saveOK;
        }

        public bool LoadData()
        {
            bool loadOK = true;
            object[] cfg = _loadData(this.configName);
            try
            {
                if (cfg.Length == 2)
                {
                    this.commonConfig = (AppConfig)cfg[0];
                    this.profiles = (Dictionary<string, AppConfig>)cfg[1];
                }
                else loadOK = false;
            }
            catch (Exception ex)
            {
                loadOK = false;
                this.commonConfig = new AppConfig();
                this.profiles = new Dictionary<string, AppConfig>();
                Lib.CoreLib.WriteLog(ex, "LoadData");
            }

            if (loadOK)
                return true;

            if (File.Exists(Application.StartupPath + "\\backup\\" + this.configName + ".cfg"))
                try
                {
                    File.Copy(Application.StartupPath + "\\backup\\" + this.configName + ".cfg", Application.StartupPath + "\\" + this.configName + ".cfg", true);
                    cfg = _loadData(this.configName);
                    if (cfg.Length == 2)
                    {
                        this.commonConfig = (AppConfig)cfg[0];
                        this.profiles = (Dictionary<string, AppConfig>)cfg[1];
                        loadOK = true;
                    }
                }
                catch (Exception ex)
                {
                    Lib.CoreLib.WriteLog(ex, "LoadData");
                }
            if (!loadOK)
                MessageBox.Show("Помилка завантаження конфігурації. Звяжіться з постачальником програмного забезпечення.", Application.ProductName,
                 MessageBoxButtons.OK, MessageBoxIcon.Error);

            return loadOK;
        }

        private object[] _loadData(string cfgName)
        {
            BinaryFormatter binF = new BinaryFormatter();
            object[] all = null;

            if (!File.Exists(Application.StartupPath + "\\" + this.configName + ".cfg"))
                return all;

            using (FileStream stream = new FileStream(Application.StartupPath + "\\" + this.configName + ".cfg", FileMode.Open, FileAccess.Read))
            {
                try
                {
                    all = (object[])binF.Deserialize(stream);
                }
                catch (Exception ex)
                {
                    Lib.CoreLib.WriteLog(ex, "LoadData");
                }
            }
            return all;
        }

        private void SaveDataXml()
        {
            System.Xml.Serialization.XmlSerializer xmlS = new XmlSerializer(typeof(object[]));
            FileStream stream = new FileStream(Application.StartupPath + "\\" + this.configName + ".xcfg", FileMode.Create, FileAccess.Write);
            xmlS.Serialize(stream, new object[] { this.commonConfig, this.profiles });
            stream.Close();
            stream.Dispose();
        }

        public void LoadDataXml()
        {
            try
            {
                System.Xml.Serialization.XmlSerializer xmlS = new XmlSerializer(typeof(object[]));
                FileStream stream = new FileStream(Application.StartupPath + "\\" + this.configName + ".xcfg", FileMode.Open, FileAccess.Read);
                object[] all = (object[])xmlS.Deserialize(stream);
                this.commonConfig = (AppConfig)all[0];
                this.profiles = (Dictionary<string, AppConfig>)all[1];
                stream.Close();
                stream.Dispose();
            }
            catch (Exception ex)
            {
                this.commonConfig = new AppConfig();
                this.profiles = new Dictionary<string, AppConfig>();
                Lib.CoreLib.WriteLog(ex, "LoadData");
            }
        }

        public static bool SaveConfiguration()
        {
            return instance.SaveData();
        }

        public static bool LoadConfiguration()
        {
            return instance.LoadData();
        }

        public AppConfig this[string profile]
        {
            get
            {
                return (AppConfig)this.profiles[profile];
            }
            set
            {
                this.profiles[profile] = value;
            }
        }
        public Dictionary<string, AppConfig> Profiles { get { return this.profiles; } set { this.profiles = value; } }
        public string ConfigurationFile { get { return this.configName; } set { this.configName = value; } }
        public AppConfig[] ProfileConfigurations
        {
            get
            {

                List<AppConfig> items = new List<AppConfig>();
                foreach (KeyValuePair<string, AppConfig> c in this.profiles)
                    items.Add(c.Value);
                return items.ToArray();
            }
        }
        public AppConfig CommonConfiguration { get { return this.commonConfig; } set { this.commonConfig = value; } }
        public static ConfigManager Instance { get { return instance; } }
        #region OldConfigSaverImpl.
        /*
        /// <summary>
        /// Виконує збереження параметрів програми
        /// </summary>
        public static void SaveData()
        {
            SaveData("Config.cfg");
        }
        public static void SaveData(string configName)
        {
            //Creating data
            //TAX
            #region tax
            object[] tax = new object[vars_tax];
            tax[0] = TAX_AppTaxRates;
            tax[1] = TAX_AppTaxChar;
            tax[2] = TAX_AppTaxDisc;
            tax[3] = TAX_FxRates;
            tax[4] = TAX_AppColumn;
            tax[5] = TAX_MarketColumn;
            #endregion
            //System
            #region sysem
            object[] system = new object[vars_sys];
            system[0] = APP_MoneyDecimals;
            //system[1];
            system[2] = APP_Admin;
            system[3] = APP_SubUnit;
            system[4] = APP_PayDesk;
            system[5] = APP_ClientID;
            system[6] = APP_SubUnitName;
            system[7] = APP_RefreshRate;
            system[8] = APP_StartTotal;
            system[9] = APP_Language;
            system[10] = APP_Printer;
            system[11] = APP_DOSCommand;
            system[12] = APP_OutPrnStyle;
            system[13] = APP_UseStaticDiscount;
            system[14] = APP_AllowOneCopy;
            system[15] = APP_UseStaticRules;
            system[16] = APP_OnlyDiscount;
            system[17] = APP_StaticDiscountValue;
            system[18] = APP_ShowInventWindow;
            system[19] = APP_WeightType;
            system[20] = APP_SearchType;
            system[21] = APP_ChequeName;
            system[22] = APP_DoseDecimals;
            system[23] = APP_InvAutoSave;
            //system[24];
            system[25] = APP_SrchTypesAccess;
            //system[26] = APP_StaticDiscountType;
            system[27] = APP_ClearTEMPonExit;
            system[28] = APP_ShowInfoOnIndicator;
            system[29] = APP_RefreshTimeout;
            system[30] = APP_UsePercentTypeDisc;
            system[31] = APP_UseAbsoluteTypeDisc;
            system[32] = APP_DefaultTypeDisc;
            system[33] = APP_PrintersLinks;
            system[34] = APP_UseCommonPrinting;
            #endregion
            //Path
            #region path
            object[] path = new object[vars_path];
            path[0] = Path_Articles;
            path[1] = Path_Cheques;
            path[2] = Path_Temp;
            path[3] = Path_Bills;
            path[4] = Path_Schemes;
            path[5] = Path_Exchnage;
            path[6] = Path_Users;
            path[7] = Path_Templates;
            path[8] = Path_Tpl_1;
            path[9] = Path_Tpl_2;
            path[10] = Path_Tpl_3;
            path[11] = Path_Rules;
            path[12] = Path_Units;
            path[13] = Path_Plugins;
            path[14] = Path_PrinterTempleates;
            path[15] = Path_Reports;
            path[16] = Path_Printers;
            path[17] = Path_Config;
            #endregion
            //Style
            #region style
            object[] style = new object[vars_style];
            style[0] = STYLE_BackgroundStatPan;
            style[1] = STYLE_BackgroundArtTbl;
            style[2] = STYLE_BackgroundAChqTbl;
            style[3] = STYLE_BackgroundNAChqTbl;
            style[4] = STYLE_BackgroundAddPan;
            style[5] = STYLE_BackgroundInfPan;
            style[6] = STYLE_BackgroundSumRest;

            style[7] = STYLE_SumFont;
            style[8] = STYLE_SumFontColor;
            style[9] = STYLE_RestFont;
            style[10] = STYLE_RestFontColor;
            style[11] = STYLE_StatusFont;
            style[12] = STYLE_StatusFontColor;
            style[13] = STYLE_AppInformerFont;
            style[14] = STYLE_AppInformerFontColor;
            style[15] = STYLE_AddInformerFont;
            style[16] = STYLE_AddInformerFontColor;
            style[17] = STYLE_ChequeFont;
            style[18] = STYLE_ChequeFontColor;
            style[19] = STYLE_ArticlesFont;
            style[20] = STYLE_ArticlesFontColor;

            style[21] = STYLE_ChqColumnLock;
            style[22] = STYLE_ArtColumnLock;
            style[23] = STYLE_GridsView;
            style[24] = STYLE_ARTColumnName;
            style[25] = STYLE_ColumnCaption;
            style[26] = STYLE_ALTColumnName;
            style[27] = STYLE_CARDColumnName;

            style[28] = STYLE_ChqInformerFont;
            style[29] = STYLE_ChqInformerFontColor;

            style[30] = STYLE_SplitOrient;
            style[31] = STYLE_MainWndPosition;
            style[32] = STYLE_MainWndSize;
            style[33] = STYLE_ArtSideCollapsed;
            style[34] = STYLE_MainWndState;
            style[35] = STYLE_SplitterDistance;

            style[36] = STYLE_BillWindow;
            style[37] = STYLE_BillWindowEntry;
            style[38] = STYLE_BillWindowEntryItems;

            style[39] = STYLE_Misc_ChequeRowHeight;
            style[40] = STYLE_Misc_ArticleRowHeight;

            #endregion
            //DGP
            #region dgp
            object[] dgp = new object[vars_dgp];
            dgp[0] = DGP_HeaderFont;
            dgp[1] = DGP_TableFont;
            dgp[2] = DGP_Background;
            dgp[3] = DGP_Interchange;
            dgp[4] = DGP_Color1;
            dgp[5] = DGP_Color2;
            dgp[6] = DGP_Columns;
            #endregion
            //Add
            #region Add
            object[] add = new object[vars_add];
            add[0] = ADD_updateDateTime;
            #endregion
            // Content
            #region Content
            object[][] content = new object[vars_content][];
            #region Common
            content[0] = new object[1];
            content[0][0] = Content_Common_PromptMsgOnIllegal;
            #endregion
            #region Cheques
            content[1] = new object[6];
            content[1][0] = Content_Cheques_AddTotal;
            content[1][1] = Content_Cheques_UseSeparateCheque;
            content[1][2] = Content_Cheques_SeparatedArticleMaskById;
            content[1][3] = Content_Cheques_UseCustomClientCardBC;
            content[1][4] = Content_Cheques_CustomClientCardBC;
            content[1][5] = Content_Cheques_AddCopyToArchive;
            #endregion
            #region Bills
            content[2] = new object[2];
            content[2][0] = Content_Bills_KeepAliveAfterCheque;
            content[2][1] = Content_Bills_AddCopyToArchive;
            #endregion
            #region Articles
            content[3] = new object[1];
            content[3][0] = Content_Articles_KeepDataAfterImport;
            #endregion
            #endregion
            // Skin
            #region skin
            object[][] skin = new object[vars_skin][];
            #region Sensor
            skin[0] = new object[vars_skin_sensor];
            skin[0][0] = skin_sensor_active;
            skin[0][1] = skin_sensor_com_chqnav;
            skin[0][2] = skin_sensor_com_chqopr;
            skin[0][3] = skin_sensor_com_chqsrch;
            skin[0][4] = skin_sensor_com_chqbills;
            skin[0][5] = skin_sensor_com_artnav;
            skin[0][6] = skin_sensor_com_artscroll;
            skin[0][7] = skin_sensor_splitter_chqmain;
            skin[0][8] = skin_sensor_splitter_chqcontrols;
            skin[0][9] = skin_sensor_splitter_chq;
            skin[0][10] = skin_sensor_splitter_chq_orient;
            #endregion
            #endregion
            object[] data = new object[cfgRange];
            data[0] = tax;
            data[1] = system;
            data[2] = path;
            data[3] = style;
            data[4] = dgp;
            data[5] = add;
            data[6] = content;
            data[7] = skin;

            BinaryFormatter binF = new BinaryFormatter();
            FileStream stream = new FileStream(Application.StartupPath + "\\" + configName, FileMode.Create, FileAccess.Write);
            binF.Serialize(stream, data);
            stream.Close();
            stream.Dispose();
        }

        /// <summary>
        /// Виконує відновлення значень параметрів програми
        /// </summary>
        public static void ApiLoadData(ApiManager mgr)
        {
            try
            {
                // mgr.GetValue(
                /* if (param != null && param.ToString() != string.Empty)
                     LoadData(param.ToString());* /
            }
            catch (Exception ex)
            {
                driver.Common.pdLogger.Log(ex);
                LoadData();
            }
        }
        public static void LoadData()
        {
            LoadData("Config.cfg");
        }
        public static void LoadData(string configName)
        {
            try
            {
                BinaryFormatter binF = new BinaryFormatter();
                FileStream stream = new FileStream(Application.StartupPath + "\\" + configName, FileMode.Open, FileAccess.Read);
                object[] data = (object[])binF.Deserialize(stream);
                stream.Close();
                stream.Dispose();

                //Retriving data
                object[] tax = new object[0];
                object[] system = new object[0];
                object[] path = new object[0];
                object[] style = new object[0];
                object[] dgp = new object[0];
                object[] add = new object[0];
                object[][] content = new object[0][];
                object[][] skin = new object[0][];

                #region GetCfgBlocks
                try
                {
                    tax = (object[])data[0];
                }
                catch { }
                try
                {
                    system = (object[])data[1];
                }
                catch { }
                try
                {
                    path = (object[])data[2];
                }
                catch { }
                try
                {
                    style = (object[])data[3];
                }
                catch { }
                try
                {
                    dgp = (object[])data[4];
                }
                catch { }
                try
                {
                    add = (object[])data[5];
                }
                catch { }
                try
                {
                    content = (object[][])data[6];
                }
                catch { }
                try
                {
                    skin = (object[][])data[7];
                }
                catch { }
                #endregion

                //TAX
                #region tax
                try
                {
                    TAX_AppTaxRates = (double[])tax[0];
                    TAX_AppTaxChar = (char[])tax[1];
                    TAX_AppTaxDisc = (bool[])tax[2];
                    TAX_FxRates = (double[])tax[3];
                    TAX_AppColumn = (char[])tax[4];
                    TAX_MarketColumn = (char[])tax[5];
                }
                catch { }
                #endregion
                //System
                #region sysem
                try
                {
                    APP_MoneyDecimals = (int)system[0];
                    //system[1];
                    APP_Admin = system[2].ToString();
                    APP_SubUnit = (byte)system[3];
                    APP_PayDesk = (byte)system[4];
                    APP_ClientID = system[5].ToString();
                    APP_SubUnitName = system[6].ToString();
                    APP_RefreshRate = (int)system[7];
                    APP_StartTotal = (double)system[8];
                    APP_Language = system[9].ToString();
                    APP_Printer = (int)system[10];
                    APP_DOSCommand = system[11].ToString();
                    APP_OutPrnStyle = system[12].ToString();
                    APP_UseStaticDiscount = (bool)system[13];
                    APP_AllowOneCopy = (bool)system[14];
                    APP_UseStaticRules = (bool)system[15];
                    APP_OnlyDiscount = (bool)system[16];
                    APP_StaticDiscountValue = (double)system[17];
                    APP_ShowInventWindow = (bool)system[18];
                    APP_WeightType = (int)system[19];
                    APP_SearchType = (int)system[20];
                    APP_ChequeName = system[21].ToString();
                    APP_DoseDecimals = (int)system[22];
                    APP_InvAutoSave = (byte)system[23];
                    //system[24];
                    APP_SrchTypesAccess = (bool[])system[25];
                    //APP_StaticDiscountType = (byte)system[26];
                    APP_ClearTEMPonExit = (bool)system[27];
                    APP_ShowInfoOnIndicator = (bool)system[28];
                    APP_RefreshTimeout = (int)system[29];
                    APP_UsePercentTypeDisc = (bool)system[30];
                    APP_UseAbsoluteTypeDisc = (bool)system[31];
                    APP_DefaultTypeDisc = (int)system[32];
                    APP_PrintersLinks = (string[][])system[33];
                    APP_UseCommonPrinting = (bool)system[34];
                }
                catch { }
                #endregion
                //Path
                #region path
                try
                {
                    Path_Articles = path[0].ToString();
                    Path_Cheques = path[1].ToString();
                    Path_Temp = path[2].ToString();
                    Path_Bills = path[3].ToString();
                    Path_Schemes = path[4].ToString();
                    Path_Exchnage = path[5].ToString();
                    Path_Users = path[6].ToString();
                    Path_Templates = path[7].ToString();
                    Path_Tpl_1 = path[8].ToString();
                    Path_Tpl_2 = path[9].ToString();
                    Path_Tpl_3 = path[10].ToString();
                    Path_Rules = path[11].ToString();
                    Path_Units = path[12].ToString();
                    Path_Plugins = path[13].ToString();
                    Path_PrinterTempleates = (Dictionary<string, string>)path[14];
                    Path_Reports = path[15].ToString();
                    Path_Printers = (Dictionary<string, Dictionary<string, string>>)path[16];
                    Path_Config = path[17].ToString();
                }
                catch { }
                #endregion
                //Style
                #region style
                try
                {
                    STYLE_BackgroundStatPan = (Color)style[0];
                    STYLE_BackgroundArtTbl = (Color)style[1];
                    STYLE_BackgroundAChqTbl = (Color)style[2];
                    STYLE_BackgroundNAChqTbl = (Color)style[3];
                    STYLE_BackgroundAddPan = (Color)style[4];
                    STYLE_BackgroundInfPan = (Color)style[5];
                    STYLE_BackgroundSumRest = (Color)style[6];

                    STYLE_SumFont = (Font)style[7];
                    STYLE_SumFontColor = (Color)style[8];
                    STYLE_RestFont = (Font)style[9];
                    STYLE_RestFontColor = (Color)style[10];
                    STYLE_StatusFont = (Font)style[11];
                    STYLE_StatusFontColor = (Color)style[12];
                    STYLE_AppInformerFont = (Font)style[13];
                    STYLE_AppInformerFontColor = (Color)style[14];
                    STYLE_AddInformerFont = (Font)style[15];
                    STYLE_AddInformerFontColor = (Color)style[16];
                    STYLE_ChequeFont = (Font)style[17];
                    STYLE_ChequeFontColor = (Color)style[18];
                    STYLE_ArticlesFont = (Font)style[19];
                    STYLE_ArticlesFontColor = (Color)style[20];

                    STYLE_ChqColumnLock = (bool)style[21];
                    STYLE_ArtColumnLock = (bool)style[22];
                    STYLE_GridsView = (object[])style[23];
                    STYLE_ARTColumnName = (string[])style[24];
                    STYLE_ColumnCaption = (string[])style[25];
                    STYLE_ALTColumnName = (string[])style[26];
                    STYLE_CARDColumnName = (string[])style[27];

                    STYLE_ChqInformerFont = (Font)style[28];
                    STYLE_ChqInformerFontColor = (Color)style[29];

                    STYLE_SplitOrient = (Orientation)style[30];
                    STYLE_MainWndPosition = (Point)style[31];
                    STYLE_MainWndSize = (Size)style[32];
                    STYLE_ArtSideCollapsed = (bool)style[33];
                    STYLE_MainWndState = (FormWindowState)style[34];
                    STYLE_SplitterDistance = (int)style[35];

                    STYLE_BillWindow = (Font)style[36];
                    STYLE_BillWindowEntry = (Font)style[37];
                    STYLE_BillWindowEntryItems = (Font)style[38];

                    STYLE_Misc_ChequeRowHeight = int.Parse(style[39].ToString());
                    STYLE_Misc_ArticleRowHeight = int.Parse(style[40].ToString());
                }
                catch { }
                #endregion
                //DGP
                #region dgp
                try
                {
                    DGP_HeaderFont = (Font)dgp[0];
                    DGP_TableFont = (Font)dgp[1];
                    DGP_Background = (Color)dgp[2];
                    DGP_Interchange = (bool)dgp[3];
                    DGP_Color1 = (Color)dgp[4];
                    DGP_Color2 = (Color)dgp[5];
                    DGP_Columns = (object[][])dgp[6];
                }
                catch { }
                #endregion
                //Add
                #region Add
                ADD_updateDateTime = (DateTime[])add[0];
                #endregion
                // Content
                #region content
                // common
                try
                {
                    Content_Common_PromptMsgOnIllegal = (bool)content[0][0];
                }
                catch { }
                // cheques
                try
                {
                    Content_Cheques_AddTotal = content[1][0].ToString();
                    Content_Cheques_UseSeparateCheque = (bool)content[1][1];
                    Content_Cheques_SeparatedArticleMaskById = content[1][2].ToString();
                    Content_Cheques_UseCustomClientCardBC = (bool)content[1][3];
                    Content_Cheques_CustomClientCardBC = content[1][4].ToString();
                    Content_Cheques_AddCopyToArchive = (bool)content[1][5];
                }
                catch { }
                // bills
                try
                {
                    Content_Bills_KeepAliveAfterCheque = (bool)content[2][0];
                    Content_Bills_AddCopyToArchive = (bool)content[2][1];
                }
                catch { }
                // articles
                try
                {
                    Content_Articles_KeepDataAfterImport = (bool)content[3][0];
                }
                catch { }
                #endregion
                // Skin
                #region skins
                try
                {
                    // sensor
                    skin_sensor_active = (bool)skin[0][0];
                    skin_sensor_com_chqnav = (bool)skin[0][1];
                    skin_sensor_com_chqopr = (bool)skin[0][2];
                    skin_sensor_com_chqsrch = (bool)skin[0][3];
                    skin_sensor_com_chqbills = (bool)skin[0][4];
                    skin_sensor_com_artnav = (bool)skin[0][5];
                    skin_sensor_com_artscroll = (bool)skin[0][6];
                    skin_sensor_splitter_chqmain = int.Parse(skin[0][7].ToString());
                    skin_sensor_splitter_chqcontrols = int.Parse(skin[0][8].ToString());
                    skin_sensor_splitter_chq = int.Parse(skin[0][9].ToString());
                    skin_sensor_splitter_chq_orient = int.Parse(skin[0][10].ToString());
                }
                catch { }
                #endregion
            }
            catch
            {
                SaveData(configName);
            }

            APP_UnitFilter = new object[2] { new string[0], new bool[0] };

            if (File.Exists(Path_Units))
            {
                string[] unitsNames = new string[0];
                bool[] unitsStates = new bool[0];
                using (StreamReader sr = File.OpenText(Path_Units))
                {
                    string[] input = new string[2];
                    while ((input[0] = sr.ReadLine()) != null)
                    {
                        Array.Resize<string>(ref unitsNames, unitsNames.Length + 1);
                        Array.Resize<bool>(ref unitsStates, unitsStates.Length + 1);
                        input = input[0].Split('_');
                        unitsNames[unitsNames.Length - 1] = input[0];
                        unitsStates[unitsStates.Length - 1] = input[1] == "1" ? true : false;
                    }
                    sr.Close();
                    sr.Dispose();
                }
                APP_UnitFilter[0] = unitsNames;
                APP_UnitFilter[1] = unitsStates;
            }
            else
            {
                StreamWriter sw = File.CreateText(Path_Units);
                sw.Close();
                sw.Dispose();
            }

            APP_DiscountRules = new string[0];
            if (File.Exists(Path_Rules))
                using (StreamReader sr = File.OpenText(Path_Rules))
                {
                    string input = "";
                    while ((input = sr.ReadLine()) != null)
                    {
                        Array.Resize<string>(ref APP_DiscountRules, APP_DiscountRules.Length + 1);
                        APP_DiscountRules[APP_DiscountRules.Length - 1] = input;
                    }
                    sr.Close();
                    sr.Dispose();
                }
            else
            {
                StreamWriter sw = File.CreateText(Path_Rules);
                sw.Close();
                sw.Dispose();
            }

        }
        */
        #endregion

    }
}
