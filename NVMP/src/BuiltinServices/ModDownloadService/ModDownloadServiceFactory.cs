using NVMP.BuiltinServices.ManagedWebService;

namespace NVMP.BuiltinServices.ModDownloadService
{
    public static class ModDownloadServiceFactory
    {
        public static IModDownloadService Create(IGameServer server, IManagedWebService webService)
        {
            return new ModDownloadServiceImpl(server, webService);
        }
    }
}
