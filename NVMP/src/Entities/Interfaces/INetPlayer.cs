﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;
using NVMP.Entities.GUI;
using NVMP.Entities.Authentication;

namespace NVMP.Entities
{
    public interface INetPlayer : IAuthenticatedEntity
    {
        //
        // Static Helpers
        //
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
        public string AuthenticationToken { get; set; }

        /// <summary>
        /// Set if the connection has been authenticated by a managed authenticator module. Players can't do much
        /// with the world state, and shouldn't have any input on the game state until authenticated has been set
        /// </summary>
        public bool Authenticated { get; set; }

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
        /// Player's development state. Allowing them to be a dev allows for additional network information, that could be sensitive to the gamemode.
        /// </summary>
        public bool IsDev { get; set; }

        public void SendValidSaves(string[] digests);

        public void ShowVaultBoyMessage(string message, float time = 2.0f, INetPlayer.VaultBoyEmotion emotion = INetPlayer.VaultBoyEmotion.Happy);

        public void ShowCustomMessage(string message, string ddsPath, float time = 2.0f, string soundPath = null);

        public void RequestAutoSave();

        public void ClearValidSaves();

        /// <summary>
        /// Kicks the player from the current session in the next network stack update. This requests a disconnect from 
        /// the peer, so may not be an instant disconnection
        /// </summary>
        /// <param name="reason">reason of the kick</param>
        /// <param name="whoby">what entity performed this</param>
        /// <param name="silentFromChat">if set then this action isnt send to chat</param>
        public void Kick(string reason, string whoby = null, bool silentFromChat = false);

        /// <summary>
        /// Bans the player from the current session. This is currently not implemented
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="whoby"></param>
        public void Ban(string reason, string whoby = null);

        /// <summary>
        /// Bans the player's IP address from the session
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="whoby"></param>
        public void BanIP(string reason, string whoby = null);

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
    }
}
