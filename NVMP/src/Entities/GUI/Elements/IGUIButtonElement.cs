using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NVMP.Entities.GUI
{
    public interface IGUIButtonElement : IGUIBaseElement
    {
        /// <summary>
        /// The text label inside the button
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Fired when a player clicks this button
        /// </summary>
        public Action<INetPlayer> OnClicked { get; }
    }

}
