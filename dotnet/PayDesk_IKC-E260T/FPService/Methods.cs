using System;
using System.Collections.Generic;
using System.Text;

namespace FPService
{
    class Methods
    {

        #region GetNumber Overloaded
        public static long GetNumber(byte[] mas)
        {
            long number = mas[0];

            try
            {
                for (int i = 1; i < mas.Length; i++)
                    number += mas[i] * (long)(0x01 << (8 * i));
            }
            catch
            {
                throw new Exception("GetInt Error");
            }

            return number;
        } 
        #endregion

        #region GetByteArray Overloaded
        public static byte[] GetByteArray(short value, int range)
        {
            byte[] mas = new byte[range];

            try
            {
                for (int i = range - 1; i >= 0; i--)
                {
                    mas[i] = (byte)((ulong)value / ((ulong)0x01 << (8 * i)));
                    value -= (short)(mas[i] * ((ulong)0x01 << (8 * i)));
                }
            }
            catch
            {
                throw new Exception("GetByte Error");
            }

            return mas;
        }
        public static byte[] GetByteArray(int value, int range)
        {
            byte[] mas = new byte[range];

            try
            {
                for (int i = range - 1; i >= 0; i--)
                {
                    mas[i] = (byte)((ulong)value / ((ulong)0x01 << (8 * i)));
                    value -= (int)(mas[i] * ((ulong)0x01 << (8 * i)));
                }
            }
            catch
            {
                throw new Exception("GetByte Error");
            }

            return mas;
        }
        public static byte[] GetByteArray(long value, int range)
        {
            byte[] mas = new byte[range];
            try
            {
                for (int i = range - 1; i >= 0; i--)
                {
                    mas[i] = (byte)((ulong)value / ((ulong)0x01 << (8 * i)));
                    value -= (long)(mas[i] * ((ulong)0x01 << (8 * i)));
                }
            }
            catch
            {
                throw new Exception("GetByte Error");
            }

            return mas;
        }
        public static byte[] GetByteArray(ushort value, int range)
        {
            byte[] mas = new byte[range];

            try
            {
                for (int i = range - 1; i >= 0; i--)
                {
                    mas[i] = (byte)((ulong)value / ((ulong)0x01 << (8 * i)));
                    value -= (ushort)(mas[i] * ((ulong)0x01 << (8 * i)));
                }
            }
            catch
            {
                throw new Exception("GetByte Error");
            }

            return mas;
        }
        public static byte[] GetByteArray(uint value, int range)
        {
            byte[] mas = new byte[range];

            try
            {
                for (int i = range - 1; i >= 0; i--)
                {
                    mas[i] = (byte)((ulong)value / ((ulong)0x01 << (8 * i)));
                    value -= (uint)(mas[i] * ((ulong)0x01 << (8 * i)));
                }
            }
            catch
            {
                throw new Exception("GetByte Error");
            }


            //byte[] d = EncodeData(mas);
            

            //Encoding.GetEncoding(899).GetBytes(mas);
            return mas;
        } 
        public static byte[] GetByteArray(ulong value, int range)
        {
            byte[] mas = new byte[range];

            try
            {
                for (int i = range - 1; i >= 0; i--)
                {
                    mas[i] = (byte)(value / (ulong)(0x01 << (8 * i)));
                    value -= (ulong)(mas[i] * (0x01 << (8 * i)));
                }
            }
            catch
            {
                throw new Exception("GetByte Error");
            }

            return mas;
        }
        #endregion

