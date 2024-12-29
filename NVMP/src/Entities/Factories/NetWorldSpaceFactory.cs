using NVMP.Internal;
using System;
using System.Runtime.InteropServices;

namespace NVMP.Entities
{
    /// <summary>
    /// INetWorldSpace construction and management factory. Allows you to build new label.
    /// </summary>
    public class NetWorldSpaceFactory : INetFactory
    {
        [DllImport("Native", EntryPoint = "GameNetWorldSpace_GetNumWorldSpaces")]
        internal static extern uint Internal_GetNum();

        [DllImport("Native", EntryPoint = "GameNetWorldSpace_GetAllWorldSpaces")]
        internal static extern void Internal_GetAll(IntPtr[] targetArray, uint containerSize);

        [DllImport("Native", EntryPoint = "GameNetWorldSpace_CreateOrGet")]
        internal static extern IntPtr Internal_CreateOrGet(uint worldSpaceFormID);

        public override Type Implementation { get; } = typeof(NetWorldSpace);
        public override uint ObjectType { get; } = Hashing.Compute("GameNetWorldSpace");

        private object FactoryLock = new object();

        /// <summary>
        /// Subscribes new middleware for when a new label is created, called directly after the managed object is initialized.
        /// </summary>
        public event Action<INetWorldSpace> OnCreateMiddleware
        {
            add { CreationSubscriptions.Add(value); }
            remove { CreationSubscriptions.Remove(value); }
        }
        internal readonly SubscriptionDelegate<Action<INetWorldSpace>> CreationSubscriptions = new SubscriptionDelegate<Action<INetWorldSpace>>();

        public override NetUnmanaged Allocate(IntPtr unmanagedAddress)
        {
            var instance = Activator.CreateInstance(Implementation);
            var reference = instance as NetWorldSpace;
            if (reference == null)
                throw new Exception("Factory created a non-player type, is the implementation sane? It needs to inherit a concrete class");

            reference.__UnmanagedAddress = unmanagedAddress;

            if (unmanagedAddress != IntPtr.Zero)
            {
                reference.Pin();

                foreach (var midf in CreationSubscriptions.Subscriptions)
                    midf(reference);

                reference.OnCreate();
            }

            return reference;
        }

        /// <summary>
        /// Constructs or finds a worldspace with the specified form ID
        /// </summary>
        /// <returns></returns>
        public INetWorldSpace CreateOrGet(WorldspaceType formID)
        {
            // Must lock here because CreateOrGet could be called from multiple threads. Internal_CreateOrGet
            // is thread-safe, however the Allocation of a managed instance could be ran concurrently. So exclusive
            // lock this entire operation to create a critical section over the object, and the factory allocation.
            // NOTE: It could be better to lock something associated to the unmanaged ptr, but since we don't have an
            // object to that yet, this seems difficult. I'm happy to just lock the entire factory for these calls.
            lock (FactoryLock)
            {
                var unmanaged = Internal_CreateOrGet((uint)formID);
                if (unmanaged != IntPtr.Zero)
                {
                    // we want to find the object allocated to the native data. 
                    var managedHandle = NetReference.GetManagedHandleFromNativePointer(unmanaged);
                    if (managedHandle == IntPtr.Zero)
                    {
                        var inst = Allocate(IntPtr.Zero) as NetWorldSpace;
                        inst.__UnmanagedAddress = unmanaged;
                        inst.MarkWeak();
                    }

                    return Marshals.NetWorldSpaceMarshaler.Instance.MarshalNativeToManaged(unmanaged) as INetWorldSpace;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns an array of all players allocated
        /// </summary>
        public INetWorldSpace[] All
        {
            get
            {
                // First grab all the native pointers
                var num = Internal_GetNum();

                var items = new IntPtr[num];
                Internal_GetAll(items, num);

                // Now "marshal" them to managed by just wrapping player objects and assigning the native address
                var marshalled = new INetWorldSpace[num];
                for (uint i = 0; i < num; ++i)
                {
                    IntPtr unmanagedPlayerPointer = items[i];
                    marshalled[i] = Marshals.NetWorldSpaceMarshaler.Instance.MarshalNativeToManaged(unmanagedPlayerPointer) as INetWorldSpace;
                }

                return marshalled;
            }
        }
    }

    public static partial class Factory
    {
        public static NetWorldSpaceFactory WorldSpace = new NetWorldSpaceFactory();
    }
}
