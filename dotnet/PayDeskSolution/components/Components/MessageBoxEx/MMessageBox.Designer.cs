namespace components.Components.MMessageBox
{
    partial class MMessageBox
    {

        private static System.Windows.Forms.Panel panel1;
        private static System.Windows.Forms.PictureBox pictureBox1;
        private static System.Windows.Forms.Label label1;

        private static void InitializeComponent()
        {
            panel1 = new System.Windows.Forms.Panel();
            label1 = new System.Windows.Forms.Label();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(pictureBox1)).BeginInit();
            form1.SuspendLayout();
            // 
            // panel1
            // 
            panel1.AutoSize = true;
            panel1.Controls.Add(label1);
            panel1.Controls.Add(pictureBox1);
            panel1.Dock = System.Windows.Forms.DockStyle.Top;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Margin = new System.Windows.Forms.Padding(0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(99, 47);
            panel1.TabIndex = 0;
            // 
            // label1
            // 
            label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            label1.AutoSize = true;
            label1.BackColor = System.Drawing.Color.Transparent;
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            label1.Location = new System.Drawing.Point(50, 18);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(45, 16);
            label1.TabIndex = 17;
            label1.Text = "label1";
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = System.Drawing.Color.Transparent;
            pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            pictureBox1.Location = new System.Drawing.Point(12, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(32, 32);
            pictureBox1.TabIndex = 18;
            pictureBox1.TabStop = false;
            // 
            // MMessageBox
            // 
            form1.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            form1.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            form1.AutoSize = true;
            form1.ClientSize = new System.Drawing.Size(99, 93);
            form1.Controls.Add(panel1);
            form1.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            form1.KeyPreview = true;
            form1.MaximizeBox = false;
            form1.MinimizeBox = false;
            form1.MinimumSize = new System.Drawing.Size(105, 125);
            form1.Name = "MMessageBox";
            form1.Icon = Properties.Resources.market;// Properties.Resources.market;
            form1.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            form1.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            form1.KeyDown += new System.Windows.Forms.KeyEventHandler(MMessageBox_KeyDown);
            form1.Load += new System.EventHandler(MMessageBox_Load);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(pictureBox1)).EndInit();
            form1.ResumeLayout(false);
            form1.PerformLayout();
        }

    }
}