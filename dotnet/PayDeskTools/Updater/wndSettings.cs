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
            ((System.ComponentModel.ISupportInitialize)(this.sync_fetchTimer)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Remote path";
            // 
            // sync_remotePath
            // 
            this.sync_remotePath.Location = new System.Drawing.Point(12, 25);
            this.sync_remotePath.Name = "sync_remotePath";
            this.sync_remotePath.Size = new System.Drawing.Size(257, 20);
            this.sync_remotePath.TabIndex = 1;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(197, 229);
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
            this.label2.Location = new System.Drawing.Point(9, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Local path";
            // 
            // sync_localPath
            // 
            this.sync_localPath.Location = new System.Drawing.Point(12, 64);
            this.sync_localPath.Name = "sync_localPath";
            this.sync_localPath.Size = new System.Drawing.Size(257, 20);
            this.sync_localPath.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Fetch timer (.ms)";
            // 
            // sync_fetchTimer
            // 
            this.sync_fetchTimer.Location = new System.Drawing.Point(12, 103);
            this.sync_fetchTimer.Maximum = new decimal(new int[] {
            600000,
            0,
            0,
            0});
            this.sync_fetchTimer.Name = "sync_fetchTimer";
            this.sync_fetchTimer.Size = new System.Drawing.Size(260, 20);
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
            this.label4.Location = new System.Drawing.Point(12, 126);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Monitor files";
            // 
            // sync_monitorFiles
            // 
            this.sync_monitorFiles.Location = new System.Drawing.Point(12, 142);
            this.sync_monitorFiles.Name = "sync_monitorFiles";
            this.sync_monitorFiles.Size = new System.Drawing.Size(260, 81);
            this.sync_monitorFiles.TabIndex = 8;
            this.sync_monitorFiles.Text = "";
            // 
            // wndSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(284, 264);
            this.Controls.Add(this.sync_monitorFiles);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.sync_fetchTimer);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.sync_localPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.sync_remotePath);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "wndSettings";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            ((System.ComponentModel.ISupportInitialize)(this.sync_fetchTimer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            //this.SaveControlSettings();
            this.Close();
        }
    }
}
