using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace components.UI.Controls
{
    public partial class UploadControl : UserControl
    {
        public UploadControl()
        {
            InitializeComponent();
            this.OnFilePathChanged += new FilePathChangedDelegate(UploadControl_OnFilePathChanged);
        }


        private void button_browse_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog(this) == DialogResult.OK)
                this.textBox1.Text = this.openFileDialog1.FileName;
        }

        private void button_open_Click(object sender, EventArgs e)
        {
            OnFilePathChanged.Invoke(FilePath);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.openFileDialog1.Reset();
        }

        public string FilePath
        {
            get
            {
                if (System.IO.File.Exists(this.textBox1.Text))
                    return this.textBox1.Text;
                return string.Empty;
            }
        }


        void UploadControl_OnFilePathChanged(string file)
        {
            if (string.IsNullOrEmpty(file))
                MessageBox.Show("Select a valid file.");
        }

        public delegate void FilePathChangedDelegate(string file);
        
        public event FilePathChangedDelegate OnFilePathChanged;




    }
}
