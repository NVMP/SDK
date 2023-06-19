using NVMP.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.BuiltinServices
{
    /// <summary>
    /// A player manager is meant to handle incoming player connections, evict them if they are no longer valid,
    /// and handle authentication requests from external code if desired.
    /// </summary>
    public interface IPlayerManager
    {
        /// <summary>
        /// Registers the player manager's middleware
        /// </summary>
        public void RegisterMiddleware();

        public void UnregisterMiddleware();

        /// <summary>
        /// A list of roles this player manager provides to players.
        /// </summary>
        public IPlayerRole[] Roles { get; }

        /// <summary>
        /// A list of account types this player manager requires players to have.
        /// </summary>
        public NetPlayerAccountType[] AccountTypesUsed { get; }

        /// <summary>
        /// Adds a scope to the specified roles.
        /// </summary>
        /// <param name="roleIds"></param>
        /// <param name="scope"></param>
        public void AddScopeToRoles(ulong[] roleIds, IRoleScope scope);

        /// <summary>
        /// Adds scopes to the specified role.
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="scopes"></param>
        public void AddScopesToRole(ulong roleId, IRoleScope[] scopes);
    }
}
