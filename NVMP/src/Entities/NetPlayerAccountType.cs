using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Entities
{
	/// <summary>
	/// Defines types of account authentication provided by the player.
	/// </summary>
    public enum NetPlayerAccountType
	{
		Invalid,

		/// <summary>
		/// Epic Games external account. This is not really external, but an account available anyway.
		/// </summary>
		EpicGames,

		/// <summary>
		/// Discord account.
		/// </summary>
		Discord
	}
}
