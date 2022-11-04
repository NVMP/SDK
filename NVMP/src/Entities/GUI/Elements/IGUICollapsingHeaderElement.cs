using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NVMP.Entities.GUI
{
    public interface IGUICollapsingHeaderElement : IGUIBaseElement
    {
        /// <summary>
        /// The header text label
        /// </summary>
        public string HeaderText { get; }

        /// <summary>
        /// ImGui flags for how the header is rendered
        /// </summary>
        public ImGuiTreeNodeFlags TreeNodeFlags { get; }
    }
}
