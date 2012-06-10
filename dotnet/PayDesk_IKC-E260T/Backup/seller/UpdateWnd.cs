using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PayDesk
{
    public partial class UpdateWnd : Form
    {
        public UpdateWnd(bool onlyUpdate)
        {
            InitializeComponent();
            if (!onlyUpdate)
                label1.Text = "Відбувається завантаження бази товарів";
        }

        public void ShowUpdate(IWin32Window owner)
        {
            int x = (owner as Form).Width - this.Width;
            int y = (owner as Form).Height - this.Height;

            this.Location = new Point(x / 2, y / 2);
            this.Show();
        }
    }
}