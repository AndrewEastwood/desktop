namespace PayDesk.Components.UI.wndAdditional
{
    partial class uiWndAdditionalPortCommands
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uiWndAdditionalPortCommands));
            this.lBox_main_commands = new System.Windows.Forms.ListBox();
            this.btn_main_ok = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btn_main_cancel = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lBox_main_commands
            // 
            this.lBox_main_commands.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lBox_main_commands.FormattingEnabled = true;
            this.lBox_main_commands.Location = new System.Drawing.Point(5, 5);
            this.lBox_main_commands.Name = "lBox_main_commands";
            this.lBox_main_commands.Size = new System.Drawing.Size(274, 225);
            this.lBox_main_commands.TabIndex = 0;
            // 
            // btn_main_ok
            // 
            this.btn_main_ok.Location = new System.Drawing.Point(196, 3);
            this.btn_main_ok.Name = "btn_main_ok";
            this.btn_main_ok.Size = new System.Drawing.Size(75, 23);
            this.btn_main_ok.TabIndex = 1;
            this.btn_main_ok.Text = "Виконати";
            this.btn_main_ok.UseVisualStyleBackColor = true;
            this.btn_main_ok.Click += new System.EventHandler(this.btn_main_ok_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.btn_main_ok);
            this.flowLayoutPanel1.Controls.Add(this.btn_main_cancel);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(5, 230);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(274, 29);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // btn_main_cancel
            // 
            this.btn_main_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_main_cancel.Location = new System.Drawing.Point(115, 3);
            this.btn_main_cancel.Name = "btn_main_cancel";
            this.btn_main_cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_main_cancel.TabIndex = 2;
            this.btn_main_cancel.Text = "Скасувати";
            this.btn_main_cancel.UseVisualStyleBackColor = true;
            // 
            // uiWndAdditionalPortCommands
            // 
            this.AcceptButton = this.btn_main_ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btn_main_cancel;
            this.ClientSize = new System.Drawing.Size(284, 264);
            this.Controls.Add(this.lBox_main_commands);
            this.Controls.Add(this.flowLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "uiWndAdditionalPortCommands";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Додаткові команди";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.uiWndAdditionalPortCommands_FormClosing);
            this.Load += new System.EventHandler(this.uiWndAdditionalPortCommands_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.uiWndAdditionalPortCommands_KeyDown);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lBox_main_commands;
        private System.Windows.Forms.Button btn_main_ok;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btn_main_cancel;
    }
}