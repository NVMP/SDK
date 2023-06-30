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
		/// A dummy external account. This may be provided if the server is set to have no online account services, and should not be considered
		/// as a valid account at all. Dummy accounts are not safely queryable.
		/// </summary>
		Dummy,

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
