using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.IO;
using NVMP.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using NVMP.Entities;
using NVMP.BuiltinPlugins;
using NVMP.BuiltinServices;
using NVMP.BuiltinServices.ModDownloadService;
using Discord.WebSocket;

namespace NVMP
{
    public class ExampleServer : GameServer, IPlugin
    {
        protected INativeProgramDescription Description;
        protected IServerReporter           Reporter;
        protected IModDownloadService       ModService;
        protected DiscordSocketClient       DiscordSocket;
        protected IPlayerManager            PlayerManager;

        protected List<INetActor> DebugTestActors = new List<INetActor>();
        protected List<INetReference> References = new List<INetReference>();

        protected Color SystemColor = Color.FromArgb(195, 195, 195);
        protected Color SystemPromoColor = Color.FromArgb(255, 211, 87);
        protected Color SystemDeathColor = Color.FromArgb(255, 50, 50);

        protected float ProximityChatDistance = 4000.0f * 4000.0f;
        protected bool IsVoiceEnabled = true;

        protected static class DiscordAuthSettings
        {
            // The guild ID the example server will report to and auth members in
            public static ulong GuildId = 0;

            // Add your Discord bot token here
            public static string Token = "";

            // The moderator role
            public static ulong ModeratorRoleId = 0;

            // The logging channel ID
            public static ulong LogChannelId = 0;
        }

        protected static class PlayerScopes
        {
            public static IRoleScope Kick = new RoleScope();
            public static IRoleScope Ban = new RoleScope();
            public static IRoleScope Spawn = new RoleScope();
            public static IRoleScope SetScale = new RoleScope();
            public static IRoleScope God = new RoleScope();
            public static IRoleScope NoLimits = new RoleScope();
            public static IRoleScope WarpTo = new RoleScope();
            public static IRoleScope Goto = new RoleScope();
            public static IRoleScope Bring = new RoleScope();
            public static IRoleScope GCCollect = new RoleScope();
            public static IRoleScope ToggleHardcore = new RoleScope();
            public static IRoleScope DebugTest = new RoleScope();
            public static IRoleScope ToggleVoice = new RoleScope();
        }

        internal void LogToDiscord(string message)
        {
            if (DiscordAuthSettings.LogChannelId == 0)
                return;

            Task.Run(async () =>
            {
                var guild = DiscordSocket.GetGuild(DiscordAuthSettings.GuildId);
                if (guild != null)
                {
                    var channel = guild.GetTextChannel(DiscordAuthSettings.LogChannelId);
                    if (channel != null)
                    {
                        await channel.SendMessageAsync(message);
                    }
                }
            });
        }

