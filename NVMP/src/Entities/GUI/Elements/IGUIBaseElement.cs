using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace NVMP.Entities.GUI
{
    public interface IGUIBaseElement
    {
        public ulong ID { get; }

        /// <summary>
        /// Rendered foreground color - if supported
        /// </summary>
        public Color ForegroundColor { get; }

        /// <summary>
        /// Rendered background color - if supported
        /// </summary>
        public Color BackgroundColor { get; }

        /// <summary>
        /// Is this element rendered on the same line as it's previous element
        /// </summary>
        public bool IsSameLine { get; }

        /// <summary>
        /// The width of the item. Defaulted at 0.0f. If set to -1.0f then will fill the parent container.
        /// </summary>
        public float ItemWidth { get; }

        public IGUIWindowTemplate ParentWindow { get; }

        /// <summary>
        /// A list of sub-elements to render - if supported, only some GUI concepts will allow you to render into them (such as 
        /// a collapsing header)
        /// </summary>
        public IList<IGUIBaseElement> Children { get; }

        /// <summary>
        /// Specific UI item type
        /// </summary>
        public GUIItemType ItemType { get; }

        /// <summary>
        /// Configures the native side of this element
        /// </summary>
        /// <param name="native"></param>
        public void ConfigureNative(IntPtr native);
    }

}
