using NVMP.BuiltinServices.ManagedWebService;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Authenticator.Discord
{
    public static class DiscordAuthenticatorFactory
    {
        public static IDiscordAuthenticator Create(IManagedWebService webService)
        {
            return new DiscordAuthenticatorImpl(webService);
        }
    }
}
