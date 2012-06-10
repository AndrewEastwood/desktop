using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.FileIO;
using System.Runtime.Serialization.Formatters.Binary;
using driver.Config;
using driver.Components;
using driver.Components.UI;
using System.Collections;
using components.Components.WinApi;
using components.Components.MMessageBox;

namespace driver.Lib
{
    /// <summary>
    /// Main methods class.
    /// Contained based methods.
    /// </summary>
    public class CoreLib 
    {
        //Rules
        //-----------------------------
        /// <summary>
        /// Calculate new price for article by definded rules for that article
        /// </summary>
        /// <param name="tot">Total of article</param>
        /// <param name="dRow">Article's data row</param>
        /// <returns>Return new price</returns>
        public static double AutomaticPrice(double tot, DataRow dRow)
        {
            double newPrice = (double)dRow["ORIGPRICE"];

            if (tot > 1 && (double)dRow["PR1"] != 0.0)
                newPrice = (double)dRow["PR1"];

            if (tot > (double)dRow["Q2"] && (double)dRow["Q2"] != 0.0 && (double)dRow["PR2"] != 0.0)
                newPrice = (double)dRow["PR2"];

            if (tot > (double)dRow["Q3"] && (double)dRow["Q3"] != 0.0 && (double)dRow["PR3"] != 0.0)
                newPrice = (double)dRow["PR3"];

            return newPrice;
        }//ok
        /// <summary>
        /// Перевірка вмісту чеу за правилами формування ціни
        /// </summary>
        /// <param name="dTable">Таблиця чеку</param>
        /// <returns>Значення знижки або надбавки</returns>
        public static double UpdateSumbyRules(DataTable dTable)
        {
            double Result = 0.0;
            byte i = 0;
            string[] doubleRules = new string[0];
            string[] singleRules = new string[0];
            double suma = 0.0;
            
            if (dTable.Rows.Count > 0)
                suma = (double)dTable.Compute("Sum(SUM)", "");

            if (driver.Config.ConfigManager.Instance.CommonConfiguration.APP_DiscountRules == null)
                return 0.0;

            for (; i < driver.Config.ConfigManager.Instance.CommonConfiguration.APP_DiscountRules.Length; i++)
                if (driver.Config.ConfigManager.Instance.CommonConfiguration.APP_DiscountRules[i].Contains("?"))
                {
                    Array.Resize<string>(ref singleRules, singleRules.Length + 1);
                    singleRules[singleRules.Length - 1] = driver.Config.ConfigManager.Instance.CommonConfiguration.APP_DiscountRules[i];
                }
                else
                {
                    Array.Resize<string>(ref doubleRules, doubleRules.Length + 1);
                    doubleRules[doubleRules.Length - 1] = driver.Config.ConfigManager.Instance.CommonConfiguration.APP_DiscountRules[i];
                }


            double tot = 0.0;
            for (int j = 0; j < dTable.Rows.Count; j++)
                tot += double.Parse(dTable.Rows[j]["TOT"].ToString());
            int totEntry = (int)tot;
            double rPrice = 0.0;
            int rTot = 0;
            double rDisk = 0.0;
            string[] _rITEMS = new string[0];

            #region Set new SUM by double rules;
            for (i = 0; i < doubleRules.Length; i++)
            {
                _rITEMS = doubleRules[i].Split(';');
                rPrice = double.Parse(_rITEMS[1].Replace('.', ','));
                rTot = int.Parse(_rITEMS[4]);
                rDisk = int.Parse(_rITEMS[5].Replace('.', ','));

                switch (_rITEMS[2])
                {
                    case "|":
                        {
                            if (CompareSumEntry(suma, _rITEMS[0], rPrice) && CompareTotEntry(totEntry, _rITEMS[3], rTot))
                                if (Result < rDisk)
                                    Result = rDisk;

                            break;
                        }
                    case "&":
                        {
                            if (CompareSumEntry(suma, _rITEMS[0], rPrice) || CompareTotEntry(totEntry, _rITEMS[3], rTot))
                                if (Result < rDisk)
                                    Result = rDisk;

                            break;
                        }
                }
            }
            #endregion

            #region Set new SUM by single rules;
            for (i = 0; i < singleRules.Length; i++)
            {
                _rITEMS = singleRules[i].Split(';');
                rPrice = double.Parse(_rITEMS[1].Replace('.', ','));
                rDisk = int.Parse(_rITEMS[5].Replace('.', ','));

                if (CompareSumEntry(suma, _rITEMS[0], rPrice))
                    if (Result < rDisk)
                        Result = rDisk;
            }
            #endregion

            return Result;
        }//ok
        #region PrivateFunctions
        private static bool CompareTotEntry(int thisTot, string bySymbol, int thatTot)
        {
            bool retValue = false;

            switch (bySymbol)
            {
                case "0":
                    {
                        if (thisTot > thatTot)
                            retValue = true;
                        break;
                    }
                case "1":
                    {
                        if (thisTot < thatTot)
                            retValue = true;
                        break;
                    }
                case "2":
                    {
                        if (thisTot != thatTot)
                            retValue = true;
                        break;
                    }
                case "3":
                    {
                        if (thisTot >= thatTot)
                            retValue = true;
                        break;
                    }
                case "4":
                    {
                        if (thisTot <= thatTot)
                            retValue = true;
                        break;
                    }
            }
            return retValue;
        }//ok
        private static bool CompareSumEntry(double thisSum, string bySymbol, double thatSum)
        {
            bool retValue = false;

            switch (bySymbol)
            {
                case "0":
                    {
                        if (thisSum > thatSum)
                            retValue = true;
                        break;
                    }
                case "1":
                    {
                        if (thisSum < thatSum)
                            retValue = true;
                        break;
                    }
                case "2":
                    {
                        if (thisSum >= thatSum)
                            retValue = true;
                        break;
                    }
                case "3":
                    {
                        if (thisSum <= thatSum)
                            retValue = true;
                        break;
                    }
            }
            return retValue;
        }//ok
        #endregion

