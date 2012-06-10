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
using mdcore.Config;
using mdcore.Components;
using mdcore.Components.UI;
using System.Collections;

namespace mdcore.Lib
{
    /// <summary>
    /// Main methods class.
    /// Contained based methods.
    /// </summary>
    public class CoreLib
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

            winapi.WApi.OutputDebugString("CheckUpdate_begin");

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
                    winapi.WApi.OutputDebugString("check for index " + i);
                    if (AsyncFunc.FileExists(exFiles[i], AppConfig.APP_RefreshTimeout))
                    {
                        if (!AsyncFunc.FileExists(localFiles[i], AppConfig.APP_RefreshTimeout))
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
                        if (i == 0)
                        {
                            winapi.WApi.OutputDebugString("Art file is unvaliable _ CheckUpdate_end");
                            if (!AsyncFunc.FileExists(AppConfig.Path_Exchnage, AppConfig.APP_RefreshTimeout))
                                load[0] = "lanError";
                            //return load;
                        }
                }
                catch { ; }

            winapi.WApi.OutputDebugString("CheckUpdate_end");
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
                        winapi.WApi.OutputDebugString("copy start");
                        fOK = winapi.WApi.CopyFile(exchangeFiles[i], tmpFiles[i], false);
                        winapi.WApi.OutputDebugString("copy end");
                        if (fOK && winapi.WApi.PathFileExists(tmpFiles[i]))
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
            winapi.WApi.OutputDebugString("--- LoadData_begin");

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

                    winapi.WApi.OutputDebugString("EXCHAGE LoadingFromNewSource_begin");
                    dTables[i] = crFunc[i].Invoke();
                    rdFunc[i].Invoke(localLoadedTempFiles[i], ref dTables[i]);
                    fs = new FileStream(artFiles[i], FileMode.Create);
                    binF.Serialize(fs, dTables[i]);
                    //dTables[i].TableName = Path.GetFileNameWithoutExtension(localLoadedTempFiles[i]);
                    //dTables[i].WriteXml(fs);
                    fs.Close();
                    fs.Dispose();
                    exchangeFldUsed = true;
                    if (!AppConfig.Content_Articles_KeepDataAfterImport)
                        Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(localLoadedTempFiles[i], UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
                    winapi.WApi.OutputDebugString("EXCHAGE LoadingFromNewSource_end");
                }
                else
                {
                    if (!onlyUpdate && winapi.WApi.PathFileExists(artFiles[i]))
                    {
                        winapi.WApi.OutputDebugString("LOCAL LoadingFromLocalCopy_begin");
                        fs = new FileStream(artFiles[i], FileMode.Open, FileAccess.Read);
                        //dTables[i].ReadXml(fs);
                        dTables[i] = (DataTable)binF.Deserialize(fs);
                        if (!new sgmode.ClassMode().FullLoader())
                        {
                            DataTable tDt = dTables[i].Clone();
                            for (int jp = 0; jp < 10; jp++)
                            {
                                if (dTables[i].Rows.Count <= jp) break;
                                tDt.Rows.Add(dTables[i].Rows[jp].ItemArray);
                            }
                            dTables[i] = tDt.Copy();
                        }
                        fs.Close();
                        fs.Dispose();
                        winapi.WApi.OutputDebugString("LOCAL LoadingFromLocalCopy_end");
                    }
                }

            if (fs != null)
                fs.Dispose();

            object[] rez = new object[] { dTables, exchangeFldUsed };


            winapi.WApi.OutputDebugString("--- LoadData_end");
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

