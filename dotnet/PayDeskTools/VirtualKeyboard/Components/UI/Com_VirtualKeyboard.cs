using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using VirtualKeyboard.Lib;
using System.Diagnostics;
using System.Collections;

namespace VirtualKeyboard.Components.UI.Other
{
    public partial class Com_VirtualKeyboard : Form
    {
        private Config.manager mgr;
        private int selectedWindowIndex;

        public Com_VirtualKeyboard()
        {
            InitializeComponent();
            /* restoring window position */
            mgr = new Config.manager();
            mgr.Read();
            //this.Width = mgr.Settings.windowWidth;
            //this.Height = mgr.Settings.windowHeight;
            this.Location = new Point(mgr.Settings.windowLocX, mgr.Settings.windowLocY);
            this.Font = new Font(this.Font.FontFamily, mgr.Settings.windowFontSize);
            // window index
            // for Vista, Win7 and newer use 1
            // for older OS use 0
            selectedWindowIndex = 1;
        }

        public Com_VirtualKeyboard(string ownerName)
            : this()
        {
            /*
            if (ownerName != string.Empty)
                this.Text += " - " + ownerName;
            */
        }
        public Com_VirtualKeyboard(int selectedWindowIndex)
            : this()
        {
            // autodetect
            if (selectedWindowIndex == -1)
            {
                string result = string.Empty;
                System.Management.ManagementObjectSearcher searcher = new System.Management.ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
                foreach (System.Management.ManagementObject os in searcher.Get())
                {
                    result = os["Caption"].ToString();
                    break;
                }
                WindowsAPI.OutputDebugString("detected " + result);

                if (result.ToLower().Contains("windows vista"))
                    this.selectedWindowIndex = 1;
                if (result.ToLower().Contains("windows 7"))
                    this.selectedWindowIndex = 1;
                if (result.ToLower().Contains("windows xp"))
                    this.selectedWindowIndex = 0;
                if (result.ToLower().Contains("windows 8"))
                    this.selectedWindowIndex = 0;
                WindowsAPI.OutputDebugString("applied window index selector = " + this.selectedWindowIndex);
            }
            else
                this.selectedWindowIndex = selectedWindowIndex;
        }

