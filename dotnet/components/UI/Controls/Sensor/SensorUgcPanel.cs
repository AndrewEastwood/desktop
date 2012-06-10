using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace components.UI.Controls.SensorUgcPanel
{
    public partial class SensorUgcPanel : UserControl
    {
        private int _sensorType;

        public SensorUgcPanel()
        {
            InitializeComponent();
            this.OnSensorButtonClicked += new SensorButtonClicked(SensorPanel_OnSensorButtonClicked);
        }

        public SensorUgcPanel(int type)
            : this()
        {
            this.SensorType = type;
        }

        private void button_Click(object sender, EventArgs e)
        {
            string btnName = string.Empty;

            try
            {
                if (((Button)sender).Tag != null)
                    btnName = ((Button)sender).Tag.ToString();
            }
            catch { }

            if (btnName.Length != 0)
                this.OnSensorButtonClicked.Invoke(btnName, e);
        }

        
        public delegate void SensorButtonClicked(string buttonName, EventArgs e);

        public event SensorButtonClicked OnSensorButtonClicked;


        private void SensorPanel_OnSensorButtonClicked(string buttonName, EventArgs e)
        {

        }

        public int SensorType
        {
            get
            {
                return _sensorType;
            }
            set
            {
                _sensorType = value;
                switch (value)
                {
                    case 50:
                        {
                            sensor_controls_100.Visible = false;
                            sensor_controls_50.Visible = true;
                            main_container.RowStyles[0].Height = 0; // 100
                            main_container.RowStyles[1].Height = 100; // 50
                            break;
                        }
                    default:
                    case 100:
                        {
                            sensor_controls_100.Visible = true;
                            sensor_controls_50.Visible = false;

                            main_container.RowStyles[0].Height = 100; // 100
                            main_container.RowStyles[1].Height = 0; // 50
                            break;
                        }
                }
            }
        }

        public bool GetComponentVisiblity(SensorComponents component)
        {
            bool visibility = false;
            switch ((int)component)
            {
                case 1:
                    {
                        if (this._sensorType == 100)
                            visibility = this.com_scroll_100.Visible;
                        else
                            visibility = this.com_scroll_50.Visible;
                        break;
                    }
                case 2:
                    {
                        if (this._sensorType == 100)
                            visibility = this.com_oper_100.Visible;
                        else
                            visibility = this.com_oper_50.Visible;
                        break;
                    }
                case 4:
                    {
                        if (this._sensorType == 100)
                            visibility = this.com_add_100.Visible;
                        else
                            visibility = this.com_add_50.Visible;
                        break;
                    }
                case 8:
                    {
                        if (this._sensorType == 100)
                            visibility = this.com_search_100.Visible;
                        else
                            visibility = this.com_search_50.Visible;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            return visibility;
        }

        public bool ShowComponent(SensorComponents component, bool visibility)
        {
            switch ((int)component)
            {
                case 0:
                    {
                        this.com_scroll_100.Visible = com_scroll_50.Visible = visibility;
                        this.com_oper_100.Visible = this.com_oper_50.Visible = visibility;
                        this.com_add_100.Visible = this.com_add_50.Visible = visibility;
                        this.com_search_100.Visible = this.com_search_100.Visible = visibility;
                        break;
                    }
                case 1:
                    {
                        this.com_scroll_100.Visible = com_scroll_50.Visible = visibility;
                        break;
                    }
                case 2:
                    {
                        this.com_oper_100.Visible = this.com_oper_50.Visible = visibility;
                        break;
                    }
                case 4:
                    {
                        this.com_add_100.Visible = this.com_add_50.Visible = visibility;
                        break;
                    }
                case 8:
                    {
                        this.com_search_100.Visible = this.com_search_50.Visible = visibility;
                        break;
                    }
            }

            return visibility;
        }

        public void SetSplitterDistance(string name, int distance)
        {
            switch (name)
            {
                case "h_100":
                    {
                        this.split_h_100.SplitterDistance = distance;
                        break;
                    }
                case "v_100":
                    {
                        this.split_v_100.SplitterDistance = distance;
                        break;
                    }
                case "h_50":
                    {
                        this.split_h_50.SplitterDistance = distance;
                        break;
                    }
                case "v_50":
                    {
                        this.split_v_50.SplitterDistance = distance;
                        break;
                    }
            }
        }

        public SplitContainer GetSplitterControl(string name)
        {
            SplitContainer ctrl = null;

            switch (name)
            {
                case "h_100":
                    {
                        ctrl = this.split_h_100;
                        break;
                    }
                case "v_100":
                    {
                        ctrl = this.split_v_100;
                        break;
                    }
                case "h_50":
                    {
                        ctrl = this.split_h_50;
                        break;
                    }
                case "v_50":
                    {
                        ctrl = this.split_v_50;
                        break;
                    }
            }

            return ctrl;
        }


        public enum SensorComponents : int
        {
            None = 0,
            Scrolling = 1,
            Operations = 2,
            Additional = 4,
            Search = 8
        }

    }
}