        public override void Init()
        {
            base.Init();

            Debugging.Write("Starting up Discord Player Manager..");

            // The DiscordPlayerManager requires a valid DiscordSocketClient passed in so that it
            // can manage lifetime and data linkage
            DiscordSocket = new DiscordSocketClient(new DiscordSocketConfig
            {
                AlwaysDownloadUsers = false,
                GatewayIntents =
                            Discord.GatewayIntents.GuildMembers |
                            Discord.GatewayIntents.DirectMessages |
                            Discord.GatewayIntents.GuildMessages |
                            Discord.GatewayIntents.Guilds
            });

            var discordPlayerManager = new DiscordPlayerManager(DiscordSocket)
            {
                GuildId = DiscordAuthSettings.GuildId
            };

            PlayerManager = discordPlayerManager;

            // Once the DiscordSocket is ready, add some scopes to the moderator role id as it is now
            // downloaded from the available guilds.
            DiscordSocket.Ready += (() =>
            {
                Debugging.Write("Configuring Discord Player Manager..");
                PlayerManager.AddScopesToRole(DiscordAuthSettings.ModeratorRoleId, new IRoleScope[]
                {
                    PlayerScopes.Kick,
                    PlayerScopes.Ban,
                    PlayerScopes.Spawn,
                    PlayerScopes.SetScale,
                    PlayerScopes.God,
                    PlayerScopes.NoLimits,
                    PlayerScopes.WarpTo,
                    PlayerScopes.Goto,
                    PlayerScopes.Bring,
                    PlayerScopes.GCCollect,
                    PlayerScopes.ToggleHardcore,
                    PlayerScopes.DebugTest,
                    PlayerScopes.ToggleVoice
                });

                return Task.CompletedTask;
            });

            // Log into the Discord socket
            DiscordSocket.LoginAsync(Discord.TokenType.Bot, DiscordAuthSettings.Token);

            // Mod downloading
            Debugging.Write("Starting up mod service..");
            ModService = ModDownloadServiceFactory.Create(this, WebService);

            // List the mods here you want to make available to download and play with by players
            // ModService.AddCustomMod("MyCustomMod.esp");

            // Serverlist Reporter Module
            Debugging.Write("Starting server reporter..");
            Reporter = ServerReporterFactory.Create(ModService);

            Reporter.Name = "An Example NVMPX Server";
            Reporter.Description = "Built uisng the SDK kit";

            // Basic Save Init
            if (!Directory.Exists("Saves"))
            {
                Directory.CreateDirectory("Saves");
            }

            // Safe Zone
            var safeZone = Factory.Zone.Create(15000.0f);
            safeZone.Name = "Goodsprings";
            safeZone.NameColor = Color.MediumPurple;
            safeZone.SetExterior(WorldspaceType.WastelandNV, -18, 0);
            safeZone.Position = new Vector3 { X = -71819.055f, Y = 2014.8998f, Z = 8371.848f };
            safeZone.Title = "Safezone";
            safeZone.Description = "Continue out into the wilderness to enable PVP";

            safeZone.ReferenceEntered = delegate (INetReference refe)
            {
                if (refe.IsActor)
                {
                    var actor = refe as INetActor;
                    if (actor.IsCharacter)
                    {
                        actor.HasGodmode = true;

                        if (actor.PlayerOwner != null)
                        {
                            actor.PlayerOwner.SendGenericChatMessage("Welcome to Goodsprings, this is a safezone!", SystemPromoColor);
                        }
                    }
                }
            };

            safeZone.ReferenceExited = delegate (INetReference refe)
            {
                if (refe.IsActor)
                {
                    var actor = refe as INetActor;
                    if (actor.IsCharacter)
                    {
                        actor.HasGodmode = false;

                        if (actor.PlayerOwner != null)
                        {
                            actor.PlayerOwner.SendGenericChatMessage("You have left Goodsprings, your immunity has been disabled and PVP is now possible!", SystemPromoColor);
                        }
                    }
                }
            };

            References.Add(safeZone);

            // Create a test Actor
            var greeter = Factory.Actor.Create(0x0002977A);
            greeter.Name = "Mister Orderly";
            greeter.NameColor = Color.LightGray;
            greeter.Level = 99;
            greeter.IsFemale = false;
            greeter.SimType = NetReferenceSimulationType.HotSwap;
            greeter.Title = "Fixed Test NPC";
            greeter.SetExterior(WorldspaceType.WastelandNV, -18, 0);
            greeter.Position = new Vector3 { X = -71055.62f, Y = 2734.6982f, Z = 8347.849f };
            greeter.Rotation = new Quaternion { X = 0.995541f, Y = 0.074327275f, Z = -0.052930247f, W = -0.023917452f };

            References.Add(greeter);

            // Boss
            var bossActor = Factory.Actor.Create(0x000E59E7);
            bossActor.Scale = 4.0f;
            bossActor.Name = "Mad Bastard";
            bossActor.SetExterior(WorldspaceType.WastelandNV, -8, -7);
            bossActor.SimType = NetReferenceSimulationType.HotSwap;
            bossActor.Position = new Vector3 { X = -32357.904f, Y = -26299.209f, Z = 5913.434f };
            bossActor.SaveState();

            References.Add(bossActor);

            IsHardcore = true;
            Difficulty = GameServerDifficulty.Normal;

            Debugging.Write("Initialized!");
        }

        public new async Task PlayerMessaged(INetPlayer player, string message)
        {
            string name;

            var actor = player.Actor;
            if (actor != null)
            {
                name = $"{actor.Name} ({player.Name})";
            }
            else
            {
                name = player.Name;
            }

            Debugging.Write($"[chat] {name}: {message}");
            await Task.CompletedTask;
        }

        public new bool CanResendChatTo(INetPlayer player, INetPlayer target, string message, ref string username, ref System.Drawing.Color usercolor)
        {
            // Proximity chat!
            if (message.StartsWith("//"))
            {
                return true;
            }

            // Fixed global chat
            return true;
        }

