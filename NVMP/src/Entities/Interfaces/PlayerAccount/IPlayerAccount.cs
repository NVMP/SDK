using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Entities
{
    /// <summary>
    /// This is an account associated with a player. 
    /// </summary>
    public interface IPlayerAccount
    {
        /// <summary>
        /// Returns the type of the account.
        /// </summary>
        public NetPlayerAccountType Type { get; }

        /// <summary>
        /// Returns the string representation of the unique account ID.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Returns the display name of the account. This may not be unique at all, but is useful for representing.
        /// </summary>
        public string DisplayName { get; }
    }
}
