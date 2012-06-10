namespace Components.UI
{
    partial class uiWndLoader
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uiWndLoader));
            this.maskedTextBox1 = new System.Windows.Forms.MaskedTextBox();
            this.maskedTextBox2 = new System.Windows.Forms.MaskedTextBox();
            this.button_WndLoader_MakeCode = new System.Windows.Forms.Button();
            this.maskedTextBox3 = new System.Windows.Forms.MaskedTextBox();
            this.SuspendLayout();
            // 
            // maskedTextBox1
            // 
            this.maskedTextBox1.Location = new System.Drawing.Point(12, 12);
            this.maskedTextBox1.Mask = "\\Client: 0000-0000-0000-0000-0000";
            this.maskedTextBox1.Name = "maskedTextBox1";
            this.maskedTextBox1.Size = new System.Drawing.Size(260, 20);
            this.maskedTextBox1.TabIndex = 0;
            // 
            // maskedTextBox2
            // 
            this.maskedTextBox2.Location = new System.Drawing.Point(12, 38);
            this.maskedTextBox2.Mask = "Reg: 00000-00000-00000-00000-00000";
            this.maskedTextBox2.Name = "maskedTextBox2";
            this.maskedTextBox2.Size = new System.Drawing.Size(260, 20);
            this.maskedTextBox2.TabIndex = 1;
            // 
            // button_WndLoader_MakeCode
            // 
            this.button_WndLoader_MakeCode.Location = new System.Drawing.Point(197, 90);
            this.button_WndLoader_MakeCode.Name = "button_WndLoader_MakeCode";
            this.button_WndLoader_MakeCode.Size = new System.Drawing.Size(75, 23);
            this.button_WndLoader_MakeCode.TabIndex = 2;
            this.button_WndLoader_MakeCode.Text = "Generate";
            this.button_WndLoader_MakeCode.UseVisualStyleBackColor = true;
            this.button_WndLoader_MakeCode.Click += new System.EventHandler(this.button_WndLoader_MakeCode_Click);
            // 
            // maskedTextBox3
            // 
            this.maskedTextBox3.Location = new System.Drawing.Point(12, 64);
            this.maskedTextBox3.Name = "maskedTextBox3";
            this.maskedTextBox3.Size = new System.Drawing.Size(260, 20);
            this.maskedTextBox3.TabIndex = 1;
            // 
            // uiWndLoader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 125);
            this.Controls.Add(this.button_WndLoader_MakeCode);
            this.Controls.Add(this.maskedTextBox3);
            this.Controls.Add(this.maskedTextBox2);
            this.Controls.Add(this.maskedTextBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "uiWndLoader";
            this.Text = "PayDesk Registration v.2.7.1D";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MaskedTextBox maskedTextBox1;
        private System.Windows.Forms.MaskedTextBox maskedTextBox2;
        private System.Windows.Forms.Button button_WndLoader_MakeCode;
        private System.Windows.Forms.MaskedTextBox maskedTextBox3;
    }
}

