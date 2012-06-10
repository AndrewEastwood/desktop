using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CSharp;
using System.CodeDom;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using components.Public;

namespace mpwsDBU
{
    public class corelib
    {
        public const string UPLOAD_MODE_INSERT = "INSERT";
        public const string UPLOAD_MODE_UPDATE = "UPDATE";
        public const string UPLOAD_MODE_DELETE = "DELETE";

        private static int _internalIdx = 0;

        public static string ToLiteral(string input)
        {
            return System.Security.SecurityElement.Escape(input);
        }

        public static string ToLiteralEx(string input)
        {
            return System.Security.SecurityElement.Escape(input).Replace(',', '.');
        }

        // get all data command
        public string getSqlForAllTables(DataLayoutCollection ds)
        {
            List<string> data = new List<string>();

            foreach (excelDataView dtp in ds.GetDataControls())
            {
                // startup comment
                data.Add(Environment.NewLine);
                data.Add("/" + string.Empty.PadLeft(50, '*'));
                data.Add("                     QUERY FOR " + dtp.dataLayout.TableName);
                data.Add(string.Empty.PadLeft(50, '*') + "/");
                data.Add(this.getSqlCommandWithDataTable(dtp.Copy()));
                _internalIdx = 0;
            }

            return string.Join(Environment.NewLine, data.ToArray());
        }

        public string getSqlCommandWithDataTable(excelDataView xdv)
        {
            string sqlCommandPattern = string.Empty;
            if (ApplicationConfiguration.Instance.GetValueByPath<string>("formatConfiguration.uploadMode") == corelib.UPLOAD_MODE_INSERT)
                sqlCommandPattern = "INSERT INTO {0} ({1}) VALUES ({2});";

            if (ApplicationConfiguration.Instance.GetValueByPath<string>("formatConfiguration.uploadMode") == corelib.UPLOAD_MODE_UPDATE)
                sqlCommandPattern = "UPDATE {0} SET {1} WHERE {0}.{3} = {2};";

            if (ApplicationConfiguration.Instance.GetValueByPath<string>("formatConfiguration.uploadMode") == corelib.UPLOAD_MODE_DELETE)
                sqlCommandPattern = "DELETE FROM {0} WHERE {0}.{3} = {1};";


            return getSqlCommandWithDataTable(xdv, sqlCommandPattern);
        }

        public string getSqlCommandWithDataTable(excelDataView xdv, string sqlCommandPattern)
        {
            return getSqlCommandWithDataTable(xdv, sqlCommandPattern, false);
        }

        public string getSqlCommandWithDataTable(excelDataView xdv, string sqlCommandPattern, bool useSelectedRows)
        {
            string uploadMode = string.Empty;

            if (ApplicationConfiguration.Instance.GetValueByPath<string>("commandsConfiguration.update") == sqlCommandPattern)
                uploadMode = corelib.UPLOAD_MODE_UPDATE;

            if (ApplicationConfiguration.Instance.GetValueByPath<string>("commandsConfiguration.insert") == sqlCommandPattern)
                uploadMode = corelib.UPLOAD_MODE_INSERT;

            if (ApplicationConfiguration.Instance.GetValueByPath<string>("commandsConfiguration.delete") == sqlCommandPattern)
                uploadMode = corelib.UPLOAD_MODE_DELETE;

            return getSqlCommandWithDataTable(xdv, sqlCommandPattern, useSelectedRows, uploadMode);
        }

