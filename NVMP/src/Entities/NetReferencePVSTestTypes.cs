using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Entities
{
    public enum NetReferencePVSTestTypes : uint
    {
        /// <summary>
        /// Default scope rules, this reference will use a combination of world space information and transform state
        /// to evaluate whether it should synchronise itself to a player.
        /// </summary>
        UseDefaultScopeRules = 0,

        /// <summary>
        /// Forces the PVS state to false, which will block potential synchronisation - or if already synced, the client will
        /// delete the reference and receive no further communication until PVS is re-established
        /// </summary>
        ForcePVSFalse = 1,

        /// <summary>
        /// Forces the PVS state to true, which will always synchronise changes - if not synced, the client will then create
        /// the reference and start synchronising latest and stale changes.
        /// </summary>
        ForcePVSTrue = 2
    };

}
