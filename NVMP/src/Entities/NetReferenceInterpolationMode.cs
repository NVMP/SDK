using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Entities
{
	/// <summary>
	/// Specifies an interpolation mode for an object. This controls how the object is rendered on player's screens, and gives
	/// more freedom to how a reference may move freely. It is good practise that if you are using objects that use more rapid or
	/// flowy motion, to consider using Blending - as it allows for more error without fighting on the object's interpolation.
	/// </summary>
    public enum NetReferenceInterpolationMode
	{
		/// <summary>
		/// Interpolates from the last snapshot to the present snapshot. By default all references in the game use linear interpolation, and
		/// it ensures that objects always remain in the exact position the server intended.
		/// </summary>
		LinearInterp,

		/// <summary>
		/// Ramp blends to the target position, with error tolerance. This may incur error if the object overshoots, and these objects
		/// should have velocities set so that the blender can make good judgement. Use INetReference.MaxBlendingError to control how 
		/// much error a reference can edure before being teleported back into the correct position.
		/// </summary>
		Blending,

		/// <summary>
		/// Directly sets to the target location, no interpolation. Every position update will jump the network object. 
		/// </summary>
		NoInterp
	}
}
