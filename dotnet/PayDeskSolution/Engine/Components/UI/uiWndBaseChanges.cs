using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using driver.Config;
//0using mdcore;
//0using mdcore.Config;

namespace PayDesk.Components.UI
{
    public partial class uiWndBaseChanges : Form
    {
        private FileInfo art;
        private FileInfo alt;
        private FileInfo cli;

        public uiWndBaseChanges()
        {
            InitializeComponent();

            art = new FileInfo(ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format("Art_{0:D2}.xml", ConfigManager.Instance.CommonConfiguration.APP_SubUnit));
            alt = new FileInfo(ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format("Alt_{0:D2}.xml", ConfigManager.Instance.CommonConfiguration.APP_SubUnit));
            cli = new FileInfo(ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + "DCards.xml");

            if (art.Exists)
                dateTimePicker1.Value = art.LastWriteTime;

            if (alt.Exists)
                dateTimePicker2.Value = alt.LastWriteTime;

            if (cli.Exists)
                dateTimePicker3.Value = cli.LastWriteTime;

            label1.Enabled = dateTimePicker1.Enabled = art.Exists;
            label2.Enabled = dateTimePicker2.Enabled = alt.Exists;
            label3.Enabled = dateTimePicker3.Enabled = cli.Exists;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (dateTimePicker1.Enabled)
            {
                art.CreationTime = dateTimePicker1.Value;
                art.LastAccessTime = dateTimePicker1.Value;
                art.LastWriteTime = dateTimePicker1.Value;
                ConfigManager.Instance.CommonConfiguration.ADD_updateDateTime[0] = dateTimePicker1.Value;
            }
            if (dateTimePicker2.Enabled)
            {
                alt.CreationTime = dateTimePicker2.Value;
                alt.LastAccessTime = dateTimePicker2.Value;
                alt.LastWriteTime = dateTimePicker2.Value;
                ConfigManager.Instance.CommonConfiguration.ADD_updateDateTime[1] = dateTimePicker2.Value;
            }
            if (dateTimePicker3.Enabled)
            {
                cli.CreationTime = dateTimePicker3.Value;
                cli.LastAccessTime = dateTimePicker3.Value;
                cli.LastWriteTime = dateTimePicker3.Value;
                ConfigManager.Instance.CommonConfiguration.ADD_updateDateTime[2] = dateTimePicker3.Value;
            }
            art.Refresh();
            alt.Refresh();
            cli.Refresh();
            ConfigManager.SaveConfiguration();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void LastDBCh_KeyDown(object sender, KeyEventArgs e)
        {
            KeyEventArgs esc = new KeyEventArgs(Keys.Escape);

            if (e.KeyValue == esc.KeyValue)
            {
                Close();
                return;
            }
        }
    }
}