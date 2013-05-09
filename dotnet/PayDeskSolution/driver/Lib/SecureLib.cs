using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Management;
using System.Windows.Forms;
using System.IO;
using driver.Components.Objects;
using driver.Config;
using components.Components.MMessageBox;

namespace driver.Lib
{
    public static class SecureLib
    {

        /// <summary>
        /// ����������� ����������� � ������
        /// </summary>
        /// <param name="pass">������</param>
        /// <param name="login">����</param>
        /// <returns>��������� �����������</returns>
        public static string Authorize(string pass, string login)
        {
            if (!Directory.Exists(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Users))
                Directory.CreateDirectory(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Users);

            string servpass = string.Format("{0:dMyyyy}", DateTime.Now);
            if (pass == servpass && login == "0")
                return "service";

            string[] userFiles = Directory.GetFiles(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Users, "*.usr");
            for (byte i = 0; userFiles != null && i < userFiles.Length; i++)
            {
                try
                {
                    UserConfig.LoadData(userFiles[i]);
                    if (UserConfig.UserLogin == login && UserConfig.UserPassword == pass)
                    {
                        if (UserConfig.Properties[0])
                            return "main";
                        else
                        {
                            MMessageBoxEx.Show("���������� ������������\n���������� �� �������������", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return "";
                        }
                    }
                }
                catch { }
            }

            MessageBox.Show("������� ���� ��� ������", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return "";
        }//ok
    }
}
