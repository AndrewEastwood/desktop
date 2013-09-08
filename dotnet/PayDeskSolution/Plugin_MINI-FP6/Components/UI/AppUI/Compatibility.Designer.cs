namespace MINI_FP6.UI.AppUI
{
    partial class Compatibility
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkBox_runAsOP6 = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.checkBox_runAsOP6);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(20, 20);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(218, 224);
            this.panel1.TabIndex = 0;
            // 
            // checkBox_runAsOP6
            // 
            this.checkBox_runAsOP6.AutoSize = true;
            this.checkBox_runAsOP6.Location = new System.Drawing.Point(3, 3);
            this.checkBox_runAsOP6.Name = "checkBox_runAsOP6";
            this.checkBox_runAsOP6.Size = new System.Drawing.Size(156, 17);
            this.checkBox_runAsOP6.TabIndex = 0;
            this.checkBox_runAsOP6.Text = "Використовувати модем";
            this.checkBox_runAsOP6.UseVisualStyleBackColor = true;
            this.checkBox_runAsOP6.CheckedChanged += new System.EventHandler(this.checkBox_runAsOP6_CheckedChanged);
            // 
            // Compatibility
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.panel1);
            this.Name = "Compatibility";
            this.Padding = new System.Windows.Forms.Padding(20);
            this.Size = new System.Drawing.Size(258, 264);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox checkBox_runAsOP6;
    }
}
