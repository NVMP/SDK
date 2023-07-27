using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Entities
{
    /// <summary>
    /// Defines a sound request passed to a player, or assigned to a reference.
    /// </summary>
    public struct NetSoundRequest
    {
        /// <summary>
        /// Loops the sound until stopped manually, or the sound is cleared from the target.
        /// </summary>
        public bool IsLooping { get; set; }

        /// <summary>
        /// Sets if the sound is to be in 3D space, and not played from screenspace. 
        /// </summary>
        public bool Is3D { get; set; }

        /// <summary>
        /// Applies a random frequency shift to the audio on playback, to give uniqueness if desired.
        /// </summary>
        public bool HasRandomFrequencyShift { get; set; }

        /// <summary>
        /// The absolute filename to use for playback. This is relative to the Data\ folder, so do not include it
        /// as part of the filename. You can also specify a folder name of sounds in which a random sound is selected.
        /// </summary>
        public string FileName { get; set; }
    }
}
