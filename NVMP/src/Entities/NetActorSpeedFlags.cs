using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Entities
{
	[Flags]
	public enum NetActorSpeedFlags : uint
	{
		Walk       = (1 << 0),
		Run        = (1 << 1),
		Crouched   = (1 << 2),
	};
}
