using System;
using System.Runtime.InteropServices;

namespace NVMP.Entities
{
    internal class NetActor : NetReference, INetActor
    {
        #region Natives
        //
        // Static Helpers
        //
        [DllImport("Native", EntryPoint = "GameNetActor_IsDead")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_IsDead(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetActor_IsCharacter")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_IsCharacter(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetActor_SetGodmode")]
        private static extern void Internal_SetGodmode(IntPtr self, bool godmode);

        [DllImport("Native", EntryPoint = "GameNetActor_SetLevel")]
        private static extern void Internal_SetLevel(IntPtr self, int level);

        [DllImport("Native", EntryPoint = "GameNetActor_GetLevel")]
        private static extern int Internal_GetLevel(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetActor_SetScale")]
        private static extern void Internal_SetScale(IntPtr self, float scale);

        [DllImport("Native", EntryPoint = "GameNetActor_GetScale")]
        private static extern float Internal_GetScale(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetActor_SetIsFemale")]
        private static extern void Internal_SetIsFemale(IntPtr self, bool female);

        [DllImport("Native", EntryPoint = "GameNetActor_GetIsFemale")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_GetIsFemale(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetActor_GetGodmode")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_GetGodmode(IntPtr self);

        // 
        //
        //
        [DllImport("Native", EntryPoint = "GameNetActor_AddItem")]
        private static extern IntPtr Internal_AddItem(IntPtr self, IntPtr item, uint count, bool equipped);

        [DllImport("Native", EntryPoint = "GameNetActor_RemoveItem")]
        private static extern IntPtr Internal_RemoveItem(IntPtr self, IntPtr item, uint count);

        [DllImport("Native", EntryPoint = "GameNetActor_RemoveAllItems")]
        private static extern IntPtr Internal_RemoveAllItems(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetActor_GetNumItems")]
        private static extern uint Internal_GetNumItems(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetActor_GetItems")]
        private static extern void Internal_GetItems(IntPtr self, IntPtr[] result);

        [DllImport("Native", EntryPoint = "GameNetActor_GetActiveItem")]
        private static extern IntPtr Internal_GetActiveItem(IntPtr self, IntPtr item);

        [DllImport("Native", EntryPoint = "GameNetActor_ResetActorValues")]
        private static extern void Internal_ResetActorValues(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetActor_GetGravityMult")]
        private static extern float Internal_GetGravityMult(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetActor_SetGravityMult")]
        private static extern void Internal_SetGravityMult(IntPtr self, float gravityMult);

        [DllImport("Native", EntryPoint = "GameNetActor_GetKnockedState")]
        private static extern int Internal_GetKnockedState(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetActor_SetKnockedState")]
        private static extern void Internal_SetKnockedState(IntPtr self, int state);

        [DllImport("Native", EntryPoint = "GameNetActor_GetActorValue")]
        private static extern float Internal_GetActorValue(IntPtr self, NetActorValues actorValue);

        [DllImport("Native", EntryPoint = "GameNetActor_GetActorValueBase")]
        private static extern float Internal_GetActorValueBase(IntPtr self, NetActorValues actorValue);

        [DllImport("Native", EntryPoint = "GameNetActor_SetActorValue")]
        private static extern void Internal_SetActorValue(IntPtr self, NetActorValues actorValue, float value);

        [DllImport("Native", EntryPoint = "GameNetActor_PlayIdle")]
        private static extern void Internal_PlayIdle(IntPtr self, uint formID);

        [DllImport("Native", EntryPoint = "GameNetActor_GetPlayingIdle")]
        private static extern uint Internal_GetPlayingIdle(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetActor_ClearTargets")]
        private static extern void Internal_ClearTargets(IntPtr self);

        [DllImport("Native", EntryPoint = "GameNetActor_AddTarget")]
        private static extern void Internal_AddTarget(IntPtr self, IntPtr target);

        [DllImport("Native", EntryPoint = "GameNetActor_SetActorValueBase")]
        private static extern void Internal_SetActorValueBase(IntPtr self, NetActorValues actorValue, float baseValue);
        #endregion
        
        /// <summary>
        /// Returns the number of inventory items on this actor
        /// </summary>
        /// <returns></returns>
        public uint GetNumItems()
        {
            return Internal_GetNumItems(__UnmanagedAddress);
        }

        /// <summary>
        /// Returns all inventory items on this actor
        /// </summary>
        /// <returns></returns>
        public NetActorInventoryReference[] GetItems()
        {
            uint numItems = GetNumItems();

            // First grab all the native pointers
            var items = new IntPtr[numItems];
            Internal_GetItems(__UnmanagedAddress, items);

            if (items == null)
            {
                return null;
            }

            // Now "marshal" them to managed by just wrapping player objects and assigning the native address
            var marshalledItems = new NetActorInventoryReference[items.Length];
            for (uint i = 0; i < items.Length; ++i)
            {
                var activeItemInformation = Marshal.PtrToStructure<NetActorInventoryReference>( items[i] );
                marshalledItems[i] = activeItemInformation;
            }

            return marshalledItems;
        }

        public NetActorInventoryReference GetItemByForm(uint form)
        {
            var item = NetActorInventoryItem.GetByReference(form);
            if (item == null)
            {
                throw new Exception("Invalid form ID passed!");
            }

            return GetItemByForm(item);
        }

        public NetActorInventoryReference GetItemByForm(NetActorInventoryItem item)
        {
            IntPtr addr = Internal_GetActiveItem(__UnmanagedAddress, item.__UnmanagedAddress);
            if (addr == IntPtr.Zero)
            {
                return null;
            }

            return Marshal.PtrToStructure<NetActorInventoryReference>( addr );
        }

        /// <summary>
        /// Adds a new inventory item to this actor's inventory
        /// </summary>
        /// <param name="item">item reference to add</param>
        /// <param name="count">num to stack</param>
        /// <param name="equipped">equipped or not</param>
        public void AddItem(NetActorInventoryItem item, uint count = 1, bool equipped = false)
        {
            Internal_AddItem(__UnmanagedAddress, item.__UnmanagedAddress, count, equipped);
        }

        /// <summary>
        /// Adds a collection of items to this actor's inventory
        /// </summary>
        /// <param name="items"></param>
        /// <param name="equipped"></param>
        public void AddItems(NetActorInventoryItem[] items, bool equipped = false)
        {
            foreach (var item in items)
            {
                AddItem(item, 1, equipped);
            }
        }

        public void PlayIdle(uint formID)
        {
            Internal_PlayIdle(__UnmanagedAddress, formID);
        }

        public void StopIdle()
        {
            Internal_PlayIdle(__UnmanagedAddress, uint.MinValue);
        }

        public void AddPackage(INetActorPackage package)
        {
            // Nothing impressive, yet.
            package.Run(this);
        }

        public void ClearTargets()
        {
            Internal_ClearTargets(__UnmanagedAddress);
        }

        public void AddTarget(INetActor target)
        {
            Internal_AddTarget(__UnmanagedAddress, (target as NetActor).__UnmanagedAddress);
        }

        public void SetActorValue(NetActorValues av, float value)
        {
            Internal_SetActorValue(__UnmanagedAddress, av, value);
        }

        public void SetBaseActorValue(NetActorValues av, float value)
        {
            Internal_SetActorValueBase(__UnmanagedAddress, av, value);
        }

        public float GetActorValue(NetActorValues av)
        {
            return Internal_GetActorValue(__UnmanagedAddress, av);
        }

        public float GetBaseActorValue(NetActorValues av)
        {
            return Internal_GetActorValueBase(__UnmanagedAddress, av);
        }


        /// <summary>
        /// Removes an item from this actor's inventory with the specified item form
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        public void RemoveItem(NetActorInventoryItem item, uint count = 1)
        {
            Internal_RemoveItem(__UnmanagedAddress, item.__UnmanagedAddress, count);
        }

        public void ResetActorValues()
        {
            Internal_ResetActorValues(__UnmanagedAddress);
        }

        /// <summary>
        /// Removes all items from this actor's inventory
        /// </summary>
        public void RemoveAllItems()
        {
            Internal_RemoveAllItems(__UnmanagedAddress);
        }

        /// <summary>
        /// The sex flag of this actor
        /// </summary>
        public bool IsFemale
        {
            set
            {
                Internal_SetIsFemale(__UnmanagedAddress, value);
            }
            get
            {
                return Internal_GetIsFemale(__UnmanagedAddress);
            }
        }

        public NetActorKnockedState KnockedState
        {
            get => (NetActorKnockedState)Internal_GetKnockedState(__UnmanagedAddress);
            set => Internal_SetKnockedState(__UnmanagedAddress, (int)value);
        }

        public uint PlayingIdle
        {
            get => Internal_GetPlayingIdle(__UnmanagedAddress);
        }

        /// <summary>
        /// Is this actor a player character
        /// </summary>
        public bool IsCharacter
        {
            get
            {
                return Internal_IsCharacter(__UnmanagedAddress);
            }
        }

        /// <summary>
        /// The gravity multiplier this actor is set with
        /// </summary>
        public float GravityMult
        {
            get
            {
                return Internal_GetGravityMult(__UnmanagedAddress);
            }
            set
            {
                Internal_SetGravityMult(__UnmanagedAddress, value);
            }
        }

        /// <summary>
        /// Whether the actor will be able to take damage or not
        /// </summary>
        public bool HasGodmode
        {
            set
            {
                Internal_SetGodmode(__UnmanagedAddress, value);
            }
            get
            {
                return Internal_GetGodmode(__UnmanagedAddress);
            }
        }

        /// <summary>
        /// A readonly value of the actor's lifestate
        /// </summary>
        public bool IsDead
        {
            get
            {
                return Internal_IsDead(__UnmanagedAddress);
            }
        }

        /// <summary>
        /// The actor's game level
        /// </summary>
        public int Level
        {
            set
            {
                Internal_SetLevel(__UnmanagedAddress, value);
            }
            get
            {
                return Internal_GetLevel(__UnmanagedAddress);
            }
        }

        public float Health
        {
            set
            {
                Internal_SetActorValue(__UnmanagedAddress, NetActorValues.Health, value);
            }
            get
            {
                return Internal_GetActorValue(__UnmanagedAddress, NetActorValues.Health);
            }
        }

        public float MaxHealth
        {
            set
            {
                Internal_SetActorValueBase(__UnmanagedAddress, NetActorValues.Health, value);
            }
            get
            {
                return Internal_GetActorValueBase(__UnmanagedAddress, NetActorValues.Health);
            }
        }

        /// <summary>
        /// XP currently awarded to this actor. This is only supported on characters!
        /// </summary>
        public int XP
        {
            set
            {
                if (!IsCharacter)
                {
                    throw new Exception("Cannot reward XP to non-characters!");
                }

                if (value < XP)
                {
                    throw new Exception("Cannot unreward XP!");
                }

                Internal_SetActorValue(__UnmanagedAddress, NetActorValues.XP, (float)value);
            }
            get
            {
                return (int)Internal_GetActorValue(__UnmanagedAddress, NetActorValues.XP);
            }
        }

        public uint Caps
        {
            get
            {
                var items = GetItemByForm(INetActor.GameCurrency);
                if (items != null)
                {
                    return items.Count;
                }
                return 0;
            }
            set
            {
                var items = GetItemByForm(INetActor.GameCurrency);
                if (items == null)
                {
                    AddItem(INetActor.GameCurrency, value);
                }
                else
                {
                    if (value < items.Count)
                    {
                        RemoveItem(INetActor.GameCurrency, items.Count - value);
                    }
                    else
                    {
                        AddItem(INetActor.GameCurrency, value - items.Count);
                    }
                }
            }
        }

        /// <summary>
        /// Defines the 3D model scale of this actor. Fallout internally limits this between 0.001-10.000.
        /// </summary>
        public float Scale
        {
            set
            {
                Internal_SetScale(__UnmanagedAddress, value);
            }
            get
            {
                return Internal_GetScale(__UnmanagedAddress);
            }
        }
    }
}