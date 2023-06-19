using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Entities
{
    /// <summary>
    /// Roles associated to a player. These are descriptive tags you may add for describing
    /// a player's authentication or player statuses.
    /// </summary>
    public interface IPlayerRole
    {
        /// <summary>
        /// Display name of the role.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Internal identification for the role.
        /// </summary>
        public ulong Id { get; }

        /// <summary>
        /// Returns if the role is meant to be publicly displayed or not.
        /// </summary>
        public bool IsPrivate { get; }

        /// <summary>
        /// List of scopes this role is entitled to.
        /// </summary>
        public ICollection<IRoleScope> Scopes { get; }
    }
}