        // get all rows command
        public string getSqlCommandWithDataTable(excelDataView xdv, string sqlCommandPattern, bool useSelectedRows, string uploadMode)
        {
            DataTable dt = xdv.IndependentDataLayout;
            string dbTableName = dt.TableName;

            // command data
            List<string> sqlLines = new List<string>();

            // get repalced columns
            Dictionary<string, string> filedNameExchange = ApplicationConfiguration.Instance.GetTableDictValueByPath("mappingConfiguration.TableField", "ColumnSource", "ColumnDB");
            // get replaced tablename
            Dictionary<string, string> tableNameExchange = ApplicationConfiguration.Instance.GetTableDictValueByPath("mappingConfiguration.TableName", "dataGridViewTextBoxColumn1", "dataGridViewTextBoxColumn2");
            // get indexes
            Dictionary<string, string> tableIndexesAdd = ApplicationConfiguration.Instance.GetTableDictValueByPath("mappingConfiguration.Indexes", "ColumnTableOwner", "ColumnIdxName");

            // if table page has to be chnaged --- replace it
            if (tableNameExchange.ContainsKey(dbTableName) && tableNameExchange[dbTableName] != string.Empty)
                dbTableName = tableNameExchange[dbTableName];

            if (dbTableName == string.Empty)
                return string.Empty;

            if (ApplicationConfiguration.Instance.GetValueByPath<bool>("formatConfiguration.alwaysTruncate"))
                sqlLines.Add(string.Format("TRUNCATE {0};", dbTableName));


            // loop by columns (fields)
            List<string> columns = new List<string>();
            string columnName = string.Empty;
            // get new column names
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                columnName = dt.Columns[j].ColumnName;

                if (filedNameExchange.ContainsKey(dt.Columns[j].ColumnName))
                    columnName = filedNameExchange[dt.Columns[j].ColumnName];

                if (columns.Contains(columnName))
                    continue;

                if (columnName != "ST")
                    columns.Add(columnName);
            }
            // add or detect primary data column
            bool indexWasAdded = false;
            bool indexIsManual = false;
            string dataRowIndexName = ApplicationConfiguration.Instance.GetValueByPath<string>("mappingConfiguration.defaultIndexFiledName");
            // use id as row index
            if (ApplicationConfiguration.Instance.GetValueByPath<bool>("formatConfiguration.useRecordIdAsRowIndex"))
                columns.Add(ApplicationConfiguration.Instance.GetValueByPath<string>("mappingConfiguration.defaultIndexFiledName"));
            else
            // try to add custom index
                if (ApplicationConfiguration.Instance.GetValueByPath<bool>("mappingConfiguration.useCustomIndexes") && tableIndexesAdd.ContainsKey(dt.TableName))
                {
                    if (columns.Contains(tableIndexesAdd[dt.TableName]))
                        indexIsManual = true;
                    else
                    {
                        columns.Add(tableIndexesAdd[dt.TableName]);
                        indexWasAdded = true;
                    }
                    dataRowIndexName = tableIndexesAdd[dt.TableName];
                }

            // column formatting
            // string.Format(Properties.Settings.Default.dataFormatColumnQuote, tableIndexesAdd[dt.TableName])
            columns = Array.ConvertAll<string, string>(columns.ToArray(), ci => string.Format(ApplicationConfiguration.Instance.GetValueByPath<string>("formatConfiguration.columnFormat"), ci)).ToList<string>();


            // sql params
            string sqlCommand = string.Empty;


            // loop by rows (values)
            List<object> dataValues = new List<object>();
            int jv = 0;

            Dictionary<int, DataRow> drwColl = new Dictionary<int, DataRow>();

            // collect only selected rows
            if (useSelectedRows)
            {
                DataRowView drv = null;
                foreach (DataGridViewRow dgvRow in xdv.getViewSelected())
                {
                    drv = (DataRowView)dgvRow.DataBoundItem;
                    drwColl.Add(dgvRow.Index, drv.Row);
                }
            }//dt.Rows.Cast<int, DataRow>().Select(xr=>xr x=>x)
            else
            {
                int dxidx = 0;
                drwColl = dt.Rows.Cast<DataRow>().Select(x => x).ToDictionary<DataRow, int>(x => dxidx++);
            }

            // getting linked values for this list
            Hashtable valueLinker = new Hashtable();
            if (ApplicationConfiguration.Instance.GetValueByPath<Hashtable>("mappingConfiguration.TableValue") != null)
                valueLinker = ApplicationConfiguration.Instance.GetTableHashValueByPath("mappingConfiguration.TableValue");

