using System;
using System.Collections.Generic;
using System.Text;

namespace driver.Common
{
    public class Enums
    {

        public enum DataContainerType : int
        {
            Table = 0,
            DataSet = 1
        }

        public enum PrinterType : int
        {
            OrderLegal = 0,
            OrderNormal = 1,
            BillCustomer = 2,
            BillKitchen = 3,
            ReportBills = 4,
            ReportOrders = 5
        }

        public static Dictionary<string, string> Printers
        {
            get
            {
                Dictionary<string, string> pt = new Dictionary<string, string>();

                pt.Add("OrderLegal", "Фіскальний");
                pt.Add("OrderNormal", "Чек");
                pt.Add("BillCustomer", "Рахунок");
                pt.Add("BillKitchen", "Кухня (різниця товарів)");
                pt.Add("ReportBills", "Звіт Рахунків");
                pt.Add("ReportCheques", "Звіт Чеків");

                return pt;
            }
        }
    }
}
