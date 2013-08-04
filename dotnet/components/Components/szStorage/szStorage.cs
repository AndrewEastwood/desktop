using System;
using System.Collections.Generic;
using System.Text;
/* internal */
using components.Components.pdLogger;
/* references */
using Ionic.Zip;

namespace components.Components.szStorage
{
    public class szStorage
    {
        public void CompressFiles(string destinationDirectory, string archiveName, Shared.Enums.pdFileCompressor type, string pwd, params string[] files)
        {
            switch (type)
            {
                case Shared.Enums.pdFileCompressor.SEVEN_ZIP:
                    this.CompressFiles_7z(destinationDirectory, archiveName, pwd, files);
                    break;
                case Shared.Enums.pdFileCompressor.ZIP:
                    this.CompressFiles_zip(destinationDirectory, archiveName, pwd, files);
                    break;
            }
        }

        public void CompressFiles(string destinationDirectory, string archiveName, string pwd, params string[] files)
        {
            CompressFiles_7z(destinationDirectory, archiveName, pwd, files);
        }

        public void CompressFiles_7z(string destinationDirectory, string archiveName, string pwd, params string[] files)
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
                string pathZip = destinationDirectory + "\\" + archiveName + ".bin";

                StringBuilder sb = new StringBuilder();
                sb.Append("a");
                sb.AppendFormat(" {0}", "\"" + pathZip + "\"");
                sb.Append(" -t7z");
                sb.Append(" -mhe=on");
                sb.AppendFormat(" -p{0}", pwd);
                sb.AppendFormat(" {0}", string.Join(" ", files));

                System.Diagnostics.Process p = new System.Diagnostics.Process();

                //p.StartInfo.Arguments = "test.7z -t7z -mhe=on -ptest 7-zip.dll";
                p.StartInfo.Arguments = sb.ToString();
                p.StartInfo.FileName = @"tools\compressor\7za.exe";
                p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                p.Start();

            }
            catch (Exception ex)
            {
                pdLogger.pdLogger.Logme(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
        }

        public void CompressFiles_zip(string destinationDirectory, string archiveName, string pwd, params string[] files)
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
                string pathZip = destinationDirectory + "\\" + archiveName + ".storage";


                using (ZipFile zip = new ZipFile(pathZip))
                {
                    //zip.AddFile("ReadMe.txt"); // unencrypted
                    //zip.
                    zip.Password = pwd;
                    zip.Encryption = EncryptionAlgorithm.WinZipAes256;
                    string singleName = string.Empty;
                    //ZipEntry itemZ = new ZipEntry();
                    foreach (string item in files)
                    {
                        singleName = item;

                        if (zip.ContainsEntry(singleName))
                            zip.UpdateFile(singleName).Password = pwd;
                        else
                        {
                            //itemZ.Encryption = EncryptionAlgorithm.WinZipAes256;
                            //itemZ.Password = pwd;
                            //itemZ.FileName = item;

                            zip.AddFile(singleName).Password = pwd;

                            //zip.Entries.Add(itemZ);

                            //ZipEntry.itemZ = new ZipEntry();
                        }
                    }
                    //zip.AddFile().pa(files, destinationDirectory);
                    //zip.AddFile("2008_Annual_Report.pdf");
                    try
                    {
                        zip.Comment = string.Format("{0}", int.Parse(zip.Comment) + 1);
                    }
                    catch (Exception ex)
                    {
                        pdLogger.pdLogger.Logme(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                        zip.Comment = zip.Count.ToString();
                    }

                    zip.Save(pathZip);

                }

            }
            catch (Exception ex)
            {
                pdLogger.pdLogger.Logme(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
        }
    }
}