        private void Com_VirtualKeyboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            mgr.Settings.windowWidth = this.Width;
            mgr.Settings.windowHeight = this.Height;
            mgr.Settings.windowLocX = this.Location.X;
            mgr.Settings.windowLocY = this.Location.Y;
            mgr.Settings.windowFontSize = this.Font.Size;
            mgr.Save();
        }
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("User32.Dll", EntryPoint = "PostMessageA")]
        static extern bool PostMessage(
            IntPtr hWnd,
            uint msg,
            IntPtr wParam,
            IntPtr lParam
            );

        [DllImport("user32.dll", SetLastError = true)]
        public static extern Int32 SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);


        [DllImport("user32.dll")]
        public static extern bool GetMessage(ref Message lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);



        enum GetWindow_Cmd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }

        const uint WM_KEYDOWN = 0x100;

        public enum MyHotKeys : int
        {
            HK_CtrlDel = 0x10,
            HK_CtrlShiftDel = 0x11,
            HK_CtrlPgDn = 0x12,
            HK_CtrlPgUp = 0x13,
            HK_ShiftDel = 0x14,
            HK_Enter = 0x15,
            HK_CtrlEnter = 0x16,
            HK_CtrlShiftEnter = 0x17,
            HK_F5 = 0x18,
            HK_F6 = 0x19,
            HK_F7 = 0x1A,
            HK_F8 = 0x1B,
            HK_F9 = 0x1C,
            HK_Esc = 0x1D,
            HK_CtrlQ = 0x1E,
            HK_Ctrl = 0x1F
        }
        /// <summary>
        /// Набір повідомлень для виконання певних операцій
        /// </summary>
        public enum MyMsgs : int
        {
            WM_HOTKEY = 0x312,
            WM_UPDATE = 0x456,
            WM_ENDUPDATE = 0x435,
            WM_KEYDOWN = 0x100,
            WM_KEYUP = 0x101,
            WM_CHAR = 0x105, 
            WM_SYSKEYDOWN = 0x104,
            WM_SYSKEYUP = 0x105
        }

        #region Virtual Keys Constants
        private const int VK_LBUTTON = 0X1; //Left mouse button.
        private const int VK_RBUTTON = 0X2; //Right mouse button.
        private const int VK_CANCEL = 0X3; //Used for control-break processing.
        private const int VK_MBUTTON = 0X4; //''Middle mouse button (3-button mouse).
        private const int KEYEVENTF_KEYUP = 0X2; // Release key
        private const int VK_OEM_PERIOD = 0XBE; //.
        private const int KEYEVENTF_EXTENDEDKEY = 0X1;
        private const int VK_STARTKEY = 0X5B; //Start Menu key
        private bool lockbool;
        private bool numlockbool;
        private bool ctrlbool;
        private bool onoff;
        private const int VK_OEM_COMMA = 0XBC; //, comma
        public const int VK_0 = 0x30;
        public const int VK_1 = 0x31;
        public const int VK_2 = 0x32;
        public const int VK_3 = 0x33;
        public const int VK_4 = 0x34;
        public const int VK_5 = 0x35;
        public const int VK_6 = 0x36;
        public const int VK_7 = 0x37;
        public const int VK_8 = 0x38;
        public const int VK_9 = 0x39;
        public const int VK_A = 0x41;
        public const int VK_B = 0x42;
        public const int VK_C = 0x43;
        public const int VK_D = 0x44;
        public const int VK_E = 0x45;
        public const int VK_F = 0x46;
        public const int VK_G = 0x47;
        public const int VK_H = 0x48;
        public const int VK_I = 0x49;
        public const int VK_J = 0x4A;
        public const int VK_K = 0x4B;
        public const int VK_L = 0x4C;
        public const int VK_M = 0x4D;
        public const int VK_N = 0x4E;
        public const int VK_O = 0x4F;
        public const int VK_P = 0x50;
        public const int VK_Q = 0x51;
        public const int VK_R = 0x52;
        public const int VK_S = 0x53;
        public const int VK_T = 0x54;
        public const int VK_U = 0x55;
        public const int VK_V = 0x56;
        public const int VK_W = 0x57;
        public const int VK_X = 0x58;
        public const int VK_Y = 0x59;
        public const int VK_Z = 0x5A;

        public const int VK_BACK = 0x08;
        public const int VK_TAB = 0x09;
        public const int VK_CLEAR = 0x0C;
        public const int VK_RETURN = 0x0D;
        public const int VK_SHIFT = 0x10;
        public const int VK_CONTROL = 0x11;
        public const int VK_MENU = 0x12;
        public const int VK_PAUSE = 0x13;
        public const int VK_CAPITAL = 0x14;
        public const int VK_KANA = 0x15;
        public const int VK_HANGEUL = 0x15;
        public const int VK_HANGUL = 0x15;
        public const int VK_JUNJA = 0x17;
        public const int VK_FINAL = 0x18;
        public const int VK_HANJA = 0x19;
        public const int VK_KANJI = 0x19;
        public const int VK_ESCAPE = 0x1B;
        public const int VK_CONVERT = 0x1C;
        public const int VK_NONCONVERT = 0x1D;
        public const int VK_ACCEPT = 0x1E;
        public const int VK_MODECHANGE = 0x1F;
        public const int VK_SPACE = 0x20;
        public const int VK_PRIOR = 0x21;
        public const int VK_NEXT = 0x22;
        public const int VK_END = 0x23;
        public const int VK_HOME = 0x24;
        public const int VK_LEFT = 0x25;
        public const int VK_UP = 0x26;
        public const int VK_RIGHT = 0x27;
        public const int VK_DOWN = 0x28;
        public const int VK_SELECT = 0x29;
        public const int VK_PRINT = 0x2A;
        public const int VK_EXECUTE = 0x2B;
        public const int VK_SNAPSHOT = 0x2C;
        public const int VK_INSERT = 0x2D;
        public const int VK_DELETE = 0x2E;
        public const int VK_HELP = 0x2F;
        public const int VK_LWIN = 0x5B;
        public const int VK_RWIN = 0x5C;
        public const int VK_APPS = 0x5D;
        public const int VK_SLEEP = 0x5F;
        public const int VK_NUMPAD0 = 0x60;
        public const int VK_NUMPAD1 = 0x61;
        public const int VK_NUMPAD2 = 0x62;
        public const int VK_NUMPAD3 = 0x63;
        public const int VK_NUMPAD4 = 0x64;
        public const int VK_NUMPAD5 = 0x65;
        public const int VK_NUMPAD6 = 0x66;
        public const int VK_NUMPAD7 = 0x67;
        public const int VK_NUMPAD8 = 0x68;
        public const int VK_NUMPAD9 = 0x69;
        public const int VK_MULTIPLY = 0x6A;
        public const int VK_ADD = 0x6B;
        public const int VK_SEPARATOR = 0x6C;
        public const int VK_SUBTRACT = 0x6D;
        public const int VK_DECIMAL = 0x6E;
        public const int VK_DIVIDE = 0x6F;
        public const int VK_F1 = 0x70;
        public const int VK_F2 = 0x71;
        public const int VK_F3 = 0x72;
        public const int VK_F4 = 0x73;
        public const int VK_F5 = 0x74;
        public const int VK_F6 = 0x75;
        public const int VK_F7 = 0x76;
        public const int VK_F8 = 0x77;
        public const int VK_F9 = 0x78;
        public const int VK_F10 = 0x79;
        public const int VK_F11 = 0x7A;
        public const int VK_F12 = 0x7B;
        public const int VK_F13 = 0x7C;
        public const int VK_F14 = 0x7D;
        public const int VK_F15 = 0x7E;
        public const int VK_F16 = 0x7F;
        public const int VK_F17 = 0x80;
        public const int VK_F18 = 0x81;
        public const int VK_F19 = 0x82;
        public const int VK_F20 = 0x83;
        public const int VK_F21 = 0x84;
        public const int VK_F22 = 0x85;
        public const int VK_F23 = 0x86;
        public const int VK_F24 = 0x87;
        public const int VK_NUMLOCK = 0x90;
        public const int VK_SCROLL = 0x91;

        #endregion Virtual Keys Constants

        IntPtr lastHandle = IntPtr.Zero;
        Dictionary<int, string> windowsV = new Dictionary<int, string>();

        private Hashtable useHotKeys = new Hashtable() { 
            {"{ENTER}", 0x15 },
            {"{ESC}", 0x1D },
            {"{F5}", 0x18 },
            {"{F6}", 0x19 },
            {"{F7}", 0x1A },
            {"{F8}", 0x1B },
            {"{F9}", 0x1C }
        };


        private Hashtable replaceHotKeys = new Hashtable() { 
            {",", 0x2C }
        };

        private void button_Click(object sender, EventArgs e)
        {
            GetTaskWindows();
            int iHandle = NativeWin32.FindWindow(null, windowsV[this.selectedWindowIndex]);

            NativeWin32.SetForegroundWindow(iHandle);

            Button btn = (Button)sender;

            System.Threading.Thread.Sleep(Program.SleepTime);
            System.Windows.Forms.SendKeys.Flush();
            System.Windows.Forms.SendKeys.SendWait(btn.Tag.ToString());

            //NativeWin32.PostMessage(new IntPtr(iHandle), NativeWin32.WM_KEYDOWN, (int)'F', 0);
            try
            {
                if (useHotKeys[btn.Tag] != null)
                {
                    SendMessage(new IntPtr(iHandle), (uint)MyMsgs.WM_HOTKEY, new IntPtr(int.Parse(useHotKeys[btn.Tag].ToString())), new IntPtr(0));
                    /*Message msg = new Message();
                    GetMessage(ref msg, new IntPtr(iHandle), 0, 0);
                    if (msg.Result.ToInt32() == 356)
                        return;*/
                }
            }
            catch { }
            /* {*/
            /*
          if (btn.Tag.ToString() == ",")
          {
              PostMessage(new IntPtr(iHandle), (uint)MyMsgs.WM_KEYDOWN, new IntPtr((int)Keys.S), new IntPtr(0));
              PostMessage(new IntPtr(iHandle), (uint)MyMsgs.WM_KEYUP, new IntPtr((int)Keys.S), new IntPtr(0));
          }
          else
              System.Windows.Forms.SendKeys.SendWait(btn.Tag.ToString());

      }
      else*/
            //this.BringToFront();
        }


        /// <summary>
        /// Get all the top level visible windows
        /// </summary>
        private void GetTaskWindows()
        {
            // Get the desktopwindow handle
            int nDeshWndHandle = NativeWin32.GetDesktopWindow();
            // Get the first child window
            int nChildHandle = NativeWin32.GetWindow(nDeshWndHandle, NativeWin32.GW_CHILD);

            windowsV.Clear();
            int idx = 0;

            while (nChildHandle != 0)
            {
                //If the child window is this (SendKeys) application then ignore it.
                if (nChildHandle == this.Handle.ToInt32())
                {
                    nChildHandle = NativeWin32.GetWindow(nChildHandle, NativeWin32.GW_HWNDNEXT);
                }

                // Get only visible windows
                if (NativeWin32.IsWindowVisible(nChildHandle) != 0)
                {
                    StringBuilder sbTitle = new StringBuilder(1024);
                    // Read the Title bar text on the windows to put in combobox
                    NativeWin32.GetWindowText(nChildHandle, sbTitle, sbTitle.Capacity);
                    String sWinTitle = sbTitle.ToString();
                    {
                        if (sWinTitle.Length > 0)
                        {
                            windowsV.Add(idx, sWinTitle);
                            WindowsAPI.OutputDebugString(idx + " " + sWinTitle);
                            idx++;
                        }
                    }
                }
                // Look for the next child.
                nChildHandle = NativeWin32.GetWindow(nChildHandle, NativeWin32.GW_HWNDNEXT);
            }
        }

        public static void PressKey(char ch)
        {
            byte vk = WindowsAPI.VkKeyScan(ch);
            ushort scanCode = (ushort)WindowsAPI.MapVirtualKey(vk, 0);
            KeyDown(scanCode);
            KeyUp(scanCode);
        }
        public static void PressKey(char ch, bool press)
        {
            byte vk = WindowsAPI.VkKeyScan(ch);
            ushort scanCode = (ushort)WindowsAPI.MapVirtualKey(vk, 0);

            if (press)
                KeyDown(scanCode);
            else
                KeyUp(scanCode);
        }

        public static void KeyDown(ushort scanCode)
        {
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = WindowsAPI.INPUT_KEYBOARD;
            inputs[0].ki.dwFlags = 0;
            inputs[0].ki.wScan = (ushort)(scanCode & 0xff);

            uint intReturn = WindowsAPI.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send key: " + scanCode);
            }
        }

        public static void KeyUp(ushort scanCode)
        {
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = WindowsAPI.INPUT_KEYBOARD;
            inputs[0].ki.wScan = scanCode;
            inputs[0].ki.dwFlags = WindowsAPI.KEYEVENTF_KEYUP;
            uint intReturn = WindowsAPI.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send key: " + scanCode);
            }
        }

        private void button_size_Click(object sender, EventArgs e)
        {
            Button sBtn = (Button)sender;
            if (sBtn.Tag.ToString() == "++")
                this.Font = new Font(this.Font.FontFamily, this.Font.Size + 1);
            else
                this.Font = new Font(this.Font.FontFamily, this.Font.Size - 1);

            button_size_more.Font = new Font(this.Font.FontFamily, 8.25F);
            button_size_less.Font = new Font(this.Font.FontFamily, 8.25F);
        }



    }
}
