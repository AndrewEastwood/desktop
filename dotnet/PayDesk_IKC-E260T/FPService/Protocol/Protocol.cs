using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FPService.Protocol
{
    abstract class Protocol
    {
        //FP Name
        public string Protocol_Name = "";

        //FP Panel
        public System.Windows.Forms.UserControl FP_Panel;

        //Data
        public byte[] InputData;
        public byte[] OutputData;
        public byte[] DataForSend;
        public uint ReadedBytes;

        //Error Flags
        public ErrorFlags Errors = new ErrorFlags();

        //LastFunctonName
        public string lastFuncName = "";

        //PublicFunctions
        public string[][] ProtocolPublicFunctions;

        internal abstract object[] CallFunction(string name, string description, ComPort port, object[] param);
    }
}
