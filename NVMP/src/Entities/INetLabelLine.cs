using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace NVMP.Entities
{
    public interface INetLabelLine
    {
        public Color Color { get; set; }
        public string Text { get; set; }
    }
}
