using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using components.Shared.Structures;

namespace components.Components.WinApi
{
    // Functions
    public partial class Com_WinApi
    {
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 GetShortPathName(String lpszLongPath, StringBuilder lpszShortPath, Int32 shortPathLength);
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern Int32 SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool InSendMessage();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ReplyMessage(IntPtr lResult);

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
        public static extern IntPtr CreateFileA(String lpFileName, UInt32 dwDesiredAccess,
            UInt32 dwShareMode, IntPtr lpSecurityAttributes,
            UInt32 dwCreationDisposion, UInt32 dwFlagsAndAttributes, IntPtr hTemplateFile);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean GetFileTime(IntPtr hFile, components.Shared.Structures.FILETIME lpCreationTime,
            out components.Shared.Structures.FILETIME lpLastAccessTime, out components.Shared.Structures.FILETIME lpLastWriteTime);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern UInt32 GetLastError();
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Int64 CompareFileTime(ref components.Shared.Structures.FILETIME lpFileTime1,
            ref components.Shared.Structures.FILETIME lpFileTime2);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean SetFileTime(IntPtr hFile, ref components.Shared.Structures.FILETIME lpCreationTime,
            ref components.Shared.Structures.FILETIME lpLastAccessTime, ref components.Shared.Structures.FILETIME lpLastWriteTime);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean FileTimeToSystemTime(ref components.Shared.Structures.FILETIME lpFileTime, 
            out SYSTEMTIME lpSystemTime);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean FileTimeToLocalFileTime(ref components.Shared.Structures.FILETIME lpFileTime,
            out components.Shared.Structures.FILETIME lpLocalFileTime);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean CopyFile(String lpExistingFileName, String lpNewFileName, Boolean bFailIfExists);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean WriteFile(IntPtr fFile, Byte[] lpBuffer, UInt32 nNumberOfBytesToWrite, 
            out UInt32 lpNumberOfBytesWritten, ref OVERLAPPED lpOverlapped);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean ReadFile(IntPtr hFile, Byte[] lpBuffer, 
            UInt32 nNumberOfBytesToRead, out UInt32 nNumberOfBytesRead, ref OVERLAPPED lpOverlapped);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateEventA(IntPtr lpEventAttributes, 
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
        public static extern void OutputDebugString(String lpOutputString);

        // GUI
        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(int hWnd /* handle to window */);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, ref System.Drawing.Rectangle lpRect);

        [DllImport("user32.dll")]
        public static extern int MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("User32.dll")]
        public static extern UIntPtr SetTimer(IntPtr hWnd, UIntPtr nIDEvent, uint uElapse, TimerProc lpTimerFunc);

        [DllImport("User32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll")]
        public static extern int UnhookWindowsHookEx(IntPtr idHook);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int maxLength);

        [DllImport("user32.dll")]
        public static extern int EndDialog(IntPtr hDlg, IntPtr nResult);

        // E:GUI
    }
}
