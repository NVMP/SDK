﻿using McMaster.NETCore.Plugins;
using NVMP.Entities;
using NVMP.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;

namespace NVMP
{
    public static class Startup
    {
        static private void GCCollect()
        {
            GC.Collect();
        }

        static private bool IsGCHandleValid(IntPtr ptrhandle)
        {
            try
            {
                var handle = GCHandle.FromIntPtr(ptrhandle);
                return handle.IsAllocated && handle.Target != null;
            }
            catch (Exception)
            {
            }
            return false;
        }

        static private void FreeGCHandle(IntPtr handle)
        {
            try {
                var gcHandle = GCHandle.FromIntPtr(handle);
                if (gcHandle.Target is NetUnmanaged)
                {
                    (gcHandle.Target as NetUnmanaged).Unbind();
                }
                gcHandle.Free();
            } catch (Exception e)
            {
                Debugging.Error(e);
            }
        }

        static private void CreateManagedObjectForUnmanagedObject(IntPtr unmanaged)
        {
            var unmanagedGameType = NetReferenceFactory.GetUnmanagedNetworkType(unmanaged);
            if (unmanagedGameType != 0)
            {
                if (INetFactory.AllocationTable.TryGetValue(unmanagedGameType, out Func<IntPtr, NetUnmanaged> factoryAllocate))
                {
                    factoryAllocate(unmanaged);
                    return;
                }
            }

            throw new Exception($"Failed to create new managed data for unmanage type {unmanagedGameType} for object pointer {unmanaged}");
        }

        public static void Main(string[] args)
        {
            // This must be called from static main at the start of main execution. This makes sure that external libraries that P/Invoke the native program resolve correctly. It binds
            // the 'Native' Dll name to the native executable, so that C# can query the parent process instead of another DLL.
            NativeResolver.Initialize();
            Factory.Initialize();

            NativeResolver.BindManagedFunctions(GCCollect, FreeGCHandle, CreateManagedObjectForUnmanagedObject, IsGCHandleValid);

            // Plugins
            var pluginInstances = new Dictionary<string, List<IPlugin>>();
            var rootDescription = INativeProgramDescription.Create("NVMP.Startup");

            // Cache root - I cache this here because every VOIP packet is coming in as raw managed bytes, and we need to marshal it
            // to a target destination. These number of allocations can be in the thousands per second, so we use a single cache here considering
            // that every call into here is single-threaded, so this is safe fow now.
            //
            // This will grow to the largest packet size if needed, but for now just allocate 1024 bytes here considering we don't at the lower-level
            // allow exceeding the MTU.
            byte[] voiceFrameCache = new byte[1024];

            rootDescription.UpdateDelegate = delta =>
            {
                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        try
                        {
                            using var _ = new Optick.Event($"{plugin.GetName()}.Update");
                            plugin.Update(delta);
                        }
                        catch (Exception e)
                        {
                            Debugging.Error($"{plugin.GetName()}: {e}");
                        }
                    }
                }
            };
            
