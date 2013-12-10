namespace PayDesk.Components.UI.wndBills
{
    partial class uiWndBillSave
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uiWndBillSave));
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button_save = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button_clear = new System.Windows.Forms.Button();
            this.flowLayoutPanel_top = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel_manual = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel_manual.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(20, 23);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(20, 0, 20, 0);
            this.richTextBox1.MaxLength = 200;
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(258, 66);
            this.richTextBox1.TabIndex = 50;
            this.richTextBox1.Text = "";
            // 
            // button_save
            // 
            this.button_save.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_save.Location = new System.Drawing.Point(187, 99);
            this.button_save.Margin = new System.Windows.Forms.Padding(20, 10, 20, 20);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(92, 36);
            this.button_save.TabIndex = 40;
            this.button_save.Text = "Добре";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 5);
            this.label1.Margin = new System.Windows.Forms.Padding(20, 5, 20, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 13);
            this.label1.TabIndex = 501;
            this.label1.Text = "Додатковий коментар";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // button_clear
            // 
            this.button_clear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_clear.Location = new System.Drawing.Point(20, 99);
            this.button_clear.Margin = new System.Windows.Forms.Padding(20, 10, 20, 20);
            this.button_clear.Name = "button_clear";
            this.button_clear.Size = new System.Drawing.Size(127, 36);
            this.button_clear.TabIndex = 60;
            this.button_clear.Text = "Очистити";
            this.button_clear.UseVisualStyleBackColor = true;
            this.button_clear.Click += new System.EventHandler(this.button2_Click);
            // 
            // flowLayoutPanel_top
            // 
            this.flowLayoutPanel_top.AutoSize = true;
            this.flowLayoutPanel_top.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel_top.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel_top.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel_top.Name = "flowLayoutPanel_top";
            this.flowLayoutPanel_top.Size = new System.Drawing.Size(300, 1);
            this.flowLayoutPanel_top.TabIndex = 10;
            // 
            // flowLayoutPanel_manual
            // 
            this.flowLayoutPanel_manual.AutoSize = true;
            this.flowLayoutPanel_manual.Controls.Add(this.label1);
            this.flowLayoutPanel_manual.Controls.Add(this.richTextBox1);
            this.flowLayoutPanel_manual.Controls.Add(this.button_clear);
            this.flowLayoutPanel_manual.Controls.Add(this.button_save);
            this.flowLayoutPanel_manual.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel_manual.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel_manual.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel_manual.Name = "flowLayoutPanel_manual";
            this.flowLayoutPanel_manual.Size = new System.Drawing.Size(300, 155);
            this.flowLayoutPanel_manual.TabIndex = 11;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 300F));
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel_manual, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel_top, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(300, 0);
            this.tableLayoutPanel2.TabIndex = 12;
            // 
            // uiWndBillSave
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(300, 0);
            this.Controls.Add(this.tableLayoutPanel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "uiWndBillSave";
            this.Opacity = 0D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Збереження рахунку";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.uiWndBillSave_FormClosing);
            this.Load += new System.EventHandler(this.uiWndBillSave_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BillRequets_KeyDown);
            this.flowLayoutPanel_manual.ResumeLayout(false);
            this.flowLayoutPanel_manual.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_clear;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel_top;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel_manual;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    }
}