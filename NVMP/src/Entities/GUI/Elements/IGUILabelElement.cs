using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NVMP.Entities.GUI
{
    public interface IGUILabelElement : IGUIBaseElement
    {
        /// <summary>
        /// The text label
        /// </summary>
        public string Text { get; }
    }

}
