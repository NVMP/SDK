using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NVMP.Entities.GUI
{
    public abstract class GUIBaseElement : IGUIBaseElement
    {
        #region Natives
        //
        // Element Modification
        //
        [DllImport("Native", EntryPoint = "GUI_BaseElement_SetForegroundColor")]
        internal static extern void Internal_GUI_BaseElement_SetForegroundColor(IntPtr message, uint rgba);
        [DllImport("Native", EntryPoint = "GUI_BaseElement_SetBackgroundColor")]
        internal static extern void Internal_GUI_BaseElement_SetBackgroundColor(IntPtr message, uint rgba);
        [DllImport("Native", EntryPoint = "GUI_BaseElement_SetIsSameLine")]
        internal static extern void Internal_GUI_BaseElement_SetIsSameLine(IntPtr message, bool isSameLine);
        [DllImport("Native", EntryPoint = "GUI_BaseElement_SetElementType")]
        internal static extern void Internal_GUI_BaseElement_SetElementType(IntPtr message, GUIItemType utype);
        //
        [DllImport("Native", EntryPoint = "GUI_Element_AddSubElement")]
        internal static extern IntPtr Internal_GUI_Element_AddSubElement(IntPtr element);
        #endregion

        public Color ForegroundColor { get; internal set; } = Color.White;

        public Color BackgroundColor { get; internal set; } = Color.White;

        public bool IsSameLine { get; internal set; } = false;

        public IGUIWindowTemplate ParentWindow { get; internal set; }

        public IList<IGUIBaseElement> Children { get; internal set; } = new List<IGUIBaseElement>();

        public abstract GUIItemType ItemType { get; }

        public ulong ID { get; internal set; }

        public GUIBaseElement WithElements(Action<GUIWindowElementBuilder> builder)
        {
            builder(new GUIWindowElementBuilder(Children, ParentWindow));
            return this;
        }

        public GUIBaseElement WithForeground(Color col)
        {
            ForegroundColor = col;
            return this;
        }

        public GUIBaseElement SameLine()
        {
            IsSameLine = true;
            return this;
        }

        public GUIBaseElement WithBackground(Color col)
        {
            BackgroundColor = col;
            return this;
        }

        public void ConfigureNative(IntPtr native)
        {
            // basic stuff
            Internal_GUI_BaseElement_SetElementType(native, ItemType);
            Internal_GUI_BaseElement_SetForegroundColor(native, (uint)ForegroundColor.ToArgb());
            Internal_GUI_BaseElement_SetBackgroundColor(native, (uint)BackgroundColor.ToArgb());
            Internal_GUI_BaseElement_SetIsSameLine(native, IsSameLine);

            // children
            foreach (var child in Children)
            {
                var subelement = Internal_GUI_Element_AddSubElement(native);
                child.ConfigureNative(subelement);
            }
        }
    }

}
