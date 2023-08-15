using System.Collections.Generic;

namespace NVMP.BuiltinServices
{
    /// <summary>
    /// Interface wrapper for the ModDownloadService, which adds mod files as binary content available for players to download
    /// on connection to your server. 
    /// </summary>
    public interface IModDownloadService
    {
        /// <summary>
        /// Returns all downloadable mods added to the mod download service.
        /// </summary>
        public IEnumerable<ModFile> DownloadableMods { get; }

        /// <summary>
        /// Returns the URL the service is providing to download mods from the game server.
        /// </summary>
        public string DownloadURL { get; }

        /// <summary>
        /// Registers a custom mod to the download service. This mod must be available in the Data folder of the server.
        /// </summary>
        /// <param name="modName"></param>
        public bool AddCustomMod(string modName);

        public void Shutdown();
    }
}