        //Record Manager
        //-----------------------------
        /// <summary>
        /// Додавання товарк в чек або зміна кількості товару в чеку
        /// </summary>
        /// <param name="chqDGW">Таблиця чеку</param>
        /// <param name="artDGW">Таблиця товарів (можливе застосування фільтру до записів)</param>
        /// <param name="article">Запис з товаром</param>
        /// <param name="startTotal">Стартова кількість</param>
        /// <param name="artsTable">Оригінальна таблиця товарів (без затстосування фільтру до записів)</param>
        public static void AddArticleToCheque(DataGridView chqDGW, DataGridView artDGW, DataRow article, double startTotal, DataTable artsTable)
        {
            //winapi.Funcs.OutputDebugString("A");
            Hashtable profileDefinedTaxGrid = new Hashtable();
            Hashtable profileCompatibleTaxGrid = new Hashtable();
            bool taxGridError = false;
            try
            {
                profileDefinedTaxGrid = (Hashtable)driver.Config.ConfigManager.Instance.CommonConfiguration.TAX_DefinedRates[article["F"]];
                profileCompatibleTaxGrid = (Hashtable)driver.Config.ConfigManager.Instance.CommonConfiguration.TAX_Compatibility[article["F"]];
            }
            catch { taxGridError = false; }

            if (taxGridError || profileDefinedTaxGrid == null || profileDefinedTaxGrid.Count == 0)
            {
                MMessageBoxEx.Show(chqDGW, "Немає податкових ставок", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // old tax validator
            /*if (driver.Config.ConfigManager.Instance.CommonConfiguration.TAX_AppTaxChar == null || driver.Config.ConfigManager.Instance.CommonConfiguration.TAX_AppTaxChar.Length == 0)
            {
                MMessageBoxEx.Show(chqDGW, "Немає податкових ставок", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }*/

            if ((double)article["PRICE"] == 0)
            {
                MMessageBox.Show(chqDGW, "Нульова ціна товару", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            /*
             * 1) If article exist
             *  a) CTOT=TOT
             *  b) TOT=startValue
            */
            //winapi.Funcs.OutputDebugString("G");
            int index = 0;
            bool rowIsUpdated = false;
            DataRow dRow = null;
            bool funcRezult = false;
            DataTable cheque = chqDGW.DataSource as DataTable;

            if (UserConfig.Properties[9] && startTotal == driver.Config.ConfigManager.Instance.CommonConfiguration.APP_StartTotal)
                startTotal = CheckByMask(article["UNIT"], startTotal);

            //Update existed rows
            //winapi.Funcs.OutputDebugString("H");
            if (UserConfig.Properties[7] && cheque.Rows.Count != 0)
            {
                DataRow[] dRows = cheque.Select("ID='" + article["ID"] + "'");
                if (dRows.Length != 0 && dRows[0] != null)
                    try
                    {
                        dRow = dRows[0];
                        dRow["TMPTOT"] = dRow["TOT"];

                        if (UserConfig.Properties[17] || startTotal == 0.0)
                        {
                            Request req = new Request(dRow, startTotal);
                            funcRezult = req.UpdateRowSource(chqDGW);
                            req.Dispose();
                            //winapi.Funcs.OutputDebugString("U");
                            if (!funcRezult)
                                return;
                        }
                        else
                            dRow["TOT"] = MathLib.GetRoundedDose(startTotal);

                        index = cheque.Rows.IndexOf(dRow);
                        rowIsUpdated = true;
                    }
                    catch (Exception ex)
                    {
                        WriteLog(ex, MethodInfo.GetCurrentMethod().Name);
                    }
            }
            
            //Add new row
            if (!rowIsUpdated)
            {
                //winapi.Funcs.OutputDebugString("J");
                dRow = cheque.NewRow();
                dRow["ORIGPRICE"] = article["PRICE"];

                //C
                string c = dRow["C"].ToString();
                dRow.ItemArray = article.ItemArray;
                dRow["C"] = long.Parse(c);

                //TAX

                // new tax mode
                try
                {

                    // get application tax char with compatible tax grid
                    string definedTaxChar = profileCompatibleTaxGrid[dRow["VG"].ToString()[0]].ToString();
                    string[] definedTaxData = profileDefinedTaxGrid[definedTaxChar].ToString().Split(';');

                    dRow["VG"] = definedTaxChar;
                    dRow["TAX_VAL"] = MathLib.GetDouble(definedTaxData[0]);
                    dRow["USEDDISC"] = Boolean.Parse(definedTaxData[1]);
                }
                catch
                {
                    dRow["TAX_VAL"] = 0.0;
                    dRow["USEDDISC"] = true;
                }
                /*
                try
                {
                    index = Array.IndexOf<char>(driver.Config.ConfigManager.Instance.CommonConfiguration.TAX_MarketColumn, dRow["VG"].ToString()[0]);
                }
                catch { index = 0; }
                if (index < 0)
                    index = 0;
                char pch = driver.Config.ConfigManager.Instance.CommonConfiguration.TAX_AppColumn[index];
                index = Array.IndexOf<char>(driver.Config.ConfigManager.Instance.CommonConfiguration.TAX_AppTaxChar, pch);
                if (index >= 0)
                {
                    dRow["VG"] = pch;
                    dRow["TAX_VAL"] = driver.Config.ConfigManager.Instance.CommonConfiguration.TAX_AppTaxRates[index];
                    dRow["USEDDISC"] = driver.Config.ConfigManager.Instance.CommonConfiguration.TAX_AppTaxDisc[index];
                }
                */
                if (UserConfig.Properties[17] || startTotal == 0.0)
                {
                    Request req = new Request(dRow, startTotal);
                    funcRezult = req.UpdateRowSource(chqDGW);
                    req.Dispose();
                    if (!funcRezult) return;
                }
                else
                    dRow["TOT"] = startTotal;

                #region Sorting article by ID and adding
                if (UserConfig.Properties[14] && cheque.Rows.Count != 0)
                {
                    index = 0;
                    do
                    {
                        if (GetIDCode(cheque.Rows[index]["ID"]) < GetIDCode(dRow["ID"]))
                            index++;
                        else
                            break;

                    } while (cheque.Rows.Count > index);
                    cheque.Rows.InsertAt(dRow, index);
                }
                else
                {
                    cheque.Rows.Add(dRow);
                    index = cheque.Rows.Count - 1;
                }
                #endregion
            }

            //winapi.Funcs.OutputDebugString("K");

            if (rowIsUpdated)
                index = dRow.Table.Rows.IndexOf(dRow);
            chqDGW.CurrentCell = chqDGW.Rows[index].Cells["TOT"];

            try
            {
                object uniqueKey = article["C"];
                article = (artDGW.DataSource as DataTable).Rows.Find(uniqueKey);
                if (article != null)
                    index = (artDGW.DataSource as DataTable).Rows.IndexOf(article);
                else
                {
                    artDGW.DataSource = artsTable;
                    article = artsTable.Rows.Find(uniqueKey);
                    index = artsTable.Rows.IndexOf(article);
                }
                artDGW.CurrentCell = artDGW.Rows[index].Cells[artDGW.Columns.GetFirstColumn(DataGridViewElementStates.Visible).Name];
            }
            catch { }

            chqDGW.BeginEdit(true);
            if (!UserConfig.Properties[22])
                chqDGW.EndEdit();
            //winapi.Funcs.OutputDebugString("E");
        }//ok

        #region PrivateFunctions
        private static uint GetIDCode(object str)
        {
            uint cod = 0;
            for (byte i = 0; i < str.ToString().Length; i++)
                cod += (uint)(i * (byte)str.ToString()[i]);
            return cod;
        }//ok
        private static double CheckByMask(object unit, double startTotal)
        {
            int idx = Array.IndexOf(((string[])driver.Config.ConfigManager.Instance.CommonConfiguration.APP_UnitFilter[0]), unit.ToString());
            if (idx != -1 && ((bool[])driver.Config.ConfigManager.Instance.CommonConfiguration.APP_UnitFilter[1])[idx])
                return 0.0;

            return startTotal;
        }//ok
        #endregion

        //DataInfo
        //-----------------------------
        /// <summary>
        /// Create short information about selected article
        /// </summary>
        /// <param name="grid1">Table of cheque</param>
        /// <param name="grid2">Table of all articles</param>
        /// <returns>Return short information about selected article. If no selected article it's return empty string.</returns>
        public static string ShowArticleInfo(DataGridView grid1, DataGridView grid2)
        {
            string id = "";
            string name = "";
            string price = "";
            string unit = "";
            string bc = "";

            if (!grid1.Focused && !grid2.Focused)
                grid2.Focus();
            
            if (grid1.Focused && grid1.Rows.Count != 0 && grid1.SelectedRows.Count != 0)
            {
                id = grid1.SelectedRows[0].Cells["ID"].Value.ToString();
                name = grid1.SelectedRows[0].Cells["DESC"].Value.ToString();
                price = grid1.SelectedRows[0].Cells["PRICE"].Value.ToString();
                unit = grid1.SelectedRows[0].Cells["UNIT"].Value.ToString();
                bc = grid1.SelectedRows[0].Cells["BC"].Value.ToString();

                return id + " " + name + " : Ціна " + price + " за 1" + unit + " Штрих-код : " + bc;
            }

            if (grid2.Focused && grid2.Rows.Count != 0 && grid2.SelectedRows.Count != 0)
            {
                id = grid2.SelectedRows[0].Cells["ID"].Value.ToString();
                name = grid2.SelectedRows[0].Cells["DESC"].Value.ToString();
                price = grid2.SelectedRows[0].Cells["PRICE"].Value.ToString();
                unit = grid2.SelectedRows[0].Cells["UNIT"].Value.ToString();
                bc = grid2.SelectedRows[0].Cells["BC"].Value.ToString();

                return id + " " + name + " : Ціна " + price + " за 1" + unit + " Штрих-код : " + bc;
            }

            return "";
        }//ok

        //Administaration
        //-----------------------------
        /// <summary>
        /// Запис виникненої внутрішньої помилки в звіт
        /// </summary>
        /// <param name="e">Опис помилки</param>
        /// <param name="methodName">Назва методу в якому відбулася помилка</param>
        public static void WriteLog(Exception e, string methodName)
        {
            try
            {
                string reportName = string.Format("{0}\\report_{1}.log", driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Reports, DateTime.Now.ToShortDateString());
                FileStream fs = new FileStream(reportName, FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                sw.WriteLine("at Method : " + methodName);
                sw.WriteLine(e.Message);
                sw.WriteLine(e.StackTrace);
                sw.WriteLine(e.InnerException);
                sw.WriteLine(e.Source);
                sw.WriteLine(e.HelpLink);
                sw.WriteLine("******************************************************************");
                sw.WriteLine();
                sw.WriteLine();

                sw.Close();
                sw.Dispose();
            }
            catch { }
        }//ok

        //Collection manager
        //-----------------------------
        public static Hashtable SortByKey(Hashtable obj)
        {
            Hashtable sortedObj = new Hashtable();
            List<string> keys = new List<string>();
            foreach (DictionaryEntry item in obj)
                keys.Add(item.Key.ToString());

            keys.Sort();

            foreach (string item in keys)
                sortedObj.Add(item, obj[item]);

            return sortedObj;
        }

        public static List<string> SortedKeys(Hashtable obj)
        {
            Hashtable sortedObj = new Hashtable();
            List<string> keys = new List<string>();
            foreach (DictionaryEntry item in obj)
                keys.Add(item.Key.ToString());

            keys.Sort();

            return keys;
        }

        public static Hashtable ArgumentParser(string[] args)
        {
            Hashtable ht = new Hashtable();

            return ht;
        }

        public static bool SetContainerValue(ref Hashtable data, string valuePath, object value)
        {
            bool r = false;

            Hashtable d = (Hashtable)data.Clone();
            string[] keys = valuePath.Split('.');

            for (int i = 0; i < keys.Length - 1; i++)
            {
                if (d.ContainsKey(keys[i]))
                {
                    d = (Hashtable)d[keys[i]];
                }
                else
                    d[keys[i]] = new Hashtable();
            }

            d[keys[keys.Length - 1]] = value;
            data = d;
            return r;
        }

        public static object GetContainerValue(Hashtable data, string valuePath, object defaultValue)
        {
            Hashtable d = (Hashtable)data.Clone();
            string[] keys = valuePath.Split('.');
            object rezult = new object();

            for (int i = 0; i < keys.Length; i++)
            {
                if (d.ContainsKey(keys[i]))
                {
                    rezult = d[keys[i]];
                }

                if (i + 1 < keys.Length && rezult.GetType() == typeof(DataTable))
                    d = (Hashtable)rezult;

            }

            return rezult;
        }

        public static T GetValue<T>(Hashtable data, string valuePath, T defaultValue)
        {
            T rez = default(T);
            try
            {
                rez = (T)GetContainerValue(data, valuePath, defaultValue);
            }
            catch { }

            return rez;
        }

        public static T GetValue<T>(Hashtable data, string valuePath)
        {
            T rez = (T)GetValue(data, valuePath, default(T));
            return rez;
        }

        /// <summary>
        /// Реєстрація гарячої клавіші
        /// </summary>
        /// <param name="f">Вікно до якого будуть підвязані клавіші</param>
        /// <param name="key">Клавіша або її комбінація</param>
        /// <param name="keyID">Код клавіші (комбінації)</param>
        public static void RegisterHotKey(Form f, Keys key, MyHotKeys keyID)
        {
            int modifiers = 0;
            int MOD_ALT = 0x1;
            int MOD_CONTROL = 0x2;
            int MOD_SHIFT = 0x4;
            int MOD_WIN = 0x8;

            if ((key & Keys.Alt) == Keys.Alt)
                modifiers = modifiers | MOD_ALT;

            if ((key & Keys.Control) == Keys.Control)
                modifiers = modifiers | MOD_CONTROL;

            if ((key & Keys.Shift) == Keys.Shift)
                modifiers = modifiers | MOD_SHIFT;

            Keys k = key & ~Keys.Control & ~Keys.Shift & ~Keys.Alt;

            Com_WinApi.RegisterHotKey((IntPtr)f.Handle, (int)keyID, modifiers, (int)k);

        }
        /// <summary>
        /// Розреєстрація гарачої клавіші
        /// </summary>
        /// <param name="f">Вікно, яке містить гарячі клавіші</param>
        /// <param name="keyID">Код клавіші</param>
        public static void UnregisterHotKey(Form f, int keyID)
        {
            try
            {
                Com_WinApi.UnregisterHotKey(f.Handle, keyID); // modify this if you want more than one hotkey
            }
            catch (Exception ex)
            {
                Com_WinApi.OutputDebugString(ex.ToString());
            }
        }
        
        /// <summary>
        /// Набір клавіш та їх комбінацій, які використовуються в програмі
        /// </summary>
        public enum MyHotKeys : int
        {
            HK_CtrlDel = 0x10,
            HK_CtrlShiftDel = 0x11,
            HK_CtrlPgDn = 0x12,
            HK_CtrlPgUp = 0x13,
            HK_ShiftDel = 0x14,
            HK_Enter = 0x15,
            HK_CtrlEnter = 0x16,
            HK_CtrlShiftEnter = 0x17,
            HK_F5 = 0x18,
            HK_F6 = 0x19,
            HK_F7 = 0x1A,
            HK_F8 = 0x1B,
            HK_F9 = 0x1C,
            HK_Esc = 0x1D,
            HK_CtrlQ = 0x1E,
            HK_Ctrl = 0x1F
        }
        /// <summary>
        /// Набір повідомлень для виконання певних операцій
        /// </summary>
        public enum MyMsgs : int
        {
            WM_HOTKEY = 0x312,
            WM_UPDATE = 0x456,
            WM_ENDUPDATE = 0x435
        }

    }
}