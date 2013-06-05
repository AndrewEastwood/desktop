using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace components.Components.XmlDocumentParser
{

    public class Com_XmlDocumentParser_Configuration : Shared.Defaults.DefaultComponentSettings
    {
        /* constants */
        private const string DS = @"\";

        public string ConfigDirectoryPathGeneral { get; set; }
        public string ConfigDirectoryNameApplication { get; set; }
        public string ConfigDirectoryNameDefault { get; set; }
        public bool CheckForDocumentVersion { get; set; }
        public Version DocumentVersion { get; set; }
        public bool UseVersionForConfiguration { get; set; }
        public string VersionDirectoryFormat { get { return "{1}{0}v.{2}"; } }
        public string VersionFormat { get { return "v.{0}"; } }
        public bool TrackVersionConfiguration { get; set; }
        public bool MergeData { get; set; }


        /* full path deprecated * /
        /// <summary>
        /// Get full path of specific application config directory
        /// </summary>
        public string ConfigDirectoryPathApplication
        {
            get { return ConfigDirectoryPathGeneral + DS + ConfigDirectoryNameApplication; }
        }
        /// <summary>
        /// Get full path of default config directory
        /// </summary>
        public string ConfigDirectoryPathDefault
        {
            get { return ConfigDirectoryPathGeneral + DS + ConfigDirectoryNameDefault; }
        }*/
        /* full path + version directory */
        public string AutoPathApplicationConfigDirectory
        {
            get
            {
                string appPath = ConfigDirectoryNameApplication;
                if (UseVersionForConfiguration)
                    appPath = string.Format(VersionDirectoryFormat, DS, appPath, DocumentVersion.ToString());
                if (ConfigDirectoryNameApplication.Length != 0)
                    appPath = DS + appPath;
                return ConfigDirectoryPathGeneral + appPath;
            }
        }
        public string AutoPathDefaultConfigDirectory
        {
            get
            {
                if (ConfigDirectoryNameDefault.Length == 0)
                    return AutoPathApplicationConfigDirectory;

                string defPath = ConfigDirectoryNameDefault;
                if (UseVersionForConfiguration)
                    defPath = string.Format(VersionDirectoryFormat, DS, defPath, DocumentVersion.ToString());
                if (ConfigDirectoryNameDefault.Length != 0)
                    defPath = DS + defPath;
                return ConfigDirectoryPathGeneral + defPath;
            }
        }
    
        /* full path with version pattern */
        public string PatternFullPathApplicationConfigDirectory
        {
            get
            {
                string appPath = ConfigDirectoryNameApplication;
                if (UseVersionForConfiguration)
                    appPath = string.Format(VersionDirectoryFormat, DS, ConfigDirectoryNameApplication, VersionDirectoryFormat);
                return ConfigDirectoryPathGeneral + DS + appPath;
            }
        }
        public string PatternFullPathDefaultConfigDirectory
        {
            get
            {
                string appPath = ConfigDirectoryNameDefault;
                if (UseVersionForConfiguration)
                    appPath = string.Format(VersionDirectoryFormat, DS, ConfigDirectoryNameDefault, VersionDirectoryFormat);
                return ConfigDirectoryPathGeneral + DS + appPath;
            }
        }
    
    }

    public class Com_XmlDocumentParser : Shared.Defaults.DefaultComponent
    {
        /* constants */
        private const string DS = @"\";
        private const string EXT = ".xml";

        /* internal variables */
        //private string cfgDir;
        //private string cfgDefDir;
        //private string cfgAppDir;
        private Stack<string> docSectionPath;
        private Dictionary<string, string> sectionParents;
        public HashObject.Com_HashObject Commnets { get; set; }
        //public Version DocumentVersion { get; set; }
        //public bool CheckForDocumentVersion { get; set; }

        public Com_XmlDocumentParser_Configuration Settings { get; set; }

        public Com_XmlDocumentParser()
            : base("1.6.1", "XmlDocumentParser")
        {
            Settings = new Com_XmlDocumentParser_Configuration();

            Settings.ConfigDirectoryPathGeneral = "config";
            Settings.ConfigDirectoryNameDefault = "default";
            Settings.ConfigDirectoryNameApplication = string.Empty;
            Settings.DocumentVersion = base.Version;
            Settings.CheckForDocumentVersion = false;
            Settings.TrackVersionConfiguration = true;

            //cfgDir = "config";
            //cfgDefDir = "default";
            //cfgAppDir = string.Empty;
            Commnets = new HashObject.Com_HashObject();
            docSectionPath = new Stack<string>();
            sectionParents = new Dictionary<string, string>();
        }
        public Com_XmlDocumentParser(string configDir)
            : this()
        {
            Settings.ConfigDirectoryPathGeneral = configDir;
        }

        /* Reader */
        public Hashtable GetXmlData()
        {
            //return GetXmlData(this.Settings.GeneralConfigDirectory + DS + this.Settings.ApplicationConfigDirectory, this.Settings.GeneralConfigDirectory + DS + this.Settings.DefaultConfigDirectory, true);

            return GetXmlData(this.Settings.AutoPathApplicationConfigDirectory, this.Settings.AutoPathDefaultConfigDirectory, this.Settings.MergeData);
        }
        public Hashtable GetXmlData(bool merge)
        {
            //return GetXmlData(this.Settings.GeneralConfigDirectory + DS + this.Settings.ApplicationConfigDirectory, this.Settings.GeneralConfigDirectory + DS + this.Settings.DefaultConfigDirectory, merge);
            return GetXmlData(this.Settings.AutoPathApplicationConfigDirectory, this.Settings.AutoPathDefaultConfigDirectory, merge);
        }
        public Hashtable GetXmlData(string appConfig)
        {
            //return GetXmlData(appConfig, this.Settings.DefaultConfigDirectory, false);
            return GetXmlData(appConfig, this.Settings.AutoPathDefaultConfigDirectory, this.Settings.MergeData);
        }
        public Hashtable GetXmlData(string appConfig, bool merge)
        {
            return GetXmlData(appConfig, this.Settings.AutoPathDefaultConfigDirectory, merge);
        }
        public Hashtable GetXmlData(string configAppDir, string configDefDir, bool merge)
        {
            /*
            string applicationDirectory = configAppDir;
            string defaultDirectory = configDefDir;

            if (Settings.UseVersionForConfiguration)
            {
                applicationDirectory = string.Format(Settings.VersionDirectoryFormat, DS, applicationDirectory, Settings.DocumentVersion.ToString());
                defaultDirectory = string.Format(Settings.VersionDirectoryFormat, DS, defaultDirectory, Settings.DocumentVersion.ToString());
            }
            */

            bool copyAppFiles = !System.IO.Directory.Exists(configAppDir);
            bool copyDefFiles = !System.IO.Directory.Exists(configDefDir);

            if (!System.IO.Directory.Exists(configDefDir))
                System.IO.Directory.CreateDirectory(configDefDir);

            if (!System.IO.Directory.Exists(configAppDir))
                System.IO.Directory.CreateDirectory(configAppDir);

            if (Settings.ConfigDirectoryNameApplication == Settings.ConfigDirectoryNameDefault)
                copyDefFiles = false;

            if (Settings.TrackVersionConfiguration && Settings.UseVersionForConfiguration && copyDefFiles)
                try
                {
                    /* try to copy all files from previous version directory */
                    //string[] __dirs = System.IO.Directory.GetDirectories(Settings.ConfigDirectoryPathDefault, "v.*");
                    string[] __dirs = System.IO.Directory.GetDirectories(Settings.AutoPathDefaultConfigDirectory, "v.*");
                    if (__dirs.Length - 2 >= 0)
                    {
                        string[] __files = System.IO.Directory.GetFiles(__dirs[__dirs.Length - 2]);
                        using (StreamWriter strWr = File.CreateText(configDefDir + DS + "import.log"))
                        {
                            foreach (string __cpFile in __files)
                            {
                                System.IO.File.Copy(__cpFile, configDefDir + DS + Path.GetFileName(__cpFile));
                                strWr.WriteLine(string.Format("{0:HH:mm:ss} copy:\r\nfrom: {1}\r\nto: {2}", DateTime.Now, __cpFile, configDefDir + DS + Path.GetFileName(__cpFile)));
                            
                            }
                            strWr.Close();
                        }
                    }
                }
                catch { }

            if (Settings.TrackVersionConfiguration && Settings.UseVersionForConfiguration && copyAppFiles)
                try
                {
                    /* try to copy all files from previous version directory */
                    //string[] __dirs = System.IO.Directory.GetDirectories(Settings.ConfigDirectoryPathApplication, "v.*");
                    string __appConfigDirPath = Settings.AutoPathApplicationConfigDirectory;
                    if (Settings.ConfigDirectoryNameApplication.Length == 0)
                        __appConfigDirPath = Settings.ConfigDirectoryPathGeneral;


                    string[] __dirs = System.IO.Directory.GetDirectories(__appConfigDirPath, "v.*");
                    if (__dirs.Length - 2 >= 0)
                    {
                        string[] __files = System.IO.Directory.GetFiles(__dirs[__dirs.Length - 2]);
                        using (StreamWriter strWr = File.CreateText(configAppDir + DS + "import.log"))
                        {
                            foreach (string __cpFile in __files)
                            {
                                System.IO.File.Copy(__cpFile, configAppDir + DS + Path.GetFileName(__cpFile));
                                strWr.WriteLine(string.Format("{0:HH:mm:ss} copy:\r\nfrom: {1}\r\nto: {2}", DateTime.Now, __cpFile, configAppDir + DS + Path.GetFileName(__cpFile)));
                            }
                            strWr.Close();
                        }
                    }
                }
                catch { }
                
            /*
            string[] xmlCfg = System.IO.Directory.GetFiles(applicationDirectory, "*.xml");
            string[] xmlCfgDef = System.IO.Directory.GetFiles(defaultDirectory, "*.xml");
            */

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
                    if (ovFiles.ContainsKey(pathItem.Key.ToString()) && merge)
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
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);

            string thisDir = Directory.GetParent(filePath).Name;
            string pcode = string.Empty;
            string pname = string.Empty;
            string pvers = string.Empty;

            try
            {
                if (xmlDoc.GetElementsByTagName("document")[0].Attributes["parent"] != null)
                    pcode = xmlDoc.GetElementsByTagName("document")[0].Attributes["parent"].Value.ToString();
            }
            catch { }

            try
            {
                if (xmlDoc.GetElementsByTagName("document")[0].Attributes["name"] != null)
                    pname = xmlDoc.GetElementsByTagName("document")[0].Attributes["name"].Value.ToString();
            }
            catch { }

            try
            {
                if (xmlDoc.GetElementsByTagName("document")[0].Attributes["version"] != null)
                    pvers = xmlDoc.GetElementsByTagName("document")[0].Attributes["version"].Value.ToString();
            }
            catch { }

            // 0 - parent section (dir)
            // 1 - parent file (file)
            // if parentPath.length == 1
            //      check if at 0 index there is a directory name
            //      if true that path will be formed
            //      with this dir with this actual file name
            //      check if at 0 index there is a file name
            //      if true that path will be formed with a
            //      file from same directory
            string[] parentPath = pcode.ToString().Split('.');

            Dictionary<string, string> parentPathContainer = new Dictionary<string, string>();

            /* predefined keys */

            parentPathContainer.Add("RDIR", this.Settings.ConfigDirectoryPathGeneral);
            parentPathContainer.Add("PDIR", string.Empty);
            parentPathContainer.Add("PVER", string.Empty);
            parentPathContainer.Add("PFILE", string.Empty);

            if (this.Settings.UseVersionForConfiguration)
                parentPathContainer["PVER"] = string.Format(this.Settings.VersionFormat, pvers);

            if (parentPath.Length == 2)
            {
                parentPathContainer["PDIR"] = parentPath[0];
                parentPathContainer["PFILE"] = parentPath[1];
            }
            else
            {
                bool parentNameIsFile = false;
                bool parentNameIsDirectory = false;

                Dictionary<string, string> parentPathContainerForFile = new Dictionary<string, string>(parentPathContainer);
                Dictionary<string, string> parentPathContainerForDir = new Dictionary<string, string>(parentPathContainer);

                // File behaviour
                // we sugget that in the document is the name of the parent file
                // so if it is true we use the same directory as current document
                parentPathContainerForDir["PDIR"] = Path.GetDirectoryName(filePath);
                parentPathContainerForDir["PFILE"] = parentPath[0];
                // check for a valid suggestion

                // Directory behaviour
                // we sugget that in the document is the name of the parent directory
                parentPathContainerForFile["PDIR"] = parentPath[0];
                // so if it is true we use the same name as current document
                parentPathContainerForFile["PFILE"] = pname;
                // check for a valid suggestion

                if (Directory.Exists(string.Join(DS, new string[] { " " })))
                    parentNameIsDirectory = true;

                if (File.Exists(string.Join(DS, new string[] { " " })))
                    parentNameIsFile = true;

                if (parentNameIsFile)
                    parentPathContainer = parentPathContainerForFile;
                else
                    if (parentNameIsDirectory)
                        parentPathContainer = parentPathContainerForDir;

                // check for file
            }

            string parentConfigurationFile = EXT;

            if (File.Exists(parentConfigurationFile))
                GetMergedXmlDataFromFile(parentConfigurationFile, ref prevData);

            XmlTextReader _xreader = new XmlTextReader(filePath);
            _xreader.WhitespaceHandling = WhitespaceHandling.All;
            Hashtable data = ReadSection(_xreader);
            _xreader.Close();

            // saving current data
            prevData[prevData.Count + "_" + pname] = data;

            /*
            object pcode = GetValueByKey(data, "parent");
            object pname = GetValueByKey(data, "name");
            object pfile = GetRootKey(data);
            
            // saving current data
            prevData[prevData.Count + "_" + pname] = data;

            // making path to the parent file
            string parentFilePath = this.Settings.GeneralConfigDirectory + DS + pcode + DS + /*pfile +*-/ EXT;

            // read data if this file exists
            if (System.IO.File.Exists(parentFilePath))
                GetMergedXmlDataFromFile(parentFilePath, ref prevData);
            */
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
                            if (rd["name"] != null)
                                docSectionPath.Push(rd["name"]);
                            bool readDoc = true;
                            if (this.Settings.CheckForDocumentVersion)
                                if (rd.GetAttribute("version") == string.Empty || !this.Settings.DocumentVersion.Equals(new Version(rd["version"])))
                                    readDoc = false;
                            if (readDoc)
                                cont[rd["name"]] = ReadSection(rd, ref section);
                            else
                                Console.WriteLine("Document version must be the same as you are attempting to load");
                            docSectionPath.Pop();
                            docSectionPath.Clear();
                            break;
                        }
                    case "section":
                        {
                            try
                            {
                                section++;
                                string sectionName = rd["name"];

                                if (rd["name"] != null)
                                    docSectionPath.Push(rd["name"]);

                                if (rd.GetAttribute("parent") != null)
                                {
                                    string[] __arr = docSectionPath.ToArray();
                                    Array.Reverse(__arr);
                                    sectionParents.Add(string.Join(".", __arr), rd["parent"]);
                                }

                                cont[sectionName] = ReadSection(rd, ref section);
                                if (comment.Length != 0)
                                {
                                    string[] elementNameUI = docSectionPath.ToArray();
                                    Array.Reverse(elementNameUI);
                                    Commnets.Add(string.Join("_", elementNameUI), comment);
                                }

                                docSectionPath.Pop();
                            }
                            catch { }
                            break;
                        }
                    case "property":
                        {
                            try
                            {
                                if (rd["name"] != null)
                                    docSectionPath.Push(rd["name"]);

                                if (rd.GetAttribute("value") != null)
                                    cont.Add(rd["name"], rd["value"]);

                                if (rd.GetAttribute("ref") != null)
                                    cont.Add(rd["name"], "xref:" + rd["ref"]);

                                if (comment.Length != 0)
                                {
                                    string[] elementNameUI = docSectionPath.ToArray();
                                    Array.Reverse(elementNameUI);
                                    Commnets.Add(string.Join("_", elementNameUI), comment);
                                }

                                docSectionPath.Pop();
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

        /* writer */
        public bool SetXmlData(Hashtable data)
        {
            return SetXmlData(data, this.Settings.AutoPathApplicationConfigDirectory);
        }
        public bool SetXmlData(Hashtable data, string configAppDir)
        {
            bool state = true;

            XmlTextWriter xmlWr = null;
            foreach (DictionaryEntry de in data)
            {
                try
                {
                    /*
                    string applicationDirectory = configAppDir;
                    if (Settings.UseVersionForConfiguration)
                        applicationDirectory = string.Format(Settings.VersionDirectoryFormat, DS, applicationDirectory, Settings.DocumentVersion.ToString());
                    
                    if (!Directory.Exists(this.Settings.GeneralConfigDirectory + DS + applicationDirectory))
                        Directory.CreateDirectory(this.Settings.GeneralConfigDirectory + DS + applicationDirectory);
                    */
                    if (!Directory.Exists(configAppDir))
                        Directory.CreateDirectory(configAppDir);

                    xmlWr = new XmlTextWriter(configAppDir + DS + de.Key + ".xml", Encoding.Default);
                    xmlWr.Formatting = Formatting.Indented; 
                    xmlWr.Indentation = 4;
                    
                    WriteDocument(xmlWr, de, data);
                }
                catch
                {
                    state = false;
                }

                xmlWr.Close();
            }

            return state;
        }
        public void WriteDocument(XmlTextWriter xmlWr, DictionaryEntry de, Hashtable data)
        {
            xmlWr.WriteStartDocument();
            // autocomment
            string comment = de.Key.ToString().ToLower().Replace("configuration", string.Empty) + " Configuration Data File" + "\r\n";
            comment += "do not modity anything inside the document section" + "\r\n";
            comment += "geretated by " + this.GetComponentInfo;
            xmlWr.WriteComment("\r\n" + comment + "\r\n");
            xmlWr.WriteStartElement("document");
            xmlWr.WriteAttributeString("name", de.Key.ToString());
            xmlWr.WriteAttributeString("version", this.Settings.DocumentVersion.ToString());

            /* parent document */
            // best practice to use the next format of parent name:
            // ParentDirectoryName.ParentFileName
            // During of reading it wiil form the next path:
            // GeneralPath\\ParentDirectoryName\\Version[if needed]\\ParentFileName.EXT
            if (this.Settings.AutoPathApplicationConfigDirectory != this.Settings.AutoPathDefaultConfigDirectory)
                xmlWr.WriteAttributeString("parent", this.Settings.ConfigDirectoryNameDefault + "." + de.Key.ToString());

            docSectionPath.Push(de.Key.ToString());
            WriteSection(xmlWr, string.Empty, (Hashtable)de.Value, data);
            docSectionPath.Clear();
            xmlWr.WriteEndElement();
            xmlWr.WriteEndDocument();
        }
        public void WriteProperty(XmlTextWriter xmlWr, DictionaryEntry de)
        {
            /*
            string[] elementNameUI = docSectionPath.ToArray();
            Array.Reverse(elementNameUI);
            if (Commnets.ContainsKey(string.Join("_", elementNameUI)))
                xmlWr.WriteComment(Commnets.GetValue(string.Join("_", elementNameUI)).ToString());
            */
            WriteComment(xmlWr, docSectionPath.ToArray());

            xmlWr.WriteStartElement("property");
            xmlWr.WriteAttributeString("name", de.Key.ToString());
            if (de.Value != null && de.Value.ToString().StartsWith("xref:"))
                xmlWr.WriteAttributeString("ref", de.Value.ToString().Substring(5));
            else
                xmlWr.WriteAttributeString("value", de.Value == null ? string.Empty : de.Value.ToString());
            xmlWr.WriteEndElement();
        }
        public void WriteSection(XmlTextWriter xmlWr, string name, Hashtable innerData, Hashtable fullData)
        {
            if (name.Length != 0)
            {
                docSectionPath.Push(name);


                WriteComment(xmlWr, docSectionPath.ToArray());

                /*
                string[] elementNameUI = docSectionPath.ToArray();
                Array.Reverse(elementNameUI);
                if (Commnets.ContainsKey(string.Join("_", elementNameUI)))
                    xmlWr.WriteComment(Commnets.GetValue(string.Join("_", elementNameUI)).ToString());
                */

                xmlWr.WriteStartElement("section");
                xmlWr.WriteAttributeString("name", name);

                /* write parent class */
                try
                {
                    string[] __arr = docSectionPath.ToArray();
                    Array.Reverse(__arr);
                    string __path = string.Join(".", __arr);
                    if (sectionParents.ContainsKey(__path))
                    {
                        //Hashtable __parentData = (Hashtable)GetValueByPath(innerData, __path);
                        
                        /* remove parent properties */
                        
                        xmlWr.WriteAttributeString("parent", sectionParents[__path]);

                        // removing parent data
                        Hashtable parentData = (Hashtable)GetValueByPath(fullData, sectionParents[__path]);
                        CompareCustomDataWithDefault(ref parentData, ref innerData, __path);
                    }

                }
                catch { }
            }

            // sorted keys

            List<string> innerKeys = new List<string>();
            foreach (DictionaryEntry kde in innerData)
                innerKeys.Add(kde.Key.ToString());
            innerKeys.Sort();

            //foreach (DictionaryEntry de in innerData)
            foreach(string entryKey in innerKeys)
            {
                //DictionaryEntry de = (DictionaryEntry);
                
                DictionaryEntry de = new DictionaryEntry(entryKey, innerData[entryKey]);

                try
                {
                    if (de.Value.Equals(DBNull.Value))
                        continue;

                    if (de.Value == null)
                        WriteProperty(xmlWr, de);
                    else
                        switch (de.Value.GetType().ToString())
                        {
                            case "System.Collections.Hashtable":
                                {
                                    WriteSection(xmlWr, de.Key.ToString(), (Hashtable)de.Value, fullData);
                                    break;
                                }
                            default:
                                {
                                    docSectionPath.Push(de.Key.ToString());
                                    WriteProperty(xmlWr, de);
                                    docSectionPath.Pop();
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

            docSectionPath.Pop();

            //xmlWr.WriteWhitespace("\r\n\r\n");
        }
        public void WriteComment(XmlTextWriter xmlWr, string[] keyPath)
        {
            Array.Reverse(keyPath);
            if (Commnets.ContainsKey(string.Join("_", keyPath)))
                xmlWr.WriteComment(" " + Commnets.GetValue(string.Join("_", keyPath)).ToString().Trim() + " ");
        }
        /* tools */

        /* data access */
        /* data access :: getter */
        public object GetValueByKey(Hashtable data, string key)
        {
            bool wasSet = false;
            object value = GetValueByKey(data, key, ref wasSet);
            if (value.ToString().StartsWith("xref:"))
                value = GetValueByKey(data, value.ToString().Substring(5));

            return value;
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
            return GetValueByPath(data, path, true);
        }
        public object GetValueByPath(Hashtable data, string path, bool addParentData)
        {
            if (path.Length == 0)
                return DBNull.Value;

            string[] keys = path.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            Hashtable cr = (Hashtable)data.Clone();
            object res = DBNull.Value;

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
            {
                res = cr[keys[keys.Length - 1]];

                if (res != null && res.GetType() == typeof(Hashtable))
                {
                    Hashtable result = new Hashtable((Hashtable)res);
                    /* parent merging */
                    try
                    {/*
                        Hashtable __parentData = new Hashtable();
                        if (sectionParents.ContainsKey(path))
                            __parentData = (Hashtable)GetValueByPath(data, sectionParents[path]);

                        /* merging * /
                        foreach (DictionaryEntry de in __parentData)
                            if (!result.ContainsKey(de.Key))
                                result[de.Key] = de.Value;
                        */
                        if (addParentData && sectionParents.ContainsKey(path))
                            AppendAllParents(data, result/*, path*/);
                    }
                    catch { }

                    res = new Hashtable(result);
                }
            }

            if (res != null && res.ToString().StartsWith("xref:"))
                res = GetValueByPath(data, res.ToString().Substring(5));

            return res;
        }

        /* data access :: setter */
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
                    SetNewValueByPath(ref data, keys, 0, value);

            }
            while (reset);

        }
        public void SetNewValueByPath(ref Hashtable data, string[] keys, int index, object value)
        {
            if (index + 1 < keys.Length)
            {
                Hashtable ht = new Hashtable();

                if (!data.ContainsKey(keys[index]) || !data[keys[index]].GetType().Equals(typeof(Hashtable)))
                    data[keys[index]] = ht;
                else
                    ht = (Hashtable)data[keys[index]];

                SetNewValueByPath(ref ht, keys, index + 1, value);
            }
            else
                data[keys[index]] = value;
        }

        /* data keys and wrappers */
        public string[] GetTopKeys(Hashtable dataItems)
        {
            List<string> rk = new List<string>();
            foreach (DictionaryEntry e in dataItems)
            {
                rk.Add(e.Key.ToString());
            }
            return rk.ToArray();
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
        public Hashtable GetDataContent(Hashtable data)
        {
            return (Hashtable)data[GetRootKey(data)];
        }

        /* references */
        public void ReferenceUpdate(ref Hashtable dataFull)
        {
            this.ReferenceUpdate(ref dataFull, dataFull, new Stack());
        }
        public void ReferenceUpdate(ref Hashtable dataFull, Hashtable trackingData, Stack trackingPath)
        {
            foreach (DictionaryEntry refItem in trackingData)
            {
                if (refItem.Value.GetType() == typeof(Hashtable))
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

        /* parent sections */
        public void AppendAllParents(Hashtable data, Hashtable result/*, string path*/)
        {
            AppendAllParents(data, result/*, path, string.Empty/* new Stack<string>()*/);
        }
        public void AppendAllParents(Hashtable data, Hashtable result, string path/*, string runnigKey/*Stack<string> depthMonitor*/)
        {

            Hashtable __parentData = new Hashtable();
            //string[] _arr = depthMonitor.ToArray();
            //Array.Reverse(_arr);
            //string __path = runnigKey;
            //if (runnigKey != string.Empty)
            //    __path += "." + runnigKey;// string.Join(".", _arr);

            bool propertySet = false;


            foreach (DictionaryEntry de in (Hashtable)result.Clone())
            {

                /* parent merging */
                if (!propertySet)
                    try
                    {
                        if (sectionParents.ContainsKey(path))
                        {
                            __parentData = (Hashtable)GetValueByPath(data, sectionParents[path]);

                            /* merging */
                            foreach (DictionaryEntry dep in __parentData)
                            {
                                if (!result.ContainsKey(dep.Key.ToString()))
                                    result[dep.Key] = dep.Value;
                                else
                                    if (dep.Value.GetType() == typeof(Hashtable) && result[dep.Key].GetType() == typeof(Hashtable))
                                    {
                                        //depthMonitor.Push(de.Key.ToString());
                                        AppendAllParents(data, (Hashtable)(result[dep.Key]), path + "." + de.Key/*, path + "." + de.Key /*depthMonitor*/);
                                    }
                            }
                        }
                        propertySet = true;
                    }
                    catch { }
            }

            /*
            if (depthMonitor.Count > 0)
                depthMonitor.Pop();
            */
        }

        /* merging */
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

        /* comparers */
        public void CompareCustomDataWithDefault(ref Hashtable fullData, ref Hashtable customData, string path)
        {
            CompareCustomDataWithDefault(ref fullData, ref customData, path, new Stack<string>());
        }
        public void CompareCustomDataWithDefault(ref Hashtable fullData, ref Hashtable customData, string path, Stack<string> depthMonitor)
        {
            if (customData.Count == 0)
                return;

            foreach (DictionaryEntry de in fullData)
                if (customData[de.Key].GetType() == typeof(Hashtable) && fullData[de.Key].GetType() == typeof(Hashtable))
                {
                    Hashtable __tCustom = (Hashtable)customData[de.Key];
                    Hashtable __tFull = (Hashtable)fullData[de.Key];
                    depthMonitor.Push(de.Key.ToString());
                    CompareCustomDataWithDefault(ref __tFull, ref  __tCustom, path, depthMonitor);
                    if (IsAllItemsAreNullable(__tCustom))
                        customData[de.Key] = DBNull.Value;
                    else
                    {
                        // if not all elements are inherited from default
                        // add parent target to tis section

                        string __path = ".";
                        if (depthMonitor.Count > 0)
                        {
                            string[] __arr = depthMonitor.ToArray();
                            Array.Reverse(__arr);
                            __path += string.Join(".", __arr);
                        }
                        __path += de.Key.ToString();

                        if (!sectionParents.ContainsKey(path + __path))
                        {
                            string __rootPathOwner = sectionParents[path];
                            sectionParents[path + __path] = __rootPathOwner +__path;
                        }

                    }
                }
                else
                    if (customData.ContainsKey(de.Key) && customData[de.Key].Equals(fullData[de.Key]))
                        customData[de.Key] = DBNull.Value;

            depthMonitor.Pop();
        }

        /* validators */
        public bool IsAllItemsAreNullable(Hashtable container)
        {
            bool isNull = true;
            foreach (DictionaryEntry de in container)
                if (!de.Value.Equals(DBNull.Value))
                    return false;
            return isNull;
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
        /* Properties */
        /*
        public string ConfigDir { set { this.cfgDir = value; } get { return this.cfgDir; } }
        public string ConfigDefaultDir { set { this.cfgDefDir = value; } get { return this.cfgDefDir; } }
        public string ConfigAppDir { set { this.cfgAppDir = value; } get { return this.cfgAppDir; } }
        */
    }
}
