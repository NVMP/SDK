using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Entities
{
    /// <summary>
    /// HAVOK ragdoll knocked states.
    /// </summary>
    public enum NetActorKnockedState
    {
        FatigueKnockout = -1,
        Normal = 0,
        KnockedOut = 1,
        KnockedDownRagdoll = 2
    }
}
