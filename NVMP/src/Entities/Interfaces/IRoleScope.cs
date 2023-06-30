using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Entities
{
    /// <summary>
    /// Role scopes define permisions. Multiple roles may use various scopes to control access to specific
    /// functionalities you may want to lock behind authorization.
    /// </summary>
    public interface IRoleScope
    {
        /// <summary>
        /// Optional description of what this scope offers in relation to it's permissions.
        /// </summary>
        public string Description { get; }
    }

    public class RoleScope : IRoleScope
    {
        public RoleScope(string description = null)
        {
            Description = description;
        }

        public string Description { get; internal set; }
    }
}
