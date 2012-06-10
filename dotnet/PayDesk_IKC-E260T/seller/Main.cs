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
using System.Diagnostics;
using mdcore;
using System.Runtime.InteropServices;
using winapi;

namespace PayDesk
{
    public partial class Main : Form
    {
        private Admin admin = new Admin();
        // Data
        private DataTable Cheque;
        private DataTable Articles;
        private DataTable AltBC;
        private DataTable Cards;
        // sacnner
        private string chararray;
        private DateTime lastInputChar;
        //vars
        private double chqSUMA;
        private double realSUMA;
        private double taxSUMA;
        private string clientID;
        private byte currentSubUnit;
        private bool nakladna;
        private int currSrchType;
        //cheques type
        private bool retriveChq;
        private bool inventChq;
        //discount
        private double discConstPercent;
        private int clientPriceNo;
        //private double cash_constDiscount;
        private double[] discArrPercent = new double[2] { 0.0, 0.0 };
        private double[] discArrCash = new double[2] { 0.0, 0.0 };
        private double discOnlyPercent = 0.0;
        private double discOnlyCash = 0.0;
        private double discCommonPercent = 0.0;
        private double discCommonCash = 0.0;
        //flags
        private bool _fl_artUpdated;
        private bool _fl_canUpdate = false;
        private bool _fl_onlyUpdate;
        private bool ADMIN_STATE = false;
        private bool _fl_menuIsActive = false;
        private bool _fl_SubUnitChanged = false;
        private bool _fl_useTotDisc = true;
        //private System.Threading.Thread updateThread;

        public Main()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Перевизначений метод для виконання операцій відновлення
        /// інтерфейсу та інших параметрів програми під час її завантаження
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            //winapi.Funcs.OutputDebugString("load_begin");

            //Restore position
            this.Text = Application.ProductName;
            this.WindowState = AppConfig.STYLE_MainWndState;
            this.Location = new Point(AppConfig.STYLE_MainWndPosition.X, AppConfig.STYLE_MainWndPosition.Y);
            this.Size = new Size(AppConfig.STYLE_MainWndSize.Width, AppConfig.STYLE_MainWndSize.Height);
            this.splitContainer1.SplitterDistance = AppConfig.STYLE_SplitterDistance;
            this.splitContainer1.Panel2Collapsed = AppConfig.STYLE_ArtSideCollapsed;
            //updateThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(CheckForUpdate));

            ADMIN_STATE = UserStruct.AdminState;

            //create tables
            AppFunc.CreateTables(ref Cheque, ref Articles, ref AltBC, ref Cards);
            chequeDGV.DataSource = Cheque;
            articleDGV.DataSource = Articles;
            DataGridView[] grids = new DataGridView[] { chequeDGV, articleDGV };
            AppFunc.LoadGridsView(ref grids, splitContainer1.Orientation);

