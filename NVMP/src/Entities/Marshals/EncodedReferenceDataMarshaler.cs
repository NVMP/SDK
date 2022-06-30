using NVMP.Entities.Encoding;
using System;
using System.Runtime.InteropServices;

namespace NVMP.Marshals
{
    public class EncodedReferenceDataMarshaler : ICustomMarshaler
    {
        [DllImport("Native", EntryPoint = "GameNetReference_ReleaseEncodeData")]
        private static extern void Internal_ReleaseEncodeData(IntPtr encodedDataEntry);

        private static EncodedReferenceDataMarshaler PrivateInstance;

        public static ICustomMarshaler GetInstance(string cookie)
        {
            if (PrivateInstance == null)
            {
                PrivateInstance = new EncodedReferenceDataMarshaler();
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
            return ((ReferenceData)ManagedObj).__UnmanagedAllocatedEncoding;
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            if (pNativeData == IntPtr.Zero)
            {
                return null;
            }

            var result =  new ReferenceData
            {
                __UnmanagedAllocatedEncoding = pNativeData
            };

            return result;
        }
    }
}
