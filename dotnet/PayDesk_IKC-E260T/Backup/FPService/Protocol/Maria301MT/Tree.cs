using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace FPService.Protocol.Maria301MT
{
    public partial class Tree : UserControl
    {

        public Tree()
        {
            InitializeComponent();
        }

        public TreeView FP_Tree
        {
            get { return this.treeView1; }
            set { this.treeView1 = value; }
        }

    }
}
