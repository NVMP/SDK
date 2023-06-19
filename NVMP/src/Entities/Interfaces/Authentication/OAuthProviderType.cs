using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Entities.Authentication
{
    /// <summary>
    /// Enumerable provider types supported by authenticated entities.
    /// </summary>
    public enum OAuthProviderType
    {
        /// <summary>
        /// The entity was authorized by Discord, and may be queried for this information
        /// </summary>
        Discord
    }
}
