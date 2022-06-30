using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NVMP
{
    public static class NativeBuildDetails
    {
        [DllImport("Native", EntryPoint = "GetBuildNetworkVersion")]
        internal static extern string Internal_GetBuildNetworkVersion();

        [DllImport("Native", EntryPoint = "GetBuildConnectionHeader")]
        internal static extern uint Internal_GetBuildConnectionHeader();

        /// <summary>
        /// Returns the static connection header the server will use to lock off incoming connections. This is not 
        /// the version of the build, as versions may increment without locking off a player from connecting.
        /// </summary>
        static public uint ConnectionHeader => Internal_GetBuildConnectionHeader();

        /// <summary>
        /// Returns the static build version the server was built with. This is the version the server was built with in release, and
        /// reflects against clients whenever a new version is composed.
        /// </summary>
        static public string BuildVersion => Internal_GetBuildNetworkVersion();
    }
}
