using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Entities.Authentication
{
    /// <summary>
    /// An authenticated entity is an object that has been authenticated with the native code. Instances that use authenticated entity interfaces
    /// are guarenteed to have been authorized for use. 
    /// </summary>
    public interface IAuthenticatedEntity
    {
        /// <summary>
        /// This is a unique identifier with the online services component. No other authenticated entity will ever share
        /// the same Identifier as another entity, even if they are from different entity types.
        /// </summary>
        public ulong Identifier { get; }

        /// <summary>
        /// Returns OAuth Discord information associated to the entity if this entity has been authorized externally via Discord previously.
        /// </summary>
        public IOAuthProviderDiscord OAuthDiscord { get; }
    }
}
