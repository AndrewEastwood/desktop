using System;
using System.Collections.Generic;
using System.Text;
/* internal */
using driver.Config;

namespace driver.Lib
{
    public class MathLib
    {
        public static double GetDouble(object value)
        {
            if (value == DBNull.Value)
                return 0.0;
            try
            {
                //int ndsCount = 0;
                string val = value.ToString().Trim();
                for (int i = 0; i < val.Length; i++)
                    if (!Char.IsDigit(val, i))
                    {
                        if (val[i] == '-' || val[i] == '+') continue;
                        val = val.Replace(val[i].ToString(), System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
                        //ndsCount++;
                    }

                //if (ndsCount > 0)
                //    val = val.Replace(System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator, string.Empty);

                return double.Parse(val);
            }
            catch { return 0.0; }
        }//ok
        public static double GetRoundedMoney(double value)
        {
            value = Math.Round(value, driver.Config.ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals, MidpointRounding.AwayFromZero);
            /*
            for (int i = AppConfig.APP_MoneyDecimals + 1; i >= AppConfig.APP_MoneyDecimals; i--)
                value = Math.Round(value, i, MidpointRounding.AwayFromZero);*/
            return value;
        }//ok
        public static double GetRoundedDose(double value)
        {
            for (int i = driver.Config.ConfigManager.Instance.CommonConfiguration.APP_DoseDecimals + 2; i >= driver.Config.ConfigManager.Instance.CommonConfiguration.APP_DoseDecimals; i--)
                value = Math.Round(value, i, MidpointRounding.AwayFromZero);
            return value;
        }//ok
        public static string ReplaceNDS(string source, string replacingValue)
        {
            return source.Replace(System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator, replacingValue);
        }//ok
        public static string ReplaceValueByNDS(string source, string value)
        {
            return source.Replace(value, System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
        }//ok


    }
}
