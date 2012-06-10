using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Runtime.Remoting.Messaging;

namespace winapi
{
    public static class FuncT
    {
        private static object lastResult;
        private static Thread th;

        /// <summary>
        /// Check for valid file path. Return true when path is valid else false.
        /// </summary>
        /// <param name="fileName">File path for check</param>
        /// <param name="timeout">Timeout for executing function</param>
        /// <exception cref="Timeout"></exception>
        public static bool FileExists(string fileName, int timeout)
        {
            lastResult = null;
            API.OutputDebugString("func start");
            th = new Thread(new ParameterizedThreadStart(FileExsist));
            th.IsBackground = true;
            th.Start(fileName);
            th.Join(timeout);
            if (th.IsAlive)
            {
                th.Abort();
                API.OutputDebugString("PathFileExists: time is out");
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
        private static void FileExsist(object parameter)
        {
            API.OutputDebugString("fex start");

            if (parameter == null)
            {
                lastResult = false;
                return;
            }

            string fileName = parameter.ToString();

            if (string.IsNullOrEmpty(fileName))
            {
                lastResult = false;
                return;
            }

            uint attributes = API.GetFileAttributes(fileName);

            if (API.INVALID_FILE_ATTRIBUTE == attributes)
            {
                API.GetLastError();

                //if (winapi.Consts.ERROR_FILE_NOT_FOUND == error ||
                //    winapi.Consts.ERROR_PATH_NOT_FOUND == error)
                API.OutputDebugString("fex end _ not valid atribute");
                lastResult = false;
                return;
            }

            //isDirectory = 0 != (winapi.Consts.FILE_ATTRIBUTE_DIRECTORY & attributes);

            API.OutputDebugString("fex end");
            lastResult = true;
        }
        //private delegate bool fdPathFileExists(string fileName);
        //private static void WhenPathFileExists(IAsyncResult result)
        //{
        //    if (!fl_timeout)
        //    {
        //        winapi.Funcs.OutputDebugString("callabck start");
        //        AsyncResult async = (AsyncResult)result;
        //        fdPathFileExists fe = (fdPathFileExists)async.AsyncDelegate;
        //        lastResult = fe.EndInvoke(result);
        //        winapi.Funcs.OutputDebugString("callabck end");
        //    }
        //}




    }
}