        public new bool CanResendVoiceTo(INetPlayer player, INetPlayer target, ref VoiceFrame voiceFrame)
        {
            if (!IsVoiceEnabled)
            {
                return false;
            }

            voiceFrame.Is3D = true;
            return false;
        }

        internal string GetPlayerCharacterSaveFileName(INetPlayer player)
        {
            var discordID =  player["UniqueID"];
            if (discordID == null)
                throw new Exception("Invalid unique ID");

            return $"Saves/{discordID}.json";
        }

        internal void DeletePlayerCharacter(INetPlayer player)
        {
            string filename = GetPlayerCharacterSaveFileName(player);
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
        }

        internal void SavePlayerCharacter(INetPlayer player)
        {
            if (player["Loaded"] == null)
            {
                throw new Exception("Not previously loaded/set up");
            }

            var actor = player.Actor;
            if (actor != null)
            {
                string filename = GetPlayerCharacterSaveFileName(player);
                File.WriteAllText(filename, actor.EncodeToJSON());
            }
        }
        internal bool LoadPlayerCharacter(INetPlayer player)
        {
            var actor = player.Actor;
            if (actor != null)
            {
                string filename = GetPlayerCharacterSaveFileName(player);
                if (File.Exists(filename))
                {
                    actor.DecodeFromJSON(File.ReadAllText(filename));
                   return true;
                }
            }
            return false;
        }

        internal bool IsFormIDBanned(uint formID)
        {
            switch (formID)
            {
                case 0x001465A6: // dbg pistol
                case 0x0000432C: // fatman
                case 0x00050F92: // msy pistol
                case 0x00127C6C:
                case 0x0014EA5A: // holy frag grenades
                case 0x000F82AA: // disintegrator
                    return true;
            }
            return false;
        }

        internal bool FindPlayerActor(string targetName, out INetActor actor)
        {
            var players = Factory.Player.All;
            var targetPlayer = players.Where(player => player.Actor != null && player.Actor.Name.ToLower().Contains(targetName)).FirstOrDefault();
            if (targetPlayer != null)
            {
                actor = targetPlayer.Actor;
                return actor != null;
            }

            actor = null;
            return false;
        }

