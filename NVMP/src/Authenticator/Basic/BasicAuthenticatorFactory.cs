using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Authenticator.Basic
{
    public static class BasicAuthenticatorFactory
    {
        public static IBasicAuthenticator Create()
        {
            return new BasicAuthenticatorImpl();
        }
    }
}
