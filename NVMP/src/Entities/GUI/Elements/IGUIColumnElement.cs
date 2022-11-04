using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NVMP.Entities.GUI
{
    public interface IGUIColumnElement : IGUIBaseElement
    {
        /// <summary>
        /// Optional width allocated to the column
        /// </summary>
        public float Width { get; }
    }
}
