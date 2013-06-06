using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace components.UI.Controls.UploadControl
{
    public partial class UploadControl : UserControl
    {
        private bool showFolderBrowser;

        public UploadControl()
        {
            InitializeComponent();
            this.OnFilePathChanged += new FilePathChangedDelegate(UploadControl_OnFilePathChanged);
        }


        private void button_browse_Click(object sender, EventArgs e)
        {
            // folder browser
            if (showFolderBrowser && this.folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
                this.textBox1.Text = this.folderBrowserDialog1.SelectedPath;
            //file browser
            if (!showFolderBrowser && this.openFileDialog1.ShowDialog(this) == DialogResult.OK)
                this.textBox1.Text = this.openFileDialog1.FileName;
        }

        private void button_open_Click(object sender, EventArgs e)
        {
            OnFilePathChanged.Invoke(FilePath);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.openFileDialog1.Reset();
            this.folderBrowserDialog1.Reset();
        }

        public string FilePath
        {
            get
            {
                if (showFolderBrowser && System.IO.Directory.Exists(this.textBox1.Text))
                    return this.textBox1.Text;
                if (!showFolderBrowser && System.IO.File.Exists(this.textBox1.Text))
                    return this.textBox1.Text;
                return string.Empty;
            }
        }

        void UploadControl_OnFilePathChanged(string file)
        {
            if (string.IsNullOrEmpty(file))
                MessageBox.Show("Select a valid file.");
        }

        public void initSelectedPath(string path)
        {
            this.textBox1.Text = path;
        }

        public delegate void FilePathChangedDelegate(string file);
        
        public event FilePathChangedDelegate OnFilePathChanged;

        public bool ShowFolderBrowser { get { return showFolderBrowser; } set { showFolderBrowser = value; } }
    }
}
