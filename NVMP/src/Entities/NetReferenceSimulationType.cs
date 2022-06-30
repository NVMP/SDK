using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Entities
{
    /// <summary>
    /// Simulation types control how the object is networked and persisted depending on how the owner's connection to the server remains, 
    /// or if they lose vision to the object.
    /// </summary>
    public enum NetReferenceSimulationType : uint
    {
        /// <summary>
        /// The reference will stay on the player's list until they disconnect. If no player owns this object, then
        /// the reference stays until the server ends, or is deleted on the server
        /// Examples:   (fixed in place actors, objects that dont need simulation)
        /// </summary>
        Static = 0,

        /// <summary>
        /// The reference will swap to other player's lists if they become invalid, either by disconnecting or losing PVS. The object
        /// will only be destroyed if the server ends, or is deleted on the server
        /// If the reference swaps to the server, the object will reset to it's default state until the next player that is available receives it.
        /// Examples:   (actors created by the server to be simulated in place by players)
        /// </summary>
        HotSwap = 1,

        /// <summary>
        /// The reference will swap to other player's lists if they become invalid, but the object will not land on the server. If it does, it gets destroyed
        /// Examples:   (dropped objects, random actors created by a player)
        /// </summary>
        HotSwapTemp = 2,

        /// <summary>
        /// The reference will self destruct if the owner disconnects.
        /// </summary>
        Temp = 3
    }

}
