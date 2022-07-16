using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NVMP.Entities
{
    internal class NetCharacter : NetActor, INetCharacter
    {
        #region Natives
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

        public float SpectateDistance
        {
            get
            {
                return Internal_GetSpectateDistance(__UnmanagedAddress);
            }
            set
            {
                Internal_SetSpectateDistance(__UnmanagedAddress, value);
            }
        }

        public float SpectateSpeed
        {
            get
            {
                return Internal_GetSpectateSpeed(__UnmanagedAddress);
            }
            set
            {
                Internal_SetSpectateSpeed(__UnmanagedAddress, value);
            }
        }

        public bool CanFastTravel
        {
            get
            {
                return Internal_GetCanFastTravel(__UnmanagedAddress);
            }
            set
            {
                Internal_SetCanFastTravel(__UnmanagedAddress, value);
            }
        }

    }
}
