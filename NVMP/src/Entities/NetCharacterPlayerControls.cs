using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Entities
{
    /// <summary>
    /// Defines flags for player controls a player may observe or have disabled.
    /// </summary>
    [Flags]
    public enum NetCharacterPlayerControls
    {
        /// <summary>
        /// No controls set on these flags.
        /// </summary>
        None,

        /// <summary>
        /// Ability to control player movement, activation of objects, and some partial HUD elements.
        /// </summary>
        Movement        = (1 << 0),

        /// <summary>
        /// Ability to adjust player angle.
        /// </summary>
        Look            = (1 << 1),

        /// <summary>
        /// Ability to use their pipboy, and wait menu.
        /// </summary>
        Pipboy          = (1 << 2),

        /// <summary>
        /// Ability to raise their weapon.
        /// </summary>
        Fight           = (1 << 3),

        /// <summary>
        /// Ability to go into 3rd person.
        /// </summary>
        POVSwitch       = (1 << 4),

        /// <summary>
        /// Ability to see rollover text on objects.
        /// </summary>
        RolloverText    = (1 << 5),

        /// <summary>
        /// Ability to sneak.
        /// </summary>
        Sneak           = (1 << 6)
    }
}
