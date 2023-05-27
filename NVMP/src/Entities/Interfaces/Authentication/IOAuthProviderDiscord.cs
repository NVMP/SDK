using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Entities.Authentication
{
    /// <summary>
    /// Generic external readonly OAuth provider.
    /// </summary>
    public interface IOAuthProviderDiscord
    {
        /// <summary>
        /// The snowflake of the Discord account used.
        /// </summary>
        public ulong DiscordID { get; }

        /// <summary>
        /// The username of the Discord account (including potential discriminator).
        /// Not Implemented.
        /// </summary>
        public string Username { get; }
    }
}
