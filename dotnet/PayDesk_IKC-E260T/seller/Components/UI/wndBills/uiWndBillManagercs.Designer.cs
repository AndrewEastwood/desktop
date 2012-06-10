namespace PayDesk.Components.UI.wndBills
{
    partial class uiWndBillManagercs
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uiWndBillManagercs));
            this.dateTimePicker_StartDate = new System.Windows.Forms.DateTimePicker();
            this.listGrid = new System.Windows.Forms.DataGridView();
            this.ColumnOID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSelect = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ColumnBillNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnBillComment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnBillSuma = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnLock = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ColumnOrder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.позначитиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.знятиВиділенняToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.позначитиВсіToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.знятиДляВсіхToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.видалитиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.видалитиПозначеніToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.billGrid = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.dateTimePicker_EndDate = new System.Windows.Forms.DateTimePicker();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label_orderInfo_General = new System.Windows.Forms.Label();
            this.label_orderInfo_discount = new System.Windows.Forms.Label();
            this.label_orderInfo_orderNo = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label_orderInfo_realSuma = new System.Windows.Forms.Label();
            this.label_orderInfo_suma = new System.Windows.Forms.Label();
            this.label_billInfo_State = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.listGrid)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.billGrid)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dateTimePicker_StartDate
            // 
            this.dateTimePicker_StartDate.Location = new System.Drawing.Point(12, 25);
            this.dateTimePicker_StartDate.Name = "dateTimePicker_StartDate";
            this.dateTimePicker_StartDate.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker_StartDate.TabIndex = 0;
            this.dateTimePicker_StartDate.ValueChanged += new System.EventHandler(this.dateTimePicker_DateFilter_ValueChanged);
            // 
            // listGrid
            // 
            this.listGrid.AllowUserToAddRows = false;
            this.listGrid.AllowUserToDeleteRows = false;
            this.listGrid.AllowUserToResizeRows = false;
            this.listGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.listGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnOID,
            this.ColumnPath,
            this.ColumnSelect,
            this.ColumnBillNo,
            this.ColumnDate,
            this.ColumnBillComment,
            this.ColumnBillSuma,
            this.ColumnLock,
            this.ColumnOrder});
            this.listGrid.Location = new System.Drawing.Point(12, 51);
            this.listGrid.Name = "listGrid";
            this.listGrid.ReadOnly = true;
            this.listGrid.RowHeadersVisible = false;
            this.listGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.listGrid.Size = new System.Drawing.Size(406, 447);
            this.listGrid.StandardTab = true;
            this.listGrid.TabIndex = 2;
            this.listGrid.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.listGrid_CellMouseClick);
            this.listGrid.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.listGrid_CellMouseDoubleClick);
            this.listGrid.SelectionChanged += new System.EventHandler(this.listGrid_SelectionChanged);
            // 
            // ColumnOID
            // 
            this.ColumnOID.HeaderText = "OID";
            this.ColumnOID.Name = "ColumnOID";
            this.ColumnOID.ReadOnly = true;
            this.ColumnOID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnOID.Visible = false;
            // 
            // ColumnPath
            // 
            this.ColumnPath.HeaderText = "Path";
            this.ColumnPath.Name = "ColumnPath";
            this.ColumnPath.ReadOnly = true;
            this.ColumnPath.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnPath.Visible = false;
            // 
            // ColumnSelect
            // 
            this.ColumnSelect.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnSelect.HeaderText = "*";
            this.ColumnSelect.Name = "ColumnSelect";
            this.ColumnSelect.ReadOnly = true;
            this.ColumnSelect.Width = 17;
            // 
            // ColumnBillNo
            // 
            this.ColumnBillNo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnBillNo.HeaderText = "№";
            this.ColumnBillNo.Name = "ColumnBillNo";
            this.ColumnBillNo.ReadOnly = true;
            this.ColumnBillNo.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnBillNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnBillNo.Width = 24;
            // 
            // ColumnDate
            // 
            this.ColumnDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnDate.HeaderText = "Дата";
            this.ColumnDate.Name = "ColumnDate";
            this.ColumnDate.ReadOnly = true;
            this.ColumnDate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnDate.Width = 39;
            // 
            // ColumnBillComment
            // 
            this.ColumnBillComment.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnBillComment.HeaderText = "Коментар";
            this.ColumnBillComment.Name = "ColumnBillComment";
            this.ColumnBillComment.ReadOnly = true;
            this.ColumnBillComment.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnBillComment.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColumnBillSuma
            // 
            this.ColumnBillSuma.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnBillSuma.HeaderText = "Сума";
            this.ColumnBillSuma.Name = "ColumnBillSuma";
            this.ColumnBillSuma.ReadOnly = true;
            this.ColumnBillSuma.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnBillSuma.Width = 39;
            // 
            // ColumnLock
            // 
            this.ColumnLock.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnLock.HeaderText = "!";
            this.ColumnLock.Name = "ColumnLock";
            this.ColumnLock.ReadOnly = true;
            this.ColumnLock.Width = 16;
            // 
            // ColumnOrder
            // 
            this.ColumnOrder.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnOrder.HeaderText = "Чек";
            this.ColumnOrder.Name = "ColumnOrder";
            this.ColumnOrder.ReadOnly = true;
            this.ColumnOrder.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnOrder.Width = 33;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.позначитиToolStripMenuItem,
            this.знятиВиділенняToolStripMenuItem,
            this.toolStripMenuItem1,
            this.позначитиВсіToolStripMenuItem,
            this.знятиДляВсіхToolStripMenuItem,
            this.toolStripMenuItem2,
            this.видалитиToolStripMenuItem,
            this.toolStripMenuItem3,
            this.видалитиПозначеніToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(217, 154);
            this.contextMenuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip1_ItemClicked);
            // 
            // позначитиToolStripMenuItem
            // 
            this.позначитиToolStripMenuItem.Name = "позначитиToolStripMenuItem";
            this.позначитиToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.позначитиToolStripMenuItem.Tag = "ItemSelectOne";
            this.позначитиToolStripMenuItem.Text = "Позначити";
            // 
            // знятиВиділенняToolStripMenuItem
            // 
            this.знятиВиділенняToolStripMenuItem.Name = "знятиВиділенняToolStripMenuItem";
            this.знятиВиділенняToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.знятиВиділенняToolStripMenuItem.Tag = "ItemDeselectOne";
            this.знятиВиділенняToolStripMenuItem.Text = "Скасувати позначення";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(213, 6);
            // 
            // позначитиВсіToolStripMenuItem
            // 
            this.позначитиВсіToolStripMenuItem.Name = "позначитиВсіToolStripMenuItem";
            this.позначитиВсіToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.позначитиВсіToolStripMenuItem.Tag = "ItemCheckAll";
            this.позначитиВсіToolStripMenuItem.Text = "Позначити всі";
            // 
            // знятиДляВсіхToolStripMenuItem
            // 
            this.знятиДляВсіхToolStripMenuItem.Name = "знятиДляВсіхToolStripMenuItem";
            this.знятиДляВсіхToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.знятиДляВсіхToolStripMenuItem.Tag = "ItemDeselectAll";
            this.знятиДляВсіхToolStripMenuItem.Text = "Скасувати всі позначення";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(213, 6);
            // 
            // видалитиToolStripMenuItem
            // 
            this.видалитиToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.видалитиToolStripMenuItem.ForeColor = System.Drawing.Color.Maroon;
            this.видалитиToolStripMenuItem.Name = "видалитиToolStripMenuItem";
            this.видалитиToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.видалитиToolStripMenuItem.Tag = "ItemDelete";
            this.видалитиToolStripMenuItem.Text = "Видалити";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(213, 6);
            // 
            // видалитиПозначеніToolStripMenuItem
            // 
            this.видалитиПозначеніToolStripMenuItem.Name = "видалитиПозначеніToolStripMenuItem";
            this.видалитиПозначеніToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.видалитиПозначеніToolStripMenuItem.Tag = "ItemDeleteChecked";
            this.видалитиПозначеніToolStripMenuItem.Text = "Видалити позначені";
            // 
            // billGrid
            // 
            this.billGrid.AllowUserToAddRows = false;
            this.billGrid.AllowUserToDeleteRows = false;
            this.billGrid.AllowUserToResizeColumns = false;
            this.billGrid.AllowUserToResizeRows = false;
            this.billGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.billGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.billGrid.Enabled = false;
            this.billGrid.Location = new System.Drawing.Point(424, 51);
            this.billGrid.Name = "billGrid";
            this.billGrid.ReadOnly = true;
            this.billGrid.RowHeadersVisible = false;
            this.billGrid.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.White;
            this.billGrid.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
            this.billGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.billGrid.Size = new System.Drawing.Size(296, 258);
            this.billGrid.StandardTab = true;
            this.billGrid.TabIndex = 3;
            this.billGrid.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Фільтр рахунків по даті";
            // 
            // dateTimePicker_EndDate
            // 
            this.dateTimePicker_EndDate.Location = new System.Drawing.Point(218, 25);
            this.dateTimePicker_EndDate.Name = "dateTimePicker_EndDate";
            this.dateTimePicker_EndDate.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker_EndDate.TabIndex = 1;
            this.dateTimePicker_EndDate.ValueChanged += new System.EventHandler(this.dateTimePicker_DateFilter_ValueChanged);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label_orderInfo_General);
            this.panel1.Controls.Add(this.label_orderInfo_discount);
            this.panel1.Controls.Add(this.label_orderInfo_orderNo);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label_orderInfo_realSuma);
            this.panel1.Controls.Add(this.label_orderInfo_suma);
            this.panel1.Controls.Add(this.label_billInfo_State);
            this.panel1.Location = new System.Drawing.Point(424, 315);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(296, 183);
            this.panel1.TabIndex = 6;
            // 
            // label_orderInfo_General
            // 
            this.label_orderInfo_General.AutoSize = true;
            this.label_orderInfo_General.Location = new System.Drawing.Point(22, 153);
            this.label_orderInfo_General.Name = "label_orderInfo_General";
            this.label_orderInfo_General.Size = new System.Drawing.Size(120, 13);
            this.label_orderInfo_General.TabIndex = 0;
            this.label_orderInfo_General.Text = "label_orderInfo_General";
            // 
            // label_orderInfo_discount
            // 
            this.label_orderInfo_discount.AutoSize = true;
            this.label_orderInfo_discount.Location = new System.Drawing.Point(22, 106);
            this.label_orderInfo_discount.Name = "label_orderInfo_discount";
            this.label_orderInfo_discount.Size = new System.Drawing.Size(123, 13);
            this.label_orderInfo_discount.TabIndex = 0;
            this.label_orderInfo_discount.Text = "label_orderInfo_discount";
            // 
            // label_orderInfo_orderNo
            // 
            this.label_orderInfo_orderNo.AutoSize = true;
            this.label_orderInfo_orderNo.Location = new System.Drawing.Point(22, 85);
            this.label_orderInfo_orderNo.Name = "label_orderInfo_orderNo";
            this.label_orderInfo_orderNo.Size = new System.Drawing.Size(121, 13);
            this.label_orderInfo_orderNo.TabIndex = 0;
            this.label_orderInfo_orderNo.Text = "label_orderInfo_orderNo";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(3, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(137, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Загальна інформація:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(3, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(146, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Інформація по рахунку:";
            // 
            // label_orderInfo_realSuma
            // 
            this.label_orderInfo_realSuma.AutoSize = true;
            this.label_orderInfo_realSuma.Location = new System.Drawing.Point(22, 64);
            this.label_orderInfo_realSuma.Name = "label_orderInfo_realSuma";
            this.label_orderInfo_realSuma.Size = new System.Drawing.Size(127, 13);
            this.label_orderInfo_realSuma.TabIndex = 0;
            this.label_orderInfo_realSuma.Text = "label_orderInfo_realSuma";
            // 
            // label_orderInfo_suma
            // 
            this.label_orderInfo_suma.AutoSize = true;
            this.label_orderInfo_suma.Location = new System.Drawing.Point(22, 43);
            this.label_orderInfo_suma.Name = "label_orderInfo_suma";
            this.label_orderInfo_suma.Size = new System.Drawing.Size(108, 13);
            this.label_orderInfo_suma.TabIndex = 0;
            this.label_orderInfo_suma.Text = "label_orderInfo_suma";
            // 
            // label_billInfo_State
            // 
            this.label_billInfo_State.AutoSize = true;
            this.label_billInfo_State.Location = new System.Drawing.Point(22, 22);
            this.label_billInfo_State.Name = "label_billInfo_State";
            this.label_billInfo_State.Size = new System.Drawing.Size(96, 13);
            this.label_billInfo_State.TabIndex = 0;
            this.label_billInfo_State.Text = "label_billInfo_State";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(421, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Вибраний рахунок";
            // 
            // uiWndBillManagercs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(732, 510);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.listGrid);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.billGrid);
            this.Controls.Add(this.dateTimePicker_StartDate);
            this.Controls.Add(this.dateTimePicker_EndDate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "uiWndBillManagercs";
            this.Text = "Менеджер Рахунків";
            ((System.ComponentModel.ISupportInitialize)(this.listGrid)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.billGrid)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimePicker_StartDate;
        private System.Windows.Forms.DataGridView listGrid;
        private System.Windows.Forms.DataGridView billGrid;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dateTimePicker_EndDate;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem видалитиToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem позначитиВсіToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem позначитиToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem знятиВиділенняToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem знятиДляВсіхToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem видалитиПозначеніToolStripMenuItem;
        private System.Windows.Forms.Label label_orderInfo_discount;
        private System.Windows.Forms.Label label_orderInfo_orderNo;
        private System.Windows.Forms.Label label_orderInfo_realSuma;
        private System.Windows.Forms.Label label_orderInfo_suma;
        private System.Windows.Forms.Label label_billInfo_State;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label_orderInfo_General;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnOID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPath;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColumnSelect;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnBillNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnBillComment;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnBillSuma;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColumnLock;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnOrder;
    }
}