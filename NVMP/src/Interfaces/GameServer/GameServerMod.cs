using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP
{
    /// <summary>
    /// A client-modification information
    /// </summary>
    public class GameServerMod
    {
        /// <summary>
        /// File path relative on disk
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Name of the mod
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The computed digest to check integrity against
        /// </summary>
        public string Digest { get; set; }

        /// <summary>
        /// The mod index
        /// </summary>
        public uint Index { get; set; }
    }

}
