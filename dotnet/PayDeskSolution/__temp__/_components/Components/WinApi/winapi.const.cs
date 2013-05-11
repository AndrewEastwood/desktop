using System;
using System.Collections.Generic;
using System.Text;

namespace components.Components.WinApi
{
    //constants
    public partial class Com_WinApi
    {
        public const UInt32 WAIT_OBJECT_0 = 0x00000000;
        public const UInt32 WAIT_ABANDONED = 0x00000080;
        public const UInt32 WAIT_ABANDONED_0 = 0x00000080;
        //Constants for return value:
        public const Int32 INVALID_HANDLE_VALUE = -1;
        public const UInt32 INVALID_FILE_ATTRIBUTE = 0xFFFFFFFF;

        ////Constants for dwFlagsAndAttributes:
        //public const UInt32 FILE_FLAG_OVERLAPPED = 0x40000000;

        ////Constants for dwCreationDisposition:
        //public const UInt32 OPEN_EXISTING = 3;

        ////Constants for dwDesiredAccess:
        //public const UInt32 GENERIC_READ = 0x80000000;
        //public const UInt32 GENERIC_WRITE = 0x40000000;

        // Constants for dwEvtMask:
        public const UInt32 EV_RXCHAR = 0x0001;
        public const UInt32 EV_RXFLAG = 0x0002;
        public const UInt32 EV_TXEMPTY = 0x0004;
        public const Int32 EV_CTS = 0x0008;
        public const UInt32 EV_DSR = 0x0010;
        public const UInt32 EV_RLSD = 0x0020;
        public const UInt32 EV_BREAK = 0x0040;
        public const UInt32 EV_ERR = 0x0080;
        public const UInt32 EV_RING = 0x0100;
        public const UInt32 EV_PERR = 0x0200;
        public const UInt32 EV_RX80FULL = 0x0400;
        public const UInt32 EV_EVENT1 = 0x0800;
        public const UInt32 EV_EVENT2 = 0x1000;
        // Added to enable use of "return immediately" timeout.
        public const UInt32 MAXDWORD = 0xffffffff;
        //Purge
        public const UInt16 PURGE_TXABORT = 0x0001; // Kill the pending/current writes to the comm port.
        public const UInt16 PURGE_RXABORT = 0x0002; // Kill the pending/current reads to the comm port.
        public const UInt16 PURGE_TXCLEAR = 0x0004; // Kill the transmit queue if there.
        public const UInt16 PURGE_RXCLEAR = 0x0008; // Kill the typeahead buffer if there.
        //// File attributes
        //public const UInt32 FILE_ATTRIBUTE_ARCHIVE = 0x20;
        //public const UInt32 FILE_ATTRIBUTE_COMPRESSED = 0x800;
        //public const UInt32 FILE_ATTRIBUTE_DEVICE = 0x40;
        //public const UInt32 FILE_ATTRIBUTE_DIRECTORY = 0x10;
        //public const UInt32 FILE_ATTRIBUTE_ENCRYPTED = 0x4000;
        //public const UInt32 FILE_ATTRIBUTE_HIDDEN = 0x2;
        //public const UInt32 FILE_ATTRIBUTE_NORMAL = 0x80;
        //public const UInt32 FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x2000;
        //public const UInt32 FILE_ATTRIBUTE_OFFLINE = 0x1000;
        //public const UInt32 FILE_ATTRIBUTE_READONLY = 0x1;
        //public const UInt32 FILE_ATTRIBUTE_REPARSE_POINT = 0x400;
        //public const UInt32 FILE_ATTRIBUTE_SPARSE_FILE = 0x200;
        //public const UInt32 FILE_ATTRIBUTE_SYSTEM = 0x4;
        //public const UInt32 FILE_ATTRIBUTE_TEMPORARY = 0x100;
        //public const UInt32 FILE_ATTRIBUTE_VIRTUAL = 0x10000;
        // Error codes
        public const UInt32 ERROR_FILE_NOT_FOUND = 0x02;
        public const UInt32 ERROR_PATH_NOT_FOUND = 0x03;
        public const UInt32 ERROR_INVALID_NAME = 123;
        public const UInt32 ERROR_ACCESS_DENIED = 5;
        public const UInt32 ERROR_INVALID_HANDLE = 6;
        public const UInt32 ERROR_IO_PENDING = 997;
        public const UInt32 ERROR_HANDLE_EOF = 38;

        // GUI

        public const int WH_CALLWNDPROCRET = 12;

        // E:GUI
    }
}
