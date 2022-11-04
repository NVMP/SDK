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
        [DllImport("Native", EntryPoint = "GUI_BaseElement_SetItemWidth")]
        internal static extern void Internal_GUI_BaseElement_SetItemWidth(IntPtr message, float itemWidth);
        [DllImport("Native", EntryPoint = "GUI_BaseElement_SetElementType")]
        internal static extern void Internal_GUI_BaseElement_SetElementType(IntPtr message, GUIItemType utype);
        [DllImport("Native", EntryPoint = "GUI_BaseElement_SetElementID")]
        internal static extern void Internal_GUI_BaseElement_SetElementID(IntPtr message, ulong ID);
        //
        [DllImport("Native", EntryPoint = "GUI_Element_AddSubElement")]
        internal static extern IntPtr Internal_GUI_Element_AddSubElement(IntPtr element);
        #endregion

        public Color ForegroundColor { get; internal set; } = Color.White;

        public Color BackgroundColor { get; internal set; } = Color.White;

        public bool IsSameLine { get; internal set; } = false;

        public float ItemWidth { get; internal set; } = 0.0f;

        public IGUIWindowTemplate ParentWindow { get; internal set; }

        public IList<IGUIBaseElement> Children { get; internal set; }

        public abstract GUIItemType ItemType { get; }

        public ulong ID { get; internal set; }

        public GUIBaseElement()
        {
            Children = new List<IGUIBaseElement>();
        }

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

        /// <summary>
        /// Sets the item width relative to the window. Setting this to -1.0f will fill the width.
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public GUIBaseElement WithItemWidth(float width)
        {
            ItemWidth = width;
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
            Internal_GUI_BaseElement_SetElementID(native, ID);
            Internal_GUI_BaseElement_SetForegroundColor(native, (uint)ForegroundColor.ToArgb());
            Internal_GUI_BaseElement_SetBackgroundColor(native, (uint)BackgroundColor.ToArgb());
            Internal_GUI_BaseElement_SetIsSameLine(native, IsSameLine);
            Internal_GUI_BaseElement_SetItemWidth(native, ItemWidth);

            // children
            foreach (var child in Children)
            {
                var subelement = Internal_GUI_Element_AddSubElement(native);
                child.ConfigureNative(subelement);
            }
        }
    }

}
