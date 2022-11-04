using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NVMP.Entities.GUI
{
    public class GUIButtonElement : GUIBaseElement, IGUIButtonElement
    {
        [DllImport("Native", EntryPoint = "GUI_ButtonElement_SetText")]
        internal static extern void Internal_GUI_ButtonElement_SetText(IntPtr message, string text);

        public override GUIItemType ItemType => GUIItemType.Button;

        public string Text { get; internal set; }

        public Action<INetPlayer> OnClicked { get; internal set; }

        public GUIButtonElement OnClick(Action<INetPlayer> click)
        {
            OnClicked = click;
            return this;
        }

        public GUIButtonElement WithText(string text)
        {
            Text = text;
            return this;
        }

        public new void ConfigureNative(IntPtr native)
        {
            base.ConfigureNative(native);

            Internal_GUI_ButtonElement_SetText(native, Text);
        }
    }

}
