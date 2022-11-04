using NVMP.BuiltinServices;
using NVMP.BuiltinServices.ManagedWebService;
using NVMP.Entities;
using NVMP.UserInterface;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace NVMP
{
    /// <summary>
    /// Primary implementation of the game server
    /// </summary>
    public class GameServer : IGameServer, IPlugin
    {
        #region Natives
        [DllImport("Native", EntryPoint = "GameServer_SetIsHardcore")]
        private static extern void Internal_SetIsHardcore(bool isHardcore);

        [DllImport("Native", EntryPoint = "GameServer_GetIsHardcore")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_GetIsHardcore();

        [DllImport("Native", EntryPoint = "GameServer_SetGameDifficulty")]
        private static extern void Internal_SetGameDifficulty(GameServerDifficulty difficulty);

        [DllImport("Native", EntryPoint = "GameServer_GetGameDifficulty")]
        private static extern GameServerDifficulty Internal_GetGameDifficulty();

        [DllImport("Native", EntryPoint = "GameServer_SetReferenceDeletionTimeout")]
        private static extern void Internal_SetReferenceDeletionTimeout(long ms);


        //
        // Mods
        //
        [DllImport("Native", EntryPoint = "GameServer_FindAvailableModByName")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_FindAvailableModByName(string name, ref string filename, ref string digest, ref byte index);

        [DllImport("Native", EntryPoint = "GameServer_NumAvailableMods")]
        private static extern uint Internal_NumAvailableMods();

        [DllImport("Native", EntryPoint = "GameServer_GetAvailableMod")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_GetAvailableMod(uint index, ref string filename, ref string name, ref string digest);
        #endregion

        public IManagedWebService WebService;
        public ISyncBlockInterface SyncBlocks;

        public GameServerMod FindModByName(string mod)
        {
            string digest = null;
            string filePath = null;
            byte modIndex = 0;
            if (Internal_FindAvailableModByName(mod, ref filePath, ref digest, ref modIndex))
            {
                var sMod = new GameServerMod
                {
                    Digest = digest,
                    FilePath = filePath,
                    Name = mod,
                    Index = ((uint)modIndex << 24)
                };

                return sMod;
            }

            return null;
        }

        public GameServerMod[] GetMods()
        {
            string digest = null;
            string filePath = null;
            string name = null;

            uint numMods = Internal_NumAvailableMods();
            
            var result = new GameServerMod[numMods];
            for (uint i = 0; i < numMods; ++i)
            {
                if (Internal_GetAvailableMod(i, ref filePath, ref name, ref digest))
                {
                    result[i] = new GameServerMod
                    {
                        Digest = digest,
                        FilePath = filePath,
                        Name = name,
                        Index = (i << 24)
                    };
                }
            }

            return result;
        }

        public virtual void Init()
        {
            NativeSettings.SetupDefaultString("Server", "WebHostname", "http://localhost");
            NativeSettings.SetupDefaultString("Server", "WebPort", "8080");

            var hostname = NativeSettings.GetStringValue("Server", "WebHostname");
            if (hostname == null ||
                hostname.Length == 0)
                throw new Exception("WebHostname is not set! This should be your public WAN IP or a valid hostname to connect to the gameserver. Format is [https/http]://[hostname], do not specify port!");

            int port = (int)NativeSettings.GetFloatValue("Server", "WebPort");
            if (port == 0)
                throw new Exception("WebPort is not set! Specify a port that is guarenteed to be open!");

            WebService = ManagedWebServiceFactory.Create(hostname, port);
            WebService.Initialize();

            SyncBlocks = new SyncBlockManager();
        }

        public string GetName()
        {
            return "GameServer";
        }

        public void Update(float delta)
        {
        }

        public void Shutdown()
        {
        }

        public Task PlayerJoined(INetPlayer target)
        {
            return Task.CompletedTask;
        }

        public Task PlayerLeft(INetPlayer player)
        {
            return Task.CompletedTask;
        }

        public Task PlayerCheated(INetPlayer target)
        {
            target.Kick("Cheater");
            return Task.CompletedTask;
        }

        public Task PlayerAuthenticating(INetPlayer player, string authToken)
        {
            player.Authenticated = true;
            return Task.CompletedTask;
        }

        public Task PlayerUpdatedSave(INetPlayer player, string name, string digest)
        {
            return Task.CompletedTask;
        }

        public Task PlayerNewSave(INetPlayer player)
        {
            return Task.CompletedTask;
        }

        public Task PlayerFinishLoad(INetPlayer player)
        {
            return Task.CompletedTask;
        }

        public Task PlayerRequestsRespawn(INetPlayer player)
        {
            return Task.CompletedTask;
        }

        public Task ActorDied(INetActor actor, INetActor killer)
        {
            return Task.CompletedTask;
        }

        public Task PlayerMessaged(INetPlayer player, string message)
        {
            return Task.CompletedTask;
        }

        public bool CanCharacterChangeName(INetActor character)
        {
            return true;
        }

        public bool CanResendChatTo(INetPlayer player, INetPlayer target, string message, ref string username, ref Color usercolor)
        {
            return true;
        }

        public bool CanResendVoiceTo(INetPlayer player, INetPlayer target, ref float volume)
        {
            return true;
        }

        public bool PlayerExecutedCommand(INetPlayer player, string commandName, uint numParams, string[] paramList)
        {
            return false;
        }

        public Task PlayerInputUpdate(INetPlayer player, InputType inputType, uint key)
        {
            return Task.CompletedTask;
        }
        
        public Task PlayerMouseUpdate(INetPlayer player, int mouseX, int mouseY, int mousedeltaZ)
        {
            return Task.CompletedTask;
        }

        public bool IsHardcore
        {
            set
            {
                Internal_SetIsHardcore(value);
            }
            get
            {
                return Internal_GetIsHardcore();
            }
        }

        public GameServerDifficulty Difficulty
        {
            set
            {
                Internal_SetGameDifficulty(value);
            }
            get
            {
                return Internal_GetGameDifficulty();
            }
        }

        public long ReferenceTimeoutMS
        {
            set
            {
                Internal_SetReferenceDeletionTimeout(value);
            }
        }
    }
}
