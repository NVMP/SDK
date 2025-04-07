using NVMP.Entities.GUI;
using NVMP.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using static NVMP.Entities.NetPlayerDelegates;

namespace NVMP.Entities
{
    internal class NetPlayer : NetUnmanaged, INetPlayer
    {
        static private Color KickChatColor = Color.FromArgb(255, 50, 50);

        [DllImport("Native", EntryPoint = "NetPlayer_GetNumPlayers")]
        internal static extern uint Internal_GetNumPlayers();

        [DllImport("Native", EntryPoint = "NetPlayer_GetMaxPlayers")]
        internal static extern uint Internal_GetMaxPlayers();

        [DllImport("Native", EntryPoint = "NetPlayer_GetAllPlayers")]
        internal static extern void Internal_GetAllPlayers(IntPtr[] players, uint containerSize);

        //
        // Member Accessors
        // 
        #region Natives
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

        [DllImport("Native", EntryPoint = "NetPlayer_RunScript")]
        private static extern void Internal_RunScript(IntPtr self, string script);

        [DllImport("Native", EntryPoint = "NetPlayer_ShowVaultBoyMessage")]
        private static extern void Internal_ShowVaultBoyMessage(IntPtr self, string message, float time, uint emotion);

        [DllImport("Native", EntryPoint = "NetPlayer_ShowCustomMessage")]
        private static extern void Internal_ShowCustomMessage(IntPtr self, string message, string ddsPath, float time, string soundPath);

        [DllImport("Native", EntryPoint = "NetPlayer_RequestAutoSave")]
        private static extern void Internal_RequestAutoSave(IntPtr self);

        [DllImport("Native", EntryPoint = "NetPlayer_GetIP")]
        private static extern string Internal_GetIP(IntPtr self);

        [DllImport("Native", EntryPoint = "NetPlayer_GetPing")]
        private static extern uint Internal_GetPing(IntPtr self);

        [DllImport("Native", EntryPoint = "NetPlayer_SetVOIPPitch")]
        private static extern void Internal_SetVoicePitch(IntPtr self, float pitch);

        [DllImport("Native", EntryPoint = "NetPlayer_GetVOIPPitch")]
        private static extern float Internal_GetVoicePitch(IntPtr self);

        [DllImport("Native", EntryPoint = "NetPlayer_SetVOIPVolume")]
        private static extern void Internal_SetVoiceVolume(IntPtr self, float volume);

        [DllImport("Native", EntryPoint = "NetPlayer_GetVOIPVolume")]
        private static extern float Internal_GetVoiceVolume(IntPtr self);

        [DllImport("Native", EntryPoint = "NetPlayer_GetMsSinceLastPing")]
        private static extern uint Internal_GetMsSinceLastPing(IntPtr self);

        [DllImport("Native", EntryPoint = "NetPlayer_Kick")]
        private static extern void Internal_Kick(IntPtr self, string reason, string whoby = null);

        [DllImport("Native", EntryPoint = "NetPlayer_GetConnectionID")]
        private static extern uint Internal_GetConnectionID(IntPtr self);

        [DllImport("Native", EntryPoint = "NetPlayer_SetCanUseSaves")]
        private static extern void Internal_SetCanUseSaves(IntPtr self, bool authenticated);

        [DllImport("Native", EntryPoint = "NetPlayer_IsKicked")]
        private static extern bool Internal_WasKicked(IntPtr self);

