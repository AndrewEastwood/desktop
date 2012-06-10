namespace mpwsDBU
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.menuStrip_main = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip_file = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.reopenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip_tools = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.sQLPreviewToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.syncToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.syncPreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.backupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel_status = new System.Windows.Forms.ToolStripStatusLabel();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.optionsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.uploadToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_main.SuspendLayout();
            this.contextMenuStrip_file.SuspendLayout();
            this.contextMenuStrip_tools.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Excel Files 97-2003|*.xls|Excel Files 2007|*.xlsx";
            // 
            // tabControl1
            // 
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(10, 34);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(472, 387);
            this.tabControl1.TabIndex = 4;
            // 
            // menuStrip_main
            // 
            this.menuStrip_main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip_main.Location = new System.Drawing.Point(10, 10);
            this.menuStrip_main.Name = "menuStrip_main";
            this.menuStrip_main.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip_main.Size = new System.Drawing.Size(472, 24);
            this.menuStrip_main.TabIndex = 5;
            this.menuStrip_main.Text = "menuStrip1";
            this.menuStrip_main.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip_main_ItemClicked);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDown = this.contextMenuStrip_file;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // contextMenuStrip_file
            // 
            this.contextMenuStrip_file.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem1,
            this.reopenToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem1});
            this.contextMenuStrip_file.Name = "contextMenuStrip_file";
            this.contextMenuStrip_file.Size = new System.Drawing.Size(115, 76);
            this.contextMenuStrip_file.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip_main_ItemClicked);
            // 
            // openToolStripMenuItem1
            // 
            this.openToolStripMenuItem1.Name = "openToolStripMenuItem1";
            this.openToolStripMenuItem1.Size = new System.Drawing.Size(114, 22);
            this.openToolStripMenuItem1.Tag = "file_open";
            this.openToolStripMenuItem1.Text = "&Open";
            // 
            // reopenToolStripMenuItem
            // 
            this.reopenToolStripMenuItem.Image = global::mpwsDBU.Properties.Resources.icon16_refresh;
            this.reopenToolStripMenuItem.Name = "reopenToolStripMenuItem";
            this.reopenToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.reopenToolStripMenuItem.Tag = "file_reopen";
            this.reopenToolStripMenuItem.Text = "Reopen";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(111, 6);
            // 
            // exitToolStripMenuItem1
            // 
            this.exitToolStripMenuItem1.Name = "exitToolStripMenuItem1";
            this.exitToolStripMenuItem1.Size = new System.Drawing.Size(114, 22);
            this.exitToolStripMenuItem1.Tag = "file_exit";
            this.exitToolStripMenuItem1.Text = "Exit";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDown = this.contextMenuStrip_tools;
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // contextMenuStrip_tools
            // 
            this.contextMenuStrip_tools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem1,
            this.uploadToolStripMenuItem1,
            this.sQLPreviewToolStripMenuItem1,
            this.toolStripMenuItem2,
            this.syncToolStripMenuItem,
            this.syncPreviewToolStripMenuItem,
            this.toolStripMenuItem3,
            this.backupToolStripMenuItem,
            this.restoreToolStripMenuItem});
            this.contextMenuStrip_tools.Name = "contextMenuStrip_tools";
            this.contextMenuStrip_tools.OwnerItem = this.toolsToolStripMenuItem;
            this.contextMenuStrip_tools.Size = new System.Drawing.Size(157, 192);
            this.contextMenuStrip_tools.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip_main_ItemClicked);
            // 
            // sQLPreviewToolStripMenuItem1
            // 
            this.sQLPreviewToolStripMenuItem1.Name = "sQLPreviewToolStripMenuItem1";
            this.sQLPreviewToolStripMenuItem1.Size = new System.Drawing.Size(156, 22);
            this.sQLPreviewToolStripMenuItem1.Tag = "tools_sqlpreview";
            this.sQLPreviewToolStripMenuItem1.Text = "Upload &Preview";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(153, 6);
            // 
            // syncToolStripMenuItem
            // 
            this.syncToolStripMenuItem.Image = global::mpwsDBU.Properties.Resources.icon16_communication;
            this.syncToolStripMenuItem.Name = "syncToolStripMenuItem";
            this.syncToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.syncToolStripMenuItem.Tag = "tools_sync";
            this.syncToolStripMenuItem.Text = "Sync";
            // 
            // syncPreviewToolStripMenuItem
            // 
            this.syncPreviewToolStripMenuItem.Name = "syncPreviewToolStripMenuItem";
            this.syncPreviewToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.syncPreviewToolStripMenuItem.Tag = "tools_syncpreview";
            this.syncPreviewToolStripMenuItem.Text = "Sync Preview";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(153, 6);
            // 
            // backupToolStripMenuItem
            // 
            this.backupToolStripMenuItem.Name = "backupToolStripMenuItem";
            this.backupToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.backupToolStripMenuItem.Tag = "tools_backup";
            this.backupToolStripMenuItem.Text = "Backup";
            // 
            // restoreToolStripMenuItem
            // 
            this.restoreToolStripMenuItem.Image = global::mpwsDBU.Properties.Resources.icon16_date;
            this.restoreToolStripMenuItem.Name = "restoreToolStripMenuItem";
            this.restoreToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.restoreToolStripMenuItem.Tag = "tools_restore";
            this.restoreToolStripMenuItem.Text = "Restore";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel_status});
            this.statusStrip1.Location = new System.Drawing.Point(10, 421);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(472, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(39, 17);
            this.toolStripStatusLabel1.Text = "Status";
            // 
            // toolStripStatusLabel_status
            // 
            this.toolStripStatusLabel_status.Name = "toolStripStatusLabel_status";
            this.toolStripStatusLabel_status.Size = new System.Drawing.Size(0, 17);
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.Filter = "Bacup File|*.bak";
            this.openFileDialog2.Title = "Select backup file";
            // 
            // optionsToolStripMenuItem1
            // 
            this.optionsToolStripMenuItem1.Image = global::mpwsDBU.Properties.Resources.icon16_config;
            this.optionsToolStripMenuItem1.Name = "optionsToolStripMenuItem1";
            this.optionsToolStripMenuItem1.Size = new System.Drawing.Size(156, 22);
            this.optionsToolStripMenuItem1.Tag = "tools_options";
            this.optionsToolStripMenuItem1.Text = "&Options";
            // 
            // uploadToolStripMenuItem1
            // 
            this.uploadToolStripMenuItem1.Image = global::mpwsDBU.Properties.Resources.icon16_sign_up;
            this.uploadToolStripMenuItem1.Name = "uploadToolStripMenuItem1";
            this.uploadToolStripMenuItem1.Size = new System.Drawing.Size(156, 22);
            this.uploadToolStripMenuItem1.Tag = "tools_upload";
            this.uploadToolStripMenuItem1.Text = "&Upload";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 453);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip_main);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip_main;
            this.Name = "Form1";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "mpwsDBU";
            this.menuStrip_main.ResumeLayout(false);
            this.menuStrip_main.PerformLayout();
            this.contextMenuStrip_file.ResumeLayout(false);
            this.contextMenuStrip_tools.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.MenuStrip menuStrip_main;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_status;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_file;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_tools;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem uploadToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem sQLPreviewToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem syncToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem syncPreviewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reopenToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem backupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restoreToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
    }
}

