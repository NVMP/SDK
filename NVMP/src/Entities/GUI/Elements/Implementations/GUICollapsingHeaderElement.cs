using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NVMP.Entities.GUI
{
    public class GUICollapsingHeaderElement : GUIBaseElement, IGUICollapsingHeaderElement
    {
        [DllImport("Native", EntryPoint = "GUI_CollapsingHeader_SetHeaderText")]
        internal static extern void Internal_GUI_CollapsingHeader_SetHeaderText(IntPtr message, string text);

        [DllImport("Native", EntryPoint = "GUI_CollapsingHeader_SetTreeNodeFlags")]
        internal static extern void Internal_GUI_CollapsingHeader_SetTreeNodeFlags(IntPtr message, uint flags);

        public override GUIItemType ItemType => GUIItemType.CollapsingHeader;
        public string HeaderText { get; internal set; }
        public ImGuiTreeNodeFlags TreeNodeFlags { get; internal set; }

        public GUICollapsingHeaderElement WithHeaderText(string text)
        {
            HeaderText = text;
            return this;
        }

        public GUICollapsingHeaderElement WithTreeNodeFlags(ImGuiTreeNodeFlags flags)
        {
            TreeNodeFlags = flags;
            return this;
        }

        public new void ConfigureNative(IntPtr native)
        {
            base.ConfigureNative(native);

            Internal_GUI_CollapsingHeader_SetHeaderText(native, HeaderText);
            Internal_GUI_CollapsingHeader_SetTreeNodeFlags(native, (uint)TreeNodeFlags);
        }
    }
}
