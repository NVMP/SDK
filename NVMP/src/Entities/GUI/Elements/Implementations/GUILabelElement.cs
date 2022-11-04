using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NVMP.Entities.GUI
{
    public class GUILabelElement : GUIBaseElement, IGUILabelElement
    {
        [DllImport("Native", EntryPoint = "GUI_LabelElement_SetText")]
        internal static extern void Internal_GUI_LabelElement_SetText(IntPtr message, string text);

        public override GUIItemType ItemType => GUIItemType.Label;

        public string Text { get; internal set; }

        public GUILabelElement WithText(string text)
        {
            Text = text;
            return this;
        }

        public new void ConfigureNative(IntPtr native)
        {
            base.ConfigureNative(native);

            Internal_GUI_LabelElement_SetText(native, Text);
        }
    }

}
