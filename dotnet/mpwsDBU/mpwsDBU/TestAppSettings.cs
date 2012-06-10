using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mpwsDBU
{
    public class TestAppSettings : components.UI.Windows.AppSettingsWindow.AppSettingsWindow
    {
        private System.Windows.Forms.TextBox test_textBox1;
        private System.Windows.Forms.NumericUpDown test_numericUpDown1;
        private System.Windows.Forms.DataGridView test_eee_dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn A;
        private System.Windows.Forms.DataGridViewTextBoxColumn B;
        private System.Windows.Forms.CheckBox test_checkBox1;
        private System.Windows.Forms.RichTextBox test_richTextBox1;
        private System.Windows.Forms.TrackBar test_trackBar1;
        private System.Windows.Forms.CheckedListBox test_dd_checkedListBox1;
        private System.Windows.Forms.DomainUpDown test_dd_domainUpDown1;
        private System.Windows.Forms.ListBox test_dd_listBox1;
        private System.Windows.Forms.DateTimePicker test_dd_dateTimePicker1;
        private System.Windows.Forms.ComboBox test_eee_comboBox1;
        private System.Windows.Forms.Button button1;

        public TestAppSettings()
            : base()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.test_textBox1 = new System.Windows.Forms.TextBox();
            this.test_numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.test_eee_dataGridView1 = new System.Windows.Forms.DataGridView();
            this.A = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.B = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.test_checkBox1 = new System.Windows.Forms.CheckBox();
            this.test_richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.test_trackBar1 = new System.Windows.Forms.TrackBar();
            this.test_dd_checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.test_dd_domainUpDown1 = new System.Windows.Forms.DomainUpDown();
            this.test_dd_listBox1 = new System.Windows.Forms.ListBox();
            this.test_dd_dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.test_eee_comboBox1 = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.test_numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.test_eee_dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.test_trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(192, 298);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Apply";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // test_textBox1
            // 
            this.test_textBox1.Location = new System.Drawing.Point(12, 301);
            this.test_textBox1.Name = "test_textBox1";
            this.test_textBox1.Size = new System.Drawing.Size(100, 20);
            this.test_textBox1.TabIndex = 1;
            // 
            // test_numericUpDown1
            // 
            this.test_numericUpDown1.AccessibleDescription = "It sets limit for rows which are used";
            this.test_numericUpDown1.AccessibleName = "test_numericUpDown1";
            this.test_numericUpDown1.Location = new System.Drawing.Point(12, 275);
            this.test_numericUpDown1.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.test_numericUpDown1.Name = "test_numericUpDown1";
            this.test_numericUpDown1.Size = new System.Drawing.Size(120, 20);
            this.test_numericUpDown1.TabIndex = 2;
            // 
            // test_eee_dataGridView1
            // 
            this.test_eee_dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.test_eee_dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.A,
            this.B});
            this.test_eee_dataGridView1.Location = new System.Drawing.Point(12, 26);
            this.test_eee_dataGridView1.Name = "test_eee_dataGridView1";
            this.test_eee_dataGridView1.Size = new System.Drawing.Size(255, 83);
            this.test_eee_dataGridView1.TabIndex = 3;
            // 
            // A
            // 
            this.A.HeaderText = "A";
            this.A.Name = "A";
            // 
            // B
            // 
            this.B.HeaderText = "B";
            this.B.Name = "B";
            // 
            // test_checkBox1
            // 
            this.test_checkBox1.AccessibleDescription = "demo check box";
            this.test_checkBox1.AutoSize = true;
            this.test_checkBox1.Location = new System.Drawing.Point(12, 252);
            this.test_checkBox1.Name = "test_checkBox1";
            this.test_checkBox1.Size = new System.Drawing.Size(80, 17);
            this.test_checkBox1.TabIndex = 4;
            this.test_checkBox1.Text = "checkBox1";
            this.test_checkBox1.UseVisualStyleBackColor = true;
            // 
            // test_richTextBox1
            // 
            this.test_richTextBox1.AccessibleDescription = "rich text area demo test";
            this.test_richTextBox1.Location = new System.Drawing.Point(192, 238);
            this.test_richTextBox1.Name = "test_richTextBox1";
            this.test_richTextBox1.Size = new System.Drawing.Size(75, 54);
            this.test_richTextBox1.TabIndex = 5;
            this.test_richTextBox1.Text = "";
            // 
            // test_trackBar1
            // 
            this.test_trackBar1.AccessibleDescription = "racking bar demo test";
            this.test_trackBar1.AutoSize = false;
            this.test_trackBar1.Location = new System.Drawing.Point(12, 213);
            this.test_trackBar1.Name = "test_trackBar1";
            this.test_trackBar1.Size = new System.Drawing.Size(159, 33);
            this.test_trackBar1.TabIndex = 6;
            // 
            // test_dd_checkedListBox1
            // 
            this.test_dd_checkedListBox1.FormattingEnabled = true;
            this.test_dd_checkedListBox1.Items.AddRange(new object[] {
            "Q",
            "W",
            "E",
            "R",
            "T",
            "Y"});
            this.test_dd_checkedListBox1.Location = new System.Drawing.Point(12, 115);
            this.test_dd_checkedListBox1.Name = "test_dd_checkedListBox1";
            this.test_dd_checkedListBox1.Size = new System.Drawing.Size(120, 94);
            this.test_dd_checkedListBox1.TabIndex = 7;
            // 
            // test_dd_domainUpDown1
            // 
            this.test_dd_domainUpDown1.Items.Add("1111");
            this.test_dd_domainUpDown1.Items.Add("2222");
            this.test_dd_domainUpDown1.Items.Add("3333");
            this.test_dd_domainUpDown1.Items.Add("4444");
            this.test_dd_domainUpDown1.Items.Add("aaaa");
            this.test_dd_domainUpDown1.Items.Add("bbb");
            this.test_dd_domainUpDown1.Items.Add("ccc");
            this.test_dd_domainUpDown1.Location = new System.Drawing.Point(138, 115);
            this.test_dd_domainUpDown1.Name = "test_dd_domainUpDown1";
            this.test_dd_domainUpDown1.Size = new System.Drawing.Size(120, 20);
            this.test_dd_domainUpDown1.TabIndex = 8;
            this.test_dd_domainUpDown1.Text = "domainUpDown1";
            // 
            // test_dd_listBox1
            // 
            this.test_dd_listBox1.FormattingEnabled = true;
            this.test_dd_listBox1.Items.AddRange(new object[] {
            "amber",
            "cold",
            "silver",
            "emerald"});
            this.test_dd_listBox1.Location = new System.Drawing.Point(138, 141);
            this.test_dd_listBox1.Name = "test_dd_listBox1";
            this.test_dd_listBox1.Size = new System.Drawing.Size(120, 43);
            this.test_dd_listBox1.TabIndex = 9;
            // 
            // test_dd_dateTimePicker1
            // 
            this.test_dd_dateTimePicker1.Location = new System.Drawing.Point(138, 190);
            this.test_dd_dateTimePicker1.Name = "test_dd_dateTimePicker1";
            this.test_dd_dateTimePicker1.Size = new System.Drawing.Size(120, 20);
            this.test_dd_dateTimePicker1.TabIndex = 10;
            // 
            // test_eee_comboBox1
            // 
            this.test_eee_comboBox1.FormattingEnabled = true;
            this.test_eee_comboBox1.Items.AddRange(new object[] {
            "A",
            "B",
            "C",
            "D"});
            this.test_eee_comboBox1.Location = new System.Drawing.Point(118, 301);
            this.test_eee_comboBox1.Name = "test_eee_comboBox1";
            this.test_eee_comboBox1.Size = new System.Drawing.Size(68, 21);
            this.test_eee_comboBox1.TabIndex = 11;
            // 
            // TestAppSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(279, 333);
            this.Controls.Add(this.test_eee_comboBox1);
            this.Controls.Add(this.test_dd_dateTimePicker1);
            this.Controls.Add(this.test_dd_listBox1);
            this.Controls.Add(this.test_dd_domainUpDown1);
            this.Controls.Add(this.test_dd_checkedListBox1);
            this.Controls.Add(this.test_trackBar1);
            this.Controls.Add(this.test_richTextBox1);
            this.Controls.Add(this.test_checkBox1);
            this.Controls.Add(this.test_eee_dataGridView1);
            this.Controls.Add(this.test_numericUpDown1);
            this.Controls.Add(this.test_textBox1);
            this.Controls.Add(this.button1);
            this.Name = "TestAppSettings";
            ((System.ComponentModel.ISupportInitialize)(this.test_numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.test_eee_dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.test_trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveSettingsOnClose = true;
            Close();
        }
    }
}
