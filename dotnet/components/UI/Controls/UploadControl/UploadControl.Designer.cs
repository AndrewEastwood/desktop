namespace components.UI.Controls.UploadControl
{
    partial class UploadControl
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.button_browse = new System.Windows.Forms.Button();
            this.button_open = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox1.Location = new System.Drawing.Point(5, 5);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(43, 13);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // button_browse
            // 
            this.button_browse.AutoSize = true;
            this.button_browse.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button_browse.Dock = System.Windows.Forms.DockStyle.Right;
            this.button_browse.Image = global::components.Properties.Resources._050;
            this.button_browse.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_browse.Location = new System.Drawing.Point(67, 10);
            this.button_browse.Name = "button_browse";
            this.button_browse.Size = new System.Drawing.Size(67, 28);
            this.button_browse.TabIndex = 1;
            this.button_browse.Text = "     Browse";
            this.button_browse.UseVisualStyleBackColor = true;
            this.button_browse.Click += new System.EventHandler(this.button_browse_Click);
            // 
            // button_open
            // 
            this.button_open.AutoSize = true;
            this.button_open.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button_open.Dock = System.Windows.Forms.DockStyle.Right;
            this.button_open.Image = global::components.Properties.Resources._009;
            this.button_open.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_open.Location = new System.Drawing.Point(134, 10);
            this.button_open.Name = "button_open";
            this.button_open.Size = new System.Drawing.Size(56, 28);
            this.button_open.TabIndex = 2;
            this.button_open.Text = "     Load";
            this.button_open.UseVisualStyleBackColor = true;
            this.button_open.Click += new System.EventHandler(this.button_open_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(10, 10);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5, 5, 5, 0);
            this.panel1.Size = new System.Drawing.Size(57, 28);
            this.panel1.TabIndex = 3;
            // 
            // UploadControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button_browse);
            this.Controls.Add(this.button_open);
            this.MaximumSize = new System.Drawing.Size(1000, 48);
            this.MinimumSize = new System.Drawing.Size(200, 48);
            this.Name = "UploadControl";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(200, 48);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button_browse;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button button_open;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}
