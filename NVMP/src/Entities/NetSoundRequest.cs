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
        /// Sets if the sound is a one-shot, meaning it is meant to play once and then expire. Players who enter scope of the reference
        /// after the sound is played will never hear this sound, so if the sound is important for fidelity - one-shots are not recommended.
        ///
        /// Another benefit of one shots is that they can stack, whereas when not using a one-shot sound, it will replace the last playing sound on the 
        /// reference.
        /// </summary>
        public bool IsOneShot { get; set; }

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
