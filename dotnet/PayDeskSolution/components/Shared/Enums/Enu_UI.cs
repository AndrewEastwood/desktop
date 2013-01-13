using System;
using System.Collections.Generic;
using System.Text;

namespace components.Shared.Enums
{

    public enum uiComponents : int
    {
        All = 0x00FF,
        Menus = 0x0000,
        Labels = 0x0001,
        Widgets = 0x0002,
        Colors = 0x0004,
        InformersType1 = 0x0008, // 0000.0000|0000.1000
        InformersType2 = 0x0010,
        InformersType3 = 0x0020,
        Components = 0x0040,
        AppWindow = 0x0080, // 0000.0000|1000.0000
        ControlsType1 = 0x0100,
        ControlsType2 = 0x0200,
        InformersAll = (InformersType1 | InformersType2 | InformersType3)
    }
}
