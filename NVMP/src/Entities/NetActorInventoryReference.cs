using System;
using System.Runtime.InteropServices;

namespace NVMP.Entities
{
	/// <summary>
	/// Biped flags.
	/// </summary>
	public enum BipedFlag
    {
		LightArmor  = 0,
        Backpack    = (1 << 2),
        MediumArmor = (1 << 3),
        PowerArmor  = (1 << 5),
        NonPlayable = (1 << 6),
        HeavyArmor  = (1 << 7)
    }

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

		[MarshalAs(UnmanagedType.U1)]
		public byte BipedFlags;

		public bool IsBipedFlagSet(BipedFlag flag)
		{
			if (flag == BipedFlag.LightArmor)
			{
				return BipedFlags == 0;
			}

			return (BipedFlags & (uint)flag) == (uint)flag;
		}
    }

}