            rootDescription.PlayerInputUpdate = (player, inputType, key, timestamp) =>
            {
                if (player.Actor == null)
                    return;

                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                using var _ = new Optick.Event($"{plugin.GetName()}.PlayerInputUpdate");
                                await plugin.PlayerInputUpdate(player, (UserInterface.InputType)inputType, key, timestamp);
                            }
                            catch (Exception e)
                            {
                                Debugging.Error($"{plugin.GetName()}: {e}");
                            }
                        }).GetAwaiter().GetResult();
                    }
                }

                try
                {
                    foreach (var sub in (player as NetPlayer).InputSubscriptions.Subscriptions.ToArray())
                    {
                        using var _ = new Optick.Event($"{player.Name}.InputSubscriptions.Fire");
                        sub(player, (UserInterface.InputType)inputType, key, timestamp);
                    }
                }
                catch (Exception e)
                {
                    Debugging.Error(e);
                }
            };

            rootDescription.PlayerMouseUpdate = (player, mouseX, mouseY, mousewheelZ) =>
            {
                if (player.Actor == null)
                    return;

                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                using var _ = new Optick.Event($"{plugin.GetName()}.PlayerMouseUpdate");
                                await plugin.PlayerMouseUpdate(player, mouseX, mouseY, mousewheelZ);
                            }
                            catch (Exception e)
                            {
                                Debugging.Error($"{plugin.GetName()}: {e}");
                            }
                        }).GetAwaiter().GetResult();
                    }
                }

                try
                {
                    foreach (var sub in (player as NetPlayer).MouseUpdateSubscriptions.Subscriptions.ToArray())
                    {
                        using var _ = new Optick.Event($"{player.Name}.MouseUpdateSubscriptions.Fire");
                        sub(player, mouseX, mouseY, mousewheelZ);
                    }
                }
                catch (Exception e)
                {
                    Debugging.Error(e);
                }
            };

            rootDescription.PlayerFinishLoad = player =>
            {
                if (player.Actor == null)
                    return;

                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                using var _ = new Optick.Event($"{plugin.GetName()}.PlayerFinishLoad");
                                await plugin.PlayerFinishLoad(player);
                            }
                            catch (Exception e)
                            {
                                Debugging.Error($"{plugin.GetName()}: {e}");
                            }
                        }).GetAwaiter().GetResult();
                    }
                }
            };

            rootDescription.PlayerNewSave = player =>
            {
                if (player.Actor == null)
                    return;

                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                using var _ = new Optick.Event($"{plugin.GetName()}.PlayerNewSave");
                                await plugin.PlayerNewSave(player);
                            }
                            catch (Exception e)
                            {
                                Debugging.Error($"{plugin.GetName()}: {e}");
                            }
                        }).GetAwaiter().GetResult();
                    }
                }
            };

            rootDescription.PlayerUpdatedSave = (player, name, digest) =>
            {
                if (player.Actor == null)
                    return;

                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                using var _ = new Optick.Event($"{plugin.GetName()}.PlayerUpdatedSave");
                                await plugin.PlayerUpdatedSave(player, name, digest);
                            }
                            catch (Exception e)
                            {
                                Debugging.Error($"{plugin.GetName()}: {e}");
                            }
                        }).GetAwaiter().GetResult();
                    }
                }
            };

            rootDescription.PlayerCheated = player =>
            {
                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                using var _ = new Optick.Event($"{plugin.GetName()}.PlayerCheated");
                                await plugin.PlayerCheated(player);
                            }
                            catch (Exception e)
                            {
                                Debugging.Error($"{plugin.GetName()}: {e}");
                            }
                        }).GetAwaiter().GetResult();
                    }
                }
            };

            rootDescription.PlayerJoined = player =>
            {
                if (player.Actor == null)
                    return;

                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                using var _ = new Optick.Event($"{plugin.GetName()}.PlayerJoined");
                                await plugin.PlayerJoined(player);
                            }
                            catch (Exception e)
                            {
                                Debugging.Error($"{plugin.GetName()}: {e}");
                            }
                        }).GetAwaiter().GetResult();
                    }
                }
            };

            rootDescription.PlayerLeft = player =>
            {
                if (player.Actor == null)
                    return;

                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                using var _ = new Optick.Event($"{plugin.GetName()}.PlayerLeft");
                                await plugin.PlayerLeft(player);
                            }
                            catch (Exception e)
                            {
                                Debugging.Error($"{plugin.GetName()}: {e}");
                            }
                        }).GetAwaiter().GetResult();
                    }
                }

                foreach (var sub in Factory.Player.DisconnectionSubscriptions.Subscriptions.ToArray())
                {
                    sub(player);
                }
            };

            rootDescription.PlayerAuthenticating = (player) =>
            {
                bool shouldPermit = true;

                // Raise the authenticated event.
                try
                {
                    foreach (var sub in (player as NetPlayer).AuthenticatedSubscriptions.Subscriptions.ToArray())
                    {
                        using var _ = new Optick.Event($"{player.Name}.AuthenticatedSubscriptions.Fire");
                        sub(player);
                    }
                }
                catch (Exception e)
                {
                    player.Kick("Authentication Failed due to Raised Exception");
                    Debugging.Error(e);
                    return false;
                }

                if (player.IsKicked)
                {
                    return false;
                }

                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                shouldPermit &= await plugin.PlayerAuthenticating(player);
                            }
                            catch (Exception e)
                            {
                                Debugging.Error($"{plugin.GetName()}: {e}");
                            }
                        }).GetAwaiter().GetResult();
                    }
                }

                return shouldPermit;
            };

            rootDescription.PlayerRequestsRespawn = player =>
            {
                if (player.Actor == null)
                    return;

                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                await plugin.PlayerRequestsRespawn(player);
                            }
                            catch (Exception e)
                            {
                                Debugging.Error($"{plugin.GetName()}: {e}");
                            }
                        }).GetAwaiter().GetResult();
                    }
                }
            };

            rootDescription.ActorDied = (actor, killer) =>
            {
                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                await plugin.ActorDied(actor, killer);
                            }
                            catch (Exception e)
                            {
                                Debugging.Error($"{plugin.GetName()}: {e}");
                            }
                        }).GetAwaiter().GetResult();
                    }
                }
            };

            rootDescription.PlayerMessaged = (player, message) =>
            {
                if (player.Actor == null)
                    return;

                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                using var _ = new Optick.Event($"{plugin.GetName()}.PlayerMessaged");
                                await plugin.PlayerMessaged(player, message);
                            }
                            catch (Exception e)
                            {
                                Debugging.Error($"{plugin.GetName()}: {e}");
                            }
                        }).GetAwaiter().GetResult();
                    }
                }
            };

            rootDescription.CanCharacterChangeName = character =>
            {
                if (character == null)
                    return false;

                bool canResend = true;
                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        try
                        {
                            canResend &= plugin.CanCharacterChangeName(character);
                            if (!canResend)
                            {
                                break;
                            }
                        }
                        catch (Exception e)
                        {
                            Debugging.Error($"{plugin.GetName()}: {e}");
                        }
                    }
                }

                return canResend;
            };

            rootDescription.CanResendChatTo = (INetPlayer player, INetPlayer target,
                string     message,
                ref string username,
                ref byte uca,
                ref byte ucr,
                ref byte ucg,
                ref byte ucb) =>
            {
                if (player.Actor == null)
                    return false;

                if (target.Actor == null)
                    return false;

                bool canResend = true;

                var usercolor = System.Drawing.Color.FromArgb(uca, ucr, ucg, ucb);
                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        try
                        {
                            using var _ = new Optick.Event($"{plugin.GetName()}.CanResendChatTo");
                            canResend &= plugin.CanResendChatTo(player, target, message, ref username, ref usercolor);
                            if (!canResend)
                            {
                                break;
                            }
                        }
                        catch (Exception e)
                        {
                            Debugging.Error($"{plugin.GetName()}: {e}");
                        }
                    }
                }

                uca = usercolor.A;
                ucr = usercolor.R;
                ucg = usercolor.G;
                ucb = usercolor.B;

                return canResend;
            };

            rootDescription.CanResendVoiceTo = (INetPlayer player, INetPlayer target, ref bool is3D, ref float volume) =>
            {
                if (player.Actor == null)
                    return false;

                if (target.Actor == null)
                    return false;

                bool canResend = true;

                var voiceFrame = new VoiceFrame();
                voiceFrame.Is3D = false;
                voiceFrame.Volume = 1.0f;

                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        try
                        {
                            using var _ = new Optick.Event($"{plugin.GetName()}.CanResendVoiceTo");
                            canResend &= plugin.CanResendVoiceTo(player, target, ref voiceFrame);
                            if (!canResend)
                            {
                                break;
                            }
                        }
                        catch (Exception e)
                        {
                            Debugging.Error($"{plugin.GetName()}: {e}");
                        }
                    }
                }

                is3D = voiceFrame.Is3D;
                volume = voiceFrame.Volume;

                return canResend;
            };

            rootDescription.PlayerExecutedCommand = (player, commandName, numParams, paramList) =>
            {
                if (player.Actor == null)
                    return false;

                bool consumeCommand = false;
                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        try
                        {
                            using var _ = new Optick.Event($"{plugin.GetName()}.PlayerExecutedCommand");
                            consumeCommand |= plugin.PlayerExecutedCommand(player, commandName, numParams, paramList);
                        }
                        catch (Exception e)
                        {
                            Debugging.Error($"{plugin.GetName()}: {e}");
                        }
                    }
                }

                return consumeCommand;
            };

            // Query for plugins
            string plugins = NativeSettings.GetStringValue("SDK", "Plugins");
            string path = $"{Directory.GetCurrentDirectory()}/Plugins";
            Debugging.Write($"Plugins: {plugins}");
            
            var loaders = new Dictionary<string, PluginLoader>();

            // Create plugins
            foreach (string plugin in plugins.Split(","))
            {
                Debugging.Write($"Loading {plugin}...");

                try
                {
                    var loader = PluginLoader.CreateFromAssemblyFile(
                        assemblyFile: $"{path}/{plugin}",
                        isUnloadable: false,
                        sharedTypes: new[] { typeof(IPlugin) },
                        config => config.EnableHotReload = false);

                    loader.Reloaded += new PluginReloadedEventHandler((object sender, PluginReloadedEventArgs eventArgs) =>
                    {
                        Debugging.Write("Hot-reloadeding plugin: '" + plugin + "'...");

                        if (pluginInstances.TryGetValue(plugin, out List<IPlugin> activeInstance))
                        {
                            Debugging.Write("Shutting down existing managed data...");

                            try
                            {
                                foreach (var plugin in activeInstance)
                                {
                                    try
                                    {
                                        plugin.Shutdown();
                                    }
                                    catch (Exception e)
                                    {
                                        Debugging.Error($"Exception at {plugin.GetName()} Shutdown: {e.Message}");
                                    }
                                }

                                if (!pluginInstances.Remove(plugin))
                                {
                                    throw new Exception("Failed to remove existing managed plugin");
                                }
                            } catch (Exception e)
                            {
                                Debugging.Error(e.Message);
                            }
                        }

                        GC.Collect();

                        // Initialize new copy
                        var subplugins = new List<IPlugin>();
                        foreach (var pluginType in eventArgs.Loader
                            .LoadDefaultAssembly()
                            .GetTypes()
                            .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract))
                        {
                            // This assumes the implementation of IPlugin has a parameterless constructor
                            IPlugin pluginInstance = (IPlugin)Activator.CreateInstance(pluginType);
                            pluginInstance.Init();
                        
                            Debugging.Write($"Loaded plugin '{pluginInstance.GetName()}'");
                        
                            subplugins.Add(pluginInstance);
                        }
                        
                        pluginInstances.Add(plugin, subplugins);
                    });

                    loaders.Add(plugin, loader);
                }
                catch (Exception e)
                {
                    Debugging.Error(e);
                    if (e.InnerException != null)
                    {
                        Debugging.Error(e.InnerException);
                    }
                }
            }

            // Create plugin instances
            foreach (var pluginLoader in loaders)
            {
                var subplugins = new List<IPlugin>();
                foreach (var pluginType in pluginLoader.Value
                    .LoadDefaultAssembly()
                    .GetTypes()
                    .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract))
                {
                    // This assumes the implementation of IPlugin has a parameterless constructor
                    Debugging.Write($"Creating plugin instance for '{pluginLoader.Key}'");
                    IPlugin plugin = (IPlugin)Activator.CreateInstance(pluginType);
                    try
                    {
                        plugin.Init();

                        Debugging.Write($"Loaded plugin '{pluginLoader.Key}', '{plugin.GetName()}'");
                        subplugins.Add(plugin);
                    } catch(Exception e)
                    {
                        Debugging.Error(e);

                        // Silent catch, got to shut down any potential resources that might have ran midway into Init() but are still held on
                        try
                        {
                            plugin.Shutdown();
                        }
                        catch (Exception) {
                            // Not bothered with this
                        }
                    }

                }

                if (subplugins.Count == 0)
                {
                    Debugging.Write($"No IPlugin definitions were created");
                }
                else
                {
                    pluginInstances.Add(pluginLoader.Key, subplugins);
                }
            }

            NativeResolver.SetMainThreadRunning(true); // allow the main thread to now continue
            GC.Collect();
        }
    }
}
