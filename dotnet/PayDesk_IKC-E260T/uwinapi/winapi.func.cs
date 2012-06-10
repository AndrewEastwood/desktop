using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using winapi.Components;

namespace winapi
{
    // Functions
    public partial class WApi
    {
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 GetShortPathName(String lpszLongPath, StringBuilder lpszShortPath, Int32 shortPathLength);
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern Int32 SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern Boolean RegisterHotKey(IntPtr hWnd, Int32 id, Int32 fsModifiers, Int32 vlc);
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern Boolean UnregisterHotKey(IntPtr hWnd, Int32 id);
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern Boolean PostMessage(IntPtr HWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        
        [DllImport("shlwapi.dll", SetLastError = true)]
        public static extern Boolean PathFileExists(String pszPath);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern UInt32 GetFileAttributes(String lpFileName);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFile(String lpFileName, UInt32 dwDesiredAccess,
            UInt32 dwShareMode, IntPtr lpSecurityAttributes,
            UInt32 dwCreationDisposion, UInt32 dwFlagsAndAttributes, IntPtr hTemplateFile);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean GetFileTime(IntPtr hFile, winapi.Components.FILETIME lpCreationTime,
            out winapi.Components.FILETIME lpLastAccessTime, out winapi.Components.FILETIME lpLastWriteTime);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern UInt32 GetLastError();
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Int64 CompareFileTime(ref winapi.Components.FILETIME lpFileTime1, 
            ref winapi.Components.FILETIME lpFileTime2);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean SetFileTime(IntPtr hFile, ref winapi.Components.FILETIME lpCreationTime,
            ref winapi.Components.FILETIME lpLastAccessTime, ref winapi.Components.FILETIME lpLastWriteTime);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean FileTimeToSystemTime(ref winapi.Components.FILETIME lpFileTime, 
            out SYSTEMTIME lpSystemTime);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean FileTimeToLocalFileTime(ref winapi.Components.FILETIME lpFileTime, 
            out winapi.Components.FILETIME lpLocalFileTime);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean CopyFile(String lpExistingFileName, String lpNewFileName, Boolean bFailIfExists);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean WriteFile(IntPtr fFile, Byte[] lpBuffer, UInt32 nNumberOfBytesToWrite, 
            out UInt32 lpNumberOfBytesWritten, ref OVERLAPPED lpOverlapped);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean ReadFile(IntPtr hFile, Byte[] lpBuffer, 
            UInt32 nNumberOfBytesToRead, out UInt32 nNumberOfBytesRead, ref OVERLAPPED lpOverlapped);
        
        [DllImport("Kernel32.dll")]
        public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, 
            Boolean bManualReset, Boolean bInitialState, String lpName);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean WaitCommEvent(IntPtr hFile, ref Int32 lpEvtMask, 
            ref OVERLAPPED lpOverlapped);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean ClearCommError(IntPtr hFile, ref Int32 lpErrors, 
            ref COMMSTAT lpCommStat);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean GetOverlappedResult(IntPtr hFile, ref OVERLAPPED lpOverlapped, 
            ref UInt32 nNumberOfBytesTransferred, Boolean bWait);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean CloseHandle(IntPtr hObject);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean CancelIo(IntPtr hFile);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean SetCommMask(IntPtr hFile, UInt32 dwEvtMask);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean GetCommState(IntPtr hFile, ref DCB lpDCB);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean GetCommTimeouts(IntPtr hFile, out COMMTIMEOUTS lpCommTimeouts);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean BuildCommDCBAndTimeouts(String lpDef, ref DCB lpDCB, 
            ref COMMTIMEOUTS lpCommTimeouts);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean SetCommState(IntPtr hFile, [In] ref DCB lpDCB);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean SetCommTimeouts(IntPtr hFile, [In] ref COMMTIMEOUTS lpCommTimeouts);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean SetupComm(IntPtr hFile, UInt32 dwInQueue, UInt32 dwOutQueue);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean PurgeComm(IntPtr hFile, UInt32 dwFlags);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean SetCommMask(IntPtr hFile, UInt16 dwEvtMask);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern void OutputDebugString(String lpOutputString);


        [DllImport("user32")]
        public static extern int GetDesktopWindow();

        [DllImport("user32")]
        public static extern int GetWindow(int hwnd, int wCmd);

        [DllImport("user32")]
        public static extern int IsWindowVisible(int hwnd);


        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(
            int hWnd // handle to window
            );

        [DllImport("User32.Dll")]
        public static extern void GetWindowText(int h, StringBuilder s, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern int FindWindow(
            string lpClassName, // class name 
            string lpWindowName // window name 
        );


        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, ref System.Drawing.Rectangle rect);

        // custom

        public static Dictionary<int, string> GetTaskWindows(/*IntPtr handle*/)
        {
            Dictionary<int, string> windowsV = new Dictionary<int, string>();

            // Get the desktopwindow handle
            int nDeshWndHandle = GetDesktopWindow();
            // Get the first child window
            int nChildHandle = GetWindow(nDeshWndHandle, GW_CHILD);

            windowsV.Clear();
            int idx = 0;

            while (nChildHandle != 0)
            {
                //If the child window is this (SendKeys) application then ignore it.
                /*if (nChildHandle == handle.ToInt32())
                {
                    nChildHandle = GetWindow(nChildHandle, GW_HWNDNEXT);
                }*/

                // Get only visible windows
                if (IsWindowVisible(nChildHandle) != 0)
                {
                    StringBuilder sbTitle = new StringBuilder(1024);
                    // Read the Title bar text on the windows to put in combobox
                    GetWindowText(nChildHandle, sbTitle, sbTitle.Capacity);
                    String sWinTitle = sbTitle.ToString();
                    {
                        if (sWinTitle.Length > 0)
                        {
                            windowsV.Add(idx, sWinTitle);
                            idx++;
                        }
                    }
                }
                // Look for the next child.
                nChildHandle = GetWindow(nChildHandle, GW_HWNDNEXT);
            }

            return windowsV;
        }
    }
}
