using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace FPService.Protocol.IKSE260T
{
    public partial class Tree : UserControl
    {
        private string protocolName;
        private string[][] publicFunctions;

        public Tree(string protocolName)
        {
            InitializeComponent();
            this.protocolName = protocolName;
            publicFunctions = Methods.RestorePublic(protocolName, ref functionsTree);
        }

        private void functionsTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            for (int i = 0; i < e.Node.Nodes.Count; i++)
                e.Node.Nodes[i].Checked = e.Node.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte f_idx = 0;

            publicFunctions[0] = new string[0];
            publicFunctions[1] = new string[0];

            for (byte i = 0; i < (byte)functionsTree.Nodes[0].Nodes.Count; i++)
            {
                for (f_idx = 0; f_idx < functionsTree.Nodes[0].Nodes[i].Nodes.Count; f_idx++)
                {
                    if (functionsTree.Nodes[0].Nodes[i].Nodes[f_idx].Checked)
                    {
                        Array.Resize<string>(ref publicFunctions[0], publicFunctions[0].Length + 1);
                        Array.Resize<string>(ref publicFunctions[1], publicFunctions[1].Length + 1);
                        publicFunctions[0][publicFunctions[0].Length - 1] = functionsTree.Nodes[0].Nodes[i].Nodes[f_idx].Text;
                        publicFunctions[1][publicFunctions[1].Length - 1] = functionsTree.Nodes[0].Nodes[i].Nodes[f_idx].Name;
                    }
                }
            }

            Methods.SavePublicFunctions(protocolName, publicFunctions);
        }

        public string GetDescription(string methodName)
        {
            TreeNode[] nodes = functionsTree.Nodes.Find(methodName, true);
            try
            {
                return nodes[0].Text;
            }
            catch { return ""; }
        }

        public string[][] PublicFunctions
        {
            get
            {
                return publicFunctions;
            }
        }
    }
}
