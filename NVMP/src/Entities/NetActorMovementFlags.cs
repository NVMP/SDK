using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Entities
{
	[Flags]
	public enum NetActorMovementFlags : uint
	{
		MoveForward  = (1 << 0),
		MoveBackward = (1 << 1),
		MoveLeft     = (1 << 2),
		MoveRight    = (1 << 3),
		TurnLeft     = (1 << 4),
		TurnRight    = (1 << 5),
		IsActive     = (1 << 6),
	};
}
