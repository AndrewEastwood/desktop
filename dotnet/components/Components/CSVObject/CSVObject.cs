using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.Windows.Forms;

namespace components.Components.CSVObject
{
    public class CSVObject
    {

        public List<DataTable> GetObjectsByPath(string reportDirectory)
        {
            string[] files = System.IO.Directory.GetFiles(reportDirectory, "*.csv");
            List<DataTable> objects = new List<DataTable>();
            foreach (string singleFilePath in files)
                objects.Add(this.GetObjectByPath(singleFilePath));

            return objects;
        }

        public DataTable GetObjectByPath(string path)
        {
            DataTable data = new DataTable(System.IO.Path.GetFileNameWithoutExtension(path));
            string dataLine = "";
            int lineIndex = 0;
            List<string> dataArray = new List<string>();
            using (System.IO.TextReader txtRd = new System.IO.StreamReader(path))
            {
                while ((dataLine = txtRd.ReadLine()) != null)
                {
                    dataArray.Clear();

                    // if index == 0 it means that there are filed headers
                    dataArray.AddRange(dataLine.Split( new string[] { "\",\"" }, StringSplitOptions.None));
                    if (lineIndex == 0)
                    {
                        foreach (string dataArrayValue in dataArray)
                            data.Columns.Add(dataArrayValue.Trim('"'));
                        lineIndex++;
                        continue;
                    }

                    // data import
                    if (dataArray.Count == data.Columns.Count)
                    {
                        List<string> _rawItems = new List<string>();

                        foreach (string dataArrayValue in dataArray)
                            _rawItems.Add(dataArrayValue.Trim('"'));

                        data.Rows.Add(_rawItems.ToArray());

                    }

                    lineIndex++;
                }
            }


            return data;
        }

        public BindingSource GetBindingReportsByPath(string path)
        {
            List<DataTable> objects = GetObjectsByPath(path);

            Dictionary<string, DataTable> vui = new Dictionary<string, DataTable>();

            foreach (DataTable obj in objects)
                vui.Add(obj.TableName, obj);

            BindingSource bs = new BindingSource();
            bs.DataSource = vui;

            return bs;
        }

        public void BindDataSource(ref Control ctrl, string path)
        {
            
            
        }

        public void ExportToFile(DataTable table, string filePath)
        {
            var result = new StringBuilder();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                result.Append("\"" + table.Columns[i].ColumnName + "\"");
                result.Append(i == table.Columns.Count - 1 ? "\n" : ",");
            }

            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    result.Append("\"" + row[i].ToString() + "\"");
                    result.Append(i == table.Columns.Count - 1 ? "\n" : ",");
                }
            }
            System.IO.File.WriteAllText(filePath, result.ToString());
        }
    }
}
