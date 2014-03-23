using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using driver.Config;
using System.Diagnostics;
using components.Components.MMessageBox;

namespace driver.Lib
{
    public static class FileMgrLib
    {
        public static void ClearCheckedBox(ref CheckedListBox checkedListBox1)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
                checkedListBox1.SetItemChecked(i, false);
            checkedListBox1.SetItemChecked(checkedListBox1.Items.Count - 1, false);
        }

        /// <summary>
        /// Перевірка та сворення всіх необхідних директорій, які використовуються програмою
        /// </summary>
        public static void VerifyAllFolders()
        {
            List<string> dirList = new List<string>();
            dirList.Add(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles); // 0
            dirList.Add(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Bills); // 1
            dirList.Add(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Cheques); // 2
            dirList.Add(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Schemes); // 3
            dirList.Add(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Users); // 4
            dirList.Add(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Temp); // 5
            dirList.Add(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Reports); // 6
            foreach (string dirPath in dirList)
            {
                if (!Directory.Exists(dirPath))
                {
                    try
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    catch
                    {
                        MMessageBoxEx.Show("Не вдалося отримати доступ до папки: " + dirPath + "\r\nЗверніться до адміністратора.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(0);
                    }
                }
            }
        }//ok
        /// <summary>
        /// Очищення директорії
        /// </summary>
        /// <param name="path">Шлях до директорії</param>
        public static void ClearFolder(string path)
        {
            if (Directory.Exists(path))
            {
                try
                {
                    Directory.Delete(path, true);
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    CoreLib.WriteLog(ex, MethodInfo.GetCurrentMethod().Name);
                }
            }

            // removing temporary archives
            string[] tmpFiles = Directory.GetFiles(Application.StartupPath, "*.bin.tmp*");
            foreach (string tmpFilePath in tmpFiles) try
                {
                    System.IO.File.Delete(tmpFilePath);
                }
                catch { }

            // removing previous logs
            string[] logFiles = Directory.GetFiles(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Reports, "*.log*");
            foreach (string logFilePath in logFiles) try
                {
                    if (!Path.GetFileNameWithoutExtension(logFilePath).Contains(DateTime.Now.ToString(".MM.yyyy")))
                        System.IO.File.Delete(logFilePath);
                }
                catch { }
        }//ok

    }
}