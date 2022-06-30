using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Entities
{
    /// <summary>
    /// Players will sync to the server specific gameplay objects they encounter. The server is never fully aware of the state around a player
    /// unless they send encounter messages to the server, to start synchronising entities on the network. The encounter config can be specified
    /// per player to prevent, or allow, specific entity types from being synchronised. This is helpful if you want to instance players into 
    /// gameplay where they shouldn't sync local entity states to other players.
    /// </summary>
    public interface INetCharacterEncounterConfig
    {
        /// <summary>
        /// Disables every encounter type supported.
        /// </summary>
        public void DisableAll();

        /// <summary>
        /// Enables every encounter type supported.
        /// </summary>
        public void EnableAll();

        public bool this[NetCharacterEncounterTypes key] { get; set; }
    }

}
