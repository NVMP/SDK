namespace NVMP.BuiltinServices.ManagedWebService
{
    public static class ManagedWebServiceFactory
    {
        public static IManagedWebService Create(string hostname, int portOverride)
        {
            return new ManagedWebServiceImpl(hostname, portOverride);
        }
    }
}