        public new bool PlayerExecutedCommand(INetPlayer player, string commandName, uint numParams, string[] paramList)
        {
            if (commandName == "goodsprings" || commandName == "gs")
            {
                var actor = player.Actor;
                if (actor != null)
                {
                    // Just outside of Doc Mitchell's house
                    actor.SetExterior(WorldspaceType.WastelandNV, -18, 0);
                    actor.Position = new Vector3 { X = -73203.07f, Y = 1273.1935f, Z = 8702.504f };
                    actor.Rotation = new Quaternion { X = 0.5902365f, Y = 0.06397164f, Z = 0.047003075f, W = 0.8033176f };

                    INetPlayer.BroadcastGenericChatMessage($"{actor.Name} is teleporting to goodsprings...");
                }
                else
                {
                    player.SendGenericChatMessage("Couldn't find your actor!");
                }

                return true;
            }
            else if (commandName == "wow")
            {
                player.ShowVaultBoyMessage("Wow! Great", 5.0f, INetPlayer.VaultBoyEmotion.Pain);
                return true;
            }
            else if (commandName == "mole123" && paramList != null)
            {
                var players = Factory.Player.All;
                foreach (var otherPlayer in players)
                {
                    otherPlayer.SendPlayerChatMessage("MOLE", SystemPromoColor, String.Join(" ", paramList));
                }
                return true;
            }
            else if (commandName == "sex")
            {
                var rng = new Random();
                if (rng.Next(16) == 0)
                {
                    if (player.Actor != null)
                    {
                        player.Actor.AddItem(NetActorInventoryItem.GetByReference(0x00120853), 1, true);
                    }

                    player.ShowCustomMessage("...", "interface\\icons\\pipboyimages\\apparel\\apparel_whiteglove_mask.dds", 5.0f);
                    player.ShowCustomMessage("The White Glove Society rejected your sex request, but have sent a gift of goodwill", "interface\\icons\\pipboyimages\\apparel\\apparel_whiteglove_mask.dds", 5.0f);
                    return true;
                }
            }
            else if (commandName == "gccollect")
            {
                if (player.HasRoleScope(PlayerScopes.GCCollect))
                {
                    GC.Collect();
                    return true;
                }
            }
            else if (commandName == "deletemycharacter")
            {
                try
                {
                    DeletePlayerCharacter(player);
                    player["CharacterDeleted"] = "true";

                    player.Kick("Character Death Requested");
                }
                catch (Exception)
                {
                    player.SendGenericChatMessage("Failed to delete your character", SystemDeathColor);
                }

                return true;
            }
            else if (commandName == "save")
            {
                try
                {
                    SavePlayerCharacter(player);
                    player.SendGenericChatMessage("Saved", SystemPromoColor);
                } catch (Exception e)
                {
                    player.SendGenericChatMessage("Failed to save your character, error: " + e.Message, SystemDeathColor);
                }
                return true;
            }
            else if (commandName == "whereami")
            {
                var actor = player.Actor;
                if (actor != null)
                {
                    if (actor.IsInInterior)
                    {
                        player.SendGenericChatMessage($"You are in an interior cell, in cell {actor.Interior}", SystemPromoColor);
                    }
                    else
                    {
                        player.SendGenericChatMessage($"You are in an exterior cell, in worldspace {actor.Worldspace.FormID} [{actor.Worldspace.X}, {actor.Worldspace.Y}]", SystemPromoColor);
                    }

                    player.SendGenericChatMessage($"Position is <{actor.Position.X}, {actor.Position.Y}, {actor.Position.Z}>", SystemPromoColor);
                    player.SendGenericChatMessage($"Rotation is <{actor.Rotation.X}, {actor.Rotation.Y}, {actor.Rotation.Z}, {actor.Rotation.W}> ", SystemPromoColor);

                    return true;
                }
            }
            else if (commandName == "kick" && numParams >= 1)
            {
                if (player.HasRoleScope(PlayerScopes.Kick))
                {
                    var players = Factory.Player.All;

                    string searchPattern = paramList[0].ToLower();
                    string reason = numParams > 1 ? paramList[1] : null;

                    if (searchPattern == "*")
                    {
                        // Kick all
                        foreach (var patternPlayer in players)
                        {
                            if (patternPlayer.ConnectionID != player.ConnectionID)
                            {
                                patternPlayer.Kick(reason, player.Name);

                                LogToDiscord($"{patternPlayer.Name} was kicked by {player.Name}, reason is {reason}");
                            }
                        }

                        return true;
                    }

                    var targetPlayer = players.Where(player => player.Name.ToLower().Contains(searchPattern)).FirstOrDefault();
                    if (targetPlayer != null)
                    {
                        targetPlayer.Kick(reason, player.Name);

                        LogToDiscord($"{targetPlayer.Name} was kicked by {player.Name}, reason is {reason}");
                    }
                    else
                    {
                        player.SendGenericChatMessage("Could not find the specified user");
                    }

                    return true;
                }
            }
            else if (commandName == "warptocell" && numParams >= 1)
            {
                if (player.HasRoleScope(PlayerScopes.WarpTo))
                {
                    var actor = player.Actor;
                    if (actor != null)
                    {
                        string warpMarker = paramList[0];

                        player.SendGenericChatMessage($"Trying to warp to {warpMarker} warp marker, if you are still in the same position - you got the cellname wrong!");
                        actor.TeleportToMarker(warpMarker);
                        return true;
                    }
                }
            }
            else if (commandName == "togglehardcore")
            {
                if (player.HasRoleScope(PlayerScopes.ToggleHardcore))
                {
                    IsHardcore = !IsHardcore;
                    INetPlayer.BroadcastGenericChatMessage("Hardcore mode has been " + (IsHardcore ? "enabled" : "disabled") + " by " + player.Name, SystemPromoColor);
                    return true;
                }
            }
            else if (commandName == "togglevoice")
            {
                if (player.HasRoleScope(PlayerScopes.ToggleVoice))
                {
                    IsVoiceEnabled = !IsVoiceEnabled;
                    INetPlayer.BroadcastGenericChatMessage("Voice has been " + (IsVoiceEnabled ? "enabled" : "disabled") + " by " + player.Name, SystemPromoColor);
                    return true;
                }
            }
            else if (commandName == "goto" && numParams >= 1)
            {
                if (player.HasRoleScope(PlayerScopes.Goto))
                {
                    var playerActor = player.Actor;

                    if (playerActor != null)
                    {
                        if (FindPlayerActor(paramList[0], out INetActor target))
                        {
                            player.SendGenericChatMessage($"Teleporting to {target.Name}", SystemPromoColor);
                            playerActor.TeleportTo(target);

                            if (target.PlayerOwner != null)
                            {
                                target.PlayerOwner.SendGenericChatMessage($"{playerActor.Name} has teleported to you", SystemPromoColor);
                            }
                        }
                        else
                        {
                            player.SendGenericChatMessage("Could not find player " + paramList[0]);
                        }
                    }
                    return true;
                }
            }
            else if (commandName == "bring" && numParams >= 1)
            {
                if (player.HasRoleScope(PlayerScopes.Bring))
                {
                    var playerActor = player.Actor;

                    if (playerActor != null)
                    {
                        if (FindPlayerActor(paramList[0], out INetActor target))
                        {
                            player.SendGenericChatMessage($"Bringing {target.Name} to you", SystemPromoColor);
                            target.TeleportTo(playerActor);

                            if (target.PlayerOwner != null)
                            {
                                target.PlayerOwner.SendGenericChatMessage($"{playerActor.Name} has brought you to them", SystemPromoColor);
                            }
                        }
                        else
                        {
                            player.SendGenericChatMessage("Could not find player " + paramList[0]);
                        }
                    }
                    return true;
                }
            }
            else if ((commandName == "spawn" || commandName == "give") && numParams >= 1)
            {
                if (player.HasRoleScope(PlayerScopes.Spawn))
                {
                    string formHex = paramList[0];
                    if (formHex.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase))
                    {
                        formHex = formHex.Substring(2);
                    }

                    uint formID = 0;
                    if (!uint.TryParse(formHex, System.Globalization.NumberStyles.HexNumber, null, out formID))
                    {
                        player.SendGenericChatMessage("Could not parse a valid hex code!");
                        return true;
                    }

                    var item = NetActorInventoryItem.GetByReference(formID);
                    if (item == null)
                    {
                        player.SendGenericChatMessage($"Could not find item {formHex}");
                        return true;
                    }

                    if (IsFormIDBanned(formID))
                    {
                        player.SendGenericChatMessage($"{item.Name} is banned from being spawned >:)", SystemDeathColor);
                        return true;
                    }

                    uint quantity = 1;
                    if (numParams >= 2)
                    {
                        uint.TryParse(paramList[1], out quantity);
                    }

                    string targetName = null;
                    if (numParams >= 3)
                    {
                        targetName = paramList[2].ToLower();
                    }

                    INetActor actor = null;
                    if (targetName == null)
                    {
                        actor = player.Actor;
                    }
                    else
                    {
                        var players = Factory.Player.All;
                        var targetPlayer = players.Where(player => player.Actor != null && player.Actor.Name.ToLower().Contains(targetName)).FirstOrDefault();
                        if (targetPlayer != null)
                        {
                            actor = targetPlayer.Actor;
                        }
                        else
                        {
                            player.SendGenericChatMessage("Could not find player " + targetName);
                            return true;
                        }
                    }

                    if (actor != null)
                    {
                        actor.AddItem(item, quantity);

                        player.SendGenericChatMessage($"Spawned {item.Name} x{quantity} on {actor.Name}", SystemPromoColor);
                        LogToDiscord($"{player.Name} spawned {item.Name} x{quantity}");
                    }
                    else
                    {
                        player.SendGenericChatMessage("No valid actor!");
                    }

                    return true;
                }
            }
            else if (commandName == "god")
            {
                if (player.HasRoleScope(PlayerScopes.God))
                {
                    var actor = player.Actor;
                    if (actor != null)
                    {
                        actor.HasGodmode = !actor.HasGodmode;

                        if (actor.HasGodmode)
                        {
                            player.SendGenericChatMessage("Enabled godmode", SystemPromoColor);
                            LogToDiscord($"{player.Name} enabled godmode");
                        }
                        else
                        {
                            player.SendGenericChatMessage("Disabled godmode", SystemPromoColor);
                            LogToDiscord($"{player.Name} disabled godmode");
                        }

                        return true;
                    }
                }
            }
            else if (commandName == "createactor")
            {
                if (player.HasRoleScope(PlayerScopes.DebugTest))
                {
                    if (numParams >= 1)
                    {
                        string formHex = paramList[0];
                        if (formHex.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase))
                        {
                            formHex = formHex.Substring(2);
                        }

                        uint formID = 0;
                        if (!uint.TryParse(formHex, System.Globalization.NumberStyles.HexNumber, null, out formID))
                        {
                            player.SendGenericChatMessage("Could not parse a valid hex code!");
                            return true;
                        }

                        var actor = Factory.Actor.Create(formID);
                        actor.TeleportTo(player.Actor);
                        actor.SimType = NetReferenceSimulationType.HotSwap;
                        actor.SaveState();

                        DebugTestActors.Add(actor);
                     
                        player.SendGenericChatMessage("Created", SystemPromoColor);
                    }
                    return true;
                }
            }
            else if (commandName == "reloadactors")
            {
                if (player.HasRoleScope(PlayerScopes.DebugTest))
                {
                    foreach (var actor in DebugTestActors)
                    {
                        actor.ReloadState();
                    }

                    return true;
                }
            }
            else if (commandName == "deleteactors")
            {
                if (player.HasRoleScope(PlayerScopes.DebugTest))
                {
                    foreach (var actor in DebugTestActors)
                    {
                        actor.Destroy();
                    }

                    DebugTestActors.Clear();
                    return true;
                }
            }
            else if (commandName == "setscale" && numParams >= 1)
            {
                if (player.HasRoleScope(PlayerScopes.SetScale))
                {
                    var actor = player.Actor;
                    if (actor != null)
                    {
                        float scale = 0.0f;
                        if (!float.TryParse(paramList[0], out scale))
                        {
                            player.SendGenericChatMessage("Could not parse the scale, please make sure you use a valid number between 0.01 - 10.00");
                            return true;
                        }

                        if (!player.HasRoleScope(PlayerScopes.NoLimits))
                        {
                            if (scale > 3.0f)
                            {
                                scale = 3.0f;
                            }
                        }

                        actor.Scale = scale;
                        player.SendGenericChatMessage($"Set scale to {scale}", SystemPromoColor);

                        return true;
                    }
                }
            }
            else if (commandName == "players")
            {
                var players = Factory.Player.All;
                player.SendGenericChatMessage("Online players: " + String.Join(", ", players
                    .Select(player => new { player, player.Actor })
                    .Select(pair => $"{pair.Actor?.Name} ({pair.player.Name})")));
                return true;
            }
            else if (commandName == "creationmenu")
            {
                player.ShowMenu(UserInterface.MenuType.RaceFaceMenu);
                return true;
            }
            else if (commandName == "specialmenu")
            {
                player.ShowMenu(UserInterface.MenuType.LoveTesterMenu);
                return true;
            }

