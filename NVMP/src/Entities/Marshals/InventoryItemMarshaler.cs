using System;
using System.Runtime.InteropServices;

namespace NVMP.Marshals
{
    public class InventoryItemMarshaler : ICustomMarshaler
    {
        private static InventoryItemMarshaler PrivateInstance;

        public static ICustomMarshaler GetInstance(string cookie)
        {
            if (PrivateInstance == null)
            {
                PrivateInstance = new InventoryItemMarshaler();
            }

            return PrivateInstance;
        }

        public void CleanUpManagedData(object ManagedObj)
        {
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            throw new NotImplementedException();
        }

        public int GetNativeDataSize()
        {
            return 0;
        }

        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            return ((NetActorInventoryItem)ManagedObj).__UnmanagedAddress;
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            if (pNativeData == IntPtr.Zero)
            {
                return null;
            }

            return new NetActorInventoryItem
            {
                __UnmanagedAddress = pNativeData
            };
        }
    }
}
