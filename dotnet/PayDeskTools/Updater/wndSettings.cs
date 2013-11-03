using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Updater
{
    public class wndSettings : components.UI.Windows.wndAppSettings.wndAppSettings
    {
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox sync_localPath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown sync_fetchTimer;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RichTextBox sync_monitorFiles;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView sync_dataReaders;
        private System.Windows.Forms.DataGridViewTextBoxColumn Source;
        private System.Windows.Forms.DataGridViewComboBoxColumn Reader;
        private System.Windows.Forms.TextBox sync_remotePath;

        public wndSettings()
            : base()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(wndSettings));
            this.label1 = new System.Windows.Forms.Label();
            this.sync_remotePath = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.sync_localPath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.sync_fetchTimer = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.sync_monitorFiles = new System.Windows.Forms.RichTextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.sync_dataReaders = new System.Windows.Forms.DataGridView();
            this.Source = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Reader = new System.Windows.Forms.DataGridViewComboBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.sync_fetchTimer)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sync_dataReaders)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Remote path";
            // 
            // sync_remotePath
            // 
            this.sync_remotePath.Location = new System.Drawing.Point(6, 19);
            this.sync_remotePath.Name = "sync_remotePath";
            this.sync_remotePath.Size = new System.Drawing.Size(237, 20);
            this.sync_remotePath.TabIndex = 1;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(197, 287);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Local path";
            // 
            // sync_localPath
            // 
            this.sync_localPath.Location = new System.Drawing.Point(6, 58);
            this.sync_localPath.Name = "sync_localPath";
            this.sync_localPath.Size = new System.Drawing.Size(237, 20);
            this.sync_localPath.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Fetch period (.ms)";
            // 
            // sync_fetchTimer
            // 
            this.sync_fetchTimer.Location = new System.Drawing.Point(6, 97);
            this.sync_fetchTimer.Maximum = new decimal(new int[] {
            600000,
            0,
            0,
            0});
            this.sync_fetchTimer.Name = "sync_fetchTimer";
            this.sync_fetchTimer.Size = new System.Drawing.Size(240, 20);
            this.sync_fetchTimer.TabIndex = 6;
            this.sync_fetchTimer.Value = new decimal(new int[] {
            15000,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 120);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Monitor files";
            // 
            // sync_monitorFiles
            // 
            this.sync_monitorFiles.Location = new System.Drawing.Point(6, 136);
            this.sync_monitorFiles.Name = "sync_monitorFiles";
            this.sync_monitorFiles.Size = new System.Drawing.Size(240, 101);
            this.sync_monitorFiles.TabIndex = 8;
            this.sync_monitorFiles.Text = "";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(260, 269);
            this.tabControl1.TabIndex = 9;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.sync_monitorFiles);
            this.tabPage1.Controls.Add(this.sync_remotePath);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.sync_fetchTimer);
            this.tabPage1.Controls.Add(this.sync_localPath);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(252, 243);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.sync_dataReaders);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(252, 243);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Readers";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // sync_dataReaders
            // 
            this.sync_dataReaders.AllowUserToResizeRows = false;
            this.sync_dataReaders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sync_dataReaders.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Source,
            this.Reader});
            this.sync_dataReaders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sync_dataReaders.Location = new System.Drawing.Point(3, 3);
            this.sync_dataReaders.Name = "sync_dataReaders";
            this.sync_dataReaders.Size = new System.Drawing.Size(246, 237);
            this.sync_dataReaders.TabIndex = 0;
            // 
            // Source
            // 
            this.Source.HeaderText = "Source";
            this.Source.Name = "Source";
            // 
            // Reader
            // 
            this.Reader.HeaderText = "Reader";
            this.Reader.Items.AddRange(new object[] {
            "Products",
            "AlternativeBarCodes",
            "ClientCards"});
            this.Reader.Name = "Reader";
            // 
            // wndSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(284, 322);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "wndSettings";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            ((System.ComponentModel.ISupportInitialize)(this.sync_fetchTimer)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sync_dataReaders)).EndInit();
            this.ResumeLayout(false);

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            //this.SaveControlSettings();
            this.Close();
        }
    }
}
