using System;
using System.Collections.Generic;
using System.Text;
using System.Management;

namespace components.Components.SecureRuntime
{
    public class Com_SecureRuntime
    {


        public Com_SecureRuntime()
        {
        }

        private void addToFile(string ln)
        {
            System.IO.FileStream txt = new System.IO.FileStream("logsg.txt", System.IO.FileMode.Append);
            System.IO.StreamWriter trw = new System.IO.StreamWriter(txt);
            trw.WriteLine(ln);
            trw.Close();
            trw.Dispose();
        }


        // it's on client's side only [ok]
        public string getPublicNumber()
        {// this number we show for client.
            string h = this.GetCleanMD5Hash(this.getNumber());
            string code = string.Empty;
            for (int i = 0; i < h.Length - 1; i++)
                code += byte.Parse(h[i].ToString()) ^ byte.Parse(h[i + 1].ToString());
            h = this.GetCleanMD5Hash(code);
            if (h.Length < 20) h += this.GetCleanMD5Hash(h);
            if (h.Length > 20) h = h.Substring(0, 20);
            //addToFile("getPublicNumber: " + h);

            return h;
        }

        public void setClientCode(string clientCryptedPrivateNumber)
        {
            System.IO.TextWriter twr = System.IO.File.CreateText(System.Windows.Forms.Application.StartupPath + @"\client.lic");
            twr.Write(this.getSCompared(clientCryptedPrivateNumber));
            twr.Close();
            twr.Dispose();
        }

        public bool FullLoader()
        {
            if (!System.IO.File.Exists(System.Windows.Forms.Application.StartupPath + @"\client.lic"))
            {
                //addToFile("STATE FALSE (no file client.lic)\r\n");
                return false;
            }

            System.IO.TextReader trd = System.IO.File.OpenText("client.lic");
            try
            {
                string dd = trd.ReadToEnd();
                string spn = this.getSPrivateNumber();

                //addToFile("FullLoader() ReadToEnd: " + dd);
                //addToFile("FullLoader() SPN Value : " + spn);

                //addToFile("client.lic === getSPrivateNumber() ::: " + dd + " === " + spn);
                if (dd == spn)
                {
                    trd.Close();
                    trd.Dispose();

                    //addToFile("STATE TRUE\r\n");
                    return true;
                }
            }
            catch { }

            trd.Close();
            trd.Dispose();
            //addToFile("STATE FALSE\r\n");
            return false;
        }

        /* Private Methods */

        private string getSCompared(string clientCryptedPrivateNumber)
        {
            return this.GetMD5Hash(this.compareClient(clientCryptedPrivateNumber));
        }

        private string compareClient(string clientCryptedPrivateNumber)
        {// compare clients number with holded
            string code = this.stringFiltering(clientCryptedPrivateNumber);
            string pk = this.getPrivateNumber();
            string cryptedCode = string.Empty;
            int[] cpk = new int[code.Length];
            int[] coded = new int[cpk.Length];
            for (int j = 0; j < code.Length; j += 5)
            {
                cpk[j] = int.Parse(string.Format("{0}{1}{2}{3}{4}", code[j], code[j + 1], code[j + 2], code[j + 3], code[j + 4]));
                coded[j] = cpk[j] ^ 0xd9a;
                cryptedCode += coded[j].ToString("0000");
            }
            
            if (pk == cryptedCode)
                return cryptedCode;

            return string.Empty;
        }

        private string getPrivateNumber()
        {// this number we hold inside app
            string h = this.getPublicNumber();
            string code = string.Empty;
            int cc = 0;
            for (int i = 0, j = 1; i < h.Length; i++, j++)
            {
                if (j >= h.Length)
                    j = 0;

                cc = byte.Parse(h[i].ToString()) ^ byte.Parse(h[j].ToString());
                code += this.GetCleanDMD5Hash(cc.ToString())[i];
            }
            //addToFile("getPrivateNumber: " + code);
            return code;
        }

        private string getSPrivateNumber()
        {
            string sprv = this.GetMD5Hash(this.getPrivateNumber());
            //addToFile("getSPrivateNumber: " + sprv);
            return sprv;
        }

        private string stringFiltering(string input)
        {
            string output = string.Empty;
            for (int i = 0; i < input.Length; i++)
            {
                if (Char.IsNumber(input[i]))
                    output += input[i];
            }
            return output;
        }

        private string GetCleanMD5Hash(string input)
        {
            return this.stringFiltering(this.GetMD5Hash(input));
        }

        private string GetCleanDMD5Hash(string input)
        {
            return this.stringFiltering(this.GetMD5Hash(input) + this.GetMD5Hash(input, 2));
        }

        private string GetMD5Hash(string input, byte cycle)
        {
            string sh = input;
            for (byte i = 0; i < cycle; i++)
                sh = this.GetMD5Hash(sh);
            return sh;
        }

        private string GetMD5Hash(string input)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            string password = s.ToString();
            return password;
        }

        private string getNumber()
        {
            ManagementClass mangnmt = new ManagementClass("Win32_LogicalDisk");
            ManagementObjectCollection mcol = mangnmt.GetInstances();
            string result = string.Empty;
            /*
            string envDir = string.Empty;
            string sysVolumeLabel = string.Empty;
            try
            {
                envDir = Environment.GetEnvironmentVariable("windir", EnvironmentVariableTarget.Machine);
                sysVolumeLabel = envDir.Split(new char[] { ':' })[0];
            }
            catch { }
            */
            foreach (ManagementObject strt in mcol)
            {
                //result += "Name              : " + Convert.ToString(strt["Name"]) + Environment.NewLine;
                //result += "VolumeName        : " + Convert.ToString(strt["VolumeName"]) + Environment.NewLine;
                //result += "VolumeSerialNumber: " + Convert.ToString(strt["VolumeSerialNumber"]) + Environment.NewLine;

                /*
                 * DriveType:
                    0 Unknown
                    1 No Root Directory
                    2 Removeable Disk
                    3 Local Disk
                    4 Network Drive
                    5 Compact Disc
                    6 RAM Disk
                */

                if (strt["DriveType"].ToString().Equals("3"))
                    result += Convert.ToString(strt["VolumeSerialNumber"]);


            }
            //Console.Out.WriteLine(result);
            //Console.In.ReadLine();

            //addToFile("getNumber: " + result);
            return result;
        }

        private DateTime getClientDate(ref string clientPrivateNumber)
        {// -1 = unlimited
         // other count is number of remaining days.
            return DateTime.Now;
        }

        private DateTime getExpireDate(ref string clientPrivateNumber)
        {// -1 = unlimited
            // other count is number of remaining days.
            return DateTime.Now;
        }
    
    }
}
