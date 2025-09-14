using NVMP.BuiltinServices;
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

        #endregion

        public IManagedWebService WebService;
        public ISyncBlockInterface SyncBlocks;

        public bool HasWebServices { get; set; } = true;

        public virtual void Init()
        {
            if (HasWebServices)
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
            }

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

        public Task<bool> PlayerAuthenticating(INetPlayer player)
        {
            return Task.FromResult(true);
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

        public bool CanResendVoiceTo(INetPlayer player, INetPlayer target, ref VoiceFrame voiceFrame)
        {
            return true;
        }

        public bool PlayerExecutedCommand(INetPlayer player, string commandName, uint numParams, string[] paramList)
        {
            return false;
        }

        public Task PlayerInputUpdate(INetPlayer player, InputType inputType, Keyboard.ScanCodes key, Keyboard.ControlCodes code, ulong timestamp)
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
