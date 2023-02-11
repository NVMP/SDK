using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NVMP.Entities
{
    internal class NetWorldSpace : NetUnmanaged, INetWorldSpace
    {
        #region Natives
        [DllImport("Native", EntryPoint = "GameNetWorldSpace_GetWorldSpaceFormID")]
        private static extern uint Internal_GetWorldSpaceFormID(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetWorldSpace_GetIsTransient")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_GetIsTransient(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetWorldSpace_SetIsTransient")]
        private static extern void Internal_SetIsTransient(IntPtr self, bool isTransient);

        // Weather
        [DllImport("Native", EntryPoint = "GameNetWorldSpace_SetWeatherCurrent")]
        private static extern void Internal_SetWeatherCurrent(IntPtr self, uint weather);

        [DllImport("Native", EntryPoint = "GameNetWorldSpace_GetWeatherCurrent")]
        private static extern uint Internal_GetWeatherCurrent(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetWorldSpace_SetWeatherPrevious")]
        private static extern void Internal_SetWeatherPrevious(IntPtr self, uint weather);

        [DllImport("Native", EntryPoint = "GameNetWorldSpace_GetWeatherPrevious")]
        private static extern uint Internal_GetWeatherPrevious(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetWorldSpace_SetWeatherOverride")]
        private static extern void Internal_SetWeatherOverride(IntPtr self, uint weather);

        [DllImport("Native", EntryPoint = "GameNetWorldSpace_GetWeatherOverride")]
        private static extern uint Internal_GetWeatherOverride(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetWorldSpace_SetTime")]
        private static extern void Internal_SetTime(IntPtr self, float hourTime);

        [DllImport("Native", EntryPoint = "GameNetWorldSpace_GetTime")]
        private static extern float Internal_GetTime(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetWorldSpace_GetTimeScale")]
        private static extern float Internal_GetTimeScale(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetWorldSpace_SetTimeScale")]
        private static extern void Internal_SetTimeScale(IntPtr self, float timeScale);

        #endregion

        public WorldspaceType FormID
        {
            get => (WorldspaceType)Internal_GetWorldSpaceFormID(__UnmanagedAddress);
        }

        public bool IsTransient
        {
            get => Internal_GetIsTransient(__UnmanagedAddress);
            set => Internal_SetIsTransient(__UnmanagedAddress, value);
        }

        public uint WeatherCurrent
        {
            get => Internal_GetWeatherCurrent(__UnmanagedAddress);
            set => Internal_SetWeatherCurrent(__UnmanagedAddress, value);
        }

        public uint WeatherPrevious
        {
            get => Internal_GetWeatherPrevious(__UnmanagedAddress);
            set => Internal_SetWeatherPrevious(__UnmanagedAddress, value);
        }

        public uint WeatherOverride
        {
            get => Internal_GetWeatherOverride(__UnmanagedAddress);
            set => Internal_SetWeatherOverride(__UnmanagedAddress, value);
        }

        public DateTime Time
        {
            get
            {
                float fCurrentTime = Internal_GetTime(__UnmanagedAddress);

                var result = new DateTime(1, 1, 1
                    , (int)fCurrentTime
                    , (int)(60.0f * (fCurrentTime % 1.0f))
                    , 0);

                return result;
            }

            set
            {
                float result = (float)value.Hour;
                result += (value.Minute / 60.0f);
                Internal_SetTime(__UnmanagedAddress, result);
            }
        }

        public void TransitionToWeather(uint targetWeatherID)
        {
            WeatherOverride = uint.MinValue;
            WeatherPrevious = WeatherCurrent;
            WeatherCurrent = targetWeatherID;
        }
    }
}
