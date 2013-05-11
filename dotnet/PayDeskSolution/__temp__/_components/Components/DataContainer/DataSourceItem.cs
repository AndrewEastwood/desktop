using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;

namespace components.Components.DataContainer
{
    public class DataSourceItem : DataStructureItem
    {
        DataTable data;
        components.Shared.Enums.Enu_SourceEnums.pdDataItemType type;

        public DataSourceItem()
        {
            this.type = components.Shared.Enums.Enu_SourceEnums.pdDataItemType.None;
            data = new DataTable();
        }

        public DataSourceItem(DataTable dt)
            : this()
        {
            base.Properties = dt.ExtendedProperties;
            this.data = dt;
            base.Name = dt.TableName;
        }

        public DataSourceItem(DataTable dt, string tableName)
            : this()
        {
            base.Properties = dt.ExtendedProperties;
            this.data = dt;
            base.Name = tableName;
            dt.TableName = tableName;
        }

        public DataSourceItem(string tableName)
            : this()
        {
            DataTable dt = new DataTable(tableName);
            this.data = dt;
            base.Name = dt.TableName;
        }

        public DataSourceItem(Hashtable props)
            : this(new DataTable())
        {
            base.Properties = props;
        }

        public DataTable Source
        {
            get { return this.data; }
            set { this.data = value; }
        }

        public components.Shared.Enums.Enu_SourceEnums.pdDataItemType SourceType { get { return this.type; } set { this.type = value; } }
    }
}
