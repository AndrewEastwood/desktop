using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace components.Components.XmlDocumentParser
{

    public class Com_XmlDocumentParser_Configuration : Public.DefaultComponentSettings
    {
        public string GeneralConfigDirectory { get; set; }
        public string ApplicationConfigDirectory { get; set; }
        public string DefaultConfigDirectory { get; set; }
        public bool CheckForDocumentVersion { get; set; }
        public Version DocumentVersion { get; set; }
        public bool UseVersionForConfiguration { get; set; }
    }

    public class Com_XmlDocumentParser : Public.DefaultComponent
    {
        /* constants */
        private const string DS = "//";
        private const string EXT = ".xml";
        /* internal variables */
        //private string cfgDir;
        //private string cfgDefDir;
        //private string cfgAppDir;
        public HashObject.Com_HashObject Commnets { get; set; }
        //public Version DocumentVersion { get; set; }
        //public bool CheckForDocumentVersion { get; set; }

        public Com_XmlDocumentParser_Configuration Settings { get; set; }

        public Com_XmlDocumentParser()
            : base("1.5", "XmlDocumentParser")
        {
            Settings = new Com_XmlDocumentParser_Configuration();

            Settings.GeneralConfigDirectory = "config";
            Settings.DefaultConfigDirectory = "default";
            Settings.ApplicationConfigDirectory = string.Empty;
            Settings.DocumentVersion = base.Version;
            Settings.CheckForDocumentVersion = false;
            //cfgDir = "config";
            //cfgDefDir = "default";
            //cfgAppDir = string.Empty;
            Commnets = new HashObject.Com_HashObject();
        }
        public Com_XmlDocumentParser(string configDir)
            : this()
        {
            Settings.GeneralConfigDirectory = configDir;
        }

        /* Methods */
        public Hashtable GetXmlData()
        {
            return GetXmlData(this.Settings.GeneralConfigDirectory + DS + this.Settings.ApplicationConfigDirectory, this.Settings.GeneralConfigDirectory + DS + this.Settings.DefaultConfigDirectory, true);
        }
        public Hashtable GetXmlData(bool merge)
        {
            return GetXmlData(this.Settings.GeneralConfigDirectory + DS + this.Settings.ApplicationConfigDirectory, this.Settings.GeneralConfigDirectory + DS + this.Settings.DefaultConfigDirectory, merge);
        }
        public Hashtable GetXmlData(string appConfig)
        {
            return GetXmlData(appConfig, this.Settings.DefaultConfigDirectory, false);
        }
        public Hashtable GetXmlData(string configAppDir, string configDefDir, bool merge)
        {
            if (!Directory.Exists(configDefDir))
                Directory.CreateDirectory(configDefDir);


            string[] xmlCfg = System.IO.Directory.GetFiles(configAppDir, "*.xml");
            string[] xmlCfgDef = System.IO.Directory.GetFiles(configDefDir, "*.xml");
            // application files
            Dictionary<string, string> ovFiles = new Dictionary<string, string>();
            // default files
            Dictionary<string, string> dfFiles = new Dictionary<string, string>();
            // collecting app files
            for (int i = 0; i < xmlCfg.Length; i++)
                ovFiles[System.IO.Path.GetFileNameWithoutExtension(xmlCfg[i])] = xmlCfg[i];
            // collecting default files
            for (int i = 0; i < xmlCfgDef.Length; i++)
                dfFiles[System.IO.Path.GetFileNameWithoutExtension(xmlCfgDef[i])] = xmlCfgDef[i];

            Hashtable data = new Hashtable();
            string path = string.Empty;
            Hashtable cdata = new Hashtable();
            foreach (KeyValuePair<string, string> pathItem in dfFiles)
            {
                try
                {
                    // if app files has the same file as in the default
                    // we'll override it, otherways we'll add it to the collection
                    if (ovFiles.ContainsKey(pathItem.Key) && merge)
                    {
                        // overridding
                        path = ovFiles[pathItem.Key];
                        cdata = GetMergedXmlDataFromFile(path);
                    }
                    else
                    {
                        // simple adding
                        path = pathItem.Value;
                        cdata = GetXmlDataFromFile(path);
                    }

                    data[GetRootKey(cdata)] = GetDataContent(cdata);
                }
                catch { }
            }


            // references update
            // this.ReferenceUpdate(ref data);

            return data;
        }

        public void ReferenceUpdate(ref Hashtable dataFull, Hashtable trackingData, Stack trackingPath)
        {
            foreach (DictionaryEntry refItem in trackingData)
            {
                if (refItem.Value.GetType() == typeof(Hashtable) )
                    try
                    {
                        trackingPath.Push(refItem.Key + ".");
                        this.ReferenceUpdate(ref dataFull, (Hashtable)refItem.Value, trackingPath);
                        trackingPath.Pop();
                    }
                    catch { }
                else
                    if (refItem.Value.ToString().StartsWith("xref:"))
                    {
                        trackingPath.Push(refItem.Key);
                        object[] td = trackingPath.ToArray();
                        Array.Reverse(td);
                        SetValueByPath(dataFull, string.Concat(td).Trim('.'), this.GetValueByPath(dataFull, refItem.Value.ToString().Substring(5)));
                        ;// refItem.Value = this.GetValueByPath(dataFull, refItem.Value.ToString().Substring(5));
                        trackingPath.Pop();
                    }
            }
        }
        public void ReferenceUpdate(ref Hashtable dataFull)
        {
            this.ReferenceUpdate(ref dataFull, dataFull, new Stack());
        }
        
        public Hashtable GetXmlDataFromFile(string filePath)
        {
            Hashtable data = new Hashtable();
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                XmlTextReader _xreader = new XmlTextReader(fs);
                _xreader.WhitespaceHandling = WhitespaceHandling.All;
                data = ReadSection(_xreader);
                _xreader.Close();
                fs.Close();
            }
            return data;
        }
        public Hashtable GetMergedXmlDataFromFile(string filePath)
        {
            Hashtable data = new Hashtable();
            Hashtable block = GetMergedXmlDataFromFile(filePath, ref data);
            Hashtable ht = new Hashtable();
            string[] items = GetTopKeys(block);
            Array.Sort<string>(items);
            string rootKey = GetRootKey((Hashtable)block[items[items.Length - 1]]);

            ht = GetDataContent((Hashtable)block[items[items.Length - 1]]);
            for (int i = items.Length - 2; i >= 0; i--)
                ht = MergeData(ht, GetDataContent((Hashtable)block[items[i]]));

            Hashtable mergedData = new Hashtable();
            mergedData[rootKey] = ht;

            return mergedData;
        }
        public Hashtable GetMergedXmlDataFromFile(string filePath, ref Hashtable prevData)
        {
            XmlTextReader _xreader = new XmlTextReader(filePath);
            _xreader.WhitespaceHandling = WhitespaceHandling.All;
            Hashtable data = ReadSection(_xreader);
            _xreader.Close();

            object pcode = GetValueByKey(data, "parent");
            object pname = GetValueByKey(data, "name");
            object pfile = GetRootKey(data);

            // saving current data
            prevData[prevData.Count + "_" + pname] = data;

            // making path to the parent file
            string parentFilePath = this.Settings.GeneralConfigDirectory + DS + pcode + DS + pfile + EXT;

            // read data if this file exists
            if (System.IO.File.Exists(parentFilePath))
                GetMergedXmlDataFromFile(parentFilePath, ref prevData);

            //List<string> ovpr = GetOverridePath(data, ovp);
            return prevData;
        }
        public Hashtable ReadSection(XmlTextReader rd)
        {
            int i = 0;
            return ReadSection(rd, ref i);
        }
        public Hashtable ReadSection(XmlTextReader rd, ref int section)
        {
            return ReadSection(rd, ref section , string.Empty);
        }
        public Hashtable ReadSection(XmlTextReader rd, ref int section, string comment)
        {
            Hashtable cont = new Hashtable();
            while (rd.Read())
            {
                if (section != 0 && rd.NodeType == XmlNodeType.EndElement)
                {
                    section--;
                    return cont;
                }

                if (rd.NodeType == XmlNodeType.Comment)
                    comment = rd.Value;

                if (rd.NodeType != XmlNodeType.Element)
                    continue;

                switch (rd.Name)
                {
                    case "document":
                        {
                            bool readDoc = true;
                            if (this.Settings.CheckForDocumentVersion)
                                if (rd.GetAttribute("version") == string.Empty || !this.Settings.DocumentVersion.Equals(new Version(rd["version"])))
                                    readDoc = false;
                            if (readDoc)
                                cont[rd["name"]] = ReadSection(rd, ref section);
                            else
                                Console.WriteLine("Document version must be the same as you are attempting to load");
                            break;
                        }
                    case "section":
                        {
                            try
                            {
                                section++;
                                string sectionName = rd["name"];
                                cont[sectionName] = ReadSection(rd, ref section);
                                if (comment.Length != 0)
                                    Commnets.Add("section::" + sectionName, comment);
                            }
                            catch { }
                            break;
                        }
                    case "property":
                        {
                            try
                            {
                                if (rd.GetAttribute("value") != null)
                                    cont.Add(rd["name"], rd["value"]);

                                if (rd.GetAttribute("ref") != null)
                                    cont.Add(rd["name"], "xref:" + rd["ref"]);

                                if (comment.Length != 0)
                                    Commnets.Add("property::" + rd["name"], comment);
                            }
                            catch { }
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }

                comment = string.Empty;
            }
            return cont;
        }
        public string[] GetTopKeys(Hashtable dataItems)
        {
            List<string> rk = new List<string>();
            foreach (DictionaryEntry e in dataItems)
            {
                rk.Add(e.Key.ToString());
            }
            return rk.ToArray();
        }
        public Hashtable MergeData(Hashtable item1, Hashtable item2)
        {
            Hashtable mdata = item1;

            foreach (DictionaryEntry e in item2)
            {
                if (mdata.ContainsKey(e.Key))
                {
                    if (mdata[e.Key].GetType() != typeof(Hashtable))
                    {
                        mdata[e.Key] = e.Value;
                    }
                    if (mdata[e.Key].GetType() == typeof(Hashtable) && e.Value.GetType() == typeof(Hashtable))
                        mdata[e.Key] = MergeData((Hashtable)mdata[e.Key], (Hashtable)e.Value);
                }
                else
                {
                    mdata[e.Key] = e.Value;
                }
            }

            return mdata;
        }
        public Hashtable GetDataContent(Hashtable data)
        {
            return (Hashtable)data[GetRootKey(data)];
        }
        public string GetRootKey(Hashtable data)
        {
            string rk = string.Empty;
            foreach (DictionaryEntry e in data)
            {
                rk = e.Key.ToString();
                break;
            }
            return rk;
        }
        public bool CheckKey(Hashtable data, string key)
        {
            bool isset = false;
            foreach (DictionaryEntry e in data)
            {
                if (e.Key.ToString() == key)
                    isset = true;
                else if (e.Value.GetType() == typeof(Hashtable))
                    isset = CheckKey((Hashtable)e.Value, key);
                if (isset)
                    break;
            }
            return isset;
        }
        public object GetValueByKey(Hashtable data, string key)
        {
            bool wasSet = false;
            return GetValueByKey(data, key, ref wasSet);
        }
        public object GetValueByKey(Hashtable data, string key, ref bool wasSet)
        {
            object kv = new object();
            foreach (DictionaryEntry e in data)
            {
                if (e.Key.ToString() == key)
                {
                    kv = e.Value;
                    wasSet = true;
                }
                else if (e.Value.GetType() == typeof(Hashtable))
                    kv = GetValueByKey((Hashtable)e.Value, key, ref wasSet);

                if (wasSet)
                    break;
            }
            return kv;
        }
        public object GetValueByPath(Hashtable data, string path)
        {
            if (path == string.Empty)
                return DBNull.Value;

            string[] keys = path.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            Hashtable cr = (Hashtable)data.Clone();
            object res = new object();

            for (int i = 0; i < keys.Length - 1; i++)
            {
                if (!cr.ContainsKey(keys[i]))
                    return DBNull.Value;
                if (cr[keys[i]].GetType() != typeof(Hashtable) && i + 1 < keys.Length)
                    return DBNull.Value;
                if (cr[keys[i]].GetType() == typeof(Hashtable))
                    cr = (Hashtable)cr[keys[i]];
                else
                    return DBNull.Value;
            }

            if (cr.GetType() == typeof(Hashtable) && cr.ContainsKey(keys[keys.Length - 1]))
                res = cr[keys[keys.Length - 1]];

            return res;

        }

        public void SetValueByPath(Hashtable data, string path, object value)
        {
            Hashtable tmpTable = (Hashtable)data.Clone();
            Hashtable td = data;
            string[] keys = path.Split('.');
            int depth = 0;
            bool reset = false;
            bool isNewValue = true;

            // new value container
            Hashtable newValue = new Hashtable();

            do
            {
                reset = false;
                isNewValue = true;

                foreach (DictionaryEntry de in td)
                {
                    if (de.Key.ToString().Equals(keys[depth]))
                    {
                        depth++;

                        isNewValue = false;
                        if (depth == keys.Length)
                        {
                            td[keys[depth - 1]] = value;
                            reset = false;
                            break;
                        }
                        else
                        {
                            td = (Hashtable)(de.Value);
                            reset = true;
                        }
                    }

                    if (reset)
                        break;
                }

                if (isNewValue && keys.Length >= 1 && keys[0] != string.Empty)
                    CreateNewValuePath(ref data, keys, 0, value);

            }
            while (reset);

        }

        public void CreateNewValuePath(ref Hashtable data, string[] keys, int index, object value)
        {
            if (index + 1 < keys.Length)
            {
                Hashtable ht = new Hashtable();

                if (!data.ContainsKey(keys[index]) || !data[keys[index]].GetType().Equals(typeof(Hashtable)))
                    data[keys[index]] = ht;
                else
                    ht = (Hashtable)data[keys[index]];

                CreateNewValuePath(ref ht, keys, index + 1, value);
            }
            else
                data[keys[index]] = value;
        }

        /* writer */
        public bool SetXmlData(Hashtable data, string configAppDir)
        {
            bool state = true;

            XmlTextWriter xmlWr = null;
            if (data !=null)
                foreach (DictionaryEntry de in data)
                {
                    try
                    {

                        if (!Directory.Exists(this.Settings.GeneralConfigDirectory + DS + configAppDir))
                            Directory.CreateDirectory(this.Settings.GeneralConfigDirectory + DS + configAppDir);


                        xmlWr = new XmlTextWriter(this.Settings.GeneralConfigDirectory + DS + configAppDir + DS + de.Key + ".xml", Encoding.Default);
                        xmlWr.Formatting = Formatting.Indented;
                        xmlWr.Indentation = 4;

                        WriteDocument(xmlWr, de);
                    }
                    catch
                    {
                        state = false;
                    }

                    xmlWr.Close();
                }

            return state;
        }

        public void WriteDocument(XmlTextWriter xmlWr, DictionaryEntry de)
        {
            xmlWr.WriteStartDocument();
            // autocomment
            string comment = de.Key.ToString().ToLower().Replace("configuration", string.Empty) + " Configuration Data File" + "\r\n";
            comment += "do not modity anything inside the document section" + "\r\n";
            comment += "geretated by " + this.GetComponentInfo;
            xmlWr.WriteComment("\r\n" + comment + "\r\n");
            xmlWr.WriteStartElement("document");
            xmlWr.WriteAttributeString("name", de.Key.ToString());
            xmlWr.WriteAttributeString("version", this.Settings.DocumentVersion.ToString(2));
            WriteSection(xmlWr, string.Empty, (Hashtable)de.Value);
            xmlWr.WriteEndElement();
            xmlWr.WriteEndDocument();
        }

        public void WriteProperty(XmlTextWriter xmlWr, DictionaryEntry de)
        {
            if (Commnets.ContainsKey("property::" + de.Key))
                xmlWr.WriteComment(Commnets.GetValue<string>("property::" + de.Key));

            xmlWr.WriteStartElement("property");
            xmlWr.WriteAttributeString("name", de.Key.ToString());
            xmlWr.WriteAttributeString("value", de.Value == null ? string.Empty : de.Value.ToString());
            xmlWr.WriteEndElement();
        }

        public void WriteSection(XmlTextWriter xmlWr, string name, Hashtable innerData)
        {
            if (Commnets.ContainsKey("section::" + name))
                xmlWr.WriteComment(Commnets.GetValue<string>("section::" + name));

            if (name.Length != 0)
            {
                xmlWr.WriteStartElement("section");
                xmlWr.WriteAttributeString("name", name);
            }

            foreach (DictionaryEntry de in innerData)
            {
                try
                {
                    if (de.Value == null)
                        WriteProperty(xmlWr, de);
                    else
                        switch (de.Value.GetType().ToString())
                        {
                            case "System.Collections.Hashtable":
                                {
                                    WriteSection(xmlWr, de.Key.ToString(), (Hashtable)de.Value);
                                    break;
                                }
                            default:
                                {
                                    WriteProperty(xmlWr, de);
                                    break;
                                }
                        }
                }
                catch { }
            }

            //if (innerData.Count == 0)
            //    xmlWr.WriteValue(null);

            if (name.Length != 0)
                xmlWr.WriteFullEndElement();

            //xmlWr.WriteWhitespace("\r\n\r\n");
        }



        /* Properties */
        /*
        public string ConfigDir { set { this.cfgDir = value; } get { return this.cfgDir; } }
        public string ConfigDefaultDir { set { this.cfgDefDir = value; } get { return this.cfgDefDir; } }
        public string ConfigAppDir { set { this.cfgAppDir = value; } get { return this.cfgAppDir; } }
        */
    }
}
