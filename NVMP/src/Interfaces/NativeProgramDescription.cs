using NVMP.Delegates;
using NVMP.Entities;
using NVMP.Marshals;
using System;
using System.Runtime.InteropServices;

namespace NVMP
{
    /// <summary>
    /// Describes operations within an assemblies program. When this is passed, if you populate this structure
    /// the implementing program needs to maintain the managed data.
    /// If the managed object is lost, your program description functions will no longer be called.
    /// </summary>
    public class INativeProgramDescription
    {
        #region Natives
        [DllImport("Native", EntryPoint = "NativeProgramDescription_SetUpdate")]
        private static extern void Internal_SetUpdate(IntPtr self, UpdateDelegate fn);

        [DllImport("Native", EntryPoint = "NativeProgramDescription_SetPlayerJoined")]
        private static extern void Internal_SetPlayerJoined(IntPtr self, PlayerJoinedDelegate fn);

        [DllImport("Native", EntryPoint = "NativeProgramDescription_SetPlayerCheated")]
        private static extern void Internal_SetPlayerCheated(IntPtr self, PlayerCheatedDelegate fn);

        [DllImport("Native", EntryPoint = "NativeProgramDescription_SetPlayerLeft")]
        private static extern void Internal_SetPlayerLeft(IntPtr self, PlayerLeftDelegate fn);

        [DllImport("Native", EntryPoint = "NativeProgramDescription_SetPlayerAuthenticating")]
        private static extern void Internal_SetPlayerAuthenticating(IntPtr self, PlayerRequestsPreJoinDelegate fn);

        [DllImport("Native", EntryPoint = "NativeProgramDescription_SetRequestsRespawn")]
        private static extern void Internal_SetRequestsRespawn(IntPtr self, PlayerRequestsRespawnDelegate fn);

        [DllImport("Native", EntryPoint = "NativeProgramDescription_SetActorDied")]
        private static extern void Internal_SetActorDied(IntPtr self, ActorDiedDelegate fn);

        [DllImport("Native", EntryPoint = "NativeProgramDescription_SetPlayerExecutedCommand")]
        private static extern void Internal_SetPlayerExecutedCommand(IntPtr self, PlayerCommandDelegate fn);

        [DllImport("Native", EntryPoint = "NativeProgramDescription_SetPlayerMessaged")]
        private static extern void Internal_SetPlayerMessaged(IntPtr self, PlayerMessageDelegate fn);

        [DllImport("Native", EntryPoint = "NativeProgramDescription_SetCanResendChatTo")]
        private static extern void Internal_SetCanResendChatTo(IntPtr self, CanResendChatToDelegate fn);

        [DllImport("Native", EntryPoint = "NativeProgramDescription_SetCanCharacterChangeName")]
        private static extern void Internal_SetCanCharacterChangeName(IntPtr self, CanCharacterChangeNameDelegate fn);

        [DllImport("Native", EntryPoint = "NativeProgramDescription_SetCanResendVoiceTo")]
        private static extern void Internal_SetCanResendVoiceTo(IntPtr self, CanResendVoiceToDelegate fn);

        [DllImport("Native", EntryPoint = "NativeProgramDescription_SetPlayerUpdatedSave")]
        private static extern void Internal_SetPlayerUpdatedSave(IntPtr self, PlayerUpdatedSaveDelegate fn);

        [DllImport("Native", EntryPoint = "NativeProgramDescription_SetPlayerNewSave")]
        private static extern void Internal_SetPlayerNewSave(IntPtr self, PlayerSaveEventDelegate fn);

        [DllImport("Native", EntryPoint = "NativeProgramDescription_SetPlayerFinishLoad")]
        private static extern void Internal_SetPlayerFinishLoad(IntPtr self, PlayerSaveEventDelegate fn);

        [DllImport("Native", EntryPoint = "NativeProgramDescription_SetPlayerInputUpdate")]
        private static extern void Internal_SetPlayerInputUpdate(IntPtr self, InputUpdateDelegate fn);

        [DllImport("Native", EntryPoint = "NativeProgramDescription_SetPlayerMouseUpdate")]
        private static extern void Internal_SetPlayerMouseUpdate(IntPtr self, MouseUpdateDelegate fn);

        [DllImport("Native", EntryPoint = "NativeProgramDescription_Create")]
        private static extern IntPtr Internal_Create(string name);
        #endregion

        // The address of the unmanaged data this interface marshals against
        public IntPtr    __UnmanagedAddress;

        public static INativeProgramDescription Create(string name)
        {
            var result = (INativeProgramDescription)Marshals.NativeProgramDescriptionMarshaler.GetInstance(null)
                .MarshalNativeToManaged(Internal_Create(name));
            GCHandle.Alloc(result); // dirty hack to pin
            return result;
        }

        /// <summary>
        /// The main thread update loop of this program. This thread runs in synchronisation with the server network stack, so it is safe to operate on NetPlayers
        /// and all network engine objects here as we know we have mutually exclusive access. 
        /// </summary>
        public UpdateDelegate UpdateDelegate
        {
            set
            {
                UpdateDelegateDelegate = value;
                Internal_SetUpdate(__UnmanagedAddress, UpdateDelegateDelegate);
            }
        }
        private UpdateDelegate UpdateDelegateDelegate;

