namespace NVMP.Authenticator.Discord
{
    public enum DiscordAuthorizationStatusTypes
    {
        /// <summary>
        /// Discord backend is down
        /// </summary>
        APIError,

        /// <summary>
        /// Discord backend could not process the request
        /// </summary>
        InternalServerError,

        /// <summary>
        /// The response generated from Discord is null, which could mean the specified parameters were incorrect
        /// </summary>
        OAuthResponseNull,

        /// <summary>
        /// The session is already in use 
        /// </summary>
        SessionInUse,

        /// <summary>
        /// The user is not a member of the primary guild.
        /// </summary>
        NotADiscordMember,

        /// <summary>
        /// The user is not authorized. This could be either that they don't meet the membership criteria (roles, join date), or for other reasons.
        /// </summary>
        Unauthorized,

        /// <summary>
        /// Authorization is successful, and the session data should be available
        /// </summary>
        AuthorizationSuccessful
    }

}
