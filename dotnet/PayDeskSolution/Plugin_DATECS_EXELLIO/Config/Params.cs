using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Globalization;
using System.IO;

namespace DATECS_EXELLIO.Config
{
    public class Params
    {
        /* PRIVATE VARIABELS */

        private static Hashtable _driverData;
        private static Hashtable _errorFlags;
        private static Hashtable _allowedMethods;
        private static Hashtable _appAccess;
        private static Hashtable _miscData;
        private static Hashtable _compatibility;
        private static NumberFormatInfo _numberFormat;

        /* CONSTRUCTORS */

        public Params()
        {
            // initialize default configuration
            _numberFormat = new NumberFormatInfo();
            _driverData = new Hashtable();
            _errorFlags = new Hashtable();
            _allowedMethods = new Hashtable();
            _appAccess = new Hashtable();
            _miscData = new Hashtable();
            _compatibility = new Hashtable();
            // 6byte
            string[] _states = new string[48]{
                "",//0
                "",//1
                "�������� �������",//2
                "������� ����� �� ������",//3
                "�� ���������� �������",//4
                "���� � ��� �� ���� ���������� � ������� ��������� ��������� ��������� RAM",
                "��� �������� ������� ������������",//6
                "�������� ����������� �� ����������� �������",//7
                "",//8
                "",//9
                "³������ ������ ��������",//10
                "���� ���������� � RAM ���� �������� ��� ������� � ������� ���������", 
                "³������� ���������� ��� ����������",//12
                "�������� ������� ��������� �����",//13
                "������� �� ��������� � ��������� �����",//14
                "������������ �������� �������",//15
                "",//16
                "",//17
                "³������� ������������ ���",//18
                "���������� ���������� ������",//19
                "³������� ���������� ���",//20
                "���� ���������� ������",//21
                "���������� ������ ��� ���������� ������",//22
                "���������� ������ ��� ���������� ������",//23
                "",//24
                "��������� Sw7 ��������� � ���������� ����� �� ���������� ������",
                "��������� Sw6 ��������� � ������� (������ ������� 1251)",//26
                "��������� Sw5 ��������� � ������ ������ ��� �������� ���������� �� ������ DOS/Windows 1251",
                "��������� Sw4 ��������� � ����� \"�������� �������\"",//28
                "��������� Sw3 ��������� � ����������� ������ ����",//29
                "��������� Sw2 ��������� � �������� �����",//30
                "��������� Sw1 ��������� - �������� �����",//31
                "",//32
                "",//33
                "������� � ��������� �����",//34
                "Գ������� ������ �����������",//35
                "� ��������� ������ ������ ��������� ��� 50 Z-����",//36
                "���� ������ ��������� �����",//37
                "",//38
                "������� ������� ��� ����� � ���������� ������",//39
                "",//40
                "",//41
                "Գ�������� � ���������� ����� �������������",//42
                "�������� ������ ����������",//43
                "������� ������������",//44
                "",//45
                "Գ������� ������ ������������",//46
                "Գ������� ������ ����������� � ����� Read Only."};
            int[] _errorStates = new int[] { 3, 4, 6, 10, 11, 13, 14, 15, 21, 23, 34, 35, 39, 47 };

            // perform configuration number format
            _numberFormat.CurrencyDecimalSeparator = ".";
            _numberFormat.NumberDecimalSeparator = ".";

            // perform configuration driver data
            _driverData.Add("Status", _states.Clone());
            _driverData.Add("ErrorStatus", _errorStates.Clone());
            _driverData.Add("ArtMemorySize", (uint)11800);
            _driverData.Add("UserNo", (byte)1);
            _driverData.Add("UserPwd", "0000");
            _driverData.Add("DeskNo", (byte)1);
            _driverData.Add("LastFunc", "");
            _driverData.Add("LastArtNo", (uint)1);
            _driverData.Add("LastFOrderNo", (uint)0);
            _driverData.Add("LastNOrderNo", (uint)0);
            _driverData.Add("LastROrderNo", (uint)0);

            // perform configuration error flags
            _errorFlags.Add("FP_Sale", false);
            _errorFlags.Add("FP_PayMoney", false);
            _errorFlags.Add("FP_Payment", false);
            _errorFlags.Add("FP_Discount", false);
        }
        public Params(Hashtable Parameters)
            : this()
        {
            // Restoring configuration by recived parameters
            if (Parameters.Contains("DriverData"))
            {
                _driverData = (Hashtable)Parameters["DriverData"];

                if (_driverData.Contains("DecimalSeparator"))
                    _numberFormat.NumberDecimalSeparator = _driverData["DecimalSeparator"].ToString();
            }

            if (Parameters.Contains("ErrorFlags"))
                _errorFlags = (Hashtable)Parameters["ErrorFlags"];

            if (Parameters.Contains("AllowedMethods"))
                _allowedMethods = (Hashtable)Parameters["AllowedMethods"];

            if (Parameters.Contains("AppAccess"))
                _appAccess = (Hashtable)Parameters["AppAccess"];

            if (Parameters.Contains("MiscData"))
                _miscData = (Hashtable)Parameters["MiscData"];

            if (Parameters.Contains("Compatibility"))
                _compatibility = (Hashtable)Parameters["Compatibility"];
        }


