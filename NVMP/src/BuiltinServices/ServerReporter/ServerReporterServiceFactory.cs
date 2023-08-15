using NVMP.BuiltinServices;

namespace NVMP.BuiltinServices
{
    public static class ServerReporterServiceFactory
    {
        /// <summary>
        /// Creates a new server reporter for reporting the server to the backend.
        /// </summary>
        /// <param name="modService"></param>
        /// <returns></returns>
        public static IServerReporterService Create(IModDownloadService modService)
        {
            return new ServerReporterServiceImpl(modService);
        }
    }
}
