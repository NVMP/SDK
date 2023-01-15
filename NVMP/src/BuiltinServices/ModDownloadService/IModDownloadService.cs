namespace NVMP.BuiltinServices.ModDownloadService
{
    public interface IModDownloadService
    {
        public ModFile[] GetDownloadableMods();

        public string GetDownloadURL();

        /// <summary>
        /// Registers a custom mod to the download service. This mod must be available in the Data folder of the server.
        /// </summary>
        /// <param name="modName"></param>
        public bool AddCustomMod(string modName);

        public void Shutdown();
    }
}
