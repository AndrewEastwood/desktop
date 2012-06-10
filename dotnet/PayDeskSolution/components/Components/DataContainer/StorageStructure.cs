using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;

namespace components.Components.DataContainer
{
    public class StorageStructure
    {
        Dictionary<string, DataStructureItem> storage;

        public StorageStructure()
        {
            storage = new Dictionary<string, DataStructureItem>();
        }

        public StorageStructure(DataSet ds)
            : this()
        {
            foreach (DataTable dt in ds.Tables)
                this.storage[dt.TableName] = new DataSourceItem(dt);
        }


        /* access */

        public object this[string dataKey, string propertyKey]
        {
            get { return GetDataObject(dataKey)[propertyKey]; }
            set { GetDataObject(dataKey)[propertyKey] = value; }
        }

        public DataStructureItem this[string dataKey]
        {
            get { return GetDataObject(dataKey); }
            set { Add(value); }
        }

        /* operations */

        public void Add(DataStructureItem ds)
        {
            this.storage.Add(ds.Name, ds);
        }

        public void AddRange(DataStructureItem[] ds)
        {
            foreach (DataStructureItem dsi in ds)
                Add(dsi);
        }

        public void Remove(string dataKey)
        {
            this.storage.Remove(dataKey);
        }

        public void Remove()
        {
            this.storage.Clear();
        }

        public void Truncate(string dataKey)
        {
            this.storage[dataKey] = new DataStructureItem();
        }

        public void Truncate()
        {
            foreach (KeyValuePair<string, DataStructureItem> pe in storage)
                this.storage[pe.Key] = new DataStructureItem();
        }

        /* non typed (all) objects */

        public DataStructureItem GetDataObject(string dataKey)
        {
            return this.storage[dataKey];
        }

    }
}
