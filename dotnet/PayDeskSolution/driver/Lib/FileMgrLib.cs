using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using driver.Config;
/* custom */
//using Ionic;
//using Ionic.Zip;
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
            FolderBrowserDialog fBrDlg = new FolderBrowserDialog();
            fBrDlg.ShowNewFolderButton = true;
            bool pathSelected = false;
            bool saveSettings = false;
            DialogResult dRez = DialogResult.None;
            int idx = 0;
            List<string> dirList = new List<string>();
            dirList.Add(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles); // 0
            dirList.Add(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Bills); // 1
            dirList.Add(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Cheques); // 2
            dirList.Add(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Schemes); // 3
            dirList.Add(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Users); // 4
            dirList.Add(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Temp); // 5
            dirList.Add(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Reports); // 6
            //dirList.Add(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Reports); // 6
            string[] newPath = new string[dirList.Capacity];

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
                        dRez = MMessageBoxEx.Show("Не вдалося отримати доступ до папки: " + dirPath + "\r\nЗверніться до адміністратора.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(0);
                    }
                }
                idx++;
            }

            fBrDlg.Dispose();
            idx = 0;
            foreach (string nP in newPath)
            {
                if (nP != null && nP != string.Empty)
                {
                    switch (idx)
                    {
                        case 0: { driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles = nP; break; }
                        case 1: { driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Bills = nP; break; }
                        case 2: { driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Cheques = nP; break; }
                        case 3: { driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Schemes = nP; break; }
                        case 4: { driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Users = nP; break; }
                        case 5: { driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Temp = nP; break; }
                        case 6: { driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Reports = nP; break; }
                        case 7: { driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles = nP; break; }
                    }
                }
                idx++;
            }

            if (saveSettings)
                driver.Config.ConfigManager.SaveConfiguration();

            /*

          if (!Directory.Exists(AppConfig.Path_Articles))
          {
              try
              {
                  Directory.CreateDirectory(AppConfig.Path_Articles);
              }
              catch
              {
                  fBrDlg.Description = "Вибрі шляху для папки товарів";
                  dRez = MMessageBox.Show("Не вдалося отримати доступ до папки: " + AppConfig.Path_Articles + "\r\nЩоб встановити новий шлях до папки натисніть ТАК.", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                  pathSelected = false;
                  if (dRez == DialogResult.Yes)
                  {
                      do
                      {
                          fBrDlg.ShowDialog();
                          if (fBrDlg.SelectedPath == string.Empty)
                          {
                              dRez = MMessageBox.Show("Скасувати вибір нового шляху та закрити програму ", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                              if (dRez == DialogResult.Yes)
                                  Application.Exit(new CancelEventArgs(true));
                          }
                          else
                              pathSelected = true;
                      } while (pathSelected);
                  }
              }
          }
        if (!Directory.Exists(AppConfig.Path_Bills))
        {
            Directory.CreateDirectory(AppConfig.Path_Bills);
        }
        if (!Directory.Exists(AppConfig.Path_Cheques))
        {
            Directory.CreateDirectory(AppConfig.Path_Cheques);
        }
        if (!Directory.Exists(AppConfig.Path_Schemes))
        {
            Directory.CreateDirectory(AppConfig.Path_Schemes);
        }
        if (!Directory.Exists(AppConfig.Path_Users))
        {
            Directory.CreateDirectory(AppConfig.Path_Users);
        }
        if (!Directory.Exists(AppConfig.Path_Temp))
        {
            Directory.CreateDirectory(AppConfig.Path_Temp);
        }
        if (!Directory.Exists(AppConfig.Path_Reports))
        {
            Directory.CreateDirectory(AppConfig.Path_Reports);
        }*/
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
        
        #region 7z
        /*
        public static void CompressFiles_7z(string destinationDirectory, string archiveName, string pwd, params string[] files)
        {


            try
            {
                if (archiveName == null || archiveName == string.Empty)
                    archiveName = DateTime.Now.ToString("dd_MM_yyyy");
                else
                    if (archiveName[0] == '#')
                        archiveName = System.String.Format("{0}_{1:dd_MM_yyyy}", archiveName.Substring(1), System.DateTime.Now);

                if (pwd == null || pwd == string.Empty)
                    pwd = Convert.ToString(int.Parse(DateTime.Now.ToString("ddMMyyyy")), 16).ToUpper();
                //DateTime.Now.ToString("C_dd_MM_yyyy");
                string pathZip = destinationDirectory + "\\" + archiveName + ".7z";

                // get existed files from archive
                System.Collections.ObjectModel.ReadOnlyCollection<string> ss = null;
                if (File.Exists(pathZip))
                {
                    try
                    {
                        SevenZipExtractor ex = new SevenZipExtractor(pathZip, pwd);
                        ss = ex.ArchiveFileNames;
                        ex = null;
                    }
                    catch (Exception e)
                    {
                        CoreLib.WriteLog(e, MethodInfo.GetCurrentMethod().Name);
                    }
                }
                
                // check for exist
                Dictionary<int, string> upFiles = new Dictionary<int, string>();
                List<string> newFiles = new List<string>();
                if (ss != null)
                {
                    int idx = 0;
                    foreach (string singleFile in files)
                    {
                        if (ss.Contains(Path.GetFileName(singleFile)))
                            upFiles.Add(idx, singleFile);
                        else
                            newFiles.Add(singleFile);
                        idx++;
                    }
                }
                else
                    newFiles.AddRange(files);

                //Convert.ToString(int.Parse(DateTime.Now.ToString("ddMMyyyy")), 16);
                SevenZip.SevenZipCompressor.SetLibraryPath(Application.StartupPath + "\\" + "7z.dll");
                SevenZip.SevenZipCompressor zipFile = new SevenZipCompressor();
                zipFile.ArchiveFormat = OutArchiveFormat.SevenZip;
                zipFile.CompressionMethod = CompressionMethod.Lzma2;
                zipFile.CompressionLevel = CompressionLevel.Normal;
                //zipFile.EncryptHeaders = true;
                if (File.Exists(pathZip))
                    zipFile.CompressionMode = CompressionMode.Append;
                else
                    zipFile.CompressionMode = CompressionMode.Create;

                // update existed files
                /*
                if (upFiles.Count > 0)
                    zipFile.ModifyArchive(pathZip, upFiles, pwd);
                * /
                // add new files
                if(newFiles.Count > 0)
                    zipFile.CompressFilesEncrypted(pathZip, pwd, files);
                /*
                Dictionary<string, string> test = new Dictionary<string, string>();
                test.Add("0", destinationDirectory + @"\test.txt");
                test.Add("1", destinationDirectory + @"\test2.txt");
                zipFile.CompressionMode = CompressionMode.Append;
                zipFile.CompressFileDictionary(test, "test.7z", "test");
                * /
                zipFile = null;

            }
            catch (Exception e)
            {
                CoreLib.WriteLog(e, MethodInfo.GetCurrentMethod().Name);
            }
        }
        */
        #endregion




    }
}
