using NVMP.Entities;

namespace NVMP.Authenticator
{
    /// <summary>
    /// AuthenticatorInterfaces are ways to plug different built-in styles of authentication for a server. You don't need to use these
    /// when making a GameServer plugin, but they are predefined authentication logic blocks that are built around the needs of NV:MP.
    /// </summary>
    public interface IAuthenticator
    {
        /// <summary>
        /// Returns whether the current authentication parameters are valid
        /// </summary>
        /// <param name="player"></param>
        /// <param name="authenticationToken"></param>
        /// <param name="badauthReason">The public visible reason the player cannot authenticate</param>
        /// <returns></returns>
        bool IsAuthenticationValid(INetPlayer player, string authenticationToken, ref string badauthReason);

        /// <summary>
        /// Implements any additional logging that needs to be fed to your server administrators
        /// </summary>
        /// <param name="message"></param>
        void Log(string message);


        /// <summary>
        /// Returns the authentication URL used to navigate this authenticator's authorization flow
        /// </summary>
        /// <returns></returns>
        string GetAuthenticationURL();

        /// <summary>
        /// Post-valid authentication, this will be called to set up any additional metadata on a player
        /// </summary>
        /// <param name="player"></param>
        void SetupAuthentication(INetPlayer player);

        /// <summary>
        /// Returns whether the scope supplied is valid on the player
        /// </summary>
        /// <param name="player"></param>
        /// <param name="scope"></param>
        bool IsScopeValid(INetPlayer player, string scope);

        /// <summary>
        /// Called when a player has left to clean up any required information
        /// </summary>
        /// <param name="player"></param>
        void PlayerLeft(INetPlayer player);

        /// <summary>
        /// Bans a player from the authenticator
        /// </summary>
        /// <param name="player"></param>
        /// <param name="reason"></param>
        void Ban(INetPlayer player, string reason = null);

        /// <summary>
        /// Updates the authenticator logic
        /// </summary>
        void Update();

        /// <summary>
        /// Returns a unique client ID in-use by the authenticator (used mainly for server broadcasting)
        /// </summary>
        string GetClientID();
    }
}