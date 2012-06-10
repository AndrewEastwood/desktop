using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace mpwsDBU
{
    public class DataLayoutCollection : IDictionary<string, excelDataView>
    {
        private Dictionary<string, excelDataView> _layout = new Dictionary<string, excelDataView>();

        public DataSet GetAllData()
        {
            DataSet ds=new DataSet();
            foreach (KeyValuePair<string, excelDataView> layoutItem in _layout)
                ds.Tables.Add(layoutItem.Value.dataLayout);

            return ds;
        }

        public excelDataView[] GetDataControls()
        {
            List<excelDataView> xdv = new List<excelDataView>();

            foreach (KeyValuePair<string, excelDataView> dl in _layout)
            {
                xdv.Add(dl.Value);
            }

            return xdv.ToArray();
        }

        public DataTable GetDataTable(string tableName)
        {
            return _layout[tableName].dataLayout;
        }

        public List<string> GetDataTableNames()
        {
            List<string> dtn = new List<string>();

            foreach (KeyValuePair<string, excelDataView> dl in _layout)
                dtn.Add(dl.Value.dataLayout.TableName);

            return dtn;
        }

        public void AddData(string id, DataTable dt)
        {
            excelDataView xdvI = new excelDataView(dt, this);

            xdvI.Dock = System.Windows.Forms.DockStyle.Left;

            _layout.Add(id, xdvI);
        }

        public void AddData(DataTable dt)
        {
            _layout.Add(dt.TableName, new excelDataView(dt, this));
        }

        public void AddRange(DataSet ds)
        {
            if (ds == null)
                return;

            foreach (DataTable dt in ds.Tables)
                AddData(dt);
        }

        #region IDictionary<string,excelDataView> Members

        public void Add(string key, excelDataView value)
        {
            _layout.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _layout.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, out excelDataView value)
        {
            throw new NotImplementedException();
        }

        public ICollection<excelDataView> Values
        {
            get { throw new NotImplementedException(); }
        }

        public excelDataView this[string key]
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

        #endregion

        #region ICollection<KeyValuePair<string,excelDataView>> Members

        public void Add(KeyValuePair<string, excelDataView> item)
        {
            _layout.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _layout.Clear();
        }

        public bool Contains(KeyValuePair<string, excelDataView> item)
        {
            return this._layout.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, excelDataView>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _layout.Count; }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(KeyValuePair<string, excelDataView> item)
        {
            return _layout.Remove(item.Key);
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,excelDataView>> Members

        public IEnumerator<KeyValuePair<string, excelDataView>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
