using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace NVMP.Entities
{
    internal class NetLabel : NetReference, INetLabel
    {
        #region Natives
        // Line Internals
        [DllImport("Native", EntryPoint = "GameNetLabel_GetNumLabelLines")]
        private static extern uint Internal_GetNumLabelLines(IntPtr label);

        [DllImport("Native", EntryPoint = "GameNetLabel_GetAllLabelLines")]
        private static extern void Internal_GetAllLabelLines(IntPtr[] labelLines, uint containerSize);

        [DllImport("Native", EntryPoint = "GameNetLabel_PushLabelLine", CharSet = CharSet.Unicode)]
        private static extern IntPtr Internal_PushLabelLine(IntPtr label, [MarshalAs(UnmanagedType.LPWStr)] string title, byte red, byte green, byte blue);

        [DllImport("Native", EntryPoint = "GameNetLabel_PopLabelLine")]
        private static extern void Internal_PopLabelLine(IntPtr label);

        [DllImport("Native", EntryPoint = "GameNetLabel_ClearLabelLines")]
        private static extern void Internal_ClearLabelLines(IntPtr label);

        [DllImport("Native", EntryPoint = "GameNetLabel_GetLabelLineString", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        private static extern string Internal_GetLabelLineString(IntPtr label);

        [DllImport("Native", EntryPoint = "GameNetLabel_SetLabelLineString", CharSet = CharSet.Unicode)]
        private static extern void Internal_SetLabelLineString(IntPtr label, [MarshalAs(UnmanagedType.LPWStr)] string str);

        [DllImport("Native", EntryPoint = "GameNetLabel_GetLabelLineColor")]
        private static extern void Internal_GetLabelLineColor(IntPtr label, out byte red, out byte green, out byte blue);

        [DllImport("Native", EntryPoint = "GameNetLabel_SetLabelLineColor")]
        private static extern void Internal_SetLabelLineColor(IntPtr label, byte red, byte green, byte blue);
        // 
        #endregion

        public NetLabel()
        {
            Labels = new NativeLabelCollection(this);
        }

        public class Line : INetLabelLine
        {
            public Color Color
            {
                get 
                {
                    byte R;
                    byte G;
                    byte B;
                    Internal_GetLabelLineColor(__UnmanagedAddress, out R, out G, out B);

                    return Color.FromArgb(R, G, B);
                }
                set
                {
                    Internal_SetLabelLineColor(__UnmanagedAddress, value.R, value.G, value.B);
                }
            }

            public string Text
            {
                get => Internal_GetLabelLineString(__UnmanagedAddress);
                set => Internal_SetLabelLineString(__UnmanagedAddress, value);
            }

            internal IntPtr __UnmanagedAddress;
        }

        internal class NativeLabelCollection : ICollection, INetLabelLineStack
        {
            internal NetLabel Label;

            public NativeLabelCollection(NetLabel label)
            {
                Label = label;
            }

            internal IntPtr[] LabelLines
            {
                get
                {
                    // First grab all the native pointers
                    uint numLabels = Internal_GetNumLabelLines(Label.__UnmanagedAddress);

                    IntPtr[] labels = new IntPtr[numLabels];
                    Internal_GetAllLabelLines(labels, numLabels);

                    return labels;
                }
            }

            public int Count => (int)Internal_GetNumLabelLines(Label.__UnmanagedAddress);
            public bool IsSynchronized => false;
            public object SyncRoot => throw new NotImplementedException();

            public void Add(ref Line item)
            {
                item.__UnmanagedAddress = Internal_PushLabelLine(Label.__UnmanagedAddress, "", 255, 255, 255);
            }

            public IEnumerator GetEnumerator()
            {
                return LabelLines.GetEnumerator();
            }

            public INetLabelLine Push()
            {
                var el = new Line();
                Add(ref el);

                if (el.__UnmanagedAddress != IntPtr.Zero)
                {
                    return el;
                }

                return null;
            }

            public void Pop()
            {
                Internal_PopLabelLine(Label.__UnmanagedAddress);
            }

            public void Clear()
            {
                Internal_ClearLabelLines(Label.__UnmanagedAddress);
            }

            public bool Contains(INetLabelLine item)
            {
                foreach (var line in LabelLines)
                {
                    if (line == (item as Line).__UnmanagedAddress)
                        return true;
                }
                return false;
            }

            public void CopyTo(Array array, int arrayIndex)
            {
                var labels = LabelLines;
                if (array.Length - arrayIndex < labels.Length)
                    throw new ArgumentException("Array is too small!");

                int i = 0;
                foreach (var line in labels)
                {
                    array.SetValue(new Line { __UnmanagedAddress = line }, i + arrayIndex);
                }
            }
        }

        public INetLabelLineStack Labels { get; set; }
    }
}