            DataRow record = null;
            int indexNo = 0;
            foreach (KeyValuePair<int, DataRow> rec in drwColl)
            {
                // use record index as sql command index
                if (!indexIsManual)
                    if (ApplicationConfiguration.Instance.GetValueByPath<bool>("formatConfiguration.useRecordIdAsRowIndex"))
                        indexNo++;
                    else
                        indexNo = rec.Key + 1;

                dataValues.Clear();

                // record
                record = rec.Value;

                // check if all items are not empty
                if (this.IsRowEmpty(record.ItemArray))
                    continue;

                // value link
                this.getLinkedValue(xdv.LayoutCollection, valueLinker, ref record);

                // escape each value
                dataValues.AddRange(this.getValuesWithDefault(rec.Value));

                // removing last state column
                if (record.Table.Columns.Contains("ST"))
                    dataValues.Remove(dataValues.Last());

                // use id as row index
                if (!indexIsManual && ApplicationConfiguration.Instance.GetValueByPath<bool>("formatConfiguration.useRecordIdAsRowIndex") && !indexWasAdded)
                    dataValues.Insert(0, indexNo);

                // data value formatting
                dataValues = Array.ConvertAll<object, object>(dataValues.ToArray(), k => string.Format(ApplicationConfiguration.Instance.GetValueByPath<string>("formatConfiguration.valueFormat"), k)).ToList<object>();

                // manual index value
                object dataIndex = indexNo;
                if (!ApplicationConfiguration.Instance.GetValueByPath<bool>("formatConfiguration.useRecordIdAsRowIndex") && indexIsManual)
                    for (int i = 0; i < columns.Count; i++)
                        if (columns[i].Contains(dataRowIndexName))
                        {
                            dataIndex = getNumer(dataValues[i].ToString());
                        }

                // this data is new
                List<string> _tmpCol;// = columns.ToList();
                List<object> _tmpDat;// = dataValues.ToList();

                getCustomTableFields(dt.TableName, out _tmpCol, out _tmpDat, columns, dataValues);

                // creating sql command
                switch (uploadMode)
                {
                    case corelib.UPLOAD_MODE_DELETE:
                        {
                            // 0 - db name
                            // 1 - idx
                            sqlLines.Add(string.Format(sqlCommandPattern, dbTableName, dataIndex, dataRowIndexName));

                            break;
                        }

                    case corelib.UPLOAD_MODE_INSERT:
                        {
                            // 0 - db name
                            // 1 - columns
                            // 2 - values
                            // 3 - idx

                            // this data is new
                            //List<string> _tmpCol;// = columns.ToList();
                            //List<object> _tmpDat;// = dataValues.ToList();
                            /*
                            _tmpCol.Add("ADATECREATE");
                            _tmpCol.Add("ADATEUPDATE");
                            _tmpDat.Add("'" + DateTime.Now.ToString() + "'");
                            _tmpDat.Add("'" + DateTime.Now.ToString() + "'");
                            */

                            sqlLines.Add(string.Format(sqlCommandPattern, dbTableName, string.Join(", ", _tmpCol.ToArray()), string.Join<object>(", ", _tmpDat), dataIndex));

                            break;
                        }
                    case corelib.UPLOAD_MODE_UPDATE:
                        {
                            // 0 - db name
                            // 1 - update fields
                            // 2 - idx

                            int index = 0;
                            List<string> upItems = new List<string>();
                            //List<string> tmpCol = columns.ToList();
                            _tmpCol.Remove('`' + dataRowIndexName + '`');
                            //List<object> tmpDv = dataValues.ToList();
                            _tmpDat.RemoveAt(0);


                            // this data exists
                            //tmpCol.Add("ADATEUPDATE");
                            //tmpDv.Add("'" + DateTime.Now.ToString() + "'");

                            upItems = _tmpCol.ConvertAll<string>(
                                col => (col.Contains("ADATECREATE") ? "-" : string.Format("{0} = {1}", col, _tmpDat[index++]))
                            );

                            upItems.RemoveAll(delegate(string currentItem) {
                                return currentItem == "-";
                            });


                            // use data value by id as row index
                            /*
                            if (!Properties.Settings.Default.dataFormat_idAsRowIndex && indexWasAdded)
                                for (int i = 0; i < upItems.Count; i++)
                                    if (upItems[i].Contains(dataRowIndexName))
                                    {
                                        dataIndex = getNumer(upItems[i]);
                                    }
                            */

                            // indexNo
                            if (upItems.Count == 1)
                                sqlLines.Add(string.Format(sqlCommandPattern, dbTableName, upItems[0], dataIndex, dataRowIndexName));
                            else
                                sqlLines.Add(string.Format(sqlCommandPattern, dbTableName, string.Join(", ", upItems.ToArray()), dataIndex, dataRowIndexName));

                            break;
                        }
                }


            }

