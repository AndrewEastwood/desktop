using System;
using System.Collections.Generic;
using System.Text;

namespace FPService.Protocol.Maria301MT
{
    class Maria301MT : Protocol
    {
        public Maria301MT()
        {
            Tree t = new Tree();
            Protocol_Name = "Maria301MT";
            FP_Panel = t;
        }

        internal override object[] CallFunction(string name, string description, ComPort port, object[] param)
        {
            object[] value = new object[0];


            return value;
        }
    }
}
