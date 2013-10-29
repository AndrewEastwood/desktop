using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace components.Lib
{
    public class CoreLib : ICollection
    {
        #region GetNumber Overloaded
        public long GetNumber(byte[] mas)
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
        public byte[] GetByteArray(short value, int range)
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
        public byte[] GetByteArray(int value, int range)
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
        public byte[] GetByteArray(long value, int range)
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
        public byte[] GetByteArray(ushort value, int range)
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
        public byte[] GetByteArray(uint value, int range)
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
        public byte[] GetByteArray(ulong value, int range)
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

        public byte[] GetNumbers(int value)
        {
            string sval = value.ToString();
            byte[] nums = new byte[sval.Length];

            for (int i = sval.Length - 1, j = 0; i >= 0; i--, j++)
                nums[j] = byte.Parse(sval[i].ToString());

            return nums;
        }
        public byte[] GetNumbers(int value, byte minRange)
        {
            string sval = value.ToString();
            byte[] nums = new byte[sval.Length];

            for (int i = sval.Length - 1, j = 0; i >= 0; i--, j++)
                nums[j] = byte.Parse(sval[i].ToString());

            if (minRange > nums.Length)
                Array.Resize<byte>(ref nums, minRange);

            return nums;
        }
        
        public int[] GetArrayFromBCD(byte[] mas)
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
        public long GetNumberFromBCD(byte[] mas)
        {
            if (mas == null || mas.Length == 0)
                return 0;

            string binLine = ""; //try { mas = EncodeData(mas); }
            //catch { }
            for (int i = mas.Length - 1; i >= 0; i--)
                binLine += string.Format("{0:D2}", byte.Parse(Convert.ToString(mas[i], 16)));
                //binLine += string.Format("{0:D2}", byte.Parse(""+(char)Convert.ToInt32("0x" + mas[i].ToString(), 16)));

            return int.Parse(binLine);
        }
        public long GetNumberFromBCD(byte[] mas, byte type)
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
        public byte[] GetBCDFromArray(byte[] mas)
        {
            byte[] bcd = new byte[mas.Length];
            for (int i = 0; i < mas.Length; i++)
                bcd[i] = Convert.ToByte(mas[i].ToString(), 16);

            return bcd;
        }
        public byte[] GetBCDFromArray(int[] mas)
        {
            byte[] bcd = new byte[mas.Length];
            for (int i = 0; i < mas.Length; i++)
                bcd[i] = Convert.ToByte(mas[i].ToString(),16);

            return bcd;
        }

        public long Pow(int number, int step)
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
        public byte SumMas(byte[] mas)
        {
            byte symbol = 0x00;

            for (byte i = 0; i < mas.Length; i++)
                symbol = (byte)(symbol + mas[i]);

            return symbol;
        }//ok
        public uint UIntSumMas(byte[] mas)
        {
            uint symbol = 0;

            for (byte i = 0; i < mas.Length; i++)
                symbol = (uint)(symbol + mas[i]);

            return symbol;
        }//ok
        public double GetDouble(object value)
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
        public byte[] EncodeData(byte[] mas)
        {
            byte[] outMas = new byte[mas.Length];

            for (int i = 0; i < mas.Length; i++)
                outMas[i] = Convert.ToByte("0x" + mas[i].ToString(), 16);

            return outMas;
        }

        public bool IsEmpty(byte[] mas)
        {
            if (mas == null)
                return true;

            byte _i=0;
            foreach (byte a in mas)
                if (a == null)
                    _i++;

            return (mas.Length == _i);
        }
        public bool IsEmpty(int[] mas)
        {
            if (mas == null)
                return true;

            int _i = 0;
            foreach (int a in mas)
                if (a == null)
                    _i++;

            return (mas.Length == _i);
        }
        public bool IsEmpty(char[] mas)
        {
            if (mas == null)
                return true;

            byte _i = 0;
            foreach (char a in mas)
                if (a == null)
                    _i++;

            return (mas.Length == _i);
        }
        public bool IsEmpty(object[] mas)
        {
            if (mas == null)
                return true;

            byte _i = 0;
            foreach (object a in mas)
                if (a == null)
                    _i++;

            return (mas.Length == _i);
        }

        /// <summary>
        /// Convert input object value into other format
        /// </summary>
        /// <typeparam name="T">Type of output format</typeparam>
        /// <param name="value">Input value</param>
        /// <returns>Return converted value in other format or return null</returns>
        public T GetValue<T>(object value)
        {
            return ((T)value);
        }

        public object[] GetResult<T>(T[] mas)
        {
            if (mas == null)
                return new object[0];
            object[] _rez = new object[mas.Length];
            mas.CopyTo(_rez, 0);
            return _rez;
        }
        public object[] GetResult<T>(T value)
        {
            if (value == null)
                return new object[0];
            return new object[1] { value };
        }

        public int GetOnlyNumericValue(string text)
        {
            int nm = 0;
            string val = string.Empty;
            try
            {
                for (int i = 0; i < text.Length; i++)
                    if (Char.IsDigit(text, i))
                        val += text[i];

                nm = int.Parse(val);
            }
            catch { }

            return nm;
        }

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Count
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool IsSynchronized
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public object SyncRoot
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        public Hashtable SortByKey(Hashtable obj)
        {
            Hashtable sortedObj = new Hashtable();
            List<string> keys = new List<string>();
            foreach (DictionaryEntry item in obj)
                keys.Add(item.Key.ToString());

            keys.Sort();

            foreach (string item in keys)
            {
                if (obj[item].GetType() == typeof(Hashtable))
                    sortedObj.Add(item, this.SortByKey((Hashtable)obj[item]));
                else
                    sortedObj.Add(item, obj[item]);
            }

            return sortedObj;
        }

        public string ArrayByteToString(byte[] arr) { return this.ArrayByteToString(";", arr); }
        public string ArrayByteToString(string delimiter, byte[] arr)
        {
            List<string> strList = new List<string>();
            for (int i = 0; i < arr.Length; i++)
                strList.Add(arr[i].ToString());
            return String.Join(delimiter, strList.ToArray());
        }

        public string ArrayIntToString(int[] arr) { return this.ArrayIntToString(";", arr); }
        public string ArrayIntToString(string delimiter, int[] arr)
        {
            List<string> strList = new List<string>();
            for (int i = 0; i < arr.Length; i++)
                strList.Add(arr[i].ToString());
            return String.Join(delimiter, strList.ToArray());
        }
    
    }
}
