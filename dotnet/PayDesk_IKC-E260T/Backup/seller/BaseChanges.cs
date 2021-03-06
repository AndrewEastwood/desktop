﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using mdcore;

namespace PayDesk
{
    public partial class BaseChanges : Form
    {
        private FileInfo art;
        private FileInfo alt;
        private FileInfo cli;

        public BaseChanges()
        {
            InitializeComponent();

            art = new FileInfo(AppConfig.Path_Articles + "\\" + string.Format("Art_{0:X2}.saf", AppConfig.APP_SubUnit));
            alt = new FileInfo(AppConfig.Path_Articles + "\\" + string.Format("Art_{0:X2}.saf", AppConfig.APP_SubUnit));
            cli = new FileInfo(AppConfig.Path_Articles + "\\" + "DCards.saf");

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
                art.LastWriteTime = dateTimePicker1.Value;
            if (dateTimePicker2.Enabled)
                alt.LastWriteTime = dateTimePicker2.Value;
            if (dateTimePicker3.Enabled)
                cli.LastWriteTime = dateTimePicker3.Value;

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