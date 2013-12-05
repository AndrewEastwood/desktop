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
                "Загальна помилка",//2
                "Механізм друку не працює",//3
                "Не підключений дисплей",//4
                "Дата і час не були встановлені з моменту останньго аварійного обнулення RAM",
                "Код отриманої команди неправильний",//6
                "Отримана інфоримація має синтаксичну помилку",//7
                "",//8
                "",//9
                "Відкрита кришка принтера",//10
                "Вміст інформації в RAM було втрачене при включені – аварійне обнулення", 
                "Відкритий фіскальний чек повернення",//12
                "Здійснене аварійне обнулення памяті",//13
                "Команда не дозволена у поточному режимі",//14
                "Переповнення операції підсумку",//15
                "",//16
                "",//17
                "Відкритий нефіскальний чек",//18
                "Закінчується контрольна стрічка",//19
                "Відкритий фіскальний чек",//20
                "Нема контрольної стрічки",//21
                "Закінчується чекова або контрольна стрічка",//22
                "Закінчилась чекова або контрольна стрічка",//23
                "",//24
                "Перемикач Sw7 увімкнений – зменьшений шрифт на контрольній стрічці",
                "Перемикач Sw6 увімкнений – дисплей (кодова таблиця 1251)",//26
                "Перемикач Sw5 увімкнений – кодова стрінка для відправки інформації на притер DOS/Windows 1251",
                "Перемикач Sw4 увімкнений – режим \"прозорий дисплей\"",//28
                "Перемикач Sw3 увімкнений – автоматична обрізка чеку",//29
                "Перемикач Sw2 увімкнений – швидкість порту",//30
                "Перемикач Sw1 увімкнений - швидкість порту",//31
                "",//32
                "",//33
                "Помилка у фіскальній памяті",//34
                "Фіскальна память переповнена",//35
                "В фіскальній память вільниа приблизно для 50 Z-звітів",//36
                "Нема модуля фіскальної памяті",//37
                "",//38
                "Виникла помилка при записі у фіскальную память",//39
                "",//40
                "",//41
                "Фіскальний і заводський номер запрограмовані",//42
                "Податкові ставки встановлені",//43
                "Пристрій фіскалізовано",//44
                "",//45
                "Фіскальна память сформатована",//46
                "Фіскальна память встановлена в режим Read Only."};
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

Байт S0 – общее назначение
0.7 = 1	резерв
0.6 = 1 	резерв
0.5 = 1	Загальна помилка
0.4 = 1#	Механізм друку не працює 
0.3 = 1   	Не підключений дисплей
0.2 = 1	Дата і час не були встановлені з моменту останньго аварійного обнулення RAM 
0.1 = 1#	Код отриманої команди неправельний 
0.0 = 1#	Отримана інфоримація має синтаксичну помилку 

Байт S1 – общее назначение
1.7 = 1	резерв 
1.6 = 1	резерв
1.5 = 1	Відкрита кришка принтера
1.4 = 1#	Вміст інформації в RAM було втрачене при включені – аварійне обнулення 
1.3 = 1	Відкритий фіскальний чек повернення	
1.2 = 1#	Здійснене аварійне обнулення памяті 
1.1 = 1 #	Команда не дозволина у поточному режимі 
1.0 = 1 	Переповнення операції підсумку

Байт S2 – общее назначение
2.7 = 1	резерв
2.6 = 1 	Не используется
2.5 = 1	Відкритий нефіскальний чек
2.4 = 1	Закінчується контрольна стрічка
2.3 = 1 Відкритий фіскальний чек
2.2 = 1	Нема контрольної стрічки
2.1 = 1	Закінчується чекова або контрольна стрічка
2.0 = 1 Закінчилась чекова або контрольна стрічка

Байт S3 – состояние переключателей
3.7 = 1	резерв
3.6 = 1 	Перемикач Sw7 увімкнений – зменьшений шрифт на контрольній стрічці
3.5 = 1 	Перемикач Sw6 увімкнений – дисплей (кодова таблиця 1251)
3.4 = 1 	Перемикач Sw5 увімкнений – кодова стрінка для відправки інформації на притер DOS/Windows 1251
3.3 = 1 	Перемикач Sw4 увімкнений – режим ”прозорий дисплей”
3.2 = 1 	Перемикач Sw3 увімкнений – автоматична обрізка чеку
3.1 = 1 	Перемикач Sw2 увімкнений – швидкість порту
3.0 = 1 	Перемикач Sw1 увімкнений - швидкість порту

