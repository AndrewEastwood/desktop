namespace PayDesk.Components.UI
{
    partial class uiWndRegistration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uiWndRegistration));
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.maskedTextBox_uiWndReg_PublicCode = new System.Windows.Forms.MaskedTextBox();
            this.maskedTextBox_uiWndReg_ClientCode = new System.Windows.Forms.MaskedTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.AutoSize = true;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button1.Location = new System.Drawing.Point(270, 156);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(148, 35);
            this.button1.TabIndex = 6;
            this.button1.Text = "Добре";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Серійний код";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(9, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Реєстраційний код";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(241, 9);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(195, 203);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // maskedTextBox_uiWndReg_PublicCode
            // 
            this.maskedTextBox_uiWndReg_PublicCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.maskedTextBox_uiWndReg_PublicCode.Location = new System.Drawing.Point(12, 25);
            this.maskedTextBox_uiWndReg_PublicCode.Mask = "PаyDesk v\\.2\\.\\7\\.1D: 0000-0000-0000-0000-0000 ";
            this.maskedTextBox_uiWndReg_PublicCode.Name = "maskedTextBox_uiWndReg_PublicCode";
            this.maskedTextBox_uiWndReg_PublicCode.ReadOnly = true;
            this.maskedTextBox_uiWndReg_PublicCode.Size = new System.Drawing.Size(406, 29);
            this.maskedTextBox_uiWndReg_PublicCode.TabIndex = 10;
            // 
            // maskedTextBox_uiWndReg_ClientCode
            // 
            this.maskedTextBox_uiWndReg_ClientCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.maskedTextBox_uiWndReg_ClientCode.Location = new System.Drawing.Point(12, 94);
            this.maskedTextBox_uiWndReg_ClientCode.Mask = "00000-00000-00000-00000-00000";
            this.maskedTextBox_uiWndReg_ClientCode.Name = "maskedTextBox_uiWndReg_ClientCode";
            this.maskedTextBox_uiWndReg_ClientCode.Size = new System.Drawing.Size(406, 29);
            this.maskedTextBox_uiWndReg_ClientCode.TabIndex = 11;
            // 
            // uiWndRegistration
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 203);
            this.Controls.Add(this.maskedTextBox_uiWndReg_ClientCode);
            this.Controls.Add(this.maskedTextBox_uiWndReg_PublicCode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "uiWndRegistration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PayDesk - Реєстрація";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MaskedTextBox maskedTextBox_uiWndReg_PublicCode;
        private System.Windows.Forms.MaskedTextBox maskedTextBox_uiWndReg_ClientCode;
    }
}