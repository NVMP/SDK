﻿using Newtonsoft.Json;
using NVMP.Entities.Encoding;
using NVMP.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NVMP.Entities
{
    internal class NetReference : NetUnmanaged, INetReference
    {
        #region Natives
        [DllImport("Native", EntryPoint = "GameNetReference_GetNumZonesInside")]
        private static extern uint Internal_GetNumZonesInside(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_GetZonesInside")]
        private static extern uint Internal_GetZonesInside(IntPtr self, IntPtr[] zonesArray);

        [DllImport("Native", EntryPoint = "GameNetReference_GetTitle", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        private static extern string Internal_GetTitle(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_SetTitle", CharSet = CharSet.Unicode)]
        private static extern void Internal_SetTitle(IntPtr self, [MarshalAs(UnmanagedType.LPWStr)] string title);

        [DllImport("Native", EntryPoint = "GameNetReference_SetTitleColor")]
        private static extern void Internal_SetTitleColor(IntPtr self, byte r, byte g, byte b, byte a);

        [DllImport("Native", EntryPoint = "GameNetReference_GetTitleColor")]
        private static extern void Internal_GetTitleColor(IntPtr self, ref byte r, ref byte g, ref byte b, ref byte a);

        [DllImport("Native", EntryPoint = "GameNetReference_Encode")]
        private static extern IntPtr Internal_Encode(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_Decode")]
        private static extern void Internal_Decode(IntPtr self, IntPtr encodedDataEntry);

        [DllImport("Native", EntryPoint = "GameNetReference_GetGroundPosZ")]
        private static extern float Internal_GetGroundPosZ(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_GetMaxBlendingError")]
        private static extern float Internal_GetMaxBlendingError(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_SetMaxBlendingError")]
        private static extern void Internal_SetMaxBlendingError(IntPtr self, float error);

        [DllImport("Native", EntryPoint = "GameNetReference_GetInterpMode")]
        private static extern NetReferenceInterpolationMode Internal_GetInterpMode(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_SetInterpMode")]
        private static extern void Internal_SetInterpMode(IntPtr self, NetReferenceInterpolationMode mode);

        [DllImport("Native", EntryPoint = "GameNetReference_GetName")]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        private static extern string Internal_GetName(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_SetName", CharSet = CharSet.Unicode)]
        private static extern void Internal_SetName(IntPtr self, [MarshalAs(UnmanagedType.LPWStr)] string name);

        [DllImport("Native", EntryPoint = "GameNetReference_SetNametagSize")]
        private static extern void Internal_SetNametagSize(IntPtr self, float size);

        [DllImport("Native", EntryPoint = "GameNetReference_GetNametagSize")]
        private static extern float Internal_GetNametagSize(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_SetDetailsSize")]
        private static extern void Internal_SetDetailsSize(IntPtr self, float size);

        [DllImport("Native", EntryPoint = "GameNetReference_GetDetailsSize")]
        private static extern float Internal_GetDetailsSize(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_GetScript")]
        private static extern string Internal_GetScript(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_SetScript")]
        private static extern void Internal_SetScript(IntPtr self, string name);

        [DllImport("Native", EntryPoint = "GameNetReference_SetNameColor")]
        private static extern void Internal_SetNameColor(IntPtr self, byte r, byte g, byte b, byte a);

        [DllImport("Native", EntryPoint = "GameNetReference_SetActivatable")]
        private static extern void Internal_SetActivatable(IntPtr self, bool val);

        [DllImport("Native", EntryPoint = "GameNetReference_GetActivatable")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_GetActivatable(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_GetNameColor")]
        private static extern void Internal_GetNameColor(IntPtr self, ref byte r, ref byte g, ref byte b, ref byte a);

        [DllImport("Native", EntryPoint = "GameNetReference_SetCell")]
        private static extern void Internal_SetCell(IntPtr self, uint cellID);

        [DllImport("Native", EntryPoint = "GameNetReference_TeleportToMarker")]
        private static extern void Internal_TeleportToMarker(IntPtr self, string markerID);

        [DllImport("Native", EntryPoint = "GameNetReference_SetExterior")]
        private static extern void Internal_SetExterior(IntPtr self, uint worldspaceID, int x, int y);

        [DllImport("Native", EntryPoint = "GameNetReference_Morph")]
        private static extern void Internal_Morph(IntPtr self, uint formID);

        [DllImport("Native", EntryPoint = "GameNetReference_SetPosition")]
        private static extern void Internal_SetPosition(IntPtr self, float x, float y, float z);

        [DllImport("Native", EntryPoint = "GameNetReference_GetPosition")]
        private static extern void Internal_GetPosition(IntPtr self, ref float x, ref float y, ref float z);

        [DllImport("Native", EntryPoint = "GameNetReference_SetVelocity")]
        private static extern void Internal_SetVelocity(IntPtr self, float x, float y, float z);

        [DllImport("Native", EntryPoint = "GameNetReference_GetVelocity")]
        private static extern void Internal_GetVelocity(IntPtr self, ref float x, ref float y, ref float z);

        [DllImport("Native", EntryPoint = "GameNetReference_SetScale")]
        private static extern void Internal_SetScale(IntPtr self, float scale);

        [DllImport("Native", EntryPoint = "GameNetReference_GetScale")]
        private static extern float Internal_GetScale(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_SetRotation")]
        private static extern void Internal_SetRotation(IntPtr self, float x, float y, float z, float w);

        [DllImport("Native", EntryPoint = "GameNetReference_GetRotation")]
        private static extern void Internal_GetRotation(IntPtr self, ref float x, ref float y, ref float z, ref float w);

        [DllImport("Native", EntryPoint = "GameNetReference_SetLocalRotation")]
        private static extern void Internal_SetLocalRotation(IntPtr self, float x, float y, float z, float w);

        [DllImport("Native", EntryPoint = "GameNetReference_GetLocalRotation")]
        private static extern void Internal_GetLocalRotation(IntPtr self, ref float x, ref float y, ref float z, ref float w);

        [DllImport("Native", EntryPoint = "GameNetReference_SetDestructableHealth")]
        private static extern void Internal_SetDestructableHealth(IntPtr self, int health);

        [DllImport("Native", EntryPoint = "GameNetReference_GetDestructableHealth")]
        private static extern int Internal_GetDestructableHealth(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_GetPlayerOwner")]
        private static extern IntPtr Internal_GetPlayerOwner(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_SetPlayerOwner")]
        private static extern void Internal_SetPlayerOwner(IntPtr self, IntPtr player);

        [DllImport("Native", EntryPoint = "GameNetReference_GetWorldspace")]
        private static extern void Internal_GetWorldspace(IntPtr self, ref WorldspaceType worldspaceID, ref int X, ref int Y);

        [DllImport("Native", EntryPoint = "GameNetReference_GetInterior")]
        private static extern uint Internal_GetInterior(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_IsInPVS")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_IsInPVS(IntPtr self, IntPtr other);

        [DllImport("Native", EntryPoint = "GameNetReference_IsInsideZone")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_IsInsideZone(IntPtr self, IntPtr zone);

        [DllImport("Native", EntryPoint = "GameNetReference_GetIsInInterior")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_GetIsInInterior(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_IsActor")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_IsActor(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_GetVirtualWorldID")]
        private static extern uint Internal_GetVirtualWorldID(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_SetVirtualWorldID")]
        private static extern void Internal_SetVirtualWorldID(IntPtr self, uint val);

        [DllImport("Native", EntryPoint = "GameNetReference_IsIdle")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_IsIdle(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_GetFormID")]
        private static extern uint Internal_GetFormID(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_GetRefID")]
        private static extern uint Internal_GetRefID(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_GetSimType")]
        private static extern uint Internal_GetSimType(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_SetSimType")]
        private static extern void Internal_SetSimType(IntPtr self, uint simType);

        [DllImport("Native", EntryPoint = "GameNetReference_SaveState")]
        private static extern void Internal_SaveState(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_ReloadState")]
        private static extern void Internal_ReloadState(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_GetIsInvisible")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_GetIsInvisible(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_SetIsInvisible")]
        private static extern void Internal_SetIsInvisible(IntPtr self, bool invisible);

        [DllImport("Native", EntryPoint = "GameNetReference_GetNametagScalingDisabled")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_GetNametagScalingDisabled(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_SetNametagScalingDisabled")]
        private static extern void Internal_SetNametagScalingDisabled(IntPtr self, bool val);

        [DllImport("Native", EntryPoint = "GameNetReference_GetNametagLOSDisabled")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_GetNametagLOSDisabled(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_SetNametagLOSDisabled")]
        private static extern void Internal_SetNametagLOSDisabled(IntPtr self, bool val);

        [DllImport("Native", EntryPoint = "GameNetReference_GetFormType")]
        private static extern NetReferenceFormType Internal_GetFormType(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_SetPVSCheckDelegate")]
        private static extern void Internal_SetPVSCheckDelegate(IntPtr self, OnPVSCheck del);

        [DllImport("Native", EntryPoint = "GameNetReference_GetPVSCheckDelegate")]
        private static extern OnPVSCheck Internal_GetPVSCheckDelegate(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_BindDelegate")]
        protected static extern void Internal_BindDelegate(IntPtr self, string name, Delegate del);

        [DllImport("Native", EntryPoint = "GameNetReference_SetOnActivatedOtherReferenceDelegate")]
        private static extern void Internal_SetOnActivatedOtherReferenceDelegate(IntPtr self, OnActivatedReference del);

        [DllImport("Native", EntryPoint = "GameNetReference_SetOnActivatedDelegate")]
        private static extern void Internal_SetOnActivatedDelegate(IntPtr self, OnActivatedReference del);

        [DllImport("Native", EntryPoint = "GameNetReference_SetOnDamagedDelegate")]
        private static extern void Internal_SetOnDamagedDelegate(IntPtr self, OnDamaged del);

        [DllImport("Native", EntryPoint = "GameNetReference_IsAttached")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_IsAttached(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_GetParentAttachment")]
        private static extern IntPtr Internal_GetParentAttachment(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_AttachTo")]
        private static extern void Internal_AttachTo(IntPtr self, IntPtr refB, float lx, float ly, float lz, string nodeName);

        [DllImport("Native", EntryPoint = "GameNetReference_GetAttachmentLocalPosition")]
        private static extern void Internal_GetAttachmentLocalPosition(IntPtr self, ref float lx, ref float ly, ref float lz);

        [DllImport("Native", EntryPoint = "GameNetReference_SetAttachmentLocalPosition")]
        private static extern void Internal_SetAttachmentLocalPosition(IntPtr self, float lx, float ly, float lz);

        [DllImport("Native", EntryPoint = "GameNetReference_GetAttachmentLocalRotation")]
        private static extern void Internal_GetAttachmentLocalRotation(IntPtr self, ref float lx, ref float ly, ref float lz, ref float lw);

        [DllImport("Native", EntryPoint = "GameNetReference_SetAttachmentLocalRotation")]
        private static extern void Internal_SetAttachmentLocalRotation(IntPtr self, float lx, float ly, float lz, float lw);

        [DllImport("Native", EntryPoint = "GameNetReference_GetAttachmentNodeName")]
        private static extern string Internal_GetAttachmentNodeName(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_GetPlayingSound")]
        private static extern string Internal_GetPlayingSound(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_PlaySound")]
        private static extern string Internal_PlaySound(IntPtr self, string fileName, bool is3D, bool isLooping, bool hasRandomFrequencyShift, bool isOneShot);

        [DllImport("Native", EntryPoint = "GameNetReference_StopSound")]
        private static extern string Internal_StopSound(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_SetAttachmentNodeName")]
        private static extern void Internal_SetAttachmentNodeName(IntPtr self, string nodeName);

        [DllImport("Native", EntryPoint = "GameNetReference_GetAttachmentFlags")]
        private static extern uint Internal_GetAttachmentFlags(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetReference_SetAttachmentFlags")]
        private static extern void Internal_SetAttachmentFlags(IntPtr self, uint flags);

        #endregion

        internal class OnActivatedSubscription : SubscriptionDelegate<OnActivatedReference>
        {
            public OnActivatedReference Execute;

            public OnActivatedSubscription()
            {
                Execute = InternalExecute;
            }

            internal bool InternalExecute(INetReference activator
                , INetReference reference
                , uint refId
                , uint baseId
                , NetReferenceFormType formType
                , ReadOnlyExtraDataList extraDataList)
            {
                foreach (var sub in Subscriptions)
                {
                    if (!sub.Invoke(activator, reference, refId, baseId, formType, extraDataList))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        internal class OnDamagedSubscription : SubscriptionDelegate<OnDamaged>
        {
            public OnDamaged Execute;

            public OnDamagedSubscription()
            {
                Execute = InternalExecute;
            }

            internal void InternalExecute(INetReference victim, INetActor attacker, float healthDamage, float armorDamage, uint weaponFormId, uint projectileFormId)
            {
                foreach (var sub in Subscriptions)
                {
                    sub.Invoke(victim, attacker, healthDamage, armorDamage, weaponFormId, projectileFormId);
                }
            }
        }

        private OnPVSCheck InternalPVSCheck;

        public OnPVSCheck PVSCheck
        {
            set
            {
                InternalPVSCheck = value;
                Internal_SetPVSCheckDelegate(__UnmanagedAddress, InternalPVSCheck);
            }
            get
            {
                return InternalPVSCheck;
            }
        }

        internal OnActivatedSubscription OnActivatedDelegate = new OnActivatedSubscription();
        internal OnDamagedSubscription OnDamagedDelegate = new OnDamagedSubscription();
        internal OnActivatedSubscription OnActivatedOtherReferenceDelegate = new OnActivatedSubscription();

        public event OnActivatedReference ActivatedOtherReference
        {
            add { OnActivatedOtherReferenceDelegate.Add(value); }
            remove { OnActivatedOtherReferenceDelegate.Remove(value); }
        }

        public event OnActivatedReference Activated
        {
            add { OnActivatedDelegate.Add(value); }
            remove { OnActivatedDelegate.Remove(value); }
        }

        public event OnDamaged Damaged
        {
            add { OnDamagedDelegate.Add(value); }
            remove { OnDamagedDelegate.Remove(value); }
        }

        protected override void PreDispose()
        {
            Internal_SetOnActivatedDelegate(__UnmanagedAddress, null);
            Internal_SetOnDamagedDelegate(__UnmanagedAddress, null);
            Internal_SetOnActivatedOtherReferenceDelegate(__UnmanagedAddress, null);
            Internal_SetPVSCheckDelegate(__UnmanagedAddress, null);
        }

        internal override void OnCreate()
        {
            Internal_SetOnActivatedDelegate(__UnmanagedAddress, OnActivatedDelegate.Execute);
            Internal_SetOnDamagedDelegate(__UnmanagedAddress, OnDamagedDelegate.Execute);
            Internal_SetOnActivatedOtherReferenceDelegate(__UnmanagedAddress, OnActivatedOtherReferenceDelegate.Execute);
        }

        public uint FormID => Internal_GetFormID(__UnmanagedAddress);

        public uint RefID => Internal_GetRefID(__UnmanagedAddress);

        /// <summary>
        /// The display name of the actor. This is additionally set as the actor's form name, 
        /// so may have direct Fallout manipulations. Maximum name of 255 characters is permitted. 
        /// Clients may not change the name of actors.
        /// </summary>
        public string Name
        {
            set => Internal_SetName(__UnmanagedAddress, value);
            get => Internal_GetName(__UnmanagedAddress);
        }

        /// <summary>
        /// Sets the script on the specified reference. This script is ran everytime when the object is created and valid,
        /// and the this-reference is set to the reference additionally. This has a limit of 4KB.
        /// </summary>
        public string Script
        {
            set => Internal_SetScript(__UnmanagedAddress, value);
            get => Internal_GetScript(__UnmanagedAddress);
        }

        /// <summary>
        /// Sets whether the reference be interacted with the E key. For actors this may be either dialogue, or looting.
        /// For containers, this is looting. And for other stuff, just check the GECK.
        /// </summary>
        public bool Activatable
        {
            set => Internal_SetActivatable(__UnmanagedAddress, value);
            get => Internal_GetActivatable(__UnmanagedAddress);
        }

        /// <summary>
        /// Sets the actor's simulation type. See SimulationType for options. Default value for SDK references is SimulationType.Static
        /// </summary>
        public NetReferenceSimulationType SimType
        {
            set => Internal_SetSimType(__UnmanagedAddress, (uint)value);
            get => (NetReferenceSimulationType)Internal_GetSimType(__UnmanagedAddress);
        }

        /// <summary>
        /// If an name has been set, this changes the colour of it in RGBA (0-255) format
        /// </summary>
        public Color NameColor
        {
            set
            {
                Internal_SetNameColor(__UnmanagedAddress, value.R, value.G, value.B, value.A);
            }
            get
            {
                byte r = 0;
                byte g = 0;
                byte b = 0;
                byte a = 0;
                Internal_GetNameColor(__UnmanagedAddress, ref r, ref g, ref b, ref a);

                return Color.FromArgb(r, g, b, a);
            }
        }

        /// <summary>
        /// Sets the nametag size of the reference.
        /// </summary>
        /// <remarks>
        /// The default value for this is 19.0
        /// </remarks>
        public float NameSize
        {
            set => Internal_SetNametagSize(__UnmanagedAddress, value);
            get => Internal_GetNametagSize(__UnmanagedAddress);
        }

        /// <summary>
        /// Sets the nametag size of the reference.
        /// </summary>
        /// <remarks>
        /// The default value for this is 19.0
        /// </remarks>
        public float DetailsSize
        {
            set => Internal_SetDetailsSize(__UnmanagedAddress, value);
            get => Internal_GetDetailsSize(__UnmanagedAddress);
        }

        /// <summary>
        /// The overhead title of the actor. 
        /// Clients may not change the title of actors.
        /// </summary>
        public string Title
        {
            set => Internal_SetTitle(__UnmanagedAddress, value);
            get => Internal_GetTitle(__UnmanagedAddress);
        }

        /// <summary>
        /// If an overhead title has been set, this changes the colour of it in RGBA (0-255) format
        /// </summary>
        public Color TitleColor
        {
            set
            {
                Internal_SetTitleColor(__UnmanagedAddress, value.R, value.G, value.B, value.A);
            }
            get
            {
                byte r = 0;
                byte g = 0;
                byte b = 0;
                byte a = 0;
                Internal_GetTitleColor(__UnmanagedAddress, ref r, ref g, ref b, ref a);

                return Color.FromArgb(a, r, g, b);
            }
        }

        /// <summary>
        /// The 3D coordinates of the actor in the world
        /// </summary>
        public Vector3 Position
        {
            set
            {
                Internal_SetPosition(__UnmanagedAddress, value.X, value.Y, value.Z);
            }
            get
            {
                Vector3 value = Vector3.Zero;
                Internal_GetPosition(__UnmanagedAddress, ref value.X, ref value.Y, ref value.Z);
                return value;
            }
        }

        /// <summary>
        /// The 3D coordinates of the actor in the world
        /// </summary>
        public Vector3 Velocity
        {
            set
            {
                Internal_SetVelocity(__UnmanagedAddress, value.X, value.Y, value.Z);
            }
            get
            {
                Vector3 value = Vector3.Zero;
                Internal_GetVelocity(__UnmanagedAddress, ref value.X, ref value.Y, ref value.Z);
                return value;
            }
        }

        /// <summary>
        /// The 3D Quaternion rotation of the actor in the world
        /// </summary>
        public Quaternion Rotation
        {
            get
            {
                Quaternion value = Quaternion.Identity;
                Internal_GetRotation(__UnmanagedAddress, ref value.X, ref value.Y, ref value.Z, ref value.W);
                return value;
            }
            set
            {
                Internal_SetRotation(__UnmanagedAddress, value.X, value.Y, value.Z, value.W);
            }
        }

        public Quaternion LocalRotation
        {
            get
            {
                Quaternion value = Quaternion.Identity;
                Internal_GetLocalRotation(__UnmanagedAddress, ref value.X, ref value.Y, ref value.Z, ref value.W);
                return value;
            }
            set
            {
                Internal_SetLocalRotation(__UnmanagedAddress, value.X, value.Y, value.Z, value.W);
            }
        }

        public int DestructableHealth
        {
            get
            {
                if (this is INetActor)
                    throw new Exception("DestructableHealth cannot be accessed or set on Actors!");

                return Internal_GetDestructableHealth(__UnmanagedAddress);
            }
            set
            {
                if (this is INetActor)
                    throw new Exception("DestructableHealth cannot be accessed or set on Actors!");

                if (value < 0.0f || value > 100.0f)
                    throw new Exception("DestructableHealth is ranged between 0 and 100, and an invalid value was passed!");

                Internal_SetDestructableHealth(__UnmanagedAddress, value);
            }
        }

        private Quaternion ConjugatedRotation()
        {
            Quaternion c = Rotation;
            c.X = -c.X;
            c.Y = -c.Y;
            c.Z = -c.Z;
            return c;
        }

        /// <summary>
        /// Currently only positions the yaw axis
        /// </summary>
        /// <param name="target"></param>
        public void LookAt(INetReference target)
        {
            LookAt(target.Position);
        }

        /// <summary>
        /// Currently only positions the yaw axis
        /// </summary>
        /// <param name="target"></param>
        public void LookAt(Vector3 target)
        {
            float deltaX = target.X - Position.X;
            float deltaY = target.Y - Position.Y;
            float yaw = (float)Math.Atan2(deltaX, deltaY);
            Rotation = Quaternion.CreateFromYawPitchRoll(0.0f, yaw, 0.0f);

        }

        /// <summary>
        /// The player who currently owns this actor for synchronisation
        /// </summary>
        public INetPlayer PlayerOwner
        {
            get
            {
                var player = Internal_GetPlayerOwner(__UnmanagedAddress);
                return Marshals.NetPlayerMarshaler.GetInstance(null).MarshalNativeToManaged(player) as INetPlayer;
            }
            set
            {
                if (this is INetCharacter)
                {
                    throw new Exception("Cannot change ownership of INetCharacter");
                }

                Internal_SetPlayerOwner(__UnmanagedAddress, value != null ? (value as NetPlayer).__UnmanagedAddress : IntPtr.Zero);
            }
        }

        public bool IsInvisible
        {
            get => Internal_GetIsInvisible(__UnmanagedAddress);
            set => Internal_SetIsInvisible(__UnmanagedAddress, value);
        }

        public bool NametagLOSDisabled
        {
            get => Internal_GetNametagLOSDisabled(__UnmanagedAddress);
            set => Internal_SetNametagLOSDisabled(__UnmanagedAddress, value);
        }

        public bool NametagScalingDisabled
        {
            get => Internal_GetNametagScalingDisabled(__UnmanagedAddress);
            set => Internal_SetNametagScalingDisabled(__UnmanagedAddress, value);
        }

        public NetReferenceFormType FormType
        {
            get => Internal_GetFormType(__UnmanagedAddress);
        }

        public WorldspaceCoordinate Worldspace
        {
            get
            {
                var formID = NVMP.WorldspaceType.None;
                int x = 0;
                int y = 0;
                Internal_GetWorldspace(__UnmanagedAddress, ref formID, ref x, ref y);

                return new WorldspaceCoordinate { FormID = formID, X = x, Y = y };
            }
        }

        public uint Interior => Internal_GetInterior(__UnmanagedAddress);

        public bool IsInInterior => Internal_GetIsInInterior(__UnmanagedAddress);

        /// <summary>
        /// Teleports the actor to a specific cell ID. Can be used in pair with Position, Rotation
        /// Do note that the formID must be an interior cell, worldspace cells will come up short if the player has not loaded that region - in which
        /// case you should be using TeleportToExterior
        /// </summary>
        /// <param name="formid"></param>
        public void SetCell(uint formid)
        {
            Internal_SetCell(__UnmanagedAddress, formid);
        }

        public void TeleportToMarker(string markerID)
        {
            if (PlayerOwner == null)
                throw new Exception("Warping a reference to a map marker requires a player to simulate the reference teleporation. ");

            Internal_TeleportToMarker(__UnmanagedAddress, markerID);
        }

        public void TeleportTo(INetReference other)
        {
            if (other.IsInInterior)
            {
                SetCell(other.Interior);
            }
            else
            {
                int cellX = (int)(other.Position.X / 4096.0f);
                int cellY = (int)(other.Position.Y / 4096.0f);
                SetExterior(other.Worldspace.FormID, cellX, cellY);
            }

            Position = other.Position;
            VirtualWorldID = other.VirtualWorldID;
        }

        /// <summary>
        /// Teleports the actor to an exterior cell. Can be used in pair with Position, Rotation
        /// </summary>
        /// <param name="worldspace">worldspace area to spawn into</param>
        /// <param name="x">grid x coord</param>
        /// <param name="y">grid y coord</param>
        public void SetExterior(WorldspaceType worldspace, int x, int y)
        {
            if (((uint)worldspace & 0xFF000000) == 0xFF000000)
                throw new Exception("Invalid worldspace passed. You need to add the first byte of the DLC in your current load order. ie. WorldspaceType.NVDLC03BigMT.AsModIndex(1)");
            Internal_SetExterior(__UnmanagedAddress, (uint)worldspace, x, y);
        }

        /// <summary>
        /// Morphs the reference to a new formID. Morphing changes the base form of the reference to a new reference,
        /// and re-applies all previously set state on client's machines. 
        /// </summary>
        /// <param name="formID"></param>
        public void Morph(uint formID)
        {
            Internal_Morph(__UnmanagedAddress, formID);
        }

        /// <summary>
        /// Teleports the actor to an exterior cell. If position is set, the CELL X and Y component on teleport 
        /// is automatically computed to the desired position to move to. The referenec is also moved into the
        /// specified position.
        /// 
        /// This is important to use if you are moving a player, as for LOD reasons the X and Y component must be
        /// set up appropriately on warp.
        /// </summary>
        /// <param name="worldspace"></param>
        /// <param name="position"></param>
        public void SetExterior(WorldspaceType worldspace, Vector3 position)
        {
            int cellX = (int)(position.X / 4096.0f);
            int cellY = (int)(position.Y / 4096.0f);
            SetExterior(worldspace, cellX, cellY);
            Position = position;
        }

        public ReferenceData Encode()
        {
            IntPtr refData = Internal_Encode(__UnmanagedAddress);
            return (ReferenceData)Marshals.EncodedReferenceDataMarshaler.GetInstance(null)
                .MarshalNativeToManaged(refData);
        }

        public string EncodeToJSON()
        {
            var data = Encode();
            var settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                Formatting = Formatting.Indented
            };

            return JsonConvert.SerializeObject(data, settings);
        }

        public void Decode(ReferenceData entry)
        {
            Internal_Decode(__UnmanagedAddress, entry.__UnmanagedAllocatedEncoding);
        }

        public void SaveState()
        {
            Internal_SaveState(__UnmanagedAddress);
        }

        public void ReloadState()
        {
            Internal_ReloadState(__UnmanagedAddress);
        }

        public void DecodeFromJSON(string json)
        {
            using (ReferenceData data = JsonConvert.DeserializeObject<ReferenceData>(json))
            {
                if (data != null)
                {
                    Decode(data);
                }
            }
        }

        public float Scale
        {
            set => Internal_SetScale(__UnmanagedAddress, value);
            get => Internal_GetScale(__UnmanagedAddress);
        }

        public bool IsActor => Internal_IsActor(__UnmanagedAddress);

        public bool IsIdle => Internal_IsIdle(__UnmanagedAddress);

        public uint VirtualWorldID
        {
            get => Internal_GetVirtualWorldID(__UnmanagedAddress);
            set => Internal_SetVirtualWorldID(__UnmanagedAddress, value);
        }

        public string ModFile
        {
            get
            {
                if (FormID == 0)
                    throw new Exception("Tried accessing ModFile for a reference with an invalid FormID");

                var mods = ModManager.GetMods();
                foreach (var mod in mods)
                {
                    if ((FormID & 0xFF000000) == mod.Index)
                    {
                        return mod.Name;
                    }
                }

                return null;
            }
        }

        public INetReference ParentAttachment
        {
            get
            {
                return Marshals.NetReferenceMarshaler.GetInstance(null).MarshalNativeToManaged(Internal_GetParentAttachment(__UnmanagedAddress)) as INetReference;
            }
            set
            {
                if (value == this)
                    throw new Exception("Attachments cannot be made to parent themselves");

                if (value == null)
                {
                    Internal_AttachTo(__UnmanagedAddress, IntPtr.Zero, 0.0f, 0.0f, 0.0f, null);

                    if (!(this is INetCharacter))
                    {
                        PlayerOwner = null;
                    }
                }
                else
                {
                    Internal_AttachTo(__UnmanagedAddress, (value as NetReference).__UnmanagedAddress, ParentAttachmentOffset.X, ParentAttachmentOffset.Y, ParentAttachmentOffset.Z, ParentAttachmentNodeName);

                    // Change ownership to the new owner if this is not a character reference, so that the owner can synchronise it
                    // alongside the parent
                    if (!(this is INetCharacter))
                    {
                        PlayerOwner = value.PlayerOwner;
                    }
                }
            }
        }

        public Vector3 ParentAttachmentOffset
        {
            get
            {
                float x = 0;
                float y = 0;
                float z = 0;

                Internal_GetAttachmentLocalPosition(__UnmanagedAddress, ref x, ref y, ref z);
                return new Vector3(x, y, z);
            }
            set
            {
                Internal_SetAttachmentLocalPosition(__UnmanagedAddress, value.X, value.Y, value.Z);
            }
        }

        public Quaternion ParentAttachmentRotation
        {
            get
            {
                float x = 0;
                float y = 0;
                float z = 0;
                float w = 0;

                Internal_GetAttachmentLocalRotation(__UnmanagedAddress, ref x, ref y, ref z, ref w);
                return new Quaternion(x, y, z, w);
            }
            set
            {
                Internal_SetAttachmentLocalRotation(__UnmanagedAddress, value.X, value.Y, value.Z, value.W);
            }
        }

        public string ParentAttachmentNodeName
        {
            get
            {
                string nodeName = Internal_GetAttachmentNodeName(__UnmanagedAddress);
                if (!string.IsNullOrEmpty(nodeName))
                    return nodeName;

                return null;
            }
            set
            {
                if (!Internal_IsAttached(__UnmanagedAddress))
                {
                    throw new Exception("Cannot query or set the attachment node name if the attachment is not valid. ");
                }

                Internal_SetAttachmentNodeName(__UnmanagedAddress, value);
            }
        }

        public AttachmentFlags ParentAttachmentFlags
        {
            get => (AttachmentFlags)Internal_GetAttachmentFlags(__UnmanagedAddress);
            set => Internal_SetAttachmentFlags(__UnmanagedAddress, (uint)value);
        }

        public string CurrentSound => Internal_GetPlayingSound(__UnmanagedAddress);

        public bool IsInPVS(INetReference other) =>  Internal_IsInPVS(__UnmanagedAddress, (other as NetReference).__UnmanagedAddress);

        public bool IsInZone(INetZone zone) => Internal_IsInsideZone(__UnmanagedAddress, (zone as NetZone).__UnmanagedAddress);

        public override int GetHashCode()
        {
            return __UnmanagedAddress.ToInt32();
        }

        public bool Equals(INetReference other)
        {
            return (other as NetReference).__UnmanagedAddress == __UnmanagedAddress;
        }

        public void PlaySound(NetSoundRequest request)
        {
            if (string.IsNullOrEmpty( request.FileName ))
                throw new ArgumentException("Request does not specify a valid filename", "request.FileName");

            if (!request.IsOneShot)
            {
                StopSound();
            }
            Internal_PlaySound(__UnmanagedAddress, request.FileName, request.Is3D, request.IsLooping, request.HasRandomFrequencyShift, request.IsOneShot);
        }

        public void StopSound()
        {
            Internal_StopSound(__UnmanagedAddress);
        }

        public INetZone[] ZonesInside
        {
            get
            {
                // Queries the cached list of zones the reference is inside of.
                uint numZonesAllocated = Internal_GetNumZonesInside(__UnmanagedAddress);
                if (numZonesAllocated == 0)
                    return new INetZone[] { };

                var zonePointers = new IntPtr[numZonesAllocated];

                // The number of zones allocated may not match up with what's returned. It <should> do, but if there is a disparity somewhere
                // in the native environment, then it may return less to what's available as this call resolves all cached pool indexes to their
                // object pointer for the zone manifest.

                uint numZonesAvailable = Internal_GetZonesInside(__UnmanagedAddress, zonePointers);
                var marshalledZones = new INetZone[numZonesAvailable];
                for (uint i = 0; i < numZonesAvailable; ++i)
                {
                    marshalledZones[i] = Marshals.NetZoneMarshaler.Instance.MarshalNativeToManaged(zonePointers[i]) as INetZone;
                }

                return marshalledZones;
            }
        }

        public uint NumZonesInside => Internal_GetNumZonesInside(__UnmanagedAddress);

        public float GroundPosZ => Internal_GetGroundPosZ(__UnmanagedAddress);

        public NetReferenceInterpolationMode InterpMode
        {
            get => Internal_GetInterpMode(__UnmanagedAddress);
            set => Internal_SetInterpMode(__UnmanagedAddress, value);
        }

        public float MaxBlendingError
        {
            get => Internal_GetMaxBlendingError(__UnmanagedAddress);
            set => Internal_SetMaxBlendingError(__UnmanagedAddress, value);
        }
    }
}
