using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Runtime.Remoting.Messaging;

namespace winapi
{
    public static class AsyncFunc
    {
        private static object lastResult;
        private static Thread th;

        /// <summary>
        /// Check for valid file path.
        /// </summary>
        /// <param name="fileName">File path for check</param>
        /// <param name="timeout">Timeout for executing function</param>
        /// <exception cref="Timeout">Throw when file is not exists</exception>
        /// <returns>Return true when path is valid else false.</returns>
        public static bool FileExists(string path, int timeout)
        {
            lastResult = null;
            WApi.OutputDebugString("func start");
            th = new Thread(new ParameterizedThreadStart(FileExsist));
            th.IsBackground = true;
            th.Start(path);
            th.Join(timeout);
            if (th.IsAlive)
            {
                th.Abort();
                WApi.OutputDebugString("PathFileExists: time is out");
                //throw new TimeoutException("Timeout");
                return false;
            }

            if (lastResult != null)
                return (bool)lastResult;

            return false;

            //fdPathFileExists ff = winapi.Funcs.PathFileExists;
            //IAsyncResult result = ff.BeginInvoke(fileName, new AsyncCallback(WhenPathFileExists), "");
            //Thread.Sleep(timeout);
            //if (lastResult == null)
            //{
            //    winapi.Funcs.OutputDebugString("PathFileExists: time is out");
            //    fl_timeout = true;
            //    //throw new TimeoutException("Timeout");
            //}
            //else
            //    return (bool)lastResult;

            //return false;
        }
        /// <summary>
        /// Check if file or directory exist
        /// </summary>
        /// <param name="parameter">File name or directory path</param>
        private static void FileExsist(object path)
        {
            WApi.OutputDebugString("FileExsist start");

            if (path == null)
            {
                lastResult = false;
                return;
            }

            string fileName = path.ToString();

            if (string.IsNullOrEmpty(fileName))
            {
                lastResult = false;
                return;
            }

            uint attributes = WApi.GetFileAttributes(fileName);

            if (WApi.INVALID_FILE_ATTRIBUTE == attributes)
            {
                WApi.GetLastError();

                //if (winapi.Consts.ERROR_FILE_NOT_FOUND == error ||
                //    winapi.Consts.ERROR_PATH_NOT_FOUND == error)
                WApi.OutputDebugString("FileExsist end _ not valid atribute");
                lastResult = false;
                return;
            }

            //isDirectory = 0 != (winapi.Consts.FILE_ATTRIBUTE_DIRECTORY & attributes);

            WApi.OutputDebugString("FileExsist end");
            lastResult = true;
        }
    }
}
