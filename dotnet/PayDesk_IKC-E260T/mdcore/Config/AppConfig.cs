using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using mdcore.API;

namespace mdcore.Config
{
    [Serializable]
    public class AppConfig
    {
        #region TAX Settings
        public static double[] TAX_AppTaxRates;
        public static char[] TAX_AppTaxChar;
        public static bool[] TAX_AppTaxDisc;
        public static double[] TAX_FxRates;
        public static char[] TAX_AppColumn = new char[] { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };
        public static char[] TAX_MarketColumn = new char[] { ' ', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        #endregion
        private const byte vars_tax = 6;

        #region System Settings
        public static int APP_MoneyDecimals = 2;
        public static object[] APP_UnitFilter = new object[3] { new string[0], new bool[0], new bool[0] };
        public static string APP_Admin = "0";
        public static byte APP_SubUnit = 2;
        public static byte APP_PayDesk = 1;
        public static string APP_ClientID = "";
        public static string APP_SubUnitName = "";
        public static int APP_RefreshRate = 15000;
        public static double APP_StartTotal = 1;
        public static string APP_Language = "";
        public static int APP_Printer = 1;
        public static string APP_DOSCommand = "";
        public static string APP_OutPrnStyle = "File";
        public static bool APP_UseStaticDiscount = false;
        public static bool APP_AllowOneCopy = true;
        public static bool APP_UseStaticRules = false;
        public static bool APP_OnlyDiscount = true;
        public static double APP_StaticDiscountValue = 0.0;
        public static bool APP_ShowInventWindow = false;
        public static int APP_WeightType = 0;
        public static int APP_SearchType = 2;
        public static string APP_ChequeName = "%0%1_%2_%3_%4_%5_%6_%7";
        public static int APP_DoseDecimals = 3;
        public static byte APP_InvAutoSave = 10;
        public static string[] APP_DiscountRules = new string[0];
        public static bool[] APP_SrchTypesAccess = new bool[3] { false, false, true };
        //public static byte APP_StaticDiscountType = 0;
        public static bool APP_ClearTEMPonExit = true;
        public static bool APP_ShowInfoOnIndicator = false;
        public static int APP_RefreshTimeout = 100;
        public static bool APP_UsePercentTypeDisc = true;
        public static bool APP_UseAbsoluteTypeDisc = false;
        public static int APP_DefaultTypeDisc = 0;
        public static string[][] APP_PrintersLinks = new string[3][];
        public static bool APP_UseCommonPrinting = false;
        public static bool APP_ScannerUseProgamMode = true;
        public static int APP_ScannerCharReadFrequency = 130;
        public static int APP_BuyerBarCodeSource = 0;
        public static int APP_BuyerBarCodeMinLen = 8;
        #endregion
        private const byte vars_sys = 39;

        #region Path_Settings
        public static string Path_Config = Application.StartupPath + @"\config";
        public static string Path_Articles = Application.StartupPath + @"\articles";
        public static string Path_Cheques = Application.StartupPath + @"\cheques";
        public static string Path_Temp = Application.StartupPath + @"\temp";
        public static string Path_Bills = Application.StartupPath + @"\bills";
        public static string Path_Schemes = Application.StartupPath + @"\schemes";
        public static string Path_Exchnage = @"";
        public static string Path_Users = Application.StartupPath + @"\users";
        public static string Path_Templates = Application.StartupPath + @"\templates";
        public static string Path_Tpl_1 = @"";
        public static string Path_Tpl_2 = @"";
        public static string Path_Tpl_3 = @"";
        public static Dictionary<string, string> Path_PrinterTempleates = new Dictionary<string, string>();
        public static string Path_Rules = Application.StartupPath + @"\Rules.ini";
        public static string Path_Units = Application.StartupPath + @"\Units.ini";
        public static string Path_Plugins = Application.StartupPath + @"\plugins";
        public static Dictionary<string, Dictionary<string, string>> Path_Printers = new Dictionary<string, Dictionary<string, string>>();
        public static string Path_Reports = Application.StartupPath + @"\reports";
        #endregion Path
        private const byte vars_path = 18;

        #region Style
        //--- Colors 7
        public static Color STYLE_BackgroundStatPan = Color.FromKnownColor(KnownColor.GradientInactiveCaption);
        public static Color STYLE_BackgroundArtTbl = Color.FromArgb(216, 228, 248);
        public static Color STYLE_BackgroundAChqTbl = Color.FromArgb(243, 244, 218);
        public static Color STYLE_BackgroundNAChqTbl = System.Drawing.Color.White;
        public static Color STYLE_BackgroundAddPan = System.Drawing.Color.White;
        public static Color STYLE_BackgroundInfPan = System.Drawing.Color.White;
        public static Color STYLE_BackgroundSumRest = System.Drawing.Color.White;

        // ---- Fonts 16
        public static Font STYLE_SumFont = new Font("Tahoma", 36.0F, FontStyle.Bold);
        public static Color STYLE_SumFontColor = System.Drawing.Color.Green;
        public static Font STYLE_RestFont = new Font("Tahoma", 36.0F, FontStyle.Bold);
        public static Color STYLE_RestFontColor = System.Drawing.Color.Red;
        public static Font STYLE_StatusFont = new Font("Tahoma", 8.25F, FontStyle.Regular);
        public static Color STYLE_StatusFontColor = System.Drawing.Color.Black;
        public static Font STYLE_AppInformerFont = new Font("Tahoma", 8.25F, FontStyle.Regular);
        public static Color STYLE_AppInformerFontColor = System.Drawing.Color.Black;
        public static Font STYLE_AddInformerFont = new Font("Microsoft Sans Serif", 14.25F, FontStyle.Regular);
        public static Color STYLE_AddInformerFontColor = System.Drawing.Color.Black;
        public static Font STYLE_ChqInformerFont = new Font("Tahoma", 9.75F, FontStyle.Bold);
        public static Color STYLE_ChqInformerFontColor = System.Drawing.Color.Black;
        public static Font STYLE_ChequeFont = new Font("Microsoft Sans Serif", 12.0F, FontStyle.Regular);
        public static Color STYLE_ChequeFontColor = System.Drawing.Color.Black;
        public static Font STYLE_ArticlesFont = new Font("Microsoft Sans Serif", 12.0F, FontStyle.Regular);
        public static Color STYLE_ArticlesFontColor = System.Drawing.Color.Black;

        // ---- DGV 7
        public static bool STYLE_ChqColumnLock = false;
        public static bool STYLE_ArtColumnLock = false;
        public static object[] STYLE_GridsView = new object[2] { new object[2], new object[2] };
        public static string[] STYLE_ARTColumnName = new string[] { "ID", "BC", "NAME", "DESC", "UNIT", "VG", "TID", "TQ", "PACK", "WEIGHT", "PRICE", "PR1", "PR2", "PR3", "Q2", "Q3" };
        public static string[] STYLE_ColumnCaption = new string[] { "Код", "Штрих-код", "Коротка назва", "Повна назва", "Одиниця", "ПДВ", "TID", "TQ", "PACK", "Вага", "Ціна", "PR1", "PR2", "PR3", "Q2", "Q3", "К-сть", "ПДВ%", "Знижка", "Знижка %", "Власна сума", "Сума", "Значення ПДВ", "К-сть", "Стартова ціна", "Надрукована к-сть" };
        public static string[] STYLE_ALTColumnName = new string[] { "ABC", "AID" };
        public static string[] STYLE_CARDColumnName = new string[] { "CBC", "CID", "CDISC", "CPRICENO" };

        // ---- Window 4
        public static Orientation STYLE_SplitOrient = Orientation.Horizontal;
        public static Point STYLE_MainWndPosition = new Point(120, 120);
        public static Size STYLE_MainWndSize = new Size(640, 480);
        public static bool STYLE_ArtSideCollapsed = false;

        public static FormWindowState STYLE_MainWndState = FormWindowState.Normal;
        public static int STYLE_SplitterDistance = 200;

        public static Font STYLE_BillWindow = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular);
        public static Font STYLE_BillWindowEntry = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular);
        public static Font STYLE_BillWindowEntryItems = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular);

        public static int STYLE_Misc_ChequeRowHeight = 21;
        public static int STYLE_Misc_ArticleRowHeight = 21;

        #endregion View
        private const byte vars_style = 41;

        #region DataGridPrinter
        public static Font DGP_HeaderFont = new Font("Arial", 12.8F, FontStyle.Bold);
        public static Font DGP_TableFont = new Font("Tahoma", 11.1F, FontStyle.Regular);
        public static Color DGP_Background = Color.White;
        public static bool DGP_Interchange = true;
        public static Color DGP_Color1 = Color.FromArgb(125, 125, 125);
        public static Color DGP_Color2 = Color.White;
        public static object[][] DGP_Columns = new object[4][];
        #endregion
        private const byte vars_dgp = 7;

        #region Additional
        public static DateTime[] ADD_updateDateTime = new DateTime[3] { 
                    new DateTime(),
                    new DateTime(),
                    new DateTime()
            };
        #endregion
        private const byte vars_add = 1;

        #region Content
        #region Common
        public static bool Content_Common_PromptMsgOnIllegal = true;

        #endregion
        #region Cheques
        public static string Content_Cheques_AddTotal = "none";
        #endregion
        #region Bills
        public static bool Content_Bills_KeepAliveAfterCheque = true;

        #endregion
        #region Articles
        public static bool Content_Articles_KeepDataAfterImport = true;

        #endregion
        #endregion
        private const byte vars_content = 4;

        private const byte cfgRange = 7;

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
            system[35] = APP_ScannerUseProgamMode;
            system[36] = APP_ScannerCharReadFrequency;
            system[37] = APP_BuyerBarCodeSource;
            system[38] = APP_BuyerBarCodeMinLen;
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
            content[1] =  new object[1];
            content[1][0] = Content_Cheques_AddTotal;
            #endregion
            #region Bills
            content[2] =  new object[1];
            content[2][0] = Content_Bills_KeepAliveAfterCheque;
            #endregion
            #region Articles
            content[3] = new object[1];
            content[3][0] = Content_Articles_KeepDataAfterImport;
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
                    LoadData(param.ToString());*/
            }
            catch (Exception ex)
            {
                mdcore.Common.pdLogger.Log(ex);
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
                    APP_ScannerUseProgamMode = (bool)system[35];
                    APP_ScannerCharReadFrequency = (int)system[36];
                    APP_BuyerBarCodeSource = (int)system[37];
                    APP_BuyerBarCodeMinLen = (int)system[38];
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
                try
                {
                    Content_Common_PromptMsgOnIllegal = (bool)content[0][0];
                }
                catch { }
                // cheques
                try
                {
                    Content_Cheques_AddTotal = content[1][0].ToString();
                }
                catch { }
                // bills
                try
                {
                    Content_Bills_KeepAliveAfterCheque = (bool)content[2][0];
                }
                catch { }
                // articles
                try
                {
                    Content_Articles_KeepDataAfterImport = (bool)content[3][0];
                }
                catch { }
                #endregion
            }
            catch
            {
                SaveData(configName);
            }

            APP_UnitFilter = new object[3] { new string[0], new bool[0], new bool[0] };

            if (File.Exists(Path_Units))
            {
                string[] unitsNames = new string[0];
                bool[] unitsStates = new bool[0];
                bool[] useScales = new bool[0];
                using (StreamReader sr = File.OpenText(Path_Units))
                {
                    string[] input = new string[2];
                    while ((input[0] = sr.ReadLine()) != null)
                    {
                        Array.Resize<string>(ref unitsNames, unitsNames.Length + 1);
                        Array.Resize<bool>(ref unitsStates, unitsStates.Length + 1);
                        Array.Resize<bool>(ref useScales, useScales.Length + 1);
                        input = input[0].Split('_');
                        unitsNames[unitsNames.Length - 1] = input[0];
                        unitsStates[unitsStates.Length - 1] = input[1] == "1" ? true : false;
                        try
                        {
                            useScales[useScales.Length - 1] = input[2] == "1" ? true : false;
                        }
                        catch { }
                    }
                    sr.Close();
                    sr.Dispose();
                }
                APP_UnitFilter[0] = unitsNames;
                APP_UnitFilter[1] = unitsStates;
                APP_UnitFilter[2] = useScales;
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
    }
}