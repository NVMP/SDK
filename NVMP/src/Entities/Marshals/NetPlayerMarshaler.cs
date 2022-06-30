using NVMP.Entities;
using System;
using System.Runtime.InteropServices;

namespace NVMP.Marshals
{
    public class NetPlayerMarshaler : ICustomMarshaler
    {
        private static NetPlayerMarshaler PrivateInstance;

        public static ICustomMarshaler GetInstance(string cookie)
        {
            if (PrivateInstance == null)
            {
                PrivateInstance = new NetPlayerMarshaler();
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
            return ((NetPlayer)ManagedObj).__UnmanagedAddress;
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            if (pNativeData == IntPtr.Zero)
            {
                return null;
            }

            return new NetPlayer
            {
                __UnmanagedAddress = pNativeData
            };
        }
    }
}
