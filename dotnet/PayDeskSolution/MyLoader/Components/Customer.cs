using System;
using System.Collections.Generic;
using System.Text;

namespace MyLoader.Components
{
    [Serializable]
    public class Customer
    {
        //public int id = 0;
        public string name = "";
        public DateTime registrationDate = DateTime.Now;
        public string clientCode = "";
        public string registerCode = "";
        public string appType = "";
        public string customerType = "";
        public string deskNumber = "";
        public string comment = "";
    }
}