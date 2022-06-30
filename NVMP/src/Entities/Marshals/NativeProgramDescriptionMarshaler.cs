using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NVMP.Marshals
{
    public class NativeProgramDescriptionMarshaler : ICustomMarshaler
    {
        private static NativeProgramDescriptionMarshaler PrivateInstance;

        public static ICustomMarshaler GetInstance(string cookie)
        {
            if (PrivateInstance == null)
            {
                PrivateInstance = new NativeProgramDescriptionMarshaler();
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
            return ((INativeProgramDescription)ManagedObj).__UnmanagedAddress;
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            if (pNativeData == IntPtr.Zero)
            {
                return null;
            }

            return new INativeProgramDescription
            {
                __UnmanagedAddress = pNativeData
            };
        }
    }
}
