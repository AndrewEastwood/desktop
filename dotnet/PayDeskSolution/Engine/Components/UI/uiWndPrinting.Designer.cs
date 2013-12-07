namespace PayDesk.Components.UI
{
    partial class uiWndPrinting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uiWndPrinting));
            this.button_SaveAndClose = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button7 = new System.Windows.Forms.Button();
            this.textBox_currentTemplate = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.listBox_printers = new System.Windows.Forms.ListBox();
            this.button10 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage0 = new System.Windows.Forms.TabPage();
            this.button_addPrinter = new System.Windows.Forms.Button();
            this.printersGrid = new System.Windows.Forms.DataGridView();
            this.ColumnPrnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnPrnPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnPrnTpl = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnPrnAction = new System.Windows.Forms.DataGridViewButtonColumn();
            this.ColumnPrnType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ColumnPrnBrowse = new System.Windows.Forms.DataGridViewButtonColumn();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.controlGrid = new System.Windows.Forms.DataGridView();
            this.ColumnCtrlDelete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.ColumnCtrlBrowse = new System.Windows.Forms.DataGridViewButtonColumn();
            this.ColumnCtrlActive = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ColumnCtrlType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button_savePrnOnly = new System.Windows.Forms.Button();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.printersGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.controlGrid)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_SaveAndClose
            // 
            this.button_SaveAndClose.Location = new System.Drawing.Point(507, 377);
            this.button_SaveAndClose.Name = "button_SaveAndClose";
            this.button_SaveAndClose.Size = new System.Drawing.Size(75, 23);
            this.button_SaveAndClose.TabIndex = 0;
            this.button_SaveAndClose.Text = "Добре";
            this.button_SaveAndClose.UseVisualStyleBackColor = true;
            this.button_SaveAndClose.Click += new System.EventHandler(this.button_SaveAndClose_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(107, 49);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(23, 23);
            this.button6.TabIndex = 15;
            this.button6.Text = "...";
            this.button6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button6.UseCompatibleTextRendering = true;
            this.button6.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(156, 94);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Фон";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(140, 19);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 18;
            this.button4.Text = "/\\";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(140, 48);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 19;
            this.button5.Text = "\\/";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // fontDialog1
            // 
            this.fontDialog1.FontMustExist = true;
            this.fontDialog1.ShowApply = true;
            this.fontDialog1.ShowColor = true;
            this.fontDialog1.ShowHelp = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "Шаблони друку|*.txt";
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "Шаблони друку|*.txt";
            this.openFileDialog1.RestoreDirectory = true;
            this.openFileDialog1.Title = "Шаблони для додаткового друку";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.listBox_printers);
            this.tabPage1.Controls.Add(this.button10);
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.Controls.Add(this.textBox3);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.button2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.textBox2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(562, 333);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Tag = "File";
            this.tabPage1.Text = "Шаблони";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button7);
            this.groupBox1.Controls.Add(this.textBox_currentTemplate);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Location = new System.Drawing.Point(6, 125);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(550, 50);
            this.groupBox1.TabIndex = 30;
            this.groupBox1.TabStop = false;
            this.groupBox1.Tag = "Шаблон принтеру для:";
            this.groupBox1.Text = "Шаблон принтеру для:";
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(427, 19);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(117, 20);
            this.button7.TabIndex = 31;
            this.button7.Text = "Зберегти Шаблон";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // textBox_currentTemplate
            // 
            this.textBox_currentTemplate.Location = new System.Drawing.Point(22, 19);
            this.textBox_currentTemplate.Name = "textBox_currentTemplate";
            this.textBox_currentTemplate.Size = new System.Drawing.Size(350, 20);
            this.textBox_currentTemplate.TabIndex = 6;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(378, 19);
            this.button3.Name = "button3";
            this.button3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.button3.Size = new System.Drawing.Size(23, 20);
            this.button3.TabIndex = 7;
            this.button3.Tag = "1";
            this.button3.Text = "...";
            this.button3.UseCompatibleTextRendering = true;
            this.button3.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(105, 13);
            this.label4.TabIndex = 29;
            this.label4.Text = "Доступні принтери:";
            // 
            // listBox_printers
            // 
            this.listBox_printers.ColumnWidth = 240;
            this.listBox_printers.DisplayMember = "CustomDataItem";
            this.listBox_printers.Location = new System.Drawing.Point(6, 24);
            this.listBox_printers.MultiColumn = true;
            this.listBox_printers.Name = "listBox_printers";
            this.listBox_printers.Size = new System.Drawing.Size(550, 95);
            this.listBox_printers.TabIndex = 28;
            this.listBox_printers.UseTabStops = false;
            this.listBox_printers.ValueMember = "CustomDataItem";
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(533, 284);
            this.button10.Name = "button10";
            this.button10.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.button10.Size = new System.Drawing.Size(23, 20);
            this.button10.TabIndex = 27;
            this.button10.Tag = "3";
            this.button10.Text = "...";
            this.button10.UseCompatibleTextRendering = true;
            this.button10.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(6, 207);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(521, 20);
            this.textBox1.TabIndex = 4;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(6, 285);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(521, 20);
            this.textBox3.TabIndex = 24;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(533, 206);
            this.button1.Name = "button1";
            this.button1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.button1.Size = new System.Drawing.Size(23, 20);
            this.button1.TabIndex = 5;
            this.button1.Tag = "1";
            this.button1.Text = "...";
            this.button1.UseCompatibleTextRendering = true;
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(533, 245);
            this.button2.Name = "button2";
            this.button2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.button2.Size = new System.Drawing.Size(23, 20);
            this.button2.TabIndex = 25;
            this.button2.Tag = "2";
            this.button2.Text = "...";
            this.button2.UseCompatibleTextRendering = true;
            this.button2.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 191);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Шаблон фіксованого чеку";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 269);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Шаблон рахунку";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 230);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(152, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Шаблон не фіксованого чеку";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(6, 246);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(521, 20);
            this.textBox2.TabIndex = 22;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage0);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(598, 426);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(570, 359);
            this.tabControl1.TabIndex = 20;
            // 
            // tabPage0
            // 
            this.tabPage0.Location = new System.Drawing.Point(4, 22);
            this.tabPage0.Name = "tabPage0";
            this.tabPage0.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage0.Size = new System.Drawing.Size(562, 333);
            this.tabPage0.TabIndex = 1;
            this.tabPage0.Text = "Принтер";
            this.tabPage0.UseVisualStyleBackColor = true;
            // 
            // button_addPrinter
            // 
            this.button_addPrinter.Location = new System.Drawing.Point(12, 377);
            this.button_addPrinter.Name = "button_addPrinter";
            this.button_addPrinter.Size = new System.Drawing.Size(103, 23);
            this.button_addPrinter.TabIndex = 24;
            this.button_addPrinter.Text = "Додати принтер";
            this.button_addPrinter.UseVisualStyleBackColor = true;
            this.button_addPrinter.Click += new System.EventHandler(this.button_addPrinter_Click);
            // 
            // printersGrid
            // 
            this.printersGrid.AllowUserToAddRows = false;
            this.printersGrid.AllowUserToResizeRows = false;
            this.printersGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.printersGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.printersGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnPrnName,
            this.ColumnPrnPath,
            this.ColumnPrnTpl,
            this.ColumnPrnAction,
            this.ColumnPrnType,
            this.ColumnPrnBrowse});
            this.printersGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.printersGrid.Location = new System.Drawing.Point(251, 0);
            this.printersGrid.Name = "printersGrid";
            this.printersGrid.RowHeadersVisible = false;
            this.printersGrid.Size = new System.Drawing.Size(317, 357);
            this.printersGrid.TabIndex = 23;
            // 
            // ColumnPrnName
            // 
            this.ColumnPrnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnPrnName.HeaderText = "Назва";
            this.ColumnPrnName.Name = "ColumnPrnName";
            this.ColumnPrnName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnPrnName.Width = 45;
            // 
            // ColumnPrnPath
            // 
            this.ColumnPrnPath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnPrnPath.HeaderText = "Шлях до програми друку";
            this.ColumnPrnPath.Name = "ColumnPrnPath";
            this.ColumnPrnPath.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnPrnPath.Width = 98;
            // 
            // ColumnPrnTpl
            // 
            this.ColumnPrnTpl.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnPrnTpl.HeaderText = "Шаблон";
            this.ColumnPrnTpl.Name = "ColumnPrnTpl";
            this.ColumnPrnTpl.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnPrnTpl.Width = 52;
            // 
            // ColumnPrnAction
            // 
            this.ColumnPrnAction.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnPrnAction.HeaderText = "";
            this.ColumnPrnAction.Name = "ColumnPrnAction";
            this.ColumnPrnAction.ReadOnly = true;
            this.ColumnPrnAction.Text = "X";
            this.ColumnPrnAction.ToolTipText = "Видалити";
            this.ColumnPrnAction.Visible = false;
            // 
            // ColumnPrnType
            // 
            this.ColumnPrnType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnPrnType.HeaderText = "Тип";
            this.ColumnPrnType.Items.AddRange(new object[] {
            "Стандарт (Фіскальний або Чек)",
            "Кухня (Різниця товарів)",
            "Рахунок Клієнту",
            "Звіт по рахунках"});
            this.ColumnPrnType.Name = "ColumnPrnType";
            this.ColumnPrnType.ReadOnly = true;
            this.ColumnPrnType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnPrnType.Visible = false;
            // 
            // ColumnPrnBrowse
            // 
            this.ColumnPrnBrowse.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnPrnBrowse.HeaderText = "";
            this.ColumnPrnBrowse.Name = "ColumnPrnBrowse";
            this.ColumnPrnBrowse.ReadOnly = true;
            this.ColumnPrnBrowse.Visible = false;
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.DefaultExt = "Програма друку|*.exe";
            this.openFileDialog2.Filter = "Програма друку|*.exe|Інші файли|*.*";
            this.openFileDialog2.RestoreDirectory = true;
            this.openFileDialog2.Title = "Програми для додаткового друку";
            // 
            // controlGrid
            // 
            this.controlGrid.AllowUserToAddRows = false;
            this.controlGrid.AllowUserToDeleteRows = false;
            this.controlGrid.AllowUserToResizeColumns = false;
            this.controlGrid.AllowUserToResizeRows = false;
            this.controlGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.controlGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.controlGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnCtrlDelete,
            this.ColumnCtrlBrowse,
            this.ColumnCtrlActive,
            this.ColumnCtrlType});
            this.controlGrid.Dock = System.Windows.Forms.DockStyle.Left;
            this.controlGrid.Location = new System.Drawing.Point(0, 0);
            this.controlGrid.Name = "controlGrid";
            this.controlGrid.RowHeadersVisible = false;
            this.controlGrid.Size = new System.Drawing.Size(251, 357);
            this.controlGrid.TabIndex = 27;
            this.controlGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.controlGrid_CellContentClick);
            // 
            // ColumnCtrlDelete
            // 
            this.ColumnCtrlDelete.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnCtrlDelete.HeaderText = "";
            this.ColumnCtrlDelete.Name = "ColumnCtrlDelete";
            this.ColumnCtrlDelete.Width = 5;
            // 
            // ColumnCtrlBrowse
            // 
            this.ColumnCtrlBrowse.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnCtrlBrowse.HeaderText = "";
            this.ColumnCtrlBrowse.Name = "ColumnCtrlBrowse";
            this.ColumnCtrlBrowse.Width = 5;
            // 
            // ColumnCtrlActive
            // 
            this.ColumnCtrlActive.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnCtrlActive.FalseValue = "0";
            this.ColumnCtrlActive.HeaderText = "";
            this.ColumnCtrlActive.IndeterminateValue = "2";
            this.ColumnCtrlActive.Name = "ColumnCtrlActive";
            this.ColumnCtrlActive.TrueValue = "1";
            this.ColumnCtrlActive.Width = 5;
            // 
            // ColumnCtrlType
            // 
            this.ColumnCtrlType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnCtrlType.HeaderText = "Тип Принтеру";
            this.ColumnCtrlType.Items.AddRange(new object[] {
            "Фіскальний",
            "Чек",
            "Рахунок Клієнту",
            "Кухня (Різниця товарів)",
            "Звіт по рахунках",
            "Звіт по чеках"});
            this.ColumnCtrlType.Name = "ColumnCtrlType";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.printersGrid);
            this.panel1.Controls.Add(this.controlGrid);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(570, 359);
            this.panel1.TabIndex = 28;
            // 
            // button_savePrnOnly
            // 
            this.button_savePrnOnly.Location = new System.Drawing.Point(121, 377);
            this.button_savePrnOnly.Name = "button_savePrnOnly";
            this.button_savePrnOnly.Size = new System.Drawing.Size(103, 23);
            this.button_savePrnOnly.TabIndex = 26;
            this.button_savePrnOnly.Text = "Зберегти Список";
            this.button_savePrnOnly.UseVisualStyleBackColor = true;
            this.button_savePrnOnly.Click += new System.EventHandler(this.button_savePrnOnly_Click);
            // 
            // uiWndPrinting
            // 
            this.AcceptButton = this.button_SaveAndClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 412);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button_savePrnOnly);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button_SaveAndClose);
            this.Controls.Add(this.button_addPrinter);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "uiWndPrinting";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "472; 570";
            this.Text = "Параметри друку";
            this.Load += new System.EventHandler(this.Printing_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Printing_KeyDown);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.printersGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.controlGrid)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_SaveAndClose;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage0;
        private System.Windows.Forms.Button button_addPrinter;
        private System.Windows.Forms.DataGridView printersGrid;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox_currentTemplate;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.ListBox listBox_printers;
        private System.Windows.Forms.DataGridView controlGrid;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPrnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPrnPath;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPrnTpl;
        private System.Windows.Forms.DataGridViewButtonColumn ColumnPrnAction;
        private System.Windows.Forms.DataGridViewComboBoxColumn ColumnPrnType;
        private System.Windows.Forms.DataGridViewButtonColumn ColumnPrnBrowse;
        private System.Windows.Forms.Button button_savePrnOnly;
        private System.Windows.Forms.DataGridViewButtonColumn ColumnCtrlDelete;
        private System.Windows.Forms.DataGridViewButtonColumn ColumnCtrlBrowse;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColumnCtrlActive;
        private System.Windows.Forms.DataGridViewComboBoxColumn ColumnCtrlType;
    }
}