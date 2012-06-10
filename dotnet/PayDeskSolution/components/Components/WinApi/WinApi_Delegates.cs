using System;
using System.Collections.Generic;
using System.Text;

namespace components.Components.WinApi
{
    static partial class Com_WinApi
    {
        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        public delegate void TimerProc(IntPtr hWnd, uint uMsg, UIntPtr nIDEvent, uint dwTime);
    }
}
