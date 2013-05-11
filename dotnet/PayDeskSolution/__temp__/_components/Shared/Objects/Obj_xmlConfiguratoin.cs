using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using components.Components.XmlDocumentParser;

namespace components.Shared.Objects
{
    /*
    public class ConfigPath {

        private Com_XmlDocumentParser xd;

        public ConfigPath(ref Com_XmlDocumentParser xmlDoc)
        {
            xd = xmlDoc;
        }

        public string GeneralConfigDirectory
        {
            get { return xd.ConfigDir; }
            set { xd.ConfigDir = value; }
        }

        public string ApplicationConfigDirectory
        {
            get { return xd.ConfigAppDir; }
            set { xd.ConfigAppDir = value; }
        }

        public string DefaultConfigDirectory
        {
            get { return xd.ConfigDefaultDir; }
            set { xd.ConfigDefaultDir = value; }
        }
    
    }
    */
    public class Obj_xmlConfiguratoin : Interfaces.IConfigurable
    {
        private Hashtable xmlHt;
        private Com_XmlDocumentParser xP;

        public Obj_xmlConfiguratoin()
        {
            XmlParser = new Com_XmlDocumentParser();
            //ConfigPath = new ConfigPath(ref xP);
            //UseVersionForConfiguration = false;
        }

        /* Properties */
        //public Com_XmlDocumentParser_Configuration XmlParserSettings { get { return xP.Settings; } set { xP.Settings = value; } }
        public Com_XmlDocumentParser XmlParser { get { return xP; } set { xP = value; } }
        public Hashtable Configuration { get { return xmlHt; } }
        //public ConfigPath ConfigPath { get; set; }
        //public Version ApplicationVersion { get { return XmlParser.DocumentVersion; } set { XmlParser.DocumentVersion = value; } }
        //public bool UseVersionForConfiguration { get; set; }

        #region IConfigurable Members

        public bool BindConfigData()
        {
            bool rezult = false;
            try
            {
                if (XmlParser.Settings.UseVersionForConfiguration)
                    this.xmlHt = XmlParser.GetXmlData(string.Format("{0}\\v.{1}.{2}", XmlParser.Settings.ApplicationConfigDirectory, XmlParser.Settings.DocumentVersion.Major, XmlParser.Settings.DocumentVersion.Minor));
                else
                    this.xmlHt = XmlParser.GetXmlData();
                rezult = true;
            }
            catch { }
            return rezult;
        }

        public bool BindConfigData(bool merge)
        {
            bool rezult = false;
            try
            {
                this.xmlHt = XmlParser.GetXmlData(merge);
                rezult = true;
            }
            catch { }
            return rezult;
        }

        public object this[string path]
        {
            get
            {
                return XmlParser.GetValueByKey(this.xmlHt, path);
            }
        }

        public object GetValueByKey(string key)
        {
            return XmlParser.GetValueByKey(this.xmlHt, key);
        }

        public object GetValueByPath(string path)
        {
            return XmlParser.GetValueByPath(this.xmlHt, path);
        }

        public Hashtable GetValueByKey(string key, string wrapper)
        {
            object o = GetValueByKey(key);
            Hashtable wrappedO = new Hashtable();
            wrappedO.Add(wrapper, o);
            return wrappedO;
        }

        public Hashtable GetValueByPath(string path, string wrapper)
        {
            object o = GetValueByPath(path);
            Hashtable wrappedO = new Hashtable();
            wrappedO.Add(wrapper, o);
            return wrappedO;
        }

        public T GetValueByKey<T>(string key)
        {
            object data = GetValueByKey(key);
            T rez = default(T);
            try
            {
                rez = (T)data;
            }
            catch { }
            return rez;
        }

        public T GetValueByPath<T>(string path)
        {
            return GetValueByPath<T>(path, System.Globalization.NumberStyles.Any);
        }

        public T GetValueByPath<T>(string path, System.Globalization.NumberStyles numFormat)
        {
            object data = GetValueByPath(path);
            T rez = default(T);
            try
            {
                rez = Lib.TypeConv.ConvertTo<T>(data, numFormat);
            }
            catch { }
            return rez;
        }

        public void SetValueByPath(string path, object value)
        {
            XmlParser.SetValueByPath(this.Configuration, path, value);
        }

        public bool StoreConfigData()
        {
            if (XmlParser.Settings.UseVersionForConfiguration)
                return XmlParser.SetXmlData(this.xmlHt, string.Format("{0}\\v.{1}.{2}", XmlParser.Settings.ApplicationConfigDirectory, XmlParser.Settings.DocumentVersion.Major, XmlParser.Settings.DocumentVersion.Minor));
            else
                return XmlParser.SetXmlData(this.xmlHt, XmlParser.Settings.ApplicationConfigDirectory);
        }

        #endregion
    }
}
