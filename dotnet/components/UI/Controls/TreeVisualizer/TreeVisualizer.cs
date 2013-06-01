using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace components.UI.Controls.TreeVisualizer
{
    public partial class TreeVisualizer : UserControl
    {
        // app does not have tree deepness more than 3 level 
        TreeNode NodeToBeDelete;
        TreeNode ExchangeTreeNode;
        bool nodeIsLocked;

        public TreeVisualizer()
        {
            InitializeComponent();
        }

        private void uploadControl1_OnFilePathChanged(string file)
        {
            if (string.IsNullOrEmpty(file))
                return;

            try
            {
                this.treeView_custom.Nodes.Clear();
                Components.XmlDocumentParser.Com_XmlDocumentParser cxml = new Components.XmlDocumentParser.Com_XmlDocumentParser();
                Hashtable user = cxml.GetXmlDataFromFile(file);
                CustomTreeLoader((Hashtable)user["productFiltering"]);
            }
            catch { MessageBox.Show("Invalid file.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void uploadControl2_OnFilePathChanged(string file)
        {
            if (string.IsNullOrEmpty(file))
                return;

            try
            {
                this.treeView_app.Nodes.Clear();
                NodeLoader(file);
            }
            catch { MessageBox.Show("Invalid file.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            Components.XmlDocumentParser.Com_XmlDocumentParser cxml = new Components.XmlDocumentParser.Com_XmlDocumentParser();

            Hashtable data = GetCustomNodeData();

            //Lib.CoreLib cl = new Lib.CoreLib();

            //data = cl.SortByKey(data);

            cxml.SetXmlData(data, "userdata");
        }

        //private void DataSordter

        // methods

        private void NodeLoader(string file)
        {
            Hashtable ht = new Hashtable();
            Hashtable ht_level2 = new Hashtable();
            Hashtable ht_level3 = new Hashtable();

            using (System.IO.StreamReader sRd = new System.IO.StreamReader(file, Encoding.Default))
            {

                string line = string.Empty;
                string[] nodeInfo = new string[2];
                while ((line = sRd.ReadLine()) != null)
                {
                    if (line.Length < 11)
                        continue;

                    // key
                    nodeInfo[0] = line.Substring(0, 6).Trim();
                    // title
                    nodeInfo[1] = line.Substring(6);

                    if (nodeInfo[0].Length == 2)
                        ht[nodeInfo[0]] = nodeInfo[1];
                    if (nodeInfo[0].Length == 4)
                        ht_level2[nodeInfo[0]] = nodeInfo[1];
                    if (nodeInfo[0].Length == 6)
                        ht_level3[nodeInfo[0]] = nodeInfo[1];


                }
            }

            // ui tree

            this.treeView_app.Nodes.Clear();
            foreach (DictionaryEntry de in ht)
            {
                this.treeView_app.Nodes.Add(de.Key.ToString(), de.Key + ":" + de.Value);
                this.treeView_app.Nodes[de.Key.ToString()].ToolTipText = de.Value.ToString();
                this.treeView_app.Nodes[de.Key.ToString()].Tag = de.Key.ToString();
            }
            foreach (DictionaryEntry de2 in ht_level2)
            {
                string parentKey = de2.Key.ToString().Substring(0, 2);
                TreeNode[] parents = this.treeView_app.Nodes.Find(parentKey, true);
                if (parents.Length == 1)
                {
                    parents[0].Nodes.Add(de2.Key.ToString(), de2.Key + ":" + de2.Value);
                    parents[0].Nodes[de2.Key.ToString()].ToolTipText = de2.Value.ToString();
                    parents[0].Nodes[de2.Key.ToString()].Tag = de2.Key.ToString();
                }
            }
            foreach (DictionaryEntry de3 in ht_level3)
            {
                string parentKey = de3.Key.ToString().Substring(0, 4);
                TreeNode[] parents = this.treeView_app.Nodes.Find(parentKey, true);
                if (parents.Length == 1)
                {
                    parents[0].Nodes.Add(de3.Key.ToString(), de3.Key + ":" + de3.Value);
                    parents[0].Nodes[de3.Key.ToString()].ToolTipText = de3.Value.ToString();
                    parents[0].Nodes[de3.Key.ToString()].Tag = de3.Key.ToString();
                }
            }

            this.treeView_app.Sort();
        }

        private void CustomTreeLoader(Hashtable user, TreeNode parentNode)
        {
            TreeNode currentNode = null;
            List<string> innerKeys = new List<string>();
            foreach (DictionaryEntry kde in user)
                innerKeys.Add(kde.Key.ToString());
            innerKeys.Sort();
            foreach (string entryKey in innerKeys)
            {
                DictionaryEntry de = new DictionaryEntry(entryKey, user[entryKey]);
                if (de.Value.GetType() != typeof(Hashtable))
                    continue;

                Hashtable currentType = (Hashtable)de.Value;
                if (!currentType.ContainsKey("filter") && !currentType.ContainsKey("title"))
                    continue;

                currentNode = new TreeNode(currentType["filter"] + ":" + currentType["title"]);
                currentNode.Tag = currentType["filter"];
                currentNode.ToolTipText = currentType["title"].ToString();

                CustomTreeLoader((Hashtable)de.Value, currentNode);
                if (parentNode != null)
                    parentNode.Nodes.Add(currentNode);
                else
                    treeView_custom.Nodes.Add(currentNode);

            }
        }

        private void CustomTreeLoader(Hashtable user)
        {
            CustomTreeLoader(user, null);
        }

        private Hashtable GetCustomNodeData()
        {
            Hashtable ht = new Hashtable();
            int idx = 1;
            ht["productFiltering"] = GetCustomNodeData(treeView_custom.Nodes, new Hashtable(), ref idx); 
            return ht;
        }
        private Hashtable GetCustomNodeData(TreeNodeCollection parent, Hashtable container, ref int index)
        {
            Hashtable item = null;
            
            foreach (TreeNode tn in parent)
            {
                item = new Hashtable();
                item["title"] = tn.ToolTipText;
                item["filter"] = tn.Tag.ToString();
                item["level"] = (tn.Level + 1);
                item["showProducts"] = true;

                container["productCategory_" + tn.Tag.ToString()] = item;

                if (tn.Nodes.Count != 0)
                    GetCustomNodeData(tn.Nodes, item, ref index);

                //container["productCategory_" + (index++)] = item;
            }

            return container;
        }

        // events
        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            nodeIsLocked = true;
            this.textBox_nodeFilter.Clear();
            this.textBox_nodeTitle.Clear();
            if (e.Node != null && e.Node.TreeView.Name == this.treeView_custom.Name)
            {
                string[] nodeInfo = e.Node.Text.Split(':');
                this.textBox_nodeFilter.Text = nodeInfo[0];
                this.textBox_nodeTitle.Text = nodeInfo[1];
            }
            nodeIsLocked = false;
        }

        private void treeView_custom_DragOver(object sender, DragEventArgs e)
        {
            if (NodeToBeDelete != null && NodeToBeDelete.Nodes.Count != 0)
            {
                e.Effect = DragDropEffects.None;
            }
            else
            {
                e.Effect = DragDropEffects.Move;
                // try to select node
                try
                {
                    Point Position = treeView_custom.PointToClient(new Point(e.X, e.Y));
                    TreeNode n = treeView_custom.GetNodeAt(Position);
                    if (n != null)
                        treeView_custom.SelectedNode = n;
                    //ParentForm.Text = Position.ToString() + " :: " + (n != null ? n.Text : "null");
                    treeView_custom.Select();
                }
                catch { }
            }
        }

        private void treeView_custom_DragDrop(object sender, DragEventArgs e)
        {
            if (ExchangeTreeNode != null)
            {
                TreeNode parentnode = new TreeNode();
                // parent node
                try
                {
                    Point Position = treeView_custom.PointToClient(new Point(e.X, e.Y));
                    parentnode = treeView_custom.GetNodeAt(Position);
                }
                catch { }

                if (parentnode != null)
                    parentnode.Nodes.Add((TreeNode)ExchangeTreeNode.Clone());
                else
                    this.treeView_custom.Nodes.Add((TreeNode)ExchangeTreeNode.Clone());
            }


            if (NodeToBeDelete != null)
            {
                if (NodeToBeDelete.Nodes.Count != 0)
                {
                    NodeToBeDelete = null;
                    return;
                }

                TreeNode parentnode = new TreeNode();


                // parent node
                try
                {
                    Point Position = treeView_custom.PointToClient(new Point(e.X, e.Y));
                    parentnode = treeView_custom.GetNodeAt(Position);
                }
                catch { }

                if (parentnode != null)
                {
                    parentnode.Nodes.Add((TreeNode)NodeToBeDelete.Clone());
                    treeView_custom.Nodes.Remove(NodeToBeDelete);
                }
                else
                {
                    treeView_custom.Nodes.Add((TreeNode)NodeToBeDelete.Clone());
                    treeView_custom.Nodes.Remove(NodeToBeDelete);
                }

            }
            NodeToBeDelete = null;
            ExchangeTreeNode = null;
        }

        private void treeView_app_ItemDrag(object sender, ItemDragEventArgs e)
        {
            ExchangeTreeNode = (TreeNode)e.Item;
            NodeToBeDelete = null;
            DoDragDrop(e.Item, DragDropEffects.Copy | DragDropEffects.Move);
        }

        private void treeView_custom_ItemDrag(object sender, ItemDragEventArgs e)
        {
            ExchangeTreeNode = null;
            NodeToBeDelete = (TreeNode)e.Item;
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void textBox_nodeInfo_TextChanged(object sender, EventArgs e)
        {
            if (!nodeIsLocked && treeView_custom.SelectedNode != null)
            {
                treeView_custom.SelectedNode.Text = string.Format("{0}:{1}", textBox_nodeFilter.Text, textBox_nodeTitle.Text);
                treeView_custom.SelectedNode.Tag = textBox_nodeFilter.Text;
                treeView_custom.SelectedNode.ToolTipText = textBox_nodeTitle.Text;
            }
        }

        private void button_del_Click(object sender, EventArgs e)
        {
            if (this.treeView_custom.SelectedNode != null)
            {
                if (this.treeView_custom.SelectedNode.Nodes.Count != 0)
                {
                    MessageBox.Show("You can not delete node " + this.treeView_custom.SelectedNode.Text + " with sub-nodes.\r\nDelete all sub-nodes before.", "Node Delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (MessageBox.Show("Do you really want to delete node:\r\n" + this.treeView_custom.SelectedNode.Text, "Node Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;

                this.treeView_custom.Nodes.Remove(this.treeView_custom.SelectedNode);

                if (this.treeView_custom.SelectedNode == null)
                {
                    this.textBox_nodeFilter.Clear();
                    this.textBox_nodeTitle.Clear();
                }

            }
        }

        private void button_addnew_Click(object sender, EventArgs e)
        {
            TreeNode tn = new TreeNode("00000:new");
            tn.Tag = "00000";
            tn.ToolTipText = "new";
            this.treeView_custom.Nodes.Add(tn);
        }

    }
}
