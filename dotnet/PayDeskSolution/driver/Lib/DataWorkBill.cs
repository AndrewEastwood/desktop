using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using driver.Config;
using driver.Lib;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data;
using System.Windows.Forms;
using driver.Components.UI;
using Microsoft.VisualBasic.FileIO;
using driver.Common;
using System.Collections;

namespace driver.Lib
{
    public static class DataWorkBill
    {
        public static string GetNextBillID()
        {
            if (!Directory.Exists(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Bills))
                Directory.CreateDirectory(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Bills);

            uint regID = 1;

            FileStream stream = new FileStream(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Bills + "\\" + "base.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamReader sRd = new StreamReader(stream);

            if (sRd.BaseStream.Length != 0)
            {
                string[] line = sRd.ReadLine().Split('_');
                if (line[1] == string.Format("{0:ddMMyy}", DateTime.Now))
                {
                    // if latest bill was copy we should remove suffix and get clear bill no.
                    if (line[0].Contains("-"))
                        regID = uint.Parse(line[0].Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                    else
                        regID = uint.Parse(line[0]);
                    regID++;
                }
            }

            sRd.Close();
            sRd.Dispose();
            stream.Dispose();

            return regID.ToString();
        }//ok

        public static bool BillUpdatePrintedCount(DataTable dtBill)
        {
            bool success = true;
            for (int i = 0; i < dtBill.Rows.Count; i++)
                try
                {
                    dtBill.Rows[i]["PRINTCOUNT"] = Convert.ToDouble(dtBill.Rows[i]["TOT"]);
                }
                catch(Exception e) {
                    CoreLib.WriteLog(e, "driver.Lib.BillUpdatePrintedCount(DataTable dtBill); can't update PRINTCOUNT field at row ["+i+"]\r\n" + DataWorkShared.DumpDataTableRow(dtBill.Rows[i]));
                    success = false; 
                }
            return success;
        }
        public static bool BillUpdatePrintedCount(DataTable dtBill, bool saveResult)
        {
            bool success = true;
            try
            {
                BillUpdatePrintedCount(dtBill);
                if (saveResult)
                    SaveBillToFile(dtBill);
            }
            catch (Exception e)
            {
                CoreLib.WriteLog(e, "driver.Lib.BillUpdatePrintedCount(DataTable dtBill, bool saveResult);");
                success = false;
            }
            return success;
        }

        public static string BillNewUID(string newBillNumber)
        {
            return components.Components.SecureRuntime.Com_SecureRuntime.GetMD5Hash(DateTime.Now.ToString() + "_" + newBillNumber);
        }

        public static bool SaveBill(bool isNewBill, string nom, string comment, ref DataTable dTable/*, bool isLocked, string fixChequeNo*/)
        {
            Dictionary<string, object> billInfo = DataWorkShared.GetStandartBillInfoStructure();
            bool fRez = false;
            string path = string.Empty;
            int i = 0;

            /* ## TEST MODE
            int iNom = int.Parse(nom);
            for (int loop = 0; loop < 10000; loop++)
            {
            */
                try
                {
                    //adding info
                    if (isNewBill)
                    {
                        i = 0;
                        do
                        {
                            path = string.Format("{0:X2}_N{1}{3}_{2:ddMMyy}.bill", driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit, nom.PadLeft(5, '0'), DateTime.Now, i != 0 ? "-" + i : "");
                            nom = string.Format("{0}{1}", nom.PadLeft(5, '0'), i != 0 ? "-" + i : "");
                            i++;
                        } while (File.Exists(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Bills + "\\" + path));


                        //dTable.ExtendedProperties.Clear();

                        billInfo["DATETIME"] = DateTime.Now.ToShortDateString();
                        billInfo["COMMENT"] = comment;
                        billInfo["PATH"] = path;
                        billInfo["OID"] = BillNewUID(nom);
                        billInfo["BILL_NO"] = nom;
                        billInfo[driver.Common.CoreConst.OWNER_NO] = string.Empty;
                        billInfo["IS_LOCKED"] = false;

                        if (dTable.ExtendedProperties.ContainsKey("BILL"))
                            dTable.ExtendedProperties["BILL"] = billInfo;
                        else
                            dTable.ExtendedProperties.Add("BILL", billInfo);

                        /*
                        dTable.ExtendedProperties.Add("OID", BillNewUID(nom));
                        dTable.ExtendedProperties.Add("NOM", nom);
                        dTable.ExtendedProperties.Add("DT", DateTime.Now.ToShortDateString());
                        dTable.ExtendedProperties.Add("CMT", comment);
                        dTable.ExtendedProperties.Add("LOCK", isLocked);
                        dTable.ExtendedProperties.Add("FXNO", fixChequeNo);
                        dTable.ExtendedProperties.Add("DISC", new object[2][]);
                        dTable.ExtendedProperties.Add("PATH", path);
                        dTable.ExtendedProperties.Add("BILL", DataWorkShared.GetStandartBillInfoStructure());*/
                    }
                    else
                    {
                        billInfo = DataWorkShared.GetBillInfo(dTable);// (Dictionary<string, object>)dTable.ExtendedProperties["BILL"];
                        if (comment != null && comment.Length != 0)
                        {
                            DataWorkShared.SetBillProperty(dTable, driver.Common.CoreConst.COMMENT, comment);
                            //billInfo["COMMENT"] = comment;
                        }
                    }

                    //saving bill to binary file
                    //FileStream stream = new FileStream(AppConfig.Path_Bills + "\\" + billInfo["PATH"].ToString(), FileMode.OpenOrCreate);
                    //BinaryFormatter binF = new BinaryFormatter();
                    //binF.TypeFormat = System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways;
                    //object[] bill = new object[2] { dTable, dTable.ExtendedProperties };
                    //binF.Serialize(stream, DataWorkShared.GetDataObject(dTable));
                    //stream.Close();
                    //FileStream stream2 = new FileStream(AppConfig.Path_Bills + "\\" + billInfo["PATH"].ToString(), FileMode.Open);
                    //object[] billdemo = (object[])binF.Deserialize(stream2);
                    //stream2.Close();
                    SaveBillToFile(dTable);
                    if (isNewBill)
                    {
                        //FileStream stream = new FileStream(AppConfig.Path_Bills + "\\" + "base.dat", FileMode.OpenOrCreate, FileAccess.Write);
                        StreamWriter sw = new StreamWriter(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Bills + "\\" + "base.dat");
                        sw.WriteLine(string.Format("{0}_{1:ddMMyy}", nom, DateTime.Now));
                        sw.Close();
                        sw.Dispose();
                    }
                    //stream.Dispose();
                    fRez = true;
                }
                catch (Exception e)
                {
                    CoreLib.WriteLog(e, "SaveBill(bool isNewBill, string nom, string comment, ref DataTable dTable); Error occured during saving bill file.");
                }

            /* # TEST MODE
                iNom = int.Parse(nom) + 1;
                nom = iNom.ToString();
            }
            */

            return fRez;
        }
        
        
        /*public static bool SaveBill(bool isNewBill, string nom, string comment, DataTable dTable, bool isLocked)
        {
            return SaveBill(isNewBill, nom, comment, dTable, isLocked, string.Empty);
        }
        public static bool SaveBill(bool isNewBill, string nom, string comment, DataTable dTable)
        {
            return SaveBill(isNewBill, nom, comment, dTable, false, string.Empty);
        }
        */
        public static void LockBill(DataTable dtBill, string fxNo)
        {
            DataWorkShared.SetBillProperty(dtBill, driver.Common.CoreConst.IS_LOCKED, true);
            //dtBill.ExtendedProperties["LOCK"] = true;
            if (fxNo != string.Empty)
            {
                DataWorkShared.SetOrderProperty(dtBill, driver.Common.CoreConst.ORDER_NO, fxNo);
                DataWorkShared.SetBillProperty(dtBill, driver.Common.CoreConst.DATETIME_LOCK, DateTime.Now.ToString("MM-dd-yy H:mm:ss"));
            }
            SaveBillToFile(dtBill);
        }
        public static void LockBill(DataTable dtBill)
        {
            LockBill(dtBill, string.Empty);
        }
        public static void UnlockBill(DataTable dtBill)
        {
            //dtBill.ExtendedProperties["LOCK"] = false;
            DataWorkShared.SetBillProperty(dtBill, driver.Common.CoreConst.IS_LOCKED, false);
            SaveBillToFile(dtBill);
        }


        public static void SaveBillToFile(DataTable dtBill)
        {
            //saving bill to binary file
            //FileStream stream = new FileStream(AppConfig.Path_Bills + "\\" + dtBill.ExtendedProperties["PATH"].ToString(), FileMode.OpenOrCreate);
            string savedBillFilePath = driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Bills + "\\" + DataWorkShared.ExtractBillProperty(dtBill, driver.Common.CoreConst.PATH);
            FileStream stream = new FileStream(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Bills + "\\" + DataWorkShared.ExtractBillProperty(dtBill, driver.Common.CoreConst.PATH), FileMode.OpenOrCreate);
            BinaryFormatter binF = new BinaryFormatter();
            DataWorkShared.SetBillProperty(dtBill, driver.Common.CoreConst.DATETIMEEDIT, DateTime.Now);
            binF.Serialize(stream, DataWorkShared.GetDataObject(dtBill));
            stream.Close();
            stream.Dispose();
            if (driver.Config.ConfigManager.Instance.CommonConfiguration.Content_Bills_AddCopyToArchive)
                new components.Components.szStorage.szStorage().CompressFiles(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Bills, "#Bills", driver.Config.ConfigManager.Instance.CommonConfiguration.APP_Admin, savedBillFilePath);
        }

        public static void MadeBillCopy(DataTable dtBill)
        {
            //string nom = dtBill.ExtendedProperties["NOM"].ToString();
            string nomOrig = DataWorkShared.ExtractBillProperty(dtBill, driver.Common.CoreConst.BILL_NO, string.Empty).ToString();
            string nom = nomOrig;
            string newNom = string.Empty;
            //int realNo = 0;
            int preffix = 0;
            string path = string.Empty;
            if (nom.Contains("-"))
            {
                string[] billno = nom.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                //realNo = int.Parse(billno[0]);
                nom = billno[0];
                preffix = int.Parse(billno[1]);
            }
            else
                ;// realNo = int.Parse(nom);
            nom = nom.Trim().PadLeft(5, '0');
            do
            {
                preffix++;
                path = string.Format("{0:X2}_N{1}{3}_{2:ddMMyy}.bill", driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit, nom, DateTime.Now, preffix != 0 ? "-" + preffix : "");
                newNom = string.Format("{0}{1}", nom, preffix != 0 ? "-" + preffix : "");
            } while (File.Exists(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Bills + "\\" + path));

            // update bill properties
            // 0 byte[] oid = System.Text.Encoding.Default.GetBytes(DateTime.Now.ToString() + "_" + newNom);
            // 1 byte[] secureOid = System.Security.Cryptography.HashAlgorithm.Create().ComputeHash(oid);
            // 0 string strOID = (System.Math.Abs(oid.GetHashCode().ToString().GetHashCode())).ToString();
            DataWorkShared.SetBillProperty(dtBill, driver.Common.CoreConst.OID, BillNewUID(newNom));
            DataWorkShared.SetBillProperty(dtBill, driver.Common.CoreConst.BILL_NO, newNom);
            DataWorkShared.SetBillProperty(dtBill, driver.Common.CoreConst.OWNER_NO, nomOrig);
            DataWorkShared.SetBillProperty(dtBill, driver.Common.CoreConst.DATETIME, DateTime.Now.ToShortDateString());
            DataWorkShared.SetBillProperty(dtBill, driver.Common.CoreConst.PATH, path);
            //dtBill.ExtendedProperties["OID"] = strOID;
            //dtBill.ExtendedProperties["NOM"] = newNom;
            //dtBill.ExtendedProperties["DT"] = DateTime.Now.ToShortDateString();
            //dtBill.ExtendedProperties["LOCK"] = false;
            // - dtBill.ExtendedProperties["FXNO"] = "-1";
            //dtBill.ExtendedProperties["PATH"] = path;
            SaveBillToFile(dtBill);
            //saving bill to binary file
            /*
            FileStream stream = new FileStream(AppConfig.Path_Bills + "\\" + dtBill.ExtendedProperties["PATH"].ToString(), FileMode.OpenOrCreate);
            BinaryFormatter binF = new BinaryFormatter();
            binF.Serialize(stream, dtBill);
            stream.Close();*/

            // updating base.dat info
            /*
            FileStream stream = new FileStream(AppConfig.Path_Bills + "\\" + "base.dat", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(stream);
            sw.WriteLine(string.Format("{0}_{1:ddMMyy}", newNom, DateTime.Now));
            sw.Close();
            sw.Dispose();
            stream.Dispose();
            */
        }

        public static bool BillDelete(DataTable dtBill)
        {
            /*
             if (DataWorkShared.GetOrderInfo(dtBill)[driver.Common.CoreConst.ORDER_NO] != string.Empty)
             {
                 MMessageBox.Show("Рахунок № " + DataWorkShared.ExtractBillProperty(dtBill, driver.Common.CoreConst.BILL_NO) + " є закритий",
                     Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                 return false;
             }

             if (MMessageBox.Show("Видалити рахунок № " + DataWorkShared.ExtractBillProperty(dtBill, driver.Common.CoreConst.BILL_NO),
                   Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
             {
                 return false;
             }
            */

            try
            {
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Bills + "\\" + DataWorkShared.ExtractBillProperty(dtBill, driver.Common.CoreConst.PATH), UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                dtBill.Clear();
                dtBill.ExtendedProperties.Clear();
                return true;
            }
            catch { }

            return false;
        }

        public static object[] LoadBillByPath(string path)
        {
            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryFormatter binF = new BinaryFormatter();
            object[] dTable = (object[])binF.Deserialize(stream);
            stream.Close();
            stream.Dispose();
            return dTable;
        }

        public static Hashtable ParseBillObject(object billEntry)
        {
            DataTable currentBill = (DataTable)((object[])billEntry)[0];
            PropertyCollection props = (PropertyCollection)((object[])billEntry)[1];
            Dictionary<string, object> billInfo = ((Dictionary<string, object>)props[CoreConst.BILL]);
            Hashtable output = new Hashtable();
            output.Add("E_BILL", currentBill);
            output.Add("E_PROPS", props);
            output.Add("E_BILLINFO", billInfo);
            //return new object[] { currentBill, props, billInfo };
            return output;
        }

        public static DataTable LoadCombinedBill(string path)
        {
            return DataWorkShared.CombineDataObject(DataWorkBill.LoadBillByPath(path));
        }

        /// <summary>
        /// Detect if there was some changes of current bill
        /// </summary>
        /// <param name="pathToBillFolder">Path to folder with bills</param>
        /// <param name="dtBill">Bill Object</param>
        /// <returns>0 = without changes; 1 = with changes; 2 = bill is closed; -1 = bill doesn't exist;</returns>
        public static int BillWasChanged(string pathToBillFolder, DataTable dtBill)
        {
            bool isNewBill = !dtBill.ExtendedProperties.Contains("BILL") || dtBill.ExtendedProperties["BILL"] == null;
            object billName = DataWorkShared.ExtractBillProperty(dtBill, CoreConst.PATH);

            if (isNewBill)
                return 0;

            if (!System.IO.File.Exists(pathToBillFolder + "\\" + billName.ToString()))
                return -1;

            DataTable loadedBill = LoadCombinedBill(pathToBillFolder + "\\" + billName.ToString());
            try
            {
                DateTime deEdit = (DateTime)DataWorkShared.ExtractBillProperty(loadedBill, CoreConst.DATETIMEEDIT);
                object deOrderNo = DataWorkShared.ExtractOrderProperty(loadedBill, CoreConst.ORDER_NO, null);
                if (deOrderNo != null && deOrderNo.ToString() != string.Empty)
                    return 2;

                DateTime deCurrentEdit = (DateTime)DataWorkShared.ExtractBillProperty(dtBill, CoreConst.DATETIMEEDIT);
                if (deEdit.CompareTo(deCurrentEdit) != 0)
                    return 1;

            }
            catch { }

            return 0;
        }

        public static Dictionary<string, object> LoadDayBills(DateTime selectedDay, string path, byte subunit)
        {
            //string item = string.Empty;
            string[] bills = Directory.GetFiles(path, string.Format("{0:X2}_N*_{1}.bill", subunit, selectedDay.ToString("ddMMyy")));
            Array.Sort(bills);
            //double billListSuma = 0.0;
            //double billSuma = 0.0;
            object[] billEntry = new object[2];
            //PropertyCollection props = new PropertyCollection();
            //Dictionary<string, object> billInfo = new Dictionary<string, object>();
            //object orderNo = new object();
            FileStream stream = null;
            DataTable dTBill = new DataTable();
            BinaryFormatter binF = new BinaryFormatter();
            //Dictionary<string, string> billFileList = new Dictionary<string, string>();
            Dictionary<string, object> output = new Dictionary<string, object>();

            for (int i = 0; i < bills.Length; i++/*, item = string.Empty*/)
            {
                try
                {
                    // load bill file
                    stream = new FileStream(bills[i], FileMode.Open, FileAccess.Read, FileShare.Read);
                    // parse bill entry
                    billEntry = (object[])binF.Deserialize(stream);
                    // get bill content
                    //dTBill = (DataTable)billEntry[0];
                    // get bill properties
                    //props = (PropertyCollection)billEntry[1];
                    // get bill info only
                    //billInfo = ((Dictionary<string, object>)props[CoreConst.BILL]);
                    // close and dispose file stream
                    stream.Close();
                    stream.Dispose();

                    output.Add(bills[i], billEntry);
                    /*
                    //Adding item
                    billFileList.Add(billInfo[CoreConst.OID].ToString(), bills[i]);
                    billSuma = (double)props[driver.Common.CoreConst.ORDER_SUMA];
                    orderNo = props[CoreConst.ORDER_NO];
                    // hide special flags
                    //if (orderNo != null && (string.Compare(orderNo.ToString(), "null") == 0 || string.Compare(orderNo.ToString(), "k") == 0))
                    //    orderNo = string.Empty;
                    listGrid.Rows.Add(
                        new object[] {
                            billInfo[CoreConst.OID], 
                            billInfo[CoreConst.BILL_NO],
                            billInfo[CoreConst.DATETIME],
                            billInfo[CoreConst.COMMENT], 
                            billSuma, 
                            bool.Parse(billInfo[CoreConst.IS_LOCKED].ToString()), 
                            orderNo
                        }
                    );
                    billListSuma += billSuma;
                    */
                }
                catch (Exception ex) { CoreLib.WriteLog(ex, "LoadDayBills(DateTime selectedDay); Unable to load bill file: " + bills[i]); }

            }

            //return billListSuma;
            return output;
        }
        public static Dictionary<string, object> LoadDayBills(string path, byte subunit)
        {
            return LoadDayBills(DateTime.Now, path, subunit);
        }
        public static Dictionary<string, object> LoadRangeBills(DateTime dateFrom, DateTime dateTo, string path, byte subunit)
        {
            if (dateFrom == dateTo)
                return LoadDayBills(dateFrom, path, subunit);

            Dictionary<string, object> rangeBills = new Dictionary<string, object>();
            Dictionary<string, object> currentBills = new Dictionary<string, object>();
            while (dateFrom < dateTo)
            {
                currentBills = LoadDayBills(dateFrom, path, subunit);
                foreach (KeyValuePair<string, object> bill in currentBills)
                    rangeBills.Add(bill.Key, bill.Value);
                dateFrom = dateFrom.AddDays(1.0);
            }
            return rangeBills;
        }
        public static Dictionary<string, object> LoadRangeBills(DateTime dateFrom, string path, byte subunit)
        {
            return LoadRangeBills(dateFrom, DateTime.Now, path, subunit);
        }
        public static Dictionary<string, object> LoadRangeBills(string path, byte subunit)
        {
            return LoadDayBills(DateTime.Now, path, subunit);
        }

        public static DataSet LoadCombinedDayBills(DateTime selectedDay, string path, byte subunit)
        {
            DataSet todayBillDataSet = new DataSet("BillDataSet_" + selectedDay.ToString("dd-MM-yyyy") + "_U" + subunit);
            Dictionary<string, object> items = LoadDayBills(selectedDay, path, subunit);
            foreach (KeyValuePair<string, object> billItem in items)
            {
                todayBillDataSet.Tables.Add(DataWorkShared.CombineDataObject((object[])billItem.Value));
            }
            
            return todayBillDataSet;
        }
        public static DataSet LoadCombinedDayBills(string path, byte subunit)
        {
            return LoadCombinedDayBills(DateTime.Now, path, subunit);
        }
        public static DataSet LoadCombinedRangeBills(DateTime dateFrom, DateTime dateTo, string path, byte subunit)
        {
            DataSet rangeBills = new DataSet();
            while (dateFrom < dateTo)
            {
                rangeBills.Merge(LoadCombinedDayBills(dateFrom, path, subunit));
                dateFrom = dateFrom.AddDays(1.0);
            }
            return rangeBills;
        }
        public static DataSet LoadCombinedRangeBills(DateTime dateFrom, string path, byte subunit)
        {
            return LoadCombinedRangeBills(dateFrom, DateTime.Now, path, subunit);
        }
        public static DataSet LoadCombinedRangeBills(string path, byte subunit)
        {
            return LoadCombinedDayBills(DateTime.Now, path, subunit);
        }
    }
}
