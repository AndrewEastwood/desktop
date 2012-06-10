using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace winapi
{
    // Functions
    public partial class API
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
        public static extern Boolean GetFileTime(IntPtr hFile, FILETIME lpCreationTime,
            out FILETIME lpLastAccessTime, out FILETIME lpLastWriteTime);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern UInt32 GetLastError();
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Int64 CompareFileTime(ref FILETIME lpFileTime1, 
            ref FILETIME lpFileTime2);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean SetFileTime(IntPtr hFile, ref FILETIME lpCreationTime,
            ref FILETIME lpLastAccessTime, ref FILETIME lpLastWriteTime);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean FileTimeToSystemTime(ref FILETIME lpFileTime, 
            out SYSTEMTIME lpSystemTime);
        
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean FileTimeToLocalFileTime(ref FILETIME lpFileTime, 
            out FILETIME lpLocalFileTime);
        
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
        public static extern void OutputDebugString(String lpOutputString);
    }
}
