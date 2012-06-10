using System;
using System.Collections.Generic;
using System.Text;

namespace FPService.Protocol
{
    public class ErrorFlags
    {
        //Total flags
        private const byte TOT = 20;
        //flags
        private string[] _flags = new string[TOT]
        { 
            "sale",
            "paymoney", 
            "discount", 
            "chqGetNom", 
            "getStatus",
            "","","","","","","","","","","","","","",""
        };
        private bool[] _state = new bool[TOT];

        public bool this[string name]
        {
            set
            {
                for (int i = 0; i < _flags.Length; i++)
                    if (_flags[i].ToUpper() == name.ToUpper())
                    {
                        _state[i] = value;
                        return;
                    }
            }
            get
            {
                for (int i = 0; i < _flags.Length; i++)
                    if (_flags[i].ToUpper() == name.ToUpper())
                        return _state[i];
                return false;
            }
        }

        public bool this[int index]
        {
            set
            {
                if (index > TOT)
                    throw new Exception("Індекс має бути меньшим за " + TOT);
                if (index < 0)
                    throw new Exception("Індекс не може бути меньшим за 0");

                _state[index] = value;
            }
            get
            {
                if (index > TOT)
                    throw new Exception("Індекс має бути меньшим за " + TOT);
                if (index < 0)
                    throw new Exception("Індекс не може бути меньшим за 0");

                return _state[index];
            }
        }

        public void Reset()
        {
            _state = new bool[TOT];
        }
    }
}
