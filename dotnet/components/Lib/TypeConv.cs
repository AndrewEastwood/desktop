using System;
using System.Collections.Generic;
using System.Text;

namespace components.Lib
{
    public class TypeConv
    {
        /// <summary>
        /// Convert text to array of selected type
        /// </summary>
        /// <typeparam name="T">Type of each value in array</typeparam>
        /// <param name="text">Input text</param>
        /// <returns>Return array with converted values to selected type</returns>
        public T[] StringTo<T>(string text)
        {
            List<T> _arr = new List<T>();
            string[] strings = text.Split(',');

            foreach (string s in strings)
            {
                try
                {
                    _arr.Add((T)Convert.ChangeType(s, typeof(T)));
                }
                catch { }

            }

            return _arr.ToArray();
        }

        public string[] StringToString(string text)
        {
            return text.Split(',');
        }
        public int[] StringToInt(string text)
        {
            string[] strings = text.Split(',');
            List<int> _arr = new List<int>();

            foreach (string s in strings)
            {
                try
                {
                    int i;
                    if (int.TryParse(s, out i))
                        _arr.Add(i);
                }
                catch { }

            }

            return _arr.ToArray();

        }
        public double[] StringToDouble(string text)
        {
            string[] strings = text.Split(',');
            List<double> _arr = new List<double>();

            foreach (string s in strings)
            {
                try
                {
                    double i;
                    if (double.TryParse(s, out i))
                        _arr.Add(i);
                }
                catch { }

            }

            return _arr.ToArray();
        }
        public uint[] StringToUInt(string text)
        {
            string[] strings = text.Split(',');
            List<uint> _arr = new List<uint>();

            foreach (string s in strings)
            {
                try
                {
                    uint i;
                    if (uint.TryParse(s, out i))
                        _arr.Add(i);
                }
                catch { }

            }

            return _arr.ToArray();
        }
        public short[] StringToShort(string text)
        {
            string[] strings = text.Split(',');
            List<short> _arr = new List<short>();

            foreach (string s in strings)
            {
                try
                {
                    short i;
                    if (short.TryParse(s, out i))
                        _arr.Add(i);
                }
                catch { }

            }

            return _arr.ToArray();
        }
        public byte[] StringToByte(string text)
        {
            string[] strings = text.Split(',');
            List<byte> _arr = new List<byte>();

            foreach (string s in strings)
            {
                try
                {
                    byte i;
                    if (byte.TryParse(s, out i))
                        _arr.Add(i);
                }
                catch { }

            }

            return _arr.ToArray();
        }
        public bool[] StringToBool(string text)
        {
            string[] strings = text.Split(',');
            List<bool> _arr = new List<bool>();

            foreach (string s in strings)
            {
                try
                {
                    bool i;
                    if (bool.TryParse(s, out i))
                        _arr.Add(i);
                }
                catch { }
            }

            return _arr.ToArray();
        }
        public float[] StringToFloat(string text)
        {
            string[] strings = text.Split(',');
            List<float> _arr = new List<float>();

            foreach (string s in strings)
            {
                try
                {
                    float i;
                    if (float.TryParse(s, out i))
                        _arr.Add(i);
                }
                catch { }
            }

            return _arr.ToArray();
        }
        public decimal[] StringToDecimal(string text)
        {
            string[] strings = text.Split(',');
            List<decimal> _arr = new List<decimal>();

            foreach (string s in strings)
            {
                try
                {
                    decimal i;
                    if (decimal.TryParse(s, out i))
                        _arr.Add(i);
                }
                catch { }
            }

            return _arr.ToArray();
        }

        /// <summary>
        /// Coneversion delegate
        /// </summary>
        /// <param name="text">Input text to converting</param>
        /// <returns>Return object array</returns>
        public delegate object[] StringToType(string text);

        public object StringToArray(string text, Type t)
        {
            string _tname = GetBaseTypeName(t);
            object _rez = new object();

            try
            {
                _rez = typeof(TypeConv).GetMethod("StringTo" + _tname).Invoke(this, new object[] { text });
            }
            catch { }

            return _rez;

        }
        public object StringToArray(object text, Type t)
        {
            return StringToArray(text.ToString(), t);
        }

        /// <summary>
        /// Create based type by type's name
        /// </summary>
        /// <param name="typeName">The name of type</param>
        /// <returns>Return based created type by name</returns>
        public Type GetBaseType(string typeName)
        {
            return Type.GetType(typeName.Trim('[', ']'));
        }
        /// <summary>
        /// Create based type by current type
        /// </summary>
        /// <param name="t">Current type</param>
        /// <returns>Return based created type by current type</returns>
        public Type GetBaseType(Type t)
        {
            return Type.GetType(t.FullName.Trim('[', ']'));
        }
        public string GetBaseTypeName(string typeName)
        {
            return typeName.Trim('[', ']');
        }
        public string GetBaseTypeName(Type t)
        {
            return t.Name.Trim('[', ']');
        }


        static public T ConvertTo<T>(object data)
        {
            return ConvertTo<T>(data, System.Globalization.NumberStyles.Any);
        }
        static public T ConvertTo<T>(object data, System.Globalization.NumberStyles numFormat)
        {
            T rez = default(T);

            try
            {
                switch (typeof(T).ToString())
                {
                    case "System.Int16":
                        {
                            rez = (T)Convert.ChangeType(System.Int16.Parse(data.ToString(), numFormat), typeof(T));
                            break;
                        }
                    case "System.Int32":
                        {
                            rez = (T)Convert.ChangeType(System.Int32.Parse(data.ToString(), numFormat), typeof(T));
                            break;
                        }
                    case "System.Int64":
                        {
                            rez = (T)Convert.ChangeType(System.Int64.Parse(data.ToString(), numFormat), typeof(T));
                            break;
                        }
                    case "System.UInt16":
                        {
                            rez = (T)Convert.ChangeType(System.UInt16.Parse(data.ToString(), numFormat), typeof(T));
                            break;
                        }
                    case "System.UInt32":
                        {
                            rez = (T)Convert.ChangeType(System.UInt32.Parse(data.ToString(), numFormat), typeof(T));
                            break;
                        }
                    case "System.UInt64":
                        {
                            rez = (T)Convert.ChangeType(System.UInt64.Parse(data.ToString(), numFormat), typeof(T));
                            break;
                        }
                    case "System.Byte":
                        {
                            rez = (T)Convert.ChangeType(System.Byte.Parse(data.ToString(), numFormat), typeof(T));
                            break;
                        }
                    case "System.SByte":
                        {
                            rez = (T)Convert.ChangeType(System.SByte.Parse(data.ToString(), numFormat), typeof(T));
                            break;
                        }
                    case "System.Decimal":
                        {
                            rez = (T)Convert.ChangeType(System.Decimal.Parse(data.ToString(), numFormat), typeof(T));
                            break;
                        }
                    case "System.UIntPtr":
                        {
                            rez = (T)Convert.ChangeType(new System.UIntPtr(System.UInt32.Parse(data.ToString(), numFormat)), typeof(T));
                            break;
                        }
                    case "System.IntPtr":
                        {
                            rez = (T)Convert.ChangeType(new System.IntPtr(System.Int32.Parse(data.ToString(), numFormat)), typeof(T));
                            break;
                        }
                    case "System.Drawing.Font":
                        {
                            System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(System.Drawing.Font));
                            rez = (T)converter.ConvertFromString(data.ToString());
                            break;
                        }
                    default:
                        {
                            rez = (T)Convert.ChangeType(data, typeof(T));
                            break;
                        }
                }
                //rez = (T)data;
            }
            catch { }

            return rez;
        }
   
    }
}
