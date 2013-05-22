using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using IKC_OP2.Config;

namespace IKC_OP2.UI.AppUI
{
    public partial class Tree : UserControl
    {
        //private string[][] functions;
        //Hashtable _f;

        /* CONSTRUCTORS */

        public Tree()
        {
            InitializeComponent();

            RestoreFunctions();
            //_f = new Hashtable();
        }

        /* PRIVATE MEMBERS */
        
        private void functionsTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            for (int i = 0; i < e.Node.Nodes.Count; i++)
                e.Node.Nodes[i].Checked = e.Node.Checked;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //Hashtable _f;

            byte f_idx = 0;

            //functions[0] = new string[0];
            //functions[1] = new string[0];

            string _n = string.Empty;
            string _t = string.Empty;

            Params.AllowedMethods = new Hashtable();

            for (byte i = 0; i < (byte)functionsTree.Nodes[0].Nodes.Count; i++)
            {
                for (f_idx = 0; f_idx < functionsTree.Nodes[0].Nodes[i].Nodes.Count; f_idx++)
                {
                    if (functionsTree.Nodes[0].Nodes[i].Nodes[f_idx].Checked)
                    {
                        //Array.Resize<string>(ref functions[0], functions[0].Length + 1);
                        //Array.Resize<string>(ref functions[1], functions[1].Length + 1);
                        //functions[0][functions[0].Length - 1] = functionsTree.Nodes[0].Nodes[i].Nodes[f_idx].Text;
                        //functions[1][functions[1].Length - 1] = functionsTree.Nodes[0].Nodes[i].Nodes[f_idx].Name;
                        
                        _t = functionsTree.Nodes[0].Nodes[i].Nodes[f_idx].Text;
                        _n = functionsTree.Nodes[0].Nodes[i].Nodes[f_idx].Name;
                        Params.AllowedMethods.Add(_n, _t);
                    }
                }
            }

            //SaveFunctions();
        }
        //private Hashtable SaveFunctions()
        //{
            //System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binF = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            //System.IO.FileStream fs = new System.IO.FileStream(Config.Path.FULL_CFG_PLUG_PATH, System.IO.FileMode.Create, System.IO.FileAccess.Write);

            /*try
            {
                object[] type = new object[2];
                type[0] = functions[0];
                type[1] = functions[1];

                binF.Serialize(fs, type);
            }
            catch { }

            fs.Close();
            fs.Dispose();*/
        //}
        private void RestoreFunctions()
        {
            //functions = new string[2][] { new string[0], new string[0] };

            //System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binF = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            //System.IO.FileStream fs = null;
            try
            {
                //fs = new System.IO.FileStream(Config.Path.FULL_CFG_PLUG_PATH, System.IO.FileMode.Open, System.IO.FileAccess.Read);

                //object[] type = (object[])binF.Deserialize(fs);
               // fs.Close();
               // fs.Dispose();

                //functions[0] = (string[])type[0];
                //functions[1] = (string[])type[1];

                byte f_idx = 0;
                byte pbf_idx = 0;

                for (byte i = 0; i < (byte)functionsTree.Nodes[0].Nodes.Count; i++)
                    for (f_idx = 0; f_idx < functionsTree.Nodes[0].Nodes[i].Nodes.Count; f_idx++)
                        for (pbf_idx = 0; pbf_idx < Params.AllowedMethods.Count; pbf_idx++)
                            if (Params.AllowedMethods.Contains(functionsTree.Nodes[0].Nodes[i].Nodes[f_idx].Name))
                            {
                                functionsTree.Nodes[0].Nodes[i].Nodes[f_idx].Checked = true;
                                break;
                            }
            }//try
            catch { }
            finally
            {
                //if (fs != null)
                //{
                //    fs.Close();
                //    fs.Dispose();
                //}
            }
        }

        /* PUBLIC MEMBERS */

        public string GetDescription(string methodName)
        {
            TreeNode[] nodes = functionsTree.Nodes.Find(methodName, true);
            try
            {
                return nodes[0].Text;
            }
            catch { return ""; }
        }

        /* PROPERTIES */
        //public string[][] Functions { get { return functions; } }
        //public Hashtable AllowedMethods { get { return _f; } }
    }
}
