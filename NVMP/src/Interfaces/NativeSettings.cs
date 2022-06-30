using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NVMP
{
    /// <summary>
    /// Reads and writes to the server.cfg file for persistent setting storage
    /// </summary>
    public static class NativeSettings
    {
        /// <summary>
        /// Returns a settings value as a float
        /// </summary>
        /// <param name="group"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [DllImport("Native", EntryPoint = "GameSettings_GetFloatValue")]
        public static extern float GetFloatValue(string group, string name);

        /// <summary>
        /// Returns a settings value as a bool
        /// </summary>
        /// <param name="group"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [DllImport("Native", EntryPoint = "GameSettings_GetBoolValue")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GetBoolValue(string group, string name);

        /// <summary>
        /// Returns a settings value as a string
        /// </summary>
        /// <param name="group"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [DllImport("Native", EntryPoint = "GameSettings_GetStringValue")]
        public static extern string GetStringValue(string group, string name);


        /// <summary>
        /// Sets a settings value as a float
        /// </summary>
        /// <param name="group"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport("Native", EntryPoint = "GameSettings_SetFloatValue")]
        public static extern void SetFloatValue(string group, string name, float value);

        /// <summary>
        /// Sets a settings value as a bool
        /// </summary>
        /// <param name="group"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        [DllImport("Native", EntryPoint = "GameSettings_SetBoolValue")]
        public static extern void SetBoolValue(string group, string name, bool value);

        /// <summary>
        /// Sets a settings value as a string
        /// </summary>
        /// <param name="group"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        [DllImport("Native", EntryPoint = "GameSettings_SetStringValue")]
        public static extern void SetStringValue(string group, string name, string value);

        /// <summary>
        /// Sets a settings default value in the configuration file as a string
        /// </summary>
        /// <param name="group"></param>
        /// <param name="name"></param>
        /// <param name="initialValue"></param>
        [DllImport("Native", EntryPoint = "GameSettings_SetupDefaultString")]
        public static extern void SetupDefaultString(string group, string name, string initialValue);

        /// <summary>
        /// Sets a settings default value in the configuration file as a float
        /// </summary>
        /// <param name="group"></param>
        /// <param name="name"></param>
        /// <param name="initialValue"></param>
        [DllImport("Native", EntryPoint = "GameSettings_SetupDefaultFloat")]
        public static extern void SetupDefaultFloat(string group, string name, float initialValue);

        /// <summary>
        /// Sets a settings default value in the configuration file as a bool
        /// </summary>
        /// <param name="group"></param>
        /// <param name="name"></param>
        /// <param name="initialValue"></param>
        [DllImport("Native", EntryPoint = "GameSettings_SetupDefaultBool")]
        public static extern void SetupDefaultBool(string group, string name, bool initialValue);
    }
}
