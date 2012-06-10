namespace PayDesk.Components.UI.wndSettings
{
    partial class uc_profileEntry
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uc_profileEntry));
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.profiles_btn_remove = new System.Windows.Forms.Button();
            this.profiles_btn_clone = new System.Windows.Forms.Button();
            this.profiles_btn_use = new System.Windows.Forms.Button();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(228, 100);
            this.panel3.TabIndex = 1008;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.label29);
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Controls.Add(this.textBox4);
            this.panel2.Controls.Add(this.profiles_btn_remove);
            this.panel2.Controls.Add(this.profiles_btn_use);
            this.panel2.Controls.Add(this.profiles_btn_clone);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(228, 81);
            this.panel2.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(162, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 1007;
            this.label1.Text = "Код";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(3, 7);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(91, 13);
            this.label29.TabIndex = 1007;
            this.label29.Text = "Назва профайлу";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(165, 23);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(47, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "1";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(6, 23);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(153, 20);
            this.textBox4.TabIndex = 1;
            this.textBox4.Text = "< Нова Назва Профілю >";
            // 
            // profiles_btn_remove
            // 
            this.profiles_btn_remove.Image = ((System.Drawing.Image)(resources.GetObject("profiles_btn_remove.Image")));
            this.profiles_btn_remove.Location = new System.Drawing.Point(2, 53);
            this.profiles_btn_remove.Name = "profiles_btn_remove";
            this.profiles_btn_remove.Size = new System.Drawing.Size(30, 23);
            this.profiles_btn_remove.TabIndex = 0;
            this.profiles_btn_remove.Tag = "1";
            this.profiles_btn_remove.UseVisualStyleBackColor = true;
            this.profiles_btn_remove.Click += new System.EventHandler(this.button_Click);
            // 
            // profiles_btn_clone
            // 
            this.profiles_btn_clone.Image = ((System.Drawing.Image)(resources.GetObject("profiles_btn_clone.Image")));
            this.profiles_btn_clone.Location = new System.Drawing.Point(38, 53);
            this.profiles_btn_clone.Name = "profiles_btn_clone";
            this.profiles_btn_clone.Size = new System.Drawing.Size(30, 23);
            this.profiles_btn_clone.TabIndex = 0;
            this.profiles_btn_clone.Tag = "2";
            this.profiles_btn_clone.UseVisualStyleBackColor = true;
            this.profiles_btn_clone.Click += new System.EventHandler(this.button_Click);
            // 
            // profiles_btn_use
            // 
            this.profiles_btn_use.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.profiles_btn_use.Location = new System.Drawing.Point(102, 53);
            this.profiles_btn_use.Name = "profiles_btn_use";
            this.profiles_btn_use.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.profiles_btn_use.Size = new System.Drawing.Size(110, 23);
            this.profiles_btn_use.TabIndex = 0;
            this.profiles_btn_use.Tag = "2";
            this.profiles_btn_use.Text = "Активувати";
            this.profiles_btn_use.UseCompatibleTextRendering = true;
            this.profiles_btn_use.UseVisualStyleBackColor = true;
            this.profiles_btn_use.Click += new System.EventHandler(this.button_Click);
            // 
            // uc_profileEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.panel3);
            this.Name = "uc_profileEntry";
            this.Size = new System.Drawing.Size(228, 100);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Button profiles_btn_remove;
        private System.Windows.Forms.Button profiles_btn_clone;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button profiles_btn_use;
    }
}