            sqlLines.Insert(0, "SET NAMES cp1251;");

            return string.Join(Environment.NewLine, sqlLines.ToArray());
        }

        private static void getCustomTableFields(string tableName, out List<string> columns, out List<object> values, List<string> currentColumns, List<object> currentValues)
        {
            columns = currentColumns.ToList();
            values = currentValues.ToList();

            Hashtable fields = ApplicationConfiguration.Instance.GetValueByPath<Hashtable>("mappingConfiguration.customDataFields");
            Hashtable _currentDataRow = null;
            foreach (DictionaryEntry htRow in fields)
            {
                _currentDataRow = (Hashtable)htRow.Value;

                if (!_currentDataRow["AdditionalColumnOwner"].ToString().Equals(tableName))
                    continue;


                columns.Add('`' + _currentDataRow["AdditionalFieldColumnName"].ToString() + '`');

                switch (_currentDataRow["AdditionalFieldColumnValue"].ToString())
                {
                    case "DT":
                        {
                            System.Data.SqlTypes.SqlDateTime dt = new System.Data.SqlTypes.SqlDateTime(DateTime.Now);

                            values.Add("'" + DateTime.Now.ToString("yyyy-MM-dd H:mm:ss") + "'");
                            break;
                        }
                    case "D":
                        {
                            values.Add("'" + DateTime.Now.ToShortDateString() + "'");
                            break;
                        }
                    case "T":
                        {
                            values.Add("'" + DateTime.Now.ToShortTimeString() + "'");
                            break;
                        }
                    default:
                        {
                            values.Add("'" + _currentDataRow["AdditionalFieldColumnValue"].ToString() + "'");
                            break;
                        }
                }

            }
        }

        private static int getNumer(string str)
        {
            string numline = string.Empty;

            foreach (char c in str)
                if (char.IsNumber(c))
                    numline += c;

            return int.Parse(numline);
        }

