using System;
using System.Collections.Generic;
using System.Text;
using components.Shared.Attributes;
using components.Shared.Interfaces;

namespace IKC_OP6
{
    [PluginSimpleAttribute(PluginType.FPDriver)]
    public class Plugin_IKC_OP6 : IKC_OP2.Plugin_IKC_OP2, IFPDriver
    {
        protected override void Hook_SendGetData_OnBusy()
        {
            base.Hook_SendGetData_OnBusy();
            base.ComPort.Write(base.InputData);
        }
    }
}
