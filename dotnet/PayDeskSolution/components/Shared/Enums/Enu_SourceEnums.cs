using System;
using System.Collections.Generic;
using System.Text;

namespace components.Shared.Enums
{
    public class Enu_SourceEnums
    {
        public enum pdStructureType : int
        {
            BILL = 0,
            ORDER = 1,
            CALCULATION = 2,
            STATEMENTS = 3
        }

        public enum pdDataItemType : int
        {
            Any = -1,
            None = 0,
            Product = 1,
            Alternative = 2,
            Client = 3,
            Order = 4
        }

    }
}
