using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;

namespace components.Components.DataReader
{
    public class Com_DataReader
    {
        public Hashtable GetData(string path)
        {
            Hashtable ht = new Hashtable();
            string[] lineArray = new string[0];

            using (System.IO.StreamReader sTr = System.IO.File.OpenText(path))
            {
                lineArray = sTr.ReadToEnd().Replace("\\\r\n", " ").Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                sTr.Close();
            }

            // transform data
            foreach (string sLine in lineArray)
            {
                if (sLine[0] == '#')
                    continue;
                if (sLine.Contains("=")) try
                    {
                        string[] lineArr = sLine.Split(new char[] { '=' }, 2);
                        ht[lineArr[0]] = lineArr[1];
                    }
                    catch { }
            }

            return ht;
        }

        public DataTable GetTableData(string path, string keyFiledName, string valueFieldName)
        {
            DataTable dt = new DataTable();
            Hashtable ht = GetData(path);
            dt.Columns.Add(new DataColumn("IDX"));
            dt.Columns.Add(new DataColumn(keyFiledName));
            dt.Columns.Add(new DataColumn(valueFieldName));

            int idx = 0;
            foreach (DictionaryEntry de in ht)
            {
                dt.Rows.Add(++idx, de.Key, de.Value);
            }

            return dt;
        }

        public Hashtable GetIndexedPropertyKeys(string path)
        {
            Hashtable ht = GetData(path);
            Hashtable idxKeys = new Hashtable();
            int idx = 0;
            foreach (DictionaryEntry de in ht)
                idxKeys[de.Key] = idx++;
            return idxKeys;
        }

        public Dictionary<string, int> GetIndexedPropertyKeysDict(string path)
        {
            string[] lineArray = new string[0];
            Dictionary<string, int> idxKeys = new Dictionary<string, int>();

            using (System.IO.StreamReader sTr = System.IO.File.OpenText(path))
            {
                lineArray = sTr.ReadToEnd().Replace("\\\r\n", " ").Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                sTr.Close();
            }

            // transform data
            int idx = 0;
            foreach (string sLine in lineArray)
            {
                if (sLine[0] == '#')
                    continue;
                if (sLine.Contains("=")) try
                    {
                        string[] lineArr = sLine.Split(new char[] { '=' }, 2);
                        idxKeys.Add(lineArr[0], idx++);
                    }
                    catch { }
            }

            return idxKeys;
        }
    }
}
