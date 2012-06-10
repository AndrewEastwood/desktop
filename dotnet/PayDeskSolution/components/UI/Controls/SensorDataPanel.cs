using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace components.UI.Controls
{
    public partial class SensorDataPanel : UserControl
    {
        public SensorDataPanel()
        {
            InitializeComponent();
        }

        public CategoryNavBar Navigator
        {
            get { return this.categoryNavBar1; }
            set { this.categoryNavBar1 = value; }
        }

        public Panel Placeholder
        {
            get { return this.dataPlaceholder; }
            set { this.dataPlaceholder = value; }
        }

        public SplitContainer Container
        {
            get { return this.mainContainer; }
            set { this.mainContainer = value; }
        }

        public TableLayoutPanel Scroller
        {
            get { return this.tableLayoutPanel2; }
            set { this.tableLayoutPanel2 = value; }
        }

        private void button_sensor_productTop_Click(object sender, EventArgs e)
        {
            if (dataPlaceholder.Controls.Count != 0 && dataPlaceholder.Controls[0] != null)
            {
                dataPlaceholder.Controls[0].Select();
                SendKeys.SendWait("{UP}");
            }
        }

        private void button_sensor_productBottom_Click(object sender, EventArgs e)
        {
            if (dataPlaceholder.Controls.Count != 0 && dataPlaceholder.Controls[0] != null)
            {
                dataPlaceholder.Controls[0].Select();
                SendKeys.SendWait("{DOWN}");
            }
        }


        // control states
        private void saveControlGUI()
        {
        }
    }
}
