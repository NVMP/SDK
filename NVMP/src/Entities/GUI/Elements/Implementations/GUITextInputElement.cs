using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NVMP.Entities.GUI
{
    public class GUITextInputElement : GUIBaseElement, IGUITextInputElement
    {
        [DllImport("Native", EntryPoint = "GUI_TextInput_SetText")]
        internal static extern void Internal_GUI_TextInput_SetText(IntPtr message, string text);

        public override GUIItemType ItemType => GUIItemType.TextInput;

        public string Text { get; internal set; }

        public GUITextInputElement WithText(string text)
        {
            Text = text;
            return this;
        }

        public new void ConfigureNative(IntPtr native)
        {
            base.ConfigureNative(native);

            Internal_GUI_TextInput_SetText(native, Text);
        }
    }

}