        public PlayerJoinedDelegate PlayerJoined
        {
            set
            {
                PlayerJoinedDelegate = value;
                Internal_SetPlayerJoined(__UnmanagedAddress, PlayerJoinedDelegate);
            }
        }
        private PlayerJoinedDelegate PlayerJoinedDelegate;

        public PlayerCheatedDelegate PlayerCheated
        {
            set
            {
                PlayerCheatedDelegate = value;
                Internal_SetPlayerCheated(__UnmanagedAddress, PlayerCheatedDelegate);
            }
        }
        private PlayerCheatedDelegate PlayerCheatedDelegate;

        public PlayerLeftDelegate PlayerLeft
        {
            set
            {
                PlayerLeftDelegate = value;
                Internal_SetPlayerLeft(__UnmanagedAddress, PlayerLeftDelegate);
            }
        }
        private PlayerLeftDelegate PlayerLeftDelegate;

        public PlayerRequestsPreJoinDelegate PlayerAuthenticating
        {
            set
            {
                PlayerAuthenticatingDelegate = value;
                Internal_SetPlayerAuthenticating(__UnmanagedAddress, PlayerAuthenticatingDelegate);
            }
        }
        private PlayerRequestsPreJoinDelegate PlayerAuthenticatingDelegate;

        public PlayerRequestsRespawnDelegate PlayerRequestsRespawn
        {
            set
            {
                PlayerRequestsRespawnDelegate = value;
                Internal_SetRequestsRespawn(__UnmanagedAddress, PlayerRequestsRespawnDelegate);
            }
        }
        private PlayerRequestsRespawnDelegate PlayerRequestsRespawnDelegate;

        public ActorDiedDelegate ActorDied
        {
            set
            {
                ActorDiedDelegate = value;
                Internal_SetActorDied(__UnmanagedAddress, ActorDiedDelegate);
            }
        }
        private ActorDiedDelegate ActorDiedDelegate;

        public PlayerCommandDelegate PlayerExecutedCommand
        {
            set
            {
                PlayerExecutedCommandDelegate = value;
                Internal_SetPlayerExecutedCommand(__UnmanagedAddress, PlayerExecutedCommandDelegate);
            }
        }
        private PlayerCommandDelegate PlayerExecutedCommandDelegate;

        public PlayerMessageDelegate PlayerMessaged
        {
            set
            {
                PlayerMessagedDelegate = value;
                Internal_SetPlayerMessaged(__UnmanagedAddress, PlayerMessagedDelegate);
            }
        }
        private PlayerMessageDelegate PlayerMessagedDelegate;

        public CanResendChatToDelegate CanResendChatTo
        {
            set
            {
                CanResendChatToDelegate = value;
                Internal_SetCanResendChatTo(__UnmanagedAddress, CanResendChatToDelegate);
            }
        }
        private CanResendChatToDelegate CanResendChatToDelegate;

        public CanCharacterChangeNameDelegate CanCharacterChangeName
        {
            set
            {
                CanCharacterChangeNameDelegate = value;
                Internal_SetCanCharacterChangeName(__UnmanagedAddress, CanCharacterChangeNameDelegate);
            }
        }
        private CanCharacterChangeNameDelegate CanCharacterChangeNameDelegate;

        public CanResendVoiceToDelegate CanResendVoiceTo
        {
            set
            {
                CanResendVoiceToDelegate = value;
                Internal_SetCanResendVoiceTo(__UnmanagedAddress, CanResendVoiceToDelegate);
            }
        }
        private CanResendVoiceToDelegate CanResendVoiceToDelegate;

        public PlayerUpdatedSaveDelegate PlayerUpdatedSave
        {
            set
            {
                PlayerUpdatedSaveDelegate = value;
                Internal_SetPlayerUpdatedSave(__UnmanagedAddress, PlayerUpdatedSaveDelegate);
            }
        }
        private PlayerUpdatedSaveDelegate PlayerUpdatedSaveDelegate;

        public PlayerSaveEventDelegate PlayerNewSave
        {
            set
            {
                PlayerNewSaveDelegate = value;
                Internal_SetPlayerNewSave(__UnmanagedAddress, PlayerNewSaveDelegate);
            }
        }
        private PlayerSaveEventDelegate PlayerNewSaveDelegate;

        public PlayerSaveEventDelegate PlayerFinishLoad
        {
            set
            {
                PlayerFinishLoadDelegate = value;
                Internal_SetPlayerFinishLoad(__UnmanagedAddress, PlayerFinishLoadDelegate);
            }
        }
        private PlayerSaveEventDelegate PlayerFinishLoadDelegate;

        public InputUpdateDelegate PlayerInputUpdate
        {
            set
            {
                PlayerInputUpdateDelegate = value;
                Internal_SetPlayerInputUpdate(__UnmanagedAddress, PlayerInputUpdateDelegate);
            }
        }
        private InputUpdateDelegate PlayerInputUpdateDelegate;

        public MouseUpdateDelegate PlayerMouseUpdate
        {
            set
            {
                PlayerMouseUpdateDelegate = value;
                Internal_SetPlayerMouseUpdate(__UnmanagedAddress, PlayerMouseUpdateDelegate);
            }
        }
        private MouseUpdateDelegate PlayerMouseUpdateDelegate;
    }
}