        public static int[] GetArrayFromBCD(byte[] mas)
        {
            int[] ans = new int[mas.Length];
            string binLine = "";
            for (byte i = 0; i < mas.Length; i++)
            {
                binLine = Convert.ToString(mas[i], 16);
                ans[i] = int.Parse(binLine);
            }
            return ans;
        }
        public static long GetNumberFromBCD(byte[] mas)
        {
            if (mas == null || mas.Length == 0)
                return 0;

            string binLine = ""; //try { mas = EncodeData(mas); }
            //catch { }
            for (int i = mas.Length - 1; i >= 0; i--)
                binLine += string.Format("{0:D2}", byte.Parse(Convert.ToString(mas[i], 16)));

            return long.Parse(binLine);
        }
        public static long GetNumberFromBCD(byte[] mas, byte type)
        {
            if (mas == null || mas.Length == 0)
                return 0;

            string binLine = ""; //try { mas = EncodeData(mas); }
            //catch { }
            for (int i = mas.Length - 1; i >= 0; i--)
                if (type == 16)
                    binLine += string.Format("{0:D2}", byte.Parse(Convert.ToString(mas[i], 16)));
                else
                    binLine += string.Format("{0:D2}", byte.Parse(mas[i].ToString()));

            return long.Parse(binLine);
        }
        public static byte[] GetBCDFromArray(byte[] mas)
        {
            byte[] bcd = new byte[mas.Length];
            for (int i = 0; i < mas.Length; i++)
                bcd[i] = Convert.ToByte(mas[i].ToString(), 16);

            return bcd;
        }
        public static byte[] GetBCDFromArray(int[] mas)
        {
            byte[] bcd = new byte[mas.Length];
            for (int i = 0; i < mas.Length; i++)
                bcd[i] = Convert.ToByte(mas[i].ToString(),16);

            return bcd;
        }

        public static long Pow(int number, int step)
        {
            long newNumber = 1;


            try
            {
                for (int i = 0; i < step; i++)
                    newNumber *= number;
            }
            catch
            {
                throw new Exception("Pow error");
            }
            return newNumber;
        }
        public static byte SumMas(byte[] mas)
        {
            byte symbol = 0x00;

            for (byte i = 0; i < mas.Length; i++)
                symbol = (byte)(symbol + mas[i]);

            return symbol;
        }//ok
        public static double GetDouble(object value)
        {
            if (value == DBNull.Value)
                return 0.0;
            try
            {
                string val = value.ToString();
                for (int i = 0; i < val.Length; i++)
                    if (!Char.IsDigit(val, i))
                        val = val.Replace(val[i].ToString(), System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
                return double.Parse(val);
            }
            catch { return 0.0; }
        }
        public static byte[] EncodeData(byte[] mas)
        {
            byte[] outMas = new byte[mas.Length];

            for (int i = 0; i < mas.Length; i++)
                outMas[i] = Convert.ToByte("0x" + mas[i].ToString(), 16);

            return outMas;
        }

        public static void SavePublicFunctions(string protocolName, string[][] publicFunctions)
        {
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binF = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            if (!System.IO.Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\Modules"))
                System.IO.Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\Modules");
            System.IO.FileStream fs = new System.IO.FileStream(System.Windows.Forms.Application.StartupPath + "\\Modules\\" + protocolName + ".smf", System.IO.FileMode.Create, System.IO.FileAccess.Write);

            try
            {
                object[] type = new object[2];
                type[0] = publicFunctions[0];
                type[1] = publicFunctions[1];

                binF.Serialize(fs, type);
            }
            catch { }

            fs.Close();
            fs.Dispose();
        }
        public static string[][] RestorePublic(string protocolName, ref System.Windows.Forms.TreeView treeV)
        {
            string[][] publicFunctions = new string[2][] { new string[0], new string[0] };

            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binF = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            System.IO.FileStream fs = null;
            try
            {
                fs = new System.IO.FileStream(System.Windows.Forms.Application.StartupPath + "\\Modules\\" + protocolName + ".smf", System.IO.FileMode.Open, System.IO.FileAccess.Read);

                object[] type = (object[])binF.Deserialize(fs);
                fs.Close();
                fs.Dispose();

                publicFunctions[0] = (string[])type[0];
                publicFunctions[1] = (string[])type[1];

                byte f_idx = 0;
                byte pbf_idx = 0;

                for (byte i = 0; i < (byte)treeV.Nodes[0].Nodes.Count; i++)
                    for (f_idx = 0; f_idx < treeV.Nodes[0].Nodes[i].Nodes.Count; f_idx++)
                        for (pbf_idx = 0; pbf_idx < publicFunctions[0].Length; pbf_idx++)
                            if (publicFunctions[1][pbf_idx] == treeV.Nodes[0].Nodes[i].Nodes[f_idx].Name)
                            {
                                treeV.Nodes[0].Nodes[i].Nodes[f_idx].Checked = true;
                                break;
                            }
            }//try
            catch { }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }

            return publicFunctions;
        }
    }
}
