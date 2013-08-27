using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Reflection;
using Microsoft.VisualBasic.FileIO;
using System.Windows.Forms;
/* internal */
using driver.Config;
using driver.Components.UI;
using System.Collections;
using components.Components.WinApi;
/* custom */
//using SevenZip;

namespace driver.Lib
{
    public class DataWorkCheque
    {

        //Without driver
        private static string SaveDBF(DataTable DT, string path)
        {
            int i = 0;

            string[] FieldName = DT.ExtendedProperties["FIELDS"].ToString().Split(new char[] { ',' }); // Массив названий полей
            string[] FieldType = new string[FieldName.Length]; // Массив типов полей
            byte[] FieldSize = new byte[FieldName.Length]; // Массив размеров полей
            byte[] FieldDigs = new byte[FieldName.Length]; // Массив размеров дробной части

            // Создаю таблицу
            string outputDBPath = path + "\\" + DT.TableName + ".DBF";
            System.IO.File.Delete(outputDBPath);
            System.IO.FileStream FS = new System.IO.FileStream(outputDBPath, System.IO.FileMode.Create);

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
                    l = MathLib.ReplaceNDS(l, "."); // Replase all , to .
                    buffer = System.Text.Encoding.Default.GetBytes(l); // Записываю в кодировке ANSI
                    FS.Write(buffer, 0, buffer.Length);
                    Application.DoEvents();
                }
            }

            FS.WriteByte(0x1A); // Конец данных
            FS.Close();
            FS.Dispose();

