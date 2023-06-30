using NVMP.BuiltinServices;
using NVMP.BuiltinServices.ModDownloadService;
using NVMP.Entities;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Timers;

namespace NVMP.BuiltinPlugins
{


    internal class ServerReporter : IServerReporter
    {
#if DEBUG
        private static readonly int BroadcastInterval = 2000;
        private static readonly string BroadcastServer = "http://localhost:8030/";
#else
        private static readonly int   BroadcastInterval = 60 * 1000;
        private static readonly string BroadcastServer = "https://nv-mp.com/";
#endif
        private IModDownloadService ModService;

        private Timer BroadcastTimer;
        private bool IsBroadcasting;

        private string CachedSecureToken;
        private string CachedHostName;

        public uint ReservedSlots { get; set; } = 0;

        internal string NameInternal;
        internal string DescriptionInternal;

        public string Name
        {
            get => NameInternal;
            set
            {
                if (value != null)
                {
                    if (value.Length >= IServerReporter.MaxServerNameSize)
                        throw new ArgumentOutOfRangeException("value", $"Name of length {value.Length} is too large to be broadcasted!");

                    NameInternal = value;
                }
                else
                {
                    NameInternal = null;
                }
            }
        }

        public string Description
        {
            get => DescriptionInternal;
            set
            {
                if (value != null)
                {
                    if (value.Length >= IServerReporter.MaxServerDescriptionSize)
                        throw new ArgumentOutOfRangeException("value", $"Description of length {value.Length} is too large to be broadcasted!");

                    DescriptionInternal = value;
                }
                else
                {
                    DescriptionInternal = null;
                }
            }
        }

        public uint MaxSlots => (uint)NativeSettings.GetFloatValue("Network", "MaxPeers");

        public ServerReporter(IModDownloadService modService)
        {
            ModService = modService;

            var strictMods = IGameServer.UnrestrictedMode == GameServerUnrestrictedModeType.None;
            Debugging.Write($"IGameServer.UnrestrictedMode = {IGameServer.UnrestrictedMode}");
            if (!strictMods)
            {
                Debugging.Warn("Not broadcasting due to non-restricted mods enabled on command line");
            }

            NativeSettings.SetupDefaultBool("Reporting", "BroadcastToPublic", false);
            NativeSettings.SetupDefaultString("Reporting", "ServerSecureToken", GenerateRandomToken());

            IsBroadcasting = false;
            CachedSecureToken = NativeSettings.GetStringValue("Reporting", "ServerSecureToken");
            CachedHostName = NativeSettings.GetStringValue("Server", "Hostname");

            if (CachedSecureToken == null || CachedSecureToken.Length == 0)
                throw new Exception("Server reporter requires ServerSecureToken to be set to a unique identifier. Remove the current entry in server.cfg to generate a fresh token. ");

            if (strictMods && NativeSettings.GetBoolValue("Reporting", "BroadcastToPublic"))
            {
                BroadcastTimer = new Timer();
                BroadcastTimer.Elapsed += new ElapsedEventHandler(Broadcast);
                BroadcastTimer.Interval = BroadcastInterval;
                BroadcastTimer.Enabled = true;
                BroadcastTimer.Start();

                Debugging.Write("[reporter] Ready");
                Broadcast(null, null);
            }
        }

        private string GenerateRandomToken()
        {
            var chars = "abcdefghijklmnopqrstuvwxyz1234567890?ABCDEFGHIJKLMNOPQRSTUVWXYZ.-";
            var rand = new Random();

            var result = "";
            for (int i = 0; i < 32; ++i)
            {
                result += chars[rand.Next(0, chars.Length - 1)];
            }

            return result;
        }

        [DataContract]
        public class ServerModInfo
        {
            [DataMember(Name = "name")]
            public string Name { get; set; }

            [DataMember(Name = "file_path")]
            public string FilePath { get; set; }

            [DataMember(Name = "digest")]
            public string Digest { get; set; }

            [DataMember(Name = "downloadable")]
            public bool Downloadable { get; set; }

            public ServerModInfo(ModFile mod)
            {
                Digest = mod.Digest;
                Name = mod.Name;
            }
        }

        private async void Broadcast(object sender, EventArgs args)
        {
            if (IsBroadcasting)
            {
                return;
            }

            Debugging.Write("[reporter] Broadcasting server presence...");
            IsBroadcasting = true;
            try
            {
                var client = new RestClient(BroadcastServer);
                var request = new RestRequest("/serverlist", Method.POST);
                request.JsonSerializer = new NewtonsoftJsonSerializer();

                // Declare content
                var mods = ModManager.GetMods();
                var downloadableMods = ModService.GetDownloadableMods();

                // If we are not very strict, then don't bother reporting these to the server list to enforce it
                var modsReport = new List<ServerModInfo>();
                for (int i = 0; i < mods.Length; ++i)
                {
                    var unregisteredContent = new ServerModInfo(mods[i])
                    {
                        Downloadable = downloadableMods.Any(m => m.Name == mods[i].Name)
                    };
                    modsReport.Add(unregisteredContent);
                }

                var obj = new NativeGameServer
                {
                    Name = NameInternal ?? "NVMPX Default Server",
                    IP = CachedHostName,
                    Port = (ushort)NativeSettings.GetFloatValue("Network", "Port"),

                    ModsDownloadURL = ModService.GetDownloadURL(),
                    Mods = modsReport.ToArray(),

                    ReservedSlots = ReservedSlots,
                    Description = DescriptionInternal ?? "A New Vegas Multiplayer server",

                    SecureToken = CachedSecureToken,
                };

                request.AddJsonBody(obj);

                IRestResponse response = await client.ExecuteAsync(request);
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.OK:
                        {
                            break;
                        }
                    case System.Net.HttpStatusCode.Forbidden:
                        {
                            Debugging.Error("");
                            Debugging.Error("[reporter] Your server has been blocked from public server reporting!");
                            Debugging.Error("");

                            BroadcastTimer.Stop();
                            BroadcastTimer = null;
                            break;
                        }
                    case System.Net.HttpStatusCode.Unauthorized:
                        {
                            Debugging.Error("[reporter] Broadcast parameters were rejected. Continual failures will lead to being blocked!");

                            BroadcastTimer.Stop();
                            BroadcastTimer = null;
                            break;
                        }
                    default:
                        {
                            Debugging.Error($"[reporter] Unknown server error trying to broadcast server to {BroadcastServer}! Error {response.StatusCode}, message {response.Content} {response.ErrorMessage}");
                            if (response.ErrorException != null)
                            {
                                throw response.ErrorException;
                            }
                            break;
                        }
                }
            }
            catch (Exception
#if !DEBUG
             e
#endif 
            )
            {
#if !DEBUG
                Debugging.Error("[reporter] Broadcast error: " + e.Message);
                Exception inner = e.InnerException;
                while (inner != null)
                {
                    Debugging.Error("[reporter]    " + inner.Message);
                    inner = inner.InnerException;
                }
#endif
            }

            IsBroadcasting = false;
        }

        public void Dispose()
        {
            if (BroadcastTimer != null)
            {
                BroadcastTimer.Stop();
                BroadcastTimer = null;
            }
        }
    }
}
