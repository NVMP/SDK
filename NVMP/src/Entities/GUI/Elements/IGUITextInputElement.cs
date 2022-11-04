using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NVMP.Entities.GUI
{
    public interface IGUITextInputElement : IGUIBaseElement
    {
        /// <summary>
        /// The input default text
        /// </summary>
        public string Text { get; }
    }
}
