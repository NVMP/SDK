
namespace NVMP.UserInterface
{
    /// <summary>
    /// Defines menu types used inside Fallout
    /// </summary>
    public enum MenuType
    {
        /// <summary>
        /// Opens a menu that will allow you to change your character’s face. Changing your face will disable perks, so they must be removed and added again manually.
        /// </summary>
        RaceFaceMenu,

        /// <summary>
        /// Opens a menu for a player to select their S.P.E.C.I.A.L stats
        /// </summary>
        LoveTesterMenu,

        /// <summary>
        /// Opens a menu for a player to change their name
        /// </summary>
        Name,
    }

    public enum InputType
    {
        KeyDown,
        KeyUp,
        MouseDown,
        MouseUp,
    }

}
