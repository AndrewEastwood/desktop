namespace mpwsDBU
{
    partial class excelDataView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip_tableCommands = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.insertSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuStrip_tableCommands.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.ContextMenuStrip = this.contextMenuStrip_tableCommands;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(150, 150);
            this.dataGridView1.TabIndex = 0;
            // 
            // contextMenuStrip_tableCommands
            // 
            this.contextMenuStrip_tableCommands.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertSelectedToolStripMenuItem,
            this.updateSelectedToolStripMenuItem,
            this.deleteSelectedToolStripMenuItem});
            this.contextMenuStrip_tableCommands.Name = "contextMenuStrip_tableCommands";
            this.contextMenuStrip_tableCommands.Size = new System.Drawing.Size(160, 92);
            this.contextMenuStrip_tableCommands.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip_tableCommands_ItemClicked);
            // 
            // insertSelectedToolStripMenuItem
            // 
            this.insertSelectedToolStripMenuItem.Name = "insertSelectedToolStripMenuItem";
            this.insertSelectedToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.insertSelectedToolStripMenuItem.Tag = "insert_selected";
            this.insertSelectedToolStripMenuItem.Text = "Insert Selected";
            // 
            // updateSelectedToolStripMenuItem
            // 
            this.updateSelectedToolStripMenuItem.Name = "updateSelectedToolStripMenuItem";
            this.updateSelectedToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.updateSelectedToolStripMenuItem.Tag = "update_selected";
            this.updateSelectedToolStripMenuItem.Text = "Update Selected";
            // 
            // deleteSelectedToolStripMenuItem
            // 
            this.deleteSelectedToolStripMenuItem.Name = "deleteSelectedToolStripMenuItem";
            this.deleteSelectedToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.deleteSelectedToolStripMenuItem.Tag = "delete_selected";
            this.deleteSelectedToolStripMenuItem.Text = "Delete Selected";
            // 
            // excelDataView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridView1);
            this.Name = "excelDataView";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.contextMenuStrip_tableCommands.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_tableCommands;
        private System.Windows.Forms.ToolStripMenuItem insertSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteSelectedToolStripMenuItem;
    }
}
