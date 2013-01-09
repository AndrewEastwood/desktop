using System;
using System.Collections.Generic;
using System.Text;

namespace components.Shared.Enums
{

    public enum uiComponents : int
    {
        All = 0xFF,
        Menus = 0x00,
        Labels = 0x01,
        Widgets = 0x02,
        Colors = 0x04,
        InformersType1 = 0x08,
        InformersType2 = 0x10,
        InformersType3 = 0x20,
        Components = 0x40,
        AppWindow = 0x80,
        InformersAll = (InformersType1 | InformersType2 | InformersType3)
    }
}
