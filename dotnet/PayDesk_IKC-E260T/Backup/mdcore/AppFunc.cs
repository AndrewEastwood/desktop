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
using winapi;
//using System.Configuration;
//using System.Diagnostics;

namespace mdcore
{
    public class AppFunc
    {
        private static BinaryFormatter binF = new BinaryFormatter();

        /// <summary>
        /// Перевірка на оновлення бази з товарами
        /// </summary>
        /// <returns>Масив з шляхами до нових файлів</returns>
        public static string[] CheckForUpdate()
        {
            string[] exFiles = new string[3];
            exFiles[0] = AppConfig.Path_Exchnage + "\\" + string.Format("Art_{0:X2}", AppConfig.APP_SubUnit) + ".SDF";
            exFiles[1] = AppConfig.Path_Exchnage + "\\" + string.Format("Alt_{0:X2}", AppConfig.APP_SubUnit) + ".SDF";
            exFiles[2] = AppConfig.Path_Exchnage + "\\" + "Cli_BC.SDF";
            string[] localFiles = new string[3];
            localFiles[0] = AppConfig.Path_Articles + "\\" + string.Format("Art_{0:X2}", AppConfig.APP_SubUnit) + ".saf";
            localFiles[1] = AppConfig.Path_Articles + "\\" + string.Format("Alt_{0:X2}", AppConfig.APP_SubUnit) + ".saf";
            localFiles[2] = AppConfig.Path_Articles + "\\" + "DCards.saf";
            DateTime dTime = new DateTime();
            string[] load = new string[3] { "", "", "" };


            API.OutputDebugString("CheckUpdate_begin");

            //Microsoft.VisualBasic.Devices.Network nwt = new Microsoft.VisualBasic.Devices.Network();
            //try
            //{
            //    nwt.DownloadFile(AppConfig.Path_Exchnage + "\\" + "a.xxx", AppConfig.Path_Temp + "\\a.xxx", "", "", false,
            //        AppConfig.APP_RefreshTimeout, true);
            //}
            //catch (System.Net.WebException we)
            //{
            //    if (we.Message == "The operation has timed out")
            //        _access = false;
            //}

            //if (!nwt.IsAvailable || !_access)
            //{
            //    winapi.Funcs.OutputDebugString("Network is unavailable");
            //    return load;
            //}
            
            for (int i = 0; i < exFiles.Length; i++)
                try
                {
                    API.OutputDebugString("check for index " + i);
                    if (FuncT.FileExists(exFiles[i], AppConfig.APP_RefreshTimeout))
                    {
                        if (!FuncT.FileExists(localFiles[i], AppConfig.APP_RefreshTimeout))
                            load[i] = exFiles[i];
                        else
                        {
                            dTime = Microsoft.VisualBasic.FileSystem.FileDateTime(exFiles[i]);

                            //hExFile = winapi.Funcs.CreateFile(exFiles[i], winapi.Enums.dwDesiredAccess.NONE,
                            //    winapi.Enums.dwShareMode.FILE_SHARE_READ, 0, winapi.Enums.dwCreationDisposion.OPEN_EXISTING, 0, IntPtr.Zero);
                            //if (hExFile == (IntPtr)winapi.Consts.INVALID_HANDLE_VALUE)
                            //    continue;
                            //fOK = winapi.Funcs.GetFileTime(hExFile, out fTime, out fTime, out fLastWiriteTime);
                            //winapi.Funcs.CloseHandle(hExFile);

                            if (dTime > AppConfig.ADD_updateDateTime[i])
                            {
                                load[i] = exFiles[i];
                                AppConfig.ADD_updateDateTime[i] = dTime;
                            }
                        }
                    }
                    else
                        //if (i == 0)
                        //{
                            API.OutputDebugString("Art file is unvaliable _ CheckUpdate_end");
                            if (!FuncT.FileExists(AppConfig.Path_Exchnage, AppConfig.APP_RefreshTimeout))
                                load[0] = "lanError";
                                //load[0] = "lanError";
                         //   return load;
                       // }

                }
                catch { ; }

            API.OutputDebugString("CheckUpdate_end");
            return load;
        }//ok
        public static string[] LoadFilesOnLocalTempFolder(string[] exchangeFiles)
        {
            string[] tmpFiles = new string[3];
            tmpFiles[0] = AppConfig.Path_Temp + "\\" + string.Format("Art_{0:X2}", AppConfig.APP_SubUnit) + ".SDF";
            tmpFiles[1] = AppConfig.Path_Temp + "\\" + string.Format("Alt_{0:X2}", AppConfig.APP_SubUnit) + ".SDF";
            tmpFiles[2] = AppConfig.Path_Temp + "\\" + "Cli_BC.SDF";
            bool fOK = false;
            string[] loadedFilesOnLoacal = new string[3] { "", "", "" };

            for (int i = 0; i < tmpFiles.Length; i++)
                if (!string.IsNullOrEmpty(exchangeFiles[i]))
                {
                    try
                    {
                        API.OutputDebugString("copy start");
                        fOK = API.CopyFile(exchangeFiles[i], tmpFiles[i], false);
                        API.OutputDebugString("copy end");
                        if (fOK && API.PathFileExists(tmpFiles[i]))
                            loadedFilesOnLoacal[i] = tmpFiles[i];
                    }
                    catch { }
                }

            return loadedFilesOnLoacal;
        }
        /// <summary>
        /// Завантаження інформації товарів
        /// </summary>
        /// <param name="localLoadedTempFiles">Шляхи до файлів</param>
        /// <param name="onlyUpdate">Якщо ture то завантажувати інформацію лише з нових файлів в іншому випадку завантажувати з всіх</param>
        /// <returns>Завантажена інформація</returns>
        public static object[] LoadData(string[] localLoadedTempFiles, bool onlyUpdate)
        {
            FileStream fs = null;
            DataTable[] dTables = new DataTable[3];
            bool exchangeFldUsed = false;
            IntPtr hFile = IntPtr.Zero;
            ReadFunc[] rdFunc = new ReadFunc[3];
            rdFunc[0] = ReadArtSDF;
            rdFunc[1] = ReadAltSDF;
            rdFunc[2] = ReadCardSDF;
            CreateFunc[] crFunc = new CreateFunc[3];
            crFunc[0] = CreateArtDataTable;
            crFunc[1] = CreateAltDataTable;
            crFunc[2] = CreateCardDataTable;

            if (!Directory.Exists(AppConfig.Path_Articles))
                Directory.CreateDirectory(AppConfig.Path_Articles);

            string[] artFiles = new string[3];
            artFiles[0] = AppConfig.Path_Articles + "\\" + string.Format("Art_{0:X2}.saf", AppConfig.APP_SubUnit);
            artFiles[1] = AppConfig.Path_Articles + "\\" + string.Format("Alt_{0:X2}.saf", AppConfig.APP_SubUnit);
            artFiles[2] = AppConfig.Path_Articles + "\\" + "DCards.saf";

            for (int i = 0; i < artFiles.Length; i++)
                if (localLoadedTempFiles != null && localLoadedTempFiles[i] != "")
                {
                    dTables[i] = crFunc[i].Invoke();
                    rdFunc[i].Invoke(localLoadedTempFiles[i], ref dTables[i]);
                    fs = new FileStream(artFiles[i], FileMode.Create);
                    binF.Serialize(fs, dTables[i]);
                    fs.Close();
                    fs.Dispose();
                    exchangeFldUsed = true;
                    File.Delete(localLoadedTempFiles[i]);
                }
                else
                {
                    if (!onlyUpdate && API.PathFileExists(artFiles[i]))
                    {
                        fs = new FileStream(artFiles[i], FileMode.Open, FileAccess.Read);
                        dTables[i] = (DataTable)binF.Deserialize(fs);
                        fs.Close();
                        fs.Dispose();
                    }
                }

            if (fs != null)
                fs.Dispose();

            object[] rez = new object[] { dTables, exchangeFldUsed };

            return rez;
        }//ok
        public static void CreateTables(ref DataTable chq, ref DataTable art, ref DataTable alt, ref DataTable cli)
        {
            chq = CreateChqDataTable();
            art = CreateArtDataTable();
            alt = CreateAltDataTable();
            cli = CreateCardDataTable();
        }//ok
        #region PrivateFunctions
        private delegate DataTable CreateFunc();
        private static DataTable CreateArtDataTable()
        {
            DataTable dTable = new DataTable();
            Type[] cTypes = {
                typeof(string),
                typeof(string),
                typeof(string),
                typeof(string),
                typeof(string),
                typeof(string),
                typeof(string),
                typeof(double),
                typeof(double),
                typeof(double),
                typeof(double),
                typeof(double),
                typeof(double),
                typeof(double),
                typeof(double),
                typeof(double)};

            dTable.Columns.Add("C", typeof(long));

            for (byte i = 0; i < AppConfig.STYLE_ARTColumnName.Length; i++)
                dTable.Columns.Add(AppConfig.STYLE_ARTColumnName[i], cTypes[i]);

            dTable.PrimaryKey = new DataColumn[] { dTable.Columns["C"] };
            dTable.Columns["C"].AutoIncrement = true;
            dTable.Columns["C"].Unique = true;

            return dTable;
        }//ok
        private static DataTable CreateAltDataTable()
        {
            DataTable dTable = new DataTable();
            Type[] cTypes = {
                typeof(string),
                typeof(string)};

            dTable.Columns.Add("C", typeof(long));
            for (byte i = 0; i < AppConfig.STYLE_ALTColumnName.Length; i++)
                dTable.Columns.Add(AppConfig.STYLE_ALTColumnName[i], cTypes[i]);

            dTable.PrimaryKey = new DataColumn[] { dTable.Columns["C"] };
            dTable.Columns["C"].AutoIncrement = true;
            dTable.Columns["C"].Unique = true;

            return dTable;
        }//ok
        private static DataTable CreateCardDataTable()
        {
            DataTable dTable = new DataTable();
            Type[] cTypes = {
                typeof(string),
                typeof(string),
                typeof(double),
                typeof(int)};

            dTable.Columns.Add("C", typeof(long));
            for (byte i = 0; i < AppConfig.STYLE_CARDColumnName.Length; i++)
                dTable.Columns.Add(AppConfig.STYLE_CARDColumnName[i], cTypes[i]);

            dTable.TableName = "DCards";

            dTable.PrimaryKey = new DataColumn[] { dTable.Columns["C"] };
            dTable.Columns["C"].AutoIncrement = true;
            dTable.Columns["C"].Unique = true;

            return dTable;
        }//ok
        private static DataTable CreateChqDataTable()
        {
            DataTable dTable = CreateArtDataTable();

            DataColumn dCol = new DataColumn("TOT");
            dCol.DataType = typeof(string);
            dCol.DefaultValue = "0";
            dTable.Columns.Add(dCol);

            dCol = new DataColumn("TAX_VAL");
            dCol.DataType = typeof(double);
            dCol.DefaultValue = (double)0.0;
            dTable.Columns.Add(dCol);

            dCol = new DataColumn("USEDDISC");
            dCol.DataType = typeof(bool);
            dCol.DefaultValue = (bool)true;
            dTable.Columns.Add(dCol);

            dCol = new DataColumn("DISC");
            dCol.DataType = typeof(double);
            dCol.DefaultValue = (double)0.0;
            dTable.Columns.Add(dCol);

            dCol = new DataColumn("SUM");
            dCol.AllowDBNull = false;
            dCol.DataType = typeof(double);
            dCol.DefaultValue = (double)0.0;
            dTable.Columns.Add(dCol);

            dCol = new DataColumn("ASUM");
            dCol.AllowDBNull = false;
            dCol.DataType = typeof(double);
            dCol.DefaultValue = (double)0.0;
            dTable.Columns.Add(dCol);

            dCol = new DataColumn("TAX_MONEY");
            dCol.DataType = typeof(double);
            //dCol.Expression = "(ASUM*TAX_VAL)/(TAX_VAL+100)";
            dCol.DefaultValue = (double)0.0;
            dTable.Columns.Add(dCol);

            dCol = new DataColumn("TMPTOT");
            dCol.DataType = typeof(string);
            dCol.DefaultValue = "0";
            dTable.Columns.Add(dCol);

            dCol = new DataColumn("ORIGPRICE");
            dCol.DataType = typeof(double);
            dCol.DefaultValue = (double)0.0;
            dTable.Columns.Add(dCol);

            dCol = new DataColumn("PRINTCOUNT");
            dCol.DataType = typeof(double);
            dCol.DefaultValue = (double)0.0;
            dTable.Columns.Add(dCol);

            dTable.PrimaryKey = new DataColumn[] { dTable.Columns["C"] };
            dTable.Columns["C"].AutoIncrement = true;
            dTable.Columns["C"].Unique = true;

            return dTable;
        }//ok
        private delegate void ReadFunc(string path, ref DataTable dTable);
        private static void ReadArtSDF(string path, ref DataTable dTable)
        {
            byte err_cnt = 0;

            if (File.Exists(path))
            {
                StreamReader sr = null;
                do
                {
                    try
                    {
                        sr = new StreamReader(path, Encoding.Default);
                        break;
                    }
                    catch
                    {
                        System.Threading.Thread.Sleep(100);
                        err_cnt++;
                    }

                } while (err_cnt < 10);

                if (sr == null)
                    return;

                dTable.Rows.Clear();
                string line = "";
                DataRow dRow = dTable.NewRow();
                //long index = 0;

                while ((line = sr.ReadLine()) != null)
                {
                    if (line == "")
                        continue;

                    //dRow["C"] = index++;
                    line = line.Replace("\\\\", "\\");
                    dRow["ID"] = line.Substring(0, 10).Trim();//id
                    dRow["BC"] = line.Substring(10, 14).Trim();//skod
                    dRow["NAME"] = line.Substring(24, 35).Trim().Replace('i', 'і').Replace('I', 'І');//name
                    dRow["DESC"] = line.Substring(59, 60).Trim().Replace('i', 'і').Replace('I', 'І');//desc
                    dRow["UNIT"] = line.Substring(119, 15).Trim();//unit
                    dRow["VG"] = line.Substring(134, 1).Trim();//vg
                    if (dRow["VG"].ToString() == "")
                        dRow["VG"] = " ";
                    dRow["TID"] = line.Substring(135, 11).Trim();//tid

                    dRow["TQ"] = GetDouble(line.Substring(146, 12).Trim());//tq
                    dRow["PACK"] = GetDouble(line.Substring(158, 18).Trim());//pack
                    dRow["WEIGHT"] = GetDouble(line.Substring(176, 18).Trim());//weight
                    dRow["PRICE"] = GetDouble(line.Substring(194, 12).Trim());//price
                    dRow["PR1"] = GetDouble(line.Substring(206, 12).Trim());//pr1
                    dRow["PR2"] = GetDouble(line.Substring(218, 12).Trim());//pr2
                    dRow["PR3"] = GetDouble(line.Substring(230, 12).Trim());//pr3
                    dRow["Q2"] = GetDouble(line.Substring(242, 12).Trim());//q2
                    dRow["Q3"] = GetDouble(line.Substring(254, 10).Trim());//q3

                    dTable.Rows.Add(dRow);
                    dRow = dTable.NewRow();
                }
                sr.Close();
                sr.Dispose();
            }
        }//ok
        private static void ReadAltSDF(string path, ref DataTable dTable)
        {
            byte err_cnt = 0;

            if (File.Exists(path))
            {
                StreamReader sr = null;
                do
                {
                    try
                    {
                        sr = new StreamReader(path, Encoding.Default);
                        break;
                    }
                    catch
                    {
                        System.Threading.Thread.Sleep(100);
                        err_cnt++;
                    }

                } while (err_cnt < 10);

                if (sr == null)
                    return;

                string line = "";
                DataRow dRow = dTable.NewRow();
                //long index = 0;

                while ((line = sr.ReadLine()) != null)
                {
                    if (line == "")
                        continue;

                    //dRow["C"] = index++;

                    dRow["ABC"] = line.Substring(0, 20).Trim();//abc
                    dRow["AID"] = line.Substring(20, 10).Trim();//aid
                    dTable.Rows.Add(dRow);
                    dRow = dTable.NewRow();
                }
                sr.Close();
                sr.Dispose();
            }
        }//ok
        private static void ReadCardSDF(string path, ref DataTable dTable)
        {
            byte err_cnt = 0;

            if (File.Exists(path))
            {
                StreamReader sr = null;
                do
                {
                    try
                    {
                        sr = new StreamReader(path, Encoding.Default);
                        break;
                    }
                    catch
                    {
                        System.Threading.Thread.Sleep(100);
                        err_cnt++;
                    }

                } while (err_cnt < 10);

                if (sr == null)
                    return;

                string line = "";
                DataRow dRow = dTable.NewRow();
                //long index = 0;

                while ((line = sr.ReadLine()) != null)
                {
                    if (line == "")
                        continue;

                    //dRow["C"] = index++;

                    dRow["CBC"] = line.Substring(0, 20).Trim();//abc
                    dRow["CID"] = line.Substring(20, 10).Trim();//aid
                    dRow["CDISC"] = GetDouble(line.Substring(30, 6).Trim());//cdisc
                    try
                    {
                        dRow["CPRICENO"] = int.Parse(line.Substring(36).Trim());//cdisc
                    }
                    catch { dRow["CPRICENO"] = (int)0; }
                    dTable.Rows.Add(dRow);
                    dRow = dTable.NewRow();
                }
                sr.Close();
                sr.Dispose();
            }
        }//ok 
        #endregion

