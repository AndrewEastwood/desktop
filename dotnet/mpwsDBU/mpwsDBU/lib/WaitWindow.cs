using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace mpwsDBU.lib
{
    public partial class WaitWindow : Form
    {
        global::components.UI.Controls.Spinner.Spinner spinner1;

        public WaitWindow()
        {
            InitializeComponent();

            spinner1 = new components.UI.Controls.Spinner.Spinner();

            // 
            // spinner1
            // 
            this.spinner1.BackColor = System.Drawing.SystemColors.Window;
            this.spinner1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spinner1.Location = new System.Drawing.Point(0, 0);
            this.spinner1.Name = "spinner1";
            this.spinner1.Segments = 12;
            this.spinner1.Size = new System.Drawing.Size(202, 149);
            this.spinner1.TabIndex = 0;
            this.spinner1.Text = "spinner1";



            this.Controls.Add(spinner1);
            spinner1.Start();
        }
    }
}
