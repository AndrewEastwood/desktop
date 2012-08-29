namespace PayDesk.Components.UI.wndBills
{
    partial class uiWndBillList
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uiWndBillList));
            this.textBox_billsList_billNoSearch = new System.Windows.Forms.TextBox();
            this.billGrid = new System.Windows.Forms.DataGridView();
            this.listGrid = new System.Windows.Forms.DataGridView();
            this.oid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LockSatet = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.FxNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.button_billsList_Delete = new System.Windows.Forms.Button();
            this.button_billsList_Print = new System.Windows.Forms.Button();
            this.button_billsList_Open = new System.Windows.Forms.Button();
            this.button_billsList_ShowAllBills = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label_billsList_listInfoPrefix = new System.Windows.Forms.Label();
            this.label_allBills_TotalRecords = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button_billsList_Refresh = new System.Windows.Forms.Button();
            this.button_billsList_madecopy = new System.Windows.Forms.Button();
            this.button_billsList_unlock = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label_orderInfo_orderNo = new System.Windows.Forms.Label();
            this.label_orderInfo_discount = new System.Windows.Forms.Label();
            this.label_orderInfo_suma = new System.Windows.Forms.Label();
            this.label_orderInfo_realSuma = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.button_billsList_Search = new System.Windows.Forms.Button();
            this.fileSystemWatcher_billIFolder = new System.IO.FileSystemWatcher();
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.panel6 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.billGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.listGrid)).BeginInit();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher_billIFolder)).BeginInit();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox_billsList_billNoSearch
            // 
            this.textBox_billsList_billNoSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox_billsList_billNoSearch.Location = new System.Drawing.Point(12, 12);
            this.textBox_billsList_billNoSearch.Name = "textBox_billsList_billNoSearch";
            this.textBox_billsList_billNoSearch.Size = new System.Drawing.Size(283, 30);
            this.textBox_billsList_billNoSearch.TabIndex = 3;
            this.textBox_billsList_billNoSearch.Tag = "введіть номер рахунку";
            this.textBox_billsList_billNoSearch.Text = "введіть номер рахунку";
            this.textBox_billsList_billNoSearch.Enter += new System.EventHandler(this.textBox_billsList_billNoSearch_Enter);
            this.textBox_billsList_billNoSearch.Validated += new System.EventHandler(this.textBox_billsList_billNoSearch_Validated);
            // 
            // billGrid
            // 
            this.billGrid.AllowUserToAddRows = false;
            this.billGrid.AllowUserToDeleteRows = false;
            this.billGrid.AllowUserToResizeColumns = false;
            this.billGrid.AllowUserToResizeRows = false;
            this.billGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.billGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.billGrid.DefaultCellStyle = dataGridViewCellStyle1;
            this.billGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.billGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.billGrid.Location = new System.Drawing.Point(0, 0);
            this.billGrid.Name = "billGrid";
            this.billGrid.ReadOnly = true;
            this.billGrid.RowHeadersVisible = false;
            this.billGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.billGrid.Size = new System.Drawing.Size(393, 243);
            this.billGrid.StandardTab = true;
            this.billGrid.TabIndex = 11;
            this.billGrid.TabStop = false;
            // 
            // listGrid
            // 
            this.listGrid.AllowUserToAddRows = false;
            this.listGrid.AllowUserToDeleteRows = false;
            this.listGrid.AllowUserToResizeColumns = false;
            this.listGrid.AllowUserToResizeRows = false;
            //this.listGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            //this.listGrid.
            this.listGrid.BackgroundColor = System.Drawing.Color.White;
            this.listGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.listGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.listGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.listGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.oid,
            this.nom,
            this.dt,
            this.cmt,
            this.sum,
            this.LockSatet,
            this.FxNo});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.listGrid.DefaultCellStyle = dataGridViewCellStyle3;
            this.listGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.listGrid.Location = new System.Drawing.Point(3, 3);
            this.listGrid.MultiSelect = false;
            this.listGrid.Name = "listGrid";
            this.listGrid.ReadOnly = true;
            this.listGrid.RowHeadersVisible = false;
            this.listGrid.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.listGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.listGrid.Size = new System.Drawing.Size(311, 278);
            this.listGrid.StandardTab = true;
            this.listGrid.TabIndex = 1;
            this.listGrid.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.listGrid_CellMouseClick);
            this.listGrid.Enter += new System.EventHandler(this.listGrid_Enter);
            this.listGrid.Leave += new System.EventHandler(this.listGrid_Leave);
            // 
            // oid
            // 
            this.oid.HeaderText = "#";
            this.oid.Name = "oid";
            this.oid.ReadOnly = true;
            this.oid.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.oid.Visible = false;
            this.oid.Width = 5;
            // 
            // nom
            // 
            this.nom.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.nom.HeaderText = "№";
            this.nom.Name = "nom";
            this.nom.ReadOnly = true;
            this.nom.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.nom.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.nom.Width = 24;
            // 
            // dt
            // 
            this.dt.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dt.HeaderText = "Дата";
            this.dt.Name = "dt";
            this.dt.ReadOnly = true;
            this.dt.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dt.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dt.Width = 39;
            // 
            // cmt
            // 
            this.cmt.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cmt.HeaderText = "Коментарій";
            this.cmt.Name = "cmt";
            this.cmt.ReadOnly = true;
            this.cmt.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.cmt.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // sum
            // 
            this.sum.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.sum.HeaderText = "СУМА";
            this.sum.Name = "sum";
            this.sum.ReadOnly = true;
            this.sum.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.sum.Width = 44;
            // 
            // LockSatet
            // 
            this.LockSatet.HeaderText = "*";
            this.LockSatet.Name = "LockSatet";
            this.LockSatet.ReadOnly = true;
            this.LockSatet.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.LockSatet.Width = 25;
            // 
            // FxNo
            // 
            this.FxNo.HeaderText = "Чек";
            this.FxNo.Name = "FxNo";
            this.FxNo.ReadOnly = true;
            this.FxNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.FxNo.Width = 60;
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(12, 440);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(770, 25);
            this.label2.TabIndex = 30;
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button_billsList_Delete
            // 
            this.button_billsList_Delete.Enabled = false;
            this.button_billsList_Delete.ForeColor = System.Drawing.Color.Red;
            this.button_billsList_Delete.Location = new System.Drawing.Point(387, 404);
            this.button_billsList_Delete.Name = "button_billsList_Delete";
            this.button_billsList_Delete.Size = new System.Drawing.Size(75, 23);
            this.button_billsList_Delete.TabIndex = 5;
            this.button_billsList_Delete.Text = "Анулювати";
            this.button_billsList_Delete.UseVisualStyleBackColor = true;
            this.button_billsList_Delete.Click += new System.EventHandler(this.button_billsList_Delete_Click);
            // 
            // button_billsList_Print
            // 
            this.button_billsList_Print.AutoSize = true;
            this.button_billsList_Print.Dock = System.Windows.Forms.DockStyle.Top;
            this.button_billsList_Print.Enabled = false;
            this.button_billsList_Print.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_billsList_Print.Location = new System.Drawing.Point(0, 0);
            this.button_billsList_Print.Name = "button_billsList_Print";
            this.button_billsList_Print.Size = new System.Drawing.Size(389, 35);
            this.button_billsList_Print.TabIndex = 2;
            this.button_billsList_Print.Text = "Друкувати Цей Рахунок";
            this.button_billsList_Print.UseVisualStyleBackColor = true;
            this.button_billsList_Print.Click += new System.EventHandler(this.button_billsList_Print_Click);
            // 
            // button_billsList_Open
            // 
            this.button_billsList_Open.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button_billsList_Open.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_billsList_Open.Location = new System.Drawing.Point(0, 286);
            this.button_billsList_Open.Name = "button_billsList_Open";
            this.button_billsList_Open.Size = new System.Drawing.Size(364, 45);
            this.button_billsList_Open.TabIndex = 8;
            this.button_billsList_Open.Text = "Відкрити позначений рахунок в основне вікно";
            this.button_billsList_Open.UseVisualStyleBackColor = true;
            this.button_billsList_Open.Click += new System.EventHandler(this.button_billsList_Open_Click);
            // 
            // button_billsList_ShowAllBills
            // 
            this.button_billsList_ShowAllBills.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_billsList_ShowAllBills.ForeColor = System.Drawing.Color.Black;
            this.button_billsList_ShowAllBills.Location = new System.Drawing.Point(66, 0);
            this.button_billsList_ShowAllBills.Margin = new System.Windows.Forms.Padding(0);
            this.button_billsList_ShowAllBills.Name = "button_billsList_ShowAllBills";
            this.button_billsList_ShowAllBills.Size = new System.Drawing.Size(298, 23);
            this.button_billsList_ShowAllBills.TabIndex = 10;
            this.button_billsList_ShowAllBills.Text = "Завантажити Рахунки";
            this.button_billsList_ShowAllBills.UseVisualStyleBackColor = true;
            this.button_billsList_ShowAllBills.Click += new System.EventHandler(this.button_billsList_ShowAllBills_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.panel6);
            this.panel1.Controls.Add(this.button_billsList_Open);
            this.panel1.Controls.Add(this.flowLayoutPanel1);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Location = new System.Drawing.Point(12, 48);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(366, 379);
            this.panel1.TabIndex = 20;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.label_billsList_listInfoPrefix);
            this.flowLayoutPanel1.Controls.Add(this.label_allBills_TotalRecords);
            this.flowLayoutPanel1.Controls.Add(this.label5);
            this.flowLayoutPanel1.Controls.Add(this.label6);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 331);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(364, 23);
            this.flowLayoutPanel1.TabIndex = 22;
            // 
            // label_billsList_listInfoPrefix
            // 
            this.label_billsList_listInfoPrefix.AutoSize = true;
            this.label_billsList_listInfoPrefix.Location = new System.Drawing.Point(3, 0);
            this.label_billsList_listInfoPrefix.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label_billsList_listInfoPrefix.Name = "label_billsList_listInfoPrefix";
            this.label_billsList_listInfoPrefix.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.label_billsList_listInfoPrefix.Size = new System.Drawing.Size(91, 23);
            this.label_billsList_listInfoPrefix.TabIndex = 26;
            this.label_billsList_listInfoPrefix.Text = "Всього рахунків:";
            // 
            // label_allBills_TotalRecords
            // 
            this.label_allBills_TotalRecords.AutoSize = true;
            this.label_allBills_TotalRecords.Location = new System.Drawing.Point(94, 0);
            this.label_allBills_TotalRecords.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.label_allBills_TotalRecords.Name = "label_allBills_TotalRecords";
            this.label_allBills_TotalRecords.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.label_allBills_TotalRecords.Size = new System.Drawing.Size(25, 23);
            this.label_allBills_TotalRecords.TabIndex = 27;
            this.label_allBills_TotalRecords.Text = "000";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(125, 0);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label5.Name = "label5";
            this.label5.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.label5.Size = new System.Drawing.Size(49, 23);
            this.label5.TabIndex = 28;
            this.label5.Text = "на суму:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(174, 0);
            this.label6.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.label6.Name = "label6";
            this.label6.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.label6.Size = new System.Drawing.Size(40, 23);
            this.label6.TabIndex = 29;
            this.label6.Text = "000,00";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button_billsList_ShowAllBills);
            this.panel2.Controls.Add(this.button_billsList_Refresh);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 354);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(364, 23);
            this.panel2.TabIndex = 21;
            // 
            // button_billsList_Refresh
            // 
            this.button_billsList_Refresh.AutoSize = true;
            this.button_billsList_Refresh.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button_billsList_Refresh.Dock = System.Windows.Forms.DockStyle.Left;
            this.button_billsList_Refresh.Location = new System.Drawing.Point(0, 0);
            this.button_billsList_Refresh.Margin = new System.Windows.Forms.Padding(0);
            this.button_billsList_Refresh.Name = "button_billsList_Refresh";
            this.button_billsList_Refresh.Size = new System.Drawing.Size(66, 23);
            this.button_billsList_Refresh.TabIndex = 9;
            this.button_billsList_Refresh.Text = "Обновити";
            this.button_billsList_Refresh.UseVisualStyleBackColor = true;
            this.button_billsList_Refresh.Click += new System.EventHandler(this.button_billsList_Refresh_Click);
            // 
            // button_billsList_madecopy
            // 
            this.button_billsList_madecopy.Enabled = false;
            this.button_billsList_madecopy.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_billsList_madecopy.ForeColor = System.Drawing.Color.Black;
            this.button_billsList_madecopy.Location = new System.Drawing.Point(468, 404);
            this.button_billsList_madecopy.Name = "button_billsList_madecopy";
            this.button_billsList_madecopy.Size = new System.Drawing.Size(114, 23);
            this.button_billsList_madecopy.TabIndex = 6;
            this.button_billsList_madecopy.Text = "Зробити Копію";
            this.button_billsList_madecopy.UseVisualStyleBackColor = true;
            this.button_billsList_madecopy.Click += new System.EventHandler(this.button_billsList_madecopy_Click);
            // 
            // button_billsList_unlock
            // 
            this.button_billsList_unlock.Enabled = false;
            this.button_billsList_unlock.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.button_billsList_unlock.Location = new System.Drawing.Point(588, 404);
            this.button_billsList_unlock.Name = "button_billsList_unlock";
            this.button_billsList_unlock.Size = new System.Drawing.Size(113, 23);
            this.button_billsList_unlock.TabIndex = 7;
            this.button_billsList_unlock.Text = "РОЗБЛОКУВАТИ";
            this.button_billsList_unlock.UseVisualStyleBackColor = true;
            this.button_billsList_unlock.Visible = false;
            this.button_billsList_unlock.Click += new System.EventHandler(this.button_billsList_unlock_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.billGrid);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Location = new System.Drawing.Point(387, 12);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(395, 386);
            this.panel3.TabIndex = 21;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.Window;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel4.Controls.Add(this.panel5);
            this.panel4.Controls.Add(this.button_billsList_Print);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 243);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(393, 141);
            this.panel4.TabIndex = 22;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label_orderInfo_orderNo);
            this.panel5.Controls.Add(this.label_orderInfo_discount);
            this.panel5.Controls.Add(this.label_orderInfo_suma);
            this.panel5.Controls.Add(this.label_orderInfo_realSuma);
            this.panel5.Controls.Add(this.label7);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 35);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(389, 102);
            this.panel5.TabIndex = 13;
            // 
            // label_orderInfo_orderNo
            // 
            this.label_orderInfo_orderNo.AutoSize = true;
            this.label_orderInfo_orderNo.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_orderInfo_orderNo.Location = new System.Drawing.Point(0, 78);
            this.label_orderInfo_orderNo.Margin = new System.Windows.Forms.Padding(0);
            this.label_orderInfo_orderNo.Name = "label_orderInfo_orderNo";
            this.label_orderInfo_orderNo.Padding = new System.Windows.Forms.Padding(20, 5, 5, 0);
            this.label_orderInfo_orderNo.Size = new System.Drawing.Size(60, 18);
            this.label_orderInfo_orderNo.TabIndex = 25;
            this.label_orderInfo_orderNo.Text = "label1";
            // 
            // label_orderInfo_discount
            // 
            this.label_orderInfo_discount.AutoSize = true;
            this.label_orderInfo_discount.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_orderInfo_discount.Location = new System.Drawing.Point(0, 60);
            this.label_orderInfo_discount.Margin = new System.Windows.Forms.Padding(0);
            this.label_orderInfo_discount.Name = "label_orderInfo_discount";
            this.label_orderInfo_discount.Padding = new System.Windows.Forms.Padding(20, 5, 5, 0);
            this.label_orderInfo_discount.Size = new System.Drawing.Size(60, 18);
            this.label_orderInfo_discount.TabIndex = 24;
            this.label_orderInfo_discount.Text = "label1";
            // 
            // label_orderInfo_suma
            // 
            this.label_orderInfo_suma.AutoSize = true;
            this.label_orderInfo_suma.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_orderInfo_suma.Location = new System.Drawing.Point(0, 42);
            this.label_orderInfo_suma.Margin = new System.Windows.Forms.Padding(0);
            this.label_orderInfo_suma.Name = "label_orderInfo_suma";
            this.label_orderInfo_suma.Padding = new System.Windows.Forms.Padding(20, 5, 5, 0);
            this.label_orderInfo_suma.Size = new System.Drawing.Size(60, 18);
            this.label_orderInfo_suma.TabIndex = 23;
            this.label_orderInfo_suma.Text = "label1";
            // 
            // label_orderInfo_realSuma
            // 
            this.label_orderInfo_realSuma.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_orderInfo_realSuma.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_orderInfo_realSuma.Location = new System.Drawing.Point(0, 23);
            this.label_orderInfo_realSuma.Name = "label_orderInfo_realSuma";
            this.label_orderInfo_realSuma.Padding = new System.Windows.Forms.Padding(20, 2, 0, 0);
            this.label_orderInfo_realSuma.Size = new System.Drawing.Size(389, 19);
            this.label_orderInfo_realSuma.TabIndex = 22;
            this.label_orderInfo_realSuma.Text = "label9";
            // 
            // label7
            // 
            this.label7.Dock = System.Windows.Forms.DockStyle.Top;
            this.label7.Location = new System.Drawing.Point(0, 0);
            this.label7.Name = "label7";
            this.label7.Padding = new System.Windows.Forms.Padding(5);
            this.label7.Size = new System.Drawing.Size(389, 23);
            this.label7.TabIndex = 21;
            this.label7.Text = "Загальна Інформація:";
            // 
            // button_billsList_Search
            // 
            this.button_billsList_Search.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_billsList_Search.ForeColor = System.Drawing.Color.Green;
            this.button_billsList_Search.Location = new System.Drawing.Point(301, 13);
            this.button_billsList_Search.Name = "button_billsList_Search";
            this.button_billsList_Search.Size = new System.Drawing.Size(77, 29);
            this.button_billsList_Search.TabIndex = 4;
            this.button_billsList_Search.Text = "ШУКАТИ";
            this.button_billsList_Search.UseVisualStyleBackColor = true;
            this.button_billsList_Search.Click += new System.EventHandler(this.button_billsList_Search_Click);
            // 
            // fileSystemWatcher_billIFolder
            // 
            this.fileSystemWatcher_billIFolder.EnableRaisingEvents = true;
            this.fileSystemWatcher_billIFolder.SynchronizingObject = this;
            this.fileSystemWatcher_billIFolder.Changed += new System.IO.FileSystemEventHandler(this.fileSystemWatcher_billIFolder_Changed);
            // 
            // vScrollBar1
            // 
            this.vScrollBar1.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar1.Location = new System.Drawing.Point(314, 3);
            this.vScrollBar1.Name = "vScrollBar1";
            this.vScrollBar1.Size = new System.Drawing.Size(47, 278);
            this.vScrollBar1.TabIndex = 23;
            this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.listGrid);
            this.panel6.Controls.Add(this.vScrollBar1);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Margin = new System.Windows.Forms.Padding(0);
            this.panel6.Name = "panel6";
            this.panel6.Padding = new System.Windows.Forms.Padding(3, 3, 3, 5);
            this.panel6.Size = new System.Drawing.Size(364, 286);
            this.panel6.TabIndex = 24;
            // 
            // uiWndBillList
            // 
            this.AcceptButton = this.button_billsList_Open;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 474);
            this.Controls.Add(this.button_billsList_unlock);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button_billsList_Search);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.textBox_billsList_billNoSearch);
            this.Controls.Add(this.button_billsList_madecopy);
            this.Controls.Add(this.button_billsList_Delete);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "uiWndBillList";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Рахунки";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.uiWndBillList_FormClosing);
            this.Load += new System.EventHandler(this.BillsList_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BillsList_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.billGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.listGrid)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher_billIFolder)).EndInit();
            this.panel6.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_billsList_billNoSearch;
        private System.Windows.Forms.DataGridView billGrid;
        private System.Windows.Forms.DataGridView listGrid;
        private System.Windows.Forms.Button button_billsList_Open;
        private System.Windows.Forms.Button button_billsList_Print;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_billsList_Delete;
        private System.Windows.Forms.Button button_billsList_ShowAllBills;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button_billsList_madecopy;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button_billsList_Refresh;
        private System.Windows.Forms.Button button_billsList_unlock;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button button_billsList_Search;
        private System.Windows.Forms.Label label_allBills_TotalRecords;
        private System.Windows.Forms.Label label_billsList_listInfoPrefix;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label_orderInfo_realSuma;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label_orderInfo_orderNo;
        private System.Windows.Forms.Label label_orderInfo_discount;
        private System.Windows.Forms.Label label_orderInfo_suma;
        private System.Windows.Forms.DataGridViewTextBoxColumn oid;
        private System.Windows.Forms.DataGridViewTextBoxColumn nom;
        private System.Windows.Forms.DataGridViewTextBoxColumn dt;
        private System.Windows.Forms.DataGridViewTextBoxColumn cmt;
        private System.Windows.Forms.DataGridViewTextBoxColumn sum;
        private System.Windows.Forms.DataGridViewCheckBoxColumn LockSatet;
        private System.Windows.Forms.DataGridViewTextBoxColumn FxNo;
        private System.IO.FileSystemWatcher fileSystemWatcher_billIFolder;
        private System.Windows.Forms.VScrollBar vScrollBar1;
        private System.Windows.Forms.Panel panel6;
    }
}