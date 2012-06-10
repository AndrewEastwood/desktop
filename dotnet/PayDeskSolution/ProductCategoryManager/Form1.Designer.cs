namespace ProductCategoryManager
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.treeVisualizer1 = new components.UI.Controls.TreeVisualizer();
            this.SuspendLayout();
            // 
            // treeVisualizer1
            // 
            this.treeVisualizer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeVisualizer1.Location = new System.Drawing.Point(0, 0);
            this.treeVisualizer1.Name = "treeVisualizer1";
            this.treeVisualizer1.Padding = new System.Windows.Forms.Padding(10);
            this.treeVisualizer1.Size = new System.Drawing.Size(622, 628);
            this.treeVisualizer1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 628);
            this.Controls.Add(this.treeVisualizer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Product Category Manager";
            this.ResumeLayout(false);

        }

        #endregion

        private components.UI.Controls.TreeVisualizer treeVisualizer1;
    }
}

