using NVMP.BuiltinServices.ManagedWebService;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Authenticator.Discord
{
    public static class DiscordAuthenticatorFactory
    {
        public static IDiscordAuthenticator Create(IManagedWebService webService, IDiscordAuthenticator.DiscordInitializationParams initializationParams = IDiscordAuthenticator.DiscordInitializationParams.All)
        {
            return new DiscordAuthenticatorImpl(webService, initializationParams);
        }
    }
}
