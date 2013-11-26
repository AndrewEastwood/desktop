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
            this.label1 = new System.Windows.Forms.Label();
            this.sync_remotePath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.sync_profileDisplayText = new System.Windows.Forms.TextBox();
            this.sync_srcSchema = new System.Windows.Forms.NumericUpDown();
            this.sync_srcSubunit = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.sync_srcSchema)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sync_srcSubunit)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Віддалена папка";
            // 
            // sync_remotePath
            // 
            this.sync_remotePath.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sync_remotePath.Location = new System.Drawing.Point(3, 55);
            this.sync_remotePath.Name = "sync_remotePath";
            this.sync_remotePath.Size = new System.Drawing.Size(252, 20);
            this.sync_remotePath.TabIndex = 1;
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
            this.sync_profileDisplayText.Size = new System.Drawing.Size(252, 20);
            this.sync_profileDisplayText.TabIndex = 14;
            this.sync_profileDisplayText.Text = "Новий профіль";
            this.sync_profileDisplayText.TextChanged += new System.EventHandler(this.sync_profileDisplayText_TextChanged);
            // 
            // sync_srcSchema
            // 
            this.sync_srcSchema.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sync_srcSchema.Location = new System.Drawing.Point(3, 17);
            this.sync_srcSchema.Name = "sync_srcSchema";
            this.sync_srcSchema.Size = new System.Drawing.Size(123, 20);
            this.sync_srcSchema.TabIndex = 15;
            // 
            // sync_srcSubunit
            // 
            this.sync_srcSubunit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sync_srcSubunit.Location = new System.Drawing.Point(132, 17);
            this.sync_srcSubunit.Name = "sync_srcSubunit";
            this.sync_srcSubunit.Size = new System.Drawing.Size(123, 20);
            this.sync_srcSubunit.TabIndex = 16;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(123, 14);
            this.label4.TabIndex = 17;
            this.label4.Text = "Номер схеми";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(132, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(123, 14);
            this.label5.TabIndex = 18;
            this.label5.Text = "Номер підрозділу";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.sync_srcSubunit, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.sync_srcSchema, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label5, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 81);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(258, 48);
            this.tableLayoutPanel1.TabIndex = 19;
            // 
            // DataSyncProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.sync_profileDisplayText);
            this.Controls.Add(this.sync_remotePath);
            this.Controls.Add(this.label3);
            this.Name = "DataSyncProfile";
            this.Size = new System.Drawing.Size(258, 136);
            ((System.ComponentModel.ISupportInitialize)(this.sync_srcSchema)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sync_srcSubunit)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox sync_remotePath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox sync_profileDisplayText;
        private System.Windows.Forms.NumericUpDown sync_srcSchema;
        private System.Windows.Forms.NumericUpDown sync_srcSubunit;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
