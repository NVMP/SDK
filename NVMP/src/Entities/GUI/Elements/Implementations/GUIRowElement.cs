using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NVMP.Entities.GUI
{
    public class GUIRowElement : GUIBaseElement, IGUIRowElement
    {
        public override GUIItemType ItemType => GUIItemType.Row;
    }
}
