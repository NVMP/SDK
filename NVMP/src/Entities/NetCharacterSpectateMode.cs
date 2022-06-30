namespace NVMP.Entities
{
    /// <summary>
    /// The current mode of spectating the player is using. If this is set to Disabled, then the player is not spectating anything.
    /// </summary>
    public enum NetCharacterSpectateMode : uint
    {
        Disabled = 0,

        /// <summary>
        /// Focuses on the SpectateEntity.
        /// You must specify an entity to focus the character on. Be aware that the local player is silently teleported to the target entity's position
        /// and will remain invisible. If you disable spectating afterwards, the character will not return to their original location, instead they'll
        /// be where the target entity was. 
        /// </summary>
        FocusEntity = 1,

        /// <summary>
        /// Focuses on the SpectatePosition, and can also have SpectateRotation.
        /// </summary>
        FocusPosition = 2,

        /// <summary>
        /// Unlocks freecam, with full control to the player
        /// </summary>
        FreeCam = 3
    }
}