        public void Load()
        {
            try
            {
                object _obj = LoadData(Path.FULL_CFG_PARAM_PATH);

                if (_obj == null)
                    Save();

                object[] _dat = (object[])_obj;
                _driverData = (Hashtable)_dat[0];
                _allowedMethods = (Hashtable)_dat[1];
                _errorFlags = (Hashtable)_dat[2];
            }
            catch { Save(); }
        }

        public void Save()
        {
            try
            {
                object[] _dat = new object[3];
                _dat[0] = _driverData.Clone();
                _dat[1] = _allowedMethods.Clone();
                _dat[2] = _errorFlags.Clone();

                SaveData(Path.FULL_CFG_PARAM_PATH, _dat);

            }
            catch { }
        }

        /// <summary>
        /// Perform saving data into file using binary formatter
        /// </summary>
        /// <param name="path">Path to file where data would be saved</param>
        /// <param name="data">Data which would be saved in selected file</param>
        private void SaveData(string path, object data)
        {
            FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binF = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            binF.TypeFormat = System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways;
            try
            {
                binF.Serialize(stream, data);
            }
            catch { }

            stream.Close();
            stream.Dispose();
        }

        /// <summary>
        /// Perform loading data from selected file
        /// </summary>
        /// <param name="path">Path of binary file which was saved using binary formatter</param>
        /// <returns>Return parsed data from selected file otherwise return null</returns>
        private object LoadData(string path)
        {
            if (!File.Exists(path))
                return null;

            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            object _data = new object();

            try
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binF = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                _data = binF.Deserialize(stream);

            }
            catch { _data = null; }

            stream.Close();
            stream.Dispose();

            return _data;
        }

        /* PROPERTIES */

        public static Hashtable DriverData { get { return _driverData; } set { _driverData = value; } }
        public static Hashtable ErrorFlags { get { return _errorFlags; } set { _errorFlags = value; } }
        public static Hashtable AllowedMethods { get { return _allowedMethods; } set { _allowedMethods = value; } }
        public static Hashtable AppAccess { get { return _appAccess; } set { _appAccess = value; } }
        public static Hashtable MiscData { get { return _miscData; } set { _miscData = value; } }
        public static Hashtable Compatibility { get { return _compatibility; } set { _compatibility = value; } }
        public static NumberFormatInfo NumberFormat { get { return _numberFormat; } set { _numberFormat = value; } }
    }
}


