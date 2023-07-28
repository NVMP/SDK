using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NVMP
{
    public static class ServerTiming
    {

        [DllImport("Native", EntryPoint = "GameServer_GetDeltaTime")]
        internal static extern float Internal_GetDeltaTime();

        [DllImport("Native", EntryPoint = "GameServer_GetTargetFrequency")]
        internal static extern float Internal_GetTargetFrequency();

        /// <summary>
        /// The current measurement of the main thread's update delta time. This is safe to access across multiple threads, however
        /// should only be used in the context of main thread updates (for example, worker threads that are synchronised against the main
        /// thread update). 
        /// </summary>
        public static float DeltaTime => Internal_GetDeltaTime();

        /// <summary>
        /// The current target frequency the server is running at. This may not be the same as the delta time if an update takes longer,
        /// however the delta time will be no shorter than the target frequency, as the thread is yielded.
        /// </summary>
        public static float TargetFrequency => Internal_GetTargetFrequency();

        /// <summary>
        /// Returns a ratio of used delta update against the target frequency. This dynamically scales if the server
        /// is under strain and not meeting the frequency budget.
        /// </summary>
        public static float FrequencyRatio => DeltaTime / TargetFrequency;
    }
}
