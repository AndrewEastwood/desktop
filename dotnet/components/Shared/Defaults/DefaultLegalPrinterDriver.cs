using System;
using System.Collections.Generic;
using System.Text;
using components.Shared.Attributes;
using components.Shared.Interfaces;
using System.Windows.Forms;
using System.Collections;
using components.Components.SerialPort;
using components.Lib;

namespace components.Shared.Defaults
{
    [PluginSimpleAttribute(PluginType.LegalPrinterDriver)]
    public class DefaultLegalPrinterDriver : DefaultPlugin//, ILegalPrinterDriver
    {

        private Com_SerialPort _port;
        private CoreLib _utils;
        // private Params _param;

        // 
        public virtual bool Activate()
        {
            return _port.Open();
        }

        public virtual bool Deactivate()
        {
            return _port.Close();
        }

        // Standart Legal Printer Commands
        protected virtual void FP_Sale(object[] param)
        {
            throw new Exception("Implement FP_Sale");
        }
        protected virtual void FP_PayMoney(object[] param)
        {
            throw new Exception("Implement FP_PayMoney");
        }
        protected virtual void FP_Payment(object[] param)
        {
            throw new Exception("Implement FP_Payment");
        }
        protected virtual void FP_Discount(object[] param)
        {
            throw new Exception("Implement FP_Discount");
        }
        protected virtual uint FP_LastChqNo(object[] param)
        {
            throw new Exception("Implement FP_LastChqNo");
        }
        protected virtual uint FP_LastZRepNo(object[] param)
        {
            throw new Exception("Implement FP_LastZRepNo");
        }
        protected virtual void FP_OpenBox()
        {
            throw new Exception("Implement FP_OpenBox");
        }
        protected virtual bool FP_SetCashier(object[] param)
        {
            throw new Exception("Implement FP_SetCashier");
        }
        protected virtual void FP_SendCustomer(object[] param)
        {
            throw new Exception("Implement FP_SendCustomer");
        }

        // Hooks

        protected virtual UserControl _hook_control_driverUI()
        {
            throw new Exception("Implement DriverUI component");
        }

        protected virtual UserControl _hook_control_portUI()
        {
            throw new Exception("Implement PortUI component");
        }

        protected virtual UserControl _hook_control_settingsUI()
        {
            throw new Exception("Implement SettingsUI component");
        }

        // = PROPERTIES
        public Com_SerialPort ComPort { get { return _port; } set { _port = value; } }
        public CoreLib Utils { get { return _utils; } }


        public object CallFunction(string name, params object[] param)
        {
            throw new NotImplementedException();
        }

        public Hashtable AllowedMethods
        {
            get { throw new NotImplementedException(); }
        }

        public UserControl DriverUI
        {
            get { throw new NotImplementedException(); }
        }

        public UserControl PortUI
        {
            get { throw new NotImplementedException(); }
        }

        public UserControl CompatibilityUI
        {
            get { throw new NotImplementedException(); }
        }
    }
}
