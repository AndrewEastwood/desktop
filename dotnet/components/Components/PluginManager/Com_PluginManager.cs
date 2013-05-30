using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Xml;
using System.Windows.Forms;
using components.Shared.Attributes;
using components.Shared.Interfaces;
using components.Lib;

namespace components.Components.PluginManager
{
    public class Com_PluginManager
    {
        private const string CFG_FILE_NAME = "Plugins.xml";

        private Dictionary<PluginType, Dictionary<string, IPlugin>> _plist;
        //private LinkedListNode<string> a;
        private Dictionary<PluginType, List<string>> _names;
        //private List<string> _inappropriates;
        // For non-multi executable
        private Dictionary<PluginType, string> _active;
        // For multi executable
        //private Dictionary<PluginType, List<string>> _actives;


        Lib.BinData bdata;




        public Com_PluginManager(string Path) :
            this()
        {
            InitializeModules(Path);
        }

        // Constructor
        public Com_PluginManager()
        {

            Array arr = Enum.GetValues(typeof(PluginType));

            _plist = new Dictionary<PluginType, Dictionary<string, IPlugin>>();
            _names = new Dictionary<PluginType, List<string>>();
            _active = new Dictionary<PluginType, string>();
            //_actives = new Dictionary<PluginType, List<string>>();

            //_inappropriates = new List<string>();

            foreach (PluginType t in arr)
            {
                _plist.Add(t, new Dictionary<string, IPlugin>());
                _names.Add(t, new List<string>());
                _active.Add(t, string.Empty);
                //_actives.Add(t, new List<string>());
            }
            
            bdata = new BinData();

            object _ldat = bdata.LoadData(Application.StartupPath + "\\" + CFG_FILE_NAME);

            if (_ldat != null)
            {
                _active[PluginType.LegalPrinterDriver] = _ldat.ToString();
            }


            // It's not correct. It soult be removed when Save/Load method will be exists
            //_active[PluginType.FPDriver] = "DATECS_FP3530T";
        }

        ~Com_PluginManager()
        {
            object _data = _active[PluginType.LegalPrinterDriver];
            bdata.SaveData(Application.StartupPath + "\\" + CFG_FILE_NAME, _data);
        }



        // Private
        public bool InitializeModules(string PluginDir)
        {
            if (!Directory.Exists(PluginDir))
            {
                Directory.CreateDirectory(PluginDir);
            }
            List<string> _pluginFiles = new List<string>();

            FileInfo fi = null;
            foreach (string f in Directory.GetFiles(PluginDir, "*.dll", SearchOption.AllDirectories))
                _pluginFiles.Add(f);

            if (_pluginFiles.Count == 0)
                return false;

            Assembly asm;
            PluginType pt = PluginType.Default;
            Type PluginClass = null;

            foreach (string pname in _pluginFiles.ToArray())
            {
                asm = Assembly.LoadFile(pname);

                if (asm != null)
                {
                    foreach (Type type in asm.GetTypes())
                    {
                        if (type.IsAbstract) continue;
                        object[] attrs = type.GetCustomAttributes(typeof(PluginSimpleAttribute), true);
                        if (attrs.Length > 0)
                        {
                            foreach (PluginSimpleAttribute pa in attrs)
                            {
                                pt = pa.Type;
                            }
                            PluginClass = type;
                            //To support multiple plugins in a single assembly, modify this
                        }
                    }
                    if (pt == PluginType.Default)
                    {
                        return false;
                    }
                    try
                    {
                        //Get the plugin
                        IPlugin __plg = Activator.CreateInstance(PluginClass) as IPlugin;
                        //
                        _plist[pt].Add(__plg.Name, __plg);
                        //
                        _names[pt].Add(__plg.Name);

                    }
                    catch { }

                    //return true;
                }
            }

            return true;
        }




        // Public
        public void Perform(PluginType pTy)
        {
            switch ((int)pTy)
            {
                case 0:
                    {
                        
                        break;
                    }
                case 1:
                    {

                        break;
                    }
                case 2:
                    {
                        // AppUI


                        break;
                    }
                case 3:
                    {

                        break;
                    }
            }
        }
        
        public T GetPlugin<T>(string name)
        {
            try
            {
                Type tt = typeof(T);
                PluginType _pt = (PluginType)Enum.Parse(typeof(PluginType), tt.Name.Substring(1));

                if (_plist[_pt].ContainsKey(name))
                    return (T)_plist[PluginType.LegalPrinterDriver][name];
            }
            catch { }

            throw new Exception("That plugin's name was not declarated in program");
        }                /**/
        public string[] GetNames(PluginType p)
        {
            return _names[p].ToArray();
        }
        public bool IsActive(PluginType p)
        {
            if (_active[p] != string.Empty)
                return true;

            return false;
        }
        public void SetActive(PluginType p, string name)
        {
            _active[p] = name;
        }
        public T GetActive<T>()
        {
            try
            {
                Type tt = typeof(T);
                PluginType _pt = (PluginType)Enum.Parse(typeof(PluginType), tt.Name.Substring(1));

                foreach (KeyValuePair<string, IPlugin> _typedItems in _plist[_pt])
                {
                    ((ILegalPrinterDriver)_typedItems.Value).Deactivate();
                }

                if (_plist[_pt].ContainsKey(_active[_pt]))
                {
                    ((ILegalPrinterDriver)_plist[PluginType.LegalPrinterDriver][_active[_pt]]).Activate();
                    return (T)_plist[PluginType.LegalPrinterDriver][_active[_pt]];
                }
            }
            catch { }


            //return false;
            throw new Exception("That plugin's name was not declarated in program");

        }



        /*
        public bool Load() { }
        public bool Reload() { }
        public bool Unload() { }
        public string[] GetNames() { }
        public string[] GetNames(PluginType Type) { }
        public string[] GetNames(PluginMode Mode) { }
        public IPlugin[] GetPlugins(PluginType Type) { }
        public IPlugin[] GetPlugins(PluginMode Mode) { }
        public bool Execute(IWin32Window Target) { }
        public T GetActiveSingle<T>() { }
        public T Activate<T>() { }*/

        // Properties
        public int Count { get { return _plist.Count; } }

    }
}
