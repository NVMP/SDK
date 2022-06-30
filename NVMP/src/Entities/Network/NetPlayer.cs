using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace NVMP.Entities
{
    public class NetPlayer
    {
        private Color KickChatColor = Color.FromArgb(255, 50, 50);

        //
        // Static Helpers
        //
        #region Natives
        [DllImport("Native", EntryPoint = "NetPlayer_GetNumPlayers")]
        private static extern uint Internal_GetNumPlayers();

        [DllImport("Native", EntryPoint = "NetPlayer_GetMaxPlayers")]
        private static extern uint Internal_GetMaxPlayers();

        [DllImport("Native", EntryPoint = "NetPlayer_GetAllPlayers")]
        private static extern void Internal_GetAllPlayers(IntPtr[] players, uint containerSize);

        [DllImport("Native", EntryPoint = "NetPlayer_GetPlayerByConnectionID")]
        private static extern IntPtr Internal_GetPlayerByConnectionID(uint id);

        //
        // Member Accessors
        // 

        [DllImport("Native", EntryPoint = "NetPlayer_GetPlayerName", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        private static extern string Internal_GetPlayerName(IntPtr self);

        [DllImport("Native", EntryPoint = "NetPlayer_SetPlayerName", CharSet = CharSet.Unicode)]
        private static extern void Internal_SetPlayerName(IntPtr self, [MarshalAs(UnmanagedType.LPWStr)] string name);

        // Returns the primary actor being puppetted by the networked player as their local player
        [DllImport("Native", EntryPoint = "NetPlayer_GetPlayerActor")]
        private static extern IntPtr Internal_GetPlayerActor(IntPtr self);

        [DllImport("Native", EntryPoint = "NetPlayer_SendValidSaves")]
        private static extern void Internal_SendValidSaves(IntPtr self, uint numSaves, string[] saves);

        [DllImport("Native", EntryPoint = "NetPlayer_ShowVaultBoyMessage")]
        private static extern void Internal_ShowVaultBoyMessage(IntPtr self, string message, float time, uint emotion);

        [DllImport("Native", EntryPoint = "NetPlayer_ShowCustomMessage")]
        private static extern void Internal_ShowCustomMessage(IntPtr self, string message, string ddsPath, float time, string soundPath);

        [DllImport("Native", EntryPoint = "NetPlayer_RequestAutoSave")]
        private static extern void Internal_RequestAutoSave(IntPtr self);

        [DllImport("Native", EntryPoint = "NetPlayer_GetIP")]
        private static extern string Internal_GetIP(IntPtr self);

        [DllImport("Native", EntryPoint = "NetPlayer_GetAuthenticationToken")]
        private static extern string Internal_GetAuthenticationToken(IntPtr self);

        [DllImport("Native", EntryPoint = "NetPlayer_SetAuthenticationToken")]
        private static extern void Internal_SetAuthenticationToken(IntPtr self, string authenticationToken);

        [DllImport("Native", EntryPoint = "NetPlayer_Kick")]
        private static extern void Internal_Kick(IntPtr self, string reason, string whoby = null);

        [DllImport("Native", EntryPoint = "NetPlayer_Ban")]
        private static extern void Internal_Ban(IntPtr self, string reason, string whoby = null);

        [DllImport("Native", EntryPoint = "NetPlayer_BanIP")]
        private static extern void Internal_BanIP(IntPtr self, string reason, string whoby = null);

        [DllImport("Native", EntryPoint = "NetPlayer_GetConnectionID")]
        private static extern uint Internal_GetConnectionID(IntPtr self);

        [DllImport("Native", EntryPoint = "NetPlayer_SetCanUseSaves")]
        private static extern void Internal_SetCanUseSaves(IntPtr self, bool authenticated);

        [DllImport("Native", EntryPoint = "NetPlayer_GetCanUseSaves")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_GetCanUseSaves(IntPtr self);

        [DllImport("Native", EntryPoint = "NetPlayer_SetAuthenticated")]
        private static extern void Internal_SetAuthenticated(IntPtr self, bool authenticated);

        [DllImport("Native", EntryPoint = "NetPlayer_GetAuthenticated")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_GetAuthenticated(IntPtr self);

        [DllImport("Native", EntryPoint = "NetPlayer_SendGenericChatMessage", CharSet = CharSet.Unicode)]
        private static extern void Internal_SendGenericChatMessage(IntPtr self, [MarshalAs(UnmanagedType.LPWStr)] string message, byte r = 255, byte g = 255, byte b = 255);

        [DllImport("Native", EntryPoint = "NetPlayer_SendPlayerChatMessage", CharSet = CharSet.Unicode)]
        private static extern void Internal_SendPlayerChatMessage(IntPtr self
            , [MarshalAs(UnmanagedType.LPWStr)] string sender
            , byte sender_r
            , byte sender_g
            , byte sender_b

            , [MarshalAs(UnmanagedType.LPWStr)] string message
            , byte message_r
            , byte message_g
            , byte message_b
        );

        [DllImport("Native", EntryPoint = "NetPlayer_ShowMenu")]
        private static extern void Internal_ShowMenu(IntPtr self, int type);

        [DllImport("Native", EntryPoint = "NetPlayer_BroadcastGenericChatMessage", CharSet = CharSet.Unicode)]
        private static extern void Internal_BroadcastGenericChatMessage([MarshalAs(UnmanagedType.LPWStr)] string message, byte r = 255, byte g = 255, byte b = 255);

        //
        // Custom Data
        //
        [DllImport("Native", EntryPoint = "NetPlayer_GetCustomData")]
        private static extern string Internal_GetCustomData(IntPtr self, string key);

        [DllImport("Native", EntryPoint = "NetPlayer_SetCustomData")]
        private static extern void Internal_SetCustomData(IntPtr self, string key, string value);

        [DllImport("Native", EntryPoint = "NetPlayer_RemoveCustomData")]
        private static extern void Internal_RemoveCustomData(IntPtr self, string key);
        #endregion

        // The address of the unmanaged data this interface marshals against
        public IntPtr __UnmanagedAddress;

        /// <summary>
        /// Builds an array of every player in the current game session
        /// </summary>
        /// <returns>array of players</returns>
        public static NetPlayer[] All
        {
            get
            {
                // First grab all the native pointers
                uint numPlayers = Internal_GetNumPlayers();

                IntPtr[] players = new IntPtr[numPlayers];
                Internal_GetAllPlayers(players, numPlayers);

                // Now "marshal" them to managed by just wrapping player objects and assigning the native address
                NetPlayer[] marshalledPlayers = new NetPlayer[numPlayers];
                for (uint i = 0; i < numPlayers; ++i)
                {
                    IntPtr unmanagedPlayerPointer = players[i];
                    marshalledPlayers[i] = Marshals.NetPlayerMarshaler.GetInstance(null).MarshalNativeToManaged(unmanagedPlayerPointer) as NetPlayer;
                }

                return marshalledPlayers;
            }
        }

        public static uint NumPlayers
        {
            get
            {
                return Internal_GetNumPlayers();
            }
        }

        public static uint MaxPlayers
        {
            get
            {
                return Internal_GetMaxPlayers();
            }
        }

        /// <summary>
        /// Finds a NetPlayer object by connection ID, or null if the ID is not valid
        /// </summary>
        /// <param name="id">connection identifier</param>
        /// <returns>player</returns>
        public static NetPlayer GetPlayerByConnectionID(uint id)
        {
            IntPtr player = Internal_GetPlayerByConnectionID(id);
            return Marshals.NetPlayerMarshaler.GetInstance(null).MarshalNativeToManaged(player) as NetPlayer;
        }

        /// <summary>
        /// The player's name set by the game. This is not set by a player and cannot be wrote to other than via 
        /// the server and any managed native
        /// </summary>
        public string Name
        {
            set
            {
                Internal_SetPlayerName(__UnmanagedAddress, value);
            }
            get
            {
                return Internal_GetPlayerName(__UnmanagedAddress);
            }
        }

        /// <summary>
        /// The custom token provided by the connection to authenticate on. This could be an OAuth bearer token,
        /// a password, or auth key. Ensure this data is not exposed to the game state
        /// </summary>
        public string AuthenticationToken
        {
            get
            {
                return Internal_GetAuthenticationToken(__UnmanagedAddress);
            }
            set
            {
                Internal_SetAuthenticationToken(__UnmanagedAddress, value);
            }
        }

        /// <summary>
        /// Set if the connection has been authenticated by a managed authenticator module. Players can't do much
        /// with the world state, and shouldn't have any input on the game state until authenticated has been set
        /// </summary>
        public bool Authenticated
        {
            get
            {
                return Internal_GetAuthenticated(__UnmanagedAddress);
            }
            set
            {
                Internal_SetAuthenticated(__UnmanagedAddress, value);
            }
        }

        public bool CanUseSaves
        {
            get
            {
                return Internal_GetCanUseSaves(__UnmanagedAddress);
            }
            set
            {
                Internal_SetCanUseSaves(__UnmanagedAddress, value);
            }
        }

        /// <summary>
        /// An unsigned integer representing the connection ID of this player. This ID is generated genetically per 
        /// session and is unique per player/connection in the game
        /// </summary>
        public uint ConnectionID
        {
            get
            {
                return Internal_GetConnectionID(__UnmanagedAddress);
            }
        }

        /// <summary>
        /// Sets custom data on the unmanaged pointer type. Custom data is not synchronised but persists on a player until it is 
        /// either removed through the same interface, or the player disconnects. Only strings are supported
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key]
        {
            get
            {
                return Internal_GetCustomData(__UnmanagedAddress, key);
            }
            set
            {
                if (value == null)
                {
                    Internal_RemoveCustomData(__UnmanagedAddress, key);
                }
                else
                {
                    Internal_SetCustomData(__UnmanagedAddress, key, value);
                }
            }
        }

        /// <summary>
        /// Returns the actor in use by this player's character, this is the actor that is allocated for the user to use
        /// in gameplay synchronisation. In some cases the actor can be null if the connection is in a connection transition
        /// </summary>
        public INetCharacter Actor
        {
            get
            {
                var actorPtr = Internal_GetPlayerActor(__UnmanagedAddress);
                if (actorPtr == IntPtr.Zero)
                {
                    return null;
                }

                var actor = Marshals.NetCharacterMarshaler.Instance.MarshalNativeToManaged(actorPtr) as NetCharacter;
                if (!actor.IsCharacter)
                {
                    throw new Exception("Player actor is not a character");
                }

                return actor;
            }
        }

        /// <summary>
        /// Player's IP address in IPv4 format
        /// </summary>
        public string IP
        {
            get
            {
                return Internal_GetIP(__UnmanagedAddress);
            }
        }

        public void SendValidSaves(string[] digests)
        {
            Internal_SendValidSaves(__UnmanagedAddress, (uint)digests.Length, digests);
        }

        public enum VaultBoyEmotion : uint
        {
            Happy = 0,
            Sad = 1,
            Neutral = 2,
            Pain = 3
        };

        public void ShowVaultBoyMessage(string message, float time = 2.0f, VaultBoyEmotion emotion = VaultBoyEmotion.Happy)
        {
            Internal_ShowVaultBoyMessage(__UnmanagedAddress, message, time, (uint)emotion);
        }

        public void ShowCustomMessage(string message, string ddsPath, float time = 2.0f, string soundPath = null)
        {
            Internal_ShowCustomMessage(__UnmanagedAddress, message, ddsPath, time, soundPath);
        }

        public void RequestAutoSave()
        {
            Internal_RequestAutoSave(__UnmanagedAddress);
        }

        public void ClearValidSaves()
        {
            Internal_SendValidSaves(__UnmanagedAddress, 0, null);
        }
        
        /// <summary>
        /// Kicks the player from the current session in the next network stack update. This requests a disconnect from 
        /// the peer, so may not be an instant disconnection
        /// </summary>
        /// <param name="reason">reason of the kick</param>
        /// <param name="whoby">what entity performed this</param>
        /// <param name="silentFromChat">if set then this action isnt send to chat</param>
        public void Kick(string reason, string whoby = null, bool silentFromChat = false)
        {
            Internal_Kick(__UnmanagedAddress, reason, whoby);

            if (Authenticated)
            {
                if (!silentFromChat)
                {
                    string message = $"{Name} was kicked";

                    if (whoby != null)
                    {
                        message += $" by {whoby}";
                    }

                    if (reason != null)
                    {
                        message += $", reason: {reason}";
                    }

                    BroadcastGenericChatMessage(message, KickChatColor);
                }
            }
        }

        /// <summary>
        /// Bans the player from the current session. This is currently not implemented
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="whoby"></param>
        public void Ban(string reason, string whoby = null)
        {
            Internal_Ban(__UnmanagedAddress, reason, whoby);
        }

        /// <summary>
        /// Bans the player's IP address from the session
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="whoby"></param>
        public void BanIP(string reason, string whoby = null)
        {
            Internal_BanIP(__UnmanagedAddress, reason, whoby);
        }

        /// <summary>
        /// Sends a message to the player's chat box
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public void SendGenericChatMessage(string message, Color? color = null)
        {
            if (!color.HasValue)
            {
                color = Color.White;
            }
            Internal_SendGenericChatMessage(__UnmanagedAddress, message, color.Value.R, color.Value.G, color.Value.B);
        }

        /// <summary>
        /// Sends a message to the player's chat box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="sendercolor"></param>
        /// <param name="message"></param>
        /// <param name="messagecolor"></param>
        public void SendPlayerChatMessage(string sender, Color sendercolor, string message, Color? messagecolor = null)
        {
            if (!messagecolor.HasValue)
            {
                messagecolor = Color.White;
            }

            Internal_SendPlayerChatMessage(__UnmanagedAddress
                , sender
                , sendercolor.R
                , sendercolor.G
                , sendercolor.B
                , message
                , messagecolor.Value.R
                , messagecolor.Value.G
                , messagecolor.Value.B
            );
        }


        /// <summary>
        /// Shows a menu on the target player's screen
        /// </summary>
        /// <param name="type"></param>
        public void ShowMenu(UserInterface.MenuType type)
        {
            Internal_ShowMenu(__UnmanagedAddress, (int)type);
        }

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