            return false;
        }

        public new async Task PlayerJoined(INetPlayer player)
        {
            // Pro-tip:  You shouldn't start altering the actor's state here. There will be a valid actor object, but we don't know
            //           if it is safe to teleport the player to a desired location yet, or if the data will be overwrote.
            //           Use PlayerRequestsRespawn to deliver setups
            Debugging.Write($"Player {player.Name} joined! ");
            player.SendGenericChatMessage("Connected, currently authenticating your player...", SystemColor);
            await Task.CompletedTask;
        }

        public bool ArePlayerChangesBroadcasted
        {
            get
            {
                return INetPlayer.NumPlayers < 10;
            }
        }

        public new async Task PlayerLeft(INetPlayer player)
        {
            Debugging.Write($"{player.Name} left");

            var actor = player.Actor;
            if (actor != null)
            {
                if (ArePlayerChangesBroadcasted)
                {
                    INetPlayer.BroadcastGenericChatMessage($"Player {actor.Name} ({player.Name}) left!", SystemColor);
                }

                if (player["CharacterDeleted"] == null)
                {
                    try
                    {
                        SavePlayerCharacter(player);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            else
            {
                INetPlayer.BroadcastGenericChatMessage($"Player {player.Name} left!", SystemColor);
            }

            await Task.CompletedTask;
        }

        public new Task<bool> PlayerAuthenticating(INetPlayer player)
        {
            player.SendGenericChatMessage($"Authenticated as {player.Name}, welcome!");
            player.SendGenericChatMessage($"This is the test server for NV:MP-X, player saving is experimental at the moment! DC'ing may lose your stuff. Please post in #thestrip if any issues arise! Use /players to view online members, /creationmenu to set up your appearance.",
                SystemPromoColor);

            var actor = player.Actor;
            if (actor != null)
            {
                //
                // Actor Setup on Load
                //
                string serverNickName = player["DiscordServerNickname"];
                string discordTopRole = player["DiscordServerTopRoleName"];
                if (serverNickName != null)
                {
                    actor.Name = serverNickName;
                }
                else
                {
                    actor.Name = player.Name;
                }

                if (discordTopRole != null)
                {
                    actor.Title = discordTopRole;

                    string discordTopRoleColor = player["DiscordServerTopRoleColor"];
                    if (discordTopRoleColor != null)
                    {
                        actor.TitleColor = ColorTranslator.FromHtml(discordTopRoleColor);
                    }
                }
                else
                {
                    actor.Title = "Player";
                    actor.TitleColor = Color.LightGray;
                }
            }

            if (ArePlayerChangesBroadcasted)
            {
                if (actor != null)
                {
                    INetPlayer.BroadcastGenericChatMessage($"Player {actor.Name} ({player.Name}) joined!", SystemColor);
                }
                else
                {
                    INetPlayer.BroadcastGenericChatMessage($"Player {player.Name} joined!", SystemColor);
                }
            }

            return Task.FromResult(true);
        }

        public new async Task PlayerRequestsRespawn(INetPlayer player)
        {
            var actor = player.Actor;
            if (actor != null)
            {
                try
                {
                    if (LoadPlayerCharacter(player))
                    {
                        player["Loaded"] = "true";
                        player["CharacterDeleted"] = null;
                        player.SendGenericChatMessage("Loaded player character", SystemPromoColor);
                    }
                    else
                    {
                        throw new Exception("Needs new character"); //hack
                    }
                }
                catch (Exception)
                {
                    player["Loaded"] = "true";
                    player["CharacterDeleted"] = null;

                    // Just outside of Doc Mitchell's house
                    actor.SetExterior(WorldspaceType.WastelandNV, -18, 0);
                    actor.Position = new Vector3 { X = -73203.07f, Y = 1273.1935f, Z = 8702.504f };
                    actor.Rotation = new Quaternion { X = 0.5902365f, Y = 0.06397164f, Z = 0.047003075f, W = 0.8033176f };

                    if (!actor.IsDead)
                    {
                        actor.Level = 0;
                    }

                    actor.Health = actor.MaxHealth;

                    player.SendGenericChatMessage("A new character was created", SystemPromoColor);

                    actor.RemoveAllItems();

                    var pistol = NetActorInventoryItem.GetByReference(0x000E3778);
                    var ammo = NetActorInventoryItem.GetByReference(0x0008ED03);
                    var armour = NetActorInventoryItem.GetByReference(0x0001CBDC);
                    var stims = NetActorInventoryItem.GetByReference(0x00015169);
                    var doctorsbag = NetActorInventoryItem.GetByReference(0x000CB05C);
                    actor.AddItem(pistol, 1, true);
                    actor.AddItem(armour, 1, true);
                    actor.AddItem(ammo, 100);
                    actor.AddItem(stims, 5, false);
                    actor.AddItem(doctorsbag, 1, false);
                }
            }
            await Task.CompletedTask;
        }

        public new async Task ActorDied(INetActor actor, INetActor killer)
        {
            if (actor != null)
            {
                if (actor.IsCharacter)
                {
                    // A dead player loses their inventory
                    if (actor.PlayerOwner != null)
                    {
                        try
                        {
                            DeletePlayerCharacter(actor.PlayerOwner);
                            actor.PlayerOwner["CharacterDeleted"] = "true";
                            actor.PlayerOwner["Loaded"] = null;
                            actor.PlayerOwner.SendGenericChatMessage("Character wiped - you died! If you stay to respawn, your statistics will carry to your new life!", SystemDeathColor);
                        }
                        catch (Exception)
                        {
                            actor.PlayerOwner.SendGenericChatMessage("Failed to delete your character", SystemDeathColor);
                        }
                    }

                    INetPlayer.BroadcastGenericChatMessage($"{actor.Name} died!", SystemDeathColor);
                }
            }
            await Task.CompletedTask;
        }

        public new void Update(float delta)
        {
#if DEBUG
            GC.Collect();
#endif
        }

        public new void Shutdown()
        {
            WebService.Shutdown();
            ModService.Shutdown();

            foreach (var actor in DebugTestActors)
            {
                actor.Destroy();
            }

            DebugTestActors.Clear();

            foreach (var reference in References)
            {
                reference.Destroy();
            }

            References.Clear();
        }

        public new bool CanCharacterChangeName(INetActor character)
        {
            return false;
        }
    }
}