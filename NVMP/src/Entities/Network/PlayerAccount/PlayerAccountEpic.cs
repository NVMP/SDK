using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Entities
{
    internal class PlayerAccountEpic : IPlayerAccount
    {
        public NetPlayerAccountType Type => NetPlayerAccountType.EpicGames;

        public string Id { get; internal set; }

        public string DisplayName { get; internal set; }
    }
}