            if (AppConfig.Path_Exchnage == "")
            {
                MMessageBox.Show(this, "Вкажіть шлях до папки обміну", Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                folderBrowserDialog1.RootFolder = Environment.SpecialFolder.Desktop;
                folderBrowserDialog1.ShowDialog();
                AppConfig.Path_Exchnage = folderBrowserDialog1.SelectedPath;
                AppConfig.SaveData();
            }

            UpdateMyControls();

            //Set default type of search
            SearchFilter(false, AppConfig.APP_SearchType, true);
            UpdateSumInfo(true);

            if (Program.Service.UseEKKR)
            {
                try
                {
                    Program.Service.CallFunction("SetCashier", new object[] { UserStruct.UserFLogin, UserStruct.UserFPassword, UserStruct.UserID });
                    DDM_FPStatus.Image = Properties.Resources.ok;
                }
                catch (Exception ex)
                {
                    MMessageBox.Show(this, ex.Message + "\r\nНеможливо зареєструвати касира в ЕККР",
                        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            base.OnLoad(e);
            //WinAPI.OutputDebugString("load_end");
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
                AppConfig.STYLE_MainWndSize = this.Size;
            base.OnSizeChanged(e);
        }
        protected override void OnLocationChanged(EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
                AppConfig.STYLE_MainWndPosition = this.Location;
            if (WindowState == FormWindowState.Minimized)
                this.OnDeactivate(EventArgs.Empty);
            else
                this.OnActivated(EventArgs.Empty);
            base.OnLocationChanged(e);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            if (inventChq)
            {
                MMessageBox.Show(this, "Відкритий чек інвентаризації!", Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
                return;
            }

            DialogResult rez = DialogResult.No;

            if (chequeDGV.RowCount != 0)
            {
                MMessageBox.Show(this, "Закрийте чек перед тим, як вийти", Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.Cancel = true;
                return;
            }

            if (DialogResult == DialogResult.Retry)
                rez = MMessageBox.Show(this, "Змінити касира ?", Application.ProductName,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            else
                rez = MMessageBox.Show(this, "Вийти з програми ?", Application.ProductName,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            switch (rez)
            {
                case DialogResult.Yes:
                    {
                        AppConfig.STYLE_SplitterDistance = splitContainer1.SplitterDistance;
                        AppConfig.STYLE_MainWndState = this.WindowState;

                        if (AppConfig.APP_ClearTEMPonExit)
                            AppFunc.ClearFolder(AppConfig.Path_Temp);
                        if (Program.Service.UseEKKR)
                        {
                            try
                            {
                                Program.Service.CallFunction("SetCashier", new object[] { UserStruct.UserFLogin, UserStruct.UserFPassword, "" });
                            }
                            catch (Exception ex)
                            {
                                MMessageBox.Show(this, ex.Message + "\r\nНеможливо розреєструвати касира в ЕККР",
                                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        break;
                    }
                default:
                    {
                        DialogResult = DialogResult.None;
                        e.Cancel = true;
                        break;
                    }
            }

            base.OnClosing(e);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            //winapi.WinAPI.OutputDebugString(string.Concat((char)e.KeyValue));

            if (!UserStruct.Properties[22])
            {
                if (lastInputChar.AddMilliseconds(130) < DateTime.Now)
                    chararray = string.Empty;

                if (!e.Alt && !e.Control && !e.Shift)
                {
                    lastInputChar = DateTime.Now;
                    chararray += (char)e.KeyValue; 
                    //winapi.WinAPI.OutputDebugString(chararray);
                }
            }

            if (e.Control && !e.SuppressKeyPress && chequeDGV.CurrentCell != null && chequeDGV.CurrentCell.IsInEditMode)
                chequeDGV.EndEdit();
        }
        protected override void OnActivated(EventArgs e)
        {
            //winapi.API.OutputDebugString("regHotKeys");
            
            Keys myHotKey = Keys.Delete | Keys.Control;
            AppFunc.RegisterHotKey(this, myHotKey, AppFunc.MyHotKeys.HK_CtrlDel);

            myHotKey = Keys.Delete | Keys.Control | Keys.Shift;
            AppFunc.RegisterHotKey(this, myHotKey, AppFunc.MyHotKeys.HK_CtrlShiftDel);

            myHotKey = Keys.PageDown | Keys.Control;
            AppFunc.RegisterHotKey(this, myHotKey, AppFunc.MyHotKeys.HK_CtrlPgDn);

            myHotKey = Keys.PageUp | Keys.Control;
            AppFunc.RegisterHotKey(this, myHotKey, AppFunc.MyHotKeys.HK_CtrlPgUp);

            myHotKey = Keys.Delete | Keys.Shift;
            AppFunc.RegisterHotKey(this, myHotKey, AppFunc.MyHotKeys.HK_ShiftDel);

            myHotKey = Keys.Enter;
            AppFunc.RegisterHotKey(this, myHotKey, AppFunc.MyHotKeys.HK_Enter);

            myHotKey = Keys.Enter | Keys.Control;
            AppFunc.RegisterHotKey(this, myHotKey, AppFunc.MyHotKeys.HK_CtrlEnter);

            myHotKey = Keys.Enter | Keys.Control | Keys.Shift;
            AppFunc.RegisterHotKey(this, myHotKey, AppFunc.MyHotKeys.HK_CtrlShiftEnter);

            myHotKey = Keys.F5;
            AppFunc.RegisterHotKey(this, myHotKey, AppFunc.MyHotKeys.HK_F5);

            myHotKey = Keys.F6;
            AppFunc.RegisterHotKey(this, myHotKey, AppFunc.MyHotKeys.HK_F6);

            myHotKey = Keys.F7;
            AppFunc.RegisterHotKey(this, myHotKey, AppFunc.MyHotKeys.HK_F7);

            myHotKey = Keys.F8;
            AppFunc.RegisterHotKey(this, myHotKey, AppFunc.MyHotKeys.HK_F8);

            myHotKey = Keys.F9;
            AppFunc.RegisterHotKey(this, myHotKey, AppFunc.MyHotKeys.HK_F9);

            myHotKey = Keys.Escape;
            AppFunc.RegisterHotKey(this, myHotKey, AppFunc.MyHotKeys.HK_Esc);

            myHotKey = Keys.Q | Keys.Control;
            AppFunc.RegisterHotKey(this, myHotKey, AppFunc.MyHotKeys.HK_CtrlQ);

            myHotKey = Keys.Control;
            AppFunc.RegisterHotKey(this, myHotKey, AppFunc.MyHotKeys.HK_Ctrl);
            base.OnActivated(e);

            winapi.API.OutputDebugString("regHotKeys");
        }
        protected override void OnDeactivate(EventArgs e)
        {
            //winapi.API.OutputDebugString("un_RegHotKeys");
            for (int i = 0x10; i < 0x20; i++)
                AppFunc.UnregisterHotKey(this, i);
            base.OnDeactivate(e);
            winapi.API.OutputDebugString("un_RegHotKeys");
        }
        /// <summary>
        /// Перевизначений метод обробки повідомлень.
        /// </summary>
        /// <param name="m">Повідомлення</param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            //winapi.WinAPI.OutputDebugString(m.ToString());

            if (m.Msg == (int)AppFunc.MyMsgs.WM_HOTKEY)
            {
                //winapi.Funcs.OutputDebugString("Q");
                #region hot key control
                switch (m.WParam.ToInt32())
                {
                    case 0x10:
                        #region CONTROL + DELETE
                        {
                            if (Cheque.Rows.Count == 0)
                                return;

                            if (!(ADMIN_STATE || UserStruct.Properties[24]))
                                if (admin.ShowDialog() != DialogResult.OK)
                                    return;

                            try
                            {
                                int index = chequeDGV.CurrentRow.Index;
                                Cheque.Rows.RemoveAt(index);
                                //AppFunc.OutputDebugString("k");
                                RowsRemoved_MyEvent(true);
                                index--;
                                if (index < 0)
                                    if (Cheque.Rows.Count != 0)
                                        index = 0;
                                    else
                                        return;
                                chequeDGV.CurrentCell = chequeDGV.Rows[index].Cells[chequeDGV.Columns.GetFirstColumn(DataGridViewElementStates.Visible).Name];
                                chequeDGV.Rows[index].Selected = true;
                            }
                            catch { }
                            break;
                        } 
                        #endregion 
                    case 0x11:
                        #region CONTROL + SHIFT + DELETE
                        {
                            if (Cheque.Rows.Count == 0)
                                return;

                            if (!(ADMIN_STATE || UserStruct.Properties[24]))
                                if (admin.ShowDialog() != DialogResult.OK)
                                    return;

                            Cheque.Rows.Clear();
                            RowsRemoved_MyEvent(true);
                            break;
                        } 
                        #endregion
                    case 0x12:
                        #region CONTROL + PageDown
                        {
                            if (inventChq)
                                return;

                            if (!(ADMIN_STATE || UserStruct.Properties[3]))
                                if (admin.ShowDialog() != DialogResult.OK)
                                    return;

                            double discSUMA = 0.0;
                            try
                            {
                                discSUMA = (double)Cheque.Compute("Sum(SUM)", "USEDDISC = " + Boolean.TrueString);
                            }
                            catch { }
                            DiscountRequest d = new DiscountRequest(discSUMA, true);
                            d.SetDiscount(ref discArrPercent, ref discArrCash);
                            d.Dispose();

                            if (discArrPercent[0] == 0.0 && discArrPercent[1] == 0.0 && discArrCash[0] == 0.0 && discArrCash[1] == 0.0)
                                ResetDiscount();
                            else
                            {
                                відмінитиЗнижкунадбавкуToolStripMenuItem.Enabled = true;
                                if (AppConfig.APP_OnlyDiscount)
                                    відмінитиЗнижкунадбавкуToolStripMenuItem.Text = "Скасувати знижку";
                                else
                                {
                                    if ((discArrPercent[0] != 0.0 && discArrPercent[1] != 0.0) || (discArrCash[0] != 0.0 && discArrCash[1] != 0.0))
                                        відмінитиЗнижкунадбавкуToolStripMenuItem.Text = "Скасувати знижку і націнку";
                                    if ((discArrPercent[0] == 0.0 && discArrPercent[1] != 0.0) || (discArrCash[0] == 0.0 && discArrCash[1] != 0.0))
                                        відмінитиЗнижкунадбавкуToolStripMenuItem.Text = "Скасувати націнку";
                                    if ((discArrPercent[0] != 0.0 && discArrPercent[1] == 0.0) || (discArrCash[0] != 0.0 && discArrCash[1] == 0.0))
                                        відмінитиЗнижкунадбавкуToolStripMenuItem.Text = "Скасувати знижку";
                                }
                            }

                            UpdateSumInfo(true);
                            break;
                        } 
                        #endregion
                    case 0x13:
                        #region CONTROL + PageUp
                        {
                            if (inventChq)
                                return;

                            if (!(ADMIN_STATE || UserStruct.Properties[3]))
                                if (admin.ShowDialog() != DialogResult.OK)
                                    return;

                            double discSUMA = 0;
                            try
                            {
                                discSUMA = (double)Cheque.Compute("Sum(SUM)", "USEDDISC = " + Boolean.TrueString);
                            }
                            catch { }
                            DiscountRequest d = new DiscountRequest(discSUMA, false);
                            d.SetDiscount(ref discArrPercent, ref discArrCash);
                            d.Dispose();

                            if (discArrPercent[0] == 0.0 && discArrPercent[1] == 0.0 && discArrCash[0] == 0.0 && discArrCash[1] == 0.0)
                                ResetDiscount();
                            else
                            {
                                відмінитиЗнижкунадбавкуToolStripMenuItem.Enabled = true;
                                if (AppConfig.APP_OnlyDiscount)
                                    відмінитиЗнижкунадбавкуToolStripMenuItem.Text = "Скасувати націнку";
                                else
                                {
                                    if ((discArrPercent[0] != 0.0 && discArrPercent[1] != 0.0) || (discArrCash[0] != 0.0 && discArrCash[1] != 0.0))
                                        відмінитиЗнижкунадбавкуToolStripMenuItem.Text = "Скасувати знижку і націнку";
                                    if ((discArrPercent[0] == 0.0 && discArrPercent[1] != 0.0) || (discArrCash[0] == 0.0 && discArrCash[1] != 0.0))
                                        відмінитиЗнижкунадбавкуToolStripMenuItem.Text = "Скасувати націнку";
                                    if ((discArrPercent[0] != 0.0 && discArrPercent[1] == 0.0) || (discArrCash[0] != 0.0 && discArrCash[1] == 0.0))
                                        відмінитиЗнижкунадбавкуToolStripMenuItem.Text = "Скасувати знижку";
                                }
                            }

                            UpdateSumInfo(true);
                            break;
                        } 
                        #endregion
                    case 0x14:
                        #region SHIFT + DELETE
                        {
                            if (inventChq)
                                return;

                            ResetDiscount();
                            UpdateSumInfo(true);
                            break;
                        } 
                        #endregion
                    case 0x15:
                        #region ENTER
                        {
                            //for (int i = 0x10; i < 0x20; i++)
                            //AppFunc.UnregisterHotKey(this, i);

                            bool editWasClosed = false;

                            //winapi.WinAPI.OutputDebugString("Enter");

                            if (!UserStruct.Properties[22])
                                if (lastInputChar.AddMilliseconds(130) > DateTime.Now &&
                                    chararray != null && chararray.Length != 0)
                                {
                                    //if (chequeDGV.CurrentCell != null && chequeDGV.CurrentCell.IsInEditMode)
                                    //{
                                    //    if (chequeDGV.CurrentCell.EditedFormattedValue.ToString().Contains(chararray))
                                    //    {
                                    //        string val = chequeDGV.CurrentCell.EditedFormattedValue.ToString();
                                    //        int bcidx = val.IndexOf(chararray);
                                    //        val = val.Substring(0, bcidx);
                                    //        if (val == string.Empty)
                                    //            val = "0";
                                    //        chequeDGV.CurrentCell.Value = Convert.ToDouble(val);
                                    //    }
                                    //    chequeDGV.EndEdit();
                                    //    editWasClosed = true;
                                    //}

                                    //winapi.WinAPI.OutputDebugString("srch: " + chararray);
                                    SearchFilter(false, 2, true);
                                    SrchTbox.Text = chararray;
                                    SrchTbox.Select();
                                    chararray = string.Empty;
                                }

                            //close edit
                            if (chequeDGV.CurrentCell != null && chequeDGV.CurrentCell.IsInEditMode)
                            {
                                chequeDGV.EndEdit();
                                editWasClosed = true;
                            }

                            //lastInputChar = DateTime.Now;
                            //launch article property
                            if (chequeDGV.Focused && chequeDGV.RowCount != 0)
                            {
                                if (!(ADMIN_STATE || UserStruct.Properties[24]))
                                    if (admin.ShowDialog() != DialogResult.OK)
                                        return;

                                DataRow dRow = Cheque.Rows.Find(chequeDGV.CurrentRow.Cells["C"].Value);
                                Request req = new Request(dRow, AppFunc.GetDouble(dRow["TOT"]));
                                req.UpdateRowSource();
                                req.Dispose();
                                UpdateSumInfo(true);
                                return;
                            }

                            //Adding article to Cheque
                            if (articleDGV.Focused && articleDGV.RowCount != 0)
                            {
                                DataRow article = Articles.Rows.Find(articleDGV.CurrentRow.Cells["C"].Value);
                                if (article != null)
                                    AppFunc.AddArticleToCheque(chequeDGV, articleDGV, article, AppConfig.APP_StartTotal, Articles);
                                return;
                            }
                            //winapi.WinAPI.OutputDebugString("srch: " + SrchTbox.Text);

                            //Searching
                            if (!editWasClosed && SrchTbox.Focused && SrchTbox.Text != string.Empty)
                            {
                                DataTable sTable = Articles.Clone();
                                bool allowToShow = false;
                                int i = 0;

                                //Debug.Write("BeginAdd");
                                #region search box
                                if (SrchTbox.Text != "")
                                {
                                    switch (currSrchType)
                                    {
                                        case 0:
                                            {
                                                #region by name
                                                string[] words = SrchTbox.Text.Trim().Split(' ');
                                                DataRow[] dr1 = new DataRow[0];
                                                DataRow[] dr2 = new DataRow[0];
                                                DataTable dTable = (DataTable)articleDGV.DataSource;

                                                //string srchString = string.Empty;
                                                SrchTbox.Text = string.Empty;
                                                for (int l = 0; l < words.Length; l++)
                                                {
                                                    try
                                                    {
                                                        dr1 = dTable.Select("NAME Like '%" + words[l] + "%'");
                                                        dr2 = dTable.Select("DESC Like '%" + words[l] + "%'");
                                                    }
                                                    catch { }

                                                    sTable.Clear();
                                                    sTable.BeginLoadData();

                                                    if (dr1.Length > dr2.Length)
                                                    {
                                                        for (i = 0; i < dr1.Length; i++)
                                                            sTable.Rows.Add(dr1[i].ItemArray);
                                                    }
                                                    else
                                                    {
                                                        for (i = 0; i < dr2.Length; i++)
                                                            sTable.Rows.Add(dr2[i].ItemArray);
                                                    }

                                                    sTable.EndLoadData();

                                                    dTable = sTable.Copy();

                                                    if (dTable.Rows.Count > 0)
                                                    {
                                                        articleDGV.DataSource = dTable;
                                                        articleDGV.Select();
                                                        allowToShow = true;
                                                        SrchTbox.Text += words[l] + " ";
                                                        //SrchTbox.Select(0, srchString.Length);
                                                    }
                                                }

                                                if (SrchTbox.Text == string.Empty)
                                                //if (SrchTbox.SelectedText == string.Empty)
                                                {
                                                    MMessageBox.Show(this, "Нажаль нічого не вдалось знайти", "Результат пошуку",
                                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                    SearchFilter(false, AppConfig.APP_SearchType, true);
                                                }

                                                #endregion
                                                break;
                                            }
                                        case 1:
                                            {
                                                #region by id
                                                try
                                                {
                                                    DataRow[] dr = Articles.Select("ID Like \'" + SrchTbox.Text + "%\'");

                                                    if (dr.Length == 0)
                                                    {
                                                        MMessageBox.Show(this, "Нажаль нічого не вдалось знайти", "Результат пошуку", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                        SearchFilter(false, AppConfig.APP_SearchType, true);
                                                        break;
                                                    }
                                                    if (dr.Length == 1)
                                                    {
                                                        SearchFilter(false, currSrchType, true);
                                                        AppFunc.AddArticleToCheque(chequeDGV, articleDGV, dr[0], AppConfig.APP_StartTotal, Articles);
                                                        allowToShow = false;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        sTable.Clear();
                                                        sTable.BeginLoadData();
                                                        for (i = 0; i < dr.Length; i++)
                                                            sTable.Rows.Add(dr[i].ItemArray);
                                                        sTable.EndLoadData();

                                                        articleDGV.DataSource = sTable;
                                                        articleDGV.Select();
                                                        allowToShow = true;
                                                    }

                                                }
                                                catch
                                                {
                                                    SrchTbox.Focus();
                                                    SrchTbox.SelectAll();
                                                }
                                                #endregion
                                                break;
                                            }
                                        case 2:
                                            {
                                                #region by bc
                                                try
                                                {
                                                    allowToShow = BCSearcher(SrchTbox.Text, true);
                                                }
                                                catch (FormatException)
                                                {
                                                    SrchTbox.Focus();
                                                    SrchTbox.SelectAll();
                                                }
                                                #endregion
                                                break;
                                            }
                                    }
                                }
                                #endregion

                                if (splitContainer1.Panel2Collapsed && allowToShow)
                                {
                                    вікноТоварівToolStripMenuItem.PerformClick();
                                    splitContainer1.Panel2.Tag = new object();
                                    articleDGV.Select();
                                }

                                articleDGV.Update();
                                //Debug.Write("EndAdd");
                            }

                            break;
                        } 
                        #endregion
                    case 0x16:
                        #region CONTROL + ENTER
                        {
                            if (inventChq || Cheque.Rows.Count == 0)
                                return;

                            if (!(ADMIN_STATE || UserStruct.Properties[23]))
                                if (admin.ShowDialog() != DialogResult.OK)
                                    return;

                            CloseCheque(Program.Service.UseEKKR);
                            break;
                        } 
                        #endregion
                    case 0x17:
                        #region CONTROL + SHIFT + ENTER
                        {
                            if (inventChq)
                                return;

                            if (Cheque.Rows.Count == 0 && UserStruct.Properties[12])
                            {
                                string nextChqNom = "";
                                object[] localData = AppFunc.NonFxChqsInfo(0, ref nextChqNom);
                                DDM_Status.Text = string.Format("За {1} продано {0} чек(ів) на суму {2:F" + AppConfig.APP_MoneyDecimals + "}", localData[0], localData[1], double.Parse(localData[2].ToString()));
                                return;
                            }
                            
                            if (Cheque.Rows.Count == 0/* || !Program.Service.UseEKKR*/)
                                return;

                            if (AppConfig.APP_IllegalMsgPrompt &&
                                DialogResult.Yes != MMessageBox.Show(this, "Закрити чек без фіксації оплати",
                                Application.ProductName,
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question,
                                MessageBoxDefaultButton.Button1))
                                return;

                            if (!(ADMIN_STATE || (UserStruct.Properties[23] && UserStruct.Properties[6])))
                                if (admin.ShowDialog() != DialogResult.OK)
                                    return;

                            CloseCheque(false);
                            break;
                        } 
                        #endregion
                    case 0x18:
                        #region F5
                        {
                            if (!AppConfig.APP_SrchTypesAccess[0])
                            {
                                MMessageBox.Show(this, "Пошук по назві не дозволений", Application.ProductName,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            if (currSrchType != 0)
                                SearchFilter(false, 0, true);
                            else
                            {
                                SrchTbox.Focus();
                                SrchTbox.Select(SrchTbox.Text.Length, 0);
                            }
                            break;
                        } 
                        #endregion
                    case 0x19:
                        #region F6
                        {
                            if (!AppConfig.APP_SrchTypesAccess[1])
                            {
                                MMessageBox.Show(this, "Пошук по коду не дозволений", Application.ProductName,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            if (currSrchType != 1)
                                SearchFilter(false, 1, true);
                            else
                            {
                                SrchTbox.Focus();
                                SrchTbox.Select(SrchTbox.Text.Length, 0);
                            }
                            break;
                        } 
                        #endregion
                    case 0x1A:
                        #region F7
                        {
                            //winapi.WinAPI.OutputDebugString("F7");
                            if (!AppConfig.APP_SrchTypesAccess[2])
                            {
                                MMessageBox.Show(this, "Пошук по штрих-коду не дозволений", Application.ProductName,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            if (currSrchType != 2)
                                SearchFilter(false, 2, true);
                            else
                            {
                                SrchTbox.Focus();
                                SrchTbox.Select(0, SrchTbox.Text.Length);
                            }
                            break;
                        } 
                        #endregion
                    case 0x1B:
                        #region F8
                        {
                            if (Cheque.ExtendedProperties.Contains("BILL"))
                                MMessageBox.Show(this, "Відкритий рахунок №" + " " + Cheque.ExtendedProperties["NOM"], Application.ProductName,
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        } 
                        #endregion
                    case 0x1C:
                        #region F9
                        {
                            string infoText = string.Empty;
                            UserSchema us = new UserSchema();
                            infoText += UserStruct.UserID;
                            infoText += "\r\n\r\n--------------------------------------------------\r\n\r\n";
                            for (int i = 0; i < UserSchema.ItemsCount; i++)
                                infoText += us.SchemaItems[i] + " : " + (UserStruct.Properties[i] ? "Так" : "Ні") + "\r\n";
                            MMessageBox.Show(infoText, UserStruct.UserID);
                            break;
                        } 
                        #endregion
                    case 0x1D:
                        #region ESCAPE
                        {
                            SearchFilter(false, AppConfig.APP_SearchType, true);
                            break;
                        } 
                        #endregion
                    case 0x1E:
                        #region CONTROL + Q
                        {
                            if (inventChq)
                                return;

                            nakladna = !nakladna;

                            if (nakladna)
                                CashLbl.Image = Properties.Resources.naklad;
                            else
                                CashLbl.Image = null;
                            break;
                        } 
                        #endregion
                    case 0x1F:
                        #region CONTROL
                        {
                            if (chequeDGV.CurrentCell != null && chequeDGV.CurrentCell.IsInEditMode)
                                chequeDGV.EndEdit();
                            break;
                        } 
                        #endregion
                }
                #endregion
                //winapi.Funcs.OutputDebugString("W");
            }

            if (m.Msg == (int)AppFunc.MyMsgs.WM_UPDATE)
            {
                if (_fl_canUpdate)
                    this.timer1_Tick(this.timer1, EventArgs.Empty);
            }
        }

        private void UpdateMyControls()
        {
            //winapi.Funcs.OutputDebugString("UpdateMyControls_begin");
            RefreshAppInformer();
            RefreshChequeInformer(true);
            RefershStyles();
            RefershMenus();
            RefreshWindowMenu();

            timer1.Interval = AppConfig.APP_RefreshRate;

            if (currentSubUnit != AppConfig.APP_SubUnit)
            {
                _fl_onlyUpdate = false;
                _fl_SubUnitChanged = true;
                timer1_Tick(timer1, EventArgs.Empty);
                currentSubUnit = AppConfig.APP_SubUnit;
            }
            //winapi.Funcs.OutputDebugString("UpdateMyControls_end");
        }
        #region InitCtrl SubMethods
        private void RefreshAppInformer()
        {
            appInfoLabel.Text = string.Format("{0}: {1}     {2}: \"{3}\"     {4}: {5}     {6}: \"{7}\"",
                "Підрозділ №",
                AppConfig.APP_SubUnit,
                "Назва підрозділу",
                AppConfig.APP_SubUnitName == string.Empty ? "без назви" : AppConfig.APP_SubUnitName,
                "Каса №",
                AppConfig.APP_PayDesk,
                "Касир",
                UserStruct.UserID);
        }//ok//label
        private void RefreshChequeInformer(bool resetDigitalPanel)
        {
            if (inventChq)
            {
                CashLbl.Text = string.Format("{0}", "ІНВЕНТАРИЗАЦІЯ"); ;
                chequeInfoLabel.Text = string.Format("{0}", Cheque.ExtendedProperties["Date"]);
            }
            else
            {
                string ctrlWord = "чеку";
                if (Cheque.ExtendedProperties["BILL"] != null)
                    ctrlWord = "рахунку";
                string totalWord = "позиці";
                int numValue = Cheque.Rows.Count;

                while (numValue > 20)
                    numValue %= 10;

                switch (numValue)
                {
                    case 1: totalWord += 'я'; break;
                    case 2: totalWord += 'ї'; break;
                    case 3: totalWord += 'ї'; break;
                    case 4: totalWord += 'ї'; break;
                    default: totalWord += 'й'; break;
                }
                
                if (retriveChq)
                    chequeInfoLabel.Text = string.Format("{0} {1} {2} {3} {4}", "В", ctrlWord, Cheque.Rows.Count, totalWord, "повертається на суму");
                else
                    chequeInfoLabel.Text = string.Format("{0} {1} {2} {3} {4}", "В", ctrlWord, Cheque.Rows.Count, totalWord, "продається на суму");
                CashLbl.Text = string.Format("{0:F" + AppConfig.APP_MoneyDecimals + "}", realSUMA);
            }

            if(resetDigitalPanel)
            {
                CashLbl.ForeColor = AppConfig.STYLE_SumFontColor;
                CashLbl.Font = AppConfig.STYLE_SumFont;

                CashLbl.Image = null;
                digitalPanel.BackgroundImage = null;
                nakladna = false;
            }
        }
        private void RefershStyles()
        {
            //Colors
            infoPanel.BackColor = AppConfig.STYLE_BackgroundInfPan;
            addChequeInfo.BackColor = AppConfig.STYLE_BackgroundAddPan;
            digitalPanel.BackColor = AppConfig.STYLE_BackgroundSumRest;
            chequeDGV.BackgroundColor = AppConfig.STYLE_BackgroundNAChqTbl;
            chequeDGV.DefaultCellStyle.BackColor = AppConfig.STYLE_BackgroundNAChqTbl;
            articleDGV.BackgroundColor = AppConfig.STYLE_BackgroundArtTbl;
            articleDGV.DefaultCellStyle.BackColor = AppConfig.STYLE_BackgroundArtTbl;
            statusStrip1.BackColor = AppConfig.STYLE_BackgroundStatPan;

            //Fonts
            CashLbl.Font = AppConfig.STYLE_SumFont;
            CashLbl.ForeColor = AppConfig.STYLE_SumFontColor;
            articleDGV.Font = AppConfig.STYLE_ArticlesFont;
            articleDGV.ForeColor = AppConfig.STYLE_ArticlesFontColor;
            chequeDGV.Font = AppConfig.STYLE_ChequeFont;
            chequeDGV.ForeColor = AppConfig.STYLE_ChequeFontColor;
            statusStrip1.Font = AppConfig.STYLE_StatusFont;
            statusStrip1.ForeColor = AppConfig.STYLE_StatusFontColor;
            addChequeInfo.Font = AppConfig.STYLE_AddInformerFont;
            addChequeInfo.ForeColor = AppConfig.STYLE_AddInformerFontColor;
            chequeInfoLabel.Font = AppConfig.STYLE_ChqInformerFont;
            chequeInfoLabel.ForeColor = AppConfig.STYLE_ChqInformerFontColor;
            appInfoLabel.Font = AppConfig.STYLE_AppInformerFont;
            appInfoLabel.ForeColor = AppConfig.STYLE_AppInformerFontColor;
        }//ok
        private void RefershMenus()
        {
            fxFunc_toolStripMenuItem.Enabled = Program.Service.PublicFunctions[0].Length != 0 && Program.Service.UseEKKR;
            адміністраторToolStripMenuItem.Checked = ADMIN_STATE;
            фільтрОдиницьToolStripMenuItem.Enabled = !inventChq && Cheque.Rows.Count == 0 && (ADMIN_STATE || UserStruct.Properties[9]);
            формуванняЧекуToolStripMenuItem.Enabled = !inventChq && Cheque.Rows.Count == 0 && ADMIN_STATE;
            інвентаризаціяToolStripMenuItem.Enabled = (inventChq || Cheque.Rows.Count == 0) && ADMIN_STATE ;
            чекПоверненняToolStripMenuItem.Enabled = !inventChq && Cheque.Rows.Count == 0 && (ADMIN_STATE || UserStruct.Properties[5]);
            налаштуванняToolStripMenuItem.Enabled = !inventChq && Cheque.Rows.Count == 0 && ADMIN_STATE;
            параметриДрукуToolStripMenuItem.Enabled = !inventChq && Cheque.Rows.Count == 0 && ADMIN_STATE;
            змінитиКористувачаToolStripMenuItem.Enabled = !inventChq && Cheque.Rows.Count == 0;
            вихідToolStripMenuItem.Enabled = !inventChq && Cheque.Rows.Count == 0;

            друкуватиРахунокToolStripMenuItem.Enabled = Cheque.ExtendedProperties.Contains("BILL");
            анулюватиРахунокToolStripMenuItem.Enabled = Cheque.ExtendedProperties.Contains("BILL");
            зберегтиРахунокToolStripMenuItem.Enabled = Cheque.Rows.Count != 0 && !inventChq;
            всіРахункиToolStripMenuItem.Enabled = !inventChq;

            змінитиКстьТоваруToolStripMenuItem.Enabled = Cheque.Rows.Count != 0 && (ADMIN_STATE || UserStruct.Properties[24]);
            видалитиВибранийТоварToolStripMenuItem.Enabled = Cheque.Rows.Count != 0 && (ADMIN_STATE || UserStruct.Properties[24]);
            видалитиВсіТовариToolStripMenuItem.Enabled = Cheque.Rows.Count != 0 && (ADMIN_STATE || UserStruct.Properties[24]);                
            здійснитиОплатуToolStripMenuItem.Enabled = !inventChq && Cheque.Rows.Count != 0 && (ADMIN_STATE || UserStruct.Properties[23]);
            задатиЗнижкаToolStripMenuItem.Enabled = !inventChq && Cheque.Rows.Count != 0 && (ADMIN_STATE || UserStruct.Properties[3]);
            задатиНадбавкуToolStripMenuItem1.Enabled = !inventChq && Cheque.Rows.Count != 0 && (ADMIN_STATE || UserStruct.Properties[3]);
        }//ok
        private void RefreshWindowMenu()
        {
            if (splitContainer1.Orientation == Orientation.Horizontal)
            {
                горизонтальноToolStripMenuItem.Checked = true;
                вертикальноToolStripMenuItem.Checked = false;
            }
            else
            {
                горизонтальноToolStripMenuItem.Checked = false;
                вертикальноToolStripMenuItem.Checked = true;
            }

            вікноТоварівToolStripMenuItem.Checked = !splitContainer1.Panel2Collapsed;
        }
        #endregion

        /// <summary>
        /// Обробляє виконання операцій відповідно до того, який пункт меню був вибраний
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (_fl_menuIsActive || e.ClickedItem.Tag == null) 
                return;

            _fl_menuIsActive = true;
            switch (e.ClickedItem.Tag.ToString())
            {
                #region Main
                case "fiscalFunctions":
                    {
                        FiscalFunctions ff = new FiscalFunctions(Program.Service.ComPort.Tag, Program.Service.PublicFunctions);
                        ff.ShowDialog(this);
                        ff.Dispose();
                        try
                        {
                            if (ff.DialogResult == DialogResult.OK)
                                Program.Service.CallFunction(ff.Function, ff.Descriprion, null);
                        }
                        catch (Exception ex)
                        {
                            MMessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case "LastDBChanges":
                    {
                        BaseChanges DBChanges = new BaseChanges();
                        DBChanges.ShowDialog();
                        DBChanges.Dispose();

                        if (DBChanges.DialogResult == DialogResult.OK)
                            timer1_Tick(timer1, EventArgs.Empty);
                        break;
                    }
                case "Administrator":
                    {
                        DialogResult rez = DialogResult.None;
                        if (ADMIN_STATE)
                            rez = MMessageBox.Show(this, "Вийти з режиму адміністратора", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        else
                            rez = admin.ShowDialog();

                        switch (rez)
                        {
                            case DialogResult.OK:
                                ADMIN_STATE = true;
                                break;
                            case DialogResult.Yes:
                                ADMIN_STATE = false;
                                break;
                        }

                        RefershMenus();
                        break;
                    }
                case "UnitFilter":
                    {
                        UnitFilter fl = new UnitFilter(Articles);
                        fl.ShowDialog();
                        fl.Dispose();
                        break;
                    }
                case "ChequeFormat":
                    {
                        DiscountSettings billRul = new DiscountSettings();
                        billRul.ShowDialog();
                        billRul.Dispose();
                        ResetDiscount();
                        UpdateSumInfo(true);
                        break;
                    }
                case "Invent":
                    {
                        if (Cheque.Rows.Count != 0 && !inventChq)
                            return;

                        inventChq = !inventChq;

                        if (inventChq)
                        {
                            DataTable dTable = AppFunc.OpenInvent();
                            if (dTable != null)
                            {
                                dTable.ExtendedProperties.Add("loading", true);
                                Cheque.Merge(dTable);
                                Cheque.ExtendedProperties.Remove("loading");
                                dTable.ExtendedProperties.Remove("loading");
                            }
                            else
                                inventChq = false;
                        }
                        else
                        {
                            AppFunc.SaveInvent(Cheque, false);
                            Cheque.Rows.Clear();
                        }

                        RefershMenus();
                        RefreshChequeInformer(true);
                        інвентаризаціяToolStripMenuItem.Checked = inventChq;
                        break;
                    }
                case "RetriveCheque":
                    {
                        retriveChq = !retriveChq;
                        чекПоверненняToolStripMenuItem.Checked = retriveChq;
                        RefreshChequeInformer(true);
                        break;
                    }
                case "Settings":
                    {
                        Settings set = new Settings();
                        set.ShowDialog();
                        set.Dispose();

                        if (set.DialogResult == DialogResult.OK)
                        {
                            UpdateMyControls();
                            SearchFilter(false, AppConfig.APP_SearchType, true);
                        }
                        break;
                    }
                case "PrintingSettings":
                    {
                        Printing pSet = new Printing();
                        pSet.ShowDialog();
                        pSet.Dispose();
                        break;
                    }
                case "Registration":
                    {
                        Registration rf = new Registration();
                        rf.ShowDialog();
                        rf.Dispose();
                        break;
                    }
                case "AboutApp":
                    {
                        AboutBox abox = new AboutBox();
                        abox.ShowDialog();
                        abox.Dispose();
                        break;
                    }
                case "ChangeCashier":
                    {
                        DialogResult = DialogResult.Retry;
                        Close();
                        break;
                    }
                case "Exit":
                    {
                        DialogResult = DialogResult.Cancel;
                        Close();
                        break;
                    } 
                #endregion
                #region TablesView
                case "Horizontal":
                    {
                        splitContainer1.Orientation = Orientation.Horizontal;
                        AppConfig.STYLE_SplitOrient = Orientation.Horizontal;
                        RefreshWindowMenu();
                        break;
                    }
                case "Vertical":
                    {
                        splitContainer1.Orientation = Orientation.Vertical;
                        AppConfig.STYLE_SplitOrient = Orientation.Vertical;
                        RefreshWindowMenu();
                        break;
                    }
                case "ArticleWindow":
                    {
                        splitContainer1.Panel2Collapsed = !splitContainer1.Panel2Collapsed;
                        AppConfig.STYLE_ArtSideCollapsed = !AppConfig.STYLE_ArtSideCollapsed;
                        RefreshWindowMenu();
                        break;
                    }
                #endregion
                #region Bills
                case "ResetBill":
                    {
                        if (BillsList.DeleteBill(Cheque))
                            RowsRemoved_MyEvent(true);
                        break;
                    }
                case "AllBills":
                    {
                        BillsList bl = new BillsList();
                        bl.ShowDialog();
                        bl.Dispose();

                        if (bl.DialogResult == DialogResult.OK)
                            if (Cheque.Rows.Count == 0)
                            {
                                Cheque.Merge(bl.LoadedBill);
                                UpdateSumInfo(true);
                            }
                            else MMessageBox.Show(this, "Неможливо відкрити рахунок\nТаблиця чеку не є порожня", Application.ProductName,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                case "PrintNewBill":
                    {
                        object[] printerData = new object[21];
                        //Копія таблиці типу DataTable з записами товарів
                        printerData[0] = Cheque.Copy();
                        //Номер чеку
                        /*printerData[1] = 0;
                        //Якщо true то цей чек є чеком поверення
                        printerData[2] = false;
                        //Якщо true то чек є фіскальний
                        printerData[3] = false;
                        //Сума товарів без знижки чи надбавки
                        printerData[4] = chqSUMA;
                        //Сума товарів з зщнижкою або надбавкою
                        printerData[5] = realSUMA;
                        //Тип закриття чеку (Готівка, чек, кредит, картка)
                        printerData[6] = new List<byte>();
                        //Загальна сума грошей покупця
                        printerData[7] = 0;
                        //Сума грошей з кожного типу оплати
                        printerData[8] = new List<double>();
                        //Здача
                        printerData[9] = 0;
                        //Якщо true то знижка чи надбавка діє на всі позиції(товари) чеку
                        printerData[10] = _fl_useTotDisc;
                        //Масив з значеннями знижки та надбавки в процентних значеннях
                        printerData[11] = new double[2] { discArrPercent[0], discArrPercent[1] };
                        //Масив з значеннями знижки та надбавки в грошових значеннях
                        printerData[12] = new double[2] { discArrCash[0], discArrCash[1] };
                        //Значення постійної знижки в процентному значенні
                        printerData[13] = discConstPercent;
                        //
                        printerData[14] = null;
                        //Сума знижки і надбавки з процентними значеннями
                        printerData[15] = discOnlyPercent;
                        //Сума знижки і надбавки з грошовими значеннями
                        printerData[16] = discOnlyCash;
                        //Загальний коефіціент знижки в процентному значенні
                        printerData[17] = discCommonPercent;
                        //Загальний коефіціент знижки в грошовому значенні
                        printerData[18] = discCommonCash;
                        //Номер рахунку
                        printerData[19] = Cheque.ExtendedProperties["NOM"].ToString();
                        //Коментр рахунку
                        printerData[20] = Cheque.ExtendedProperties["CMT"].ToString();*/

                        AppFunc.Print(printerData, "kitchen", 1);

                        for (int i = 0; i < Cheque.Rows.Count; i++)
                            Cheque.Rows[i]["PRINTCOUNT"] = Convert.ToDouble(Cheque.Rows[i]["TOT"]);
                       
                        break;
                    }
                case "PrintAllBill":
                    {
                        object[] printerData = new object[21];
                        //Копія таблиці типу DataTable з записами товарів
                        printerData[0] = Cheque.Copy();
                        //Номер чеку
                        /*printerData[1] = 0;
                        //Якщо true то цей чек є чеком поверення
                        printerData[2] = false;
                        //Якщо true то чек є фіскальний
                        printerData[3] = false;
                        //Сума товарів без знижки чи надбавки
                        printerData[4] = chqSUMA;
                        //Сума товарів з зщнижкою або надбавкою
                        printerData[5] = realSUMA;
                        //Тип закриття чеку (Готівка, чек, кредит, картка)
                        printerData[6] = new List<byte>();
                        //Загальна сума грошей покупця
                        printerData[7] = 0;
                        //Сума грошей з кожного типу оплати
                        printerData[8] = new List<double>();
                        //Здача
                        printerData[9] = 0;
                        //Якщо true то знижка чи надбавка діє на всі позиції(товари) чеку
                        printerData[10] = _fl_useTotDisc;
                        //Масив з значеннями знижки та надбавки в процентних значеннях
                        printerData[11] = new double[2] { discArrPercent[0], discArrPercent[1] };
                        //Масив з значеннями знижки та надбавки в грошових значеннях
                        printerData[12] = new double[2] { discArrCash[0], discArrCash[1] };
                        //Значення постійної знижки в процентному значенні
                        printerData[13] = discConstPercent;
                        //
                        printerData[14] = null;
                        //Сума знижки і надбавки з процентними значеннями
                        printerData[15] = discOnlyPercent;
                        //Сума знижки і надбавки з грошовими значеннями
                        printerData[16] = discOnlyCash;
                        //Загальний коефіціент знижки в процентному значенні
                        printerData[17] = discCommonPercent;
                        //Загальний коефіціент знижки в грошовому значенні
                        printerData[18] = discCommonCash;
                        //Номер рахунку
                        printerData[19] = Cheque.ExtendedProperties["NOM"].ToString();
                        //Коментр рахунку
                        printerData[20] = Cheque.ExtendedProperties["CMT"].ToString();*/

                        AppFunc.Print(printerData, "default", 1);

                        for (int i = 0; i < Cheque.Rows.Count; i++)
                            Cheque.Rows[i]["PRINTCOUNT"] = Convert.ToDouble(Cheque.Rows[i]["TOT"]);

                        break;
                    }
                case "SaveBill":
                    {
                        if (inventChq)
                            return;

                        BillSave bs = new BillSave(Cheque);
                        bs.ShowDialog();
                        bs.Dispose();

                        if (bs.DialogResult == DialogResult.OK)
                        {
                            Cheque.Rows.Clear();
                            RowsRemoved_MyEvent(true);
                        }
                        break;
                    }
                #endregion
            }

            _fl_menuIsActive = false;
        }
        private void Context_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Tag == null)
                return;

            switch (((ContextMenuStrip)sender).Tag.ToString())
            {
                case "ChequeTableContext": chequeDGV.Select(); break;
                case "ArticleTableContext": articleDGV.Select(); break;
                case "ColumnsTableContext": break;
            }

            switch (e.ClickedItem.Tag.ToString())
            {
                #region ChequeContextMenu
                case "ChangeDose":
                    {
                        // Send ENTER Key
                        winapi.API.SendMessage(this.Handle, (uint)AppFunc.MyMsgs.WM_HOTKEY, new IntPtr((int)AppFunc.MyHotKeys.HK_Enter), new IntPtr(0));
                        break;
                    }
                case "DeleteArticle":
                    {
                        // Send CTRL+ENTER Key
                        winapi.API.SendMessage(this.Handle, (uint)AppFunc.MyMsgs.WM_HOTKEY, new IntPtr((int)AppFunc.MyHotKeys.HK_CtrlDel), new IntPtr(0));
                        break;
                    }
                case "DeleteAllArticles":
                    {
                        // Send CTRL+SHIFT+DEL Key
                        winapi.API.SendMessage(this.Handle, (uint)AppFunc.MyMsgs.WM_HOTKEY, new IntPtr((int)AppFunc.MyHotKeys.HK_CtrlShiftDel), new IntPtr(0));
                        break;
                    }
                case "Payment":
                    {
                        // Send CTRL+ENTER Key
                        winapi.API.SendMessage(this.Handle, (uint)AppFunc.MyMsgs.WM_HOTKEY, new IntPtr((int)AppFunc.MyHotKeys.HK_CtrlEnter), new IntPtr(0));
                        break;
                    }
                case "SetDiscount":
                    {
                        // Send CTRL+PgDn Key
                        winapi.API.SendMessage(this.Handle, (uint)AppFunc.MyMsgs.WM_HOTKEY, new IntPtr((int)AppFunc.MyHotKeys.HK_CtrlPgDn), new IntPtr(0));
                        break;
                    }
                case "SetUpcount":
                    {
                        // Send CTRL+PgUp Key
                        winapi.API.SendMessage(this.Handle, (uint)AppFunc.MyMsgs.WM_HOTKEY, new IntPtr((int)AppFunc.MyHotKeys.HK_CtrlPgUp), new IntPtr(0));
                        break;
                    }
                case "CancelDiscount":
                    {
                        // Send ENTER Key
                        winapi.API.SendMessage(this.Handle, (uint)AppFunc.MyMsgs.WM_HOTKEY, new IntPtr((int)AppFunc.MyHotKeys.HK_ShiftDel), new IntPtr(0));
                        break;
                    }
                #endregion
                #region ArticleContextMenu
                case "SelectArticle":
                    {
                        // Send ENTER Key
                        winapi.API.SendMessage(this.Handle, (uint)AppFunc.MyMsgs.WM_HOTKEY, new IntPtr((int)AppFunc.MyHotKeys.HK_Enter), new IntPtr(0));
                        break;
                    }
                case "SearchByName":
                    {
                        // Send F5 Key
                        winapi.API.SendMessage(this.Handle, (uint)AppFunc.MyMsgs.WM_HOTKEY, new IntPtr((int)AppFunc.MyHotKeys.HK_F5), new IntPtr(0));
                        break;
                    }
                case "SearchByCode":
                    {
                        // Send F6 Key
                        winapi.API.SendMessage(this.Handle, (uint)AppFunc.MyMsgs.WM_HOTKEY, new IntPtr((int)AppFunc.MyHotKeys.HK_F6), new IntPtr(0));
                        break;
                    }
                case "SearchByBarCode":
                    {
                        // Send F7 Key
                        winapi.API.SendMessage(this.Handle, (uint)AppFunc.MyMsgs.WM_HOTKEY, new IntPtr((int)AppFunc.MyHotKeys.HK_F7), new IntPtr(0));
                        break;
                    } 
                #endregion
                #region FieldContext
                case "FieldEditor":
                    {
                        ColumnsEditorBox colEd = null;

                        if (chequeDGV.NewRowIndex == -1 && chequeDGV.Focused)
                            colEd = new ColumnsEditorBox(ref chequeDGV, 1);
                        if (articleDGV.NewRowIndex == -1 && articleDGV.Focused)
                            colEd = new ColumnsEditorBox(ref articleDGV, 2);
                        
                        colEd.ShowDialog();
                        colEd.Dispose();

                        AppFunc.SaveGridsView(new DataGridView[] { chequeDGV, articleDGV }, splitContainer1.Orientation);
                        break;
                    }
                case "FieldLock":
                    {
                        закріпитиToolStripMenuItem.Checked = !закріпитиToolStripMenuItem.Checked;

                        if (chequeDGV.Focused)
                        {
                            chequeDGV.AllowUserToOrderColumns = !закріпитиToolStripMenuItem.Checked;
                            chequeDGV.AllowUserToResizeColumns = !закріпитиToolStripMenuItem.Checked;
                            AppConfig.STYLE_ChqColumnLock = закріпитиToolStripMenuItem.Checked;
                            break;
                        }
                        if (articleDGV.Focused)
                        {
                            articleDGV.AllowUserToOrderColumns = !закріпитиToolStripMenuItem.Checked;
                            articleDGV.AllowUserToResizeColumns = !закріпитиToolStripMenuItem.Checked;
                            AppConfig.STYLE_ArtColumnLock = закріпитиToolStripMenuItem.Checked;
                        }
                        break;
                    }
                case "SaveFieldPositions":
                    {
                        AppFunc.SaveGridsView(new DataGridView[] { chequeDGV, articleDGV }, splitContainer1.Orientation);
                        break;
                    } 
                #endregion
            }
        }

        #region Table events status : ok
        //CHEQUE DataGridView
        private void chequeDGV_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (Cheque.ExtendedProperties.Contains("loading"))
                return;

            if (Cheque.Rows.Count % AppConfig.APP_InvAutoSave == 0 && inventChq)
                AppFunc.SaveInvent(Cheque, true);

            if (chequeDGV.Rows.Count == 1)
                RefershMenus();

            if (!inventChq)
                RefreshChequeInformer(Cheque.Rows.Count == 1);
        }//ok
        private void chequeDGV_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            //winapi.Funcs.OutputDebugString("1");
            chequeDGV.Rows[e.RowIndex].Selected = true;
            //winapi.Funcs.OutputDebugString("2");
        }
        private void chequeDGV_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //winapi.WinAPI.OutputDebugString("3");
            double addedTot = AppFunc.GetDouble(chequeDGV["TOT", e.RowIndex].Value.ToString());
            addedTot = AppFunc.GetRoundedDose(addedTot);

            if (addedTot <= 0)
            {
                MMessageBox.Show(this, "Помилкове значення кількості", Application.ProductName);
                chequeDGV.BeginEdit(true);
            }
            else
            {
                double thisTot = addedTot + AppFunc.GetDouble(chequeDGV["TMPTOT", e.RowIndex].Value);
                thisTot = AppFunc.GetRoundedDose(thisTot);
                double price = AppFunc.GetDouble(chequeDGV["PRICE", e.RowIndex].Value.ToString());
               /* if (UserStruct.Properties[8])
                {
                    DataRow dRow = Cheque.Rows.Find(chequeDGV.CurrentRow.Cells["C"].Value);
                    price = AppFunc.AutomaticPrice(thisTot, dRow);
                }*/
                double sum = AppFunc.GetRoundedMoney(thisTot * price);

                chequeDGV["TOT", e.RowIndex].Value = thisTot;
                chequeDGV["TMPTOT", e.RowIndex].Value = thisTot;
                chequeDGV["PRICE", e.RowIndex].Value = price;
                chequeDGV["SUM", e.RowIndex].Value = sum;
                chequeDGV["ASUM", e.RowIndex].Value = sum;
                chequeDGV.Update();

                UpdateSumInfo(true);
                SrchTbox.Select();
                SrchTbox.SelectAll();

                try
                {
                    DataRow[] dr = Articles.Select("ID like '" + chequeDGV.CurrentRow.Cells["TID"].Value + "'");
                    if (dr != null && dr.Length != 0 && dr[0] != null)
                    {
                        thisTot = AppFunc.GetDouble(chequeDGV["TQ", e.RowIndex].Value);
                        if (thisTot != 0)
                            addedTot *= AppFunc.GetDouble(chequeDGV["TQ", e.RowIndex].Value);
                        AppFunc.AddArticleToCheque(chequeDGV, articleDGV, dr[0], addedTot, Articles);
                    }
                }
                catch { }
            }
            //winapi.Funcs.OutputDebugString("4");
        }//OK

        //COMMON EVENTS OF TABLES
        private void DGV_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            (sender as DataGridView).Select();

            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex < 0)
                {
                    if ((sender as DataGridView).Name == "chequeDGV")
                        закріпитиToolStripMenuItem.Checked = AppConfig.STYLE_ChqColumnLock;
                    else
                        закріпитиToolStripMenuItem.Checked = AppConfig.STYLE_ArtColumnLock;
                    columnsEditor.Show(Control.MousePosition);
                    return;
                }
                if ((sender as DataGridView).Name == "chequeDGV")
                    chequeContextMenu.Show(Control.MousePosition);
                else
                    articleContextMenu.Show(Control.MousePosition);
            }

            if (e.RowIndex < 0)
                return;

            (sender as DataGridView).CurrentCell = (sender as DataGridView)[e.ColumnIndex, e.RowIndex];
            DDM_Status.Text = AppFunc.ShowArticleInfo(chequeDGV, articleDGV);
        }
        private void DGV_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            (sender as DataGridView).Select();

            if (e.RowIndex < 0)
                return;

            (sender as DataGridView).CurrentCell = (sender as DataGridView)[e.ColumnIndex, e.RowIndex];
            DDM_Status.Text = AppFunc.ShowArticleInfo(chequeDGV, articleDGV);

            // Send ENTER Key
            API.SendMessage(this.Handle, (uint)AppFunc.MyMsgs.WM_HOTKEY, new IntPtr((int)AppFunc.MyHotKeys.HK_Enter), new IntPtr(0));
        }
        private void DGV_Enter(object sender, EventArgs e)
        {
            if ((sender as DataGridView).Name == "chequeDGV")
                chequeDGV.BackgroundColor = AppConfig.STYLE_BackgroundAChqTbl;

            (sender as DataGridView).DefaultCellStyle.SelectionBackColor = Color.FromKnownColor(KnownColor.Highlight);
        }//ok
        private void DGV_Leave(object sender, EventArgs e)
        {
            if ((sender as DataGridView).Name == "chequeDGV")
                chequeDGV.BackgroundColor = AppConfig.STYLE_BackgroundNAChqTbl;
            (sender as DataGridView).DefaultCellStyle.SelectionBackColor = Color.FromKnownColor(KnownColor.InactiveCaption);
        }//ok
        #endregion

        private void timer1_Tick(object sender, EventArgs e)//lbl
        {
            timer1.Stop();
            this.Update();

            if (Cheque.Rows.Count != 0)
            {
                _fl_canUpdate = true;
                timer1.Start();
                return;
            }

            _fl_canUpdate = false;

            string[] files = AppFunc.CheckForUpdate();

            if (files[0] == "lanError")
                DDM_UpdateStatus.Image = Properties.Resources.ExNotOk;
            else
                DDM_UpdateStatus.Image = Properties.Resources.ok;

            if ((files[0] == "lanError" || files[0] == "") && files[1] == "" && files[2] == "" && _fl_onlyUpdate)
            {
                timer1.Start();
                GC.Collect();
                return;
            }

            UpdateWnd uw = new UpdateWnd(_fl_onlyUpdate);
            uw.ShowUpdate(this);
            uw.Update();
            uw.Refresh();

            files = AppFunc.LoadFilesOnLocalTempFolder(files);
            object[] loadResult = AppFunc.LoadData(files, _fl_onlyUpdate);

            AppConfig.SaveData();

            DataTable[] tables = (DataTable[])loadResult[0];
            _fl_artUpdated = (bool)loadResult[1];

            if (_fl_SubUnitChanged)
            {
                Articles.Rows.Clear();
                AltBC.Rows.Clear();
            }

            if (tables[0] != null)
            {
                Articles.Rows.Clear();
                Articles.Merge(tables[0]);
            }
            if (tables[1] != null)
            {
                AltBC.Rows.Clear();
                AltBC.Merge(tables[1]);
            }
            if (tables[2] != null)
            {
                Cards.Rows.Clear();
                Cards.Merge(tables[2]);
            }

            uw.Close();
            uw.Dispose();

            if (_fl_artUpdated)
                MMessageBox.Show(this, "Були внесені зміни в базу товарів", Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

            _fl_onlyUpdate = true;
            _fl_SubUnitChanged = false;

            timer1.Start();
            SrchTbox.Select();
            GC.Collect();
        }

        /// <summary>
        /// Обробляє чек після видалення одного або всіх товару(ів).
        /// (обраховує суму, відновлує фільтрацію таблиці товарів, оновлює повідомлення на панелях)
        /// </summary>
        /// <param name="updateCustomer">Якщо true то результати обчислення будуть ще виведені на дисплей ФП</param>
        private void RowsRemoved_MyEvent(bool updateCustomer)
        {
            //winapi.Funcs.OutputDebugString("t");
            if (Cheque.Rows.Count == 0)
            {
                RefershMenus();
                if (retriveChq)
                    чекПоверненняToolStripMenuItem.PerformClick();
                //winapi.Funcs.OutputDebugString("3");
                SearchFilter(false, AppConfig.APP_SearchType, true);
                //winapi.Funcs.OutputDebugString("4");
                clientID = string.Empty;
                chqSUMA = 0.0;
                realSUMA = 0.0;
                ResetDiscount();
            }
            //winapi.Funcs.OutputDebugString("l");
            UpdateSumInfo(updateCustomer);
            RefreshChequeInformer(false);
            //winapi.Funcs.OutputDebugString("e");
            //winapi.Funcs.OutputDebugString("r");

            this.Update();
            API.PostMessage(this.Handle, (uint)AppFunc.MyMsgs.WM_UPDATE, IntPtr.Zero, IntPtr.Zero);             
        }
        /// <summary>
        /// Виконує обрахунок суми товарів, які знаходяться в таблиці чеку
        /// а також вираховує коефіціенти знижок чи надбавок
        /// </summary>
        /// <param name="updateCustomer">Якщо true то результати обчислення будуть ще виведені на дисплей ФП</param>
        private void UpdateSumInfo(bool updateCustomer)
        {
            //OnDeactivate(EventArgs.Empty);
            //winapi.Funcs.OutputDebugString("X");
            if (inventChq)
                return;

            bool useConstDisc = discArrPercent[0] == 0.0 && discArrPercent[1] == 0.0 &&
                discArrCash[0] == 0.0 && discArrCash[1] == 0.0;

            // Get discount value
            if (useConstDisc)
            {
                discConstPercent = 0.0;
                //form "sum" by static discount
                if (AppConfig.APP_UseStaticDiscount)
                    discConstPercent = AppConfig.APP_StaticDiscountValue;
                //form "sum" by dynamic discount
                if (AppConfig.APP_UseStaticRules)
                    discConstPercent = AppFunc.UpdateSumbyRules(Cheque);
            }
            else
            {
                discOnlyPercent = discArrPercent[0] + discArrPercent[1];
                discOnlyCash = discArrCash[0] + discArrCash[1];
                discOnlyPercent = AppFunc.GetRoundedMoney(discOnlyPercent);
                discOnlyCash = AppFunc.GetRoundedMoney(discOnlyCash);
            }

            if (Cheque.Rows.Count == 0)
            {
                realSUMA = chqSUMA = taxSUMA = 0.0;
                UpdateSumDisplay(false, updateCustomer);
                return;
            }

            int i = 0;
            double dValue = 0.0;
            double taxValue = 0.0;
            double artSum = 0.0;
            int index = 0;
            DataRow[] dRows = null;
            double discSUMA = 0.0;

            // restore native cheque sum
            // and set price acording to client's discount card
            //DataRow[] artRecord = null;
            double newPrice = 0.0;
            double newTmpPrice = 0.0; //bool isSet = false;
            for (i = 0; i < Cheque.Rows.Count; i++)
            {
                newPrice = AppFunc.GetDouble(Cheque.Rows[i]["ORIGPRICE"]);

                //isSet = false;
                if (this.clientPriceNo != 0)
                {
                    newTmpPrice = AppFunc.GetDouble(Cheque.Rows[i]["PR" + this.clientPriceNo].ToString());
                    if (newTmpPrice != 0.0) newPrice = newTmpPrice;
                }
                else if (UserStruct.Properties[8])
                {
                    //DataRow dRow = Cheque.Rows.Find(chequeDGV.CurrentRow.Cells["C"].Value);
                    //price = AppFunc.AutomaticPrice(thisTot, dRow);
                    newPrice = AppFunc.AutomaticPrice(AppFunc.GetDouble(Cheque.Rows[i]["TOT"].ToString()), Cheque.Rows[i]);
                }
                Cheque.Rows[i]["PRICE"] = newPrice;
                Cheque.Rows[i]["ASUM"] = Cheque.Rows[i]["SUM"] = AppFunc.GetRoundedMoney(AppFunc.GetDouble(Cheque.Rows[i]["TOT"].ToString()) * newPrice);
                Cheque.Rows[i]["DISC"] = 0.0;
            }
            chqSUMA = (double)Cheque.Compute("sum(SUM)", "");
            chqSUMA = AppFunc.GetRoundedMoney(chqSUMA);
            realSUMA = chqSUMA;

            //select rows with discount mode
            try
            {
                dRows = Cheque.Select("USEDDISC = " + Boolean.TrueString);
                _fl_useTotDisc = (dRows.Length == Cheque.Rows.Count);
                discSUMA = (double)Cheque.Compute("Sum(SUM)", "USEDDISC = " + Boolean.TrueString);
            }
            catch { };

            //procentnuj zagalnuj koeficient
            if (useConstDisc)
                discCommonPercent = discConstPercent;
            else
                discCommonPercent = discOnlyPercent;
            if (discSUMA != 0.0)
                discCommonPercent += (discOnlyCash * 100) / discSUMA;
            discCommonPercent = AppFunc.GetRoundedMoney(discCommonPercent);


            DataRow[] prRows = null;
            if (this.clientPriceNo != 0)
                prRows = Cheque.Select("PR" + this.clientPriceNo + " <> 0");

            if (_fl_useTotDisc && prRows != null && prRows.Length == 0)
            {
                //obrahunok realnoi sumu cheku zi znugkojy
                if (useConstDisc)
                {
                    dValue = (discConstPercent * discSUMA) / 100;
                    dValue = AppFunc.GetRoundedMoney(dValue);
                    realSUMA -= dValue;
                }
                else
                {
                    dValue = (discOnlyPercent * discSUMA) / 100;
                    dValue = AppFunc.GetRoundedMoney(dValue);
                    realSUMA -= dValue;
                    realSUMA -= discOnlyCash;
                }
            }
            else
            {
                _fl_useTotDisc = false;
                for (i = 0; i < dRows.Length; i++)
                {
                    // don't use discount when client want to has another price for current article
                    if (this.clientPriceNo != 0 && AppFunc.GetDouble(dRows[i]["PR" + this.clientPriceNo].ToString()) > 0.0)
                    {
                        dRows[i]["DISC"] = 0.0;
                        continue;
                    }
                    dRows[i]["DISC"] = discCommonPercent;
                    dValue = (discCommonPercent * (double)dRows[i]["SUM"]) / 100;
                    //discValue = AppFunc.GetRoundedMoney(discValue);
                    dValue = (double)dRows[i]["SUM"] - dValue;
                    dRows[i]["ASUM"] = AppFunc.GetRoundedMoney(dValue);
                }
                realSUMA = (double)Cheque.Compute("Sum(ASUM)", "");
            }
            realSUMA = AppFunc.GetRoundedMoney(realSUMA);

            //vuvedennja zagalnogo koeficientu znugku v 2oh tupah
            //groshovuj koeficient
            discCommonCash = chqSUMA - realSUMA;
            discCommonCash = AppFunc.GetRoundedMoney(discCommonCash);

            // calculating tax sum
            taxSUMA = 0.0;
            for (i = 0; i < Cheque.Rows.Count; i++)
            {
                index = Array.IndexOf<char>(AppConfig.TAX_AppTaxChar, Cheque.Rows[i]["VG"].ToString()[0]);
                if (index >= 0)
                {
                    taxValue = AppConfig.TAX_AppTaxRates[index];

                    if (AppConfig.TAX_AppTaxDisc[index])
                    {
                        artSum = (discCommonPercent * (double)Cheque.Rows[i]["SUM"]) / 100;
                        artSum = (double)Cheque.Rows[i]["SUM"] - artSum;
                        artSum = AppFunc.GetRoundedMoney(artSum);
                        taxValue = (artSum * taxValue) / (taxValue + 100);
                    }
                    else
                        taxValue = (((double)Cheque.Rows[i]["ASUM"]) * taxValue) / (taxValue + 100);
                }
                else
                    taxValue = 0;

                //if (!_fl_useTotDisc)
                //else
                //taxValue = AppFunc.GetRoundedMoney(taxValue);
                Cheque.Rows[i]["TAX_MONEY"] = taxValue;
                taxSUMA += taxValue;
            }
            //taxSUMA = (double)Cheque.Compute("sum(TAX_MONEY)", "");
            taxSUMA = AppFunc.GetRoundedMoney(taxSUMA);

            UpdateSumDisplay(true, updateCustomer);

            //winapi.Funcs.OutputDebugString("Z");
        }//ok
        private void UpdateSumDisplay(bool updateAddChequeInfo, bool updateCustomer)
        {
            addChequeInfo.Text = string.Empty;
            // if (updateAddChequeInfo && discCommonPercent != 0.0)

            if (discConstPercent != 0.0 || discArrPercent[0] != 0.0 || discArrPercent[1] != 0.0 ||
                discArrCash[0] != 0.0 || discArrCash[1] != 0.0)
            {
                object[] discInfo = new object[6];
                string valueMask = "{0:F" + AppConfig.APP_MoneyDecimals + "}{1}";
                bool useConstDisc = discArrPercent[0] == 0.0 && discArrPercent[1] == 0.0 &&
                    discArrCash[0] == 0.0 && discArrCash[1] == 0.0;

                discInfo[0] = "";
                if (Cheque.Rows.Count != 0)
                    if (_fl_useTotDisc)
                        discInfo[0] = " загальна";
                    else 
                        discInfo[0] = " позиційна";

                if (useConstDisc)
                {
                    discInfo[0] = "постійна" + discInfo[0].ToString();
                    discInfo[1] = discConstPercent > 0 ? "знижка" : "націнка";
                    discInfo[2] = string.Format(valueMask, Math.Abs(discConstPercent), "%");
                }
                else
                    if (AppConfig.APP_OnlyDiscount)
                    {
                        if (discArrPercent[0] != 0.0 || discArrCash[0] != 0.0)
                        {
                            discInfo[1] = "знижка";
                            if (AppConfig.APP_DefaultTypeDisc == 0)
                                if (discArrCash[0] == 0.0)
                                    discInfo[2] = string.Format(valueMask, discArrPercent[0], "%");
                                else
                                    discInfo[2] = string.Format(valueMask, discArrCash[0], "грн.");
                            else
                                if (discArrPercent[0] == 0.0)
                                    discInfo[2] = string.Format(valueMask, discArrCash[0], "грн.");
                                else
                                    discInfo[2] = string.Format(valueMask, discArrPercent[0], "%");
                        }
                        if (discArrPercent[1] != 0.0 || discArrCash[1] != 0.0)
                        {
                            discInfo[1] = "націнка";
                            if (AppConfig.APP_DefaultTypeDisc == 0)
                                if (discArrCash[1] == 0.0)
                                    discInfo[2] = string.Format(valueMask, Math.Abs(discArrPercent[1]), "%");
                                else
                                    discInfo[2] = string.Format(valueMask, Math.Abs(discArrCash[1]), "грн.");
                            else
                                if (discArrPercent[1] == 0.0)
                                    discInfo[2] = string.Format(valueMask, Math.Abs(discArrCash[1]), "грн.");
                                else
                                    discInfo[2] = string.Format(valueMask, Math.Abs(discArrPercent[1]), "%");
                        }
                    }
                    else
                    {
                        discInfo[1] = "знижка";

                        if (AppConfig.APP_DefaultTypeDisc == 0)
                            if (discArrCash[0] == 0.0)
                                discInfo[2] = string.Format(valueMask, discArrPercent[0], "%");
                            else
                                discInfo[2] = string.Format(valueMask, discArrCash[0], "грн.");
                        else
                            if (discArrPercent[0] == 0.0)
                                discInfo[2] = string.Format(valueMask, discArrCash[0], "грн.");
                            else
                                discInfo[2] = string.Format(valueMask, discArrPercent[0], "%");

                        discInfo[3] = "i";
                        discInfo[4] = "націнка";

                        if (AppConfig.APP_DefaultTypeDisc == 0)
                            if (discArrCash[1] == 0.0)
                                discInfo[5] = string.Format(valueMask, Math.Abs(discArrPercent[1]), "%");
                            else
                                discInfo[5] = string.Format(valueMask, Math.Abs(discArrCash[1]), "грн.");
                        else
                            if (discArrPercent[1] == 0.0)
                                discInfo[5] = string.Format(valueMask, Math.Abs(discArrCash[1]), "грн.");
                            else
                                discInfo[5] = string.Format(valueMask, Math.Abs(discArrPercent[1]), "%");
                    }

                addChequeInfo.Text = valueMask = string.Empty;
                for (byte i = 0; i < discInfo.Length && discInfo[i] != null; i++)
                    valueMask += (discInfo[i] + " ");
                addChequeInfo.Text = valueMask.Remove(valueMask.Length - 1, 1);
            }

            //Show cheque Suma on display
            CashLbl.Text = string.Format("{0:F" + AppConfig.APP_MoneyDecimals + "}", realSUMA);

            if (AppConfig.APP_ShowInfoOnIndicator && Program.Service.UseEKKR && updateCustomer)
                try
                {
                    string[] lines = new string[] { "СУМА" + " : " + CashLbl.Text, (discCommonPercent > 0 ? "Знижка" : "Надбавка") + " " + Math.Abs(discCommonPercent) + " %" };
                    bool[] show = new bool[] { true, true };
                    Program.Service.CallFunction("SendCustomer", new object[] { lines, show });
                }
                catch { }
        }
        /// <summary>
        /// Закриття чеку
        /// </summary>
        /// <param name="fix">Якщо true то чек є фіскальний</param>
        private void CloseCheque(bool fix)//1_msg//lbl7
        {
            Payment pMethod = new Payment(realSUMA);
            pMethod.ShowDialog();
            pMethod.Dispose();
            //winapi.Funcs.OutputDebugString("A");

            if (pMethod.DialogResult != DialogResult.OK)
                return;

            object[] localData = new object[8];
            object[] printerData = new object[21];
            string chqNom = string.Empty;

            switch (pMethod.Type[0])
            {
                case 0: { digitalPanel.BackgroundImage = Properties.Resources.payment_card; break; }
                case 1: { digitalPanel.BackgroundImage = Properties.Resources.payment_credit; break; }
                case 2: { digitalPanel.BackgroundImage = Properties.Resources.payment_cheque; break; }
                case 3: { digitalPanel.BackgroundImage = Properties.Resources.payment_cash; break; }
            }

            if (UserStruct.Properties[4] &&
                DialogResult.Yes == MMessageBox.Show(this, "Видати накладну згідно цього чеку ?", Application.ProductName,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
            {
                nakladna = true;
                CashLbl.Image = Properties.Resources.naklad;
            }

            localData[0] = clientID == string.Empty ? AppConfig.APP_ClientID : clientID;
            localData[1] = discCommonPercent;
            localData[2] = realSUMA;
            localData[3] = taxSUMA;
            localData[4] = nakladna;
            localData[5] = retriveChq;
            localData[6] = _fl_useTotDisc;

            //Копія таблиці типу DataTable з записами товарів
            printerData[0] = Cheque.Copy();
            //Якщо true то цей чек є чеком поверення
            printerData[2] = retriveChq;
            //Якщо true то чек є фіскальний
            printerData[3] = fix;
            //Сума товарів без знижки чи надбавки
            printerData[4] = chqSUMA;
            //Сума товарів з зщнижкою або надбавкою
            printerData[5] = realSUMA;
            //Тип закриття чеку (Готівка, чек, кредит, картка)
            printerData[6] = pMethod.Type;
            //Загальна сума грошей покупця
            printerData[7] = pMethod.CashSum;
            //Сума грошей з кожного типу оплати
            printerData[8] = pMethod.ItemsCash;
            //Здача
            printerData[9] = pMethod.Rest;
            //Якщо true то знижка чи надбавка діє на всі позиції(товари) чеку
            printerData[10] = _fl_useTotDisc;
            //Масив з значеннями знижки та надбавки в процентних значеннях
            printerData[11] = new double[2] { discArrPercent[0], discArrPercent[1] };
            //Масив з значеннями знижки та надбавки в грошових значеннях
            printerData[12] = new double[2] { discArrCash[0], discArrCash[1] };
            //Значення постійної знижки в процентному значенні
            printerData[13] = discConstPercent;
            //
            printerData[14] = null;
            //Сума знижки і надбавки з процентними значеннями
            printerData[15] = discOnlyPercent;
            //Сума знижки і надбавки з грошовими значеннями
            printerData[16] = discOnlyCash;
            //Загальний коефіціент знижки в процентному значенні
            printerData[17] = discCommonPercent;
            //Загальний коефіціент знижки в грошовому значенні
            printerData[18] = discCommonCash;
            //Якщо чек є закритий з рахунку
            if (Cheque.ExtendedProperties.Contains("BILL"))
            {
                //Номер рахунку
                printerData[19] = Cheque.ExtendedProperties["NOM"];
                //Коментр рахунку
                printerData[20] = Cheque.ExtendedProperties["CMT"];
            }
            else
            {
                //Номер рахунку
                printerData[19] = string.Empty;
                //Коментр рахунку
                printerData[20] = string.Empty;
            }

            //winapi.Funcs.OutputDebugString("B");
            if (fix)
            {
                try
                {
                    if (retriveChq)
                        Program.Service.CallFunction("PayMoney", new object[] { Cheque, AppConfig.APP_DoseDecimals, _fl_useTotDisc, AppConfig.APP_MoneyDecimals });
                    else
                        Program.Service.CallFunction("Sale", new object[] { Cheque, AppConfig.APP_DoseDecimals, _fl_useTotDisc, AppConfig.APP_MoneyDecimals });

                    if (_fl_useTotDisc && discCommonPercent != 0.0)
                    {
                        //if ((discount[0] + discount[1]) != 0 || constDiscount != 0)
                        //OnDeactivate(EventArgs.Empty);
                        //double[] valueDISC = new double[] {
                        //    discount[0],
                        //    cash_discount[0],
                        //    discount[1],
                        //    cash_discount[1],
                        //    constDiscount};
                        //byte[] types = new byte[] { 2, 3, 2, 3, 2 };

                        //for (int i = 0; i < valueDISC.Length; i++)
                        //    if (valueDISC[i] != 0.0)
                        Program.Service.CallFunction(
                            "Discount",
                            new object[] { 
                                    (byte)2/*types[i]*/, 
                                    /*valueDISC[i]*/discCommonPercent/*(discount[0] + discount[1]) == 0 ? constDiscount : (discount[0] + discount[1])*/,
                                    AppConfig.APP_MoneyDecimals, "" });
                    }

                    for (int i = 0; i < pMethod.Type.Count; i++)
                        Program.Service.CallFunction("Payment", new object[] { pMethod.Type[i], false, pMethod.ItemsCash[i], pMethod.Autoclose, "" });

                    object[] memory = null;
                    if (retriveChq)
                        memory = Program.Service.CallFunction("GetMemory", new object[] { "30AB", (byte)16, (byte)2 });
                    else
                        memory = Program.Service.CallFunction("GetMemory", new object[] { "301B", (byte)16, (byte)2 });
                    chqNom = memory[0].ToString();
                    //memory = Program.Service.CallFunction("GetMemory", new object[] { "0037", (byte)16, (byte)2 });
                    localData[7] = Program.Service.CallFunction("FP_GetLastZNo", null)[0].ToString();
                }
                catch (Exception ex)
                {
                    MMessageBox.Show(this, "Помилка під час закриття чеку" + "\r\n" + ex.Message,
                        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AppFunc.WriteLog(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                    ChqNomRequest cnr = new ChqNomRequest();
                    cnr.ShowDialog();
                    cnr.Dispose();
                    if (cnr.DialogResult != DialogResult.Yes)
                        return;
                    chqNom = cnr.ChequeNumber.ToString();
                }

                try
                {
                    Program.Service.CallFunction("OpenBox", null);
                }
                catch (Exception ex)
                {
                    MMessageBox.Show(this, "Помилка відкриття грошової скриньки" + "\r\n" + ex.Message,
                        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AppFunc.WriteLog(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                }
            }
            //winapi.Funcs.OutputDebugString("C");
            chqNom = AppFunc.SaveCheque(Cheque, localData, pMethod.Type[0], chqNom);

            printerData[1] = chqNom;

            //Printing
            //winapi.Funcs.OutputDebugString("D");
            if (fix && UserStruct.Properties[10])
                AppFunc.Print(printerData, "fix", 0);
            if (!fix && UserStruct.Properties[11])
                AppFunc.Print(printerData, "none", 0);

            Cheque.Clear();
            if (Cheque.ExtendedProperties.Contains("BILL"))
                File.Delete(Cheque.ExtendedProperties["PATH"].ToString());
            Cheque.ExtendedProperties.Clear();
            RowsRemoved_MyEvent(false);

            CashLbl.ForeColor = AppConfig.STYLE_RestFontColor;
            CashLbl.Font = AppConfig.STYLE_RestFont;
            CashLbl.Text = string.Format("{0:F" + AppConfig.APP_MoneyDecimals + "}", pMethod.Rest);
            chequeInfoLabel.Text = string.Format("{0} {1:F" + AppConfig.APP_MoneyDecimals + "}", "залишок з суми", pMethod.CashSum);
            addChequeInfo.Text = string.Format("{0} {1} ", "Закритий чек №", chqNom);
            if (printerData[19].ToString() != string.Empty)
                addChequeInfo.Text += string.Format("{0} {1} ", "з рахунку №", printerData[19]);

            if (fix && AppConfig.APP_ShowInfoOnIndicator)
                try
                {
                    string[] lines = new string[2];
                    lines[0] = string.Format("{0} : {1:F" + AppConfig.APP_MoneyDecimals + "}", "Гроші", pMethod.CashSum);
                    lines[1] = string.Format("{0} : {1}", "Здача", CashLbl.Text);
                    bool[] show = new bool[] { true, true };
                    Program.Service.CallFunction("SendCustomer", new object[] { lines, show });
                }
                catch { }

            //winapi.Funcs.OutputDebugString("E");
        }//Make exceptions

        private void ResetDiscount()
        {
            //discConstPercent = 0.0;
            discArrPercent[0] = 0.0;
            discArrPercent[1] = 0.0;
            discArrCash[0] = 0.0;
            discArrCash[1] = 0.0;
            discOnlyPercent = 0.0;
            discOnlyCash = 0.0;
            discCommonPercent = 0.0;
            discCommonCash = 0.0;
            clientPriceNo = 0;

            відмінитиЗнижкунадбавкуToolStripMenuItem.Enabled = false;
            відмінитиЗнижкунадбавкуToolStripMenuItem.Text = "Без знижки/надбавки";
            //addChequeInfo.Text = string.Empty;
            //UpdateSumDisplay(true, true);
        }//ok
        private bool BCSearcher(string barcode, bool showMsg)//3_msg
        {
            barcode = barcode.Trim();
            //winapi.WinAPI.OutputDebugString("BC=" + barcode + "____Ln=" + barcode.Length.ToString());
            bool allowToShow = false;//returned value

            DataTable sTable = Articles.Clone();
            DataRow[] dr = new DataRow[1];
            double weightOfArticle = AppConfig.APP_StartTotal;

            //search by weight-barcodes
            if (AppConfig.APP_WeightType == 1)
            {
                if (barcode.Length >= 12 && barcode.Substring(0, 2) == "20")
                {
                    weightOfArticle = AppFunc.GetDouble(barcode.Substring(7, 5)) / 1000;
                    barcode = barcode.Substring(2, 5);
                }
            }


            //search by cards of clients
            #region Using Discount for client 998
            
            if (UserStruct.Properties[15])
            {
                // Get custom client's card prefix
                string cliCardPrefix = AppConfig.APP_ClientCard;
                bool customClientCard = false;
                if (cliCardPrefix.Length != 0 && cliCardPrefix.Length < barcode.Length &&
                    barcode.Substring(0, cliCardPrefix.Length) == cliCardPrefix)
                    customClientCard = true;

                if (barcode.Length > 5 && (barcode.Substring(0, 3) == "998" || customClientCard))
                {
                    dr = Cards.Select("CBC =\'" + barcode + "\'");

                    if (dr.Length != 0 && dr[0] != null)
                    {
                        if (discArrPercent[0] < (double)dr[0]["CDISC"] && discConstPercent < (double)dr[0]["CDISC"])
                        {
                            if (AppConfig.APP_OnlyDiscount)
                                discArrPercent[0] = discArrPercent[1] = 0.0;
                            discArrPercent[0] = (double)dr[0]["CDISC"];
                        }
                        this.clientPriceNo = (int)dr[0]["CPRICENO"];
                        UpdateSumInfo(true);
                        clientID = (string)dr[0]["CID"];
                        SearchFilter(false, AppConfig.APP_SearchType, false);
                    }

                    else
                        MMessageBox.Show("Немає клієнта з кодом" + " " + barcode, "Результат пошуку",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return allowToShow;
                }
            }
            #endregion
            //search by cards of buyers - not finish
            #region Using Discount for buyer 999
            if (UserStruct.Properties[13] && barcode.Length > 5 && barcode.Substring(0, 3) == "999")
            {
                double dec = (double)Convert.ToInt32(barcode.Substring(3, 5), 8);
                dec /= 100;
                dec = Math.Round(dec, AppConfig.APP_MoneyDecimals, MidpointRounding.AwayFromZero);

                if (dec <= 100)
                    if (discArrPercent[0] < dec && discConstPercent < dec)
                    {
                        if (AppConfig.APP_OnlyDiscount)
                            discArrPercent[0] = discArrPercent[1] = 0.0;
                        discArrPercent[0] = dec;
                    }

                if (dec > 100 && (discArrPercent[0] == 0 || discConstPercent >= 0))
                {
                    dec -= 100;
                    dec = -Math.Round(dec, AppConfig.APP_MoneyDecimals, MidpointRounding.AwayFromZero);
                    if (discArrPercent[1] < dec || discConstPercent < dec)
                    {
                        if (AppConfig.APP_OnlyDiscount)
                            discArrPercent[0] = discArrPercent[1] = 0.0;
                        discArrPercent[1] = dec;
                    }
                }

                UpdateSumInfo(true);
                SearchFilter(false, AppConfig.APP_SearchType, false);

                return allowToShow;
            }
            #endregion
            //search by barcodes of articles

            dr = Articles.Select("BC = \'" + barcode.Trim() + "\'");
            if (dr.Length == 0 && UserStruct.Properties[16])
            {
                dr = AltBC.Select("ABC = \'" + barcode + "\'");
                if (dr.Length != 0)
                {
                    string cmd = string.Empty;

                    for (int i = 0; i < dr.Length; i++)
                    {
                        cmd += "ID='" + dr[i]["AID"] + "'";
                        if (i + 1 < dr.Length)
                            cmd += " OR ";
                    }

                    DataRow[] rows = new DataRow[dr.Length];
                    try
                    {
                        rows = Articles.Select(cmd);
                    }
                    catch { }

                    dr = (DataRow[])rows.Clone();
                }
            }

            if (dr.Length == 0)
            {
                if (showMsg)
                    MMessageBox.Show(this, "Немає товару з таким штрих-кодом", Application.ProductName, 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (currSrchType != AppConfig.APP_SearchType)
                    SearchFilter(false, AppConfig.APP_SearchType, true);
                else
                    SearchFilter(true, currSrchType, false);
                return allowToShow;
            }

            if (dr.Length == 1)
            {
                AppFunc.AddArticleToCheque(chequeDGV, articleDGV, dr[0], weightOfArticle, Articles);
                if (!UserStruct.Properties[22])
                    SearchFilter(true, currSrchType, true);
                allowToShow = false;
            }
            else
            {
                sTable.Clear();
                sTable.BeginLoadData();
                
                for (int i = 0; i < dr.Length; i++)
                    sTable.Rows.Add(dr[i].ItemArray);
                sTable.EndLoadData();

                articleDGV.DataSource = sTable;
                articleDGV.Select();
                allowToShow = true;
            }
            
            return allowToShow;
        }
        private void SearchFilter(bool saveSearchText, int SrchType, bool close)
        {
            if (close)
                articleDGV.DataSource = Articles;

            if (!saveSearchText)
                SrchTbox.Text = "";

            if (splitContainer1.Panel2.Tag != null)
            {
                вікноТоварівToolStripMenuItem.PerformClick();
                splitContainer1.Panel2.Tag = null;
            }

            switch (SrchType)
            {
                case 0:
                    {
                        SrchTbox.BackColor = Color.FromArgb(255, 255, 192);
                        searchImage.BackColor = Color.FromArgb(255, 255, 192);
                        searchImage.BackgroundImage = Properties.Resources.by_name;
                        break;
                    }
                case 1:
                    {
                        SrchTbox.BackColor = Color.FromArgb(192, 255, 192);
                        searchImage.BackColor = Color.FromArgb(192, 255, 192);
                        searchImage.BackgroundImage = Properties.Resources.by_c;
                        break;
                    }
                case 2:
                    {
                        SrchTbox.BackColor = Color.FromArgb(255, 192, 192);
                        searchImage.BackColor = Color.FromArgb(255, 192, 192);
                        searchImage.BackgroundImage = Properties.Resources.by_bc;
                        break;
                    }
            }

            SrchTbox.Focus();
            SrchTbox.Select();
            SrchTbox.SelectAll();

            currSrchType = SrchType;
        }
    }
}