using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace components.Shared.Interfaces
{
    public interface IConfigurable
    {
        #region Data Load
        bool LoadConfigurationData();
        bool SaveConfigurationData();
        bool ReloadConfigurationData();
        #endregion
        #region Value Selector
        object GetValueByKey(string key);
        object GetValueByPath(string path);
        Hashtable GetValueByKey(string key, string wrapper);
        Hashtable GetValueByPath(string path, string wrapper);
        T GetValueByKey<T>(string key);
        T GetValueByPath<T>(string path);
        T GetValueByPath<T>(string path, System.Globalization.NumberStyles numFormat);
        Hashtable GetTableHashValueByPath(string path, string keyName, string valueName);
        Hashtable GetTableHashValueByPath(string path);
        Dictionary<string, string> GetTableDictValueByPath(string path, string keyName, string valueName);
        Dictionary<string, string> GetTableDictValueByPath(string path);
        #endregion
        #region Value Modify
        object this[string path] { get; }
        void SetValueByPath(string path, object value);
        #endregion
    }

}
