namespace NVMP.BuiltinServices
{
    public static class ManagedWebServiceFactory
    {
        public static IManagedWebService Create(string hostname, int portOverride)
        {
            return new ManagedWebServiceImpl(hostname, portOverride);
        }
    }
}
