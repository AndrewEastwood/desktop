namespace Updater
{
    partial class DataSyncProfile
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.sync_monitorFiles = new System.Windows.Forms.RichTextBox();
            this.sync_remotePath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.sync_localPath = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.sync_dataReaders = new System.Windows.Forms.DataGridView();
            this.Source = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Reader = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.label6 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.sync_dataTransform = new System.Windows.Forms.DataGridView();
            this.SourceList = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DestinationName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Function = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.sync_profileDisplayText = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sync_dataReaders)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sync_dataTransform)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(3, 42);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(500, 299);
            this.tabControl1.TabIndex = 10;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.sync_monitorFiles);
            this.tabPage1.Controls.Add(this.sync_remotePath);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.sync_localPath);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(492, 273);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Загальні";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Віддалена папка";
            // 
            // sync_monitorFiles
            // 
            this.sync_monitorFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sync_monitorFiles.Location = new System.Drawing.Point(6, 97);
            this.sync_monitorFiles.Name = "sync_monitorFiles";
            this.sync_monitorFiles.Size = new System.Drawing.Size(480, 170);
            this.sync_monitorFiles.TabIndex = 8;
            this.sync_monitorFiles.Text = "";
            // 
            // sync_remotePath
            // 
            this.sync_remotePath.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sync_remotePath.Location = new System.Drawing.Point(6, 19);
            this.sync_remotePath.Name = "sync_remotePath";
            this.sync_remotePath.Size = new System.Drawing.Size(480, 20);
            this.sync_remotePath.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 81);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(139, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Список віддалених файлів";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Локальна папка";
            // 
            // sync_localPath
            // 
            this.sync_localPath.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sync_localPath.Location = new System.Drawing.Point(6, 58);
            this.sync_localPath.Name = "sync_localPath";
            this.sync_localPath.Size = new System.Drawing.Size(480, 20);
            this.sync_localPath.TabIndex = 4;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.sync_dataReaders);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(492, 273);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Перетворення";
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
            this.sync_dataReaders.Location = new System.Drawing.Point(3, 33);
            this.sync_dataReaders.Name = "sync_dataReaders";
            this.sync_dataReaders.Size = new System.Drawing.Size(486, 237);
            this.sync_dataReaders.TabIndex = 0;
            // 
            // Source
            // 
            this.Source.HeaderText = "Файл для пететворення";
            this.Source.Name = "Source";
            this.Source.Width = 270;
            // 
            // Reader
            // 
            this.Reader.HeaderText = "Функція";
            this.Reader.Items.AddRange(new object[] {
            "Products",
            "AlternativeBarCodes",
            "ClientCards"});
            this.Reader.Name = "Reader";
            this.Reader.Width = 150;
            // 
            // label6
            // 
            this.label6.Dock = System.Windows.Forms.DockStyle.Top;
            this.label6.Location = new System.Drawing.Point(3, 3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(486, 30);
            this.label6.TabIndex = 7;
            this.label6.Text = "Вкажіть список завантажених файлів для петворення";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.sync_dataTransform);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(492, 273);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Сумісність";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // sync_dataTransform
            // 
            this.sync_dataTransform.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sync_dataTransform.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SourceList,
            this.DestinationName,
            this.Function});
            this.sync_dataTransform.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sync_dataTransform.Location = new System.Drawing.Point(3, 33);
            this.sync_dataTransform.Name = "sync_dataTransform";
            this.sync_dataTransform.Size = new System.Drawing.Size(486, 237);
            this.sync_dataTransform.TabIndex = 5;
            // 
            // SourceList
            // 
            this.SourceList.HeaderText = "Список файлів";
            this.SourceList.Name = "SourceList";
            this.SourceList.Width = 180;
            // 
            // DestinationName
            // 
            this.DestinationName.HeaderText = "Кінцева назва файлу";
            this.DestinationName.Name = "DestinationName";
            this.DestinationName.Width = 150;
            // 
            // Function
            // 
            this.Function.HeaderText = "Фукнкція";
            this.Function.Items.AddRange(new object[] {
            "JoinedProducts",
            "JoinedAlternativeBC",
            "JoinedClientCards"});
            this.Function.Name = "Function";
            // 
            // label5
            // 
            this.label5.Dock = System.Windows.Forms.DockStyle.Top;
            this.label5.Location = new System.Drawing.Point(3, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(486, 30);
            this.label5.TabIndex = 6;
            this.label5.Text = "Вкажіть список перетворених файлів для створення сумісних";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Назва профілю";
            // 
            // sync_profileDisplayText
            // 
            this.sync_profileDisplayText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sync_profileDisplayText.Location = new System.Drawing.Point(3, 16);
            this.sync_profileDisplayText.Name = "sync_profileDisplayText";
            this.sync_profileDisplayText.Size = new System.Drawing.Size(500, 20);
            this.sync_profileDisplayText.TabIndex = 14;
            this.sync_profileDisplayText.Text = "Новий профіль";
            this.sync_profileDisplayText.TextChanged += new System.EventHandler(this.sync_profileDisplayText_TextChanged);
            // 
            // DataSyncProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.sync_profileDisplayText);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tabControl1);
            this.Name = "DataSyncProfile";
            this.Size = new System.Drawing.Size(506, 344);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sync_dataReaders)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sync_dataTransform)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox sync_monitorFiles;
        private System.Windows.Forms.TextBox sync_remotePath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox sync_localPath;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView sync_dataReaders;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox sync_profileDisplayText;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataGridView sync_dataTransform;
        private System.Windows.Forms.DataGridViewTextBoxColumn SourceList;
        private System.Windows.Forms.DataGridViewTextBoxColumn DestinationName;
        private System.Windows.Forms.DataGridViewComboBoxColumn Function;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Source;
        private System.Windows.Forms.DataGridViewComboBoxColumn Reader;
        private System.Windows.Forms.Label label6;
    }
}
