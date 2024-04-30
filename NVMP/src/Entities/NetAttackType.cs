using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Entities
{
    /// <summary>
    /// Describes an attack type an actor gives off.
    /// </summary>
    public enum NetAttackType : uint
    {
        /// <summary>
        /// Melee attack - obviously.
        /// </summary>
        Melee,

        /// <summary>
        /// Projectile attack - obviously.
        /// </summary>
        Projectile
    }
}
