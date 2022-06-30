using System;
using System.Runtime.Serialization;

namespace NVMP.Authenticator.Discord
{
    [DataContract]
    public class DiscordOAuthResponse
    {
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

        [DataMember(Name = "expires_in")]
        public int ExpiresIn { get; set; }
    }

}
