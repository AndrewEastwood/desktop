using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace mdcore
{
    public static class WinAPI
    {
        //Constants for errors:
        public readonly static UInt32 ERROR_FILE_NOT_FOUND = 2;
        public readonly static UInt32 ERROR_INVALID_NAME = 123;
        public readonly static UInt32 ERROR_ACCESS_DENIED = 5;
        public readonly static UInt32 ERROR_INVALID_HANDLE = 6;
        public readonly static UInt32 ERROR_IO_PENDING = 997;
        public readonly static UInt32 ERROR_HANDLE_EOF = 38;

        public readonly static UInt32 WAIT_OBJECT_0 = 0x00000000;
        public readonly static UInt32 WAIT_ABANDONED = 0x00000080;
        public readonly static UInt32 WAIT_ABANDONED_0 = 0x00000080;
        //Constants for return value:
        public readonly static Int32 INVALID_HANDLE_VALUE = -1;

        //Constants for dwFlagsAndAttributes:
        public readonly static UInt32 FILE_FLAG_OVERLAPPED = 0x40000000;

        //Constants for dwCreationDisposition:
        public readonly static UInt32 OPEN_EXISTING = 3;

        //Constants for dwDesiredAccess:
        public readonly static UInt32 GENERIC_READ = 0x80000000;
        public readonly static UInt32 GENERIC_WRITE = 0x40000000;

        // Constants for dwEvtMask:
        public readonly static UInt32 EV_RXCHAR = 0x0001;
        public readonly static UInt32 EV_RXFLAG = 0x0002;
        public readonly static UInt32 EV_TXEMPTY = 0x0004;
        public readonly static Int32 EV_CTS = 0x0008;
        public readonly static UInt32 EV_DSR = 0x0010;
        public readonly static UInt32 EV_RLSD = 0x0020;
        public readonly static UInt32 EV_BREAK = 0x0040;
        public readonly static UInt32 EV_ERR = 0x0080;
        public readonly static UInt32 EV_RING = 0x0100;
        public readonly static UInt32 EV_PERR = 0x0200;
        public readonly static UInt32 EV_RX80FULL = 0x0400;
        public readonly static UInt32 EV_EVENT1 = 0x0800;
        public readonly static UInt32 EV_EVENT2 = 0x1000;
        // Added to enable use of "return immediately" timeout.
        public readonly static UInt32 MAXDWORD = 0xffffffff;
        //Purge
        public readonly static UInt16 PURGE_TXABORT = 0x0001; // Kill the pending/current writes to the comm port.
        public readonly static UInt16 PURGE_RXABORT = 0x0002; // Kill the pending/current reads to the comm port.
        public readonly static UInt16 PURGE_TXCLEAR = 0x0004; // Kill the transmit queue if there.
        public readonly static UInt16 PURGE_RXCLEAR = 0x0008; // Kill the typeahead buffer if there.

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetShortPathName([MarshalAs(UnmanagedType.LPTStr)]string lpszLongPath, [MarshalAs(UnmanagedType.LPTStr)]StringBuilder lpszShortPath, int shortPathLength);
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr HWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("shlwapi.dll")]
        public static extern bool PathFileExists(string pszPath);
        [DllImport("Kernel32.dll")]
        public static extern IntPtr CreateFile(string lpFileName, dwDesiredAccess dwDesiredAccess, dwShareMode dwShareMode, int lpSecurityAttributes,
            dwCreationDisposion dwCreationDisposion, int dwFlagsAndAttributes, IntPtr hTemplateFile);
        [DllImport("Kernel32.dll")]
        public static extern bool GetFileTime(IntPtr hFile, out _FILETIME lpCreationTime, out _FILETIME lpLastAccessTime, out _FILETIME lpLastWriteTime);
        [DllImport("Kernel32.dll")]
        public static extern long CompareFileTime(ref _FILETIME lpFileTime1, ref _FILETIME lpFileTime2);
        [DllImport("Kernel32.dll")]
        public static extern bool SetFileTime(IntPtr hFile, ref _FILETIME lpCreationTime, ref _FILETIME lpLastAccessTime, ref _FILETIME lpLastWriteTime);
        [DllImport("Kernel32.dll")]
        public static extern bool FileTimeToSystemTime(ref _FILETIME lpFileTime, out _SYSTEMTIME lpSystemTime);
        [DllImport("Kernel32.dll")]
        public static extern bool FileTimeToLocalFileTime(ref _FILETIME lpFileTime, out _FILETIME lpLocalFileTime);
        [DllImport("Kernel32.dll")]
        public static extern bool CopyFile(string lpExistingFileName, string lpNewFileName, bool bFailIfExists);
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFile(String lpFileName, UInt32 dwDesiredAccess, UInt32 dwShareMode, IntPtr lpSecurityAttributes, UInt32 dwCreationDisposition, UInt32 dwFlagsAndAttributes, IntPtr hTemplateFile);
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean WriteFile(IntPtr fFile, Byte[] lpBuffer, UInt32 nNumberOfBytesToWrite, out UInt32 lpNumberOfBytesWritten, ref OVERLAPPED lpOverlapped);
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean ReadFile(IntPtr hFile, Byte[] lpBuffer, UInt32 nNumberOfBytesToRead, out UInt32 nNumberOfBytesRead, ref OVERLAPPED lpOverlapped);
        [DllImport("Kernel32.dll")]
        public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, Boolean bManualReset, Boolean bInitialState, String lpName);
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool WaitCommEvent(IntPtr hFile, ref int lpEvtMask, ref OVERLAPPED lpOverlapped);
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool ClearCommError(IntPtr hFile, ref int lpErrors, ref COMMSTAT lpCommStat);
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Boolean GetOverlappedResult(IntPtr hFile, ref OVERLAPPED lpOverlapped, ref UInt32 nNumberOfBytesTransferred, Boolean bWait);
        [DllImport("Kernel32.dll")]
        public static extern Boolean CloseHandle(IntPtr hObject);
        [DllImport("Kernel32.dll")]
        public static extern Boolean CancelIo(IntPtr hFile);
        [DllImport("Kernel32.dll")]
        public static extern Boolean SetCommMask(IntPtr hFile, UInt32 dwEvtMask);
        [DllImport("Kernel32.dll")]
        public static extern Boolean GetCommState(IntPtr hFile, ref DCB lpDCB);
        [DllImport("Kernel32.dll")]
        public static extern Boolean GetCommTimeouts(IntPtr hFile, out COMMTIMEOUTS lpCommTimeouts);
        [DllImport("Kernel32.dll")]
        public static extern Boolean BuildCommDCBAndTimeouts(String lpDef, ref DCB lpDCB, ref COMMTIMEOUTS lpCommTimeouts);
        [DllImport("Kernel32.dll")]
        public static extern Boolean SetCommState(IntPtr hFile, [In] ref DCB lpDCB);
        [DllImport("Kernel32.dll")]
        public static extern Boolean SetCommTimeouts(IntPtr hFile, [In] ref COMMTIMEOUTS lpCommTimeouts);
        [DllImport("Kernel32.dll")]
        public static extern Boolean SetupComm(IntPtr hFile, UInt32 dwInQueue, UInt32 dwOutQueue);
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool PurgeComm(IntPtr hFile, uint dwFlags);
        [DllImport("Kernel32.dll")]
        public static extern void OutputDebugString(string lpOutputString);

        public enum vkButtons : int
        {
            VK_LBUTTON = 0x01,
            VK_RBUTTON = 0x02,
            VK_CANCEL = 0x03,
            VK_MBUTTON = 0x04,
            //
            VK_XBUTTON1 = 0x05,
            VK_XBUTTON2 = 0x06,
            //
            VK_BACK = 0x08,
            VK_TAB = 0x09,
            //
            VK_CLEAR = 0x0C,
            VK_RETURN = 0x0D,
            //
            VK_SHIFT = 0x10,
            VK_CONTROL = 0x11,
            VK_MENU = 0x12,
            VK_PAUSE = 0x13,
            VK_CAPITAL = 0x14,
            //
            VK_KANA = 0x15,
            VK_HANGEUL = 0x15,  /* old name - should be here for compatibility */
            VK_HANGUL = 0x15,
            VK_JUNJA = 0x17,
            VK_FINAL = 0x18,
            VK_HANJA = 0x19,
            VK_KANJI = 0x19,
            //
            VK_ESCAPE = 0x1B,
            //
            VK_CONVERT = 0x1C,
            VK_NONCONVERT = 0x1D,
            VK_ACCEPT = 0x1E,
            VK_MODECHANGE = 0x1F,
            //
            VK_SPACE = 0x20,
            VK_PRIOR = 0x21,
            VK_NEXT = 0x22,
            VK_END = 0x23,
            VK_HOME = 0x24,
            VK_LEFT = 0x25,
            VK_UP = 0x26,
            VK_RIGHT = 0x27,
            VK_DOWN = 0x28,
            VK_SELECT = 0x29,
            VK_PRINT = 0x2A,
            VK_EXECUTE = 0x2B,
            VK_SNAPSHOT = 0x2C,
            VK_INSERT = 0x2D,
            VK_DELETE = 0x2E,
            VK_HELP = 0x2F,
            //
            VK_LWIN = 0x5B,
            VK_RWIN = 0x5C,
            VK_APPS = 0x5D,
            //
            VK_SLEEP = 0x5F,
            //
            VK_NUMPAD0 = 0x60,
            VK_NUMPAD1 = 0x61,
            VK_NUMPAD2 = 0x62,
            VK_NUMPAD3 = 0x63,
            VK_NUMPAD4 = 0x64,
            VK_NUMPAD5 = 0x65,
            VK_NUMPAD6 = 0x66,
            VK_NUMPAD7 = 0x67,
            VK_NUMPAD8 = 0x68,
            VK_NUMPAD9 = 0x69,
            VK_MULTIPLY = 0x6A,
            VK_ADD = 0x6B,
            VK_SEPARATOR = 0x6C,
            VK_SUBTRACT = 0x6D,
            VK_DECIMAL = 0x6E,
            VK_DIVIDE = 0x6F,
            VK_F1 = 0x70,
            VK_F2 = 0x71,
            VK_F3 = 0x72,
            VK_F4 = 0x73,
            VK_F5 = 0x74,
            VK_F6 = 0x75,
            VK_F7 = 0x76,
            VK_F8 = 0x77,
            VK_F9 = 0x78,
            VK_F10 = 0x79,
            VK_F11 = 0x7A,
            VK_F12 = 0x7B,
            VK_F13 = 0x7C,
            VK_F14 = 0x7D,
            VK_F15 = 0x7E,
            VK_F16 = 0x7F,
            VK_F17 = 0x80,
            VK_F18 = 0x81,
            VK_F19 = 0x82,
            VK_F20 = 0x83,
            VK_F21 = 0x84,
            VK_F22 = 0x85,
            VK_F23 = 0x86,
            VK_F24 = 0x87,
            //
            VK_NUMLOCK = 0x90,
            VK_SCROLL = 0x91,
            //
            VK_OEM_NEC_EQUAL = 0x92,   // '=' key on numpad
            //
            VK_OEM_FJ_JISHO = 0x92,   // 'Dictionary' key
            VK_OEM_FJ_MASSHOU = 0x93,   // 'Unregister word' key
            VK_OEM_FJ_TOUROKU = 0x94,   // 'Register word' key
            VK_OEM_FJ_LOYA = 0x95,   // 'Left OYAYUBI' key
            VK_OEM_FJ_ROYA = 0x96,   // 'Right OYAYUBI' key
            //
            VK_LSHIFT = 0xA0,
            VK_RSHIFT = 0xA1,
            VK_LCONTROL = 0xA2,
            VK_RCONTROL = 0xA3,
            VK_LMENU = 0xA4,
            VK_RMENU = 0xA5,
            //
            VK_BROWSER_BACK = 0xA6,
            VK_BROWSER_FORWARD = 0xA7,
            VK_BROWSER_REFRESH = 0xA8,
            VK_BROWSER_STOP = 0xA9,
            VK_BROWSER_SEARCH = 0xAA,
            VK_BROWSER_FAVORITES = 0xAB,
            VK_BROWSER_HOME = 0xAC,
            //
            VK_VOLUME_MUTE = 0xAD,
            VK_VOLUME_DOWN = 0xAE,
            VK_VOLUME_UP = 0xAF,
            VK_MEDIA_NEXT_TRACK = 0xB0,
            VK_MEDIA_PREV_TRACK = 0xB1,
            VK_MEDIA_STOP = 0xB2,
            VK_MEDIA_PLAY_PAUSE = 0xB3,
            VK_LAUNCH_MAIL = 0xB4,
            VK_LAUNCH_MEDIA_SELECT = 0xB5,
            VK_LAUNCH_APP1 = 0xB6,
            VK_LAUNCH_APP2 = 0xB7,
            //
            VK_OEM_1 = 0xBA,   // ';:' for US
            VK_OEM_PLUS = 0xBB,   // '+' any country
            VK_OEM_COMMA = 0xBC,   // ',' any country
            VK_OEM_MINUS = 0xBD,   // '-' any country
            VK_OEM_PERIOD = 0xBE,   // '.' any country
            VK_OEM_2 = 0xBF,   // '/?' for US
            VK_OEM_3 = 0xC0,   // '`~' for US
            //
            VK_OEM_4 = 0xDB,  //  '[{' for US
            VK_OEM_5 = 0xDC,  //  '\|' for US
            VK_OEM_6 = 0xDD,  //  ']}' for US
            VK_OEM_7 = 0xDE,  //  ''"' for US
            VK_OEM_8 = 0xDF,
            //
            VK_OEM_AX = 0xE1,  //  'AX' key on Japanese AX kbd
            VK_OEM_102 = 0xE2,  //  "<>" or "\|" on RT 102-key kbd.
            VK_ICO_HELP = 0xE3,  //  Help key on ICO
            VK_ICO_00 = 0xE4,  //  00 key on ICO
            //
            VK_PROCESSKEY = 0xE5,
            //
            VK_ICO_CLEAR = 0xE6,
            //
            VK_PACKET = 0xE7,
            //
            VK_OEM_RESET = 0xE9,
            VK_OEM_JUMP = 0xEA,
            VK_OEM_PA1 = 0xEB,
            VK_OEM_PA2 = 0xEC,
            VK_OEM_PA3 = 0xED,
            VK_OEM_WSCTRL = 0xEE,
            VK_OEM_CUSEL = 0xEF,
            VK_OEM_ATTN = 0xF0,
            VK_OEM_FINISH = 0xF1,
            VK_OEM_COPY = 0xF2,
            VK_OEM_AUTO = 0xF3,
            VK_OEM_ENLW = 0xF4,
            VK_OEM_BACKTAB = 0xF5,
            //
            VK_ATTN = 0xF6,
            VK_CRSEL = 0xF7,
            VK_EXSEL = 0xF8,
            VK_EREOF = 0xF9,
            VK_PLAY = 0xFA,
            VK_ZOOM = 0xFB,
            VK_NONAME = 0xFC,
            VK_PA1 = 0xFD,
            VK_OEM_CLEAR = 0xFE
        }
        public enum dwDesiredAccess : uint
        {
            GENERIC_READ = 0x80000000,
            GENERIC_WRITE = 0x40000000,
            GENERIC_EXECUTE = 0x20000000,
            GENERIC_ALL = 0x10000000,
            RIGHT_ACCESS_SACL = 0x01000000,
            NONE = 0x00000000
        }
        public enum dwShareMode : int
        {
            ANY_TYPE_ACCESS = 0x00000000,
            FILE_SHARE_DELETE = 0x00000004,
            FILE_SHARE_READ = 0x00000001,
            FILE_SHARE_Write = 0x00000002
        }
        public enum dwCreationDisposion : int
        {
            CREATE_ALWAYS = 2,
            CREATE_NEW = 1,
            OPEN_ALWAYS = 4,
            OPEN_EXISTING = 3,
            TRUNCATE_EXISTING = 5
        }
        public enum dwFileAttributes : uint
        {
            FILE_ATTRIBUTE_ARCHIVE = 0x20,
            FILE_ATTRIBUTE_ENCRYPTED = 0x4000,
            FILE_ATTRIBUTE_HIDDEN = 0x2,
            FILE_ATTRIBUTE_NORMAL = 0x80,
            FILE_ATTRIBUTE_OFFLINE = 0x1000,
            FILE_ATTRIBUTE_READONLY = 0x1,
            FILE_ATTRIBUTE_SYSTEM = 0x4,
            FILE_ATTRIBUTE_TEMPORARY = 0x100
        }
        public enum dwFileFlags : uint
        {
            FILE_FLAG_BACKUP_SEMANTICS = 0x02000000,
            FILE_FLAG_DELETE_ON_CLOSE = 0x04000000,
            FILE_FLAG_NO_BUFFERING = 0x20000000,
            FILE_FLAG_OPEN_NO_RECALL = 0x00100000,
            FILE_FLAG_OPEN_REPARSE_POINT = 0x00200000,
            FILE_FLAG_OVERLAPPED = 0x40000000,
            FILE_FLAG_POSIX_SEMANTICS = 0x01000000,
            FILE_FLAG_RANDOM_ACCESS = 0x10000000,
            FILE_FLAG_SEQUENTIAL_SCAN = 0x08000000,
            FILE_FLAG_WRITE_THROUGH = 0x80000000
        }
        public struct _FILETIME
        {
            public int dwLowDateTime;
            public int dwHightDateTime;
        }
        public struct _SYSTEMTIME
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct DCB
        {
            //public Int32 DCBlength; // sizeof(DCB)
            //public Int32 BaudRate; // current baud rate
            //public Int32 PackedValues;
            //public Int32 fBinary; // binary mode, no EOF check 
            //public Int32 fParity; // enable parity checking 
            //public Int32 fOutxCtsFlow; // CTS output flow control 
            //public Int32 fOutxDsrFlow; // DSR output flow control 
            //public Int32 fDtrControl; // DTR flow control type 
            //public Int32 fDsrSensitivity; // DSR sensitivity 
            //public Int32 fTXContinueOnXoff; // XOFF continues Tx 
            //public Int32 fOutX; // XON/XOFF out flow control 
            //public Int32 fInX; // XON/XOFF in flow control 
            //public Int32 fErrorChar; // enable error replacement 
            //public Int32 fNull; // enable null stripping 
            //public Int32 fRtsControl; // RTS flow control 
            //public Int32 fAbortOnError; // abort reads/writes on error 
            //public Int32 fDummy2; // reserved
            //public Int16 wReserved; // not currently used
            //public Int16 XonLim; // transmit XON threshold
            //public Int16 XoffLim; // transmit XOFF threshold
            //public Byte ByteSize; // number of bits/byte, 4-8
            //public Byte Parity; // 0-4=no,odd,even,mark,space
            //public Byte StopBits; // 0,1,2 = 1, 1.5, 2
            //public Byte XonChar; // Tx and Rx XON character
            //public Byte XoffChar; // Tx and Rx XOFF character
            //public Byte ErrorChar; // error replacement character
            //public Byte EofChar; // end of input character
            //public Byte EvtChar; // received event character
            //public Int16 wReserved1; // reserved; do not use
            public Int32 DCBlength;
            public Int32 BaudRate;

            public Int32 Flags;
            //public Int32 fBinary;// = 1;
            //public Int32 fParity;// = 1;
            //public Int32 fOutxCtsFlow;// = 1;
            //public Int32 fOutxDsrFlow;// = 1;
            //public Int32 fDtrControl;// = 2;
            //public Int32 fDsrSensitivity;// = 1;
            //public Int32 fTXContinueOnXoff;// = 1;
            //public Int32 fOutX;// = 1;
            //public Int32 fInX;// = 1;
            //public Int32 fErrorChar;// = 1;
            //public Int32 fNull;// = 1;
            //public Int32 fRtsControl;// = 2;
            //public Int32 fAbortOnError;// = 1;
            //public Int32 fDummy2;// = 17;

            public Int16 wReserved;
            public Int16 XonLim;
            public Int16 XoffLim;
            public Byte ByteSize;
            public Byte Parity;
            public Byte StopBits;
            public char XonChar;
            public char XoffChar;
            public char ErrorChar;
            public char EofChar;
            public char EvtChar;
            public Int16 wReserved1;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct COMMTIMEOUTS
        {
            public UInt32 ReadIntervalTimeout;
            public UInt32 ReadTotalTimeoutMultiplier;
            public UInt32 ReadTotalTimeoutConstant;
            public UInt32 WriteTotalTimeoutMultiplier;
            public UInt32 WriteTotalTimeoutConstant;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct COMMSTAT
        {
            public UInt32 bitfield;
            public UInt32 cbInQue;
            public UInt32 cbOutQue;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct OVERLAPPED
        {
            public UIntPtr internalLow;
            public UIntPtr internalHigh;
            public UInt32 offset;
            public UInt32 offsetHigh;
            public IntPtr hEvent;
        }
        /// 
        /// Число информационных бит в байте.
        /// 
        public enum ByteSize : byte
        {
            Five = 5,
            Six = 6,
            Seven = 7,
            Eight = 8
        }
        /// 
        /// Скорости передачи данных.
        ///
        public enum BaudRate : int
        {
            Baud_110 = 110,
            Baud_300 = 300,
            Baud_600 = 600,
            Baud_1200 = 1200,
            Baud_2400 = 2400,
            Baud_4800 = 4800,
            Baud_9600 = 9600,
            Baud_14400 = 14400,
            Baud_19200 = 19200,
            Baud_38400 = 38400,
            Baud_56000 = 56000,
            Baud_57600 = 57600,
            Baud_115200 = 115200,
            Baud_128000 = 128000,
            Baud_256000 = 256000,
        }
        /// 
        /// Установки четности.
        /// 
        public enum Parity : byte
        {
            /// 
            /// Без бита четности.
            /// 
            None = 0,
            /// 
            /// Дополнение до нечетности.
            /// 
            Odd = 1,
            /// 
            /// Дополнение до четности.
            /// 
            Even = 2,
            /// 
            /// Бит четности всегда 1.
            /// 
            Mark = 3,
            /// 
            /// Бит четности всегда 0.
            /// 
            Space = 4
        }
        /// 
        /// Количество стоповых бит
        /// 
        public enum StopBits : byte
        {
            /// 
            /// Один стоповый бит
            /// 
            One = 0,
            /// 
            /// Полтора стоповых бита
            /// 
            OnePointFive = 1,
            /// 
            /// Два стоповых бита
            /// 
            Two = 2
        }
    }
}
