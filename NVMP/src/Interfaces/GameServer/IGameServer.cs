using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NVMP
{
    /// <summary>
    /// The primary interface for game server logic. This can manage and maintain the server's configuraiton and mod definitions.
    /// It is undefined to have multiple instances of IGameServer in code, so only use one and use it wisely.
    /// </summary>
    public interface IGameServer
    {

        [DllImport("Native", EntryPoint = "GameServer_GetUMT")]
        internal static extern int Internal_GetUMT();

        /// <summary>
        /// Initializes all mods
        /// </summary>
        public void Init();

        /// <summary>
        /// Is the server running in hardcore mode
        /// </summary>
        public bool IsHardcore { get; set; }

        /// <summary>
        /// What difficulty is the server running at 
        /// </summary>
        public GameServerDifficulty Difficulty { get; set; }

        static public GameServerUnrestrictedModeType UnrestrictedMode
        {
            get
            {
                return (GameServerUnrestrictedModeType)Internal_GetUMT();
            }
        }
    }

}
