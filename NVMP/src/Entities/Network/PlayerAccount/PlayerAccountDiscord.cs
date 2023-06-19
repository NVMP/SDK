using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Entities
{
    internal class PlayerAccountDiscord : IPlayerAccount
    {
        public NetPlayerAccountType Type => NetPlayerAccountType.Discord;

        public string Id { get; internal set; }

        public string DisplayName { get; internal set; }
    }
}