        //Rules
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

            if (AppConfig.APP_DiscountRules == null)
                return 0.0;

            for (; i < AppConfig.APP_DiscountRules.Length; i++)
                if (AppConfig.APP_DiscountRules[i].Contains("?"))
                {
                    Array.Resize<string>(ref singleRules, singleRules.Length + 1);
                    singleRules[singleRules.Length - 1] = AppConfig.APP_DiscountRules[i];
                }
                else
                {
                    Array.Resize<string>(ref doubleRules, doubleRules.Length + 1);
                    doubleRules[doubleRules.Length - 1] = AppConfig.APP_DiscountRules[i];
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

        //Cheques
        /// <summary>
        /// Збереження чеку в спеціальний документ
        /// </summary>
        /// <param name="dTable">Таблиця чеку</param>
        /// <param name="param">Інформація чеку</param>
        /// <param name="pType">Тип закриття чеку</param>
        /// <param name="chqNom">Фіскальний номер чеку (якщо чек фіскальний)</param>
        /// <returns>Номер закритого чеку (якщо чек не фіскальний то порядковий номер чеку інакше той самий фіскальний номер)</returns>
        public static string SaveCheque(DataTable dTable, object[] param, int pType, string chqNom)
        {
            if (!Directory.Exists(AppConfig.Path_Cheques))
                Directory.CreateDirectory(AppConfig.Path_Cheques);

            string client = (string)param[0];
            double discount = (double)param[1];
            double chqSUMA = (double)param[2];
            double taxSUMA = (double)param[3];
            bool report = (bool)param[4];
            bool retrive = (bool)param[5];
            bool useTotDisc = (bool)param[6];
            string discColumn = useTotDisc ? "DR" : "DISC";
            string znom = "";
            bool isFx = (chqNom != string.Empty);

            if (param[7] != null)
                znom = param[7].ToString();

            if (retrive)
            {
                chqSUMA *= -1;
                taxSUMA *= -1;

                for (ushort i = 0; retrive && i < dTable.Rows.Count; i++)
                    dTable.Rows[i]["TOT"] = -GetDouble(dTable.Rows[i]["TOT"]);
            }

            if (!isFx)
                NonFxChqsInfo(chqSUMA, ref chqNom);
            
            DataTable ALTdTable = dTable.Copy();
            ALTdTable.Columns.Add("DR", typeof(double));

            DataRow dRow = ALTdTable.NewRow();
            dRow["ID"] = client;
            dRow["PRICE"] = chqSUMA - taxSUMA;
            dRow["TOT"] = taxSUMA;

            if (useTotDisc)
                dRow["DR"] = discount;
            else
                dRow["DR"] = 0.0;

            ALTdTable.Rows.InsertAt(dRow, 0);
            ALTdTable.TableName = ChequeTableName(chqSUMA, report, isFx ? chqNom : "N" + chqNom, retrive, pType, znom);

            ALTdTable.ExtendedProperties.Clear();
            ALTdTable.ExtendedProperties.Add("FIELDS", "ID,PRICE,TOT," + discColumn);

            ALTdTable.Columns["ID"].ExtendedProperties.Add("NAME", "ID");
            ALTdTable.Columns["ID"].ExtendedProperties.Add("SIZE", "10");

            ALTdTable.Columns["PRICE"].ExtendedProperties.Add("NAME", "AP");
            ALTdTable.Columns["PRICE"].ExtendedProperties.Add("SIZE", "12");
            ALTdTable.Columns["PRICE"].ExtendedProperties.Add("DIGITS", "5");

            ALTdTable.Columns["TOT"].ExtendedProperties.Add("NAME", "VQ");
            ALTdTable.Columns["TOT"].ExtendedProperties.Add("SIZE", "12");
            ALTdTable.Columns["TOT"].ExtendedProperties.Add("DIGITS", "3");
            ALTdTable.Columns["TOT"].ExtendedProperties.Add("TYPE", "System.Double");

            ALTdTable.Columns[discColumn].ExtendedProperties.Add("NAME", "DR");
            ALTdTable.Columns[discColumn].ExtendedProperties.Add("SIZE", "9");
            ALTdTable.Columns[discColumn].ExtendedProperties.Add("DIGITS", "2");

            try
            {
                //winapi.Funcs.OutputDebugString("1");
                SaveDBF(ALTdTable, AppConfig.Path_Cheques);
                //winapi.Funcs.OutputDebugString("2");
            }
            catch (Exception e)
            {
                WriteLog(e, MethodInfo.GetCurrentMethod().Name);
            }

            return chqNom;
        }//ok
        public static string[] NonFxChqsInfo(double suma, ref string chqNom)
        {
            string[] localData = new string[3] { "0", DateTime.Now.ToShortDateString(), "0.00" };

            if (!File.Exists(AppConfig.Path_Cheques + "\\" + "base.dat"))
            {
                StreamWriter sw = new StreamWriter(AppConfig.Path_Cheques + "\\" + "base.dat", false, Encoding.Default);
                sw.WriteLine("0");
                sw.WriteLine(DateTime.Now.ToShortDateString());
                sw.Write("0.00");

                if (sw != null)
                    sw.Close();
                sw.Dispose();
            }
            else
            {
                StreamReader sr = File.OpenText(AppConfig.Path_Cheques + "\\" + "base.dat");
                localData[0] = sr.ReadLine();
                localData[1] = sr.ReadLine();
                localData[2] = sr.ReadLine();

                //get info baout cheques (total, date, suma)
                if ((!string.IsNullOrEmpty(localData[1]) && DateTime.Now.ToShortDateString() != localData[1]))
                {
                    localData[0] = "0";//total chqs
                    localData[1] = DateTime.Now.ToShortDateString();//date
                    localData[2] = "0.00";//Chqs sum
                }
                
                if (sr != null)
                    sr.Close();
                sr.Dispose();
            }

            try
            {
                long cN = long.Parse(localData[0]);
                cN++;
                chqNom = cN.ToString();
            }
            catch
            {
                chqNom = "1";
            }

            if (suma != 0.0)
            {
                //save cheque and get his number.
                StreamWriter sw = new StreamWriter(AppConfig.Path_Cheques + "\\" + "base.dat", false, Encoding.Default);
                sw.WriteLine(chqNom);
                sw.WriteLine(localData[1]);
                sw.Write(GetDouble(localData[2]) + suma);

                if (sw != null)
                    sw.Close();
                sw.Dispose();
            }

            return localData;
        }//ok
        #region PrivateFunctions
        /// <summary>
        /// Створення назви для документу чеку
        /// </summary>
        /// <param name="suma">Сума чеку</param>
        /// <param name="rep">Якщо true то чек вимагає накладної</param>
        /// <param name="nom">Номер чеку</param>
        /// <param name="ret">Якщо true то чек є чеком поверення в іншому випадку це звичайний чек</param>
        /// <param name="pType">Тип закриття чеку</param>
        /// <param name="znom">Номер Z-звіту (для фіскального чеку)</param>
        /// <returns>Назва для файлу чеку</returns>
        private static string ChequeTableName(double suma, bool rep, string nom, bool ret, int pType, string znom)
        {
            string[] values = new string[9];
            values[0] = string.Format("{0:X2}", AppConfig.APP_SubUnit);
            values[1] = string.Format("{0:X2}", AppConfig.APP_PayDesk);
            values[2] = string.Format("{0:yyMMdd}", DateTime.Now);
            values[3] = nom;
            values[4] = suma.ToString();
            values[5] = (rep ? "$" : "-");
            //Pyament type integrated on EKKR IKC-E260 payment types
            switch (pType)
            {
                case 0: values[6] = "K"; break;//card
                case 1: values[6] = "%"; break;//credit
                case 2: values[6] = "#"; break;//cheque
                case 3: values[6] = "H"; break;//cash
            }
            values[7] = UserStruct.UserLogin;
            values[8] = znom;

            string chqName = AppConfig.APP_ChequeName;
            for (byte i = 0; i < values.Length; i++)
                chqName = chqName.Replace("%" + i, values[i]);

            if (File.Exists(AppConfig.Path_Cheques + "\\" + "C" + chqName + ".DBF"))
                return ChequeTableName(suma, rep, nom + "c", ret, pType, znom);
            else
                return "C" + chqName;
        }//ok
        #endregion

        //Invent
        /// <summary>
        /// Збереження чеку інвентаризації
        /// </summary>
        /// <param name="dTable">Таблиця чеку</param>
        /// <param name="isBackUp">Якщо true то чек буде збережено, як резервну копію
        /// в іншому випадку буде чек буде збережено як кінцевий документ</param>
        public static void SaveInvent(DataTable dTable, bool isBackUp)
        {
            if (File.Exists(AppConfig.Path_Cheques + "\\" + "_" + dTable.ExtendedProperties["Name"] + ".inv"))
                FileSystem.DeleteFile(AppConfig.Path_Cheques + "\\" + "_" + dTable.ExtendedProperties["Name"] + ".inv",
                    UIOption.OnlyErrorDialogs,
                    RecycleOption.SendToRecycleBin);
            if (File.Exists(AppConfig.Path_Cheques + "\\" + dTable.ExtendedProperties["Name"] + ".dbf"))
                FileSystem.DeleteFile(AppConfig.Path_Cheques + "\\" + dTable.ExtendedProperties["Name"] + ".dbf",
                    UIOption.OnlyErrorDialogs,
                    RecycleOption.SendToRecycleBin);

            //dTable.TableName = dTable.ExtendedProperties["Name"].ToString();
            if (bool.Parse(dTable.ExtendedProperties["GetNewName"].ToString()))
            {
                dTable.TableName = InventTableName(DateTime.Now);
                dTable.ExtendedProperties["Name"] = dTable.TableName;
                dTable.ExtendedProperties["Date"] = DateTime.Now.ToShortDateString();
            }

            FileStream stream = new FileStream(AppConfig.Path_Cheques + "\\" + "_" + dTable.ExtendedProperties["Name"] + ".inv",
                FileMode.OpenOrCreate);
            binF.Serialize(stream, dTable);
            stream.Close();
            stream.Dispose();

            if (!isBackUp)
            {
                DataTable ALTdTable = dTable.Copy();
                ALTdTable.Columns.Add("AP", typeof(double));

                ALTdTable.ExtendedProperties.Add("FIELDS", "ID,AP,TOT,PRICE");

                ALTdTable.Columns["ID"].ExtendedProperties.Clear();
                ALTdTable.Columns["ID"].ExtendedProperties.Add("NAME", "ID");
                ALTdTable.Columns["ID"].ExtendedProperties.Add("SIZE", "10");

                ALTdTable.Columns["AP"].ExtendedProperties.Clear();
                ALTdTable.Columns["AP"].ExtendedProperties.Add("NAME", "AP");
                ALTdTable.Columns["AP"].ExtendedProperties.Add("SIZE", "9");
                ALTdTable.Columns["AP"].ExtendedProperties.Add("DIGITS", "2");

                ALTdTable.Columns["TOT"].ExtendedProperties.Clear();
                ALTdTable.Columns["TOT"].ExtendedProperties.Add("NAME", "VQ");
                ALTdTable.Columns["TOT"].ExtendedProperties.Add("SIZE", "12");
                ALTdTable.Columns["TOT"].ExtendedProperties.Add("DIGITS", "3");
                ALTdTable.Columns["TOT"].ExtendedProperties.Add("TYPE", "System.Double");

                ALTdTable.Columns["PRICE"].ExtendedProperties.Clear();
                ALTdTable.Columns["PRICE"].ExtendedProperties.Add("NAME", "DR");
                ALTdTable.Columns["PRICE"].ExtendedProperties.Add("SIZE", "12");
                ALTdTable.Columns["PRICE"].ExtendedProperties.Add("DIGITS", "5");

                SaveDBF(ALTdTable, AppConfig.Path_Cheques);

                ALTdTable.ExtendedProperties.Clear();
                ALTdTable.Columns["ID"].ExtendedProperties.Clear();
                ALTdTable.Columns["PACK"].ExtendedProperties.Clear();
                ALTdTable.Columns["TOT"].ExtendedProperties.Clear();
                ALTdTable.Columns["PRICE"].ExtendedProperties.Clear();
            }
        }//ok
        /// <summary>
        /// Відкриття інвентаризаційного чеку
        /// </summary>
        /// <returns>Таблиця інветаризаційного чеку</returns>
        public static DataTable OpenInvent()
        {
            DataTable dTable = new DataTable();

            DateTime? dTime = DateTime.Now;
            if (AppConfig.APP_ShowInventWindow)
                dTime = new InventList().OpenInvent();
            string tbName = InventTableName(dTime);
            if (tbName == "") 
                return null;
            //{
            //    dTable.ExtendedProperties["Load"] = false;
            //    return dTable;
            //}

            if (!File.Exists(AppConfig.Path_Cheques + "\\" + "_" + tbName + ".inv"))
            {
                dTable.ExtendedProperties.Add("Name", tbName);
                dTable.ExtendedProperties.Add("GetNewName", true);
                dTable.ExtendedProperties.Add("Date", dTime.Value.ToShortDateString());
                return dTable;
            }

            FileStream stream = new FileStream(AppConfig.Path_Cheques + "\\" + "_" + tbName + ".inv", FileMode.Open);
            dTable = (DataTable)binF.Deserialize(stream);
            stream.Close();
            stream.Dispose();

            if (dTime.Value.Day != DateTime.Now.Day ||
                dTime.Value.Month != DateTime.Now.Month ||
                dTime.Value.Year != DateTime.Now.Year)
                dTable.ExtendedProperties["GetNewName"] = false;

            return dTable;
        }//ok
        private static string InventTableName(DateTime? date)
        {
            if (!date.HasValue)
                return "";

            string[] values = new string[3];
            values[0] = string.Format("{0:X2}", AppConfig.APP_SubUnit);
            values[1] = string.Format("{0:X2}", AppConfig.APP_PayDesk);
            values[2] = string.Format("{0:yyMMdd}", date.Value);

            string NamebyFormat = "%0%1_%2";

            for (byte i = 0; i < values.Length; i++)
                NamebyFormat = NamebyFormat.Replace("%" + i, values[i]);

            return "IS" + NamebyFormat;
        }//ok

        //Printing
        /// <summary>
        /// Сворює чек в текстовому файлі
        /// </summary>
        /// <param name="tplFile">Шлях до шаблону за яким буде відбуватися форматування даних в файлі</param>
        /// <param name="data">Дані, які використовуються під час форматування чеку</param>
        /// <returns>Шлях до свореного файлу</returns>
        public static string FormTxtChq(string tplFile, object[] data)
        {
            if (!File.Exists(tplFile))
                return "";

            if (!Directory.Exists(AppConfig.Path_Temp))
                Directory.CreateDirectory(AppConfig.Path_Temp);

            string dCor = string.Empty;
            for (byte i = 0; i < AppConfig.APP_MoneyDecimals; i++)
                dCor += '0';

            //Set directives
            byte LoAN = 15;
            byte LoAD = 20;
            byte TSpL = 0;

            //Load messages
            Dictionary<string, string> _MSG = new Dictionary<string, string>();
            using (System.Xml.XmlTextReader xmlRd = new System.Xml.XmlTextReader(AppConfig.Path_Templates + "\\" + "Messages.xml"))
            {
                string varName = string.Empty;
                xmlRd.WhitespaceHandling = System.Xml.WhitespaceHandling.None;
                while (xmlRd.Read())
                    if (xmlRd.NodeType == System.Xml.XmlNodeType.Element && xmlRd.Name != null
                        && xmlRd.Name != string.Empty && xmlRd.Name.ToUpper() != "MESSAGES")
                    {
                        varName = xmlRd.Name;
                        xmlRd.Read();
                        _MSG.Add(varName.ToUpper(), xmlRd.Value.TrimStart('\r', '\n').TrimEnd('\r', '\n'));
                    }
                xmlRd.Close();
            }

            // Aligment blocks
            string[][] _alignBlocks = new string[2][];
            string[] _separators = new string[] { "[hex:", "[dec:" };
            _alignBlocks[0] = new string[] { "[right]", "[center]" };
            _alignBlocks[1] = new string[] { "[\\right]", "[\\center]" };
            // Separators

            //Making info values
            object[] infoV = null;
            #region Info values
            //input data
            //0 - chqTable (DataTable)
            //1 - chqNumber (string)
            //2 - retriveCheque (bool)
            //3 - fix (bool)
            //4 - chequeSuma (double)
            //5 - realSuma (double)
            //6 - paymentTypes (List)
            //7 - buyersCash (double)
            //8 - buyersItemCash (List)
            //9 - buyersRest (double)
            //10 - useTotDiscount (bool)
            //11 - discountPercent (double[])
            //12 - discountCash (double[])
            //13 - discConstPercent (double)
            //14 - null
            //15 - discOnlyPercent (double)
            //16 - discOnlyCash (double)
            //17 - discCommonPercent (double)
            //18 - discCommonCash (double)
            //19 - billNumber (string)
            //20 - billComment (string)

            string SiMask = "{0:0." + dCor + ";0." + dCor + ";0." + dCor + "}";
            string DiMask = "{0:0." + dCor + ";0." + dCor + ";!ZeRo!}";
            List<byte> paymentTypes = (List<byte>)data[6];
            List<double> buyersMoney = (List<double>)data[8];

            //Info values (is as type).
            object[] sValues = new object[40];
            //00 - subUnit (byte)
            sValues[00] = AppConfig.APP_SubUnit;//0
            //01 - subUnitName (string)
            sValues[01] = AppConfig.APP_SubUnitName;//1
            //02 - payDesk (byte)
            sValues[02] = AppConfig.APP_PayDesk;//2
            //03 - cashierName (string)
            sValues[03] = UserStruct.UserID;//3
            //04 - currentDateTime (DateTime)
            sValues[04] = DateTime.Now;//4
            //05 - chequeNumber (string)
            sValues[05] = data[1];//5
            //06 - chequeType (bool) fix || !fix
            sValues[06] = (bool)data[3] ? 1 : 0;//6
            //07 - retirveCheque (bool)
            sValues[07] = (bool)data[2] ? 1 : 0;//7
            //08 - chequeSuma (double)
            sValues[08] = data[4];//8
            //09 - realSuma (double)
            sValues[09] = data[5];//9
            //10 - --null-- payment Type [cash] (string)
            sValues[10] = null;//_MSG["PAYMENT_CASH"],//10
            //11 - --null-- payment Type [card] (string)
            sValues[11] = null;//_MSG["PAYMENT_CARD"],//11
            //12 - --null-- payment Type [credit] (string)
            sValues[12] = null;//_MSG["PAYMENT_CREDIT"],//12
            //13 - --null-- payment Type [cheque] (string)
            sValues[13] = null;//_MSG["PAYMENT_CHEQUE"],//13
            //14 - buyers money [cash] (double)
            sValues[14] = paymentTypes.IndexOf((byte)3) >= 0 ? buyersMoney[paymentTypes.IndexOf((byte)3)] : 0;//14
            //15 - buyers money [card] (double)
            sValues[15] = paymentTypes.IndexOf((byte)0) >= 0 ? buyersMoney[paymentTypes.IndexOf((byte)0)] : 0;//15
            //16 - buyers money [credit] (double)
            sValues[16] = paymentTypes.IndexOf((byte)1) >= 0 ? buyersMoney[paymentTypes.IndexOf((byte)1)] : 0;//16
            //17 - buyers money [cheque] (double)
            sValues[17] = paymentTypes.IndexOf((byte)2) >= 0 ? buyersMoney[paymentTypes.IndexOf((byte)2)] : 0;//17
            //18 - billNumber (string)
            sValues[18] = data[19];//18
            //19 - billComment (string)
            sValues[19] = data[20];//19
            //20 - buyersCash (double)
            sValues[20] = data[7];//20
            //21 - buyersRest (double)
            sValues[21] = (buyersMoney.Count == 1 && paymentTypes[0] == 3) ? data[9] : 0;//21
            //22 - useTotDiscount (bool)
            sValues[22] = (bool)data[10] ? 1 : 0;//22
            //23 - discountPercent (double)
            sValues[23] = ((double[])data[11])[0];//23
            //24 - [-discountPercent] (double)
            sValues[24] = ((double[])data[11])[1];//24
            //25 - discountCash (double)
            sValues[25] = ((double[])data[12])[0];//25
            //26 - [-discountCash] (double)
            sValues[26] = ((double[])data[12])[1];//26
            //27 - discountConstPercent (double)
            sValues[27] = data[13];//27
            //28 - -
            sValues[28] = null;//28
            //29 - E_discountPercent[] (double)
            sValues[29] = data[15];//29
            //30 - E_discountCash[] (double)
            sValues[30] = data[16];//30
            //31 - discountCommonPercent (double)
            sValues[31] = data[17];//31
            //32 - discountCommonCash (double)
            sValues[32] = data[18];//32
            //33 - -
            sValues[33] = null;//33
            //34 - -
            sValues[34] = null;//34
            //35 - -
            sValues[35] = null;//35
            //36 - -
            sValues[36] = null;//36
            //37 - -
            sValues[37] = null;//37
            //38 - -
            sValues[38] = null;//38
            //39 - -
            sValues[39] = null;//39

            infoV = new object[sValues.Length];
            Array.Copy(sValues, infoV, sValues.Length);

            //Info values (type parsed to string (used SiMaks)).
            object[] sStrValues = new object[40];
            //40 - -
            sStrValues[00] = null;//40
            //41 - -
            sStrValues[01] = null;//41
            //42 - -
            sStrValues[02] = null;//42
            //43 - -
            sStrValues[03] = null;//43
            //44 - -
            sStrValues[04] = null;//44
            //45 - -
            sStrValues[05] = null;//45
            //46 - -
            sStrValues[06] = null;//46
            //47 - -
            sStrValues[07] = null;//47
            //48 - chequeSuma (double)
            sStrValues[08] = string.Format(SiMask, data[4]);//48 - chequeSuma
            //49 - realSuma (double)
            sStrValues[09] = string.Format(SiMask, data[5]);//49 - realSuma
            //50 - -
            sStrValues[10] = null;//_MSG["PAYMENT_CASH"];//50
            //51 - -
            sStrValues[11] = null;//_MSG["PAYMENT_CARD"];//51
            //52 - -
            sStrValues[12] = null;//_MSG["PAYMENT_CREDIT"];//52
            //53 - -
            sStrValues[13] = null;//_MSG["PAYMENT_CHEQUE"];//53
            //54 - buyers money [cash] (double)
            sStrValues[14] = string.Format(SiMask, paymentTypes.IndexOf((byte)3) >= 0 ? buyersMoney[paymentTypes.IndexOf((byte)3)] : 0);//54
            //55 - buyers money [card] (double)
            sStrValues[15] = string.Format(SiMask, paymentTypes.IndexOf((byte)0) >= 0 ? buyersMoney[paymentTypes.IndexOf((byte)0)] : 0);//55
            //56 - buyers money [credit] (double)
            sStrValues[16] = string.Format(SiMask, paymentTypes.IndexOf((byte)1) >= 0 ? buyersMoney[paymentTypes.IndexOf((byte)1)] : 0);//56
            //57 - buyers money [cheque] (double)
            sStrValues[17] = string.Format(SiMask, paymentTypes.IndexOf((byte)2) >= 0 ? buyersMoney[paymentTypes.IndexOf((byte)2)] : 0);//57
            //58 - -
            sStrValues[18] = null;//58
            //59 - -
            sStrValues[19] = null;//59
            //60 - buyersCash (double)
            sStrValues[20] = string.Format(SiMask, data[7]);//60 - 
            //61 - buyersRest (double)
            sStrValues[21] = string.Format(SiMask, (buyersMoney.Count == 1 && paymentTypes[0] == 3) ? data[9] : 0);//61 - 
            //62 - -
            sStrValues[22] = null;//62
            //63 - discountPercent (double)
            sStrValues[23] = string.Format(SiMask, ((double[])data[11])[0]);//63 - 
            //64 - [-discountPercent] (double)
            sStrValues[24] = string.Format(SiMask, ((double[])data[11])[1]);//64 - 
            //65 - discountCash (double)
            sStrValues[25] = string.Format(SiMask, ((double[])data[12])[0]);//65 - 
            //66 - [-discountCash] (double)
            sStrValues[26] = string.Format(SiMask, ((double[])data[12])[1]);//66 - 
            //67 - discountConstPercent (double)
            sStrValues[27] = string.Format(SiMask, data[13]);//67 - 
            //68 - -
            sStrValues[28] = null;//68
            //69 - E_discountPercent[] (double)
            sStrValues[29] = string.Format(SiMask, data[15]);//69 - 
            //70 - E_discountCash[] (double) 
            sStrValues[30] = string.Format(SiMask, data[16]);//70 - 
            //71 - discountCommonPercent (double)
            sStrValues[31] = string.Format(SiMask, data[17]);//71 - 
            //72 - discountCommonCash (double)
            sStrValues[32] = string.Format(SiMask, data[18]);//72 - 
            //73 - -
            sStrValues[33] = null;//73 - 
            //74 - -
            sStrValues[34] = null;//74 - 
            //75 - -
            sStrValues[35] = null;//75 - 
            //76 - -
            sStrValues[36] = null;//76 - 
            //77 - -
            sStrValues[37] = null;//77 - 
            //78 - -
            sStrValues[38] = null;//78 - 
            //79 - -
            sStrValues[39] = null;//79 - 

            Array.Resize<object>(ref infoV, infoV.Length + sStrValues.Length);
            Array.Copy(sStrValues, 0, infoV, infoV.Length - sStrValues.Length, sStrValues.Length);
            
            //Info values (type parsed to string (used DiMaks)).
            object[] sDynStrValues = new object[40];
            //80 - -
            sDynStrValues[00] = null;//80
            //81 - -
            sDynStrValues[01] = null;//81
            //82 - -
            sDynStrValues[02] = null;//82
            //83 - -
            sDynStrValues[03] = null;//83
            //84 - -
            sDynStrValues[04] = null;//84
            //85 - -
            sDynStrValues[05] = null;//85
            //86 - -
            sDynStrValues[06] = null;//86
            //87 - -
            sDynStrValues[07] = null;//87
            //88 - chequeSuma (double)
            sDynStrValues[08] = null;//88 - chequeSuma (double)
            //89 - realSuma (double)
            sDynStrValues[09] = null;//89 - realSuma (double)
            //90 - -
            sDynStrValues[10] = null;//_MSG["PAYMENT_CASH"];//90
            //91 - -
            sDynStrValues[11] = null;//_MSG["PAYMENT_CARD"];//91
            //92 - -
            sDynStrValues[12] = null;//_MSG["PAYMENT_CREDIT"];//92
            //93 - -
            sDynStrValues[13] = null;//_MSG["PAYMENT_CHEQUE"];//93
            //94 - buyers money [cash] (double)
            sDynStrValues[14] = string.Format(DiMask, paymentTypes.IndexOf((byte)3) >= 0 ? buyersMoney[paymentTypes.IndexOf((byte)3)] : 0);//94
            //95 - buyers money [card] (double)
            sDynStrValues[15] = string.Format(DiMask, paymentTypes.IndexOf((byte)0) >= 0 ? buyersMoney[paymentTypes.IndexOf((byte)0)] : 0);//95
            //96 - buyers money [credit] (double)
            sDynStrValues[16] = string.Format(DiMask, paymentTypes.IndexOf((byte)1) >= 0 ? buyersMoney[paymentTypes.IndexOf((byte)1)] : 0);//96
            //97 - buyers money [cheque] (double)
            sDynStrValues[17] = string.Format(DiMask, paymentTypes.IndexOf((byte)2) >= 0 ? buyersMoney[paymentTypes.IndexOf((byte)2)] : 0);//97
            //98 - billNumber (string)
            sDynStrValues[18] = data[19].ToString() == string.Empty ? "!ZeRo!" : data[19];//98
            //99 - billComment (string)
            sDynStrValues[19] = data[20].ToString() == string.Empty ? "!ZeRo!" : data[20];//99
            //100 - buyersCash (double)
            sDynStrValues[20] = null;//100 - 
            //101 - buyersRest (double)
            sDynStrValues[21] = string.Format(DiMask, (buyersMoney.Count == 1 && paymentTypes[0] == 3) ? data[9] : 0);//101 - 
            //102 - -
            sDynStrValues[22] = null;//102
            //103 - discountPercent (double)
            sDynStrValues[23] = string.Format(DiMask, ((double[])data[11])[0]);//103 - 
            //104 - [-discountPercent] (double)
            sDynStrValues[24] = string.Format(DiMask, ((double[])data[11])[1]);//104 - 
            //105 - discountCash (double)
            sDynStrValues[25] = string.Format(DiMask, ((double[])data[12])[0]);//105 - 
            //106 - [-discountCash] (double)
            sDynStrValues[26] = string.Format(DiMask, ((double[])data[12])[1]);//106 - 
            //107 - discountConstPercent (double)
            sDynStrValues[27] = string.Format(DiMask, data[13]);//107 - 
            //108 - -
            sDynStrValues[28] = null;//108
            //109 - E_discountPercent[] (double)
            sDynStrValues[29] = string.Format(DiMask, data[15]);//109 - 
            //110 - E_discountCash[] (double) 
            sDynStrValues[30] = string.Format(DiMask, data[16]);//110 - 
            //111 - discountCommonPercent (double)
            sDynStrValues[31] = string.Format(DiMask, data[17]);//111 - 
            //112 - discountCommonCash (double)
            sDynStrValues[32] = string.Format(DiMask, data[18]);//112 - 
            //113 - -
            sDynStrValues[33] = null;//113 - 
            //114 - -
            sDynStrValues[34] = null;//114 - 
            //115 - -
            sDynStrValues[35] = null;//115 - 
            //116 - -
            sDynStrValues[36] = null;//116 - 
            //117 - -
            sDynStrValues[37] = null;//117 - 
            //118 - -
            sDynStrValues[38] = null;//118 - 
            //119 - -
            sDynStrValues[39] = null;//119 - 

            Array.Resize<object>(ref infoV, infoV.Length + sDynStrValues.Length);
            Array.Copy(sDynStrValues, 0, infoV, infoV.Length - sDynStrValues.Length, sDynStrValues.Length);

            #endregion

            //Creating output file
            string chqTxtName = string.Format("{0:X2}{1:X2}_{2:yyMMdd}_{2:HHmmss}.txt", AppConfig.APP_SubUnit, AppConfig.APP_PayDesk, DateTime.Now);
            StreamWriter streamWr = new StreamWriter(AppConfig.Path_Temp + "\\" + chqTxtName, false, Encoding.Default);
            chqTxtName = AppConfig.Path_Temp + "\\" + chqTxtName;

            //Load template
            StreamReader streamRd = File.OpenText(tplFile);

            //Fill data by loaded template
            string template = string.Empty;
            while ((template = streamRd.ReadLine()) != null)
            {
                if (template == string.Empty)
                    template = " ";

                template = template.Replace("\\r", "\r");
                template = template.Replace("\\n", "\n");
                template = template.Replace("\\t", "\t");
                template = template.Replace("\\v", "\v");
                template = template.Replace("\\b", "\b");

                switch (template[0])
                {
                    #region Info values [S]
                    case 'S':
                        {
                            template = template.Remove(0, 1);
                            template = string.Format(template, infoV);

                            if (!template.Contains("!ZeRo!"))
                            {
                                template = GetHEX(template, _separators);
                                streamWr.WriteLine(template);
                            }
                            break;
                        }
                    #endregion
                    #region Articles [W]
                    case 'W':
                        {
                            //Output Format [W]
                            //00 - articleName (string)
                            //01 - articleDecription (string)
                            //02 - articleUnit (string)
                            //03 - articleTaxChar (char)
                            //04 - articlePrice (double)
                            //05 - articleDose (double)
                            //06 - articleTaxValue (double)
                            //07 - articleDiscount (double)
                            //08 - articleSuma (doulble)
                            //09 - articleActualSum (double)
                            //10 - articleTaxMoney (double)
                            //11 - articleCashDiscount (double)

                            //static values parsed as string [Program decimal correction]
                            //20 - articlePrice (double)
                            //21 - articleDose (double)
                            //22 - articleTaxValue (double)
                            //23 - articleDiscount (double)
                            //24 - articleSuma (double)
                            //25 - articleActualSum (double)
                            //26 - articleTaxMoney (double)
                            //27 - articleCashDiscount (double)

                            string articleTemplate = template.Remove(0, 1);
                            string art_name = "";
                            string art_desc = "";

                            for (int i = 0; i < ((DataTable)data[0]).Rows.Count; i++)
                            {
                                art_name = ((DataTable)data[0]).Rows[i]["NAME"].ToString();
                                art_desc = ((DataTable)data[0]).Rows[i]["DESC"].ToString();

                                if (art_name.Length > LoAN)
                                    art_name = art_name.Substring(0, LoAN);

                                if (art_desc.Length > LoAD)
                                    art_desc = art_desc.Substring(0, LoAD);

                                template = string.Format(articleTemplate,
                                    //0
                                    art_name,
                                    art_desc,
                                    ((DataTable)data[0]).Rows[i]["UNIT"],
                                    ((DataTable)data[0]).Rows[i]["VG"],
                                    ((DataTable)data[0]).Rows[i]["PRICE"],
                                    ((DataTable)data[0]).Rows[i]["TOT"],
                                    ((DataTable)data[0]).Rows[i]["TAX_VAL"],
                                    (bool)data[10] ? 0 : (double)((DataTable)data[0]).Rows[i]["DISC"] / 100,
                                    ((DataTable)data[0]).Rows[i]["SUM"],
                                    ((DataTable)data[0]).Rows[i]["ASUM"],
                                    //10
                                    ((DataTable)data[0]).Rows[i]["TAX_MONEY"],
                                    GetRoundedMoney((double)((System.Data.DataTable)data[0]).Rows[i]["SUM"] - (double)((System.Data.DataTable)data[0]).Rows[i]["ASUM"]),//11
                                    null,//12
                                    null,//13
                                    null,//14
                                    null,//15
                                    null,//16
                                    null,//17
                                    null,//18
                                    null,//19
                                    string.Format("{0:F" + AppConfig.APP_MoneyDecimals + "}", ((DataTable)data[0]).Rows[i]["PRICE"]),
                                    string.Format("{0:F" + AppConfig.APP_DoseDecimals + "}", ((DataTable)data[0]).Rows[i]["TOT"]),
                                    string.Format("{0:F" + AppConfig.APP_MoneyDecimals + "}", ((DataTable)data[0]).Rows[i]["TAX_VAL"]),
                                    (bool)data[10] ? "" : string.Format("{0:-0." + dCor + "%;+0." + dCor + "%;0." + dCor + "%}", (double)((DataTable)data[0]).Rows[i]["DISC"] / 100),
                                    string.Format("{0:F" + AppConfig.APP_MoneyDecimals + "}", ((DataTable)data[0]).Rows[i]["SUM"]),
                                    string.Format("{0:F" + AppConfig.APP_MoneyDecimals + "}", ((DataTable)data[0]).Rows[i]["ASUM"]),
                                    string.Format("{0:F" + AppConfig.APP_MoneyDecimals + "}", ((DataTable)data[0]).Rows[i]["TAX_MONEY"]),
                                    string.Format("{0:F" + AppConfig.APP_MoneyDecimals + "}", GetRoundedMoney((double)((System.Data.DataTable)data[0]).Rows[i]["SUM"] - (double)((System.Data.DataTable)data[0]).Rows[i]["ASUM"]))
                                );

                                if (!template.Contains("!ZeRo!"))
                                {
                                    if (i == 0)
                                        template = template.TrimStart(new char[] { '\n', '\r' });
                                    if (i + 1 == ((DataTable)data[0]).Rows.Count)
                                        template = template.TrimEnd(new char[] { '\n', '\r' });

                                    template = GetHEX(template, _separators);
                                    streamWr.WriteLine(template);
                                }
                            }

                            break;
                        } 
                    #endregion
                    #region Directives [#]
                    case '#':
                        {
                            //Template variables
                            //LoAN - Limit of Article Name (byte)
                            //LoAD - Limit of Article Description (byte)
                            //TSpL - Total Symbols per Line (byte)

                            template = template.Remove(0, 1);
                            object[] var = template.Split('=');

                            try
                            {
                                switch (var[0].ToString())
                                {
                                    case "LoAN": LoAN = byte.Parse(var[1].ToString()); break;
                                    case "LoAD": LoAD = byte.Parse(var[1].ToString()); break;
                                    case "TSpL": TSpL = byte.Parse(var[1].ToString()); break;
                                }
                            }
                            catch { }

                            break;
                        } 
                    #endregion
                    #region Messages [M]
                    case 'M':
                        {
                            template = template.Remove(0, 1).ToUpper();
                            string[] keys = new string[_MSG.Keys.Count];
                            string[] blocks = new string[0];
                            string newLine = string.Empty;
                            string cuttedLine = string.Empty;
                            int startIndex = 0;
                            int length = 0;
                            int i = 0;
                            int j = 0;

                            _MSG.Keys.CopyTo(keys, 0);

                            try
                            {
                                for (i = 0; i < _MSG.Keys.Count; i++)
                                    template = template.Replace(keys[i], _MSG[keys[i]]);
                            }
                            catch { }

                            //Aligment
                            for (i = 0; i < _alignBlocks[0].Length; i++)
                            {
                                while (template.Contains(_alignBlocks[0][i]) && template.Contains(_alignBlocks[1][i]))
                                {
                                    startIndex = template.IndexOf(_alignBlocks[0][i]) + _alignBlocks[0][i].Length;
                                    length = template.IndexOf(_alignBlocks[1][i]) - startIndex;

                                    newLine = cuttedLine = template.Substring(startIndex, length);
                                    blocks = newLine.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                                    newLine = string.Empty;
                                    for (j = 0; j < blocks.Length; j++)
                                    {
                                        switch (_alignBlocks[0][i])
                                        {
                                            case "[center]":
                                                if (blocks[j].Length % 2 == 0)
                                                    length = (TSpL - blocks[j].Length) / 2 + blocks[j].Length;
                                                else
                                                    length = (TSpL - (blocks[j].Length - 1)) / 2 + blocks[j].Length;
                                                break;
                                            case "[right]":
                                                length = TSpL;
                                                break;
                                        }
                                        blocks[j] = blocks[j].PadLeft(length);
                                        newLine += blocks[j];
                                        if (j + 1 < blocks.Length)
                                            newLine += "\r\n";
                                    }
                                    cuttedLine = _alignBlocks[0][i] + cuttedLine + _alignBlocks[1][i];
                                    template = template.Replace(cuttedLine, newLine);
                                }

                                //remove first single aligment's block
                                if (template.Contains(_alignBlocks[0][i]) && !template.Contains(_alignBlocks[1][i]))
                                    template = template.Replace(_alignBlocks[0][i], "");

                                //remove end single aligment's block
                                if (!template.Contains(_alignBlocks[0][i]) && template.Contains(_alignBlocks[1][i]))
                                    template = template.Replace(_alignBlocks[1][i], "");
                            }

                            template = GetHEX(template, _separators);
                            streamWr.WriteLine(template);

                            break;
                        } 
                    #endregion
                    #region Comment [;]
                    case ';':
                        {
                            break;
                        } 
                    #endregion
                    default:
                        {
                            template = GetHEX(template, _separators);
                            streamWr.WriteLine(template);
                            break;
                        }
                }
            }

            //End fill data
            streamWr.Close();
            streamWr.Dispose();
            streamRd.Close();
            streamRd.Dispose();

            //Path for txt file.
            return chqTxtName;
        }//ok
        public static string FormTxtBill(string tplFile, DataTable dTable)
        {
            if (!File.Exists(tplFile))
                return "";

            if (!Directory.Exists(AppConfig.Path_Temp))
                Directory.CreateDirectory(AppConfig.Path_Temp);

            string dCor = string.Empty;
            for (byte i = 0; i < AppConfig.APP_MoneyDecimals; i++)
                dCor += '0';

            //Set directives
            byte LoAN = 15;
            byte LoAD = 20;
            byte TSpL = 0;

            //Load messages
            Dictionary<string, string> _MSG = new Dictionary<string, string>();
            using (System.Xml.XmlTextReader xmlRd = new System.Xml.XmlTextReader(AppConfig.Path_Templates + "\\" + "Messages.xml"))
            {
                string varName = string.Empty;
                xmlRd.WhitespaceHandling = System.Xml.WhitespaceHandling.None;
                while (xmlRd.Read())
                    if (xmlRd.NodeType == System.Xml.XmlNodeType.Element && xmlRd.Name != null
                        && xmlRd.Name != string.Empty && xmlRd.Name.ToUpper() != "MESSAGES")
                    {
                        varName = xmlRd.Name;
                        xmlRd.Read();
                        _MSG.Add(varName.ToUpper(), xmlRd.Value.TrimStart('\r', '\n').TrimEnd('\r', '\n'));
                    }
                xmlRd.Close();
            }

            // Aligment blocks
            string[][] _alignBlocks = new string[2][];
            string[] _separators = new string[] { "[hex:", "[dec:" };
            _alignBlocks[0] = new string[] { "[right]", "[center]" };
            _alignBlocks[1] = new string[] { "[\\right]", "[\\center]" };
            // Separators

            //Making info values
            #region Info values
            object[] infoV = new object[7];
            //00 - subUnit (byte)
            infoV[00] = AppConfig.APP_SubUnit;//0
            //01 - subUnitName (string)
            infoV[01] = AppConfig.APP_SubUnitName;//1
            //02 - payDesk (byte)
            infoV[02] = AppConfig.APP_PayDesk;//2
            //03 - cashierName (string)
            infoV[03] = UserStruct.UserID;//3
            //04 - currentDateTime (DateTime)
            infoV[04] = DateTime.Now;//4
            //05 - billNumber (string)
            infoV[05] = dTable.ExtendedProperties["NOM"].ToString();//05
            //19 - billComment (string)
            infoV[06] = dTable.ExtendedProperties["CMT"].ToString();//06
            
            #endregion

            //Creating output file
            string chqTxtName = string.Format("{0:X2}{1:X2}_{2:yyMMdd}_{2:HHmmss}.txt", AppConfig.APP_SubUnit, AppConfig.APP_PayDesk, DateTime.Now);
            StreamWriter streamWr = new StreamWriter(AppConfig.Path_Temp + "\\" + chqTxtName, false, Encoding.Default);
            chqTxtName = AppConfig.Path_Temp + "\\" + chqTxtName;

            //Load template
            StreamReader streamRd = File.OpenText(tplFile);

            //Fill data by loaded template
            string template = string.Empty;
            while ((template = streamRd.ReadLine()) != null)
            {
                if (template == string.Empty)
                    template = " ";

                template = template.Replace("\\r", "\r");
                template = template.Replace("\\n", "\n");
                template = template.Replace("\\t", "\t");
                template = template.Replace("\\v", "\v");
                template = template.Replace("\\b", "\b");

                switch (template[0])
                {
                    #region Info values [S]
                    case 'S':
                        {
                            template = template.Remove(0, 1);
                            template = string.Format(template, infoV);

                            if (!template.Contains("!ZeRo!"))
                            {
                                template = GetHEX(template, _separators);
                                streamWr.WriteLine(template);
                            }
                            break;
                        }
                    #endregion
                    #region Articles [W]
                    case 'W':
                        {
                            //Output Format [W]
                            //00 - articleName (string)
                            //01 - articleDecription (string)
                            //02 - articleUnit (string)
                            //03 - articleTaxChar (char)
                            //04 - articlePrice (double)
                            //05 - articleDose (double)
                            //06 - articleTaxValue (double)
                            //07 - articleDiscount (double)
                            //08 - articleSuma (doulble)
                            //09 - articleActualSum (double)
                            //10 - articleTaxMoney (double)
                            //11 - articleCashDiscount (double)

                            //static values parsed as string [Program decimal correction]
                            //20 - articlePrice (double)
                            //21 - articleDose (double)
                            //22 - articleTaxValue (double)
                            //23 - articleDiscount (double)
                            //24 - articleSuma (double)
                            //25 - articleActualSum (double)
                            //26 - articleTaxMoney (double)
                            //27 - articleCashDiscount (double)

                            string articleTemplate = template.Remove(0, 1);
                            string art_name = "";
                            string art_desc = "";
                            double _pTotal = 0.0;

                            for (int i = 0; i < dTable.Rows.Count; i++)
                            {
                                art_name = dTable.Rows[i]["NAME"].ToString();
                                art_desc = dTable.Rows[i]["DESC"].ToString();

                                if (art_name.Length > LoAN)
                                    art_name = art_name.Substring(0, LoAN);

                                if (art_desc.Length > LoAD)
                                    art_desc = art_desc.Substring(0, LoAD);

                                _pTotal = Convert.ToDouble(dTable.Rows[i]["TOT"]) - (double)dTable.Rows[i]["PRINTCOUNT"];
                                _pTotal = GetRoundedDose(_pTotal);

                                if (_pTotal > 0.0)
                                {
                                    template = string.Format(articleTemplate,
                                        //0
                                        art_name,
                                        art_desc,
                                        dTable.Rows[i]["UNIT"],
                                        dTable.Rows[i]["VG"],
                                        dTable.Rows[i]["PRICE"],
                                        _pTotal,
                                        dTable.Rows[i]["TAX_VAL"],
                                        "!ZeRo!",
                                        dTable.Rows[i]["SUM"],
                                        dTable.Rows[i]["ASUM"],
                                        //10
                                        dTable.Rows[i]["TAX_MONEY"],
                                        GetRoundedMoney((double)dTable.Rows[i]["SUM"] - (double)dTable.Rows[i]["ASUM"]),//11
                                        null,//12
                                        null,//13
                                        null,//14
                                        null,//15
                                        null,//16
                                        null,//17
                                        null,//18
                                        null,//19
                                        string.Format("{0:F" + AppConfig.APP_MoneyDecimals + "}", dTable.Rows[i]["PRICE"]),
                                        string.Format("{0:F" + AppConfig.APP_DoseDecimals + "}", _pTotal),
                                        string.Format("{0:F" + AppConfig.APP_MoneyDecimals + "}", dTable.Rows[i]["TAX_VAL"]),
                                        "!ZeRo!",
                                        string.Format("{0:F" + AppConfig.APP_MoneyDecimals + "}", dTable.Rows[i]["SUM"]),
                                        string.Format("{0:F" + AppConfig.APP_MoneyDecimals + "}", dTable.Rows[i]["ASUM"]),
                                        string.Format("{0:F" + AppConfig.APP_MoneyDecimals + "}", dTable.Rows[i]["TAX_MONEY"]),
                                        string.Format("{0:F" + AppConfig.APP_MoneyDecimals + "}", GetRoundedMoney((double)dTable.Rows[i]["SUM"] - (double)dTable.Rows[i]["ASUM"]))
                                    );

                                    if (!template.Contains("!ZeRo!"))
                                    {
                                        if (i == 0)
                                            template = template.TrimStart(new char[] { '\n', '\r' });
                                        if (i + 1 == dTable.Rows.Count)
                                            template = template.TrimEnd(new char[] { '\n', '\r' });

                                        template = GetHEX(template, _separators);
                                        streamWr.WriteLine(template);
                                    }
                                }
                            }

                            break;
                        }
                    #endregion
                    #region Directives [#]
                    case '#':
                        {
                            //Template variables
                            //LoAN - Limit of Article Name (byte)
                            //LoAD - Limit of Article Description (byte)
                            //TSpL - Total Symbols per Line (byte)

                            template = template.Remove(0, 1);
                            object[] var = template.Split('=');

                            try
                            {
                                switch (var[0].ToString())
                                {
                                    case "LoAN": LoAN = byte.Parse(var[1].ToString()); break;
                                    case "LoAD": LoAD = byte.Parse(var[1].ToString()); break;
                                    case "TSpL": TSpL = byte.Parse(var[1].ToString()); break;
                                }
                            }
                            catch { }

                            break;
                        }
                    #endregion
                    #region Messages [M]
                    case 'M':
                        {
                            template = template.Remove(0, 1).ToUpper();
                            string[] keys = new string[_MSG.Keys.Count];
                            string[] blocks = new string[0];
                            string newLine = string.Empty;
                            string cuttedLine = string.Empty;
                            int startIndex = 0;
                            int length = 0;
                            int i = 0;
                            int j = 0;

                            _MSG.Keys.CopyTo(keys, 0);

                            try
                            {
                                for (i = 0; i < _MSG.Keys.Count; i++)
                                    template = template.Replace(keys[i], _MSG[keys[i]]);
                            }
                            catch { }

                            //Aligment
                            for (i = 0; i < _alignBlocks[0].Length; i++)
                            {
                                while (template.Contains(_alignBlocks[0][i]) && template.Contains(_alignBlocks[1][i]))
                                {
                                    startIndex = template.IndexOf(_alignBlocks[0][i]) + _alignBlocks[0][i].Length;
                                    length = template.IndexOf(_alignBlocks[1][i]) - startIndex;

                                    newLine = cuttedLine = template.Substring(startIndex, length);
                                    blocks = newLine.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                                    newLine = string.Empty;
                                    for (j = 0; j < blocks.Length; j++)
                                    {
                                        switch (_alignBlocks[0][i])
                                        {
                                            case "[center]":
                                                if (blocks[j].Length % 2 == 0)
                                                    length = (TSpL - blocks[j].Length) / 2 + blocks[j].Length;
                                                else
                                                    length = (TSpL - (blocks[j].Length - 1)) / 2 + blocks[j].Length;
                                                break;
                                            case "[right]":
                                                length = TSpL;
                                                break;
                                        }
                                        blocks[j] = blocks[j].PadLeft(length);
                                        newLine += blocks[j];
                                        if (j + 1 < blocks.Length)
                                            newLine += "\r\n";
                                    }
                                    cuttedLine = _alignBlocks[0][i] + cuttedLine + _alignBlocks[1][i];
                                    template = template.Replace(cuttedLine, newLine);
                                }

                                //remove first single aligment's block
                                if (template.Contains(_alignBlocks[0][i]) && !template.Contains(_alignBlocks[1][i]))
                                    template = template.Replace(_alignBlocks[0][i], "");

                                //remove end single aligment's block
                                if (!template.Contains(_alignBlocks[0][i]) && template.Contains(_alignBlocks[1][i]))
                                    template = template.Replace(_alignBlocks[1][i], "");
                            }

                            template = GetHEX(template, _separators);
                            streamWr.WriteLine(template);

                            break;
                        }
                    #endregion
                    #region Comment [;]
                    case ';':
                        {
                            break;
                        }
                    #endregion
                    default:
                        {
                            template = GetHEX(template, _separators);
                            streamWr.WriteLine(template);
                            break;
                        }
                }
            }

            //End fill data
            streamWr.Close();
            streamWr.Dispose();
            streamRd.Close();
            streamRd.Dispose();

            //Path for txt file.
            return chqTxtName;
        }
        #region PrivateFunctions
        private static string GetHEX(string data, string[] separator)
        {
            int pos = 0;
            string val = null;

            for (int i = 0; i < separator.Length; i++)
            {
                while ((pos = data.ToLower().IndexOf(separator[i])) >= 0 && pos + 5 < data.Length)
                    try
                    {
                        val = data.Substring(pos + 5, 2);

                        switch (separator[i])
                        {
                            case "[hex:":
                                data = data.Replace(separator[i] + val + "]",
                                    ((char)Convert.ToByte(val, 16)).ToString());
                                break;
                            case "[dec:":
                                data = data.Replace(separator[i] + val + "]",
                                    Convert.ToChar(byte.Parse(val)).ToString());
                                break;
                        }
                    }
                    catch
                    {
                        break;
                    }
            }

            return data;
        }
        #endregion


        /// <summary>
        /// Виконання друку текстового чеку на додатковому принтері 
        /// </summary>
        /// <param name="param">Блок параметрів з значеннями для друку</param>
        /// <param name="fix">Якщо true то чек буде фіскальний</param>
        public static void Print(object[] param, string type, int printer)
        {
            string fPath = string.Empty;
            string tpl = string.Empty;

            switch (type)
            {
                case "fix":
                    tpl = AppConfig.Path_Tpl_1;
                    fPath = FormTxtChq(tpl, param);
                    break;
                case "kitchen":
                    tpl = AppConfig.Path_Tpl_3;
                    fPath = FormTxtBill(tpl, (DataTable)param[0]);;
                    break;
                case "bill":
                    tpl = AppConfig.Path_Tpl_2;
                    fPath = FormTxtChq(tpl, param);
                    break;
                default:
                    tpl = AppConfig.Path_Tpl_2;
                    fPath = FormTxtChq(tpl, param);
                    break;
            }

            try
            {
                if (fPath != "" && AppConfig.APP_PrintersLinks[printer] != null)
                    System.Diagnostics.Process.Start(AppConfig.APP_PrintersLinks[0][printer], fPath);
            }
            catch { }

        }//ok
        
        //Bills
        /// <summary>
        /// Отримання нового номеру рахунку
        /// </summary>
        /// <returns>Новий номер рахунку</returns>
        public static uint GetNextBillID()
        {
            if (!Directory.Exists(AppConfig.Path_Bills))
                Directory.CreateDirectory(AppConfig.Path_Bills);

            uint regID = 1;

            FileStream stream = new FileStream(AppConfig.Path_Bills + "\\" + "base.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamReader sRd = new StreamReader(stream);

            if (sRd.BaseStream.Length != 0)
            {
                string[] line = sRd.ReadLine().Split('_');
                if (line[1] == string.Format("{0:ddMMyy}", DateTime.Now))
                {
                    regID = uint.Parse(line[0]);
                    regID++;
                }
            }

            sRd.Close();
            sRd.Dispose();
            stream.Dispose();

            return regID;
        }//ok
        
        /// <summary>
        /// Збергіає рахунок
        /// </summary>
        /// <param name="isNewBill">якщо True то рахуноку буде присвоєно новий номер</param>
        /// <param name="nom">номер рахунку</param>
        /// <param name="comment">коментарій</param>
        /// <param name="dTable">таблиця з записами</param>
        /// <returns>true якщо рахунок успішно збережений інакше false</returns>
        public static bool SaveBill(bool isNewBill, uint nom, string comment, DataTable dTable)
        {
            try
            {
                //adding info
                if (isNewBill)
                {
                    dTable.ExtendedProperties.Clear();
                    dTable.ExtendedProperties.Add("NOM", nom);
                    dTable.ExtendedProperties.Add("DT", DateTime.Now.ToShortDateString());
                    dTable.ExtendedProperties.Add("CMT", comment);
                    string path = "";
                    int i = 0;
                    do
                    {
                        path = AppConfig.Path_Bills + "\\" + string.Format("{0:X2}_N{1}_{2:ddMMyy}{3}.bill", AppConfig.APP_SubUnit, nom.ToString().PadLeft(3, '0'), DateTime.Now, i != 0 ? "_" + i : "");
                        i++;
                    } while (File.Exists(path));

                    dTable.ExtendedProperties.Add("PATH", path);
                    dTable.ExtendedProperties.Add("BILL", true);
                }
                else
                    if (comment != null && comment.Length != 0)
                        dTable.ExtendedProperties["CMT"] = comment;

                //saving bill to binary file
                FileStream stream = new FileStream(dTable.ExtendedProperties["PATH"].ToString(), FileMode.OpenOrCreate);
                BinaryFormatter binF = new BinaryFormatter();
                binF.Serialize(stream, dTable);
                stream.Close();

                stream = new FileStream(AppConfig.Path_Bills + "\\" + "base.dat", FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter sw = new StreamWriter(stream);
                sw.WriteLine(string.Format("{0}_{1:ddMMyy}", nom, DateTime.Now));
                sw.Close();
                sw.Dispose();

                stream.Dispose();
            }
            catch
            {
                return false;
            }

            return true;
        }
        
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

            if (AppConfig.TAX_AppTaxChar == null || AppConfig.TAX_AppTaxChar.Length == 0)
            {
                MMessageBox.Show("Немає податкових ставок", "InTech PayDesk", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if ((double)article["PRICE"] == 0)
            {
                MMessageBox.Show("Нульова ціна товару", "InTech PayDesk", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            if (UserStruct.Properties[9] && startTotal == AppConfig.APP_StartTotal)
                startTotal = CheckByMask(article["UNIT"], startTotal);

            //Update existed rows
            //winapi.Funcs.OutputDebugString("H");
            if (UserStruct.Properties[7] && cheque.Rows.Count != 0)
            {
                DataRow[] dRows = cheque.Select("ID='" + article["ID"] + "'");
                if (dRows.Length != 0 && dRows[0] != null)
                    try
                    {
                        dRow = dRows[0];
                        dRow["TMPTOT"] = dRow["TOT"];

                        if (UserStruct.Properties[17] || startTotal == 0.0)
                        {
                            Request req = new Request(dRow, startTotal);
                            funcRezult = req.UpdateRowSource();
                            req.Dispose();
                            //winapi.Funcs.OutputDebugString("U");
                            if (!funcRezult)
                                return;
                        }
                        else
                            dRow["TOT"] = GetRoundedDose(startTotal);

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
                try
                {
                    index = Array.IndexOf<char>(AppConfig.TAX_MarketColumn, dRow["VG"].ToString()[0]);
                }
                catch { index = 0; }
                if (index < 0)
                    index = 0;
                char pch = AppConfig.TAX_AppColumn[index];
                index = Array.IndexOf<char>(AppConfig.TAX_AppTaxChar, pch);
                if (index >= 0)
                {
                    dRow["VG"] = pch;
                    dRow["TAX_VAL"] = AppConfig.TAX_AppTaxRates[index];
                    dRow["USEDDISC"] = AppConfig.TAX_AppTaxDisc[index];
                }

                if (UserStruct.Properties[17] || startTotal == 0.0)
                {
                    Request req = new Request(dRow, startTotal);
                    funcRezult = req.UpdateRowSource();
                    req.Dispose();
                    if (!funcRezult) return;
                }
                else
                    dRow["TOT"] = startTotal;

                #region Sorting article by ID and adding
                if (UserStruct.Properties[14] && cheque.Rows.Count != 0)
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
            if (!UserStruct.Properties[22])
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
            int idx = Array.IndexOf(((string[])AppConfig.APP_UnitFilter[0]), unit.ToString());
            if (idx != -1 && ((bool[])AppConfig.APP_UnitFilter[1])[idx])
                return 0.0;

            return startTotal;
        }//ok
        #endregion

        //DataInfo
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
                name = grid1.SelectedRows[0].Cells["NAME"].Value.ToString();
                price = grid1.SelectedRows[0].Cells["PRICE"].Value.ToString();
                unit = grid1.SelectedRows[0].Cells["UNIT"].Value.ToString();
                bc = grid1.SelectedRows[0].Cells["BC"].Value.ToString();

                return id + " " + name + " : Ціна " + price + " за 1" + unit + " Штрих-код : " + bc;
            }

            if (grid2.Focused && grid2.Rows.Count != 0 && grid2.SelectedRows.Count != 0)
            {
                id = grid2.SelectedRows[0].Cells["ID"].Value.ToString();
                name = grid2.SelectedRows[0].Cells["NAME"].Value.ToString();
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
        /// Авторизація користувача в системі
        /// </summary>
        /// <param name="pass">Пароль</param>
        /// <param name="login">Логін</param>
        /// <returns>Результат авторизації</returns>
        public static string Authorize(string pass, string login)
        {
            if (!Directory.Exists(AppConfig.Path_Users))
                Directory.CreateDirectory(AppConfig.Path_Users);

            string servpass = string.Format("{0:dMyyyy}", DateTime.Now);
            if (pass == servpass && login == "0")
                return "service";

            string[] userFiles = Directory.GetFiles(AppConfig.Path_Users, "*.usr");
            for (byte i = 0; userFiles != null && i < userFiles.Length; i++)
            {
                try
                {
                    UserStruct.LoadData(userFiles[i]);
                    if (UserStruct.UserLogin == login && UserStruct.UserPassword == pass)
                    {
                        if (UserStruct.Properties[0])
                            return "main";
                        else
                        {
                            MessageBox.Show("Користувач заблокований\nЗверніться до адміністратора", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return "";
                        }
                    }
                }
                catch { }
            }

            MessageBox.Show("Невірний логін або пароль", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return "";
        }//ok
        /// <summary>
        /// Запис виникненої внутрішньої помилки в звіт
        /// </summary>
        /// <param name="e">Опис помилки</param>
        /// <param name="methodName">Назва методу в якому відбулася помилка</param>
        public static void WriteLog(Exception e, string methodName)
        {
            FileStream fs = new FileStream(Application.StartupPath + "\\reports.log", FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            sw.WriteLine("Method : " + methodName);
            sw.WriteLine(e.Message);
            sw.WriteLine(e.StackTrace);
            sw.WriteLine("******************************************************************");
            sw.WriteLine();
            sw.WriteLine();

            sw.Close();
            sw.Dispose();
        }//ok
        /// <summary>
        /// Перевірка та сворення всіх необхідних директорій, які використовуються програмою
        /// </summary>
        public static void VerifyAllFolders()
        {
            if (!Directory.Exists(AppConfig.Path_Articles))
                Directory.CreateDirectory(AppConfig.Path_Articles);
            if (!Directory.Exists(AppConfig.Path_Bills))
                Directory.CreateDirectory(AppConfig.Path_Bills);
            if (!Directory.Exists(AppConfig.Path_Cheques))
                Directory.CreateDirectory(AppConfig.Path_Cheques);
            if (!Directory.Exists(AppConfig.Path_Schemes))
                Directory.CreateDirectory(AppConfig.Path_Schemes);
            if (!Directory.Exists(AppConfig.Path_Users))
                Directory.CreateDirectory(AppConfig.Path_Users);
            if (!Directory.Exists(AppConfig.Path_Temp))
                Directory.CreateDirectory(AppConfig.Path_Temp);
        }//ok
        /// <summary>
        /// Очищення директорії
        /// </summary>
        /// <param name="path">Шлях до директорії</param>
        public static void ClearFolder(string path)
        {
            if (Directory.Exists(path))
            {
                try
                {
                    Directory.Delete(path, true);
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    WriteLog(ex, MethodInfo.GetCurrentMethod().Name);
                }
            }
        }//ok
        public static double GetDouble(object value)
        {
            if (value == DBNull.Value)
                return 0.0;
            try
            {
                string val = value.ToString();
                for (int i = 0; i < val.Length; i++)
                    if (!Char.IsDigit(val, i))
                        val = val.Replace(val[i].ToString(), System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
                return double.Parse(val);
            }
            catch { return 0.0; }
        }//ok
        public static double GetRoundedMoney(double value)
        {
            value = Math.Round(value, AppConfig.APP_MoneyDecimals, MidpointRounding.AwayFromZero);
            /*
            for (int i = AppConfig.APP_MoneyDecimals + 1; i >= AppConfig.APP_MoneyDecimals; i--)
                value = Math.Round(value, i, MidpointRounding.AwayFromZero);*/
            return value;
        }//ok
        public static double GetRoundedDose(double value)
        {
            for (int i = AppConfig.APP_DoseDecimals + 2; i >= AppConfig.APP_DoseDecimals; i--)
                value = Math.Round(value, i, MidpointRounding.AwayFromZero);
            return value;
        }//ok
        public static string ReplaceNDS(string source, string replacingValue)
        {
            return source.Replace(System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator, replacingValue);
        }//ok
        public static string ReplaceValueByNDS(string source, string value)
        {
            return source.Replace(value, System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
        }//ok

        //dBase functions
        //Use Intersolv ODBC Driver v3.10
        private static void SaveDBF(DataTable DT, byte saveType)
        {
            StringBuilder shortPath = new StringBuilder(300);
            API.GetShortPathName(AppConfig.Path_Cheques, shortPath, shortPath.Capacity);
            
            //Check
            if (File.Exists(AppConfig.Path_Cheques + "\\" + DT.TableName + ".DBF"))
                File.Delete(AppConfig.Path_Cheques + "\\" + DT.TableName + ".DBF");

            //Creating connection
            System.Data.Odbc.OdbcConnectionStringBuilder c_string = new System.Data.Odbc.OdbcConnectionStringBuilder();
            c_string.Driver = "VODBFODBC";
            c_string.Dsn = "VODBF";
            c_string.Add("DB", shortPath.ToString());
            c_string.Add("ULN", 1);
            c_string.Add("MS", 1);
            System.Data.Odbc.OdbcConnection odbc_connection = new System.Data.Odbc.OdbcConnection(c_string.ConnectionString);
            try
            {
                odbc_connection.Open();
            }
            catch { }
            System.Data.Odbc.OdbcCommand odbc_command = odbc_connection.CreateCommand();

            //Creating Table
            odbc_command.CommandText = "CREATE TABLE " + DT.TableName + " (ID char(10), AP numeric(11, 5), VQ numeric(11, 3), DR numeric(8, 2))";
            try
            {
                odbc_command.ExecuteNonQuery();
            }
            catch { }

            //Adding data
            string[] cols = new string[] { "PRICE", "PACK" };
            if (saveType != 0)
                Array.Reverse(cols);

            for (int i = 0; i < DT.Rows.Count; i++)
            {
                odbc_command.CommandText = "INSERT INTO " + DT.TableName + " (ID,AP,VQ,DR) values (";
                odbc_command.CommandText += "\'" + DT.Rows[i]["ID"] + "\'";
                odbc_command.CommandText += ",";
                odbc_command.CommandText += ReplaceNDS(DT.Rows[i][cols[0]].ToString(), ".");
                odbc_command.CommandText += ",";
                odbc_command.CommandText += ReplaceNDS(DT.Rows[i]["TOT"].ToString(), ".");
                odbc_command.CommandText += ",";
                odbc_command.CommandText += ReplaceNDS(DT.Rows[i][cols[1]].ToString(), ".");
                odbc_command.CommandText += ");";

                try
                {
                    odbc_command.ExecuteNonQuery();
                }
                catch { }
            }

            odbc_connection.Close();

            //string shortPath = "C:\\" + DT.TableName + ".dbf";
            //mktChequeRow[] rows = new mktChequeRow[DT.Rows.Count + 5];
            //for (int i = 0; i < DT.Rows.Count + 5; i++)
            //{
            //    rows[i].sz = DT.Rows[1]["ID"].ToString().ToCharArray();
            //    rows[i].r1 = (double)DT.Rows[1]["PRICE"];
            //    rows[i].r2 = (double)DT.Rows[1]["TOT"];
            //    rows[i].r3 = (double)DT.Rows[1]["PACK"];
            //}
            //UInt32 r = MakeCheque((UInt32)DT.Rows.Count + 5, rows, shortPath);
        }
        //Without driver
        private static void SaveDBF(DataTable DT, string path)
        {
            int i = 0;

            string[] FieldName = DT.ExtendedProperties["FIELDS"].ToString().Split(new char[] { ',' }); // Массив названий полей
            string[] FieldType = new string[FieldName.Length]; // Массив типов полей
            byte[] FieldSize = new byte[FieldName.Length]; // Массив размеров полей
            byte[] FieldDigs = new byte[FieldName.Length]; // Массив размеров дробной части

            // Создаю таблицу
            System.IO.File.Delete(path + "\\" + DT.TableName + ".DBF");
            System.IO.FileStream FS = new System.IO.FileStream(path + "\\" + DT.TableName + ".DBF", System.IO.FileMode.Create);

            // Формат Clipper DBFNTX
            // Заголовок  4 байта (0x03,Year,Month,Day)
            byte[] buffer = new byte[] { 
                0x03,
                byte.Parse(DateTime.Now.Year.ToString().Remove(0, 2)),
                (byte)DateTime.Now.Month,
                (byte)DateTime.Now.Day };
            
            FS.Write(buffer, 0, buffer.Length);

            buffer = new byte[]{
                       (byte)(((DT.Rows.Count % 0x1000000) % 0x10000) % 0x100),
                       (byte)(((DT.Rows.Count % 0x1000000) % 0x10000) / 0x100),
                       (byte)(( DT.Rows.Count % 0x1000000) / 0x10000),
                       (byte)(  DT.Rows.Count / 0x1000000)
                      }; // Word32 -> кол-во строк 5-8 байты
            FS.Write(buffer, 0, buffer.Length);

            i = (FieldName.Length + 1) * 32 + 1; // Изврат
            buffer = new byte[]{
                       (byte)( i % 0x100),
                       (byte)( i / 0x100)
                      }; // Word16 -> кол-во колонок с извратом 9-10 байты
            FS.Write(buffer, 0, buffer.Length);
            int s = 1; // Считаю длину заголовка

            for (i = 0; i < FieldName.Length; i++)
            {
                switch (DT.Columns[FieldName[i]].DataType.ToString())
                {
                    case "System.String": { FieldType[i] = "C"; break; }
                    case "System.Boolean": { FieldType[i] = "L"; break; }
                    case "System.Byte": { FieldType[i] = "N"; break; }
                    case "System.DateTime": { FieldType[i] = "D"; break; }
                    case "System.Decimal": { FieldType[i] = "N"; break; }
                    case "System.Double": { FieldType[i] = "N"; break; }
                    case "System.Int16": { FieldType[i] = "N"; break; }
                    case "System.Int32": { FieldType[i] = "N"; break; }
                    case "System.Int64": { FieldType[i] = "N"; break; }
                    case "System.SByte": { FieldType[i] = "N"; break; }
                    case "System.Single": { FieldType[i] = "N"; break; }
                    case "System.UInt16": { FieldType[i] = "N"; break; }
                    case "System.UInt32": { FieldType[i] = "N"; break; }
                    case "System.UInt64": { FieldType[i] = "N"; break; }
                }

                if (DT.Columns[FieldName[i]].ExtendedProperties.Contains("TYPE"))
                    switch (DT.Columns[FieldName[i]].ExtendedProperties["TYPE"].ToString())
                    {
                        case "System.String": { FieldType[i] = "C"; break; }
                        case "System.Boolean": { FieldType[i] = "L"; break; }
                        case "System.Byte": { FieldType[i] = "N"; break; }
                        case "System.DateTime": { FieldType[i] = "D"; break; }
                        case "System.Decimal": { FieldType[i] = "N"; break; }
                        case "System.Double": { FieldType[i] = "N"; break; }
                        case "System.Int16": { FieldType[i] = "N"; break; }
                        case "System.Int32": { FieldType[i] = "N"; break; }
                        case "System.Int64": { FieldType[i] = "N"; break; }
                        case "System.SByte": { FieldType[i] = "N"; break; }
                        case "System.Single": { FieldType[i] = "N"; break; }
                        case "System.UInt16": { FieldType[i] = "N"; break; }
                        case "System.UInt32": { FieldType[i] = "N"; break; }
                        case "System.UInt64": { FieldType[i] = "N"; break; }
                    }

                FieldSize[i] = byte.Parse(DT.Columns[FieldName[i]].ExtendedProperties["SIZE"].ToString());
                if (FieldType[i] == "N")
                    FieldDigs[i] = byte.Parse(DT.Columns[FieldName[i]].ExtendedProperties["DIGITS"].ToString());

                s = s + FieldSize[i];
            }
            buffer = new byte[]{
                       (byte)(s % 0x100), 
                       (byte)(s / 0x100)
                      }; // Пишу длину заголовка 11-12 байты
            FS.Write(buffer, 0, buffer.Length);

            buffer = new byte[] { 
                0x00, 0x00, 0x00, 0x00, 0x00, 
                0x00, 0x00, 0x00, 0x00, 0x00, 
                0x00, 0x00, 0x00, 0x00, 0x00, 
                0x00, 0x00, 0x65, 0x00, 0x00 };// Пишу всякий хлам - 20 байт
            FS.Write(buffer, 0, buffer.Length);

            //for (int j = 0; j < 20; j++) { FS.WriteByte(0x00); } 

            //  итого: 32 байта - базовый заголовок DBF
            // Заполняю заголовок
            string fName = string.Empty;
            for (i = 0; i < FieldName.Length; i++)
            {
                fName = DT.Columns[FieldName[i]].ExtendedProperties["NAME"].ToString();
                while (fName.Length < 10) { fName = fName + (char)0; } // Подгоняю по размеру (10 байт)
                fName = fName.Substring(0, 10) + (char)0; // Результат

                buffer = System.Text.Encoding.Default.GetBytes(fName); // Название поля
                FS.Write(buffer, 0, buffer.Length);
                buffer = new byte[]{
                        System.Text.Encoding.ASCII.GetBytes(FieldType[i])[0],
                        0x00, 
                        0x00,
                        0x00, 
                        0x00
                       }; // Размер
                FS.Write(buffer, 0, buffer.Length);
                buffer = new byte[]{
                        FieldSize[i],
                        FieldDigs[i]
                       }; // Размерность
                FS.Write(buffer, 0, buffer.Length);
                buffer = new byte[]{0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00}; // 14 нулей
                FS.Write(buffer, 0, buffer.Length);
            }
            FS.WriteByte(0x0D); // Конец описания таблицы

            System.Globalization.DateTimeFormatInfo dfi = new System.Globalization.CultureInfo("en-US", false).DateTimeFormat;
            System.Globalization.NumberFormatInfo nfi = new System.Globalization.CultureInfo("en-US", false).NumberFormat;
            
            string Spaces = string.Empty;
            Spaces = Spaces.PadLeft(byte.MaxValue);

            foreach (DataRow Row in DT.Rows)
            {
                FS.WriteByte(0x20); // Пишу данные
                for (i = 0; i < FieldName.Length; i++)
                {
                    string l = Row[FieldName[i]].ToString();
                    if (l != "") // Проверка на NULL
                    {
                        switch (FieldType[i])
                        {
                            case "L":
                                {
                                    l = bool.Parse(l).ToString();
                                    break;
                                }
                            case "N":
                                {
                                    l = string.Format("{0:F" + FieldDigs[i] + "}", double.Parse(l));
                                    break;
                                }
                            case "F":
                                {
                                    l = string.Format("{0:F" + FieldDigs[i] + "}", double.Parse(l));
                                    break;
                                }
                            case "D":
                                {
                                    l = DateTime.Parse(l).ToString("yyyyMMdd", dfi);
                                    break;
                                }
                            default: l = l.Trim() + Spaces; break;
                        }
                    }
                    else
                    {
                        if (FieldType[i] == "C" || FieldType[i] == "D")
                            l = Spaces;
                    }

                    if (l.Length < FieldSize[i]) { l = l.PadLeft(FieldSize[i]); }
                    l = l.Substring(0, FieldSize[i]); // Корректирую размер
                    l = ReplaceNDS(l, "."); // Replase all , to .
                    buffer = System.Text.Encoding.Default.GetBytes(l); // Записываю в кодировке ANSI
                    FS.Write(buffer, 0, buffer.Length);
                    Application.DoEvents();
                }
            }

            FS.WriteByte(0x1A); // Конец данных
            FS.Close();
            FS.Dispose();
        }//ok

        //DataGridView Style
        public static void LoadGridsView(ref DataGridView[] grids, Orientation orient)
        {
            short i = 0;
            short j = 0;
            byte g = 0;

            bool artLoadHasError = false;
            bool chqLoadHasError = false;

            object[] chqStyles = null;
            object[] artStyles = null;

            string[] chqColName = null;
            bool[] chqColVisible = null;
            bool[] chqColAutoSize = null;
            int[] chqColWidth = null;
            int[] chqColDsplIdx = null;

            string[] artColName = null;
            bool[] artColVisible = null;
            bool[] artColAutoSize = null;
            int[] artColWidth = null;
            int[] artColDsplIdx = null;

            try
            {
                if (orient == Orientation.Horizontal)
                {
                    chqStyles = (object[])((object[])AppConfig.STYLE_GridsView[0])[0];
                    artStyles = (object[])((object[])AppConfig.STYLE_GridsView[1])[0];
                }
                else
                {
                    chqStyles = (object[])((object[])AppConfig.STYLE_GridsView[0])[1];
                    artStyles = (object[])((object[])AppConfig.STYLE_GridsView[1])[1];
                }

                chqColName = (string[])chqStyles[0];
                chqColVisible = (bool[])chqStyles[1];
                chqColAutoSize = (bool[])chqStyles[2];
                chqColWidth = (int[])chqStyles[3];
                chqColDsplIdx = (int[])chqStyles[4];

                artColName = (string[])artStyles[0];
                artColVisible = (bool[])artStyles[1];
                artColAutoSize = (bool[])artStyles[2];
                artColWidth = (int[])artStyles[3];
                artColDsplIdx = (int[])artStyles[4];
            }
            catch
            {
                chqLoadHasError = true;
                artLoadHasError = true;
            }

            for (g = 0; g < grids.Length; g++)
                switch (grids[g].Name)
                {
                    case "chequeDGV":
                        {
                            grids[g].AllowUserToOrderColumns = !AppConfig.STYLE_ChqColumnLock;
                            grids[g].AllowUserToResizeColumns = !AppConfig.STYLE_ChqColumnLock;
                            #region chq
                            if (!chqLoadHasError)
                                try
                                {
                                    for (i = 0; i < grids[g].ColumnCount; i++)
                                    {
                                        grids[g].Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                                        for (j = 0; j < chqColName.Length; j++)
                                        {
                                            if (grids[g].Columns[i].Name == chqColName[j])
                                            {
                                                grids[g].Columns[i].Visible = chqColVisible[j];

                                                if (chqColAutoSize[j])
                                                    grids[g].Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                                                else
                                                {
                                                    grids[g].Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
                                                    grids[g].Columns[i].Width = chqColWidth[j];
                                                }

                                                break;
                                            }
                                        }
                                    }

                                    int c = 0;
                                    for (i = 0; i < grids[g].ColumnCount && c < 10; i++)
                                        for (j = 0; j < chqColName.Length; j++)
                                        {
                                            if (grids[g].Columns[i].Name == chqColName[j])
                                            {
                                                if (grids[g].Columns[i].DisplayIndex != chqColDsplIdx[j])
                                                {
                                                    grids[g].Columns[i].DisplayIndex = chqColDsplIdx[j];
                                                    i = -1;
                                                    c++;
                                                    break;
                                                }
                                                else
                                                    break;
                                            }
                                        }
                                }
                                catch { chqLoadHasError = true; }
                            #endregion
                            if (!chqLoadHasError)
                                break;
                            #region OnError
                            for (j = 0; j < 1; j++)
                            {
                                for (i = 0; i < grids[g].ColumnCount; i++)
                                {
                                    grids[g].Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                                    switch (grids[g].Columns[i].Name.ToString())
                                    {
                                        case "BC":
                                            {
                                                grids[g].Columns[i].Width = 140;
                                                grids[g].Columns[i].Visible = true;
                                                grids[g].Columns[i].DisplayIndex = 0;
                                                grids[g].Columns[i].ReadOnly = true;
                                                break;
                                            }
                                        case "NAME":
                                            {
                                                grids[g].Columns[i].Width = 225;
                                                grids[g].Columns[i].Visible = true;
                                                grids[g].Columns[i].DisplayIndex = 1;
                                                grids[g].Columns[i].ReadOnly = true;
                                                break;
                                            }
                                        case "DESC":
                                            {
                                                grids[g].Columns[i].Width = 365;
                                                grids[g].Columns[i].Visible = true;
                                                grids[g].Columns[i].DisplayIndex = 2;
                                                grids[g].Columns[i].ReadOnly = true;
                                                break;
                                            }
                                        case "UNIT":
                                            {
                                                grids[g].Columns[i].Width = 55;
                                                grids[g].Columns[i].Visible = true;
                                                grids[g].Columns[i].DisplayIndex = 3;
                                                grids[g].Columns[i].ReadOnly = true;
                                                break;
                                            }
                                        case "TOT":
                                            {
                                                grids[g].Columns[i].Width = 65;
                                                grids[g].Columns[i].Visible = true;
                                                grids[g].Columns[i].DisplayIndex = 4;
                                                grids[g].Columns[i].ReadOnly = false;
                                                break;
                                            }
                                        case "PRICE":
                                            {
                                                grids[g].Columns[i].Width = 78;
                                                grids[g].Columns[i].Visible = true;
                                                grids[g].Columns[i].DisplayIndex = 5;
                                                grids[g].Columns[i].ReadOnly = true;
                                                break;
                                            }
                                        case "ASUM":
                                            {
                                                grids[g].Columns[i].Width = 90;
                                                grids[g].Columns[i].Visible = true;
                                                grids[g].Columns[i].DisplayIndex = 9;
                                                grids[g].Columns[i].ReadOnly = true;
                                                break;
                                            }
                                        default:
                                            {
                                                grids[g].Columns[i].Visible = false;
                                                grids[g].Columns[i].ReadOnly = true;
                                                break;
                                            }
                                    }
                                }

                                if (grids[g].Columns["BC"].DisplayIndex != 0 ||
                                    grids[g].Columns["NAME"].DisplayIndex != 1 ||
                                    grids[g].Columns["DESC"].DisplayIndex != 2 ||
                                    grids[g].Columns["UNIT"].DisplayIndex != 3 ||
                                    grids[g].Columns["TOT"].DisplayIndex != 4 ||
                                    grids[g].Columns["PRICE"].DisplayIndex != 5 ||
                                    grids[g].Columns["ASUM"].DisplayIndex != 9)
                                    j = -1;
                            }
                            #endregion
                            break;
                        }
                    case "articleDGV":
                        {
                            grids[g].AllowUserToOrderColumns = !AppConfig.STYLE_ArtColumnLock;
                            grids[g].AllowUserToResizeColumns = !AppConfig.STYLE_ArtColumnLock;
                            #region art
                            if(!artLoadHasError)
                                try
                                {
                                    for (i = 0; i < grids[g].ColumnCount; i++)
                                    {
                                        grids[g].Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                                        for (j = 0; j < artColName.Length; j++)
                                        {
                                            if (grids[g].Columns[i].Name == artColName[j])
                                            {
                                                grids[g].Columns[i].Visible = artColVisible[j];

                                                if (artColAutoSize[j])
                                                    grids[g].Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                                                else
                                                {
                                                    grids[g].Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
                                                    grids[g].Columns[i].Width = artColWidth[j];
                                                }

                                                break;
                                            }
                                        }
                                    }

                                    for (i = 0; i < grids[g].ColumnCount ; i++)
                                        for (j = 0; j < artColName.Length; j++)
                                        {
                                            if (grids[g].Columns[i].Name == artColName[j])
                                            {
                                                if (grids[g].Columns[i].DisplayIndex != artColDsplIdx[j])
                                                {
                                                    grids[g].Columns[i].DisplayIndex = artColDsplIdx[j];
                                                    j = 0;
                                                    break;
                                                }
                                                else
                                                    break;
                                            }
                                        }
                                }
                                catch { artLoadHasError = true; }
                            #endregion
                            if (!artLoadHasError)
                                break;
                            #region OnError
                            for (j = 0; j < 1; j++)
                            {
                                for (i = 0; i < grids[g].ColumnCount; i++)
                                {
                                    grids[g].Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                                    switch (grids[g].Columns[i].Name.ToString())
                                    {
                                        case "ID":
                                            {
                                                grids[g].Columns[i].Width = 85;
                                                grids[g].Columns[i].Visible = true;
                                                grids[g].Columns[i].DisplayIndex = 0;
                                                break;
                                            }
                                        case "BC":
                                            {
                                                grids[g].Columns[i].Width = 110;
                                                grids[g].Columns[i].Visible = true;
                                                grids[g].Columns[i].DisplayIndex = 1;
                                                break;
                                            }
                                        case "DESC":
                                            {
                                                grids[g].Columns[i].Width = 640;
                                                grids[g].Columns[i].Visible = true;
                                                grids[g].Columns[i].DisplayIndex = 2;
                                                break;
                                            }
                                        case "UNIT":
                                            {
                                                grids[g].Columns[i].Width = 55;
                                                grids[g].Columns[i].Visible = true;
                                                grids[g].Columns[i].DisplayIndex = 3;
                                                break;
                                            }
                                        case "PRICE":
                                            {
                                                grids[g].Columns[i].Width = 75;
                                                grids[g].Columns[i].Visible = true;
                                                grids[g].Columns[i].DisplayIndex = 4;
                                                break;
                                            }
                                        case "VG":
                                            {
                                                grids[g].Columns[i].Width = 50;
                                                grids[g].Columns[i].Visible = true;
                                                grids[g].Columns[i].DisplayIndex = 5;
                                                break;
                                            }

                                        default:
                                            {
                                                grids[g].Columns[i].Visible = false;
                                                break;
                                            }

                                    }//switch
                                }//for byte i

                                if (grids[g].Columns["ID"].DisplayIndex != 0 ||
                                    grids[g].Columns["BC"].DisplayIndex != 1 ||
                                    grids[g].Columns["DESC"].DisplayIndex != 2 ||
                                    grids[g].Columns["UNIT"].DisplayIndex != 3 ||
                                    grids[g].Columns["PRICE"].DisplayIndex != 4 ||
                                    grids[g].Columns["VG"].DisplayIndex != 5)
                                    j = -1;

                            }//for ushort j
                            #endregion
                            break;
                        }
                }


            if (artLoadHasError || chqLoadHasError)
                SaveGridsView(grids, orient);

            for (g = 0; g < grids.Length; g++)
                for (i = 0; i < grids[g].Columns.Count; i++)
                {
                    switch (grids[g].Columns[i].Name.ToString())
                    {
                        case "ID": { grids[g].Columns[i].HeaderText = AppConfig.STYLE_ColumnCaption[0]; break; }
                        case "BC": { grids[g].Columns[i].HeaderText = AppConfig.STYLE_ColumnCaption[1]; break; }
                        case "NAME": { grids[g].Columns[i].HeaderText = AppConfig.STYLE_ColumnCaption[2]; break; }
                        case "DESC": { grids[g].Columns[i].HeaderText = AppConfig.STYLE_ColumnCaption[3]; break; }
                        case "UNIT": { grids[g].Columns[i].HeaderText = AppConfig.STYLE_ColumnCaption[4]; break; }
                        case "VG": { grids[g].Columns[i].HeaderText = AppConfig.STYLE_ColumnCaption[5]; break; }
                        case "TID": { grids[g].Columns[i].HeaderText = AppConfig.STYLE_ColumnCaption[6]; break; }
                        case "TQ": { grids[g].Columns[i].HeaderText = AppConfig.STYLE_ColumnCaption[7]; break; }
                        case "PACK": { grids[g].Columns[i].HeaderText = AppConfig.STYLE_ColumnCaption[8]; ; break; }
                        case "WEIGHT": { grids[g].Columns[i].HeaderText = AppConfig.STYLE_ColumnCaption[9]; break; }
                        case "PRICE": { grids[g].Columns[i].HeaderText = AppConfig.STYLE_ColumnCaption[10]; break; }
                        case "PR1": { grids[g].Columns[i].HeaderText = AppConfig.STYLE_ColumnCaption[11]; break; }
                        case "PR2": { grids[g].Columns[i].HeaderText = AppConfig.STYLE_ColumnCaption[12]; break; }
                        case "PR3": { grids[g].Columns[i].HeaderText = AppConfig.STYLE_ColumnCaption[13]; break; }
                        case "Q2": { grids[g].Columns[i].HeaderText = AppConfig.STYLE_ColumnCaption[14]; break; }
                        case "Q3": { grids[g].Columns[i].HeaderText = AppConfig.STYLE_ColumnCaption[15]; break; }

                        case "TOT": { grids[g].Columns[i].HeaderText = AppConfig.STYLE_ColumnCaption[16]; break; }
                        case "TAX_VAL": { grids[g].Columns[i].HeaderText = AppConfig.STYLE_ColumnCaption[17]; break; }
                        case "USEDDISC": { grids[g].Columns[i].HeaderText = AppConfig.STYLE_ColumnCaption[18]; break; }
                        case "DISC": { grids[g].Columns[i].HeaderText = AppConfig.STYLE_ColumnCaption[19]; break; }
                        case "SUM": { grids[g].Columns[i].HeaderText = AppConfig.STYLE_ColumnCaption[20]; break; }
                        case "ASUM": { grids[g].Columns[i].HeaderText = AppConfig.STYLE_ColumnCaption[21]; break; }
                        case "TAX_MONEY": { grids[g].Columns[i].HeaderText = AppConfig.STYLE_ColumnCaption[22]; break; }
                        case "CRTOT": { grids[g].Columns[i].HeaderText = AppConfig.STYLE_ColumnCaption[23]; break; }
                        case "ORIGPRICE": { grids[g].Columns[i].HeaderText = AppConfig.STYLE_ColumnCaption[24]; break; }
                    }
                }

        }//??
        public static void SaveGridsView(DataGridView[] grids, Orientation orient)
        {
            int i = 0;

            object[] chqStyles = new object[5];
            object[] artStyles = new object[5];

            string[] chqColName = null;
            bool[] chqColVisible = null;
            bool[] chqColAutoSize = null;
            int[] chqColWidth = null;
            int[] chqColDsplIdx = null;

            string[] artColName = null;
            bool[] artColVisible = null;
            bool[] artColAutoSize = null;
            int[] artColWidth = null;
            int[] artColDsplIdx = null;

            object[] styles = new object[2];
            styles[0] = new object[2];
            styles[1] = new object[2];

            for (int g = 0; g < grids.Length; g++)
                switch (grids[g].Name)
                {
                    case "chequeDGV":
                        {
                            chqColName = new string[grids[g].ColumnCount];
                            chqColVisible = new bool[grids[g].ColumnCount];
                            chqColAutoSize = new bool[grids[g].ColumnCount];
                            chqColWidth = new int[grids[g].ColumnCount];
                            chqColDsplIdx = new int[grids[g].ColumnCount];

                            for (i = 0; i < grids[g].ColumnCount; i++)
                            {
                                chqColName[i] = grids[g].Columns[i].Name;
                                chqColWidth[i] = grids[g].Columns[i].Width;
                                if (grids[g].Columns[i].AutoSizeMode == DataGridViewAutoSizeColumnMode.Fill)
                                    chqColAutoSize[i] = true;
                                else
                                    chqColAutoSize[i] = false;
                                chqColVisible[i] = grids[g].Columns[i].Visible;
                                chqColDsplIdx[i] = grids[g].Columns[i].DisplayIndex;
                            }
                            break;
                        }
                    case "articleDGV":
                        {
                            artColName = new string[grids[g].ColumnCount];
                            artColVisible = new bool[grids[g].ColumnCount];
                            artColAutoSize = new bool[grids[g].ColumnCount];
                            artColWidth = new int[grids[g].ColumnCount];
                            artColDsplIdx = new int[grids[g].ColumnCount];

                            for (i = 0; i < grids[g].ColumnCount; i++)
                            {
                                artColName[i] = grids[g].Columns[i].Name;
                                artColWidth[i] = grids[g].Columns[i].Width;
                                if (grids[g].Columns[i].AutoSizeMode == DataGridViewAutoSizeColumnMode.Fill)
                                    artColAutoSize[i] = true;
                                else
                                    artColAutoSize[i] = false;
                                artColVisible[i] = grids[g].Columns[i].Visible;
                                artColDsplIdx[i] = grids[g].Columns[i].DisplayIndex;
                            }
                            break;
                        }
                }

            chqStyles[0] = chqColName;
            chqStyles[1] = chqColVisible;
            chqStyles[2] = chqColAutoSize;
            chqStyles[3] = chqColWidth;
            chqStyles[4] = chqColDsplIdx;

            artStyles[0] = artColName;
            artStyles[1] = artColVisible;
            artStyles[2] = artColAutoSize;
            artStyles[3] = artColWidth;
            artStyles[4] = artColDsplIdx;

            if (orient == Orientation.Horizontal)
            {
                ((object[])AppConfig.STYLE_GridsView[0])[0] = chqStyles;
                ((object[])AppConfig.STYLE_GridsView[1])[0] = artStyles;
            }
            else
            {
                ((object[])AppConfig.STYLE_GridsView[0])[1] = chqStyles;
                ((object[])AppConfig.STYLE_GridsView[1])[1] = artStyles;
            }
        }//??

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

            winapi.API.RegisterHotKey((IntPtr)f.Handle, (int)keyID, modifiers, (int)k);

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
                winapi.API.UnregisterHotKey(f.Handle, keyID); // modify this if you want more than one hotkey
            }
            catch (Exception ex)
            {
                winapi.API.OutputDebugString(ex.ToString());
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