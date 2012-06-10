using System;
using System.Collections.Generic;
using System.Text;
using mdcore.Config;
using winapi;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Data;
using mdcore.Lib;
using Microsoft.VisualBasic.FileIO;

namespace mdcore.Lib
{
    public static class DataWorkSource
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
                    if (!AppConfig.Content_Articles_KeepDataAfterImport)
                        Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(localLoadedTempFiles[i], UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
                }
                else
                {
                    if (!onlyUpdate && winapi.WApi.PathFileExists(artFiles[i]))
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
                typeof(double)};

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

                    dRow["TQ"] = CoreLib.GetDouble(line.Substring(146, 12).Trim());//tq
                    dRow["PACK"] = CoreLib.GetDouble(line.Substring(158, 18).Trim());//pack
                    dRow["WEIGHT"] = CoreLib.GetDouble(line.Substring(176, 18).Trim());//weight
                    dRow["PRICE"] = CoreLib.GetDouble(line.Substring(194, 12).Trim());//price
                    dRow["PR1"] = CoreLib.GetDouble(line.Substring(206, 12).Trim());//pr1
                    dRow["PR2"] = CoreLib.GetDouble(line.Substring(218, 12).Trim());//pr2
                    dRow["PR3"] = CoreLib.GetDouble(line.Substring(230, 12).Trim());//pr3
                    dRow["Q2"] = CoreLib.GetDouble(line.Substring(242, 12).Trim());//q2
                    dRow["Q3"] = CoreLib.GetDouble(line.Substring(254, 10).Trim());//q3

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
                    dRow["CDISC"] = CoreLib.GetDouble(line.Substring(30, 6).Trim());//cdisc
                    dTable.Rows.Add(dRow);
                    dRow = dTable.NewRow();
                }
                sr.Close();
                sr.Dispose();
            }
        }//ok 
        #endregion

    }
}
