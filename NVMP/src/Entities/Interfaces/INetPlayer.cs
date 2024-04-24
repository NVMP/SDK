using NVMP.Entities.GUI;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace NVMP.Entities
{
    /// <summary>
    /// Wraps around player instance data, allowing custom event registration and player control. Players cannot be created in the managed environment, 
    /// and are instead created at runtime on a socket connection. Players own an INetCharacter instance as their primary replicated pawn in the game,
    /// and may control and simulate various objects running in the server.
    /// </summary>
    public interface INetPlayer 
    {
        #region Natives

        [DllImport("Native", EntryPoint = "NetPlayer_GetPlayerByConnectionID")]
        internal static extern IntPtr Internal_GetPlayerByConnectionID(uint id);

        [DllImport("Native", EntryPoint = "NetPlayer_BroadcastGenericChatMessage", CharSet = CharSet.Unicode)]
        internal static extern void Internal_BroadcastGenericChatMessage([MarshalAs(UnmanagedType.LPWStr)] string message, byte r = 255, byte g = 255, byte b = 255);
        #endregion

        public enum VaultBoyEmotion : uint
        {
            Happy = 0,
            Sad = 1,
            Neutral = 2,
            Pain = 3
        };

        /// <summary>
        /// Returns a number of all players
        /// </summary>
        public static uint NumPlayers { get; }

        /// <summary>
        /// Returns max players
        /// </summary>
        public static uint MaxPlayers { get; }

        /// <summary>
        /// Finds a NetPlayer object by connection ID, or null if the ID is not valid
        /// </summary>
        /// <param name="id">connection identifier</param>
        /// <returns>player</returns>
        public static INetPlayer GetPlayerByConnectionID(uint id)
        {
            var player = Internal_GetPlayerByConnectionID(id);
            return Marshals.NetPlayerMarshaler.GetInstance(null).MarshalNativeToManaged(player) as INetPlayer;
        }

        /// <summary>
        /// The player's name set by the game. This is not set by a player and cannot be wrote to other than via 
        /// the server and any managed native
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The custom token provided by the connection to authenticate on. This could be an OAuth bearer token,
        /// a password, or auth key. Ensure this data is not exposed to the game state
        /// </summary>
        [Obsolete("Authentication is now handled via GetAuthenticatedAccount, it is not safe to call this anymore.", true)]
        public string AuthenticationToken { get; set; }

        /// <summary>
        /// Returns if the account is authenticated. 
        /// </summary>
        [Obsolete("Authentication is now handled via GetAuthenticatedAccount, and unauthenticated players should never hit the CLR runtime except on Authenticating callbacks. You may remove calls to this property. ", false)]
        public bool Authenticated { get;  }

        /// <summary>
        /// Returns if the player was kicked and is waiting for connections to wrap up.
        /// </summary>
        public bool IsKicked { get; }

        /// <summary>
        /// Set if the player cannot load or save their savegame.
        /// </summary>
        public bool CanUseSaves { get; set; }

        /// <summary>
        /// An unsigned integer representing the connection ID of this player. This ID is generated genetically per 
        /// session and is unique per player/connection in the game
        /// </summary>
        public uint ConnectionID { get; }

        /// <summary>
        /// Sets custom data on the unmanaged pointer type. Custom data is not synchronised but persists on a player until it is 
        /// either removed through the same interface, or the player disconnects. Only strings are supported
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key] { get; set; }

        /// <summary>
        /// Returns the actor in use by this player's character, this is the actor that is allocated for the user to use
        /// in gameplay synchronisation. In some cases the actor can be null if the connection is in a connection transition
        /// </summary>
        public INetCharacter Actor { get; }

        /// <summary>
        /// Player's IP address in IPv4 format
        /// </summary>
        public string IP { get; }

        /// <summary>
        /// Player's round-trip time between a packet, and an acknowledgement response. 
        /// </summary>
        public uint Ping { get; }

        /// <summary>
        /// Gets the amount of seconds since the player last pinged the server with either a ping response, or sent us data.
        /// </summary>
        public float SecondsSinceLastPing { get; }

        /// <summary>
        /// VOIP volume multiplier, between 0.1 - 10.0
        /// </summary>
        public float VoiceVolume { get; set; }

        /// <summary>
        /// VOIP pitch, between 0.2 - 2.0
        /// </summary>
        public float VoicePitch { get; set; }

        /// <summary>
        /// Player's development state. Allowing them to be a dev allows for additional network information, that could be sensitive to the gamemode.
        /// </summary>
        public bool IsDev { get; set; }

        /// <summary>
        /// Player's party ID. Players in a party with other players will be able to see a blip where their friend is, and always maintains PVS status.
        /// </summary>
        public uint PartyID { get; set; }

        /// <summary>
        /// UNSAFE - not to be trusted for authentication. Mainly for rich presence.
        /// </summary>
        public ulong RichPresenceSteamID { get; }

        /// <summary>
        /// Returns an authenticated account with the player. If this returns null, then the account is not
        /// authenticated with the account provider type. This does not account for bans you may have set up on those
        /// platforms, such as Discord server bans - so you should do this after this returns the account you want to
        /// verify. 
        /// </summary>
        /// <param name="accountType"></param>
        /// <returns></returns>
        public IPlayerAccount GetAuthenticatedAccount(NetPlayerAccountType accountType);

        /// <summary>
        /// Returns a full list of authenticated accounts supplied by the player. This should never be empty or null, as at 
        /// minimum all NV:MP servers require an Epic account.
        /// </summary>
        /// <returns></returns>
        public IPlayerAccount[] GetAuthenticatedAccounts();

        /// <summary>
        /// Returns an array of all active menus being presented on the player's screen.
        /// </summary>
        public IGUIWindowTemplate[] ActiveMenus { get; }

        /// <summary>
        /// Returns a list of roles associated to the player.
        /// </summary>
        public IPlayerRole[] Roles { get; }

        /// <summary>
        /// Queries if the specified role is assigned to the player.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool HasRole(IPlayerRole role);

        /// <summary>
        /// Queries if the specified role is assigned to the player, using the ID.
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public bool HasRoleByID(ulong roleId);

        /// <summary>
        /// Adds a role to the player.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool TryAddToRole(IPlayerRole role);

        /// <summary>
        /// Removes a role from a player.
        /// </summary>
        /// <param name="role"></param>
        public void RemoveFromRole(IPlayerRole role);

        /// <summary>
        /// Removes all active roles on the player.
        /// </summary>
        public void RemoveAllRoles();

        /// <summary>
        /// Returns if the player has the associated scope on their roles.
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public bool HasRoleScope(IRoleScope scope);

        /// <summary>
        /// Sends a list of MD5 checksums that are associated to valid save game files. Players will only be able to load or browse
        /// saves that match this list provided.
        /// </summary>
        /// <param name="digests"></param>
        public void SendValidSaves(string[] digests);

        /// <summary>
        /// Removes all valid saves provided.
        /// </summary>
        public void ClearValidSaves();

        /// <summary>
        /// Displays a vault-boy user interface message for the specified amount of time if the player is in-game.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="time"></param>
        /// <param name="emotion"></param>
        public void ShowVaultBoyMessage(string message, float time = 2.0f, INetPlayer.VaultBoyEmotion emotion = INetPlayer.VaultBoyEmotion.Happy);

        /// <summary>
        /// Displays a custom VATs user interface message for the specified amount of time, and with the specified DDS path to use as the icon.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ddsPath"></param>
        /// <param name="time"></param>
        /// <param name="soundPath"></param>
        public void ShowCustomMessage(string message, string ddsPath, float time = 2.0f, string soundPath = null);

        /// <summary>
        /// Sends a request to the player to auto-save. This may not be completed depending on the player's state.
        /// </summary>
        public void RequestAutoSave();

        /// <summary>
        /// Kicks the player from the current session in the next network stack update. This requests a disconnect from 
        /// the peer, so may not be an instant disconnection
        /// </summary>
        /// <param name="reason">reason of the kick</param>
        /// <param name="whoby">what entity performed this</param>
        /// <param name="silentFromChat">if set then this action isnt send to chat</param>
        public void Kick(string reason, string whoby = null, bool silentFromChat = false);

        /// <summary>
        /// Bans the player from the current session. By default this acts as Kick(), but fires an OnBanned
        /// events for player manager's to handle.
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="whoby"></param>
        public void Ban(string reason, string whoby = null);

        /// <summary>
        /// Runs a synchronous script on the player. Be aware that any long running scripts will lock the main thread
        /// on the player, and may time them out or freeze their instance until the script completes execution.
        /// Compilation is ran and verified on the player.
        /// </summary>
        /// <param name="script">The script contents, must be less than 2KB in size or else the script will not be transmitted</param>
        public void RunScript(string script);

        /// <summary>
        /// Sends a message to the player's chat box
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        /// <param name="fontSize"></param>
        public void SendGenericChatMessage(string message, Color? color = null, float fontSize = 18.0f);

        /// <summary>
        /// Sends a message to the player's chat box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="sendercolor"></param>
        /// <param name="message"></param>
        /// <param name="messagecolor"></param>
        /// <param name="fontSize"></param>
        public void SendPlayerChatMessage(string sender, Color sendercolor, string message, Color? messagecolor = null, float fontSize = 18.0f);

        /// <summary>
        /// Shows a menu on the target player's screen
        /// </summary>
        /// <param name="type"></param>
        public void ShowMenu(UserInterface.MenuType type);

        /// <summary>
        /// Present's a template to a player. Presenting a template will keep a reference to the template
        /// in the player, until the player closes the window, or you call CloseMenu. Once presented, it is
        /// expected that if you wish to close it manually later on, to keep a reference to IGUIWindowTemplate
        /// so that you can remove it from the player later on.
        /// </summary>
        /// <param name="template"></param>
        public void PresentMenu(IGUIWindowTemplate template);

        /// <summary>
        /// Queries whether the menu template is active on the player's screen.
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public bool IsMenuActive(IGUIWindowTemplate template);

        /// <summary>
        /// Closes an active menu on the player via the template object. 
        /// </summary>
        /// <param name="template"></param>
        public void CloseMenu(IGUIWindowTemplate template);

        /// <summary>
        /// Closes all active menus.
        /// </summary>
        public void CloseAllMenus();

        /// <summary>
        /// Broadcasts a message to all player's chat boxes
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public static void BroadcastGenericChatMessage(string message, Color? color = null)
        {
            if (!color.HasValue)
            {
                color = Color.White;
            }
            Internal_BroadcastGenericChatMessage(message, color.Value.R, color.Value.G, color.Value.B);
        }

        /// <summary>
        /// Called when the player's roles have changed, either new roles added or roles removed.
        /// </summary>
        public event Action<INetPlayer> OnRolesChanged;

        /// <summary>
        /// Called when the player authenticates with the backend services.
        /// </summary>
        public event Action<INetPlayer> OnAuthenticated;

        /// <summary>
        /// Called when the player is banned via INetPlayer.Ban.
        /// </summary>
        public event Action<INetPlayer, string> OnBanned;

        /// <summary>
        /// Called on input received from the player
        /// </summary>
        public event Action<INetPlayer, UserInterface.InputType, Keyboard.ScanCodes> OnInput;

        /// <summary>
        /// Called on mouse input from the player
        /// </summary>
        public event Action<INetPlayer, int, int, int> OnMouseUpdate;
    }
}
