using System;
using System.Collections.Generic;
using System.Text;
using components.UI.Windows.wndAppSettings;
using components.Components.XmlDocumentParser;
using System.Collections;
using System.Windows.Forms;

namespace components.Public
{
    /* user mode interface */
    public class ApplicationConfiguration : components.Shared.Interfaces.IConfigurable
    {
        private Hashtable xmlHt;
        private Com_XmlDocumentParser xP;
        private static ApplicationConfiguration _instance;
        private static bool _useCustomConfigurationMethod;

        /* constructors */
        static ApplicationConfiguration()
        {
            //Instance = AppSettingsWindowContext.Instance;
            //Settings = AppSettingsWindowContext.Settings;
            _useCustomConfigurationMethod = true;
        }
        private ApplicationConfiguration()
        {
            xmlHt = new Hashtable();
            xP = new Com_XmlDocumentParser();
            // set default configuration
            DefaultCustomConfigurationMethod(xP.Settings);
        }

        /* public */
        /* public: delegates */

        public delegate void CustomConfigurationDelegate(Com_XmlDocumentParser_Configuration Settings);
        public static CustomConfigurationDelegate CustomConfigurationMethod { get; set; }

        /* public: customzied instance */
        /*
        public static ApplicationConfiguration InstanceCustomSettings
        {
            get
            {
                if (_instance == null)
                    _instance = new ApplicationConfiguration();

                /* this custom method always will be invoked *-/
                if (CustomConfigurationMethod != null)
                    CustomConfigurationMethod.Invoke(Settings);
                return _instance;
            }
        }*/

        /* public: simple references */
        public static ApplicationConfiguration Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ApplicationConfiguration();
                    /* autoload */
                    if (!_useCustomConfigurationMethod)
                        _instance.LoadConfigurationData();
                }

                /* use this method only once */
                if (_useCustomConfigurationMethod && CustomConfigurationMethod != null)
                {
                    _useCustomConfigurationMethod = false;
                    CustomConfigurationMethod.Invoke(Settings);
                    /* autoload */
                    _instance.LoadConfigurationData();
                }

