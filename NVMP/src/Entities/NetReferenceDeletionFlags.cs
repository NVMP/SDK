using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Entities
{
    public enum NetReferenceDeletionFlags
    {
        /// <summary>
        /// Forcefully destroys the game object when destroying the game object.
        /// </summary>
        ForceDestroyGameObject = (1 << 0),
    }
}
