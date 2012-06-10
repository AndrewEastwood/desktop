using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using components.Components.WinApi;

namespace components.Lib
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
        public static bool FileExists(string path, int timeout, string exceptionSuffix)
        {
            lastResult = null;
            Com_WinApi.OutputDebugString("func start");
            th = new Thread(new ParameterizedThreadStart(FileExsist));
            th.IsBackground = true;
            th.Start(path);
            th.Join(timeout);
            if (th.IsAlive)
            {
                th.Abort();
                Com_WinApi.OutputDebugString("PathFileExists: time is out");
                throw new TimeoutException("Timeout" + exceptionSuffix);
                //return false;
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
        public static bool FileExists(string path, int timeout)
        {
            return FileExists(path, timeout, string.Empty);
        }
        /// <summary>
        /// Check if file or directory exist
        /// </summary>
        /// <param name="parameter">File name or directory path</param>
        private static void FileExsist(object path)
        {
            Com_WinApi.OutputDebugString("FileExsist start");

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

            uint attributes = Com_WinApi.GetFileAttributes(fileName);

            if (Com_WinApi.INVALID_FILE_ATTRIBUTE == attributes)
            {
                Com_WinApi.GetLastError();

                //if (winapi.Consts.ERROR_FILE_NOT_FOUND == error ||
                //    winapi.Consts.ERROR_PATH_NOT_FOUND == error)
                Com_WinApi.OutputDebugString("FileExsist end _ not valid atribute");
                lastResult = false;
                return;
            }

            //isDirectory = 0 != (winapi.Consts.FILE_ATTRIBUTE_DIRECTORY & attributes);

            Com_WinApi.OutputDebugString("FileExsist end");
            lastResult = true;
        }
    }
}
