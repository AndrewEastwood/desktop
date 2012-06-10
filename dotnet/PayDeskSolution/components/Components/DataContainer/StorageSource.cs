using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;

namespace components.Components.DataContainer
{
    public class StorageSource
    {
        Dictionary<string, DataSourceItem> storage;

        public StorageSource()
        {
            storage = new Dictionary<string,DataSourceItem>();
        }

        public StorageSource(DataSet ds)
            : this()
        {
            foreach (DataTable dt in ds.Tables)
                this.storage[dt.TableName] = new DataSourceItem(dt);
        }

        /* access */

        public DataSourceItem this[string dataKey]
        {
            get { return GetDataObject(dataKey); }
            set { Add(value); }
        }

        /* operations */


        public void Add(string sourceName, Hashtable properties)
        {
            DataTable dt = new DataTable();

            foreach (DictionaryEntry de in properties)
                dt.ExtendedProperties[de.Key] = de.Value;

            Add(dt);
        }

        public void Add(DataTable dt)
        {
            DataSourceItem dsi = new DataSourceItem(dt);

            this.storage.Add(dsi.Name, dsi);
        }

        public void Add(DataSourceItem ds)
        {
            if (this.storage.ContainsKey(ds.Name))
                this.storage[ds.Name] = ds;
            else
                this.storage.Add(ds.Name, ds);
        }

        public void AddRange(DataSourceItem[] ds)
        {
            foreach (DataSourceItem dsi in ds)
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
            this.storage[dataKey] = new DataSourceItem();
        }

        public void Truncate()
        {
            this.Truncate(Shared.Enums.Enu_SourceEnums.pdDataItemType.Any);
        }

        /* non typed (all) objects */

        public DataSet GetDataSet()
        {
            return this.GetDataSet(components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Any);
        }

        public DataSourceItem GetDataObject(string dataKey)
        {
            return this.storage[dataKey];
        }

        public DataTable GetDataItemByName(string dataKey)
        {
            return this.storage[dataKey].Source;
        }

        /* strong typed data objects */

        public DataSet GetDataSet(components.Shared.Enums.Enu_SourceEnums.pdDataItemType dataType)
        {
            DataSet ds = new DataSet();

            foreach (KeyValuePair<string, DataSourceItem> de in this.storage)
                if (dataType == de.Value.SourceType || dataType == components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Any)
                    ds.Tables.Add(de.Value.Source);

            return ds;
        }

        public void Remove(components.Shared.Enums.Enu_SourceEnums.pdDataItemType dataType)
        {
            foreach (KeyValuePair<string, DataSourceItem> de in this.storage)
                if (dataType == de.Value.SourceType || dataType == components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Any)
                    this.storage.Remove(de.Key);
        }

        public void Truncate(components.Shared.Enums.Enu_SourceEnums.pdDataItemType dataType)
        {
            foreach (KeyValuePair<string, DataSourceItem> de in this.storage)
                if (dataType == de.Value.SourceType || dataType == components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Any)
                    this.storage[de.Key] = new DataSourceItem();
        }
    }
}
