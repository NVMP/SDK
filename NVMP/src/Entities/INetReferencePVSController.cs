using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Entities
{

    /// <summary>
    /// The PVS controller controls the potentially-visible-set for this network object. By default we use
    /// scoping rules compared on the server to only sync data to entities that are close enough to eachother.
    /// With overriding this controller, you can control the PVS set.
    /// </summary>
    public interface INetReferencePVSController
    {
        /// <summary>
        /// Sets whether the object is visible to all connections regardless of PVS rules. This is not great on network performance,
        /// unless you are sure every connection regardless of their world position needs to know of this objects network data.
        /// It is recommended to use the delegate instead.
        /// </summary>
        public bool IsInGlobalPVS { get; set; }

        /// <summary>
        /// Sets a delegate to be called to override PVS results
        /// </summary>
        public Func<INetPlayer, NetReferencePVSTestTypes> CheckDelegate { get; set; }
    }

}
