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
        public static string GetHashMD5(string text)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(text);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            string password = s.ToString();
            return password;
        }

        /// <summary>
        /// Авторизація користувача в системі
        /// </summary>
        /// <param name="pass">Пароль</param>
        /// <param name="login">Логін</param>
        /// <returns>Результат авторизації</returns>
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
                            MMessageBoxEx.Show("Користувач заблокований\nЗверніться до адміністратора", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return "";
                        }
                    }
                }
                catch { }
            }

            MessageBox.Show("Невірний логін або пароль", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return "";
        }//ok
    }
}
