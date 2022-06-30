using RestSharp;
using System;
using System.Runtime.Serialization;
using System.Web;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using NVMP.Entities;
using System.Net;
using NVMP.BuiltinServices.ManagedWebService;

namespace NVMP.Authenticator.Discord
{
    internal class DiscordAuthenticatorImpl : Basic.BasicAuthenticatorImpl, IDiscordAuthenticator
    {
        private static readonly string AuthenticatorName = "discord";
        private static readonly string DiscordAPI = "https://discord.com/api";
        private static readonly string NVMPAPI = "https://nv-mp.com/";
        private static readonly string ServerToLauncherRedirectURL = "http://localhost:8085/discord";

        private string CachedDiscordClientID;
        private string CachedDiscordSecret;
        private string CachedAuthenticatorHostname;

        private string CachedDiscordBotToken;
        private string CachedDiscordSyncGuildID;
        private string CachedDiscordLogChannelID;

        private DiscordSocketClient BotClientSocket;
        private RestGuild SynchronisedGuild;

        //
        // corporal temp
        //
        private List<ulong> PermittedRoles = new List<ulong> { } ;

        private class AuthReceivedEventArgs : EventArgs
        {
            public HttpListenerResponse Response { get; set; }
            public string AuthCode { get; set; }
        }


        private Dictionary<string, DiscordAuthorizationSession> AuthSessions;
        private Dictionary<ulong, List<string>> DiscordRoleScopes;

        private IManagedWebService WebService;

