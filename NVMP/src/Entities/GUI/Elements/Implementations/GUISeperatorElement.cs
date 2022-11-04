using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NVMP.Entities.GUI
{
    public class GUISeperatorElement : GUIBaseElement, IGUISeperatorElement
    {
        public override GUIItemType ItemType => GUIItemType.Seperator;
    }
}