        public static DataTable GetRemoteDataTable(string command, MySql.Data.MySqlClient.MySqlConnection msqConn)
        {
            DataTable rdt = new DataTable();

            try
            {
                msqConn.Open();

                MySql.Data.MySqlClient.MySqlCommand uploaderCmd = new MySql.Data.MySqlClient.MySqlCommand(command, msqConn);
                MySql.Data.MySqlClient.MySqlDataReader onj = uploaderCmd.ExecuteReader();

                bool colWasPerf = false;
                int colIdx = 0;
                List<object> rowValues = new List<object>();
                foreach (System.Data.Common.DbDataRecord ro in onj)
                {
                    try
                    {
                        // get fields
                        if (rdt.Columns.Count == 0 && !colWasPerf)
                        {
                            for (colIdx = 0; colIdx < ro.FieldCount; colIdx++)
                                rdt.Columns.Add(ro.GetName(colIdx));
                            colWasPerf = true;
                        }
                    }
                    catch { }

                    try
                    {
                        for (colIdx = 0; colIdx < ro.FieldCount; colIdx++)
                            rowValues.Add(ro[colIdx]);
                    }
                    catch { }

                    try
                    {
                        rdt.Rows.Add(rowValues.ToArray());
                        rowValues.Clear();
                    }
                    catch { }
                }

                onj.Close();

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            if (msqConn.State == System.Data.ConnectionState.Open)
                msqConn.Close();

            return rdt;
        }

        public static DataTable GetRemoteDataTable(string command, Hashtable connect)
        {

            MySql.Data.MySqlClient.MySqlConnectionStringBuilder msqConnStr = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder();

            msqConnStr.UserID = connect["UserID"].ToString();
            msqConnStr.Password = connect["Password"].ToString();
            msqConnStr.Database = connect["Database"].ToString();
            msqConnStr.Server = connect["Server"].ToString();
            msqConnStr.AllowZeroDateTime = true;

            MySql.Data.MySqlClient.MySqlConnection msqConn = new MySql.Data.MySqlClient.MySqlConnection(msqConnStr.ToString());

            return GetRemoteDataTable(command, msqConn);
        }

        public object[] getValuesWithDefault(DataRow drw)
        {
            object[] values = null;
            Hashtable defValues = ApplicationConfiguration.Instance.GetTableHashValueByPath("mappingConfiguration.TableDefaultValue");
            /*foreach (string scpvi in Properties.Settings.Default.comp_defValues)
            {
                valSplit = scpvi.Split(':');
                if (drw.Table.Columns.Contains(valSplit[0]) && drw[valSplit[0]] == DBNull.Value)
                    drw[valSplit[0]] = valSplit[1];
            }*/

            // escape each value
            if (ApplicationConfiguration.Instance.GetValueByPath<bool>("formatConfiguration.escapeValues"))
            {
                // for (jv = 0; jv < drwColl[i].ItemArray.Length; jv++)
                values = Array.ConvertAll<object, object>(drw.ItemArray, x => corelib.ToLiteralEx(x.ToString()));
                //  corelib.ToLiteral(drwColl[i][jv].ToString()));
            }
            else
                values = drw.ItemArray;

            return values;
        }

        public void getLinkedValue(DataLayoutCollection layouts, Hashtable linker, ref DataRow dr)
        {
            //DataRow dr = drs.Table.NewRow();

            // by each linked values
            string valDispl = string.Empty;
            string[] linkedVal = new string[2];
            string newValue = string.Empty;
            DataRow[] linkedRows = null;
            string[] linkCondition = new string[2];
            string linkedColumn = string.Empty;
            string newValueColumn = string.Empty;

            string[] _keyCondition = new string[2];

            foreach (DictionaryEntry cvc in linker)
            {
                _keyCondition = cvc.Key.ToString().Split('!');
                // if row contains filed
                if (dr.Table.Columns.Contains(_keyCondition[1]))
                {
                    // this value will be used
                    // for searching in linked
                    // data
                    valDispl = dr[_keyCondition[1]].ToString();

                    // set new value as default current
                    newValue = valDispl;

                    // try to find out 
                    // linked value
                    // 0 - layout name
                    // 1 - filed name
                    linkedVal = cvc.Value.ToString().Split('!');

                    // value is from other layout
                    if (linkedVal.Length == 2)
                    {
                        linkedColumn = linkedVal[1];
                        newValueColumn = linkedVal[1];
                        linkCondition = linkedVal[1].Split('=');

                        if (linkCondition.Length == 2)
                        {
                            linkedColumn = linkCondition[0];
                            newValueColumn = linkCondition[1];
                        }

                        // search value in all columns
                        try
                        {
                            linkedRows = layouts[linkedVal[0]].dataLayout.Select(string.Format("{0} Like '{1}'", linkedColumn, valDispl));
                            if (linkedRows.Length > 0)
                            {
                                newValue = linkedRows[0][newValueColumn].ToString();
                            }
                        }
                        catch { }

                    }

                    dr[_keyCondition[1]] = newValue;
                }
            }

            //return dr;
        }

        public string getSqlCommand(Hashtable config)
        {
            // DATA
            // CMD_PATTERN
            // ROW_ELECTED
            // ROW_USE_ID
            // CMD_PREFIX
            // CMD_SUFFIX


            return string.Empty;

        }

        public static bool Upload(string data, Hashtable connect, DataLayoutCollection layouts)
        {/*
            System.Data.SqlClient.SqlConnectionStringBuilder connString = new System.Data.SqlClient.SqlConnectionStringBuilder();

            //connString.DataSource = "localhost";
            connString.UserID = "root";
            connString.Password = "1111";
            connString.InitialCatalog = "mpwsdbu_test";
            connString.NetworkLibrary = "dbmssocn";
            connString.DataSource = "localhost";
            connString.Add("server", @".\\MySQL");


            System.Data.SqlClient.SqlConnection dataConnect = new System.Data.SqlClient.SqlConnection(connString.ToString());




            dataConnect.Open();

            System.Data.SqlClient.SqlCommand uploadCmd = new System.Data.SqlClient.SqlCommand(data, dataConnect);

            uploadCmd.ExecuteNonQuery();

            dataConnect.Close();


            */




            if (data == null || data == string.Empty)
                return false;

            if (ApplicationConfiguration.Instance.GetValueByPath<bool>("formatConfiguration.autoBackup"))
                new corelib().Backup(layouts.GetDataTableNames().ToArray(), connect);

            bool frez = true;

            MySql.Data.MySqlClient.MySqlConnectionStringBuilder msqConnStr = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder();


            
            msqConnStr.UserID = connect["UserID"].ToString();
            msqConnStr.Password = connect["Password"].ToString();
            msqConnStr.Database = connect["Database"].ToString();
            msqConnStr.Server = connect["Server"].ToString();
            msqConnStr.CharacterSet = "cp1251";
            msqConnStr.AllowZeroDateTime = true;

            MySql.Data.MySqlClient.MySqlConnection msqConn = new MySql.Data.MySqlClient.MySqlConnection(msqConnStr.ToString());

            try
            {
                msqConn.Open();
                /*
                Encoding utf8 = Encoding.GetEncoding(866);
                Encoding win1251 = Encoding.Default;

                byte[] utf8Bytes = win1251.GetBytes(data);
                byte[] win1251Bytes = Encoding.Convert(win1251, Encoding.UTF8, utf8Bytes);
                string newdata = win1251.GetString(win1251Bytes);
                */
                string[] packages = data.Split(new string[] { "-- - --" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string command in packages)
                {
                    MySql.Data.MySqlClient.MySqlCommand uploaderCmd = new MySql.Data.MySqlClient.MySqlCommand(command, msqConn);
                    uploaderCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); frez = false; }

            if (msqConn.State == System.Data.ConnectionState.Open)
                msqConn.Close();


            return frez;
        }

        public static string Sync(DataLayoutCollection ds, Hashtable connect)
        {
            MySql.Data.MySqlClient.MySqlConnectionStringBuilder msqConnStr = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder();

            msqConnStr.UserID = connect["UserID"].ToString();
            msqConnStr.Password = connect["Password"].ToString();
            msqConnStr.Database = connect["Database"].ToString();
            msqConnStr.Server = connect["Server"].ToString();

            MySql.Data.MySqlClient.MySqlConnection msqConn = new MySql.Data.MySqlClient.MySqlConnection(msqConnStr.ToString());


            DataTable rdt = new DataTable();
            List<string> data = new List<string>();
            string currentTableName = string.Empty;
            List<int> newRowIndexes = new List<int>();
            List<int> existedRowIndexes = new List<int>();
            int rowIdx = 0;
            DataRow[] rows = null;

            //data.Add("SET NAMES cp1251;");
            // get indexes
            Dictionary<string, string> tableIndexesAdd = ApplicationConfiguration.Instance.GetTableDictValueByPath("mappingConfiguration.Indexes");
            foreach (excelDataView dtp in ds.GetDataControls())
            {
                rowIdx = 0;
                newRowIndexes.Clear();
                existedRowIndexes.Clear();
                currentTableName = GetReplacedTableName(dtp.dataLayout.TableName);

                // loop by new rows
                string exid = tableIndexesAdd[dtp.dataLayout.TableName];

                // startup comment
                data.Add(Environment.NewLine);
                data.Add("-- synchronization for table " + dtp.dataLayout.TableName);

               // data.Add(new corelib().getSqlCommandWithDataTable(dtp.Copy()));

                rdt = corelib.GetRemoteDataTable(string.Format("SELECT `{0}` FROM {1}", exid, currentTableName), connect);

                // looping by received rows
                foreach (DataRow cdrw in rdt.Rows)
                {
                    try
                    {
                        rows = dtp.IndependentDataLayout.Select(exid + " = '" + cdrw[exid] + "'");
                    }
                    catch { }

                    // in case if new rows - insert them
                    if (rows != null && rows.Length != 0)
                    {
                        // in case if existed rows - update them
                        //rows[0].Table.Rows.IndexOf(rows[0]);
                        existedRowIndexes.Add(int.Parse(cdrw[exid].ToString()));
                    }
                }


                foreach (DataRow cdrw in dtp.IndependentDataLayout.Rows)
                {
                    if (!existedRowIndexes.Contains(int.Parse(cdrw[exid].ToString())))
                        newRowIndexes.Add(int.Parse(cdrw[exid].ToString()));
                }

                /*
                foreach (DataRow cdrw in dtp.IndependentDataLayout.Rows)
                {
                    try
                    {
                        rows = rdt.Select(exid + " = '" + cdrw[exid] + "'");
                    }
                    catch { }
                    // in case if new rows - insert them
                    if (rows == null || rows.Length == 0)
                    {
                        newRowIndexes.Add(rowIdx);
                    }
                    else
                    {
                        // in case if existed rows - update them
                        //rows[0].Table.Rows.IndexOf(rows[0]);
                        existedRowIndexes.Add(rowIdx);
                    }

                    rowIdx++;
                }*/

                // insert
                if (newRowIndexes.Count != 0)
                {
                    data.Add("-- adding new records into table " + dtp.dataLayout.TableName);
                    dtp.SetSelectedRows(newRowIndexes.ToArray());
                    data.Add(new corelib().getSqlCommandWithDataTable(dtp, ApplicationConfiguration.Instance.GetValueByPath<string>("commandsConfiguration.insert"), true));
                }

                // update
                data.Add("-- - --");
                if (existedRowIndexes.Count != 0)
                {
                    data.Add("-- updating new records into table " + dtp.dataLayout.TableName);
                    dtp.SetSelectedRows(existedRowIndexes.ToArray());
                    data.Add(new corelib().getSqlCommandWithDataTable(dtp, ApplicationConfiguration.Instance.GetValueByPath<string>("commandsConfiguration.update"), true));
                }

                // removal tool
                data.Add("-- - --");
                data.Add("-- removing records into table " + dtp.dataLayout.TableName);
                foreach (DataRow rdrw in rdt.Rows)
                {
                    // if current (dtp) row doesn't contain EXID in remote table
                    // we'll delete it
                    if (dtp.dataLayout.Select(exid + " = '" + rdrw[exid] + "'").Length == 0)
                        data.Add(string.Format("DELETE FROM {0} WHERE {2} = '{1}';", currentTableName, rdrw[exid], exid));
                }
                data.Add("-- - --");
            }

            if (msqConn.State != ConnectionState.Closed)
                msqConn.Close();

            return string.Join(Environment.NewLine, data.ToArray());
        }

        public void Backup(string[] tablesToBackup, Hashtable connect)
        {
            string savePath = string.Format("{0}\\{1:dd-MM-yyyy}", Application.StartupPath + "\\backup", DateTime.Now);
            string saveFile = string.Format("{0}\\data_{1:HH-mm-ss}.bak", savePath, DateTime.Now);

            if (!System.IO.Directory.Exists(savePath))
                System.IO.Directory.CreateDirectory(savePath);

            string currentTableName = string.Empty;
            DataSet ds = new DataSet();
            foreach (string tableName in tablesToBackup)
            {
                currentTableName = GetReplacedTableName(tableName);
                ds.Tables.Add(corelib.GetRemoteDataTable("SELECT * FROM " + currentTableName, connect));
                ds.Tables[ds.Tables.Count - 1].TableName = currentTableName;
            }


            ds.WriteXml(saveFile);

        }
        
        public static string GetReplacedTableName(string dbTableName)
        {
            Dictionary<string, string> tableNameExchange = ApplicationConfiguration.Instance.GetTableDictValueByPath("mappingConfiguration.TableName");
            // if table page has to be chnaged --- replace it
            if (tableNameExchange.ContainsKey(dbTableName) && !string.IsNullOrEmpty(tableNameExchange[dbTableName]))
                dbTableName = tableNameExchange[dbTableName];

            return dbTableName;
        }

        public bool IsRowEmpty(object[] itemsArray)
        {
            int emptyCount = itemsArray.Length;
            foreach (object obj in itemsArray)
            {
                if (obj == null || obj.Equals(string.Empty))
                    emptyCount--;
            }
            return (emptyCount == 0);
        }
    }
}
