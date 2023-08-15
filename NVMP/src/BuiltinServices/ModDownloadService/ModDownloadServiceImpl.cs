﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        protected List<ModFile> Mods;

        // The full reference to the download URL provided by the mod download service.
        public string DownloadURL => $"{WebService.FullURL}/{WebScheme}";

        public ModDownloadServiceImpl(IGameServer server, IManagedWebService webService)
        {
            WebService = webService;
            Server = server;
            Mods = new List<ModFile>();

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
                    "CaravanPack.esm",
                    "ClassicPack.esm",
                    "DeadMoney.esm",
                    "FalloutNV.esm",
                    "GunRunnersArsenal.esm",
                    "HonestHearts.esm",
                    "LonesomeRoad.esm",
                    "MercenaryPack.esm",
                    "OldWorldBlues.esm",
                    "TribalPack.esm",
                }
            };

            // ExecutionType is set to async, as the response is purely I/O related and we don't need to serve this on the web dispatcher thread. What may happen is
            // that if too many requests are made synchronously is that the dispatcher thread becomes locked until other downloads are complete. This could create a denial of
            // service attack, or in less worse cases a timeout to a lot of players attempting to download a new file download.
            WebService.AddRootResolver(WebScheme, ProcessRequest, executionType: IManagedWebService.ExecutionType.Async);
        }

        public async Task ProcessRequest(HttpListenerRequest req, HttpListenerResponse resp)
        {
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

        public IEnumerable<ModFile> DownloadableMods => Mods;

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

                if (BannedMods.Names.Contains(mod.Name))
                {
                    return false;
                }

                // 2. Add it to the list of available files
                Mods.Add(mod);
                Debugging.Write($"{modName} registered to mod service");
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
