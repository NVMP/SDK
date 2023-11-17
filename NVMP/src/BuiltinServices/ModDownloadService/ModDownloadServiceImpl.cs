using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NVMP.BuiltinServices
{
    internal class ModDownloadServiceImpl : IModDownloadService
    {
        public class BannedList
        {
            public List<string> Digests;
            public List<string> Names;
        }

        protected static string WebScheme = "mods";

        protected IManagedWebService WebService;
        protected IGameServer Server;
        protected BannedList BannedMods;
        protected List<DownloadableMod> Mods;

        public bool IsServingModDownloads { get; set; } = true;

        // The full reference to the download URL provided by the mod download service.
        public string DownloadURL => $"{WebService.FullURL}/{WebScheme}";

        public ModDownloadServiceImpl(IGameServer server, IManagedWebService webService)
        {
            WebService = webService;
            Server = server;
            Mods = new List<DownloadableMod>();

            BannedMods = new BannedList
            {
                Digests = new List<string>
                {
                    "91c0d7a834954057e7998921e154b4ff"
                        , // caravan pack
                    "44cb6ac23018c4394dbd6b747d6db402"
                        , // classic pack
                    "2afca0a0df2ad1024c6fcbe66711a54d"
                        , // dead money
                    "fd13cc176f14e6b6dc489e0c2500595b"
                        , // fallout base game
                    "524904da7d585c31f02db34e6dd1a010"
                        , // gun runners
                    "791d410ea87b33cf4b89a1c4efe4970e"
                        , // honest hearts
                    "c268a9e7d8bff8747c1210735e44d272"
                        , // lonesome road
                    "4dff17254e80871ec094c9fb8d51916f"
                        , // mercenary pack
                    "54b58db1a14b2a4bda83f860299f233c"
                        , // old world blues
                    "bbe875b0ecf08f337f55200c0f171cd5"
                        , // tribal pack
                },
                Names = new List<string>
                {
                    "CaravanPack",
                    "ClassicPack",
                    "DeadMoney",
                    "FalloutNV",
                    "GunRunnersArsenal",
                    "HonestHearts",
                    "LonesomeRoad",
                    "MercenaryPack",
                    "OldWorldBlues",
                    "TribalPack",
                }
            };

            // ExecutionType is set to async, as the response is purely I/O related and we don't need to serve this on the web dispatcher thread. What may happen is
            // that if too many requests are made synchronously is that the dispatcher thread becomes locked until other downloads are complete. This could create a denial of
            // service attack, or in less worse cases a timeout to a lot of players attempting to download a new file download.
            WebService.AddRootResolver(WebScheme, ProcessRequest, executionType: IManagedWebService.ExecutionType.Async);
        }

        public async Task ProcessRequest(HttpListenerRequest req, HttpListenerResponse resp)
        {
            if (!IsServingModDownloads)
            {
                byte[] data = Encoding.UTF8.GetBytes("Server is not serving mod files");
                resp.StatusCode = 404;
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.Length;
                resp.ContentType = "text/plain";
                resp.OutputStream.Write(data, 0, data.Length);
                return;
            }

            // Note well that we are in an asynchronous context, nothing is blocked on this Task, so be aware of any threading operations
            // done here may not be safe.
            Debugging.Write("Processing mod download...");
            try
            {
                string filename = req.Url.AbsolutePath.Substring(WebScheme.Length + 2);

                var serverMod = Mods.Where(mod => mod.Name == filename)
                    .FirstOrDefault();

                if (serverMod != null)
                {
                    // Always send the HEAD
                    if (!File.Exists(serverMod.FilePath))
                    {
                        byte[] data = Encoding.UTF8.GetBytes("Server resource missing");
                        resp.StatusCode = 503;
                        resp.ContentEncoding = Encoding.UTF8;
                        resp.ContentLength64 = data.Length;
                        resp.ContentType = "text/plain";
                        resp.OutputStream.Write(data, 0, data.Length);
                        return;
                    }

                    var fileInfo = new FileInfo(serverMod.FilePath);

                    resp.StatusCode = 200;
                    resp.ContentType = "application/octet-stream";
                    resp.AddHeader("Content-Disposition", $"attachment; filename={serverMod.Name}");
                    resp.SendChunked = false;

                    if (req.HttpMethod == "GET")
                    {
                        resp.ContentLength64 = fileInfo.Length;

                        // Payload the information detached
                        using (FileStream fs = File.OpenRead(serverMod.FilePath))
                        {
                            byte[] buffer = new byte[64 * 1024];
                            int read;
                            using (var bw = new BinaryWriter(resp.OutputStream))
                            {
                                while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    bw.Write(buffer, 0, read);
                                    bw.Flush(); //seems to have no effect
                                }

                                bw.Close();
                            }
                        }

                        resp.Close();
                    }
                    else
                    {
                        byte[] data = Encoding.UTF8.GetBytes("Not Found");
                        resp.StatusCode = 404;
                        resp.ContentEncoding = Encoding.UTF8;
                        resp.ContentLength64 = data.Length;
                        resp.ContentType = "text/plain";
                        resp.OutputStream.Write(data, 0, data.Length);

                        resp.Close();
                    }

                    return;
                }
                else
                {
                    byte[] data = Encoding.UTF8.GetBytes("Not Found");
                    resp.StatusCode = 404;
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.Length;
                    resp.ContentType = "text/plain";
                    resp.OutputStream.Write(data, 0, data.Length);
                }

                resp.Close();
            }
            catch (Exception e)
            {
                Debugging.Error(e.Message);
                resp.Close();
            }

            await Task.CompletedTask;
        }

        public string GetDownloadURL()
        {
            return $"{WebService.FullURL}/{WebScheme}";
        }

        public IEnumerable<DownloadableMod> DownloadableMods => Mods;

        /// <summary>
        /// Registers a custom mod to the download service. This mod must be available in the Data folder of the server.
        /// </summary>
        /// <param name="modName"></param>
        public bool AddCustomMod(string modName)
        {
            // 1. Find the mod in native and get the details
            var mod = ModManager.FindModByName(modName);
            if (mod != null)
            {
                // 2. Check that the mod isn't banned (DLCs are not permitted! that is piracy!)
                if (BannedMods.Digests.Contains(mod.Digest))
                {
                    return false;
                }

                if (BannedMods.Names.Where(_mod => mod.Name.StartsWith(_mod)).Any())
                {
                    return false;
                }

                // 3. Add it to the list of available files
                var dmod = new DownloadableMod { Digest = mod.Digest, FilePath = mod.FilePath, Name = mod.Name };
                Mods.Add(dmod);
                Debugging.Write($"{dmod.Name} registered to mod service");

                // 4. Find any BSAs associated on disk. We do this here instead of the native server since the server does not give a shit about
                // the status of MD5's.
                string parentDirectory = Path.GetDirectoryName( mod.FilePath );
                string modNameWithExt = Path.GetFileNameWithoutExtension( mod.FilePath );

                if (parentDirectory != null && modNameWithExt != null)
                {
                    var directoryInfo = new DirectoryInfo(parentDirectory);
                    var bsaFiles = directoryInfo.GetFiles($"{modNameWithExt}*.bsa");
                    
                    foreach (var bsaFilePathString in bsaFiles)
                    {
                        using (var file = File.OpenRead(bsaFilePathString.FullName))
                        {
                            using (var digester = MD5.Create())
                            {
                                byte[] hashBytes = digester.ComputeHash(file);
                                StringBuilder sb = new StringBuilder();

                                for (int i = 0; i < hashBytes.Length; i++)
                                {
                                    sb.Append(hashBytes[i].ToString("x2"));
                                }

                                // with the digest, now register it so we can offer downloads
                                var digest = sb.ToString();

                                dmod = new DownloadableMod { Digest = digest, FilePath = bsaFilePathString.FullName, Name = bsaFilePathString.Name };
                                Mods.Add(dmod);
                                Debugging.Write($"{dmod.Name} registered to mod service");
                            }
                        }
                    }
                }

                return true;
            }

            return false;
        }

        public void Shutdown()
        {
            Mods.Clear();
        }
    }
}
