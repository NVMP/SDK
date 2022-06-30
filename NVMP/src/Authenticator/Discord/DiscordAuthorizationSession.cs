using Discord.Rest;
using System;

namespace NVMP.Authenticator.Discord
{
    public class DiscordAuthorizationSession
    {
        public DiscordRestClient RestClient { get; set; }
        public RestGuildUser CurrentGuildUser { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        public uint ConnectionID { get; set; }
    }

}
