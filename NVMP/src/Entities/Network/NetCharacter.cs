using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NVMP.Entities
{
    internal class NetCharacter : NetActor, INetCharacter
    {
        #region Natives
        [DllImport("Native", EntryPoint = "GameNetCharacter_GetCurrentRadioRefID")]
        private static extern uint Internal_GetCurrentRadioRefID(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetCharacter_GetCurrentInternetRadioURL")]
        private static extern string Internal_GetInternetRadioURL(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetCharacter_SetCurrentInternetRadioURL")]
        private static extern void Internal_SetInternetRadioURL(IntPtr self, string url);

        [DllImport("Native", EntryPoint = "GameNetCharacter_GetAimPos")]
        private static extern void Internal_GetAimPos(IntPtr self, ref float x, ref float y, ref float z);

        [DllImport("Native", EntryPoint = "GameNetCharacter_SetCanFastTravel")]
        private static extern void Internal_SetCanFastTravel(IntPtr self, bool authenticated);

        [DllImport("Native", EntryPoint = "GameNetCharacter_GetCanFastTravel")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_GetCanFastTravel(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetCharacter_SetSpectateMode")]
        private static extern void Internal_SetSpectateMode(IntPtr self, uint mode);

        [DllImport("Native", EntryPoint = "GameNetCharacter_GetSpectateMode")]
        private static extern uint Internal_GetSpectateMode(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetCharacter_SetSpectateEntityDistance")]
        private static extern void Internal_SetSpectateDistance(IntPtr self, float distance);

        [DllImport("Native", EntryPoint = "GameNetCharacter_GetSpectateEntityDistance")]
        private static extern float Internal_GetSpectateDistance(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetCharacter_SetSpectateSpeed")]
        private static extern void Internal_SetSpectateSpeed(IntPtr self, float speed);

        [DllImport("Native", EntryPoint = "GameNetCharacter_GetSpectateSpeed")]
        private static extern float Internal_GetSpectateSpeed(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetCharacter_SetSpectatePosition")]
        private static extern void Internal_SetSpectatePosition(IntPtr self, float x, float y, float z);

        [DllImport("Native", EntryPoint = "GameNetCharacter_GetSpectatePosition")]
        private static extern uint Internal_GetSpectatePosition(IntPtr self, ref float x, ref float y, ref float z);

        [DllImport("Native", EntryPoint = "GameNetCharacter_SetSpectateRotation")]
        private static extern void Internal_SetSpectateRotation(IntPtr self, float x, float y, float z, float w);

        [DllImport("Native", EntryPoint = "GameNetCharacter_GetSpectateRotation")]
        private static extern uint Internal_GetSpectateRotation(IntPtr self, ref float x, ref float y, ref float z, ref float w);

        [DllImport("Native", EntryPoint = "GameNetCharacter_SetSpectateEntity")]
        private static extern void Internal_SetSpectateEntity(IntPtr self, IntPtr target);

        [DllImport("Native", EntryPoint = "GameNetCharacter_GetSpectateEntity")]
        private static extern IntPtr Internal_GetSpectateEntity(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetCharacter_GetCrosshairReference")]
        private static extern void Internal_GetCrosshairReference(IntPtr self, out IntPtr netObject
            , out uint refId
            , out NetReferenceFormType formType
            , [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ExtraDataListBitsetMarshalerToBitArray))] out ReadOnlyExtraDataList extraDataList
            );

        [DllImport("Native", EntryPoint = "GameNetCharacter_GetEnabledEncounter")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_GetEnabledEncounter(IntPtr self, uint key);

        [DllImport("Native", EntryPoint = "GameNetCharacter_SetEnabledEncounter")]
        private static extern void Internal_SetEnabledEncounter(IntPtr self, uint key, bool value);

        [DllImport("Native", EntryPoint = "GameNetCharacter_GetTCL")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_GetTCL(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetCharacter_SetTCL")]
        private static extern void Internal_SetTCL(IntPtr self, bool value);

        [DllImport("Native", EntryPoint = "GameNetCharacter_GetOverShoulderPosX")]
        private static extern int Internal_GetOverShoulderPosX(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetCharacter_SetOverShoulderPosX")]
        private static extern void Internal_SetOverShoulderPosX(IntPtr self, int value);

        [DllImport("Native", EntryPoint = "GameNetCharacter_GetOverShoulderPosZ")]
        private static extern int Internal_GetOverShoulderPosZ(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetCharacter_SetOverShoulderPosZ")]
        private static extern void Internal_SetOverShoulderPosZ(IntPtr self, int value);

        [DllImport("Native", EntryPoint = "GameNetCharacter_GetDisabledControlFlags")]
        private static extern uint Internal_GetDisabledControlFlags(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetCharacter_SetDisabledControlFlags")]
        private static extern void Internal_SetDisabledControlFlags(IntPtr self, uint value);

        [DllImport("Native", EntryPoint = "GameNetCharacter_SetChaseCameraMax")]
        private static extern int Internal_GetChaseCameraMax(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetCharacter_SetChaseCameraMax")]
        private static extern void Internal_SetChaseCameraMax(IntPtr self, int value);


        [DllImport("Native", EntryPoint = "GameNetCharacter_GetKeyboardDisabledState")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_GetKeyboardDisabledState(IntPtr self, uint key);

        [DllImport("Native", EntryPoint = "GameNetCharacter_SetKeyboardDisabledState")]
        private static extern void Internal_SetKeyboardDisabledState(IntPtr self, uint key, bool value);
        #endregion

        public class EncounterConfig : INetCharacterEncounterConfig
        {
            internal NetCharacter Parent;

            public EncounterConfig(NetCharacter parent)
            {
                Parent = parent;
            }

            public void DisableAll()
            {
                foreach (NetCharacterEncounterTypes enumValue in Enum.GetValues(typeof(NetCharacterEncounterTypes)))
                {
                    this[enumValue] = false;
                }
            }

            public void EnableAll()
            {
                foreach (NetCharacterEncounterTypes enumValue in Enum.GetValues(typeof(NetCharacterEncounterTypes)))
                {
                    this[enumValue] = true;
                }
            }

            public bool this[NetCharacterEncounterTypes key]
            {
                get => Internal_GetEnabledEncounter(Parent.__UnmanagedAddress, (uint)key);
                set => Internal_SetEnabledEncounter(Parent.__UnmanagedAddress, (uint)key, value);
            }
        }

        public NetCharacterSpectateMode SpectatingMode
        {
            get
            {
                return (NetCharacterSpectateMode)Internal_GetSpectateMode(__UnmanagedAddress);
            }
            set
            {
                Internal_SetSpectateMode(__UnmanagedAddress, (uint)value);
            }
        }

        public INetCharacterEncounterConfig EnabledEncounters { get; }

        public NetCharacter()
        {
            EnabledEncounters = new EncounterConfig(this);
        }

        public Vector3 SpectatePosition
        {
            set
            {
                Internal_SetSpectatePosition(__UnmanagedAddress, value.X, value.Y, value.Z);
            }
            get
            {
                Vector3 value = Vector3.Zero;
                Internal_GetSpectatePosition(__UnmanagedAddress, ref value.X, ref value.Y, ref value.Z);
                return value;
            }
        }

        public Quaternion SpectateRotation
        {
            set
            {
                Internal_SetSpectateRotation(__UnmanagedAddress, value.X, value.Y, value.Z, value.W);
            }
            get
            {
                Quaternion value = Quaternion.Identity;
                Internal_GetSpectateRotation(__UnmanagedAddress, ref value.X, ref value.Y, ref value.Z, ref value.W);
                return value;
            }
        }

        public INetReference SpectateEntity
        {
            set
            {
                Internal_SetSpectateEntity(__UnmanagedAddress, value != null ? (value as NetReference).__UnmanagedAddress : IntPtr.Zero);
            }
            get
            {
                return Marshals.NetReferenceMarshaler.Instance.MarshalNativeToManaged(Internal_GetSpectateEntity(__UnmanagedAddress)) as INetReference;
            }
        }

        /// <summary>
        /// The reference the character is currently aiming at. This only applies to synchronised references aimed at, 
        /// unsynced entities will not appear on this synced data.
        /// </summary>
        public NetAbstractReference CrosshairReference
        {
            get
            {
                Internal_GetCrosshairReference(__UnmanagedAddress
                    , out IntPtr netRef
                    , out uint refId
                    , out NetReferenceFormType formType
                    , out ReadOnlyExtraDataList extraDataList);

                var result = new NetAbstractReference();
                if (netRef != IntPtr.Zero)
                {
                    result.NetReference = Marshals.NetReferenceMarshaler.Instance.MarshalNativeToManaged(netRef) as INetReference;
                }

                result.RefExtraDataList = extraDataList;
                result.RefFormType = formType;
                result.RefID = refId;
                return result;
            }
        }
        public Vector3 AimPos
        {
            get
            {
                Vector3 value = Vector3.Zero;
                Internal_GetSpectatePosition(__UnmanagedAddress, ref value.X, ref value.Y, ref value.Z);
                return value;
            }
        }

        /// <summary>
        /// The current radio reference the character is listening to.
        /// </summary>
        public uint CurrentRadioRefID
        {
            get => Internal_GetCurrentRadioRefID(__UnmanagedAddress);
        }

        /// <summary>
        /// Sets a radio URL the player will listen to on their client. This works for any Shoutcast radio URL.
        /// </summary> 
        public string InternetRadioURL
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Internal_SetInternetRadioURL(__UnmanagedAddress, null);
                }
                else
                {
                    if (!value.StartsWith("http://"))
                    {
                        throw new Exception("Radio URLs must be plaintext HTTP.");
                    }

                    Internal_SetInternetRadioURL(__UnmanagedAddress, value);
                }
            }
            get
            {
                string url = Internal_GetInternetRadioURL(__UnmanagedAddress);
                if (string.IsNullOrEmpty(url))
                {
                    return null;
                }

                return url;
            }
        }
        public float SpectateDistance
        {
            get => Internal_GetSpectateDistance(__UnmanagedAddress);
            set => Internal_SetSpectateDistance(__UnmanagedAddress, value);
        }

        public float SpectateSpeed
        {
            get => Internal_GetSpectateSpeed(__UnmanagedAddress);
            set => Internal_SetSpectateSpeed(__UnmanagedAddress, value);
        }

        public bool CanFastTravel
        {
            get => Internal_GetCanFastTravel(__UnmanagedAddress);
            set => Internal_SetCanFastTravel(__UnmanagedAddress, value);
        }

        public bool HasTCL
        {
            get => Internal_GetTCL(__UnmanagedAddress);
            set => Internal_SetTCL(__UnmanagedAddress, value);
        }

        public int ChaseCameraMax
        {
            get => Internal_GetChaseCameraMax(__UnmanagedAddress);
            set => Internal_SetChaseCameraMax(__UnmanagedAddress, value);
        }

        public int OverShoulderPosX
        {
            get => Internal_GetOverShoulderPosX(__UnmanagedAddress);
            set => Internal_SetOverShoulderPosX(__UnmanagedAddress, value);
        }

        public int OverShoulderPosZ
        {
            get => Internal_GetOverShoulderPosZ(__UnmanagedAddress);
            set => Internal_SetOverShoulderPosZ(__UnmanagedAddress, value);
        }
        public NetCharacterPlayerControls DisabledPlayerControls
        {
            get => (NetCharacterPlayerControls)Internal_GetDisabledControlFlags(__UnmanagedAddress);
            set => Internal_SetDisabledControlFlags(__UnmanagedAddress, (uint)value);
        }

        public void SetControlCodeDisabled(Keyboard.ControlCodes code, bool isDisabled)
        {
            Internal_SetKeyboardDisabledState(__UnmanagedAddress, (uint)code, isDisabled);
        }

        public bool GetControlCodeDisabled(Keyboard.ControlCodes code)
        {
            return Internal_GetKeyboardDisabledState(__UnmanagedAddress, (uint)code);
        }
    }
}
