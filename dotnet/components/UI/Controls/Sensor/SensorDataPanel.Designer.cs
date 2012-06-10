namespace components.UI.Controls.SensorDataPanel
{
    partial class SensorDataPanel
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
            this.mainContainer = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.dataPlaceholder = new System.Windows.Forms.Panel();
            this.button_sensor_productTop = new System.Windows.Forms.Button();
            this.button_sensor_productBottom = new System.Windows.Forms.Button();
            this.categoryNavBar1 = new components.UI.Controls.CategoryNavBar.CategoryNavBar();
            this.mainContainer.Panel1.SuspendLayout();
            this.mainContainer.Panel2.SuspendLayout();
            this.mainContainer.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainContainer
            // 
            this.mainContainer.BackColor = System.Drawing.SystemColors.ControlDark;
            this.mainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainContainer.Location = new System.Drawing.Point(0, 0);
            this.mainContainer.Name = "mainContainer";
            this.mainContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mainContainer.Panel1
            // 
            this.mainContainer.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.mainContainer.Panel1.Controls.Add(this.categoryNavBar1);
            // 
            // mainContainer.Panel2
            // 
            this.mainContainer.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.mainContainer.Panel2.Controls.Add(this.dataPlaceholder);
            this.mainContainer.Panel2.Controls.Add(this.tableLayoutPanel2);
            this.mainContainer.Size = new System.Drawing.Size(735, 575);
            this.mainContainer.SplitterDistance = 152;
            this.mainContainer.SplitterWidth = 10;
            this.mainContainer.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.button_sensor_productTop, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.button_sensor_productBottom, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(685, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(50, 413);
            this.tableLayoutPanel2.TabIndex = 9;
            // 
            // dataPlaceholder
            // 
            this.dataPlaceholder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataPlaceholder.Location = new System.Drawing.Point(0, 0);
            this.dataPlaceholder.Name = "dataPlaceholder";
            this.dataPlaceholder.Size = new System.Drawing.Size(685, 413);
            this.dataPlaceholder.TabIndex = 10;
            // 
            // button_sensor_productTop
            // 
            this.button_sensor_productTop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_sensor_productTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_sensor_productTop.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_sensor_productTop.Image = global::components.Properties.Resources.button_sensor_productTop_50;
            this.button_sensor_productTop.Location = new System.Drawing.Point(3, 3);
            this.button_sensor_productTop.Name = "button_sensor_productTop";
            this.button_sensor_productTop.Size = new System.Drawing.Size(44, 200);
            this.button_sensor_productTop.TabIndex = 0;
            this.button_sensor_productTop.TabStop = false;
            this.button_sensor_productTop.Tag = "prodTop";
            this.button_sensor_productTop.UseVisualStyleBackColor = true;
            this.button_sensor_productTop.Click += new System.EventHandler(this.button_sensor_productTop_Click);
            // 
            // button_sensor_productBottom
            // 
            this.button_sensor_productBottom.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_sensor_productBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_sensor_productBottom.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_sensor_productBottom.Image = global::components.Properties.Resources.button_sensor_productBottom_50;
            this.button_sensor_productBottom.Location = new System.Drawing.Point(3, 209);
            this.button_sensor_productBottom.Name = "button_sensor_productBottom";
            this.button_sensor_productBottom.Size = new System.Drawing.Size(44, 201);
            this.button_sensor_productBottom.TabIndex = 1;
            this.button_sensor_productBottom.TabStop = false;
            this.button_sensor_productBottom.Tag = "prodBot";
            this.button_sensor_productBottom.UseVisualStyleBackColor = true;
            this.button_sensor_productBottom.Click += new System.EventHandler(this.button_sensor_productBottom_Click);
            // 
            // categoryNavBar1
            // 
            this.categoryNavBar1.CategoryDataSource = null;
            this.categoryNavBar1.DisplayedCategoryFilter = "";
            this.categoryNavBar1.DisplayedCategoryId = "";
            this.categoryNavBar1.DisplayedPage = 0;
            this.categoryNavBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.categoryNavBar1.Location = new System.Drawing.Point(0, 0);
            this.categoryNavBar1.Name = "categoryNavBar1";
            this.categoryNavBar1.Size = new System.Drawing.Size(735, 152);
            this.categoryNavBar1.TabIndex = 0;
            this.categoryNavBar1.UseAutomaticButtonRendering = true;
            // 
            // SensorDataPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainContainer);
            this.Name = "SensorDataPanel";
            this.Size = new System.Drawing.Size(735, 575);
            this.mainContainer.Panel1.ResumeLayout(false);
            this.mainContainer.Panel2.ResumeLayout(false);
            this.mainContainer.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer mainContainer;
        private CategoryNavBar.CategoryNavBar categoryNavBar1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button button_sensor_productTop;
        private System.Windows.Forms.Button button_sensor_productBottom;
        private System.Windows.Forms.Panel dataPlaceholder;
    }
}
