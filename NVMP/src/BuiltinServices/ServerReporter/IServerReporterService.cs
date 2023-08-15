using System;

namespace NVMP.BuiltinServices
{
    /// <summary>
    /// A built in service that allows you to report your server to the public server list. Please read the documentation carefully
    /// when using various properties, as the server list server can reject your broadcast for unspecified reasons - or temporarily
    /// time you out from broadcasting. So be gentle.
    /// </summary>
    public interface IServerReporterService : IDisposable
    {
        #region Constants
        /// <summary>
        /// Max server name size in number of characters.
        /// </summary>
        public static int MaxServerNameSize = 64;

        /// <summary>
        /// Max server descriptionsize in number of characters.
        /// </summary>
        public static int MaxServerDescriptionSize = 128;
        #endregion

        /// <summary>
        /// The public display name for your server. You must not change this significantly and frequently, else your
        /// server (unique ID and IP) may be temporarily timed out from broadcast until the name is settled. 
        /// This cannot exceed MaxServerNameSize, and will throw an exception if too large - or the server reporter will temporarily
        /// time you out.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The public display description for your server. This cannot exceed
        /// This cannot exceed MaxServerDescriptionSize, and will throw an exception if too large - or the server reporter will temporarily
        /// time you out.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Defines how many reserved slots are allocated. This is not processed by any serverlist logic, other than 
        /// how many players are reported. If you have a maximum amount of 100 peers, but have 3 reserved slots, then
        /// only 97 slots will be shown as the max player count.
        /// This will then spill over if exceeded, and you must define how this is done in your gameserver code.
        /// </summary>
        public uint ReservedSlots { get; set; }

        /// <summary>
        /// Defines a fast download URL to replace the mod service provider URL. This allows you to offload all mod downloads to a central
        /// CDN or cached server, provided that the remote server has identical mod revisions to the local server.
        /// </summary>
        public string FastDownloadURL { get; set; }

        /// <summary>
        /// The maximum slots/game sockets on the server. This is set via the server configuration, but is available here to query quickly.
        /// </summary>
        public uint MaxSlots { get; }
    }
}
