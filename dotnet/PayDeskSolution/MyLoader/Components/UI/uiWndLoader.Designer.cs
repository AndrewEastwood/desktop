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
            this.clientRegCode = new System.Windows.Forms.MaskedTextBox();
            this.activationCode = new System.Windows.Forms.MaskedTextBox();
            this.button_WndLoader_MakeCode = new System.Windows.Forms.Button();
            this.comment = new System.Windows.Forms.MaskedTextBox();
            this.appType = new System.Windows.Forms.ComboBox();
            this.customerType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.deskNumber = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.customerName = new System.Windows.Forms.MaskedTextBox();
            this.SuspendLayout();
            // 
            // clientRegCode
            // 
            this.clientRegCode.ForeColor = System.Drawing.Color.Maroon;
            this.clientRegCode.Location = new System.Drawing.Point(12, 57);
            this.clientRegCode.Mask = "\\Client: 0000-0000-0000-0000-0000";
            this.clientRegCode.Name = "clientRegCode";
            this.clientRegCode.Size = new System.Drawing.Size(260, 20);
            this.clientRegCode.TabIndex = 0;
            // 
            // activationCode
            // 
            this.activationCode.ForeColor = System.Drawing.Color.Maroon;
            this.activationCode.Location = new System.Drawing.Point(12, 83);
            this.activationCode.Mask = "Reg: 00000-00000-00000-00000-00000";
            this.activationCode.Name = "activationCode";
            this.activationCode.Size = new System.Drawing.Size(260, 20);
            this.activationCode.TabIndex = 1;
            // 
            // button_WndLoader_MakeCode
            // 
            this.button_WndLoader_MakeCode.Location = new System.Drawing.Point(197, 273);
            this.button_WndLoader_MakeCode.Name = "button_WndLoader_MakeCode";
            this.button_WndLoader_MakeCode.Size = new System.Drawing.Size(75, 23);
            this.button_WndLoader_MakeCode.TabIndex = 2;
            this.button_WndLoader_MakeCode.Text = "Generate";
            this.button_WndLoader_MakeCode.UseVisualStyleBackColor = true;
            this.button_WndLoader_MakeCode.Click += new System.EventHandler(this.button_WndLoader_MakeCode_Click);
            // 
            // comment
            // 
            this.comment.Location = new System.Drawing.Point(12, 247);
            this.comment.Name = "comment";
            this.comment.Size = new System.Drawing.Size(260, 20);
            this.comment.TabIndex = 1;
            // 
            // appType
            // 
            this.appType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.appType.FormattingEnabled = true;
            this.appType.Items.AddRange(new object[] {
            "IKC",
            "Universal - Shop",
            "Universal - Bar",
            "Universal - Bar Sensor",
            "Universal - Bar Sensor With Profiles"});
            this.appType.Location = new System.Drawing.Point(12, 128);
            this.appType.Name = "appType";
            this.appType.Size = new System.Drawing.Size(260, 21);
            this.appType.TabIndex = 3;
            // 
            // customerType
            // 
            this.customerType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.customerType.FormattingEnabled = true;
            this.customerType.Items.AddRange(new object[] {
            "New",
            "Existed (Reactivation)"});
            this.customerType.Location = new System.Drawing.Point(12, 168);
            this.customerType.Name = "customerType";
            this.customerType.Size = new System.Drawing.Size(260, 21);
            this.customerType.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Maroon;
            this.label1.Location = new System.Drawing.Point(9, 112);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Application Type";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Maroon;
            this.label2.Location = new System.Drawing.Point(9, 152);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Customer Type";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Maroon;
            this.label3.Location = new System.Drawing.Point(9, 192);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Desk Numer";
            // 
            // deskNumber
            // 
            this.deskNumber.Location = new System.Drawing.Point(12, 208);
            this.deskNumber.Name = "deskNumber";
            this.deskNumber.Size = new System.Drawing.Size(260, 20);
            this.deskNumber.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 231);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Additional Comment";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Maroon;
            this.label5.Location = new System.Drawing.Point(9, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Customer Name";
            // 
            // customerName
            // 
            this.customerName.Location = new System.Drawing.Point(12, 25);
            this.customerName.Name = "customerName";
            this.customerName.Size = new System.Drawing.Size(260, 20);
            this.customerName.TabIndex = 10;
            // 
            // uiWndLoader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 308);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.customerName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.deskNumber);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.customerType);
            this.Controls.Add(this.appType);
            this.Controls.Add(this.button_WndLoader_MakeCode);
            this.Controls.Add(this.comment);
            this.Controls.Add(this.activationCode);
            this.Controls.Add(this.clientRegCode);
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

        private System.Windows.Forms.MaskedTextBox clientRegCode;
        private System.Windows.Forms.MaskedTextBox activationCode;
        private System.Windows.Forms.Button button_WndLoader_MakeCode;
        private System.Windows.Forms.MaskedTextBox comment;
        private System.Windows.Forms.ComboBox appType;
        private System.Windows.Forms.ComboBox customerType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox deskNumber;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.MaskedTextBox customerName;
    }
}

