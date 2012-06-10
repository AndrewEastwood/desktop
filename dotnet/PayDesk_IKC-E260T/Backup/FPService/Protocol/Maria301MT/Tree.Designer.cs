namespace FPService.Protocol.Maria301MT
{
    partial class Tree
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Марія 301МТМ");
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.ItemHeight = 18;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            treeNode1.Name = "Node0";
            treeNode1.NodeFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            treeNode1.Text = "Марія 301МТМ";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.treeView1.ShowNodeToolTips = true;
            this.treeView1.Size = new System.Drawing.Size(414, 580);
            this.treeView1.TabIndex = 0;
            // 
            // Tree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeView1);
            this.Name = "Tree";
            this.Size = new System.Drawing.Size(414, 580);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
    }
}

/*

            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Читання стану реєстратора", 1, 1);
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Читання дати реєстратора", 1, 1);
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Установка дати в реєстраторі", 1, 1);
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Читання часу з реєстратора", 1, 1);
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Установка часу в реєстраторі", 1, 1);
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Установка пароля", 1, 1);
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Реєстрація касира", 1, 1);
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Реєстрація виплати", 1, 1);
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Реєстрація коментаря у фіскальному чеку", 1, 1);
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Заборона/дозвіл друку у нефіскальному режимі", 1, 1);
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Переміщення паперу на один рядок", 1, 1);
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Обнулення чеку", 1, 1);
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Службове внесення грошей в грошову скриньку", 1, 1);
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("Реєстрація продаж товару або послуг", 1, 1);
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("Реєстрація сплати і друк чеку", 1, 1);
            System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("Початкове і заключне повідомлення", 1, 1);
            System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("Видача готівки з грошової скриньки", 1, 1);
            System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("Переслати рядок на індикатор", 1, 1);
            System.Windows.Forms.TreeNode treeNode19 = new System.Windows.Forms.TreeNode("Прочитати блок памяті принтера", 1, 1);
            System.Windows.Forms.TreeNode treeNode20 = new System.Windows.Forms.TreeNode("Відкриття грошової скриньки", 1, 1);
            System.Windows.Forms.TreeNode treeNode21 = new System.Windows.Forms.TreeNode("Друк копії останнього чека", 1, 1);
            System.Windows.Forms.TreeNode treeNode22 = new System.Windows.Forms.TreeNode("Друк ПН і версії ПЗ", 1, 1);
            System.Windows.Forms.TreeNode treeNode23 = new System.Windows.Forms.TreeNode("Сума готівки у грошовій скриньці", 1, 1);
            System.Windows.Forms.TreeNode treeNode24 = new System.Windows.Forms.TreeNode("Реєстрація знижки або націнки", 1, 1);
            System.Windows.Forms.TreeNode treeNode25 = new System.Windows.Forms.TreeNode("заборона/дозвіл Online реєстрації", 1, 1);
            System.Windows.Forms.TreeNode treeNode26 = new System.Windows.Forms.TreeNode("заборона/дозвіл виводу суми на індикатор", 1, 1);
            System.Windows.Forms.TreeNode treeNode27 = new System.Windows.Forms.TreeNode("Зміна швидкості", 1, 1);
            System.Windows.Forms.TreeNode treeNode28 = new System.Windows.Forms.TreeNode("Зміна міжрядкової дистанції", 1, 1);
            System.Windows.Forms.TreeNode treeNode29 = new System.Windows.Forms.TreeNode("Друк рядка службового звіту", 1, 1);
            System.Windows.Forms.TreeNode treeNode30 = new System.Windows.Forms.TreeNode("Прочитати дані артикула", 1, 1);
            System.Windows.Forms.TreeNode treeNode31 = new System.Windows.Forms.TreeNode("Прочитати дані денного звіту", 1, 1);
            System.Windows.Forms.TreeNode treeNode32 = new System.Windows.Forms.TreeNode("Прочитати дані поточного чека", 1, 1);
            System.Windows.Forms.TreeNode treeNode33 = new System.Windows.Forms.TreeNode("Прочитати податкові ставки", 1, 1);
            System.Windows.Forms.TreeNode treeNode34 = new System.Windows.Forms.TreeNode("заборона/дозвіл користування обрізувачем", 1, 1);
            System.Windows.Forms.TreeNode treeNode35 = new System.Windows.Forms.TreeNode("Таблиця сумісності касирів", 1, 1);
            System.Windows.Forms.TreeNode treeNode36 = new System.Windows.Forms.TreeNode("Функції режиму реєстрації", 0, 0, new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5,
            treeNode6,
            treeNode7,
            treeNode8,
            treeNode9,
            treeNode10,
            treeNode11,
            treeNode12,
            treeNode13,
            treeNode14,
            treeNode15,
            treeNode16,
            treeNode17,
            treeNode18,
            treeNode19,
            treeNode20,
            treeNode21,
            treeNode22,
            treeNode23,
            treeNode24,
            treeNode25,
            treeNode26,
            treeNode27,
            treeNode28,
            treeNode29,
            treeNode30,
            treeNode31,
            treeNode32,
            treeNode33,
            treeNode34,
            treeNode35});
            System.Windows.Forms.TreeNode treeNode37 = new System.Windows.Forms.TreeNode("Функції режиму програмування");
            System.Windows.Forms.TreeNode treeNode38 = new System.Windows.Forms.TreeNode("Функції режиму звітів");
            System.Windows.Forms.TreeNode treeNode39 = new System.Windows.Forms.TreeNode("IKC-E260T", 2, 2, new System.Windows.Forms.TreeNode[] {
            treeNode36,
            treeNode37,
            treeNode38});
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.ItemHeight = 18;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            treeNode1.ImageIndex = 1;
            treeNode1.Name = "SendStatus";
            treeNode1.SelectedImageIndex = 1;
            treeNode1.Tag = "0";
            treeNode1.Text = "Читання стану реєстратора";
            treeNode2.ImageIndex = 1;
            treeNode2.Name = "GetDate";
            treeNode2.SelectedImageIndex = 1;
            treeNode2.Tag = "1";
            treeNode2.Text = "Читання дати реєстратора";
            treeNode3.ImageIndex = 1;
            treeNode3.Name = "SetDate";
            treeNode3.SelectedImageIndex = 1;
            treeNode3.Tag = "2";
            treeNode3.Text = "Установка дати в реєстраторі";
            treeNode4.ImageIndex = 1;
            treeNode4.Name = "GetTime";
            treeNode4.SelectedImageIndex = 1;
            treeNode4.Tag = "3";
            treeNode4.Text = "Читання часу з реєстратора";
            treeNode5.ImageIndex = 1;
            treeNode5.Name = "SetTime";
            treeNode5.SelectedImageIndex = 1;
            treeNode5.Tag = "4";
            treeNode5.Text = "Установка часу в реєстраторі";
            treeNode6.ImageIndex = 1;
            treeNode6.Name = "SetCod";
            treeNode6.SelectedImageIndex = 1;
            treeNode6.Tag = "5";
            treeNode6.Text = "Установка пароля";
            treeNode7.ImageIndex = 1;
            treeNode7.Name = "SetCashier";
            treeNode7.SelectedImageIndex = 1;
            treeNode7.Tag = "6";
            treeNode7.Text = "Реєстрація касира";
            treeNode8.ImageIndex = 1;
            treeNode8.Name = "PayMoney";
            treeNode8.SelectedImageIndex = 1;
            treeNode8.Tag = "8";
            treeNode8.Text = "Реєстрація виплати";
            treeNode9.ImageIndex = 1;
            treeNode9.Name = "Comment";
            treeNode9.SelectedImageIndex = 1;
            treeNode9.Tag = "11";
            treeNode9.Text = "Реєстрація коментаря у фіскальному чеку";
            treeNode10.ImageIndex = 1;
            treeNode10.Name = "CplPrint";
            treeNode10.SelectedImageIndex = 1;
            treeNode10.Tag = "12";
            treeNode10.Text = "Заборона/дозвіл друку у нефіскальному режимі";
            treeNode11.ImageIndex = 1;
            treeNode11.Name = "LineFeed";
            treeNode11.SelectedImageIndex = 1;
            treeNode11.Tag = "14";
            treeNode11.Text = "Переміщення паперу на один рядок";
            treeNode12.ImageIndex = 1;
            treeNode12.Name = "ResetOrder";
            treeNode12.SelectedImageIndex = 1;
            treeNode12.Tag = "15";
            treeNode12.Text = "Обнулення чеку";
            treeNode13.ImageIndex = 1;
            treeNode13.Name = "Avans";
            treeNode13.SelectedImageIndex = 1;
            treeNode13.Tag = "16";
            treeNode13.Text = "Службове внесення грошей в грошову скриньку";
            treeNode14.ImageIndex = 1;
            treeNode14.Name = "Sale";
            treeNode14.SelectedImageIndex = 1;
            treeNode14.Tag = "18";
            treeNode14.Text = "Реєстрація продаж товару або послуг";
            treeNode15.ImageIndex = 1;
            treeNode15.Name = "Payment";
            treeNode15.SelectedImageIndex = 1;
            treeNode15.Tag = "20";
            treeNode15.Text = "Реєстрація сплати і друк чеку";
            treeNode16.ImageIndex = 1;
            treeNode16.Name = "SetString";
            treeNode16.SelectedImageIndex = 1;
            treeNode16.Tag = "23";
            treeNode16.Text = "Початкове і заключне повідомлення";
            treeNode17.ImageIndex = 1;
            treeNode17.Name = "Give";
            treeNode17.SelectedImageIndex = 1;
            treeNode17.Tag = "24";
            treeNode17.Text = "Видача готівки з грошової скриньки";
            treeNode18.ImageIndex = 1;
            treeNode18.Name = "SendCustomer";
            treeNode18.SelectedImageIndex = 1;
            treeNode18.Tag = "27";
            treeNode18.Text = "Переслати рядок на індикатор";
            treeNode19.ImageIndex = 1;
            treeNode19.Name = "GetMemory";
            treeNode19.SelectedImageIndex = 1;
            treeNode19.Tag = "28";
            treeNode19.Text = "Прочитати блок памяті принтера";
            treeNode20.ImageIndex = 1;
            treeNode20.Name = "OpenBox";
            treeNode20.SelectedImageIndex = 1;
            treeNode20.Tag = "29";
            treeNode20.Text = "Відкриття грошової скриньки";
            treeNode21.ImageIndex = 1;
            treeNode21.Name = "PrintCopy";
            treeNode21.SelectedImageIndex = 1;
            treeNode21.Tag = "30";
            treeNode21.Text = "Друк копії останнього чека";
            treeNode22.ImageIndex = 1;
            treeNode22.Name = "PrintVer";
            treeNode22.SelectedImageIndex = 1;
            treeNode22.Tag = "32";
            treeNode22.Text = "Друк ПН і версії ПЗ";
            treeNode23.ImageIndex = 1;
            treeNode23.Name = "GetBox";
            treeNode23.SelectedImageIndex = 1;
            treeNode23.Tag = "33";
            treeNode23.Text = "Сума готівки у грошовій скриньці";
            treeNode24.ImageIndex = 1;
            treeNode24.Name = "Discount";
            treeNode24.SelectedImageIndex = 1;
            treeNode24.Tag = "35";
            treeNode24.Text = "Реєстрація знижки або націнки";
            treeNode25.ImageIndex = 1;
            treeNode25.Name = "CplOnline";
            treeNode25.SelectedImageIndex = 1;
            treeNode25.Tag = "36";
            treeNode25.Text = "заборона/дозвіл Online реєстрації";
            treeNode26.ImageIndex = 1;
            treeNode26.Name = "CplInd";
            treeNode26.SelectedImageIndex = 1;
            treeNode26.Tag = "37";
            treeNode26.Text = "заборона/дозвіл виводу суми на індикатор";
            treeNode27.ImageIndex = 1;
            treeNode27.Name = "ChangeRate";
            treeNode27.SelectedImageIndex = 1;
            treeNode27.Tag = "38";
            treeNode27.Text = "Зміна швидкості";
            treeNode28.ImageIndex = 1;
            treeNode28.Name = "LineSP";
            treeNode28.SelectedImageIndex = 1;
            treeNode28.Tag = "39";
            treeNode28.Text = "Зміна міжрядкової дистанції";
            treeNode29.ImageIndex = 1;
            treeNode29.Name = "TransPrint";
            treeNode29.SelectedImageIndex = 1;
            treeNode29.Tag = "40";
            treeNode29.Text = "Друк рядка службового звіту";
            treeNode30.ImageIndex = 1;
            treeNode30.Name = "GetArticle";
            treeNode30.SelectedImageIndex = 1;
            treeNode30.Tag = "41";
            treeNode30.Text = "Прочитати дані артикула";
            treeNode31.ImageIndex = 1;
            treeNode31.Name = "GetDayReport";
            treeNode31.SelectedImageIndex = 1;
            treeNode31.Tag = "42";
            treeNode31.Text = "Прочитати дані денного звіту";
            treeNode32.ImageIndex = 1;
            treeNode32.Name = "GetCheckSums";
            treeNode32.SelectedImageIndex = 1;
            treeNode32.Tag = "43";
            treeNode32.Text = "Прочитати дані поточного чека";
            treeNode33.ImageIndex = 1;
            treeNode33.Name = "GetTextRates";
            treeNode33.SelectedImageIndex = 1;
            treeNode33.Tag = "44";
            treeNode33.Text = "Прочитати податкові ставки";
            treeNode34.ImageIndex = 1;
            treeNode34.Name = "CplCutter";
            treeNode34.SelectedImageIndex = 1;
            treeNode34.Tag = "46";
            treeNode34.Text = "заборона/дозвіл користування обрізувачем";
            treeNode35.ImageIndex = 1;
            treeNode35.Name = "UserTable";
            treeNode35.SelectedImageIndex = 1;
            treeNode35.Text = "Таблиця сумісності касирів";
            treeNode36.ImageIndex = 0;
            treeNode36.Name = "";
            treeNode36.SelectedImageIndex = 0;
            treeNode36.Tag = "";
            treeNode36.Text = "Функції режиму реєстрації";
            treeNode37.ImageIndex = 0;
            treeNode37.Name = "Node1";
            treeNode37.Text = "Функції режиму програмування";
            treeNode38.Name = "Node2";
            treeNode38.Text = "Функції режиму звітів";
            treeNode39.ImageIndex = 2;
            treeNode39.Name = "Node0";
            treeNode39.SelectedImageIndex = 2;
            treeNode39.Tag = "fp";
            treeNode39.Text = "IKC-E260T";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode39});
            this.treeView1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.treeView1.ShowNodeToolTips = true;
            this.treeView1.Size = new System.Drawing.Size(225, 213);
            this.treeView1.TabIndex = 0;


*/
