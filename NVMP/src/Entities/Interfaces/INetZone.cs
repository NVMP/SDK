using System;
using System.Runtime.InteropServices;

namespace NVMP.Entities
{
    /// <summary>
    /// NetZones are a reference that supports normal network reference functionality (minus an actual TESObjectREFR), but allows you 
    /// to track if other references enter inside it's boundaries. 
    /// </summary>
    public interface INetZone : INetReference
    {
        /// <summary>
        /// A delegate that can be assigned to for when a reference enters the zone's bounds.
        /// </summary>
        public Action<INetReference> ReferenceEntered { set; }

        /// <summary>
        /// A delegate that can be assigned to for when a reference that has previously entered the zone leaves the zone's bounds.
        /// </summary>
        public Action<INetReference> ReferenceExited { set; }

        /// <summary>
        /// Radius in game units to consider references inside or not.
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        /// Any additional description that needs to be rendered underneath the zone's name.
        /// </summary>
        public string Description { get; set; }
    }
}
