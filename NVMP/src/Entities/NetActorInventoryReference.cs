using System;
using System.Runtime.InteropServices;

namespace NVMP.Entities
{
	/// <summary>
	/// An inventory reference, that is paired to a FormID and count.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct NetActorInventoryReference
	{
		public NetActorInventoryItem Item
		{
			get
			{
				return (NetActorInventoryItem)Marshals.InventoryItemMarshaler.GetInstance(null).MarshalNativeToManaged(__ItemAddress);
			}
		}

		internal IntPtr __ItemAddress;

		[MarshalAs(UnmanagedType.U4)]
		public uint Count;

        [MarshalAs(UnmanagedType.I1)]
        public bool Equipped;
    }

}
