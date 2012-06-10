using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using components.Public;

namespace mpwsDBU
{
    public partial class settings : components.UI.Windows.AppSettingsWindow.AppSettingsWindow
    {
        public settings()
            : base()
        {
            InitializeComponent();
        }

        #region Windows Form Designer generated code


        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.DataGridView mappingConfiguration_TableField;
        private System.Windows.Forms.DataGridView mappingConfiguration_TableName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox formatConfiguration_escapeValues;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox formatConfiguration_valueFormat;
        private System.Windows.Forms.TextBox formatConfiguration_columnFormat;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox connectionConfiguration_server;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox connectionConfiguration_database;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox connectionConfiguration_login;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox connectionConfiguration_password;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox formatConfiguration_useRecordIdAsRowIndex;
        private System.Windows.Forms.ComboBox formatConfiguration_uploadMode;
        private System.Windows.Forms.CheckBox formatConfiguration_affectSelectedRows;
        private System.Windows.Forms.CheckBox formatConfiguration_alwaysTruncate;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.TextBox mappingConfiguration_defaultIndexFiledName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.DataGridView mappingConfiguration_TableValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn Field;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSourceValue;
        private System.Windows.Forms.TabPage tabPage8;
        private System.Windows.Forms.DataGridView mappingConfiguration_TableDefaultValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn DefFieldName;
        private System.Windows.Forms.DataGridViewTextBoxColumn DefFieldVal;
        private System.Windows.Forms.TabPage tabPage9;
        private System.Windows.Forms.DataGridView mappingConfiguration_Indexes;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnTableOwner;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnIdxName;
        private System.Windows.Forms.TabPage tabPage10;
        private System.Windows.Forms.DataGridView commandsConfiguration_sqlCommandList;
        private System.Windows.Forms.CheckBox mappingConfiguration_useCustomIndexes;
        private System.Windows.Forms.CheckBox formatConfiguration_autoBackup;
        private System.Windows.Forms.DataGridViewTextBoxColumn Command;
        private System.Windows.Forms.DataGridViewTextBoxColumn ComdScript;
        private DataGridViewTextBoxColumn ColumnSource;
        private DataGridView mappingConfiguration_customDataFields;
        private DataGridViewTextBoxColumn AdditionalColumnOwner;
        private DataGridViewTextBoxColumn AdditionalFieldColumnName;
        private DataGridViewTextBoxColumn AdditionalFieldColumnValue;
        private Label label8;
        private DataGridViewTextBoxColumn ColumnDB;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_save = new System.Windows.Forms.Button();
            this.mappingConfiguration_TableField = new System.Windows.Forms.DataGridView();
            this.ColumnSource = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDB = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mappingConfiguration_TableName = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.formatConfiguration_autoBackup = new System.Windows.Forms.CheckBox();
            this.formatConfiguration_alwaysTruncate = new System.Windows.Forms.CheckBox();
            this.formatConfiguration_affectSelectedRows = new System.Windows.Forms.CheckBox();
            this.formatConfiguration_uploadMode = new System.Windows.Forms.ComboBox();
            this.formatConfiguration_escapeValues = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.formatConfiguration_valueFormat = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.formatConfiguration_columnFormat = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.formatConfiguration_useRecordIdAsRowIndex = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.connectionConfiguration_password = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.connectionConfiguration_login = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.connectionConfiguration_database = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.connectionConfiguration_server = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.mappingConfiguration_TableValue = new System.Windows.Forms.DataGridView();
            this.Field = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSourceValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.mappingConfiguration_defaultIndexFiledName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.mappingConfiguration_TableDefaultValue = new System.Windows.Forms.DataGridView();
            this.DefFieldName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DefFieldVal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage9 = new System.Windows.Forms.TabPage();
            this.mappingConfiguration_Indexes = new System.Windows.Forms.DataGridView();
            this.ColumnTableOwner = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnIdxName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mappingConfiguration_useCustomIndexes = new System.Windows.Forms.CheckBox();
            this.tabPage10 = new System.Windows.Forms.TabPage();
            this.commandsConfiguration_sqlCommandList = new System.Windows.Forms.DataGridView();
            this.Command = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ComdScript = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mappingConfiguration_customDataFields = new System.Windows.Forms.DataGridView();
            this.label8 = new System.Windows.Forms.Label();
            this.AdditionalColumnOwner = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AdditionalFieldColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AdditionalFieldColumnValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.mappingConfiguration_TableField)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mappingConfiguration_TableName)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mappingConfiguration_TableValue)).BeginInit();
            this.tabPage6.SuspendLayout();
            this.tabPage8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mappingConfiguration_TableDefaultValue)).BeginInit();
            this.tabPage9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mappingConfiguration_Indexes)).BeginInit();
            this.tabPage10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.commandsConfiguration_sqlCommandList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mappingConfiguration_customDataFields)).BeginInit();
            this.SuspendLayout();
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(259, 396);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(150, 23);
            this.button_save.TabIndex = 0;
            this.button_save.Text = "save and close";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // mappingConfiguration_TableField
            // 
            this.mappingConfiguration_TableField.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mappingConfiguration_TableField.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnSource,
            this.ColumnDB});
            this.mappingConfiguration_TableField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mappingConfiguration_TableField.Location = new System.Drawing.Point(3, 3);
            this.mappingConfiguration_TableField.Name = "mappingConfiguration_TableField";
            this.mappingConfiguration_TableField.Size = new System.Drawing.Size(369, 310);
            this.mappingConfiguration_TableField.TabIndex = 1;
            // 
            // ColumnSource
            // 
            this.ColumnSource.HeaderText = "Excel";
            this.ColumnSource.Name = "ColumnSource";
            // 
            // ColumnDB
            // 
            this.ColumnDB.HeaderText = "DataBase";
            this.ColumnDB.Name = "ColumnDB";
            // 
            // mappingConfiguration_TableName
            // 
            this.mappingConfiguration_TableName.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mappingConfiguration_TableName.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.mappingConfiguration_TableName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mappingConfiguration_TableName.Location = new System.Drawing.Point(3, 3);
            this.mappingConfiguration_TableName.Name = "mappingConfiguration_TableName";
            this.mappingConfiguration_TableName.Size = new System.Drawing.Size(369, 310);
            this.mappingConfiguration_TableName.TabIndex = 1;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Excel Page Name";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 150;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "DataBase";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 150;
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.formatConfiguration_autoBackup);
            this.groupBox1.Controls.Add(this.formatConfiguration_alwaysTruncate);
            this.groupBox1.Controls.Add(this.formatConfiguration_affectSelectedRows);
            this.groupBox1.Controls.Add(this.formatConfiguration_uploadMode);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(20, 20);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(30, 10, 50, 10);
            this.groupBox1.Size = new System.Drawing.Size(349, 135);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Upload Mode";
            // 
            // formatConfiguration_autoBackup
            // 
            this.formatConfiguration_autoBackup.AutoSize = true;
            this.formatConfiguration_autoBackup.Dock = System.Windows.Forms.DockStyle.Top;
            this.formatConfiguration_autoBackup.Location = new System.Drawing.Point(30, 98);
            this.formatConfiguration_autoBackup.Name = "formatConfiguration_autoBackup";
            this.formatConfiguration_autoBackup.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.formatConfiguration_autoBackup.Size = new System.Drawing.Size(269, 27);
            this.formatConfiguration_autoBackup.TabIndex = 5;
            this.formatConfiguration_autoBackup.Text = "auto backup";
            this.formatConfiguration_autoBackup.UseVisualStyleBackColor = true;
            // 
            // formatConfiguration_alwaysTruncate
            // 
            this.formatConfiguration_alwaysTruncate.AutoSize = true;
            this.formatConfiguration_alwaysTruncate.Dock = System.Windows.Forms.DockStyle.Top;
            this.formatConfiguration_alwaysTruncate.Location = new System.Drawing.Point(30, 71);
            this.formatConfiguration_alwaysTruncate.Name = "formatConfiguration_alwaysTruncate";
            this.formatConfiguration_alwaysTruncate.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.formatConfiguration_alwaysTruncate.Size = new System.Drawing.Size(269, 27);
            this.formatConfiguration_alwaysTruncate.TabIndex = 4;
            this.formatConfiguration_alwaysTruncate.Text = "always truncate data";
            this.formatConfiguration_alwaysTruncate.UseVisualStyleBackColor = true;
            // 
            // formatConfiguration_affectSelectedRows
            // 
            this.formatConfiguration_affectSelectedRows.AutoSize = true;
            this.formatConfiguration_affectSelectedRows.Dock = System.Windows.Forms.DockStyle.Top;
            this.formatConfiguration_affectSelectedRows.Location = new System.Drawing.Point(30, 44);
            this.formatConfiguration_affectSelectedRows.Name = "formatConfiguration_affectSelectedRows";
            this.formatConfiguration_affectSelectedRows.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.formatConfiguration_affectSelectedRows.Size = new System.Drawing.Size(269, 27);
            this.formatConfiguration_affectSelectedRows.TabIndex = 3;
            this.formatConfiguration_affectSelectedRows.Text = "force affect only selected rows";
            this.formatConfiguration_affectSelectedRows.UseVisualStyleBackColor = true;
            // 
            // formatConfiguration_uploadMode
            // 
            this.formatConfiguration_uploadMode.Dock = System.Windows.Forms.DockStyle.Top;
            this.formatConfiguration_uploadMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.formatConfiguration_uploadMode.FormattingEnabled = true;
            this.formatConfiguration_uploadMode.Items.AddRange(new object[] {
            "INSERT",
            "DELETE",
            "UPDATE"});
            this.formatConfiguration_uploadMode.Location = new System.Drawing.Point(30, 23);
            this.formatConfiguration_uploadMode.Name = "formatConfiguration_uploadMode";
            this.formatConfiguration_uploadMode.Size = new System.Drawing.Size(269, 21);
            this.formatConfiguration_uploadMode.TabIndex = 2;
            // 
            // formatConfiguration_escapeValues
            // 
            this.formatConfiguration_escapeValues.AutoSize = true;
            this.formatConfiguration_escapeValues.Checked = true;
            this.formatConfiguration_escapeValues.CheckState = System.Windows.Forms.CheckState.Checked;
            this.formatConfiguration_escapeValues.Dock = System.Windows.Forms.DockStyle.Top;
            this.formatConfiguration_escapeValues.Location = new System.Drawing.Point(30, 55);
            this.formatConfiguration_escapeValues.Name = "formatConfiguration_escapeValues";
            this.formatConfiguration_escapeValues.Size = new System.Drawing.Size(269, 17);
            this.formatConfiguration_escapeValues.TabIndex = 3;
            this.formatConfiguration_escapeValues.Text = "escape values";
            this.formatConfiguration_escapeValues.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.formatConfiguration_valueFormat);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.formatConfiguration_columnFormat);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.formatConfiguration_escapeValues);
            this.groupBox2.Controls.Add(this.formatConfiguration_useRecordIdAsRowIndex);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(20, 166);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(30, 20, 50, 20);
            this.groupBox2.Size = new System.Drawing.Size(349, 162);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Data Formatting";
            // 
            // formatConfiguration_valueFormat
            // 
            this.formatConfiguration_valueFormat.Dock = System.Windows.Forms.DockStyle.Top;
            this.formatConfiguration_valueFormat.Location = new System.Drawing.Point(30, 138);
            this.formatConfiguration_valueFormat.Name = "formatConfiguration_valueFormat";
            this.formatConfiguration_valueFormat.Size = new System.Drawing.Size(269, 20);
            this.formatConfiguration_valueFormat.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Top;
            this.label6.Location = new System.Drawing.Point(30, 120);
            this.label6.Name = "label6";
            this.label6.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.label6.Size = new System.Drawing.Size(65, 18);
            this.label6.TabIndex = 6;
            this.label6.Text = "value format";
            // 
            // formatConfiguration_columnFormat
            // 
            this.formatConfiguration_columnFormat.Dock = System.Windows.Forms.DockStyle.Top;
            this.formatConfiguration_columnFormat.Location = new System.Drawing.Point(30, 100);
            this.formatConfiguration_columnFormat.Name = "formatConfiguration_columnFormat";
            this.formatConfiguration_columnFormat.Size = new System.Drawing.Size(269, 20);
            this.formatConfiguration_columnFormat.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Top;
            this.label5.Location = new System.Drawing.Point(30, 72);
            this.label5.Name = "label5";
            this.label5.Padding = new System.Windows.Forms.Padding(0, 15, 0, 0);
            this.label5.Size = new System.Drawing.Size(73, 28);
            this.label5.TabIndex = 6;
            this.label5.Text = "column format";
            // 
            // formatConfiguration_useRecordIdAsRowIndex
            // 
            this.formatConfiguration_useRecordIdAsRowIndex.AutoSize = true;
            this.formatConfiguration_useRecordIdAsRowIndex.Checked = true;
            this.formatConfiguration_useRecordIdAsRowIndex.CheckState = System.Windows.Forms.CheckState.Checked;
            this.formatConfiguration_useRecordIdAsRowIndex.Dock = System.Windows.Forms.DockStyle.Top;
            this.formatConfiguration_useRecordIdAsRowIndex.Location = new System.Drawing.Point(30, 33);
            this.formatConfiguration_useRecordIdAsRowIndex.Name = "formatConfiguration_useRecordIdAsRowIndex";
            this.formatConfiguration_useRecordIdAsRowIndex.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.formatConfiguration_useRecordIdAsRowIndex.Size = new System.Drawing.Size(269, 22);
            this.formatConfiguration_useRecordIdAsRowIndex.TabIndex = 7;
            this.formatConfiguration_useRecordIdAsRowIndex.Text = "use recored id as row index";
            this.formatConfiguration_useRecordIdAsRowIndex.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage10);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(397, 374);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.connectionConfiguration_password);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.connectionConfiguration_login);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.connectionConfiguration_database);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.connectionConfiguration_server);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(0);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(20);
            this.tabPage1.Size = new System.Drawing.Size(389, 348);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Connect";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // connectionConfiguration_password
            // 
            this.connectionConfiguration_password.Dock = System.Windows.Forms.DockStyle.Top;
            this.connectionConfiguration_password.Location = new System.Drawing.Point(20, 152);
            this.connectionConfiguration_password.Margin = new System.Windows.Forms.Padding(0);
            this.connectionConfiguration_password.Name = "connectionConfiguration_password";
            this.connectionConfiguration_password.Size = new System.Drawing.Size(349, 20);
            this.connectionConfiguration_password.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Top;
            this.label4.Location = new System.Drawing.Point(20, 134);
            this.label4.Margin = new System.Windows.Forms.Padding(0);
            this.label4.Name = "label4";
            this.label4.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.label4.Size = new System.Drawing.Size(52, 18);
            this.label4.TabIndex = 6;
            this.label4.Text = "password";
            // 
            // connectionConfiguration_login
            // 
            this.connectionConfiguration_login.Dock = System.Windows.Forms.DockStyle.Top;
            this.connectionConfiguration_login.Location = new System.Drawing.Point(20, 114);
            this.connectionConfiguration_login.Margin = new System.Windows.Forms.Padding(0);
            this.connectionConfiguration_login.Name = "connectionConfiguration_login";
            this.connectionConfiguration_login.Size = new System.Drawing.Size(349, 20);
            this.connectionConfiguration_login.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Location = new System.Drawing.Point(20, 96);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.label3.Size = new System.Drawing.Size(52, 18);
            this.label3.TabIndex = 4;
            this.label3.Text = "user login";
            // 
            // connectionConfiguration_database
            // 
            this.connectionConfiguration_database.Dock = System.Windows.Forms.DockStyle.Top;
            this.connectionConfiguration_database.Location = new System.Drawing.Point(20, 76);
            this.connectionConfiguration_database.Margin = new System.Windows.Forms.Padding(0);
            this.connectionConfiguration_database.Name = "connectionConfiguration_database";
            this.connectionConfiguration_database.Size = new System.Drawing.Size(349, 20);
            this.connectionConfiguration_database.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(20, 58);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.label2.Size = new System.Drawing.Size(51, 18);
            this.label2.TabIndex = 2;
            this.label2.Text = "database";
            // 
            // connectionConfiguration_server
            // 
            this.connectionConfiguration_server.Dock = System.Windows.Forms.DockStyle.Top;
            this.connectionConfiguration_server.Location = new System.Drawing.Point(20, 38);
            this.connectionConfiguration_server.Margin = new System.Windows.Forms.Padding(0);
            this.connectionConfiguration_server.Name = "connectionConfiguration_server";
            this.connectionConfiguration_server.Size = new System.Drawing.Size(349, 20);
            this.connectionConfiguration_server.TabIndex = 1;
            this.connectionConfiguration_server.Tag = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(20, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.label1.Size = new System.Drawing.Size(36, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "server";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.panel1);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(20);
            this.tabPage2.Size = new System.Drawing.Size(389, 348);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Format";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(20, 155);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(349, 11);
            this.panel1.TabIndex = 5;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.tabControl2);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(389, 348);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Mapping";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Controls.Add(this.tabPage5);
            this.tabControl2.Controls.Add(this.tabPage7);
            this.tabControl2.Controls.Add(this.tabPage6);
            this.tabControl2.Controls.Add(this.tabPage8);
            this.tabControl2.Controls.Add(this.tabPage9);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(3, 3);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(383, 342);
            this.tabControl2.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.mappingConfiguration_TableName);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(375, 316);
            this.tabPage4.TabIndex = 0;
            this.tabPage4.Text = "Table";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.mappingConfiguration_TableField);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(375, 316);
            this.tabPage5.TabIndex = 1;
            this.tabPage5.Text = "Fields";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.mappingConfiguration_TableValue);
            this.tabPage7.Location = new System.Drawing.Point(4, 22);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage7.Size = new System.Drawing.Size(375, 316);
            this.tabPage7.TabIndex = 3;
            this.tabPage7.Text = "Values";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // mappingConfiguration_TableValue
            // 
            this.mappingConfiguration_TableValue.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mappingConfiguration_TableValue.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Field,
            this.ColumnSourceValue});
            this.mappingConfiguration_TableValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mappingConfiguration_TableValue.Location = new System.Drawing.Point(3, 3);
            this.mappingConfiguration_TableValue.Name = "mappingConfiguration_TableValue";
            this.mappingConfiguration_TableValue.Size = new System.Drawing.Size(369, 310);
            this.mappingConfiguration_TableValue.TabIndex = 0;
            // 
            // Field
            // 
            this.Field.HeaderText = "Field";
            this.Field.Name = "Field";
            // 
            // ColumnSourceValue
            // 
            this.ColumnSourceValue.HeaderText = "Source";
            this.ColumnSourceValue.Name = "ColumnSourceValue";
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.mappingConfiguration_customDataFields);
            this.tabPage6.Controls.Add(this.label8);
            this.tabPage6.Controls.Add(this.mappingConfiguration_defaultIndexFiledName);
            this.tabPage6.Controls.Add(this.label7);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(20);
            this.tabPage6.Size = new System.Drawing.Size(375, 316);
            this.tabPage6.TabIndex = 2;
            this.tabPage6.Text = "Additional";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // mappingConfiguration_defaultIndexFiledName
            // 
            this.mappingConfiguration_defaultIndexFiledName.Dock = System.Windows.Forms.DockStyle.Top;
            this.mappingConfiguration_defaultIndexFiledName.Location = new System.Drawing.Point(20, 38);
            this.mappingConfiguration_defaultIndexFiledName.Name = "mappingConfiguration_defaultIndexFiledName";
            this.mappingConfiguration_defaultIndexFiledName.Size = new System.Drawing.Size(335, 20);
            this.mappingConfiguration_defaultIndexFiledName.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Top;
            this.label7.Location = new System.Drawing.Point(20, 20);
            this.label7.Name = "label7";
            this.label7.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.label7.Size = new System.Drawing.Size(120, 18);
            this.label7.TabIndex = 0;
            this.label7.Text = "Default index field name";
            // 
            // tabPage8
            // 
            this.tabPage8.Controls.Add(this.mappingConfiguration_TableDefaultValue);
            this.tabPage8.Location = new System.Drawing.Point(4, 22);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage8.Size = new System.Drawing.Size(375, 316);
            this.tabPage8.TabIndex = 4;
            this.tabPage8.Text = "Default Values";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // mappingConfiguration_TableDefaultValue
            // 
            this.mappingConfiguration_TableDefaultValue.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mappingConfiguration_TableDefaultValue.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DefFieldName,
            this.DefFieldVal});
            this.mappingConfiguration_TableDefaultValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mappingConfiguration_TableDefaultValue.Location = new System.Drawing.Point(3, 3);
            this.mappingConfiguration_TableDefaultValue.Name = "mappingConfiguration_TableDefaultValue";
            this.mappingConfiguration_TableDefaultValue.Size = new System.Drawing.Size(369, 310);
            this.mappingConfiguration_TableDefaultValue.TabIndex = 0;
            // 
            // DefFieldName
            // 
            this.DefFieldName.HeaderText = "Field";
            this.DefFieldName.Name = "DefFieldName";
            // 
            // DefFieldVal
            // 
            this.DefFieldVal.HeaderText = "Default Value";
            this.DefFieldVal.Name = "DefFieldVal";
            // 
            // tabPage9
            // 
            this.tabPage9.Controls.Add(this.mappingConfiguration_Indexes);
            this.tabPage9.Controls.Add(this.mappingConfiguration_useCustomIndexes);
            this.tabPage9.Location = new System.Drawing.Point(4, 22);
            this.tabPage9.Name = "tabPage9";
            this.tabPage9.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage9.Size = new System.Drawing.Size(375, 316);
            this.tabPage9.TabIndex = 5;
            this.tabPage9.Text = "Indexes";
            this.tabPage9.UseVisualStyleBackColor = true;
            // 
            // mappingConfiguration_Indexes
            // 
            this.mappingConfiguration_Indexes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mappingConfiguration_Indexes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnTableOwner,
            this.ColumnIdxName});
            this.mappingConfiguration_Indexes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mappingConfiguration_Indexes.Location = new System.Drawing.Point(3, 20);
            this.mappingConfiguration_Indexes.Name = "mappingConfiguration_Indexes";
            this.mappingConfiguration_Indexes.Size = new System.Drawing.Size(369, 293);
            this.mappingConfiguration_Indexes.TabIndex = 0;
            // 
            // ColumnTableOwner
            // 
            this.ColumnTableOwner.HeaderText = "Table";
            this.ColumnTableOwner.Name = "ColumnTableOwner";
            // 
            // ColumnIdxName
            // 
            this.ColumnIdxName.HeaderText = "Index";
            this.ColumnIdxName.Name = "ColumnIdxName";
            // 
            // mappingConfiguration_useCustomIndexes
            // 
            this.mappingConfiguration_useCustomIndexes.AutoSize = true;
            this.mappingConfiguration_useCustomIndexes.Dock = System.Windows.Forms.DockStyle.Top;
            this.mappingConfiguration_useCustomIndexes.Location = new System.Drawing.Point(3, 3);
            this.mappingConfiguration_useCustomIndexes.Name = "mappingConfiguration_useCustomIndexes";
            this.mappingConfiguration_useCustomIndexes.Size = new System.Drawing.Size(369, 17);
            this.mappingConfiguration_useCustomIndexes.TabIndex = 1;
            this.mappingConfiguration_useCustomIndexes.Text = "use custom indexes";
            this.mappingConfiguration_useCustomIndexes.UseVisualStyleBackColor = true;
            // 
            // tabPage10
            // 
            this.tabPage10.Controls.Add(this.commandsConfiguration_sqlCommandList);
            this.tabPage10.Location = new System.Drawing.Point(4, 22);
            this.tabPage10.Name = "tabPage10";
            this.tabPage10.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage10.Size = new System.Drawing.Size(389, 348);
            this.tabPage10.TabIndex = 3;
            this.tabPage10.Text = "Commands";
            this.tabPage10.UseVisualStyleBackColor = true;
            // 
            // commandsConfiguration_sqlCommandList
            // 
            this.commandsConfiguration_sqlCommandList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.commandsConfiguration_sqlCommandList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Command,
            this.ComdScript});
            this.commandsConfiguration_sqlCommandList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.commandsConfiguration_sqlCommandList.Location = new System.Drawing.Point(3, 3);
            this.commandsConfiguration_sqlCommandList.Name = "commandsConfiguration_sqlCommandList";
            this.commandsConfiguration_sqlCommandList.Size = new System.Drawing.Size(383, 342);
            this.commandsConfiguration_sqlCommandList.TabIndex = 0;
            // 
            // Command
            // 
            this.Command.HeaderText = "Command";
            this.Command.Name = "Command";
            // 
            // ComdScript
            // 
            this.ComdScript.HeaderText = "Script";
            this.ComdScript.Name = "ComdScript";
            // 
            // mappingConfiguration_customDataFields
            // 
            this.mappingConfiguration_customDataFields.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mappingConfiguration_customDataFields.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.AdditionalColumnOwner,
            this.AdditionalFieldColumnName,
            this.AdditionalFieldColumnValue});
            this.mappingConfiguration_customDataFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mappingConfiguration_customDataFields.Location = new System.Drawing.Point(20, 81);
            this.mappingConfiguration_customDataFields.Name = "mappingConfiguration_customDataFields";
            this.mappingConfiguration_customDataFields.Size = new System.Drawing.Size(335, 215);
            this.mappingConfiguration_customDataFields.TabIndex = 2;
            this.mappingConfiguration_customDataFields.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Dock = System.Windows.Forms.DockStyle.Top;
            this.label8.Location = new System.Drawing.Point(20, 58);
            this.label8.Name = "label8";
            this.label8.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.label8.Size = new System.Drawing.Size(93, 23);
            this.label8.TabIndex = 3;
            this.label8.Text = "Custom data fields";
            // 
            // AdditionalColumnOwner
            // 
            this.AdditionalColumnOwner.HeaderText = "Table Owner";
            this.AdditionalColumnOwner.Name = "AdditionalColumnOwner";
            // 
            // AdditionalFieldColumnName
            // 
            this.AdditionalFieldColumnName.HeaderText = "Column Name";
            this.AdditionalFieldColumnName.Name = "AdditionalFieldColumnName";
            // 
            // AdditionalFieldColumnValue
            // 
            this.AdditionalFieldColumnValue.HeaderText = "Value";
            this.AdditionalFieldColumnValue.Name = "AdditionalFieldColumnValue";
            // 
            // settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 431);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button_save);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimizeBox = false;
            this.Name = "settings";
            this.Text = "settings";
            ((System.ComponentModel.ISupportInitialize)(this.mappingConfiguration_TableField)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mappingConfiguration_TableName)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mappingConfiguration_TableValue)).EndInit();
            this.tabPage6.ResumeLayout(false);
            this.tabPage6.PerformLayout();
            this.tabPage8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mappingConfiguration_TableDefaultValue)).EndInit();
            this.tabPage9.ResumeLayout(false);
            this.tabPage9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mappingConfiguration_Indexes)).EndInit();
            this.tabPage10.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.commandsConfiguration_sqlCommandList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mappingConfiguration_customDataFields)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion


        private void button_save_Click(object sender, EventArgs e)
        {
            SaveSettingsOnClose = true;
            Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

    }
}
