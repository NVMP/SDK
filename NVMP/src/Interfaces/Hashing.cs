using System.Runtime.InteropServices;

namespace NVMP
{
    /// <summary>
    /// Utility functions for converting strings to 4 byte hashes. This is kept in sync with the native hashing implementation, so you
    /// can use this for native communication - or just general purpose hashing.
    /// 
    /// It is loosely based on djb2 hashing. 
    /// </summary>
    public static class Hashing
    {
        /// <summary>
        /// Builds an NV:MP compatible hash with the desired text passed.
        /// Unicode is NOT supported, only ANSI. This is a limitation to our engine, however if you must compute
        /// a hash from WCHAR/unicode, then use ComputeUnicodeUnsafe.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [DllImport("Native", EntryPoint = "Hash_Compute", CharSet = CharSet.Ansi)]
        public static extern uint Compute(string str);

        /// <summary>
        /// Builds a hash with the desired text passed using the same algorithm the Ansi compute has.
        /// This is not guarenteed to be compatible with Compute that uses Ansi, so you should not use this for gameplay checks or hash building elsewhere.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [DllImport("Native", EntryPoint = "Hash_Compute_Uni", CharSet = CharSet.Unicode)]
        public static extern uint ComputeUnicodeUnsafe(string str);
    }
}