/*

���� S0 � ����� ����������
0.7 = 1	������
0.6 = 1 	������
0.5 = 1	�������� �������
0.4 = 1#	������� ����� �� ������ 
0.3 = 1   	�� ���������� �������
0.2 = 1	���� � ��� �� ���� ���������� � ������� ��������� ��������� ��������� RAM 
0.1 = 1#	��� �������� ������� ������������ 
0.0 = 1#	�������� ����������� �� ����������� ������� 

���� S1 � ����� ����������
1.7 = 1	������ 
1.6 = 1	������
1.5 = 1	³������ ������ ��������
1.4 = 1#	���� ���������� � RAM ���� �������� ��� ������� � ������� ��������� 
1.3 = 1	³������� ���������� ��� ����������	
1.2 = 1#	�������� ������� ��������� ����� 
1.1 = 1 #	������� �� ��������� � ��������� ����� 
1.0 = 1 	������������ �������� �������

���� S2 � ����� ����������
2.7 = 1	������
2.6 = 1 	�� ������������
2.5 = 1	³������� ������������ ���
2.4 = 1	���������� ���������� ������
2.3 = 1 ³������� ���������� ���
2.2 = 1	���� ���������� ������
2.1 = 1	���������� ������ ��� ���������� ������
2.0 = 1 ���������� ������ ��� ���������� ������

���� S3 � ��������� ��������������
3.7 = 1	������
3.6 = 1 	��������� Sw7 ��������� � ���������� ����� �� ���������� ������
3.5 = 1 	��������� Sw6 ��������� � ������� (������ ������� 1251)
3.4 = 1 	��������� Sw5 ��������� � ������ ������ ��� �������� ���������� �� ������ DOS/Windows 1251
3.3 = 1 	��������� Sw4 ��������� � ����� ��������� �������
3.2 = 1 	��������� Sw3 ��������� � ����������� ������ ����
3.1 = 1 	��������� Sw2 ��������� � �������� �����
3.0 = 1 	��������� Sw1 ��������� - �������� �����

���� S4:	���������� ������
4.7 = 1	������
4.6 = 1 	������
4.5 = 1	������� � ��������� �����
4.4 = 1 *	��������� ������ �����������
4.3 = 1  	� ��������� ������ ������ ��������� ��� 50 Z-���� 
4.2 = 1 	���� ������ ��������� �����
4.1 = 1 	�� ������������
4.0 = 1 *	������� ������� ��� ����� � ���������� ������

���� S5:	���������� ������
5.7 = 1	������
5.6 = 1 	������
5.5 = 1 	Գ�������� � ���������� ����� �������������
5.4 = 1	�������� ������ ���������� 
5.3 = 1  	������� ������������ 
5.2 = 1 *	�� ������������
5.1 = 1 	Գ������� ������ ������������ 
5.0 = 1 *	Գ������� ������ ����������� � ����� Read Only.
 * 
 * 
 * 
 * 
 * 
���� S0 � ����� ����������
0.7 = 1	������
0.6 = 1 	������
0.5 = 1	����� ������ � ���� ��� ��������������� ������ ����� ���������� ���� �� �����, ������������� �������� �#�.
0.4 = 1#	�������� ����������� ���������� ���������� 
0.3 = 1   	�� ��������� �������
0.2 = 1	���� � ����� �� ���� ����������� � ������� ���������� ���������� ��������� RAM 
0.1 = 1#	��� ���������� ������� ������� 
0.0 = 1#	���������� ������ �������� �������������� ������ 

���� S1 � ����� ����������
1.7 = 1	������ 
1.6 = 1	������
1.5 = 1	������� ������ ��������
1.4 = 1#	���������� ����������� ������ ���� ��������� (RAM) ��� ��������� � ��������� ��������� 
1.3 = 1	��� �������������  2.3  ��������, ��� ������ �� ���������� ���, � ��� ��������.	
1.2 = 1#	��������� ��������� ��������� ����������� ������ 
1.1 = 1 #	����������� ������� �� ��������� ��� �������� ����������� ������ �������� 
1.0 = 1 	��� ���������� ������� ��������� ������������ �������� ������������ � ��������� 1.1 ���� ��� ����������� ���������, �� �� ��� �������� �� ����� ���� ��������� 

���� S2 � ����� ����������
2.7 = 1	������
2.6 = 1 	�� ������������
2.5 = 1	������ ������������ ���
2.4 = 1	������������� (�� ��� �� �����������) ����������� �����
2.3 = 1  	������ ���������� ���
2.2 = 1	��� ����������� �����
2.1 = 1	������������� (�� ��� �� �����������) ������� ��� ����������� �����
2.0 = 1 	����������� ������� ��� ����������� �����

���� S3 � ��������� ��������������
3.7 = 1	������
3.6 = 1 	������������� Sw7 ��������� ON � ����������� ����� �� ����������� �����
3.5 = 1 	������������� Sw6 ��������� ON � ������� (������� ������� 1251)
3.4 = 1 	������������� Sw5 ��������� ON � ������� �������� ��� ������� ������ �� ������ DOS/Windows 1251
3.3 = 1 	������������� Sw4 ��������� ON � ����� ����������� �������
3.2 = 1 	������������� Sw3 ��������� ON � �������������� ������� ����
3.1 = 1 	������������� Sw2 ��������� ON � �������� ����������������� �����
3.0 = 1 	������������� Sw1 ��������� ON - �������� ����������������� �����

���� S4:	���������� ������
4.7 = 1	������
4.6 = 1 	������
4.5 = 1	���� ��� ��������������� ������ ����� ���������� ���� �� �����, ������������� �������� �*� � ������ 4 ��� 5
4.4 = 1 *	���������� ������ �����������
4.3 = 1  	� ���������� ������ ���� ����� �� ������� ���� ��� 50 Z-������� 
4.2 = 1 	��� ������ ���������� ������
4.1 = 1 	�� ������������
4.0 = 1 *	�������� ������ ��� ������ � ���������� ������

���� S5:	���������� ������
5.7 = 1	������
5.6 = 1 	������
5.5 = 1 	���������� � ��������� ����� �����������������
5.4 = 1	��������� ������ ���������� 
5.3 = 1  	���������� ��������������� 
5.2 = 1 *	�� ������������
5.1 = 1 	���������� ������ �������������� 
5.0 = 1 *	���������� ������ ����������� � ����� Read Only.



*/