                return _instance;
            }
        }
        public static Com_XmlDocumentParser_Configuration Settings
        {
            get
            {
                return _instance.XmlParser.Settings;
            }
            set
            {
                _instance.XmlParser.Settings = value;
            }
        }
        public Com_XmlDocumentParser XmlParser { get { return xP; } set { xP = value; } }
        public Hashtable Configuration { get { return xmlHt; } }

        #region IConfigurable Members
        #region Data Load
        public bool LoadConfigurationData()
        {
            bool rezult = false;
            try
            {
                this.xmlHt = XmlParser.GetXmlData();
                rezult = true;
            }
            catch { this.xmlHt = new Hashtable(); }
            return rezult;
        }
        public bool SaveConfigurationData()
        {/*
            if (XmlParser.Settings.UseVersionForConfiguration)
                return XmlParser.SetXmlData(this.xmlHt, string.Format("{0}\\v.{1}.{2}", XmlParser.Settings.ApplicationConfigDirectory, XmlParser.Settings.DocumentVersion.Major, XmlParser.Settings.DocumentVersion.Minor));
            else*/
            return XmlParser.SetXmlData(this.xmlHt);
        }
        public bool ReloadConfigurationData()
        {
            return LoadConfigurationData();
        }
        #endregion
        #region Value Selector
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
        public Hashtable GetTableHashValueByPath(string path, string keyName, string valueName)
        {
            Hashtable ht = GetValueByPath<Hashtable>(path);
            Hashtable data = new Hashtable();
            Hashtable row = null;
            for (int i = 0; i < ht.Count; i++)
            {
                row = (Hashtable)ht[i.ToString()];
                data[row[keyName]] = row[valueName];
                /*
                // loop by rows
                string keyToSetValue = "TEMPINDEX";
                bool valueWasSetFirst = false;
                foreach (DictionaryEntry de in ((Hashtable)ht[i.ToString()]))
                {


                    /*
                    if (de.Key.ToString().Equals(keyName))
                    {
                        data[de.Value] = "";
                        keyToSetValue = de.Value.ToString();
                        continue;
                    }
                    if (de.Key.ToString().Equals(valueName))
                    {
                        data[keyToSetValue] = de.Value.ToString();
                        valueWasSetFirst = true;
                    }* /
                }*/
            }

            return data;
        }
        public Hashtable GetTableHashValueByPath(string path)
        {
            Hashtable ht = GetValueByPath<Hashtable>(path);
            Hashtable data = new Hashtable();
            // loop by rows
            string keyToSetValue = string.Empty;
            for (int i = 0, idx = 0; i < ht.Count; i++, idx = 0)
                foreach (DictionaryEntry de in ((Hashtable)ht[i.ToString()]))
                {
                    if (idx++ % 2 == 0)
                    {
                        if (data.ContainsKey(de.Value.ToString()))
                            continue;

                        data.Add(de.Value.ToString(), "");
                        keyToSetValue = de.Value.ToString();
                    }
                    else
                        data[keyToSetValue] = de.Value.ToString();
                }

            return data;
        }
        public Dictionary<string, string> GetTableDictValueByPath(string path, string keyName, string valueName)
        {
            Hashtable ht = GetTableHashValueByPath(path, keyName, valueName);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (DictionaryEntry de in ht)
                dict.Add(de.Key.ToString(), de.Value.ToString());

            return dict;
        }
        public Dictionary<string, string> GetTableDictValueByPath(string path)
        {
            Hashtable ht = GetTableHashValueByPath(path);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (DictionaryEntry de in ht)
                dict.Add(de.Key.ToString(), de.Value.ToString());

            return dict;
        }
        #endregion
        #region Value Modify
        public object this[string path]
        {
            get
            {
                return XmlParser.GetValueByPath(this.xmlHt, path);
            }
        }
        public void SetValueByPath(string path, object value)
        {
            XmlParser.SetValueByPath(this.Configuration, path, value);
        }
        #endregion
        #endregion

        #region Control Collection
        /* bind config values to controls */
        public void BindValuesToAllControls(System.Windows.Forms.Control container)
        {/*
            if (container.Controls.Count == 0 || container.GetType() == typeof(DataGridView))
                BindValueToSingleControls(ref container);
            else*/
            Control _tct = null;
            foreach (Control ctrl in container.Controls)
            {
                if (ctrl.Controls.Count != 0)
                    BindValuesToAllControls(ctrl);
                if (ctrl.Name.Length != 0)
                {
                    _tct = container.Controls[ctrl.Name];
                    BindValueToSingleControls(ref _tct);
                }
            }
        }
        public void BindValueToSingleControls(ref Control c)
        {
            if (c.Name.Length == 0 || !c.Name.Contains("_"))
                return;

            object val = this.GetValueByPath(c.Name.Replace('_', '.'));
            if (DBNull.Value.Equals(val))
                return;

            try
            {
                if (this.xP.Commnets.ContainsKey(c.Name))
                    c.AccessibleDescription = this.xP.Commnets.GetValue(c.Name).ToString();
            }
            catch { }

            switch (c.GetType().ToString())
            {
                case "System.Windows.Forms.TextBox":
                    {
                        if (val != null)
                            ((TextBox)c).Text = val.ToString();
                        break;
                    }
                case "System.Windows.Forms.CheckBox":
                    {
                        if (val != null)
                            ((CheckBox)c).Checked = bool.Parse(val.ToString());
                        break;
                    }
                case "System.Windows.Forms.ComboBox":
                    {
                        try
                        {
                            ((ComboBox)c).SelectedItem = val;
                            break;
                        }
                        catch { }

                        ((ComboBox)c).SelectedText = val.ToString();
                        break;
                    }
                case "System.Windows.Forms.RichTextBox":
                    {
                        if (val != null)
                            ((RichTextBox)c).Text = val.ToString();
                        break;
                    }
                case "System.Windows.Forms.NumericUpDown":
                    {
                        if (val != null)
                            ((NumericUpDown)c).Value = decimal.Parse(val.ToString());
                        break;
                    }
                case "System.Windows.Forms.TrackBar":
                    {

                        if (val != null)
                            ((TrackBar)c).Value = int.Parse(val.ToString());
                        break;
                    }
                case "System.Windows.Forms.VScrollBar":
                    {
                        if (val != null)
                            ((VScrollBar)c).Value = int.Parse(val.ToString());
                        break;
                    }
                case "System.Windows.Forms.HScrollBar":
                    {
                        if (val != null)
                            ((HScrollBar)c).Value = int.Parse(val.ToString());
                        break;
                    }
                case "System.Windows.Forms.DataGridView":
                    {
                        try
                        {
                            ((DataGridView)c).Rows.Clear();
                            ((DataGridView)c).Rows.Add(((Hashtable)val).Count);
                            // loop by rows
                            foreach (DictionaryEntry de in (Hashtable)val)
                            {
                                foreach (DictionaryEntry rdi in (Hashtable)de.Value)
                                {
                                    try
                                    {
                                        ((DataGridView)c)[rdi.Key.ToString(), int.Parse(de.Key.ToString())].Value = rdi.Value;
                                    }
                                    catch { }
                                }
                            }
                        }
                        catch { }
                        break;
                    }
                /* control access 2 */
                case "System.Windows.Forms.CheckedListBox":
                    {
                        //((CheckedListBox)c)
                        foreach (DictionaryEntry checkedItem in (Hashtable)val)
                            ((CheckedListBox)c).SetItemChecked(((CheckedListBox)c).Items.IndexOf(checkedItem.Value), true);
                        break;
                    }
                case "System.Windows.Forms.DomainUpDown":
                    {
                        try
                        {
                            ((DomainUpDown)c).SelectedItem = val;
                            break;
                        }
                        catch { }

                        if (val != null)
                            ((DomainUpDown)c).Text = val.ToString();

                        break;
                    }
                case "System.Windows.Forms.ListBox":
                    {
                        foreach (DictionaryEntry selectedIndex in (Hashtable)val)
                            ((ListBox)c).SetSelected(int.Parse(selectedIndex.Value.ToString()), true);
                        break;
                    }
                case "System.Windows.Forms.DateTimePicker":
                    {
                        ((DateTimePicker)c).Value = DateTime.Parse(val.ToString());
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        /* collect config values from controls */
        public void CollectValuesFromAllControls(System.Windows.Forms.Control container)
        {/*
            if (container.Controls.Count == 0 || container.GetType() == typeof(DataGridView))
                CollectValueFromSingleControl(ref container);
            else*/
            Control _tct = null;
            foreach (Control ctrl in container.Controls)
            {
                CollectValuesFromAllControls(ctrl);
                if (ctrl.Name.Length != 0)
                {
                    _tct = container.Controls[ctrl.Name];
                    CollectValueFromSingleControl(ref _tct);
                }
            }
        }
        public void CollectValueFromSingleControl(ref Control c)
        {
            if (c.Name.Length == 0 || !c.Name.Contains("_"))
                return;

            string valuePath = c.Name.Replace('_', '.');

            try
            {
                if (!string.IsNullOrEmpty(c.AccessibleDescription))
                    this.xP.Commnets.SetValue(c.Name, c.AccessibleDescription);
            }
            catch { }

            switch (c.GetType().ToString())
            {
                case "System.Windows.Forms.TextBox":
                    {
                        this.SetValueByPath(valuePath, ((TextBox)c).Text);
                        break;
                    }
                case "System.Windows.Forms.CheckBox":
                    {
                        this.SetValueByPath(valuePath, ((CheckBox)c).Checked);
                        break;
                    }
                case "System.Windows.Forms.ComboBox":
                    {
                        this.SetValueByPath(valuePath, ((ComboBox)c).SelectedItem);
                        break;
                    }
                case "System.Windows.Forms.RichTextBox":
                    {
                        this.SetValueByPath(valuePath, ((RichTextBox)c).Text);
                        break;
                    }
                case "System.Windows.Forms.NumericUpDown":
                    {
                        this.SetValueByPath(valuePath, ((NumericUpDown)c).Value);
                        break;
                    }
                case "System.Windows.Forms.TrackBar":
                    {
                        this.SetValueByPath(valuePath, ((TrackBar)c).Value);
                        break;
                    }
                case "System.Windows.Forms.VScrollBar":
                    {
                        this.SetValueByPath(valuePath, ((VScrollBar)c).Value);
                        break;
                    }
                case "System.Windows.Forms.HScrollBar":
                    {
                        this.SetValueByPath(valuePath, ((HScrollBar)c).Value);
                        break;
                    }
                case "System.Windows.Forms.DataGridView":
                    {
                        Components.HashObject.Com_HashObject hObj = new Components.HashObject.Com_HashObject();

                        bool useRow = false;
                        foreach (DataGridViewRow drw in ((DataGridView)c).Rows)
                        {
                            useRow = false;
                            foreach (DataGridViewCell testRow in drw.Cells)
                                if (testRow.Value != null && testRow.Value.ToString() != string.Empty)
                                {
                                    useRow = true;
                                    break;
                                }

                            if (useRow)
                                foreach (DataGridViewCell dcl in drw.Cells)
                                    hObj[drw.Index.ToString()].Add(dcl.OwningColumn.Name, dcl.Value);
                        }


                        this.SetValueByPath(valuePath, hObj.GetHashtable());
                        break;
                    }
                /* control access 2 */
                case "System.Windows.Forms.CheckedListBox":
                    {
                        //((CheckedListBox)c)
                        Hashtable checkedIndexces = new Hashtable();
                        //int idx = 0;
                        foreach (object checkedItem in ((System.Windows.Forms.CheckedListBox)c).CheckedItems)
                            checkedIndexces[((System.Windows.Forms.CheckedListBox)c).Items.IndexOf(checkedItem).ToString()] = checkedItem;
                        this.SetValueByPath(valuePath, checkedIndexces);
                        break;
                    }
                case "System.Windows.Forms.DomainUpDown":
                    {
                        this.SetValueByPath(valuePath, ((DomainUpDown)c).SelectedItem);
                        break;
                    }
                case "System.Windows.Forms.ListBox":
                    {
                        Hashtable selectedIndexces = new Hashtable();
                        int idx = 0;
                        foreach (int selectedIndex in ((System.Windows.Forms.ListBox)c).SelectedIndices)
                            selectedIndexces[++idx] = selectedIndex;

                        this.SetValueByPath(valuePath, selectedIndexces);
                        break;
                    }
                case "System.Windows.Forms.DateTimePicker":
                    {
                        this.SetValueByPath(valuePath, ((DateTimePicker)c).Value);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
        #endregion

        public void ResetInstance()
        {
            _instance = new ApplicationConfiguration();
        }

        public void ForceCustomMethod()
        {
            _useCustomConfigurationMethod = true;
        }

        /* internal */

        /* default handlers */
        private void DefaultCustomConfigurationMethod(Com_XmlDocumentParser_Configuration Settings)
        {
            /* the provided configuration settings below is used by default in all applications
             * made the proper ovveride in your application
             */
            Settings.ConfigDirectoryNameApplication = string.Empty;
            Settings.ConfigDirectoryNameDefault = string.Empty;
            Settings.ConfigDirectoryPathGeneral = System.Windows.Forms.Application.StartupPath + "\\config";
            Settings.CheckForDocumentVersion = false;
            Settings.UseVersionForConfiguration = false;
            //this.ReloadConfiguration(false);
        }


    }
}
