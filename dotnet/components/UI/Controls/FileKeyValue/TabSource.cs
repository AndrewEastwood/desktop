using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace components.UI.Controls.FileKeyValue
{
    public partial class TabSource : UserControl
    {
        private int _srcid;
        public int SourceID { get { return _srcid; } }
        public Dictionary<string, string> Sources; 

        public TabSource()
        {
            InitializeComponent();

            _srcid = new Random().Next(0, 10000);
            Sources = new Dictionary<string, string>();
        }

        public bool AddSource(DataTable source)
        {
            if (source == null)
                return false;

            TabPage tpg = new TabPage(source.TableName);
            FileKeyValue fkv = new FileKeyValue(source);
            fkv.Dock = DockStyle.Fill;
            tpg.Controls.Add(fkv);
            this.tabControl1.TabPages.Add(tpg);
            return true;
        }

        public bool AddSource(string pathToSource, string title)
        {
            TabPage tpg = new TabPage(title);
            bool fRez = true;

            if (System.IO.File.Exists(pathToSource))
            {
                FileKeyValue fkv = new FileKeyValue(pathToSource);
                fkv.Dock = DockStyle.Fill;
                tpg.Controls.Add(fkv);
                Sources.Add(title, pathToSource);
            }
            else
            {
                Label lbl = new Label();
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Text = "!!!!!!!!!!!!!!!!!!!!!!!!!!!!\r\n\r\nTHERE IS NO EQUIVALENT FILE\r\n\r\nPAY ATTENTION TO THIS ISSUE \r\n IF IT IS A DEFAULT TAB PAGE\r\n\r\n!!!!!!!!!!!!!!!!!!!!!!!!!!!!";
                lbl.Font = new System.Drawing.Font(this.Font, FontStyle.Bold);
                lbl.ForeColor = Color.Red;
                lbl.Padding = new System.Windows.Forms.Padding(100);
                lbl.Dock = DockStyle.Fill;
                tpg.BackColor = Color.LightPink;
                tpg.Controls.Add(lbl);
                fRez = false;

                this.panel1.Padding = new Padding(10);
                this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                this.panel1.BackColor = Color.Red;
            }

            this.tabControl1.TabPages.Add(tpg);

            return fRez;
        }
    }
}