            return outputDBPath;
        }//ok

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
            if (!Directory.Exists(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Cheques))
                Directory.CreateDirectory(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Cheques);

            string client = (string)param[0];
            double discount = (double)param[1];
            double chqSUMA = (double)param[2];
            double taxSUMA = (double)param[3];
            bool report = (bool)param[4];
            bool retrive = (bool)param[5];
            bool useTotDisc = (bool)param[6];
            string discColumn = useTotDisc ? "DR" : "DISC";
            string znom = string.Empty;
            bool isFx = (chqNom != string.Empty);

            Hashtable info = new Hashtable();

            info["PROFILE_ID"] = dTable.TableName == string.Empty ? "00" : dTable.TableName;
           
            if (param[7] != null)
                znom = param[7].ToString();

            if (retrive)
            {
                chqSUMA *= -1;
                taxSUMA *= -1;

                for (ushort i = 0; retrive && i < dTable.Rows.Count; i++)
                    dTable.Rows[i]["TOT"] = -MathLib.GetDouble(dTable.Rows[i]["TOT"]);
            }

            if (!isFx)
            {
                if (info["PROFILE_ID"].ToString().Equals("00"))
                    NonFxChqsInfo(chqSUMA, ref chqNom);
                else
                    try
                    {
                        NonFxChqsInfo(chqSUMA, ref chqNom, int.Parse(info["PROFILE_ID"].ToString()));
                    }
                    catch { }
            }

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
            ALTdTable.TableName = ChequeTableName(chqSUMA, report, isFx ? chqNom : "N" + chqNom, retrive, pType, znom, info);

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
                //winapi.Funcs.OutputDebugString("1");   C:\Server\out-2
                string outputDir = getSavePathCheques(dTable.TableName);
                string savedChequeFilePath = SaveDBF(ALTdTable, outputDir);
                // if use secure saving
                if (driver.Config.ConfigManager.Instance.CommonConfiguration.Content_Cheques_AddCopyToArchive)
                    new components.Components.szStorage.szStorage().CompressFiles(outputDir, "#Cheques", driver.Config.ConfigManager.Instance.CommonConfiguration.APP_Admin, savedChequeFilePath);
                //winapi.Funcs.OutputDebugString("2");
            }
            catch (Exception e)
            {
               CoreLib.WriteLog(e, MethodInfo.GetCurrentMethod().Name);
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
            return NonFxChqsInfo(suma, ref chqNom, 0);
        }

        public static string[] NonFxChqsInfo(double suma, ref string chqNom, int profileID)
        {
            string[] localData = new string[3] { "0", DateTime.Now.ToShortDateString(), "0.00" };

            string fileName = string.Format("{1}{0}{2}", profileID == 0 ? string.Empty : profileID.ToString("_p0"), "base", ".dat");

            if (!File.Exists(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Cheques + "\\" + fileName))
            {
                StreamWriter sw = new StreamWriter(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Cheques + "\\" + fileName, false, Encoding.Default);
                sw.WriteLine("0");
                sw.WriteLine(DateTime.Now.ToString("dd.MM.yyyy"));
                sw.Write("0.00");

                if (sw != null)
                    sw.Close();
                sw.Dispose();
            }
            else
            {
                StreamReader sr = File.OpenText(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Cheques + "\\" + fileName);
                try
                {
                    localData[0] = sr.ReadLine();
                    localData[1] = sr.ReadLine();
                    localData[2] = sr.ReadLine();
                }
                catch { }
                //get info about cheques (total, date, suma)
                if (string.IsNullOrEmpty(localData[1]) || DateTime.Now.ToShortDateString() != localData[1])
                {
                    localData[0] = "0";//total chqs
                    localData[1] = DateTime.Now.ToString("dd.MM.yyyy");//date
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
                StreamWriter sw = new StreamWriter(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Cheques + "\\" + fileName, false, Encoding.Default);
                sw.WriteLine(chqNom);
                sw.WriteLine(localData[1]);
                sw.Write(string.Format("{0:F" + ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "} ", MathLib.GetDouble(localData[2].Trim()) + suma));

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
        private static string ChequeTableName(double suma, bool rep, string nom, bool ret, int pType, string znom, Hashtable info)
        {
            string[] values = new string[10];
            values[0] = string.Format("{0:X2}", driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit);
            values[1] = string.Format("{0:X2}", driver.Config.ConfigManager.Instance.CommonConfiguration.APP_PayDesk);
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
            if (ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles)
            {
                values[9] = string.Format("{0:X2}", int.Parse(info["PROFILE_ID"].ToString()));
                // override subunit
                Hashtable profile = (Hashtable)ConfigManager.Instance.CommonConfiguration.PROFILES_Items[info["PROFILE_ID"]];
                int profileSubunit = 0;
                if (profile.ContainsKey("SUBUNIT") && int.TryParse(profile["SUBUNIT"].ToString(), out profileSubunit))
                    values[0] = string.Format("{0:X2}", profileSubunit);
            }

            string chqName = driver.Config.ConfigManager.Instance.CommonConfiguration.APP_ChequeName;
            for (byte i = 0; i < values.Length; i++)
                chqName = chqName.Replace("%" + i, values[i]);

            string outputDir = getSavePathCheques(info["PROFILE_ID"].ToString());
            if (File.Exists(outputDir + "\\" + "C" + chqName + ".DBF"))
                return ChequeTableName(suma, rep, nom + "c", ret, pType, znom, info);
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
        /// 
        public static void SaveInvent(DataTable dTable, bool isBackUp, DataSet cheques)
        {
            if (ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles)
            {
                foreach (DataTable dt in cheques.Tables)
                {
                    SaveInvent_Single(dt.Copy(), isBackUp);
                }
            }
            else
                SaveInvent_Single(dTable.Copy(), isBackUp);
        }

        public static void SaveInvent_Single(DataTable dTable, bool isBackUp)
        {
            if (File.Exists(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Cheques + "\\" + "_" + dTable.ExtendedProperties["Name"] + ".inv"))
                FileSystem.DeleteFile(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Cheques + "\\" + "_" + dTable.ExtendedProperties["Name"] + ".inv",
                    UIOption.OnlyErrorDialogs,
                    RecycleOption.SendToRecycleBin);
            if (File.Exists(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Cheques + "\\" + dTable.ExtendedProperties["Name"] + ".dbf"))
                FileSystem.DeleteFile(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Cheques + "\\" + dTable.ExtendedProperties["Name"] + ".dbf",
                    UIOption.OnlyErrorDialogs,
                    RecycleOption.SendToRecycleBin);

            //dTable.TableName = dTable.ExtendedProperties["Name"].ToString();
            if (bool.Parse(dTable.ExtendedProperties["GetNewName"].ToString()))
            {
                if (ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles)
                    dTable.TableName = InventTableName_Profile(DateTime.Now, dTable);
                else
                    dTable.TableName = InventTableName(DateTime.Now);
                dTable.ExtendedProperties["Name"] = dTable.TableName;
                dTable.ExtendedProperties["Date"] = DateTime.Now.ToShortDateString();
            }

            FileStream stream = new FileStream(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Cheques + "\\" + "_" + dTable.ExtendedProperties["Name"] + ".inv",
                FileMode.OpenOrCreate);
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binF = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
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

                SaveDBF(ALTdTable, driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Cheques);

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
        public static DataSet OpenInvent(DataSet cheques)
        {
            DataSet dMergedDataTable = new DataSet();
            DateTime? dTime = DateTime.Now;
            DataTable tempDt = null;
            foreach (DataTable dt in cheques.Tables)
            {
                tempDt = OpenInvent(InventTableName_Profile(dTime, dt));
                tempDt.TableName = dt.TableName;
                tempDt.Merge(dt.Clone());
                dMergedDataTable.Tables.Add(tempDt);
            }

            return dMergedDataTable;
        }

        public static DataTable OpenInvent()
        {
            return OpenInvent(string.Empty);
        }

        public static DataTable OpenInvent(string tableName)
        {
            DataTable dTable = new DataTable();
            DateTime? dTime = DateTime.Now;
            if (driver.Config.ConfigManager.Instance.CommonConfiguration.APP_ShowInventWindow)
                dTime = new InventList().OpenInvent();
            string tbName = tableName;
            if (tbName == string.Empty)
                tbName = InventTableName(dTime);
            if (tbName == "")
                return null;
            //{
            //    dTable.ExtendedProperties["Load"] = false;
            //    return dTable;
            //}
            
            if (!File.Exists(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Cheques + "\\" + "_" + tbName + ".inv"))
            {
                dTable.ExtendedProperties.Add("Name", tbName);
                dTable.ExtendedProperties.Add("GetNewName", true);
                dTable.ExtendedProperties.Add("Date", dTime.Value.ToShortDateString());
                return dTable;
            }

            FileStream stream = new FileStream(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Cheques + "\\" + "_" + tbName + ".inv", FileMode.Open);
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binF = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            dTable = (DataTable)binF.Deserialize(stream);
            stream.Close();
            stream.Dispose();

            if (dTime.Value.Day != DateTime.Now.Day ||
                dTime.Value.Month != DateTime.Now.Month ||
                dTime.Value.Year != DateTime.Now.Year)
                dTable.ExtendedProperties["GetNewName"] = false;

            return dTable;
        }//ok
        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private static string InventTableName(DateTime? date)
        {
            return InventTableName_Profile(date, null);
        }

        private static string InventTableName_Profile(DateTime? date, DataTable dTable)
        {
            if (!date.HasValue)
                return "";

            string[] values = new string[4];
            values[0] = string.Format("{0:X2}", driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit);
            values[1] = string.Format("{0:X2}", driver.Config.ConfigManager.Instance.CommonConfiguration.APP_PayDesk);
            values[2] = string.Format("{0:yyMMdd}", date.Value);
            values[3] = string.Empty;

            if (dTable != null)
                values[3] = string.Format("{0:X2}", int.Parse(dTable.TableName));

            if (ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles)
            {
                // override subunit
                Hashtable profile = (Hashtable)ConfigManager.Instance.CommonConfiguration.PROFILES_Items[dTable.TableName];
                int profileSubunit = 0;
                if (profile.ContainsKey("SUBUNIT") && int.TryParse(profile["SUBUNIT"].ToString(), out profileSubunit))
                    values[0] = string.Format("{0:X2}", profileSubunit);
            }
            //string NamebyFormat = "%0%1_%2";

            string NamebyFormat = string.Format("{3}{0}{1}_{2}", values);
            /*
            for (byte i = 0; i < values.Length; i++)
                NamebyFormat = NamebyFormat.Replace("%" + i, values[i]);
            */
            return "IS" + NamebyFormat;
        }//ok

        public static string getSavePathCheques(string profileID)
        {
            string outputDir = "";
            if (ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles)
                try
                {
                    outputDir = ((System.Collections.Hashtable)driver.Config.ConfigManager.Instance.CommonConfiguration.PROFILES_Items[profileID])["OUTPUT"].ToString();
                }
                catch (Exception e) { CoreLib.WriteLog(e, MethodInfo.GetCurrentMethod().Name); }

            if (outputDir == string.Empty)
                outputDir = driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Cheques;
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);
            return outputDir;
        }

    }
}
