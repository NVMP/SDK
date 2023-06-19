using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Entities.Authentication
{
    internal class OAuthProviderDiscord : IOAuthProviderDiscord
    {
        private INetPlayer PlayerAssociated;

        public OAuthProviderDiscord(INetPlayer player)
        {
            PlayerAssociated = player;
        }

        // Temporary implementation until the native side is hooked up.
        public ulong DiscordID => ulong.Parse(PlayerAssociated["UniqueID"]);

        public string Username { get => throw new NotImplementedException();  }
    }
}
