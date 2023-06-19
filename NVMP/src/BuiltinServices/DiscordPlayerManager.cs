using Discord.WebSocket;
using NVMP.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NVMP.BuiltinServices
{
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
        public ulong GuidId { get; set; } = 0;

        internal List<IPlayerRole> InternalRoles = new List<IPlayerRole>();
        public IPlayerRole[] Roles => InternalRoles.ToArray();

        public NetPlayerAccountType[] AccountTypesUsed { get; internal set; }

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

            AccountTypesUsed = new NetPlayerAccountType[] { NetPlayerAccountType.EpicGames, NetPlayerAccountType.Discord };
        }

        private Task Discord_GuildMemberUpdated(Discord.Cacheable<SocketGuildUser, ulong> arg1, SocketGuildUser arg2)
        {
            return Task.CompletedTask;
        }

        private async Task Discord_Ready()
        {
            // We want to setup local game roles
            var guild = Discord.GetGuild(GuidId);
            if (guild == null)
                throw new Exception("No valid guild registered locally");

            foreach (var role in guild.Roles)
            {
                var gameRole = new DiscordPlayerRole()
                {
                    Id = role.Id,
                    Name = role.Name,
                    IsPrivate = !role.IsHoisted
                };

                InternalRoles.Add(gameRole);
            }

            await Task.CompletedTask;

        }

        private void Player_OnCreateMiddleware(INetPlayer obj)
        {
            obj.OnAuthenticated += Player_OnAuthenticated;
        }

        private void Player_OnAuthenticated(INetPlayer player)
        {
            // Player has authenticated, so let's check any specific flags.
            if (Flags.HasFlag(DiscordPlayerManagerFlags.PlayerLifetimeLinked))
            {
                if (GuidId == 0)
                    throw new Exception("GuildId has not been implemented for PlayerLifetimeLinked to work correctly");

                var primaryGuild = Discord.GetGuild(GuidId);
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

                player.Name = discordAuth.DisplayName;
                if (player.Actor != null)
                {
                    player.Actor.Name = player.Name;
                }
            }
        }

        public void RegisterMiddleware()
        {
            // Register middleware that on a new authentication from a player, monitors the player's Discord authentication state
            // for if they do behaviour within the server which should remove or amend their state.
            Factory.Player.OnCreateMiddleware += Player_OnCreateMiddleware;
        }

        public void UnregisterMiddleware()
        {
            // Removes the middleware
            Factory.Player.OnCreateMiddleware -= Player_OnCreateMiddleware;
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
