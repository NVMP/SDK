using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace NVMP.Entities
{
    /// <summary>
    /// NetLabels are a reference that supports normal network reference functionality (minus an actual TESObjectREFR), but allows you to 
    /// draw more interesting text in it's 3D location. Title and Name is still supported, but labels draw underneath.
    /// </summary>
    public interface INetLabel : INetReference
    {
        /// <summary>
        /// Label lines currently added to this label.
        /// </summary>
        public INetLabelLineStack Labels { get; set; }
    }
}
