using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace components.UI.Controls.FileKeyValue
{
    public partial class FileKeyValue : UserControl
    {
        public Hashtable Data { get; set; }
        public string SourceFile { get; set; }
        private Components.DataReader.Com_DataReader dReader;

        public FileKeyValue()
        {
            InitializeComponent();

            dReader = new Components.DataReader.Com_DataReader();
            Data = new Hashtable();
            this.SourceFile = string.Empty;

        }

        public FileKeyValue(string file)
            : this()
        {
            this.SourceFile = file;
            Data = GetData(SourceFile);

            this.textBox1.Text = SourceFile;
            this.dataGridView1.DataSource = GetTableData(SourceFile);
            this.Name = System.IO.Path.GetFileName(SourceFile);
            UpdateGridView();
        }

        public FileKeyValue(DataTable source)
            : this()
        {
            this.dataGridView1.DataSource = source;
            this.Name = source.TableName;
        }

        public DataTable GetTableData(string path)
        {
            this.SourceFile = path;
            return dReader.GetTableData(this.SourceFile, "PKEY", "PVAL");
        }

        public Hashtable GetData(string path)
        {
            this.SourceFile = path;
            return dReader.GetData(this.SourceFile);
        }

        private void UpdateGridView()
        {
            this.dataGridView1.Columns["IDX"].HeaderText = "#";
            this.dataGridView1.Columns["PKEY"].HeaderText = "Property Key";
            this.dataGridView1.Columns["PVAL"].HeaderText = "Value";
            this.dataGridView1.Columns["IDX"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dataGridView1.Columns["PKEY"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dataGridView1.Columns["PVAL"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }
    }
}
