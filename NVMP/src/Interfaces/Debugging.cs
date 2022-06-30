using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NVMP
{
    /// <summary>
    /// Debug helpers that write directly to the NV:MP console window, and log file, with automatic severity display. 
    /// </summary>
    public static class Debugging
    {
        #region Natives
        [DllImport("Native", EntryPoint = "Debugging_Write")]
        internal static extern void Debugging_Write(string message);

        [DllImport("Native", EntryPoint = "Debugging_Warn")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Debugging_Warn(string message);

        [DllImport("Native", EntryPoint = "Debugging_Error")]
        internal static extern void Debugging_Error(string message);
        #endregion

        #region Internal
        internal static List<string> Contexts = new List<string>();
        #endregion

        public static void PushContext(string context)
        {
            lock (Contexts)
            {
                Contexts.Add(context);
            }

        }

        public static void PopContext()
        {
            lock (Contexts)
            {
                Contexts.RemoveAt(Contexts.Count - 1);
            }
        }

        internal static string ContextString
        {
            get
            {
                lock (Contexts)
                {
                    if (Contexts.Count == 0)
                        return "";

                    return String.Join("", Contexts.Select(ctx => $"[{ctx}]")) + " ";
                }
            }
        }

        public static void Write(string message)
        {
            Debugging_Write(ContextString + message);
        }

        public static void Warn(string message)
        {
            Debugging_Warn(ContextString + message);
        }

        public static void Error(string message)
        {
            Debugging_Error(ContextString + message);
        }

        public static void Error(Exception e)
        {
            string[] lines = e.ToString().Split("\n");
            foreach (var line in lines)
            {
                Debugging.Error(line);
            }
        }
    }
}