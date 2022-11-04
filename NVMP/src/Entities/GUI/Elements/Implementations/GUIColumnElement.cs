using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NVMP.Entities.GUI
{
    public class GUIColumnElement : GUIBaseElement, IGUIColumnElement
    {
        [DllImport("Native", EntryPoint = "GUI_ColumnElement_SetWidth")]
        internal static extern void Internal_GUI_ColumnElement_SetWidth(IntPtr message, float width);

        public override GUIItemType ItemType => GUIItemType.Column;

        public float Width { get; internal set; }

        public new void ConfigureNative(IntPtr native)
        {
            base.ConfigureNative(native);

            Internal_GUI_ColumnElement_SetWidth(native, Width);
        }
    }
}
