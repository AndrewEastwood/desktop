using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace components.Shared.Interfaces
{
    public interface IConfigurable
    {
        bool LoadConfigurationData();
        bool SaveConfigurationData();
        object GetValueByKey(string key);
        Hashtable GetValueByKey(string key, string wrapper);
        object GetValueByPath(string path);
        Hashtable GetValueByPath(string path, string wrapper);
        T GetValueByKey<T>(string key);
        T GetValueByPath<T>(string path);
        void SetValueByPath(string path, object value);
        object this[string path] { get; }
    }

}
