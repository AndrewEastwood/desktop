using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Excel;
using System.Collections;
using components;
using components.Public;

namespace mpwsDBU
{
    public partial class Form1 : Form
    {

        //private DataSet mpwsData;
        //private Dictionary<string, excelDataView> mpwsPages;
        private DataLayoutCollection dataStore;

        public Form1()
        {
            InitializeComponent();
            //mpwsPages = new Dictionary<string, excelDataView>();
            dataStore = new DataLayoutCollection();
        }

        private void menuStrip_main_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            generalMenuHandler(e.ClickedItem, string.Empty);
        }


        private void generalMenuHandler(ToolStripItem clickedItem, string layoutName)
        {
            if (clickedItem.Tag == null)
                return;

            // choosing required action
            switch (clickedItem.Tag.ToString())
            {
                case "file_open":
                    {
                        file_open(string.Empty);
                        break;
                    }
                case "file_reopen":
                    {
                        file_open(openFileDialog1.FileName);
                        break;
                    }
                case "tools_options":
                    {
                        tools_settings();
                        break;
                    }
                case "tools_sqlpreview":
                    {
                        if (dataStore.Count > 0)
                        {
                            lib.WaitWindow ww = new lib.WaitWindow();
                            ww.Show();
                            string data = new corelib().getSqlForAllTables(this.dataStore);
                            ww.Close();
                            ww.Dispose();
                            tools_sqlpreview(data);
                        }
                        else
                            MessageBox.Show("Use Open file menu for load data");
                        break;
                    }
                case "tools_upload":
                    {
                        
                        if (MessageBox.Show("Do you really want to upload all data?", "", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                            return;

                        if (dataStore.Count > 0)
                        {
                            lib.WaitWindow ww = new lib.WaitWindow();
                            ww.Show();
                            string data = new corelib().getSqlForAllTables(this.dataStore);
                            ww.Close();
                            ww.Dispose();
                            tools_upload(data);
                        }
                        else
                            MessageBox.Show("Use Open file menu for load data");
                        break;
                    }

                    /**/

                case "insert_selected__":
                    {/*
                        bool tmpMode = Properties.Settings.Default.uploadMode_UseRows;
                        Properties.Settings.Default.uploadMode_UseRows = true;
                        string tmpUpload = Properties.Settings.Default.uploadMode;
                        Properties.Settings.Default.uploadMode = corelib.UPLOAD_MODE_INSERT;

                        //tools_sqlpreview();

                        if (MessageBox.Show("Insert selected rows into \"" + Properties.Settings.Default.connect_db + "\" ?", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                            tools_upload(new corelib().getSqlCommandWithDataTable(dataStore[layoutName]));

                        Properties.Settings.Default.uploadMode = tmpUpload;
                        Properties.Settings.Default.uploadMode_UseRows = tmpMode;*/
                        break;
                    }
                case "update_selected__":
                    {/*
                        bool tmpMode = Properties.Settings.Default.uploadMode_UseRows;
                        Properties.Settings.Default.uploadMode_UseRows = true;
                        string tmpUpload = Properties.Settings.Default.uploadMode;
                        Properties.Settings.Default.uploadMode = corelib.UPLOAD_MODE_UPDATE;

                        //tools_sqlpreview();

                        if (MessageBox.Show("Update selected rows in \"" + Properties.Settings.Default.connect_db + "\" ?", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                            tools_upload(new corelib().getSqlCommandWithDataTable(dataStore[layoutName]));

                        Properties.Settings.Default.uploadMode = tmpUpload;
                        Properties.Settings.Default.uploadMode_UseRows = tmpMode;*/
                        break;
                    }
                case "delete_selected__":
                    {/*
                        bool tmpMode = Properties.Settings.Default.uploadMode_UseRows;
                        Properties.Settings.Default.uploadMode_UseRows = true;
                        string tmpUpload = Properties.Settings.Default.uploadMode;
                        Properties.Settings.Default.uploadMode = corelib.UPLOAD_MODE_DELETE;

                        //tools_sqlpreview();

                        if (MessageBox.Show("Delete selected rows from \"" + Properties.Settings.Default.connect_db + "\" ?", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                            tools_upload(new corelib().getSqlCommandWithDataTable(dataStore[layoutName]));

                        Properties.Settings.Default.uploadMode = tmpUpload;
                        Properties.Settings.Default.uploadMode_UseRows = tmpMode;*/
                        break;
                    }
                case "tools_sync":
                    {
                        // load xls data file
                        // use EXID from product matching
                        // logial behaviour:
                        // get all EXIDs from remote server
                        // run update from loaded new products
                        // with removing appropriate EXID from EXIDs list
                        // All items which are in the list run removal tool
                        if (MessageBox.Show("Do you really want to synchoronize all data?", "", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                            return;

                        if (dataStore.Count > 0)
                        {
                            lib.WaitWindow ww = new lib.WaitWindow();
                            ww.Show();
                            string data = corelib.Sync(dataStore, this.getConnectionCfg());
                            ww.Close();
                            ww.Dispose();
                            tools_upload(data);
                        }
                        else
                            MessageBox.Show("Use Open file menu for load data");

                        break;
                    }
                case "tools_syncpreview":
                    {
                        if (dataStore.Count > 0)
                        {
                            lib.WaitWindow ww = new lib.WaitWindow();
                            ww.Show();
                            string data = corelib.Sync(dataStore, this.getConnectionCfg());
                            ww.Close();
                            ww.Dispose();
                            tools_sqlpreview(data);
                        }
                        else
                            MessageBox.Show("Use Open file menu for load data");
                        break;
                    }
                case "tools_backup":
                    {
                        if (dataStore.Count > 0)
                        {
                            lib.WaitWindow ww = new lib.WaitWindow();
                            ww.Show();
                            new corelib().Backup(dataStore.GetDataTableNames().ToArray(), this.getConnectionCfg());
                            ww.Close();
                            ww.Dispose();
                            MessageBox.Show("Data has been saved successful");
                        }
                        else
                            MessageBox.Show("Use Open file menu for load data");
                        break;
                    }
                case "tools_restore":
                    {
                        tools_restore();
                        break;
                    }
            }
        }

        private void file_open(string filePath)
        {
            if (filePath == string.Empty)
                if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    filePath = toolStripStatusLabel_status.Text = openFileDialog1.FileName;
                else
                    return;

            // opening
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = null;
            if (System.IO.Path.GetExtension(filePath).ToLower().Contains(".xls"))
                //1. Reading from a binary Excel file ('97-2003 format; *.xls)
                excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
            else
                //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            //3. DataSet - The result of each spreadsheet will be created in the result.Tables
            //4. DataSet - Create column names from first row
            excelReader.IsFirstRowAsColumnNames = true;
            //-mpwsData = excelReader.AsDataSet();

            dataStore.Clear();
            dataStore.AddRange(excelReader.AsDataSet());

            if (dataStore.Count == 0)
            {
                MessageBox.Show(excelReader.ExceptionMessage);
                return;
            }

            // will add tabs as mpwsData contains
            TabPage tp = null;

            //-mpwsPages = new Dictionary<string, excelDataView>();

            // clearing all pages
            this.tabControl1.TabPages.Clear();
            int i = 0;
            foreach( excelDataView item in dataStore.GetDataControls() )
            {
                // new tab page
                tp = new TabPage();
                tp.Name = "tabPage" + ++i;
                tp.Text = item.dataLayout.TableName;
                item.Dock = DockStyle.Fill;
                item.MenuHandler = this.generalMenuHandler;
                tp.Controls.Add(item);

                this.tabControl1.TabPages.Add(tp);
            }


            /*
                try
                {
                    if (mpwsData.Tables.Count > 0)
                    {
                        dataGridView_preview.DataSource = mpwsData.Tables[0];
                    }
                }
                catch { }
            */

            //5. Data Reader methods
            /*
            while (excelReader.Read())
            {
                //excelReader.GetInt32(0);
            }
            */

            //6. Free resources (IExcelDataReader is IDisposable)
            excelReader.Close();

            stream.Close();
            stream.Dispose();
        }

        private void tools_sqlpreview(string data)
        {
            sqlPreview sqlPrw = new sqlPreview();
            sqlPrw.AddSQLLine(data);
            sqlPrw.ShowDialog();
            sqlPrw.Dispose();
        }


        private void tools_restore()
        {
            try
            {
                string filePath = string.Empty;
                if (openFileDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    filePath = toolStripStatusLabel_status.Text = openFileDialog2.FileName;
                else
                    return;

                DataSet ds = new DataSet();
                ds.ReadXml(filePath);

                DataLayoutCollection __dlc = new DataLayoutCollection();
                __dlc.AddRange(ds);



                string upmode = ApplicationConfiguration.Instance.GetValueByPath<string>("formatConfiguration.uploadMode");
                bool truc = ApplicationConfiguration.Instance.GetValueByPath<bool>("formatConfiguration.alwaysTruncate");

                ApplicationConfiguration.Instance.SetValueByPath("formatConfiguration.uploadMode", corelib.UPLOAD_MODE_INSERT);
                ApplicationConfiguration.Instance.SetValueByPath("formatConfiguration.alwaysTruncate", true); 
                //Properties.Settings.Default.uploadMode = corelib.UPLOAD_MODE_INSERT;
                //Properties.Settings.Default.uploadMode_AlwaysTruncate = true;

                tools_upload(new corelib().getSqlForAllTables(__dlc));

                ApplicationConfiguration.Instance.SetValueByPath("formatConfiguration.uploadMode", upmode);
                ApplicationConfiguration.Instance.SetValueByPath("formatConfiguration.alwaysTruncate", truc); 
                //Properties.Settings.Default.uploadMode = upmode;
                //Properties.Settings.Default.uploadMode_AlwaysTruncate = truc;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Restore Error");
            }
        }

        private void tools_settings()
        {

            //new TestAppSettings().ShowDialog();

            
            settings ss = new settings();
            ss.ShowDialog();
            ss.Dispose();
            
        }

        private void tools_upload(string data)
        {
            if (corelib.Upload(data, getConnectionCfg(), dataStore))
                MessageBox.Show("Data has been uploaded successful");
            else
                MessageBox.Show("Data upload error");
        }


        private Hashtable getConnectionCfg()
        {
            Hashtable connectCfg = new Hashtable();

            connectCfg["UserID"] = ApplicationConfiguration.Instance.GetValueByPath<string>("connectionConfiguration.login");
            connectCfg["Password"] = ApplicationConfiguration.Instance.GetValueByPath<string>("connectionConfiguration.password");
            connectCfg["Database"] = ApplicationConfiguration.Instance.GetValueByPath<string>("connectionConfiguration.database");
            connectCfg["Server"] = ApplicationConfiguration.Instance.GetValueByPath<string>("connectionConfiguration.server");

            return connectCfg;
        }



    }
}