            if (AppConfig.STYLE_CARDColumnName.Length != cTypes.Length)
                AppConfig.STYLE_CARDColumnName = new string[] { "CBC", "CID", "CDISC", "CPRICENO" };

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
                //StreamReader sr = null;
                Stream sr = null;
                do
                {
                    try
                    {
                        //sr = new StreamReader(path, Encoding.Default);
                        sr = new FileStream(path, FileMode.Open);
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

                BufferedStream buff = new BufferedStream(sr, 131072);
                StreamReader reader = new StreamReader(buff, Encoding.Default);
                StringReader strRd = new StringReader(reader.ReadToEnd());
                sr.Close();
                sr.Dispose();
                string line = string.Empty;
                DataRow dRow = dTable.NewRow();
                int sc = 0;
                bool isfull = new sgmode.ClassMode().FullLoader();
                dTable.Rows.Clear();

                winapi.WApi.OutputDebugString("ReadArtSDF_begin");
                while ((line = strRd.ReadLine()) != null)
                {
                    if (line == string.Empty)
                        continue;
                    /*
                    if (!isfull && sc >= 10)
                        break;

                    if (isfull && (sc + 1) % 100 == 0)
                        isfull = new sgmode.ClassMode().FullLoader();
                    */
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

                    //sc++;
                }
                winapi.WApi.OutputDebugString("ReadArtSDF_end");


            }
        }//ok
        private static void ReadAltSDF(string path, ref DataTable dTable)
        {
            byte err_cnt = 0;

            if (File.Exists(path))
            {
                Stream sr = null;
                do
                {
                    try
                    {
                        sr = new FileStream(path, FileMode.Open);
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

                BufferedStream buff = new BufferedStream(sr, 131072);
                StreamReader reader = new StreamReader(buff, Encoding.Default);
                StringReader strRd = new StringReader(reader.ReadToEnd());
                sr.Close();
                sr.Dispose();
                string line = string.Empty;
                DataRow dRow = dTable.NewRow();
                int testdsc = 0;
                bool test1 = new sgmode.ClassMode().FullLoader();
                dTable.Rows.Clear();


                winapi.WApi.OutputDebugString("ReadAltSDF_begin");
                while ((line = strRd.ReadLine()) != null)
                {
                    if (line == string.Empty)
                        continue;
                    /*
                    if (!test1 && testdsc >= 10)
                        break;

                    if (test1 && (testdsc + 1) % 100 == 0)
                        test1 = new sgmode.ClassMode().FullLoader();
                    */
                    //dRow["C"] = index++;

                    dRow["ABC"] = line.Substring(0, 20).Trim();//abc
                    dRow["AID"] = line.Substring(20, 10).Trim();//aid
                    dTable.Rows.Add(dRow);
                    dRow = dTable.NewRow();

                    //testdsc++;
                }
                winapi.WApi.OutputDebugString("ReadAltSDF_end");
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

                string line = string.Empty;
                DataRow dRow = dTable.NewRow();
                //long index = 0;
                int sc = 0;
                bool isfull = new sgmode.ClassMode().FullLoader();

                while ((line = sr.ReadLine()) != null)
                {
                    //dRow["C"] = index++;

                    if (line == string.Empty)
                        continue;


                    if (!isfull && sc >= 10)
                        break;

                    if (isfull && (sc + 1) % 100 == 0)
                        isfull = new sgmode.ClassMode().FullLoader();

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

                    sc++;
                }
                sr.Close();
                sr.Dispose();
            }
        }//ok 
        #endregion

        //Rules
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
        /// <summary>
        /// Get information of non fiscalized orders
        /// </summary>
        /// <param name="suma">Current summa of order</param>
        /// <param name="chqNom">Current order's number</param>
        /// <returns>Return information of all non fiscalized orders.</returns>
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
            values[7] = UserConfig.UserLogin;
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

            if (UserConfig.Properties[9] && startTotal == AppConfig.APP_StartTotal)
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
            int idx = Array.IndexOf(((string[])AppConfig.APP_UnitFilter[0]), unit.ToString());
            if (idx != -1 && ((bool[])AppConfig.APP_UnitFilter[1])[idx])
                return 0.0;

            return startTotal;
        }//ok
        #endregion

        //DataInfo
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
                    UserConfig.LoadData(userFiles[i]);
                    if (UserConfig.UserLogin == login && UserConfig.UserPassword == pass)
                    {
                        if (UserConfig.Properties[0])
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
            string reportName = string.Format("{0}\\report_{1}.log", AppConfig.Path_Reports, DateTime.Now.ToShortDateString());
            using (FileStream fs = new FileStream(reportName, FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    try
                    {
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
                    }
                    catch (Exception ex) { winapi.WApi.OutputDebugString(ex.Message); }

                    sw.Close();
                }

                fs.Close();
            }
            
        }//ok
        /// <summary>
        /// Перевірка та сворення всіх необхідних директорій, які використовуються програмою
        /// </summary>
        public static void VerifyAllFolders()
        {
            FolderBrowserDialog fBrDlg = new FolderBrowserDialog();
            fBrDlg.ShowNewFolderButton = true;
            bool pathSelected = false;
            bool saveSettings = false;
            DialogResult dRez = DialogResult.None;
            int idx = 0;
            List<string> dirList = new List<string>();
            dirList.Add(AppConfig.Path_Articles); // 0
            dirList.Add(AppConfig.Path_Bills); // 1
            dirList.Add(AppConfig.Path_Cheques); // 2
            dirList.Add(AppConfig.Path_Schemes); // 3
            dirList.Add(AppConfig.Path_Users); // 4
            dirList.Add(AppConfig.Path_Temp); // 5
            dirList.Add(AppConfig.Path_Reports); // 6
            string[] newPath = new string[dirList.Capacity];

            foreach (string dirPath in dirList)
            {
                if (!Directory.Exists(dirPath))
                {
                    try
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    catch
                    {
                        fBrDlg.Description = "Вибрі шляху для папки товарів";
                        dRez = MMessageBox.Show("Не вдалося отримати доступ до папки: " + dirPath + "\r\nЩоб встановити новий шлях до папки натисніть ТАК.\r\nЩоб закрити програму натисніть НІ", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                        pathSelected = false;
                        if (dRez == DialogResult.Yes)
                        {
                            do
                            {
                                dRez = DialogResult.None;
                                fBrDlg.ShowDialog();
                                if (fBrDlg.SelectedPath == string.Empty)
                                {
                                    dRez = MMessageBox.Show("Скасувати вибір нового шляху та закрити програму?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (dRez == DialogResult.Yes)
                                        Environment.Exit(0);
                                }
                                else
                                {
                                    newPath[idx] = fBrDlg.SelectedPath;
                                    pathSelected = true;
                                    saveSettings = true;
                                }
                            } while (!pathSelected);
                        }
                        if (dRez == DialogResult.No)
                            Environment.Exit(0);
                    }
                }
                idx++;
            }

            fBrDlg.Dispose();
            idx = 0;
            foreach (string nP in newPath)
            {
                if (nP != null && nP != string.Empty)
                {
                    switch (idx)
                    {
                        case 0: { AppConfig.Path_Articles = nP; break; }
                        case 1: { AppConfig.Path_Bills = nP; break; }
                        case 2: { AppConfig.Path_Cheques = nP; break; }
                        case 3: { AppConfig.Path_Schemes = nP; break; }
                        case 4: { AppConfig.Path_Users = nP; break; }
                        case 5: { AppConfig.Path_Temp = nP; break; }
                        case 6: { AppConfig.Path_Reports = nP; break; }
                        case 7: { AppConfig.Path_Articles = nP; break; }
                    }
                }
                idx++;
            }

            if (saveSettings)
                AppConfig.SaveData();

            /*

          if (!Directory.Exists(AppConfig.Path_Articles))
          {
              try
              {
                  Directory.CreateDirectory(AppConfig.Path_Articles);
              }
              catch
              {
                  fBrDlg.Description = "Вибрі шляху для папки товарів";
                  dRez = MMessageBox.Show("Не вдалося отримати доступ до папки: " + AppConfig.Path_Articles + "\r\nЩоб встановити новий шлях до папки натисніть ТАК.", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                  pathSelected = false;
                  if (dRez == DialogResult.Yes)
                  {
                      do
                      {
                          fBrDlg.ShowDialog();
                          if (fBrDlg.SelectedPath == string.Empty)
                          {
                              dRez = MMessageBox.Show("Скасувати вибір нового шляху та закрити програму ", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                              if (dRez == DialogResult.Yes)
                                  Application.Exit(new CancelEventArgs(true));
                          }
                          else
                              pathSelected = true;
                      } while (pathSelected);
                  }
              }
          }
        if (!Directory.Exists(AppConfig.Path_Bills))
        {
            Directory.CreateDirectory(AppConfig.Path_Bills);
        }
        if (!Directory.Exists(AppConfig.Path_Cheques))
        {
            Directory.CreateDirectory(AppConfig.Path_Cheques);
        }
        if (!Directory.Exists(AppConfig.Path_Schemes))
        {
            Directory.CreateDirectory(AppConfig.Path_Schemes);
        }
        if (!Directory.Exists(AppConfig.Path_Users))
        {
            Directory.CreateDirectory(AppConfig.Path_Users);
        }
        if (!Directory.Exists(AppConfig.Path_Temp))
        {
            Directory.CreateDirectory(AppConfig.Path_Temp);
        }
        if (!Directory.Exists(AppConfig.Path_Reports))
        {
            Directory.CreateDirectory(AppConfig.Path_Reports);
        }*/
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

        //dBase functions
        //Use Intersolv ODBC Driver v3.10
        private static void SaveDBF(DataTable DT, byte saveType)
        {
            StringBuilder shortPath = new StringBuilder(300);
            winapi.WApi.GetShortPathName(AppConfig.Path_Cheques, shortPath, shortPath.Capacity);
            
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

            winapi.WApi.RegisterHotKey((IntPtr)f.Handle, (int)keyID, modifiers, (int)k);

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
                winapi.WApi.UnregisterHotKey(f.Handle, keyID); // modify this if you want more than one hotkey
            }
            catch (Exception ex)
            {
                winapi.WApi.OutputDebugString(ex.ToString());
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
            HK_Ctrl = 0x1F,
            HK_Return = 0x20,
            HK_LineFeed = 0x21
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