байт S4:	фискальная память
4.7 = 1	резерв
4.6 = 1 	резерв
4.5 = 1	Помилка у фіскальній памяті
4.4 = 1 *	Фискальна память переповнена
4.3 = 1  	В фискальній память вільниа приблизно для 50 Z-звітів 
4.2 = 1 	Нема модуля фискальної памяті
4.1 = 1 	Не используется
4.0 = 1 *	Виникла помилка при записі у фіскальную память

байт S5:	фискальная память
5.7 = 1	резерв
5.6 = 1 	резерв
5.5 = 1 	Фіскальний і заводський номер запрограмовані
5.4 = 1	Податкові ставки встановлені 
5.3 = 1  	Пристрій фіскалізовано 
5.2 = 1 *	Не используется
5.1 = 1 	Фіскальна память сформатована 
5.0 = 1 *	Фіскальна память встановлена в режим Read Only.
 * 
 * 
 * 
 * 
 * 
Байт S0 – общее назначение
0.7 = 1	резерв
0.6 = 1 	резерв
0.5 = 1	Общая ошибка – этот бит устанавливается всегда когда установлен один из битов, маркированный символом ‘#’.
0.4 = 1#	Механизм печатающего устройства неисправен 
0.3 = 1   	Не подключен дисплей
0.2 = 1	Дата и время не были установлены с момента последнего аварийного обнуления RAM 
0.1 = 1#	Код полученной команды неверен 
0.0 = 1#	Полученные данные содержат синтаксическую ошибку 

Байт S1 – общее назначение
1.7 = 1	резерв 
1.6 = 1	резерв
1.5 = 1	Открыта крышка принтера
1.4 = 1#	Содержимое оперативной памяти было разрушено (RAM) при включении – аварийное обнуление 
1.3 = 1	При установленном  2.3  означает, что открыт не фискальный чек, а чек возврата.	
1.2 = 1#	Совершено аварийное обнуление оперативной памяти 
1.1 = 1 #	Выполняемая команда не разрешена для текущего фискального режима принтера 
1.0 = 1 	При выполнении команды произошло переполнение операции суммирования – Состояние 1.1 если оно установлено указывает, на то что операция не может быть выполнена 

Байт S2 – общее назначение
2.7 = 1	резерв
2.6 = 1 	Не используется
2.5 = 1	Открыт нефискальный чек
2.4 = 1	Заканчивается (но еще не закончилась) контрольная лента
2.3 = 1  	Открыт фискальный чек
2.2 = 1	Нет контрольной ленты
2.1 = 1	Заканчивается (но еще не закончилась) чековая или контрольная лента
2.0 = 1 	Закончилась чековая или контрольная лента

Байт S3 – состояние переключателей
3.7 = 1	резерв
3.6 = 1 	переключатель Sw7 положение ON – уменьшенный шрифт на контрольной ленте
3.5 = 1 	переключатель Sw6 положение ON – дисплей (кодовая таблица 1251)
3.4 = 1 	переключатель Sw5 положение ON – кодовая страница для посылки данных на притер DOS/Windows 1251
3.3 = 1 	переключатель Sw4 положение ON – режим ”прозрачный дисплей”
3.2 = 1 	переключатель Sw3 положение ON – автоматическая обрезка чека
3.1 = 1 	переключатель Sw2 положение ON – скорость последовательного порта
3.0 = 1 	переключатель Sw1 положение ON - скорость последовательного порта

байт S4:	фискальная память
4.7 = 1	резерв
4.6 = 1 	резерв
4.5 = 1	этот бит устанавливается всегда когда установлен один из битов, маркированный символом ‘*’ в байтах 4 или 5
4.4 = 1 *	Фискальная память переполнена
4.3 = 1  	В фискальной памяти есть место по крайней мере для 50 Z-отчетов 
4.2 = 1 	Нет модуля фискальной памяти
4.1 = 1 	Не используется
4.0 = 1 *	Возникла ошибка при записи в фискальную память

байт S5:	фискальная память
5.7 = 1	резерв
5.6 = 1 	резерв
5.5 = 1 	Фискальной и заводской номер запрограммированы
5.4 = 1	Налоговые ставки определены 
5.3 = 1  	Устройство фискализировано 
5.2 = 1 *	Не используется
5.1 = 1 	Фискальная память сформатирована 
5.0 = 1 *	Фискальная память установлена в режим Read Only.



*/