using System;
using System.Collections.Generic;
using System.Text;
using components.Shared.Objects;
using components.Components.XmlDocumentParser;
using System.Collections;
using System.Windows.Forms;

namespace components.Public
{
    public class AppXmlConfig : Obj_xmlConfiguratoin
    {
        public AppXmlConfig()
            : this("config", string.Empty, string.Empty, false)
        {
        }

        public AppXmlConfig(string configDir, string defaultPath, string appPath, bool merge)
        {
            /* setting up configuration directories */
            this.XmlParser.Settings.ApplicationConfigDirectory = configDir;
            this.XmlParser.Settings.DefaultConfigDirectory = defaultPath;
            if (appPath != string.Empty)
                this.XmlParser.Settings.ApplicationConfigDirectory = appPath;
            else
                this.XmlParser.Settings.ApplicationConfigDirectory = defaultPath;
            /* getting configuration data */
            this.BindConfigData(merge);
        }

        public void ReloadConfiguration(bool merge)
        {
            this.BindConfigData(merge); 
        }

        public bool StoreConfigData(string configDir)
        {
            return this.XmlParser.SetXmlData(this.Configuration, configDir);
        }

        /* bind config values to controls */

        public void BindConfigurationValues(System.Windows.Forms.Control container)
        {
            if (container.Controls.Count == 0 || container.GetType() == typeof(DataGridView))
                BindControlConfigValue(ref container);
            else
                foreach (Control ctrl in container.Controls)
                {
                    BindConfigurationValues(ctrl);
                }
        }

        public void BindControlConfigValue(ref Control c)
        {
            if (c.Name.Length == 0 || !c.Name.Contains("_"))
                return;

            object val = this.GetValueByPath(c.Name.Replace('_', '.'));
            if (DBNull.Value.Equals(val))
                return;

            switch (c.GetType().ToString())
            {
                case "System.Windows.Forms.TextBox":
                    {
                        ((TextBox)c).Text = val.ToString();
                        break;
                    }
                case "System.Windows.Forms.CheckBox":
                    {
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
                        ((RichTextBox)c).Text = val.ToString();
                        break;
                    }
                case "System.Windows.Forms.NumericUpDown":
                    {
                        ((NumericUpDown)c).Value = decimal.Parse(val.ToString());
                        break;
                    }
                case "System.Windows.Forms.TrackBar":
                    {
                        ((TrackBar)c).Value = int.Parse(val.ToString());
                        break;
                    }
                case "System.Windows.Forms.VScrollBar":
                    {
                        ((VScrollBar)c).Value = int.Parse(val.ToString());
                        break;
                    }
                case "System.Windows.Forms.HScrollBar":
                    {
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
                default:
                    {
                        break;
                    }
            }
        }

        /* collect config values from controls */

        public void CollectConfigurationValues(System.Windows.Forms.Control container)
        {
            if (container.Controls.Count == 0 || container.GetType() == typeof(DataGridView))
                CollectControlConfigValue(ref container);
            else
                foreach (Control ctrl in container.Controls)
                {
                    CollectConfigurationValues(ctrl);
                }
        }

        public void CollectControlConfigValue(ref Control c)
        {
            if (c.Name.Length == 0 || !c.Name.Contains("_"))
                return;

            string valuePath = c.Name.Replace('_', '.');

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
                                    hObj[drw.Index].Add(dcl.OwningColumn.Name, dcl.Value);
                        }
                         
    
                        this.SetValueByPath(valuePath, hObj.GetHashtable());
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

    }
}
