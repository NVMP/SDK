using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace NVMP.Entities.GUI
{
    public enum GUIItemType
    {
        Invalid          = 0,
        Label            = 1,
        TextInput        = 2,
        Button           = 3,
        CollapsingHeader = 4,
        
        Row              = 5,
        Column           = 6,
        Seperator        = 7,
    };

}
