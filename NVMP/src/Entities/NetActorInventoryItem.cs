using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NVMP
{
    /// <summary>
    /// An interface for querying and returning inventory items.
    /// </summary>
    public class NetActorInventoryItem
    {
        [DllImport("Native", EntryPoint = "InventoryItem_GetByReference")]
        internal static extern IntPtr Internal_GetByReference(uint refID);

        [DllImport("Native", EntryPoint = "InventoryItem_GetDisplayName")]
        internal static extern string Internal_GetDisplayName(IntPtr self);

        [DllImport("Native", EntryPoint = "InventoryItem_GetReferenceID")]
        internal static extern uint Internal_GetReferenceID(IntPtr self);

        [DllImport("Native", EntryPoint = "InventoryItem_GetGroupCount")]
        internal static extern uint Internal_GetGroupCount(string groupName);

        [DllImport("Native", EntryPoint = "InventoryItem_GetGroup")]
        internal static extern IntPtr Internal_GetGroup(string groupName, IntPtr[] items);

        /// <summary>
        /// The address of the unmanaged data this interface marshals against
        /// </summary>
        public IntPtr __UnmanagedAddress;

        /// <summary>
        /// Returns an InventoryItem definition built on the reference ID
        /// </summary>
        /// <param name="refID"></param>
        /// <returns></returns>
        public static NetActorInventoryItem GetByReference(uint refID)
        {
            IntPtr unmanagedItemPointer = Internal_GetByReference(refID);
            return (NetActorInventoryItem)Marshals.InventoryItemMarshaler.GetInstance(null)
                    .MarshalNativeToManaged(unmanagedItemPointer);
        }

        /// <summary>
        /// Returns an a list of items defined inside the group name
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static ICollection<NetActorInventoryItem> GetGroup(string groupName)
        {
            var numItems = Internal_GetGroupCount(groupName);
            var itemPtrs = new IntPtr[numItems];

            Internal_GetGroup(groupName, itemPtrs);

            var group = new NetActorInventoryItem[numItems];
            for (uint i = 0; i < numItems; ++i)
            {
                group[i] = (NetActorInventoryItem)Marshals.InventoryItemMarshaler.GetInstance(null)
                        .MarshalNativeToManaged(itemPtrs[i]);
            }

            return group;
        }

        /// <summary>
        /// The reference ID form of the item
        /// </summary>
        public uint ID
        {
            get
            {
                return Internal_GetReferenceID(__UnmanagedAddress);
            }
        }

        /// <summary>
        /// The name of the item
        /// </summary>
        public string Name
        {
            get
            {
                return Internal_GetDisplayName(__UnmanagedAddress);
            }
        }
    }
}
