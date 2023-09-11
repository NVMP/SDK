using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NVMP
{
    /// <summary>
    /// Contains static NativeServer properties and methods for querying internal clocks, internal state operations, and 
    /// update graph related operations.
    /// </summary>
    public static class NativeServer
    {
        #region Natives
        [DllImport("Native", EntryPoint = "GameServer_GetDeltaTime")]
        private static extern float Internal_GetDeltaTime();

        [DllImport("Native", EntryPoint = "GameServer_GetTargetFrequency")]
        private static extern float Internal_GetTargetFrequency();

        [DllImport("Native", EntryPoint = "GameServer_GetIsInPluginMainUpdate")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_GetIsInPluginMainUpdate();

        [DllImport("Native", EntryPoint = "GameServer_GetIsInFrameSleepUpdate")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_GetIsInFrameSleepUpdate();

        [DllImport("Native", EntryPoint = "GameServer_GetIsInJobDispatchUpdate")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_GetIsInJobDispatchUpdate();
        #endregion

        /// <summary>
        /// The current measurement of the main thread's update delta time. This is safe to access across multiple threads, however
        /// should only be used in the context of main thread updates (for example, worker threads that are synchronised against the main
        /// thread update). 
        /// </summary>
        public static float DeltaTime => Internal_GetDeltaTime();

        /// <summary>
        /// Returns if the server is currently in a main thread update for all .NET plugins, or false if the server
        /// is doing other time critical updates such as dispatching sync updates.
        /// </summary>
        public static bool IsInPluginMainUpdate => Internal_GetIsInPluginMainUpdate();

        /// <summary>
        /// Returns if the server is currently in the sleep portion of its update, where no update operations are expected to
        /// be running, and the server is waiting for the next frame to complete.
        /// </summary>
        public static bool IsInFrameSleepUpdate => Internal_GetIsInFrameSleepUpdate();

        /// <summary>
        /// Returns if the server is currently in the job dispatch portion of its update, where all cores running worker threads
        /// will be processing large quantities of logic. It is good practice to avoid running heavy thread operations if this
        /// returns true, as all cores are expected to meet the demand of the native server.
        /// </summary>
        public static bool IsInJobDispatchUpdate => Internal_GetIsInJobDispatchUpdate();

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
