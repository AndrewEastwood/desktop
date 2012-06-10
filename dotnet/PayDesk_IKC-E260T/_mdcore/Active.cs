using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Management;
using System.Windows.Forms;

namespace mdcore
{
    public static class Active
    {
        static public string MakeSerial()
        {
            string mcode = GetMachineID();

            int r_num = new Random().Next(3, mcode.Length - 8);

            mcode = mcode.Insert(r_num, CodeDate(DateTime.Now));
            mcode = mcode.Insert(3, string.Format("{0:D2}", r_num));

            return mcode;
        }
        static private string GetMachineID()
        {
            //long HDD_ID = GetVolumeSerial(System.IO.Path.GetPathRoot(Environment.SystemDirectory));
            long Moth_ID = long.Parse(GetMthBId());
            long CPU_ID = long.Parse(GetCPUId());

            //HDD_ID = HDD_ID * 2 + 300;
            Moth_ID = Moth_ID * 3 - 500;
            CPU_ID = CPU_ID * 4 - 500;

            if (CPU_ID % 2 != 0)
                CPU_ID++;

            //return string.Format("{0}{1}{2}", HDD_ID, Moth_ID, CPU_ID);
            return string.Format("{0}{1}", Moth_ID, CPU_ID);
        }
        static private string CodeDate(DateTime date)
        {
            int d = date.Day;
            int m = date.Month;
            int y = date.Year;

            d *= 3;
            m = m * 7 + 11;
            y -= 123;

            string data = string.Format("{0:D4}{1:D2}{2:D2}", y, d, m);
            long n_data = long.Parse(data);
            n_data += 26111987;

            return n_data.ToString();
        }
        static private DateTime DecodeDate(string date)
        {
            try
            {
                int data = int.Parse(date);
                data -= 26111987;
                int y = int.Parse(data.ToString().Substring(0, 4));
                int d = int.Parse(data.ToString().Substring(4, 2));
                int m = int.Parse(data.ToString().Substring(6, 2));

                y += 123;
                m = (m - 11) / 7;
                d /= 3;
                return new DateTime(y, m, d);
            }
            catch { }

            return DateTime.Now;
        }
        static public bool SetGetState(string serial)
        {
            ModifyRegistry mf = new ModifyRegistry();
            if (serial == string.Empty || serial == null)
                serial = mf.Read("APID");

            if (serial == string.Empty || serial == null)
                return false;

            try
            {
                byte mask = byte.Parse(serial.Substring(0, 2));
                byte r_num = byte.Parse(serial.Substring(5, 2));
                string date = serial.Substring(r_num + 4, 8);

                //Check for valid MID
                string this_MID = GetMachineID();
                string recived_MID = serial.Remove(r_num + 4, 8);
                recived_MID = recived_MID.Remove(5, 2);
                recived_MID = recived_MID.Remove(0, 2);

                string new_code = string.Empty;
                byte num = 0;
                byte l = (byte)recived_MID.Length;
                if (l % 2 != 0)
                    l--;

                byte xor_num = 0;

                for (byte i = 0; i <= (l - 2); i += 2)
                {
                    num = byte.Parse(recived_MID.Substring(i, 2));
                    xor_num = (byte)(num ^ mask);
                    if (xor_num > (byte)99)
                        xor_num = num;
                    new_code += string.Format("{0:D2}", xor_num);
                }

                if (recived_MID.Length % 2 != 0)
                    new_code += recived_MID[recived_MID.Length - 1];


                if (new_code == this_MID)
                {
                    DateTime dt = DecodeDate(date);
                    if (dt.Day == 26 &&
                        dt.Month == 11 &&
                        dt.Year == 1987)
                    {
                        mf.Write("APID", serial);
                        return true;
                    }

                    if (dt.Day >= DateTime.Now.Day &&
                        dt.Month >= DateTime.Now.Month &&
                        dt.Year >= DateTime.Now.Year)
                    {
                        mf.Write("APID", serial);
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        //Additional
        #region AddFuncs
        static private string GetCPUId()
        {
            string cpuInfo = String.Empty;
            string temp = String.Empty;
            ManagementClass mc = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (cpuInfo == String.Empty)
                {
                    // only return cpuInfo from first CPU
                    try
                    {
                        temp += mo.Properties["Name"].Value.ToString();
                        temp += mo.Properties["Description"].Value.ToString();
                    }
                    catch { }

                }
            }

            temp = temp.Trim();

            for (int i = 0; i < temp.Length; i++)
                if (Char.IsNumber(temp[i]))
                    cpuInfo += temp[i].ToString();

            if (cpuInfo.Length > 12)
                return cpuInfo.Substring(0, 12);

            return cpuInfo;
        }
        static private string GetMthBId()
        {
            string cpuInfo = String.Empty;
            string temp = String.Empty;
            ManagementClass mc = new ManagementClass("Win32_MotherboardDevice");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                foreach (PropertyData PC in mo.Properties)
                {
                    if (cpuInfo == String.Empty)
                    {
                        // only return cpuInfo from first CPU
                        try
                        {
                            temp += PC.Value.ToString();
                            temp += PC.Value.ToString();
                        }
                        catch { }

                    }
                }
            }

            temp = temp.Trim();

            for (int i = 0; i < temp.Length; i++)
                if (Char.IsNumber(temp[i]))
                    cpuInfo += temp[i].ToString();

            return cpuInfo;
        }
        #endregion
    }
}
