using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace components.Components.ExcelDataWorker
{
    public class DataLayoutCollection : IDictionary<string, DataTable>
    {
        private Dictionary<string, DataTable> _layout = new Dictionary<string, DataTable>();

        public void AddRange(DataSet dataSet)
        {
            for (int i = 0; i < dataSet.Tables.Count; i++)
                Add(dataSet.Tables[i]);
        }

        public void Add(DataTable value)
        {
            Add(value.TableName, value);
        }

        public void Add(string key, DataTable value)
        {
            _layout[key] = value.Copy();
        }

        public bool ContainsKey(string key)
        {
            return _layout.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return _layout.Keys; }
        }

        public bool Remove(string key)
        {
            return _layout.Remove(key);
        }

        public bool TryGetValue(string key, out DataTable value)
        {
            return _layout.TryGetValue(key, out value);
        }

        public ICollection<DataTable> Values
        {
            get { return _layout.Values; }
        }

        public DataTable this[string key]
        {
            get
            {
                return _layout[key];
            }
            set
            {
                _layout[key] = value;
            }
        }

        public void Add(KeyValuePair<string, DataTable> item)
        {
            _layout.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _layout.Clear();
        }

        public bool Contains(KeyValuePair<string, DataTable> item)
        {
            if (_layout[item.Key] != null && _layout[item.Key].Equals(item.Value))
                return true;
            return false;
        }

        public void CopyTo(KeyValuePair<string, DataTable>[] array, int arrayIndex)
        {
            throw new Exception("Unsupported method");
        }

        public int Count
        {
            get { return Values.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, DataTable> item)
        {
            if (this.Contains(item))
                return this.Remove(item.Key);
            return false;
        }

        public IEnumerator<KeyValuePair<string, DataTable>> GetEnumerator()
        {
            return _layout.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _layout.GetEnumerator();
        }
    }

}