        [DllImport("Native", EntryPoint = "NetPlayer_GetCanUseSaves")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_GetCanUseSaves(IntPtr self);

        [DllImport("Native", EntryPoint = "NetPlayer_SetIsDev")]
        private static extern void Internal_SetIsDev(IntPtr self, bool isDev);

        [DllImport("Native", EntryPoint = "NetPlayer_GetIsDev")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_GetIsDev(IntPtr self);

        [DllImport("Native", EntryPoint = "NetPlayer_SetIsUsingClientsideHitDetection")]
        private static extern void Internal_SetIsUsingClientsideHitDetection(IntPtr self, bool isUsingCHD);

        [DllImport("Native", EntryPoint = "NetPlayer_GetIsUsingClientsideHitDetection")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_GetIsUsingClientsideHitDetection(IntPtr self);

        [DllImport("Native", EntryPoint = "NetPlayer_SetDamageMult")]
        private static extern void Internal_SetDamageMult(IntPtr self, float mult);

        [DllImport("Native", EntryPoint = "NetPlayer_GetDamageMult")]
        private static extern float Internal_GetDamageMult(IntPtr self);

        [DllImport("Native", EntryPoint = "NetPlayer_SetPartyID")]
        private static extern void Internal_SetPartyID(IntPtr self, uint partyID);

        [DllImport("Native", EntryPoint = "NetPlayer_GetPartyID")]
        private static extern uint Internal_GetPartyID(IntPtr self);

        [DllImport("Native", EntryPoint = "NetPlayer_GetSteamID")]
        private static extern ulong Internal_GetSteamID(IntPtr self);

        [DllImport("Native", EntryPoint = "NetPlayer_PlayOneShotSound")]
        private static extern void Internal_PlayOneShotSound(IntPtr self, string soundPath, bool hasRandomFrequencyShift);

        [DllImport("Native", EntryPoint = "NetPlayer_SendGenericChatMessage", CharSet = CharSet.Unicode)]
        private static extern void Internal_SendGenericChatMessage(IntPtr self, [MarshalAs(UnmanagedType.LPWStr)] string message, byte r = 255, byte g = 255, byte b = 255, float fontSize = 18.0f);

        [DllImport("Native", EntryPoint = "NetPlayer_SendFeedMessage", CharSet = CharSet.Unicode)]
        private static extern void Internal_SendFeedMessage(IntPtr self, [MarshalAs(UnmanagedType.LPWStr)] string id, [MarshalAs(UnmanagedType.LPWStr)] string message, byte r = 255, byte g = 255, byte b = 255, float fontSize = 18.0f);

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
            , float fontSize
        );

        [DllImport("Native", EntryPoint = "NetPlayer_ShowMenu")]
        private static extern void Internal_ShowMenu(IntPtr self, int type);

        //
        // Custom Data
        //
        [DllImport("Native", EntryPoint = "NetPlayer_GetCustomData")]
        private static extern string Internal_GetCustomData(IntPtr self, string key);

        [DllImport("Native", EntryPoint = "NetPlayer_SetCustomData")]
        private static extern void Internal_SetCustomData(IntPtr self, string key, string value);

        [DllImport("Native", EntryPoint = "NetPlayer_RemoveCustomData")]
        private static extern void Internal_RemoveCustomData(IntPtr self, string key);

        [DllImport("Native", EntryPoint = "NetPlayer_SetOnPresentedInteractionDelegate")]
        private static extern void Internal_SetOnPresentedInteractionDelegate(IntPtr self, OnPresentedInteraction del);
        #endregion

        internal OnPresentedInteraction PresentationDelegate;

        internal override void OnCreate()
        {
            PresentationDelegate = OnPresentedInteraction;
            Internal_SetOnPresentedInteractionDelegate(__UnmanagedAddress, PresentationDelegate);
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

        public string AuthenticationToken
        {
            get
            {
                throw new Exception("Deprecated");
            }
            set
            {
                throw new Exception("Deprecated");
            }
        }

        /// <summary>
        /// VOIP volume multiplier, between 0.1 - 10.0
        /// </summary>
        public float VoiceVolume
        {
            get => Internal_GetVoiceVolume(__UnmanagedAddress);
            set => Internal_SetVoiceVolume(__UnmanagedAddress, value);
        }

        /// <summary>
        /// VOIP pitch, between 0.2 - 2.0
        /// </summary>
        public float VoicePitch
        {
            get => Internal_GetVoicePitch(__UnmanagedAddress);
            set
            {
                if (value > 2.0f)
                {
                    throw new Exception("Pitch is too high!");
                }
                else if (value < 0.2f)
                {
                    throw new Exception("Pitch is too low!");
                }

                Internal_SetVoicePitch(__UnmanagedAddress, value);
            }

        }

        /// <summary>
        /// Set if the connection has been authenticated by a managed authenticator module. Players can't do much
        /// with the world state, and shouldn't have any input on the game state until authenticated has been set
        /// </summary>
        public bool Authenticated => true;

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
        /// Returns if the player was kicked and is waiting for connections to wrap up.
        /// </summary>
        public bool IsKicked => Internal_WasKicked(__UnmanagedAddress);

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
                if (key == "UniqueID")
                    throw new Exception("UniqueID must be queried through GetAuthenticatedAccounts");
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

        /// <summary>
        /// Player's round-trip time between a packet, and an acknowledgement response. 
        /// </summary>
        public uint Ping
        {
            get
            {
                return Internal_GetPing(__UnmanagedAddress);
            }
        }

        /// <summary>
        /// Gets the amount of seconds since the player last pinged the server with either a ping response, or sent us data.
        /// </summary>
        public float SecondsSinceLastPing
        {
            get
            {
                uint msSinceLastPing = Internal_GetMsSinceLastPing(__UnmanagedAddress);
                return (float)msSinceLastPing / 1000.0f;
            }
        }

        public bool IsDev
        {
            set => Internal_SetIsDev(__UnmanagedAddress, value);
            get => Internal_GetIsDev(__UnmanagedAddress);
        }

        public bool IsUsingClientsideHitDetection
        {
            set => Internal_SetIsUsingClientsideHitDetection(__UnmanagedAddress, value);
            get => Internal_GetIsUsingClientsideHitDetection(__UnmanagedAddress);
        }

        public float DamageMult
        {
            set => Internal_SetDamageMult(__UnmanagedAddress, value);
            get => Internal_GetDamageMult(__UnmanagedAddress);
        }

        public uint PartyID
        {
            get => Internal_GetPartyID(__UnmanagedAddress);
            set => Internal_SetPartyID(__UnmanagedAddress, value);
        }

        public ulong RichPresenceSteamID => Internal_GetSteamID(__UnmanagedAddress);

        internal List<IPlayerRole> InternalRoles = new List<IPlayerRole>();
        public IPlayerRole[] Roles => InternalRoles.ToArray();

        public void SendValidSaves(string[] digests)
        {
            Internal_SendValidSaves(__UnmanagedAddress, (uint)digests.Length, digests);
        }

        public void ShowVaultBoyMessage(string message, float time = 2.0f, INetPlayer.VaultBoyEmotion emotion = INetPlayer.VaultBoyEmotion.Happy)
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
        
        public void Kick(string reason, string whoby = null, bool silentFromChat = false)
        {
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

                    INetPlayer.BroadcastGenericChatMessage(message, KickChatColor);
                }
            }

            Internal_Kick(__UnmanagedAddress, reason, whoby);
        }

        public event Action<INetPlayer, string> OnBanned
        {
            add { BannedSubscriptions.Add(value); }
            remove { BannedSubscriptions.Remove(value); }
        }

        internal readonly SubscriptionDelegate<Action<INetPlayer, string>> BannedSubscriptions = new SubscriptionDelegate<Action<INetPlayer, string>>();

        public void Ban(string reason, string whoby = null)
        {
            Kick($"BANNED: {reason}", whoby);

            foreach (var sub in BannedSubscriptions.InternalSubscriptions)
            {
                sub(this, reason);
            }
        }

        public void SendGenericChatMessage(string message, Color? color, float fontSize)
        {
            if (!color.HasValue)
            {
                color = Color.White;
            }
            Internal_SendGenericChatMessage(__UnmanagedAddress, message, color.Value.R, color.Value.G, color.Value.B, fontSize);
        }

        public void SendFeedMessageWithID(string id, string message, Color? color, float fontSize)
        {
            if (!color.HasValue)
            {
                color = Color.White;
            }
            Internal_SendFeedMessage(__UnmanagedAddress, id, message, color.Value.R, color.Value.G, color.Value.B, fontSize);
        }

        public void SendFeedMessage(string message, Color? color, float fontSize)
        {
            if (!color.HasValue)
            {
                color = Color.White;
            }
            Internal_SendFeedMessage(__UnmanagedAddress, "", message, color.Value.R, color.Value.G, color.Value.B, fontSize);
        }

        public void SendPlayerChatMessage(string sender, Color sendercolor, string message, Color? messagecolor, float fontSize)
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
                , fontSize
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

        internal enum MenuUpdateType
        {
            Invalid        = 0,

		    // item events
		    TextBoxInput   = 1,
		    ButtonClick    = 2,

		    // window only events
		    WindowClosed   = 3,
	    };


        internal IList<IGUIWindowTemplate> PresentingTemplates = new List<IGUIWindowTemplate>();
        internal readonly SubscriptionDelegate<Action<uint, ulong, MenuUpdateType>> CreationSubscriptions = new SubscriptionDelegate<Action<uint, ulong, MenuUpdateType>>();

        internal void OnPresentedInteraction(uint uWindowId, ulong uElementId, MenuUpdateType menuUpdateType)
        {
            var targetWindowTemplate = PresentingTemplates.Where(template => template.ID == uWindowId).FirstOrDefault();
            if (targetWindowTemplate == null)
            {
                Debugging.Error($"{Name} could not interact with {uWindowId} as the window is invalid!");
                return;
            }

            if (menuUpdateType == MenuUpdateType.WindowClosed)
            {
                Debugging.Write($"{Name} has closed {uWindowId}...");
                PresentingTemplates.Remove(targetWindowTemplate);
                return;
            }

            // search recursively
            IGUIBaseElement targetElement = targetWindowTemplate.FindElementByID(uElementId);
            if (targetElement == null)
            {
                Debugging.Error($"{Name} could not interact with {uWindowId}:{uElementId} as the element is invalid!");
                return;
            }

            switch (menuUpdateType)
            {
                case MenuUpdateType.Invalid:
                    break;
                case MenuUpdateType.TextBoxInput:
                    break;
                case MenuUpdateType.ButtonClick:
                    if (targetElement is IGUIButtonElement)
                    {
                        var button = targetElement as IGUIButtonElement;
                        button.OnClicked?.Invoke(this);
                    }
                    break;
                default: break;
            }
        }

        public IGUIWindowTemplate[] ActiveMenus => PresentingTemplates.ToArray();

        /// <summary>
        /// Present's a template to a player. Presenting a template will keep a reference to the template
        /// in the player, until the player closes the object - or the template diposes - or the player leaves.
        /// </summary>
        /// <param name="template"></param>
        public void PresentMenu(IGUIWindowTemplate template)
        {
            // make a reference so the template is not cleaned up
            int index = PresentingTemplates.IndexOf(template);
            if (index != -1)
            {
                // update the index
                PresentingTemplates[index] = template;
            }
            else
            {
                // add to the listening list
                PresentingTemplates.Add(template);
            }

            // send the presentation
            (template as GUIWindowTemplateBuilder.GUIWindowTemplate).PresentToPlayer(this);
        }

        public bool IsMenuActive(IGUIWindowTemplate template)
        {
            return PresentingTemplates.IndexOf(template) != -1;
        }

        public void CloseMenu(IGUIWindowTemplate template)
        {
            // make a reference so the template is not cleaned up
            int index = PresentingTemplates.IndexOf(template);
            if (index == -1)
            {
                throw new ArgumentException($"The template supplied ({template.ID}) has not been presented to the player, or was closed. ", "template");
            }

            (template as GUIWindowTemplateBuilder.GUIWindowTemplate).CloseOnPlayer(this);
            PresentingTemplates.RemoveAt(index);
        }

        public void CloseAllMenus()
        {
            foreach (var template in PresentingTemplates)
            {
                (template as GUIWindowTemplateBuilder.GUIWindowTemplate).CloseOnPlayer(this);
            }

            PresentingTemplates.Clear();
        }

        public void RunScript(string script)
        {
            Internal_RunScript(__UnmanagedAddress, script);
        }

        [DllImport("Native", EntryPoint = "NetPlayer_GetNumAuthenticatedAccounts")]
        private static extern uint Internal_GetNumAuthenticatedAccounts(IntPtr player);

        [DllImport("Native", EntryPoint = "NetPlayer_GetAuthenticatedAccount")]
        private static extern void Internal_GetAuthenticatedAccount(IntPtr player, uint index
            , out string displayName
            , out string accountId
            , out NetPlayerAccountType accountType);

        public IPlayerAccount GetAuthenticatedAccount(NetPlayerAccountType accountType)
        {
            return GetAuthenticatedAccounts().Where(account => account.Type == accountType).FirstOrDefault();
        }

        public IPlayerAccount[] GetAuthenticatedAccounts()
        {
            var accounts = new List<IPlayerAccount>();

            uint numAccounts = Internal_GetNumAuthenticatedAccounts(__UnmanagedAddress);
            for (uint i = 0; i < numAccounts; ++i)
            {
                Internal_GetAuthenticatedAccount(__UnmanagedAddress, i, out string displayName, out string accountId, out NetPlayerAccountType accountType);

                switch (accountType)
                {
                    case NetPlayerAccountType.EpicGames:
                        accounts.Add(new PlayerAccountEpic { DisplayName = displayName, Id = accountId });
                        break;
                    case NetPlayerAccountType.Discord:
                        accounts.Add(new PlayerAccountDiscord { DisplayName = displayName, Id = accountId });
                        break;
                    default: break; 
                }
            }

            return accounts.ToArray();
        }

        public event Action<INetPlayer> OnAuthenticated
        {
            add { AuthenticatedSubscriptions.Add(value); }
            remove { AuthenticatedSubscriptions.Remove(value); }
        }

        internal readonly SubscriptionDelegate<Action<INetPlayer>> AuthenticatedSubscriptions = new SubscriptionDelegate<Action<INetPlayer>>();

        public event Action<INetPlayer> OnRolesChanged
        {
            add { RolesChangedSubscriptions.Add(value); }
            remove { RolesChangedSubscriptions.Remove(value); }
        }

        internal readonly SubscriptionDelegate<Action<INetPlayer>> RolesChangedSubscriptions = new SubscriptionDelegate<Action<INetPlayer>>();

        public event Action<INetPlayer, UserInterface.InputType, Keyboard.ScanCodes> OnInput
        {
            add { InputSubscriptions.Add(value); }
            remove { InputSubscriptions.Remove(value); }
        }

        internal readonly SubscriptionDelegate<Action<INetPlayer, UserInterface.InputType, Keyboard.ScanCodes>> InputSubscriptions = new SubscriptionDelegate<Action<INetPlayer, UserInterface.InputType, Keyboard.ScanCodes>>();

        public event Action<INetPlayer, int, int, int> OnMouseUpdate
        {
            add { MouseUpdateSubscriptions.Add(value); }
            remove { MouseUpdateSubscriptions.Remove(value); }
        }

        internal readonly SubscriptionDelegate<Action<INetPlayer, int, int, int>> MouseUpdateSubscriptions = new SubscriptionDelegate<Action<INetPlayer, int, int, int>>();

        public bool TryAddToRole(IPlayerRole role)
        {
            if (InternalRoles.Any(_role => _role.Id == role.Id))
                return false;

            InternalRoles.Add(role);

            foreach (var sub in RolesChangedSubscriptions.Subscriptions)
            {
                sub(this);
            }

            return true;
        }

        public void RemoveFromRole(IPlayerRole role)
        {
            InternalRoles.Remove(role);

            foreach (var sub in RolesChangedSubscriptions.Subscriptions)
            {
                sub(this);
            }
        }

        public void RemoveAllRoles()
        {
            InternalRoles.Clear();

            foreach (var sub in RolesChangedSubscriptions.Subscriptions)
            {
                sub(this);
            }
        }

        public bool HasRoleScope(IRoleScope scope)
        {
            foreach (var role in InternalRoles)
            {
                foreach (var gameScope in role.Scopes)
                {
                    if (gameScope == scope)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool HasRole(IPlayerRole role)
        {
            return InternalRoles.Any(_role => _role == role);
        }

        public bool HasRoleByID(ulong roleId)
        {
            return InternalRoles.Any(_role => _role.Id == roleId);
        }

        public void PlayOneShotSound(string soundPath, bool hasRandomFrequencyShift)
        {
            Internal_PlayOneShotSound(__UnmanagedAddress, soundPath, hasRandomFrequencyShift);
        }
    }
}