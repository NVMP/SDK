using McMaster.NETCore.Plugins;
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

        static private bool IsGCHandleValid(IntPtr handle)
        {
            try
            {
                return GCHandle.FromIntPtr(handle).IsAllocated;
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

            rootDescription.UpdateDelegate = delta =>
            {
                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        try
                        {
                            plugin.Update(delta);
                        }
                        catch (Exception e)
                        {
                            Debugging.Error($"{plugin.GetName()}: {e}");
                        }
                    }
                }
            };
            
            rootDescription.PlayerInputUpdate = (player, inputType, key) =>
            {
                if (player.Actor == null)
                    return;

                var tasks = new List<Task>();
                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                await plugin.PlayerInputUpdate(player, (UserInterface.InputType)inputType, key);
                            }
                            catch (Exception e)
                            {
                                Debugging.Error($"{plugin.GetName()}: {e}");
                            }
                        }));
                    }
                }

                Task.WaitAll(tasks.ToArray());
            };

            rootDescription.PlayerMouseUpdate = (player, mouseX, mouseY, mousewheelZ) =>
            {
                if (player.Actor == null)
                    return;

                var tasks = new List<Task>();
                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                await plugin.PlayerMouseUpdate(player, mouseX, mouseY, mousewheelZ);
                            }
                            catch (Exception e)
                            {
                                Debugging.Error($"{plugin.GetName()}: {e}");
                            }
                        }));
                    }
                }

                Task.WaitAll(tasks.ToArray());
            };

            rootDescription.PlayerFinishLoad = player =>
            {
                if (player.Actor == null)
                    return;

                var tasks = new List<Task>();
                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                await plugin.PlayerFinishLoad(player);
                            }
                            catch (Exception e)
                            {
                                Debugging.Error($"{plugin.GetName()}: {e}");
                            }
                        }));
                    }
                }

                Task.WaitAll(tasks.ToArray());
            };

            rootDescription.PlayerNewSave = player =>
            {
                if (player.Actor == null)
                    return;

                var tasks = new List<Task>();
                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                await plugin.PlayerNewSave(player);
                            }
                            catch (Exception e)
                            {
                                Debugging.Error($"{plugin.GetName()}: {e}");
                            }
                        }));
                    }
                }

                Task.WaitAll(tasks.ToArray());
            };

            rootDescription.PlayerUpdatedSave = (player, name, digest) =>
            {
                if (player.Actor == null)
                    return;

                var tasks = new List<Task>();
                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                await plugin.PlayerUpdatedSave(player, name, digest);
                            }
                            catch (Exception e)
                            {
                                Debugging.Error($"{plugin.GetName()}: {e}");
                            }
                        }));
                    }
                }

                Task.WaitAll(tasks.ToArray());
            };

            rootDescription.PlayerCheated = player =>
            {
                var tasks = new List<Task>();
                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                await plugin.PlayerCheated(player);
                            }
                            catch (Exception e)
                            {
                                Debugging.Error($"{plugin.GetName()}: {e}");
                            }
                        }));
                    }
                }

                Task.WaitAll(tasks.ToArray());
            };

            rootDescription.PlayerJoined = player =>
            {
                if (player.Actor == null)
                    return;

                var tasks = new List<Task>();
                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                await plugin.PlayerJoined(player);
                            }
                            catch (Exception e)
                            {
                                Debugging.Error($"{plugin.GetName()}: {e}");
                            }
                        }));
                    }
                }

                Task.WaitAll(tasks.ToArray());
            };

            rootDescription.PlayerLeft = player =>
            {
                if (player.Actor == null)
                    return;

                var tasks = new List<Task>();
                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                await plugin.PlayerLeft(player);
                            }
                            catch (Exception e)
                            {
                                Debugging.Error($"{plugin.GetName()}: {e}");
                            }
                        }));
                    }
                }

                Task.WaitAll(tasks.ToArray());
            };

            rootDescription.PlayerAuthenticating = (player, token) =>
            {
                token = HttpUtility.UrlDecode(token);

                var tasks = new List<Task>();
                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                await plugin.PlayerAuthenticating(player, token);
                            }
                            catch (Exception e)
                            {
                                Debugging.Error($"{plugin.GetName()}: {e}");
                            }
                        }));
                    }
                }

                Task.WaitAll(tasks.ToArray());
            };

            rootDescription.PlayerRequestsRespawn = player =>
            {
                if (player.Actor == null)
                    return;

                var tasks = new List<Task>();
                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                await plugin.PlayerRequestsRespawn(player);
                            }
                            catch (Exception e)
                            {
                                Debugging.Error($"{plugin.GetName()}: {e}");
                            }
                        }));
                    }
                }

                Task.WaitAll(tasks.ToArray());
            };

            rootDescription.ActorDied = (actor, killer) =>
            {
                var tasks = new List<Task>();
                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                await plugin.ActorDied(actor, killer);
                            }
                            catch (Exception e)
                            {
                                Debugging.Error($"{plugin.GetName()}: {e}");
                            }
                        }));
                    }
                }

                Task.WaitAll(tasks.ToArray());
            };

            rootDescription.PlayerMessaged = (player, message) =>
            {
                if (player.Actor == null)
                    return;

                var tasks = new List<Task>();
                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                await plugin.PlayerMessaged(player, message);
                            }
                            catch (Exception e)
                            {
                                Debugging.Error($"{plugin.GetName()}: {e}");
                            }
                        }));
                    }
                }

                Task.WaitAll(tasks.ToArray());
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

            rootDescription.CanResendChatTo = (NetPlayer player, NetPlayer target,
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

            rootDescription.CanResendVoiceTo = (NetPlayer player, NetPlayer target, ref float volume) =>
            {
                if (player.Actor == null)
                    return false;

                if (target.Actor == null)
                    return false;

                bool canResend = true;
                foreach (var instance in pluginInstances.Values)
                {
                    foreach (var plugin in instance)
                    {
                        try
                        {
                            canResend &= plugin.CanResendVoiceTo(player, target, ref volume);
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
                        sharedTypes: new[] { typeof(IPlugin) },
                        config => config.EnableHotReload = true);

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