        public Func<RestGuildUser, string, Task> OnDiscordUserRemotelyBanned { get; set; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public DiscordAuthenticatorImpl(IManagedWebService webService) : base()
        {
            WebService = webService;
            OnDiscordUserRemotelyBanned = null;

            Debugging.Write("Setting up Discord auth configs...");
            NativeSettings.SetStringValue("Server", "Authenticator", AuthenticatorName);

            NativeSettings.SetupDefaultString("Server", "AuthenticatorDiscordClientID", "");
            NativeSettings.SetupDefaultString("Server", "AuthenticatorDiscordSecret", "");
            NativeSettings.SetupDefaultString("Server", "AuthenticatorDiscordBotToken", "");
            NativeSettings.SetupDefaultString("Server", "AuthenticatorDiscordSyncGuildID", "");
            NativeSettings.SetupDefaultString("Server", "AuthenticatorDiscordLogChannelID", "");

            CachedDiscordClientID = NativeSettings.GetStringValue("Server", "AuthenticatorDiscordClientID");
            CachedDiscordSecret = NativeSettings.GetStringValue("Server", "AuthenticatorDiscordSecret");
            CachedAuthenticatorHostname = NativeSettings.GetStringValue("Server", "WebHostname");

            CachedDiscordBotToken = NativeSettings.GetStringValue("Server", "AuthenticatorDiscordBotToken");
            CachedDiscordSyncGuildID = NativeSettings.GetStringValue("Server", "AuthenticatorDiscordSyncGuildID");
            CachedDiscordLogChannelID = NativeSettings.GetStringValue("Server", "AuthenticatorDiscordLogChannelID");

            AuthSessions = new Dictionary<string, DiscordAuthorizationSession>();
            DiscordRoleScopes = new Dictionary<ulong, List<string>>();

            int servingPortName = (int)NativeSettings.GetFloatValue("Server", "WebPort");

            if (CachedDiscordClientID == null ||
                CachedDiscordClientID.Length == 0)
                throw new Exception("AuthenticatorDiscordClientID is not set!");

            if (CachedDiscordSecret == null ||
                CachedDiscordSecret.Length == 0)
                throw new Exception("AuthenticatorDiscordSecret is not set!");

            if (CachedAuthenticatorHostname == null ||
                CachedAuthenticatorHostname.Length == 0)
                throw new Exception("CachedAuthenticatorHostname is not set! This should be your public WAN IP or a valid hostname to connect to the gameserver. Format is [https/http]://[hostname], do not specify port!");

            if (servingPortName == 0)
                throw new Exception("AuthenticatorPortOverride is not set! Specify a port that is guarenteed to be open!");

            if (CachedDiscordSyncGuildID != null &&
                CachedDiscordSyncGuildID != "")
            {
                if (CachedDiscordBotToken == null ||
                    CachedDiscordBotToken == "")
                {
                    throw new Exception("If attempting to synchronise guild information, AuthenticatorDiscordBotToken must be set!");
                }
            }

            if (CachedDiscordBotToken != null &&
                CachedDiscordBotToken != "")
            {
                try
                {
                    var intents =
                            GatewayIntents.GuildMembers |
                            GatewayIntents.DirectMessages |
                            GatewayIntents.GuildMessages |
                            GatewayIntents.Guilds |
                            GatewayIntents.GuildPresences;

                    var config = new DiscordSocketConfig
                    {
                        AlwaysDownloadUsers = false,
                        GatewayIntents = intents
                    };

                    Debugging.Write("Creating Discord socket...");
                    BotClientSocket = new DiscordSocketClient(config);
                    BotClientSocket.LoggedIn += new Func<Task>(delegate ()
                    {
                        if (CachedDiscordSyncGuildID != "")
                        {
                            SynchronisedGuild = BotClientSocket.Rest.GetGuildAsync(ulong.Parse(CachedDiscordSyncGuildID)).GetAwaiter().GetResult();
                            if (SynchronisedGuild == null)
                            {
                                throw new Exception("Bot is not part of the specified guild ID " + CachedDiscordSyncGuildID);
                            }
                            else
                            {
                                Debugging.Write("Synchronised guild found!");
                            }
                        }

                        return Task.CompletedTask;
                    });
                    BotClientSocket.Log += new Func<LogMessage, Task>(delegate (LogMessage message)
                    {
                        Debugging.Write("[discord] " + message.Message);
                        return Task.CompletedTask;
                    });

                    BotClientSocket.Ready += new Func<Task>(delegate ()
                    {
                        Debugging.Write($"Discord bot authenticated successfully! Watching {BotClientSocket.Guilds.Count} guilds");
                        return Task.CompletedTask;
                    });

                    BotClientSocket.UserLeft += new Func<SocketGuild, SocketUser, Task>(delegate (SocketGuild guild, SocketUser user)
                    {
                        // Find if there's an authentication session associated with the changed user
                        var session = AuthSessions.Where(authsession => authsession.Value.RestClient.CurrentUser.Id == user.Id)
                            .FirstOrDefault();

                        if (!session.Equals(default(KeyValuePair<string, DiscordAuthorizationSession>)))
                        {
                            // Valid session found, find the player associated
                            var players = NetPlayer.All;
                            var player = players.Where(netplayer => netplayer.AuthenticationToken == session.Key)
                                .FirstOrDefault();

                            if (player != null)
                            {
                                Log($"<@{session.Value.RestClient.CurrentUser.Id}> ({session.Value.RestClient.CurrentUser.Username}#{session.Value.RestClient.CurrentUser.Discriminator}) lost their passport - they left the server!");
                                player.Kick("Passport Revoked - you left the Discord server!", null, true);
                            }
                            else
                            {
                                Debugging.Error("Player changed their Discord information, and their session is in game, but NetPlayer could not be found!");
                            }
                        }

                        return Task.CompletedTask;
                    });

                    BotClientSocket.GuildMemberUpdated += new Func<Cacheable<SocketGuildUser, ulong>, SocketGuildUser, Task>(async delegate (Cacheable<SocketGuildUser, ulong> beforeCache, SocketGuildUser after)
                    {
                        // Find if there's an authentication session associated with the changed user
                        var session = AuthSessions.Where(authsession => authsession.Value.RestClient.CurrentUser.Id == after.Id)
                            .FirstOrDefault();

                        var before = await beforeCache.GetOrDownloadAsync();
                        if (before == null)
                        {
                            return;
                        }

                        if (before.Roles.SequenceEqual(after.Roles))
                        {
                            return;
                        }

                        if (!session.Equals(default(KeyValuePair<string, DiscordAuthorizationSession>)))
                        {
                            // Valid session found, find the player associated
                            var players = NetPlayer.All;
                            var player = players.Where(netplayer => netplayer.AuthenticationToken == session.Key)
                                .FirstOrDefault();

                            if (player != null)
                            {
                                if (PermittedRoles.Count() != 0 && !after.Roles.Where(role => PermittedRoles.Contains(role.Id)).Any())
                                {
                                    Log($"<@{session.Value.RestClient.CurrentUser.Id}> ({session.Value.RestClient.CurrentUser.Username}#{session.Value.RestClient.CurrentUser.Discriminator}) lost their passport!");
                                    player.Kick("Passport Revoked!", null, true);
                                }
                            }
                            else
                            {
                                Debugging.Error("Player changed their Discord information, and their session is in game, but NetPlayer could not be found!");
                            }
                        }
                    });
                    BotClientSocket.LoggedOut += new Func<Task>(delegate ()
                    {
                        Debugging.Error("Discord bot has logged out");
                        return Task.CompletedTask;
                    });

                    Debugging.Write("Logging into Discord...");
                    BotClientSocket.LoginAsync(TokenType.Bot, CachedDiscordBotToken).GetAwaiter().GetResult();
                    BotClientSocket.StartAsync().GetAwaiter().GetResult();
                    BotClientSocket.SetGameAsync("New Vegas: Multiplayer").GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public void AddWebListener()
        {
            Debugging.Write("Adding web resolver...");
            WebService.AddPathResolver("discord", async (HttpListenerRequest req, HttpListenerResponse resp) =>
            {
                var authCode = req.QueryString.Get("code");
                if (authCode == null)
                {
                    resp.Close();
                }
                else
                {
                    var args = new AuthReceivedEventArgs
                    {
                        AuthCode = authCode,
                        Response = resp
                    };
                    OnDiscordAuthReceived(args);
                }
                await Task.CompletedTask;
            });
        }

        internal string GetStatusErrorURL(DiscordAuthorizationStatusTypes status)
        {
            switch (status)
            {
                case DiscordAuthorizationStatusTypes.APIError:
                    return $"{NVMPAPI}error/server_error/api";
                case DiscordAuthorizationStatusTypes.InternalServerError:
                    return $"{NVMPAPI}error/server_error";
                case DiscordAuthorizationStatusTypes.OAuthResponseNull:
                    return $"{NVMPAPI}error/oauth_response_null";
                case DiscordAuthorizationStatusTypes.SessionInUse:
                    return $"{NVMPAPI}error/session_in_use";
                case DiscordAuthorizationStatusTypes.Unauthorized:
                    return $"{NVMPAPI}error/unauthorized";
                case DiscordAuthorizationStatusTypes.AuthorizationSuccessful:
                    return null;
                default: return null;
            }
        }

        private async void OnDiscordAuthReceived(AuthReceivedEventArgs args)
        {
            Debugging.Write("Authentication received");

            if (args.AuthCode == null || args.AuthCode.Length < 1)
            {
                args.Response.Close();
                return;
            }

            var authenticatorResult = await TryAuthorization(args.AuthCode);
            if (authenticatorResult.Status != DiscordAuthorizationStatusTypes.AuthorizationSuccessful)
            {
                var redirectUrl = GetStatusErrorURL(authenticatorResult.Status);
                if (redirectUrl != null)
                {
                    args.Response.Redirect(redirectUrl);
                }

                args.Response.Close();
                return;
            }

            args.Response.Redirect($"{ServerToLauncherRedirectURL}/?server={CachedDiscordClientID}&response={HttpUtility.UrlEncode(authenticatorResult.Result)}");
            args.Response.Close();
        }

        override public string GetAuthenticationURL()
        {
            return $"{WebService.FullURL}/discord";
        }

        override public string GetClientID()
        {
            return CachedDiscordClientID;
        }

        override public void Update()
        {
            var now = DateTimeOffset.UtcNow;
            var expiredSessions = AuthSessions.Where(session => session.Value.ExpiresAt.HasValue && session.Value.ExpiresAt < now)
                .Select(kv => kv.Key)
                .ToList();

            foreach (string k in expiredSessions)
            {
                Debugging.Write($"Session {AuthSessions[k].ConnectionID} has expired!");
                AuthSessions.Remove(k);
            }
        }

        override public void PlayerLeft(NetPlayer player)
        {
            if (AuthSessions.ContainsKey(player.AuthenticationToken))
            {
                AuthSessions.Remove(player.AuthenticationToken);
            }
        }

        override public bool IsScopeValid(NetPlayer player, string scope)
        {
            var roles = player["DiscordServerRoles"];
            if (roles != null && roles.Length > 0)
            {
                var rolesList = roles.Split(",");
                foreach (var role in rolesList)
                {
                    List<string> scopes;
                    if (DiscordRoleScopes.TryGetValue(ulong.Parse(role), out scopes))
                    {
                        if (scopes.Contains(scope))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        override public void SetupAuthentication(NetPlayer player)
        {
            // If this player was authenticated locally, the socket should have this information cached to make this authentication process almost instant
            if (AuthSessions.ContainsKey(player.AuthenticationToken))
            {
                var session = AuthSessions[player.AuthenticationToken];
                var accountUsername = $"{session.RestClient.CurrentUser.Username}#{session.RestClient.CurrentUser.Discriminator}";

                // Set up the player's custom data
                player["UniqueID"] = session.RestClient.CurrentUser.Id.ToString();
                player["DiscordUsername"] = accountUsername;
                player["DiscordServerRoles"] = "";
                player["DiscordServerNickname"] = accountUsername;

                player.Name = accountUsername;
                player.Authenticated = true;

                // Set up the player's actor with Discord custom styling
                var actor = player.Actor;
                if (actor != null)
                {
                    // Set their name by default, a parent program may override this regardless.
                    actor.Name = $"{player.Name}";

                    // Synchronise the guild information
                    if (session.CurrentGuildUser != null)
                    {
                        var roles = new List<RestRole>();
                        RestRole topRole = null;

                        var userRolesSortedByPosition = SynchronisedGuild.Roles
                            .OrderByDescending(role => role.Position)
                            .Where(role => session.CurrentGuildUser.RoleIds.Contains(role.Id))
                            .ToList();

                        foreach (var role in userRolesSortedByPosition)
                        {
                            if (role != null && role.Name != "@everyone")
                            {
                                if (topRole == null)
                                {
                                    //if (role.IsHoisted)
                                    {
                                        topRole = role;
                                    }
                                }

                                roles.Add(role);
                            }
                        }

                        player["DiscordServerRoles"] = String.Join(",", roles.Select(role => role.Id.ToString()).ToArray());

                        string serverNick = session.CurrentGuildUser.Nickname ?? player.Name;
                        if (serverNick == accountUsername)
                        {
                            int discriminatorStartPos = serverNick.IndexOf("#");
                            if (discriminatorStartPos != -1)
                            {
                                serverNick = serverNick.Substring(0, discriminatorStartPos);
                            }
                        }

                        player["DiscordServerNickname"] = serverNick;

                        if (topRole != null && topRole.Color != Color.Default)
                        {
                            player["DiscordServerTopRoleName"] = topRole.Name;
                            player["DiscordServerTopRoleColor"] = topRole.Color.ToString();
                        }
                    }
                }

                return;
            }

            Debugging.Error("Invalid authentication session");
        }

        override public bool IsAuthenticationValid(NetPlayer player, string authenticationToken, ref string badauthReason)
        {
            bool isIPBanned = !base.IsAuthenticationValid(player, authenticationToken, ref badauthReason);

            if (CachedDiscordSyncGuildID != null &&
                CachedDiscordSyncGuildID != "")
            {
                if (BotClientSocket == null)
                {
                    throw new Exception("Bot client does not exist. This must exist to sync guild information!");
                }

                // For blob data assigned, try to treat it as an authorization token, as a direct flow (insteaf of captive website)
                if (authenticationToken.StartsWith("{"))
                {
                    var authenticatorResult = TryAuthorization(authenticationToken).GetAwaiter().GetResult();
                    if (authenticatorResult.Status != DiscordAuthorizationStatusTypes.AuthorizationSuccessful)
                    {
                        badauthReason = $"Authorization Invalid";
                        return false;
                    }

                    // Fall through because now AuthSessions will have a valid session
                    try
                    {
                        var result = JsonConvert.DeserializeObject<DiscordOAuthResponse>(authenticatorResult.Result);
                        authenticationToken = result.AccessToken;

                        player.AuthenticationToken = authenticationToken;
                    }
                    catch (Exception)
                    {
                        badauthReason = "Authorization Invalid";
                        return false;
                    }
                }

                // If this player was authenticated locally, the socket should have this information cached to make this authentication process almost instant
                if (AuthSessions.ContainsKey(authenticationToken))
                {
                    var session = AuthSessions[authenticationToken];

                    if (session.ExpiresAt.HasValue)
                    {
                        session.ExpiresAt = null;
                        session.ConnectionID = player.ConnectionID;
                        session.RestClient.LoggedOut += new Func<System.Threading.Tasks.Task>(delegate ()
                        {
                            Debugging.Write("Restclient logged out");

                            if (session.ConnectionID != 0)
                            {
                                NetPlayer existingPlayer = NetPlayer.GetPlayerByConnectionID(session.ConnectionID);
                                existingPlayer.Kick("This Discord account was logged out");
                            }

                            AuthSessions.Remove(authenticationToken);
                            return Task.CompletedTask;
                        });

                        if (session.RestClient.CurrentUser == null)
                        {
                            badauthReason = "Discord API failed to query your user";
                            AuthSessions.Remove(authenticationToken);
                            return false;
                        }

                        if (isIPBanned)
                        {
                            badauthReason = $"{session.RestClient.CurrentUser.Username} is banned";
                            return false;
                        }

                        // If there is already a session with this Discord user, kick the connection
                        if (AuthSessions.Values
                            .Where(_bsession => _bsession != session && _bsession.RestClient.CurrentUser?.Id == session.RestClient.CurrentUser.Id).Any())
                        {
                            badauthReason = "This account is already logged in";

                            // Maybe tell the user about this?
                            return false;
                        }

                        Debugging.Write($"{session.RestClient.CurrentUser.Username}#{session.RestClient.CurrentUser.Discriminator} authenticated successfully!");
                        return true;
                    }
                    else
                    {
                        badauthReason = "This access token is already in use";
                        return false;
                    }
                }
                else
                {
                    badauthReason = "Access token was not previously registered";
                    return false;
                }
            }

            badauthReason = "Server Error";
            return false;
        }

        override public void Log(string message)
        {
            try
            {
                if (CachedDiscordLogChannelID.Length != 0)
                {
                    // Likely not threadsafe at all
                    new Task(async () =>
                    {
                        try
                        {
                            var channel = await SynchronisedGuild.GetTextChannelAsync(ulong.Parse(CachedDiscordLogChannelID));
                            if (channel != null)
                            {
                                await channel.SendMessageAsync(message);
                            }
                        } catch (Exception e)
                        {
                            Debugging.Error(e);
                        }
                    }).Start();
                }
            }
            catch (Exception e)
            {
                Debugging.Error(e.Message + "\n" + e.StackTrace);
            }
        }

        override public void Ban(NetPlayer player, string reason = null)
        {
            var userID = player["UniqueID"];
            if (userID == null)
            {
                base.Ban(player);
            }
            else
            {
                try
                {
                    ulong id = ulong.Parse(userID);
                    BanDiscordID(id, reason);
                }
                catch (Exception)
                {
                    base.Ban(player);
                }
            }
        }

        public void BanDiscordID(ulong id, string reason = null)
        {
            new Task(async () =>
            {
                var user = await SynchronisedGuild.GetUserAsync(id);
                if (user != null)
                {
                    await SynchronisedGuild.AddBanAsync(user);
                    await OnDiscordUserRemotelyBanned?.Invoke(user, reason);
                }
            }).Start();
        }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        public void AddPermittedRole(ulong role)
        {
            PermittedRoles.Add(role);
        }


        public async Task<DiscordAuthorizationSession> TryAuthentication(string authtoken)
        {
            // Create a Discord socket for this connection, and set it to expire in 5 minutes - unless a player starts to actively use it.
            // This is early caching
            var session = new DiscordAuthorizationSession
            {
                RestClient = new DiscordRestClient()
            };

            try
            {
                await session.RestClient.LoginAsync(TokenType.Bearer, authtoken);
                if (session.RestClient.LoginState != LoginState.LoggedIn)
                {
                    throw new Exception("Could not log in user");
                }

                // Requires SERVER MEMBERS INTENT
                session.CurrentGuildUser = await SynchronisedGuild.GetUserAsync(session.RestClient.CurrentUser.Id);
                if (session.CurrentGuildUser == null)
                    throw new Exception("Not a member of server");

                return session;
            }
            catch (Exception e)
            {
                Debugging.Error("Error parsing Discord authentication block: " + e.Message + "\n" + e.StackTrace);
            }

            return null;
        }

        public async Task<DiscordAuthorizationResult> TryAuthorization(string blob, bool mustNotBeInGame = true, bool persistSession = true, string redirectUriOverride = null)
        {
            if (blob == null || blob.Length < 1)
            {
                return new DiscordAuthorizationResult { Status = DiscordAuthorizationStatusTypes.InternalServerError };
            }

            DiscordOAuthResponse oauthResponse = null;

            bool isAuthorizationToken = blob.StartsWith("{");
            if (!isAuthorizationToken)
            {
                // Get an authorization token from the code sent by Discord, then pass this over to the connection
                try
                {
                    var client = new RestClient(DiscordAPI);
                    var request = new RestRequest("/oauth2/token", Method.POST);

                    request.AddHeader("content-type", "application/x-www-form-urlencoded");
                    request.AddParameter("application/x-www-form-urlencoded",
                        $"client_id={CachedDiscordClientID}&client_secret={CachedDiscordSecret}&grant_type=authorization_code&code={blob}&redirect_uri={redirectUriOverride ?? WebService.FullURL + "/discord"}&scope=identify", ParameterType.RequestBody);

                    IRestResponse response = await client.ExecuteAsync(request);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        oauthResponse = JsonConvert.DeserializeObject<DiscordOAuthResponse>(response.Content);
                    }
                    else
                    {
                        Debugging.Error("Error converting Discord authenticator token to authorization, returned " + response.ErrorMessage + ", " + response.Content);
                        return new DiscordAuthorizationResult { Status = DiscordAuthorizationStatusTypes.InternalServerError };
                    }
                }
                catch (Exception e)
                {
                    Debugging.Error("Error parsing Discord information: " + e.Message + "\n" + e.StackTrace);
                    return new DiscordAuthorizationResult { Status = DiscordAuthorizationStatusTypes.InternalServerError };
                }
            }
            else
            {
                // Existing authorization token passed from a previous API interaction, try and use it
                try
                {
                    oauthResponse = JsonConvert.DeserializeObject<DiscordOAuthResponse>(blob);
                }
                catch (Exception e)
                {
                    Debugging.Error("Error parsing Discord authorization block: " + e.Message + "\n" + e.StackTrace);
                    return new DiscordAuthorizationResult { Status = DiscordAuthorizationStatusTypes.InternalServerError };
                }
            }

            if (oauthResponse == null)
            {
                return new DiscordAuthorizationResult { Status = DiscordAuthorizationStatusTypes.OAuthResponseNull };
            }
            
            if (mustNotBeInGame)
            {
                // Verify if the OAuth response is good to be used
                if (AuthSessions.ContainsKey(oauthResponse.AccessToken))
                {
                    Debugging.Error("Error, this token is already in use!");
                    return new DiscordAuthorizationResult { Status = DiscordAuthorizationStatusTypes.SessionInUse };
                }
            }

            // Create a Discord socket for this connection, and set it to expire in 5 minutes - unless a player starts to actively use it.
            // This is early caching
            var session = new DiscordAuthorizationSession
            {
                RestClient = new DiscordRestClient(),
                ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(30)
            };

            Debugging.Write("Logging in received token as a client socket session...");
            try
            {
                await session.RestClient.LoginAsync(TokenType.Bearer, oauthResponse.AccessToken);
                if (session.RestClient.LoginState != LoginState.LoggedIn)
                {
                    throw new Exception("Could not log in user");
                }
            }
            catch (Exception e)
            {
                Debugging.Error("Error parsing Discord authorization block: " + e.Message + "\n" + e.StackTrace);
                return new DiscordAuthorizationResult { Status = DiscordAuthorizationStatusTypes.APIError };
            }

            if (CachedDiscordSyncGuildID != null &&
                CachedDiscordSyncGuildID != "")
            {
                try
                {
                    // Requires SERVER MEMBERS INTENT
                    session.CurrentGuildUser = await SynchronisedGuild.GetUserAsync(session.RestClient.CurrentUser.Id);
                    if (session.CurrentGuildUser == null)
                    {
                        return new DiscordAuthorizationResult { Status = DiscordAuthorizationStatusTypes.NotADiscordMember };
                    }

                    if (PermittedRoles.Count() != 0 && !session.CurrentGuildUser.RoleIds.Where(role => PermittedRoles.Contains(role)).Any())
                    {
                        Log($"<@{session.RestClient.CurrentUser.Id}> ({session.RestClient.CurrentUser.Username}#{session.RestClient.CurrentUser.Discriminator}) tried logging in without a passport!");

                        Debugging.Error("Error, unauthorized role!");
                        return new DiscordAuthorizationResult { Status = DiscordAuthorizationStatusTypes.Unauthorized };
                    }
                }
                catch (Exception e)
                {
                    Debugging.Error("You likely need to allow this bot to access 'SERVER MEMBERS INTENT'!");
                    throw e; // rethrow
                }
            }
            else
            {
                session.CurrentGuildUser = null;
            }

            if (persistSession)
            {
                AuthSessions[oauthResponse.AccessToken] = session;
            }

            return new DiscordAuthorizationResult { Status = DiscordAuthorizationStatusTypes.AuthorizationSuccessful, Result = JsonConvert.SerializeObject(oauthResponse) };
        }

        public void AddScopeToRole(ulong role, string scope)
        {
            List<string> roles;
            if (DiscordRoleScopes.TryGetValue(role, out roles))
            {
                if (!roles.Contains(scope))
                {
                    roles.Add(scope);
                }
            }
            else
            {
                DiscordRoleScopes[role] = new List<string> { scope };
            }
        }
    }
}