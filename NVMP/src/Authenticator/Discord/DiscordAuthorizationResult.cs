using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Authenticator.Discord
{
    /// <summary>
    /// Result class for authorizations of Discord user token
    /// </summary>
    public class DiscordAuthorizationResult
    {
        /// <summary>
        /// Status of the authorization
        /// </summary>
        public DiscordAuthorizationStatusTypes Status { get; set; }

        /// <summary>
        /// The JSON encoded OAuth response that contains the access token, and expiration time
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// Parsed session information about the user that you can query. This data will not persist longer than it's return lifetime, unless you request for a persistent session
        /// </summary>
        public DiscordAuthorizationSession Session { get; set; }
    }
}
