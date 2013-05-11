using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace components.Components.MMessageBox
{
    public partial class MMessageBox
    {
        // Private Variables
        private static Form form1 = new Form();
        private static System.Media.SystemSound sound;
        private static int selNoBtn = 1;
        private static MessageBoxIcon icon = MessageBoxIcon.None;
        private static MessageBoxButtons buttons = MessageBoxButtons.OK;
        private static string[] btnNames;
        private static int btnYLoc = 60;
        private static int leftMargin;
        private static int ButtonWidth = 75;
        private static int ButtonsSpacing = 6;

        public static Point StartPoint { get; set; }

        // Constructors
        public static DialogResult Show(string text)
        {
            form1.Dispose();
            form1 = new Form();
            InitializeComponent();

            if (form1.ParentForm == null)
                form1.StartPosition = FormStartPosition.CenterScreen;

            if (!StartPoint.IsEmpty)
            {
                form1.StartPosition = FormStartPosition.Manual;
                form1.Location = new Point(StartPoint.X > 0 ? StartPoint.X : form1.Location.X, StartPoint.Y > 0 ? StartPoint.Y : form1.Location.Y);
                StartPoint = new Point();
            }

            label1.Location = new Point(12, label1.Location.Y);

            btnNames = AsignButtons(buttons);

            label1.Text = text;
            FormAutoHeigh();

            CenterButtons(btnNames.Length);

            MakeButtons(btnNames, selNoBtn);
            AddSound(icon);

            DialogResult rez = form1.ShowDialog();
            form1.Dispose();
            return rez;
        }
        public static DialogResult Show(string text, string head)
        {
            form1.Dispose();
            form1 = new Form();
            InitializeComponent();

            if (form1.ParentForm == null)
                form1.StartPosition = FormStartPosition.CenterScreen;

            if (!StartPoint.IsEmpty)
            {
                form1.StartPosition = FormStartPosition.Manual;
                form1.Location = new Point(StartPoint.X > 0 ? StartPoint.X : form1.Location.X, StartPoint.Y > 0 ? StartPoint.Y : form1.Location.Y);
                StartPoint = new Point();
            }

            label1.Location = new Point(12, label1.Location.Y);

            btnNames = AsignButtons(buttons);

            form1.Text = head;
            label1.Text = text;
            FormAutoHeigh();

            CenterButtons(btnNames.Length);

            MakeButtons(btnNames, selNoBtn);
            AddSound(icon);

            DialogResult rez = form1.ShowDialog();
            form1.Dispose();
            return rez;
        }
        public static DialogResult Show(string text, string head, MessageBoxButtons buttons)
        {
            form1.Dispose();
            form1 = new Form();
            InitializeComponent();

            if (form1.ParentForm == null)
                form1.StartPosition = FormStartPosition.CenterScreen;

            label1.Location = new Point(12, label1.Location.Y);

            btnNames = AsignButtons(buttons);

            form1.Text = head;
            label1.Text = text;
            FormAutoHeigh();

            CenterButtons(btnNames.Length);

            MakeButtons(btnNames, selNoBtn);
            AddSound(icon);

            DialogResult rez = form1.ShowDialog();
            form1.Dispose();
            return rez;
        }
        public static DialogResult Show(string text, string head, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            form1.Dispose();
            form1 = new Form();
            InitializeComponent();

            if (form1.ParentForm == null)
                form1.StartPosition = FormStartPosition.CenterScreen;

            if (icon == MessageBoxIcon.None)
                label1.Location = new Point(12, label1.Location.Y);

            btnNames = AsignButtons(buttons);

            form1.Text = head;
            label1.Text = text;
            FormAutoHeigh();

            CenterButtons(btnNames.Length);

            MakeButtons(btnNames, selNoBtn);
            AddSound(icon);

            DialogResult rez = form1.ShowDialog();
            form1.Dispose();
            return rez;
        }
        public static DialogResult Show(string text, string head, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defBtn)
        {
            form1.Dispose();
            form1 = new Form();
            InitializeComponent();

            if (form1.ParentForm == null)
                form1.StartPosition = FormStartPosition.CenterScreen;

            if (icon == MessageBoxIcon.None)
                label1.Location = new Point(12, label1.Location.Y);

            btnNames = AsignButtons(buttons);


            form1.Text = head;
            label1.Text = text;
            FormAutoHeigh();

            CenterButtons(btnNames.Length);

            selNoBtn = int.Parse(defBtn.ToString()[defBtn.ToString().Length - 1].ToString());

            MakeButtons(btnNames, selNoBtn);
            AddSound(icon);

            DialogResult rez = form1.ShowDialog();
            form1.Dispose();
            return rez;
        }
        public static DialogResult Show(IWin32Window owner, string text)
        {
            form1.Dispose();
            form1 = new Form();
            InitializeComponent();

            if (form1.ParentForm == null)
                form1.StartPosition = FormStartPosition.CenterParent;

            label1.Location = new Point(12, label1.Location.Y);

            btnNames = AsignButtons(buttons);

            label1.Text = text;
            FormAutoHeigh();

            CenterButtons(btnNames.Length);

            MakeButtons(btnNames, selNoBtn);
            AddSound(icon);

            try
            {
                if (((Form)owner).WindowState == FormWindowState.Minimized)
                    ((Form)owner).WindowState = FormWindowState.Normal;
            }
            catch { }

            DialogResult rez = DialogResult.None;

            try
            {
                form1.Owner = (Form)owner;
                rez = form1.ShowDialog(owner);
            }
            catch (Exception ex)
            {
                // if owner is not a window then center dialog by control
                MMBeforeDisplayHook(ref form1, (Control)owner);
                form1.StartPosition = FormStartPosition.Manual;
                rez = form1.ShowDialog();
            }

            return rez;
        }
        public static DialogResult Show(IWin32Window owner, string text, string head)
        {
            form1.Dispose();
            form1 = new Form();
            InitializeComponent();

            if (form1.ParentForm == null)
                form1.StartPosition = FormStartPosition.CenterParent;

            label1.Location = new Point(12, label1.Location.Y);

            btnNames = AsignButtons(buttons);

            form1.Text = head;
            label1.Text = text;
            FormAutoHeigh();

            CenterButtons(btnNames.Length);

            MakeButtons(btnNames, selNoBtn);
            AddSound(icon);

            try
            {
                if (((Form)owner).WindowState == FormWindowState.Minimized)
                    ((Form)owner).WindowState = FormWindowState.Normal;
            }
            catch { }

            DialogResult rez = DialogResult.None;

            try
            {
                form1.Owner = (Form)owner;
                rez = form1.ShowDialog(owner);
            }
            catch (Exception ex)
            {
                // if owner is not a window then center dialog by control
                MMBeforeDisplayHook(ref form1, (Control)owner);
                form1.StartPosition = FormStartPosition.Manual;
                rez = form1.ShowDialog();
            }

            return rez;
        }
        public static DialogResult Show(IWin32Window owner, string text, string head, MessageBoxButtons buttons)
        {
            form1.Dispose();
            form1 = new Form();
            InitializeComponent();

            if (form1.ParentForm == null)
                form1.StartPosition = FormStartPosition.CenterParent;

            label1.Location = new Point(12, label1.Location.Y);

            btnNames = AsignButtons(buttons);

            form1.Text = head;
            label1.Text = text;
            FormAutoHeigh();

            CenterButtons(btnNames.Length);

            MakeButtons(btnNames, selNoBtn);
            AddSound(icon);

            try
            {
                if (((Form)owner).WindowState == FormWindowState.Minimized)
                    ((Form)owner).WindowState = FormWindowState.Normal;
            }
            catch { }

            DialogResult rez = DialogResult.None;

            try
            {
                form1.Owner = (Form)owner;
                rez = form1.ShowDialog(owner);
            }
            catch (Exception ex)
            {
                // if owner is not a window then center dialog by control
                MMBeforeDisplayHook(ref form1, (Control)owner);
                form1.StartPosition = FormStartPosition.Manual;
                rez = form1.ShowDialog();
            }

            return rez;
        }
        public static DialogResult Show(IWin32Window owner, string text, string head, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            form1.Dispose();
            form1 = new Form();
            InitializeComponent();

            if (form1.ParentForm == null)
                form1.StartPosition = FormStartPosition.CenterParent;

            if (icon == MessageBoxIcon.None)
                label1.Location = new Point(12, label1.Location.Y);

            btnNames = AsignButtons(buttons);

            form1.Text = head;
            label1.Text = text;
            FormAutoHeigh();

            CenterButtons(btnNames.Length);

            MakeButtons(btnNames, selNoBtn);
            AddSound(icon);

            try
            {
                if (((Form)owner).WindowState == FormWindowState.Minimized)
                    ((Form)owner).WindowState = FormWindowState.Normal;
            }
            catch { }

            DialogResult rez = DialogResult.None;
            
            try
            {
                form1.Owner = (Form)owner;
                rez = form1.ShowDialog(owner);
            }
            catch (Exception ex)
            {
                // if owner is not a window then center dialog by control
                MMBeforeDisplayHook(ref form1, (Control)owner);
                form1.StartPosition = FormStartPosition.Manual;
                rez = form1.ShowDialog();
            }

            return rez;
        }
        public static DialogResult Show(IWin32Window owner, string text, string head, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defBtn)
        {
            form1.Dispose();
            form1 = new Form();
            InitializeComponent();

            if (form1.ParentForm == null)
                form1.StartPosition = FormStartPosition.CenterParent;

            if (icon == MessageBoxIcon.None)
                label1.Location = new Point(12, label1.Location.Y);

            btnNames = AsignButtons(buttons);


            form1.Text = head;
            label1.Text = text;
            FormAutoHeigh();

            CenterButtons(btnNames.Length);

            selNoBtn = int.Parse(defBtn.ToString()[defBtn.ToString().Length - 1].ToString());

            MakeButtons(btnNames, selNoBtn);
            AddSound(icon);

            try
            {
                if (((Form)owner).WindowState == FormWindowState.Minimized)
                    ((Form)owner).WindowState = FormWindowState.Normal;
            }
            catch { }

            DialogResult rez = DialogResult.None;

            try
            {
                form1.Owner = (Form)owner;
                rez = form1.ShowDialog(owner);
            }
            catch (Exception ex)
            {
                // if owner is not a window then center dialog by control
                MMBeforeDisplayHook(ref form1, (Control)owner);
                form1.StartPosition = FormStartPosition.Manual;
                rez = form1.ShowDialog();
            }

            return rez;
        }

        #region ModalMessages
        /*
        // Methods
        public static void ShowModal(string text)
        {
            form1.Dispose();
            form1 = new Form();
            InitializeComponent();

            if (form1.ParentForm == null)
                form1.StartPosition = FormStartPosition.CenterScreen;

            label1.Location = new Point(12, label1.Location.Y);

            btnNames = AsignButtons(buttons);

            label1.Text = text;
            FormAutoHeigh();

            CenterButtons(btnNames.Length);

            MakeButtons(btnNames, selNoBtn);
            AddSound(icon);

            form1.Show();
        }
        public static void ShowModal(string text, string head)
        {
            form1.Dispose();
            form1 = new Form();
            InitializeComponent();

            if (form1.ParentForm == null)
                form1.StartPosition = FormStartPosition.CenterScreen;

            label1.Location = new Point(12, label1.Location.Y);

            btnNames = AsignButtons(buttons);

            form1.Text = head;
            label1.Text = text;
            FormAutoHeigh();

            CenterButtons(btnNames.Length);

            MakeButtons(btnNames, selNoBtn);
            AddSound(icon);

            form1.Show();
        }
        public static void ShowModal(string text, string head, MessageBoxButtons buttons)
        {
            form1.Dispose();
            form1 = new Form();
            InitializeComponent();

            if (form1.ParentForm == null)
                form1.StartPosition = FormStartPosition.CenterScreen;

            label1.Location = new Point(12, label1.Location.Y);

            btnNames = AsignButtons(buttons);

            form1.Text = head;
            label1.Text = text;
            FormAutoHeigh();

            CenterButtons(btnNames.Length);

            MakeButtons(btnNames, selNoBtn);
            AddSound(icon);

            form1.Show();
        }
        public static void ShowModal(string text, string head, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            form1.Dispose();
            form1 = new Form();
            InitializeComponent();

            if (form1.ParentForm == null)
                form1.StartPosition = FormStartPosition.CenterScreen;

            if (icon == MessageBoxIcon.None)
                label1.Location = new Point(12, label1.Location.Y);

            btnNames = AsignButtons(buttons);

            form1.Text = head;
            label1.Text = text;
            FormAutoHeigh();

            CenterButtons(btnNames.Length);

            MakeButtons(btnNames, selNoBtn);
            AddSound(icon);

            form1.Show();
        }
        public static void ShowModal(string text, string head, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defBtn)
        {
            form1.Dispose();
            form1 = new Form();
            InitializeComponent();

            if (form1.ParentForm == null)
                form1.StartPosition = FormStartPosition.CenterScreen;

            if (icon == MessageBoxIcon.None)
                label1.Location = new Point(12, label1.Location.Y);

            btnNames = AsignButtons(buttons);


            form1.Text = head;
            label1.Text = text;
            FormAutoHeigh();

            CenterButtons(btnNames.Length);

            selNoBtn = int.Parse(defBtn.ToString()[defBtn.ToString().Length - 1].ToString());

            MakeButtons(btnNames, selNoBtn);
            AddSound(icon);

            form1.Show();
        }
        public static void ShowModal(IWin32Window owner, string text)
        {
            form1.Dispose();
            form1 = new Form();
            InitializeComponent();

            if (form1.ParentForm == null)
                form1.StartPosition = FormStartPosition.CenterParent;

            label1.Location = new Point(12, label1.Location.Y);

            btnNames = AsignButtons(buttons);

            label1.Text = text;
            FormAutoHeigh();

            CenterButtons(btnNames.Length);

            MakeButtons(btnNames, selNoBtn);
            AddSound(icon);

            try
            {
                if (((Form)owner).WindowState == FormWindowState.Minimized)
                    ((Form)owner).WindowState = FormWindowState.Normal;
            }
            catch { }

            form1.Show(owner);
        }
        public static void ShowModal(IWin32Window owner, string text, string head)
        {
            form1.Dispose();
            form1 = new Form();
            InitializeComponent();

            if (form1.ParentForm == null)
                form1.StartPosition = FormStartPosition.CenterParent;

            label1.Location = new Point(12, label1.Location.Y);

            btnNames = AsignButtons(buttons);

            form1.Text = head;
            label1.Text = text;
            FormAutoHeigh();

            CenterButtons(btnNames.Length);

            MakeButtons(btnNames, selNoBtn);
            AddSound(icon);

            try
            {
                if (((Form)owner).WindowState == FormWindowState.Minimized)
                    ((Form)owner).WindowState = FormWindowState.Normal;
            }
            catch { }

            form1.Show(owner);
        }
        public static void ShowModal(IWin32Window owner, string text, string head, MessageBoxButtons buttons)
        {
            form1.Dispose();
            form1 = new Form();
            InitializeComponent();

            if (form1.ParentForm == null)
                form1.StartPosition = FormStartPosition.CenterParent;

            label1.Location = new Point(12, label1.Location.Y);

            btnNames = AsignButtons(buttons);

            form1.Text = head;
            label1.Text = text;
            FormAutoHeigh();

            CenterButtons(btnNames.Length);

            MakeButtons(btnNames, selNoBtn);
            AddSound(icon);

            try
            {
                if (((Form)owner).WindowState == FormWindowState.Minimized)
                    ((Form)owner).WindowState = FormWindowState.Normal;
            }
            catch { }

            form1.Show(owner);
        }
        public static void ShowModal(IWin32Window owner, string text, string head, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            form1.Dispose();
            form1 = new Form();
            InitializeComponent();

            if (form1.ParentForm == null)
                form1.StartPosition = FormStartPosition.CenterParent;

            if (icon == MessageBoxIcon.None)
                label1.Location = new Point(12, label1.Location.Y);

            btnNames = AsignButtons(buttons);

            form1.Text = head;
            label1.Text = text;
            FormAutoHeigh();

            CenterButtons(btnNames.Length);

            MakeButtons(btnNames, selNoBtn);
            AddSound(icon);

            try
            {
                if (((Form)owner).WindowState == FormWindowState.Minimized)
                    ((Form)owner).WindowState = FormWindowState.Normal;
            }
            catch { }

            form1.Show(owner);
        }
        public static void ShowModal(IWin32Window owner, string text, string head, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defBtn)
        {
            form1.Dispose();
            form1 = new Form();
            InitializeComponent();

            if (form1.ParentForm == null)
                form1.StartPosition = FormStartPosition.CenterParent;

            if (icon == MessageBoxIcon.None)
                label1.Location = new Point(12, label1.Location.Y);

            btnNames = AsignButtons(buttons);


            form1.Text = head;
            label1.Text = text;
            FormAutoHeigh();

            CenterButtons(btnNames.Length);

            selNoBtn = int.Parse(defBtn.ToString()[defBtn.ToString().Length - 1].ToString());

            MakeButtons(btnNames, selNoBtn);
            AddSound(icon);

            try
            {
                if (((Form)owner).WindowState == FormWindowState.Minimized)
                    ((Form)owner).WindowState = FormWindowState.Normal;
            }
            catch { }

            form1.Show(owner);
        }
        */
        #endregion

        // Events
        private static void MMessageBox_Load(object sender, EventArgs e)
        {
            if (form1.Owner != null)
            {
                Point oC = new Point(form1.Owner.Location.X + (form1.Owner.Width / 2), form1.Owner.Location.Y + (form1.Owner.Height / 2));
                Point tC = new Point((form1.Width / 2), (form1.Height / 2));
                Point loc = new Point(oC.X - tC.X, oC.Y - tC.Y);
                form1.Location = loc;
            }
            System.Media.SystemSound snd = sound;
            snd.Play();
        }
        private static void MMessageBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Enter).KeyValue)
            {
                return;
            }
            if (e.KeyValue == new KeyEventArgs(Keys.Space).KeyValue)
            {
                foreach (Control ctr in form1.Controls)
                {
                    if (ctr is Button && ctr.Focused)
                        button_mouseClick(ctr, EventArgs.Empty);
                }
            }
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                form1.Close();
        }
        private static void button_mouseClick(object sender, EventArgs e)
        {
            Button b = sender as Button;
            switch (b.Text)
            {
                case "OK":
                    {
                        form1.DialogResult = DialogResult.OK;
                        break;
                    }
                case "Так":
                    {
                        form1.DialogResult = DialogResult.Yes;
                        break;
                    }
                case "Ні":
                    {
                        form1.DialogResult = DialogResult.No;
                        break;
                    }
                case "Скасувати":
                    {
                        form1.DialogResult = DialogResult.Cancel;
                        break;
                    }
                case "Зупинити":
                    {
                        form1.DialogResult = DialogResult.Abort;
                        break;
                    }
                case "Ігнорувати":
                    {
                        form1.DialogResult = DialogResult.Ignore;
                        break;
                    }
                case "Повторити":
                    {
                        form1.DialogResult = DialogResult.Retry;
                        break;
                    }
            }
            form1.Close();
            form1.Dispose();
        }

        // Private Methods
        private static string[] AsignButtons(MessageBoxButtons buttons)
        {
            switch (buttons)
            {
                case MessageBoxButtons.AbortRetryIgnore:
                    {
                        return new string[] { "Зупинити", "Повторити", "Ігнорувати" };
                    }
                case MessageBoxButtons.OK:
                    {
                        return new string[] { "OK" };
                    }
                case MessageBoxButtons.OKCancel:
                    {
                        return new string[] { "OK", "Скасувати" };
                    }
                case MessageBoxButtons.RetryCancel:
                    {
                        return new string[] { "Повторити", "Скасувати" };
                    }
                case MessageBoxButtons.YesNo:
                    {
                        return new string[] { "Так", "Ні" };
                    }
                case MessageBoxButtons.YesNoCancel:
                    {
                        return new string[] { "Так", "Ні", "Скасувати" };
                    }
                default:
                    {
                        return new string[0];
                    }
            }
        }
        private static void CenterButtons(int n)
        {
            leftMargin = 12;

            //center buttons;
            int btnLength = n * ButtonWidth + ((n - 1) * ButtonsSpacing);

            if (form1.Width > (btnLength + 24 + 3 * 2))
            {
                leftMargin = (form1.Width - btnLength) / 2 - 3;
            }
            else
            {
                form1.Size = new Size(btnLength + 2 * leftMargin + 3 * 2, form1.Height);
                //3 - form's border size
            }
        }
        private static void FormAutoHeigh()
        {
            int nHeight = panel1.Height + 47 + 29 + 3;//47-default panel.height; 3-bottomBorder.Height; 29
            int nWidth = 3 + label1.Location.X + label1.Width + 12 + 3;

            form1.Size = new Size(nWidth, nHeight);

            if ((panel1.Height + 13) > 60)
            {
                //pictureBox1.Location = new Point(pictureBox1.Location.X, panel1.Height / 2 - pictureBox1.Height / 2);
                btnYLoc = panel1.Height + 13;
            }
            else
                btnYLoc = 60;
        }
        private static void MakeButtons(string[] names, int defB)
        {
            int j = 0;

            for (int i = 0; i < names.Length; i++)
            {
                Button btn = new Button();
                btn.Location = new System.Drawing.Point(leftMargin + i * ButtonWidth + ButtonsSpacing * j, btnYLoc);
                btn.Name = "button" + (i + 1);
                btn.Size = new System.Drawing.Size(ButtonWidth, 23);
                btn.TabIndex = i;
                btn.Text = names[i];
                btn.MouseClick += new MouseEventHandler(button_mouseClick);
                btn.UseVisualStyleBackColor = true;
                form1.Controls.Add(btn);
                j++;
                if (i == (defB - 1))
                    ((Button)form1.Controls["button" + (i + 1)]).Select();
            }

            form1.Update();
            // return btnLength;
        }
        private static void AddSound(MessageBoxIcon icon)
        {
            switch (icon.ToString())
            {
                case "Asterisk":
                    {
                        pictureBox1.Image = System.Drawing.SystemIcons.Asterisk.ToBitmap();
                        sound = System.Media.SystemSounds.Asterisk;
                        break;
                    }
                case "Error":
                    {
                        pictureBox1.Image = System.Drawing.SystemIcons.Error.ToBitmap();
                        sound = System.Media.SystemSounds.Beep;
                        break;
                    }
                case "Exclamation":
                    {
                        pictureBox1.Image = System.Drawing.SystemIcons.Exclamation.ToBitmap();
                        sound = System.Media.SystemSounds.Exclamation;
                        break;
                    }
                case "Hand":
                    {
                        pictureBox1.Image = System.Drawing.SystemIcons.Hand.ToBitmap();
                        sound = System.Media.SystemSounds.Exclamation;
                        break;
                    }
                case "Information":
                    {
                        pictureBox1.Image = System.Drawing.SystemIcons.Information.ToBitmap();
                        sound = System.Media.SystemSounds.Exclamation;
                        break;
                    }
                case "None":
                    {
                        sound = System.Media.SystemSounds.Asterisk;
                        break;
                    }
                case "Question":
                    {
                        pictureBox1.Image = System.Drawing.SystemIcons.Question.ToBitmap();
                        sound = System.Media.SystemSounds.Question;
                        break;
                    }
                case "Stop":
                    {
                        pictureBox1.Image = System.Drawing.SystemIcons.Error.ToBitmap();
                        sound = System.Media.SystemSounds.Hand;
                        break;
                    }
                case "Warning":
                    {
                        pictureBox1.Image = System.Drawing.SystemIcons.Warning.ToBitmap();
                        sound = System.Media.SystemSounds.Asterisk;
                        break;
                    }
            }
        }

        // hooks
        public static void MMBeforeDisplayHook(ref Form msg, Control owner)
        {
            try
            {
                IntPtr hChildWnd = msg.Handle;

                Rectangle recChild = new Rectangle(0, 0, 0, 0);
                bool success = GetWindowRect(hChildWnd, ref recChild);

                int width = recChild.Width - recChild.X;
                int height = recChild.Height - recChild.Y;

                Rectangle recParent = new Rectangle(0, 0, 0, 0);
                success = GetWindowRect(owner.Handle, ref recParent);

                Point ptCenter = new Point(0, 0);
                ptCenter.X = recParent.X + ((recParent.Width - recParent.X) / 2);
                ptCenter.Y = recParent.Y + ((recParent.Height - recParent.Y) / 2);


                Point ptStart = new Point(0, 0);
                ptStart.X = (ptCenter.X - (width / 2));
                ptStart.Y = (ptCenter.Y - (height / 2));

                ptStart.X = (ptStart.X < 0) ? 0 : ptStart.X;
                ptStart.Y = (ptStart.Y < 0) ? 0 : ptStart.Y;

                msg.Location = ptStart;

            }
            catch { }
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, ref Rectangle lpRect);

    }
}