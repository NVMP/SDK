using NVMP.Internal;
using System;
using System.Runtime.InteropServices;

namespace NVMP.Entities
{
    /// <summary>
    /// INetLabel construction and management factory. Allows you to build new zones.
    /// </summary>
    public class NetZoneFactory : INetFactory
    {
		[DllImport("Native", EntryPoint = "GameNetZone_GetNumZones")]
		internal static extern uint Internal_GetNumZones();

		[DllImport("Native", EntryPoint = "GameNetZone_GetAllZones")]
		internal static extern void Internal_GetAllZones(IntPtr[] zones, uint containerSize);

		[DllImport("Native", EntryPoint = "GameNetZone_Create")]
		internal static extern IntPtr Internal_Create(float radius);

        public override Type Implementation { get; } = typeof(NetZone);

        public override uint ObjectType { get; } = Hashing.Compute("GameNetZone");

        /// <summary>
        /// Subscribes new middleware for when a new zone is created, called directly after the managed object is initialized.
        /// </summary>
        public event Action<INetZone> OnCreateMiddleware
        {
            add { CreationSubscriptions.Add(value); }
            remove { CreationSubscriptions.Remove(value); }
        }
        internal readonly SubscriptionDelegate<Action<INetZone>> CreationSubscriptions = new SubscriptionDelegate<Action<INetZone>>();

        public override NetUnmanaged Allocate(IntPtr unmanagedAddress)
        {
            var instance = Activator.CreateInstance(Implementation);
            var reference = instance as NetZone;
            if (reference == null)
                throw new Exception("Factory created a non-zone type, is the implementation sane? It needs to inherit a concrete class");

            reference.__UnmanagedAddress = unmanagedAddress;

            if (unmanagedAddress != IntPtr.Zero)
            {
                // we will want to pin this as it comes from unmanaged creation
                reference.Pin();

                foreach (var midf in CreationSubscriptions.Subscriptions)
                    midf(reference);

                reference.OnCreate();
            }

            return reference;
        }

        /// <summary>
        /// Constructs a new zone with the unmanaged network stack, and returns an INetZone that can interface with it.
        /// </summary>
        /// <param name="radius">initial radius of the zone, can be changed later on</param>
        /// <returns></returns>
        public INetZone Create(float radius)
		{
			var unmanaged = Internal_Create(radius);
			if (unmanaged != IntPtr.Zero)
			{
                var inst = Allocate(IntPtr.Zero) as NetZone;
                inst.__UnmanagedAddress = unmanaged;
                inst.MarkWeak();

                foreach (var midf in CreationSubscriptions.Subscriptions)
                    midf(inst);

                inst.OnCreate();

                return inst;
			}

			return null;
		}

        /// <summary>
        /// Returns an array of all zones allocated
        /// </summary>
        public INetZone[] All
        {
            get
            {
                // First grab all the native pointers
                var numZones = Internal_GetNumZones();

                var zones = new IntPtr[numZones];
                Internal_GetAllZones(zones, numZones);

                // Now "marshal" them to managed by just wrapping player objects and assigning the native address
                var marshalledZones = new INetZone[numZones];
                for (uint i = 0; i < numZones; ++i)
                {
                    IntPtr unmanagedPlayerPointer = zones[i];
                    marshalledZones[i] = Marshals.NetZoneMarshaler.Instance.MarshalNativeToManaged(unmanagedPlayerPointer) as INetZone;
                }

                return marshalledZones;
            }
        }
    }

    public static partial class Factory
    {
        public static NetZoneFactory Zone = new NetZoneFactory();
    }
}
