using System.Collections.Generic;

namespace NVMP.BuiltinServices
{
    public class DownloadableMod
    {
        /// <summary>
        /// File path relative on disk
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Name of the mod
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The computed digest to check integrity against
        /// </summary>
        public string Digest { get; set; }
    }

    /// <summary>
    /// Interface wrapper for the ModDownloadService, which adds mod files as binary content available for players to download
    /// on connection to your server. 
    /// </summary>
    public interface IModDownloadService
    {
        /// <summary>
        /// Returns all downloadable mods added to the mod download service.
        /// </summary>
        public IEnumerable<DownloadableMod> DownloadableMods { get; }

        /// <summary>
        /// Returns the URL the service is providing to download mods from the game server.
        /// </summary>
        public string DownloadURL { get; }

        /// <summary>
        /// Sets whether the service is able to serve mod downloads. This is enabled by default. Setting to false prevents any requests for mods files
        /// to fail with 404.
        /// </summary>
        public bool IsServingModDownloads { get; set; }

        /// <summary>
        /// Registers a custom mod to the download service. This mod must be available in the Data folder of the server.
        /// This also loads all associated BSAs underneath the mod into the mod service.
        /// </summary>
        /// <param name="modName"></param>
        public bool AddCustomMod(string modName);

        public void Shutdown();
    }
}
