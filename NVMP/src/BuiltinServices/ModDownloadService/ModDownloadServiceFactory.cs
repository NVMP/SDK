using NVMP.BuiltinServices;

namespace NVMP.BuiltinServices
{
    public static class ModDownloadServiceFactory
    {
        public static IModDownloadService Create(IGameServer server, IManagedWebService webService)
        {
            return new ModDownloadServiceImpl(server, webService);
        }
    }
}
