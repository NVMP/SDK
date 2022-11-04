using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NVMP.Entities
{
    /// <summary>
    /// A container class for objects that share unmanaged lifecycles. These objects will only be completely disposed when
    /// the managed environment has no more active handles, and the unmanaged object destroys (the latter can only be done when all unmanaged handles
    /// are destroyed, and the server will warn if an object is held on for too long).
    /// 
    /// You should only keep copies of these objects when the SDK itself has created the object, and is not simulated as a temporary reference. This guarentees
    /// the object will only be cleaned up on the unmanaged side when the managed code cleans it up. 
    /// 
    /// For other references, you are safe to read these in when passed in through native/unmanaged calls, but ensure that if you keep a copy of 
    /// a NetUnmanaged object in memory, you check for IsDestroyed and remove your handle so the unmanaged code can continue with deletion.
    /// </summary>
    public class NetUnmanaged : IDisposable
    {
        [DllImport("Native", EntryPoint = "NetObject_SetManagedHandle")]
        private static extern void Internal_SetManagedHandle(IntPtr self, IntPtr handle);

        [DllImport("Native", EntryPoint = "NetObject_GetManagedHandle")]
        private static extern IntPtr Internal_GetManagedHandle(IntPtr self);

        [DllImport("Native", EntryPoint = "NetObject_Track")]
        private static extern uint Internal_Track(IntPtr self, string owner);

        [DllImport("Native", EntryPoint = "NetObject_Untrack")]
        private static extern void Internal_Untrack(IntPtr self, uint handle);

        [DllImport("Native", EntryPoint = "NetObject_Destroy")]
        private static extern void Internal_Destroy(IntPtr self, uint flags);

        [DllImport("Native", EntryPoint = "NetObject_IsDestroyed")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_IsDestroyed(IntPtr self);

        // The address of the unmanaged data this interface marshals against
        private IntPtr __InternalUnmanagedAddress;
        private uint __InternalUnmanagedHandle;

        private bool __Disposed;

        internal IntPtr __UnmanagedAddress
        {
            set
            {
                // Tracking ensures that unmanaged code won't accidentally release if there is still managed handles
                if (value != __InternalUnmanagedAddress)
                {
                    {
                        if (__InternalUnmanagedAddress != IntPtr.Zero && __InternalUnmanagedHandle != 0)
                        {
                            Internal_Untrack(__InternalUnmanagedAddress, __InternalUnmanagedHandle);
                            __InternalUnmanagedHandle = 0;
                            __InternalUnmanagedAddress = IntPtr.Zero;
                        }

                        if (value != IntPtr.Zero)
                        {
                            var stackTrace = new System.Diagnostics.StackTrace();
                            var name = "unknown";
                            try
                            {
                                name = stackTrace.ToString();
                            }
                            catch (Exception)
                            {
                            }

                            __InternalUnmanagedHandle = Internal_Track(value, owner: name);
                        }
                    }

                    // free any existing gc handles on the old reference if we are pairing a new object
                    if (__UnmanagedAddress != IntPtr.Zero)
                    {
                        var existingPtr = GCHandle.FromIntPtr(Internal_GetManagedHandle(__UnmanagedAddress));
                        if (existingPtr != null)
                        {
                            existingPtr.Free();
                        }
                    }
                    
                    __InternalUnmanagedAddress = value;
                }
            }
            get
            {
                return __InternalUnmanagedAddress;
            }
        }

        internal virtual void OnCreate()
        {
        }

        internal void Pin()
        {
            if (__InternalUnmanagedAddress != IntPtr.Zero)
            {
                // when creating a new reference, we need to create a new handle so that the unmanaged code can continue to refer to this
                // instance, and that we can recycle this object until it is destroyed on the native side
                // note: this means only one netreference per unmanaged object is allowed! so when marshalling from unmanaged to managed, you should
                //       be using the factory queries, else this will assert!
                var gcHandle = GCHandle.Alloc(this);
                Internal_SetManagedHandle(__UnmanagedAddress, GCHandle.ToIntPtr(gcHandle));
            }
            else
            {
                Internal_SetManagedHandle(__UnmanagedAddress, IntPtr.Zero);
            }
        }

        internal void MarkWeak()
        {
            if (__InternalUnmanagedAddress != IntPtr.Zero)
            {
                var gcHandle = GCHandle.Alloc(this, GCHandleType.Weak);
                Internal_SetManagedHandle(__UnmanagedAddress, GCHandle.ToIntPtr(gcHandle));
            }
            else
            {
                Internal_SetManagedHandle(__UnmanagedAddress, IntPtr.Zero);
            }
        }

        public bool IsDestroyed
        {
            get
            {
                if (__UnmanagedAddress == IntPtr.Zero)
                    return true;

                return Internal_IsDestroyed(__UnmanagedAddress);
            }
        }

        static internal IntPtr GetManagedHandleFromNativePointer(IntPtr nativePtr)
        {
            return Internal_GetManagedHandle(nativePtr);
        }

        public override int GetHashCode()
        {
            return __UnmanagedAddress.ToInt32();
        }

        public void Unbind()
        {
            __UnmanagedAddress = IntPtr.Zero; // removes the handle
        }

        public void Destroy(NetReferenceDeletionFlags flags = 0)
        {
            var address = __UnmanagedAddress;
            __UnmanagedAddress = IntPtr.Zero; // removes the handle
            Internal_Destroy(address, (uint)flags); // removes the object (tags it)
        }

        protected virtual void PreDispose() { }

        protected virtual void Dispose(bool disposing)
        {
            if (!__Disposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                Debugging.Write($"{GetType()} disposed");
                if (__UnmanagedAddress != IntPtr.Zero)
                {
                    PreDispose();

                    // inform unmanaged side that this managed object is no longer valid
                    var oldAddress = __UnmanagedAddress;

                    // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                    __UnmanagedAddress = IntPtr.Zero; // implicitly untracks

                    Internal_SetManagedHandle(oldAddress, IntPtr.Zero);
                }

                // TODO: set large fields to null
                __Disposed = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~NetUnmanaged()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }
}
