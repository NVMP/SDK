using Discord.Rest;
using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace NVMP.Authenticator.Discord
{
    /// <summary>
    /// Provides authentication based on Discord. You will need to allow the module to run once to populate new entries in server.cfg, and then afterwards
    /// configure them with your Discord secrets.
    /// </summary>
    public interface IDiscordAuthenticator : IAuthenticator
    {
        /// <summary>
        /// Remotely bans a specified Discord ID in the primary guild server with the specified reason
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reason"></param>
        public void BanDiscordID(ulong id, string reason = null);

        /// <summary>
        /// Tries to authenticate with the authenticator object the passed in authentication token
        /// </summary>
        /// <param name="blob"></param>
        /// <param name="mustNotBeInGame">if set, the user associated to this token must not be in-game</param>
        /// <param name="persistSession">if set, the user associated to this token will be persisted in the session until the player leaves. set this to false if you just want to soft query</param>
        /// <param name="redirectUriOverride"></param>
        /// <returns></returns>
        public Task<DiscordAuthorizationResult> TryAuthorization(string blob, bool mustNotBeInGame = true, bool persistSession = true, string redirectUriOverride = null);

        public Task<DiscordAuthorizationSession> TryAuthentication(string authtoken);

        /// <summary>
        /// Called when a user is banned on the Discord service level, useful for informing the game server about evicting players
        /// who no longer should have a valid token.
        /// </summary>
        public Func<RestGuildUser, string, Task> OnDiscordUserRemotelyBanned { get; set; }

        /// <summary>
        /// Binds a web route to the managed web service to listen for authorization calls.
        /// </summary>
        public void AddWebListener();

        /// <summary>
        /// Adds a scope type to the Discord ID authenticated to. Scopes are abstract, invent them as you may. They'll just be accounted for when scope checks are 
        /// used on the authenticator
        /// </summary>
        /// <param name="role"></param>
        /// <param name="scope"></param>
        public void AddScopeToRole(ulong role, string scope);

        /// <summary>
        /// Adds a permitted Discord role to allow to authenticate. Use 0 to allow any role.
        /// </summary>
        /// <param name="role"></param>
        public void AddPermittedRole(ulong role);
    }
}
