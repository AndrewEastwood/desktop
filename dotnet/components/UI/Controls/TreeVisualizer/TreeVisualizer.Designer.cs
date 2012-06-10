namespace components.UI.Controls.TreeVisualizer
{
    partial class TreeVisualizer
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView_custom = new System.Windows.Forms.TreeView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.button_del = new System.Windows.Forms.Button();
            this.button_save = new System.Windows.Forms.Button();
            this.button_addnew = new System.Windows.Forms.Button();
            this.uploadControl1 = new components.UI.Controls.UploadControl.UploadControl();
            this.label1 = new System.Windows.Forms.Label();
            this.treeView_app = new System.Windows.Forms.TreeView();
            this.uploadControl2 = new components.UI.Controls.UploadControl.UploadControl();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox_nodeFilter = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_nodeTitle = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(10, 10);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView_custom);
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Panel1.Controls.Add(this.uploadControl1);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1MinSize = 300;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.treeView_app);
            this.splitContainer1.Panel2.Controls.Add(this.uploadControl2);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Size = new System.Drawing.Size(536, 373);
            this.splitContainer1.SplitterDistance = 300;
            this.splitContainer1.TabIndex = 1;
            // 
            // treeView_custom
            // 
            this.treeView_custom.AllowDrop = true;
            this.treeView_custom.CheckBoxes = true;
            this.treeView_custom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_custom.Location = new System.Drawing.Point(0, 64);
            this.treeView_custom.Name = "treeView_custom";
            this.treeView_custom.Size = new System.Drawing.Size(300, 241);
            this.treeView_custom.TabIndex = 2;
            this.treeView_custom.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView_custom_ItemDrag);
            this.treeView_custom.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseClick);
            this.treeView_custom.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView_custom_DragDrop);
            this.treeView_custom.DragOver += new System.Windows.Forms.DragEventHandler(this.treeView_custom_DragOver);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.button_del, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.button_save, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.button_addnew, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 305);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(300, 68);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // button_del
            // 
            this.button_del.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.button_del.Image = global::components.Properties.Resources.error;
            this.button_del.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_del.Location = new System.Drawing.Point(3, 3);
            this.button_del.Name = "button_del";
            this.button_del.Size = new System.Drawing.Size(94, 62);
            this.button_del.TabIndex = 4;
            this.button_del.Text = "            Delete";
            this.button_del.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_del.UseVisualStyleBackColor = true;
            this.button_del.Click += new System.EventHandler(this.button_del_Click);
            // 
            // button_save
            // 
            this.button_save.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.button_save.Image = global::components.Properties.Resources.checkmark;
            this.button_save.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_save.Location = new System.Drawing.Point(203, 3);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(94, 62);
            this.button_save.TabIndex = 2;
            this.button_save.Text = "           Save";
            this.button_save.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // button_addnew
            // 
            this.button_addnew.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_addnew.Image = global::components.Properties.Resources.add;
            this.button_addnew.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_addnew.Location = new System.Drawing.Point(103, 3);
            this.button_addnew.Name = "button_addnew";
            this.button_addnew.Size = new System.Drawing.Size(94, 62);
            this.button_addnew.TabIndex = 5;
            this.button_addnew.Text = "           Add new";
            this.button_addnew.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_addnew.UseVisualStyleBackColor = true;
            this.button_addnew.Click += new System.EventHandler(this.button_addnew_Click);
            // 
            // uploadControl1
            // 
            this.uploadControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.uploadControl1.Location = new System.Drawing.Point(0, 16);
            this.uploadControl1.MaximumSize = new System.Drawing.Size(1000, 48);
            this.uploadControl1.MinimumSize = new System.Drawing.Size(200, 48);
            this.uploadControl1.Name = "uploadControl1";
            this.uploadControl1.Padding = new System.Windows.Forms.Padding(10);
            this.uploadControl1.Size = new System.Drawing.Size(300, 48);
            this.uploadControl1.TabIndex = 0;
            this.uploadControl1.OnFilePathChanged += new components.UI.Controls.UploadControl.UploadControl.FilePathChangedDelegate(this.uploadControl1_OnFilePathChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.label1.Size = new System.Drawing.Size(54, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "User Tree";
            // 
            // treeView_app
            // 
            this.treeView_app.CheckBoxes = true;
            this.treeView_app.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_app.Location = new System.Drawing.Point(0, 64);
            this.treeView_app.Name = "treeView_app";
            this.treeView_app.Size = new System.Drawing.Size(232, 309);
            this.treeView_app.TabIndex = 2;
            this.treeView_app.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView_app_ItemDrag);
            this.treeView_app.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseClick);
            // 
            // uploadControl2
            // 
            this.uploadControl2.Dock = System.Windows.Forms.DockStyle.Top;
            this.uploadControl2.Location = new System.Drawing.Point(0, 16);
            this.uploadControl2.MaximumSize = new System.Drawing.Size(1000, 48);
            this.uploadControl2.MinimumSize = new System.Drawing.Size(200, 48);
            this.uploadControl2.Name = "uploadControl2";
            this.uploadControl2.Padding = new System.Windows.Forms.Padding(10);
            this.uploadControl2.Size = new System.Drawing.Size(232, 48);
            this.uploadControl2.TabIndex = 0;
            this.uploadControl2.OnFilePathChanged += new components.UI.Controls.UploadControl.UploadControl.FilePathChangedDelegate(this.uploadControl2_OnFilePathChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.label2.Size = new System.Drawing.Size(54, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Raw Tree";
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(10, 383);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5, 5, 5, 8);
            this.panel1.Size = new System.Drawing.Size(536, 122);
            this.panel1.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.textBox_nodeFilter);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textBox_nodeTitle);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(5, 5);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(10, 10, 10, 15);
            this.groupBox1.Size = new System.Drawing.Size(526, 109);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Item Properties";
            // 
            // textBox_nodeFilter
            // 
            this.textBox_nodeFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBox_nodeFilter.Location = new System.Drawing.Point(10, 74);
            this.textBox_nodeFilter.Name = "textBox_nodeFilter";
            this.textBox_nodeFilter.Size = new System.Drawing.Size(506, 20);
            this.textBox_nodeFilter.TabIndex = 7;
            this.textBox_nodeFilter.TextChanged += new System.EventHandler(this.textBox_nodeInfo_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Top;
            this.label4.Location = new System.Drawing.Point(10, 56);
            this.label4.Name = "label4";
            this.label4.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.label4.Size = new System.Drawing.Size(29, 18);
            this.label4.TabIndex = 6;
            this.label4.Text = "Filter";
            // 
            // textBox_nodeTitle
            // 
            this.textBox_nodeTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBox_nodeTitle.Location = new System.Drawing.Point(10, 36);
            this.textBox_nodeTitle.Name = "textBox_nodeTitle";
            this.textBox_nodeTitle.Size = new System.Drawing.Size(506, 20);
            this.textBox_nodeTitle.TabIndex = 5;
            this.textBox_nodeTitle.TextChanged += new System.EventHandler(this.textBox_nodeInfo_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Location = new System.Drawing.Point(10, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Title";
            // 
            // TreeVisualizer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Name = "TreeVisualizer";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(556, 515);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UploadControl.UploadControl uploadControl1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView_custom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView treeView_app;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox_nodeFilter;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_nodeTitle;
        private System.Windows.Forms.Label label3;
        private UploadControl.UploadControl uploadControl2;
        private System.Windows.Forms.Button button_del;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button button_addnew;


    }
}
