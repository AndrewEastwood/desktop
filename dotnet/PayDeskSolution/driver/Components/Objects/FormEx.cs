using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace driver.Components.Objects
{
    public partial class FormEx : Form
    {

        private Control _ownerControl;

        public FormEx()
        {
            InitializeComponent();
        }

        public DialogResult ShowDialogEx()
        {
            return ShowDialogEx(_ownerControl);
        }

        public DialogResult ShowDialogEx(Control owner)
        {
            DialogResult dlgRez = System.Windows.Forms.DialogResult.None;
            try
            {
                CenterWindow(owner.Handle);
                this.StartPosition = FormStartPosition.Manual;
            }
            catch { }

            dlgRez = this.ShowDialog();

            return dlgRez;
        }


        // hooks

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                CenterWindow(_ownerControl.Handle);
                this.StartPosition = FormStartPosition.Manual;
            }
            catch { }
        }

        public Control OwnerControlEx { get { return _ownerControl; } set { _ownerControl = value; } }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, ref Rectangle lpRect);

        private void CenterWindow(IntPtr _owner)
        {
            Rectangle recChild = new Rectangle(0, 0, 0, 0);
            bool success = GetWindowRect(this.Handle, ref recChild);

            int width = recChild.Width - recChild.X;
            int height = recChild.Height - recChild.Y;

            Rectangle recParent = new Rectangle(0, 0, 0, 0);
            success = GetWindowRect(_owner, ref recParent);

            Point ptCenter = new Point(0, 0);
            ptCenter.X = recParent.X + ((recParent.Width - recParent.X) / 2);
            ptCenter.Y = recParent.Y + ((recParent.Height - recParent.Y) / 2);


            Point ptStart = new Point(0, 0);
            ptStart.X = (ptCenter.X - (width / 2));
            ptStart.Y = (ptCenter.Y - (height / 2));

            ptStart.X = (ptStart.X < 0) ? 0 : ptStart.X;
            ptStart.Y = (ptStart.Y < 0) ? 0 : ptStart.Y;

            this.Location = ptStart;
        }
    }
}
