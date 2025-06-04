using Discord.WebSocket;
using NVMP.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NVMP.BuiltinServices
{
    /// <summary>
    /// Configurations for the DiscordPlayerManager.
    /// </summary>
    [Flags]
    public enum DiscordPlayerManagerFlags
    {
        None = 0,

        /// <summary>
        /// If the user's lifetime in the Discord server changes, then their in-game state changes. This means that if
        /// they are kicked or leave the Discord server, then they are removed from the game additionally.
        /// Additionally with this flag, the user cannot join the server if they are not within the Discord server's primary guild.
        /// </summary>
        PlayerLifetimeLinked = (1 << 0),

        /// <summary>
        /// Sets the player's name (and defaults the initial actor's name) to their Discord username (not display name).
        /// </summary>
        PlayerNameLinked     = (1 << 1)
    }

    /// <summary>
    /// The Discord player manager allows you to link your in-game players to their Discord account credentials, and configure how they
    /// represent in-game additionally. This manager by default enforces all players joining to have Discord linked on their Epic account,
    /// and if not - then they are kicked. See DiscordPlayerManagerFlags for additional configurations you can enable.
    /// 
    /// This manager also adds IPlayerRole instances to players associated to their Discord role, and this automatically is kept in sync
    /// with remote role changes.
    /// 
    /// Important tid-bits:
    /// * GuildId must be set to a Guild your Discord Client socket has moderation permissions on.
    /// * This manager also subscribes INetPlayer.Ban to ban via the Discord guild.
    /// * RoleWhiteList is optional, but if set requires players to have all the roles set in it's array to connect and play.
    /// </summary>
    public class DiscordPlayerManager : IPlayerManager
    {
        internal DiscordSocketClient Discord;

        /// <summary>
        /// Behaviour flags of the player manager.
        /// </summary>
        public DiscordPlayerManagerFlags Flags { get; set; } = DiscordPlayerManagerFlags.None;

        /// <summary>
        /// The Discord Guild ID this player manager is bound to.
        /// </summary>
        public ulong GuildId { get; set; } = 0;

        /// <summary>
        /// Sets a role white-list that only permits users that have this role on their Discord account from joining and playing.
        /// If this is set to NULL, then any role is permitted.
        /// PlayerLifetimeLinked must be set to use this property, as it depends on Guild information.
        /// </summary>
        public ulong[] RoleWhiteList = null;

        internal List<IPlayerRole> InternalRoles = new List<IPlayerRole>();
        public IPlayerRole[] Roles => InternalRoles.ToArray();

        [DllImport("Native", EntryPoint = "GameServer_SetServerAccountTypesUsed")]
        internal static extern void Internal_SetServerAccountTypesUsed
        (
            [MarshalAs(UnmanagedType.LPArray)]
            NetPlayerAccountType[] types,
            int numTypes
        );

        public NetPlayerAccountType[] AccountTypesUsed
        {
            set => Internal_SetServerAccountTypesUsed(value, value.Length);
        }

        internal class DiscordPlayerRole : IPlayerRole
        {
            public string Name { get; internal set; }

            public ulong Id { get; internal set; }

            public bool IsPrivate { get; internal set; }

            public ICollection<IRoleScope> Scopes { get; internal set; } =  new List<IRoleScope>();
        }

        public DiscordPlayerManager(DiscordSocketClient discordSocketClient)
        {
            Discord = discordSocketClient;
            Discord.Ready += Discord_Ready;
            Discord.GuildMemberUpdated += Discord_GuildMemberUpdated;
            Discord.UserLeft += Discord_UserLeft;

            // Register middleware that on a new authentication from a player, monitors the player's Discord authentication state
            // for if they do behaviour within the server which should remove or amend their state.
            Factory.Player.OnConnect += Player_OnCreateMiddleware;

            AccountTypesUsed = new NetPlayerAccountType[] { NetPlayerAccountType.EpicGames, NetPlayerAccountType.Discord };
        }

        public void Dispose()
        {
            if (Discord != null)
            {
                Discord.Ready -= Discord_Ready;
                Discord.GuildMemberUpdated -= Discord_GuildMemberUpdated;
                Discord.UserLeft -= Discord_UserLeft;
            }

            Factory.Player.OnConnect -= Player_OnCreateMiddleware;
        }

        private void Player_OnCreateMiddleware(INetPlayer player)
        {
            player.OnAuthenticated += Player_OnAuthenticated;
            player.OnBanned += Player_OnBanned;
        }

        internal INetPlayer FindPlayerForDiscordUserId(ulong id)
        {
            // Find if the user is in our player list
            INetPlayer player = null;
            foreach (INetPlayer checkingPlayer in Factory.Player.All)
            {
                IPlayerAccount discordAuth = checkingPlayer.GetAuthenticatedAccount(NetPlayerAccountType.Discord);
                if (discordAuth == null)
                    continue;

                if (discordAuth.Id == id.ToString())
                {
                    player = checkingPlayer;
                    break;
                }
            }

            return player;
        }

        private Task Discord_UserLeft(SocketGuild guild, SocketUser user)
        {
            INetPlayer player = FindPlayerForDiscordUserId(user.Id);
            if (player == null)
                return Task.CompletedTask;

            player.Kick("You left the community server");

            return Task.CompletedTask;
        }

        // For guild member updates, we want to check if their roles have changed and no longer represent their
        // in-game permissions, or permission into the server
        private Task Discord_GuildMemberUpdated(Discord.Cacheable<SocketGuildUser, ulong> beforeCache, SocketGuildUser after)
        {
            INetPlayer player = FindPlayerForDiscordUserId(after.Id);
            if (player == null)
                return Task.CompletedTask;

            // Update the player roles
            player.RemoveAllRoles();

            IEnumerable<ulong> discordRoleIds = after.Roles.Select(v => v.Id);
            foreach (var role in discordRoleIds)
            {
                var gameRole = InternalRoles.Where(gameRole => gameRole.Id == role).FirstOrDefault();
                if (gameRole == null)
                    continue;

                player.TryAddToRole(gameRole);
            }

            // Enforce the whitelist guard for if the player no longer has the roles for this server
            if (RoleWhiteList != null)
            {
                bool bWhitelisted = false;
                foreach (ulong whitelistRole in RoleWhiteList)
                {
                    if (discordRoleIds.Contains(whitelistRole))
                    {
                        bWhitelisted = true;
                        break;
                    }
                }

                if (!bWhitelisted)
                {
                    player.Kick($"You are not whitelisted to join this server!");
                    return Task.CompletedTask;
                }
            }

            return Task.CompletedTask;
        }

        private async Task Discord_Ready()
        {
            if (GuildId == 0)
                throw new Exception("GuildId needs to be set.");

            // We want to setup local game roles
            var guild = Discord.GetGuild(GuildId);
            if (guild == null)
                throw new Exception("No valid guild registered locally");

            if (InternalRoles.Count != 0)
            {
                return;
            }

            foreach (var role in guild.Roles)
            {
                var gameRole = new DiscordPlayerRole()
                {
                    Id = role.Id,
                    Name = role.Name,
                    IsPrivate = !role.IsHoisted
                };
                
                if (!InternalRoles.Where(_role => _role.Id == gameRole.Id).Any())
                {
                    InternalRoles.Add(gameRole);
                }
            }

            // remove any old permissions
            var rolesToRemove = new List<IPlayerRole>();
            foreach (var role in InternalRoles)
            {
                if (!guild.Roles.Where(_role => _role.Id == role.Id).Any())
                {
                    rolesToRemove.Add(role);
                }
            }

            foreach (var role in rolesToRemove)
            {
                InternalRoles.Remove(role);
            }

            await Task.CompletedTask;

        }

        private void Player_OnBanned(INetPlayer player, string reason)
        {
            var discordAuth = player.GetAuthenticatedAccount(NetPlayerAccountType.Discord);
            if (discordAuth == null)
                return;

            var guild = Discord.GetGuild(GuildId);
            Task.Run(async () => await guild.AddBanAsync(ulong.Parse(discordAuth.Id), reason: reason));
        }

        private void Player_OnAuthenticated(INetPlayer player)
        {
            // Player has authenticated, so let's check any specific flags.
            if (Flags.HasFlag(DiscordPlayerManagerFlags.PlayerLifetimeLinked))
            {
                if (GuildId == 0)
                    throw new Exception("GuildId has not been implemented for PlayerLifetimeLinked to work correctly");

                var primaryGuild = Discord.GetGuild(GuildId);
                if (primaryGuild == null)
                    throw new Exception("PrimaryGuild was not found by the bot");

                // Get the player's Discord authentication details
                var discordAuth = player.GetAuthenticatedAccount(NetPlayerAccountType.Discord);
                if (discordAuth == null)
                {
                    player.Kick($"This server requires you to link your Discord to your Epic Games account!");
                    return;
                }

                ulong discordId = ulong.Parse( discordAuth.Id );

                var discordUser = Discord.Rest.GetGuildUserAsync(primaryGuild.Id, discordId).GetAwaiter().GetResult();
                if (discordUser == null)
                {
                    player.Kick($"You must be invited to (or join) this server's community server to be able to play!");
                    return;
                }

                if (RoleWhiteList != null)
                {
                    bool bWhitelisted = false;
                    foreach (ulong whitelistRole in RoleWhiteList)
                    {
                        if (discordUser.RoleIds.Contains(whitelistRole))
                        {
                            bWhitelisted = true;
                            break;
                        }
                    }

                    if (!bWhitelisted)
                    {
                        player.Kick($"You are not whitelisted to join this server!");
                        return;
                    }
                }

                foreach (var role in discordUser.RoleIds)
                {
                    var gameRole = InternalRoles.Where(gameRole => gameRole.Id == role).FirstOrDefault();
                    if (gameRole == null)
                        continue;

                    player.TryAddToRole(gameRole);
                }
            }

            if (Flags.HasFlag(DiscordPlayerManagerFlags.PlayerNameLinked))
            {
                // Get the player's Discord authentication details
                var discordAuth = player.GetAuthenticatedAccount(NetPlayerAccountType.Discord);
                if (discordAuth == null)
                {
                    player.Kick($"This server requires you to link your Discord to your Epic Games account!");
                    return;
                }

                string displayName = discordAuth.DisplayName;
                string[] discordTokens = displayName.Split("#");
                if (discordTokens.Length == 2)
                {
                    if (discordTokens[1] == "0")
                    {
                        // just format as the username. discord abandoned these identifiers so that unique usernames were 
                        // now required. lets be friendly to it.
                        displayName = discordTokens[0];
                    }
                }

                player.Name = displayName;
                if (player.Actor != null)
                {
                    player.Actor.Name = player.Name;
                }
            }
        }

        public void AddScopeToRoles(ulong[] roleIds, IRoleScope scope)
        {
            foreach (ulong roleId in roleIds)
            {
                var gameRole = InternalRoles.Where(role => role.Id == roleId).FirstOrDefault();
                gameRole.Scopes.Add(scope);
            }
        }

        public void AddScopesToRole(ulong roleId, IRoleScope[] scopes)
        {
            var gameRole = InternalRoles.Where(role => role.Id == roleId).FirstOrDefault();
            foreach (IRoleScope scope in scopes)
            {
                gameRole.Scopes.Add(scope);
            }
        }
    }
}
