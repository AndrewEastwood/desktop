namespace PayDesk
{
    partial class BillsList
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BillsList));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.billGrid = new System.Windows.Forms.DataGridView();
            this.listGrid = new System.Windows.Forms.DataGridView();
            this.nom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.billGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.listGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox1.Location = new System.Drawing.Point(12, 40);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(219, 30);
            this.textBox1.TabIndex = 9;
            // 
            // billGrid
            // 
            this.billGrid.AllowUserToAddRows = false;
            this.billGrid.AllowUserToDeleteRows = false;
            this.billGrid.AllowUserToResizeColumns = false;
            this.billGrid.AllowUserToResizeRows = false;
            this.billGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.billGrid.DefaultCellStyle = dataGridViewCellStyle1;
            this.billGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.billGrid.Enabled = false;
            this.billGrid.Location = new System.Drawing.Point(237, 40);
            this.billGrid.Name = "billGrid";
            this.billGrid.ReadOnly = true;
            this.billGrid.RowHeadersVisible = false;
            this.billGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.billGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.billGrid.Size = new System.Drawing.Size(387, 358);
            this.billGrid.TabIndex = 11;
            // 
            // listGrid
            // 
            this.listGrid.AllowUserToAddRows = false;
            this.listGrid.AllowUserToDeleteRows = false;
            this.listGrid.AllowUserToResizeColumns = false;
            this.listGrid.AllowUserToResizeRows = false;
            this.listGrid.BackgroundColor = System.Drawing.Color.White;
            this.listGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.listGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nom,
            this.dt,
            this.cmt});
            this.listGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.listGrid.Location = new System.Drawing.Point(12, 89);
            this.listGrid.MultiSelect = false;
            this.listGrid.Name = "listGrid";
            this.listGrid.ReadOnly = true;
            this.listGrid.RowHeadersVisible = false;
            this.listGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.listGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.listGrid.Size = new System.Drawing.Size(219, 309);
            this.listGrid.TabIndex = 12;
            this.listGrid.Enter += new System.EventHandler(this.listGrid_Enter);
            this.listGrid.Leave += new System.EventHandler(this.listGrid_Leave);
            // 
            // nom
            // 
            this.nom.HeaderText = "№";
            this.nom.Name = "nom";
            this.nom.ReadOnly = true;
            this.nom.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.nom.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.nom.Width = 40;
            // 
            // dt
            // 
            this.dt.HeaderText = "Дата";
            this.dt.Name = "dt";
            this.dt.ReadOnly = true;
            this.dt.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dt.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dt.Width = 80;
            // 
            // cmt
            // 
            this.cmt.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cmt.HeaderText = "Коментарій";
            this.cmt.Name = "cmt";
            this.cmt.ReadOnly = true;
            this.cmt.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.cmt.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Список рахунків";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(9, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(219, 28);
            this.label3.TabIndex = 17;
            this.label3.Text = "Пошук рахунку";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(234, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(183, 28);
            this.label2.TabIndex = 17;
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.ForeColor = System.Drawing.Color.Red;
            this.button3.Location = new System.Drawing.Point(549, 12);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 14;
            this.button3.Text = "Видалити";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(468, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 12;
            this.button2.Text = "Друкувати";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(549, 404);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "Відкрити";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // BillsList
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 439);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listGrid);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.billGrid);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BillsList";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Рахунки";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BillsList_KeyDown);
            this.Load += new System.EventHandler(this.BillsList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.billGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.listGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.DataGridView billGrid;
        private System.Windows.Forms.DataGridView listGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn nom;
        private System.Windows.Forms.DataGridViewTextBoxColumn dt;
        private System.Windows.Forms.DataGridViewTextBoxColumn cmt;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label3;
    }
}