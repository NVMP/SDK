using NVMP.Internal;
using System;
using System.Runtime.InteropServices;

namespace NVMP.Entities
{
	/// <summary>
	/// INetReference construction and management factory. Allows you to build new references.
	/// </summary>
	public class NetReferenceFactory : INetFactory
	{
        [DllImport("Native", EntryPoint = "GameNetReference_CreateObject")]
		internal static extern IntPtr Internal_CreateObject(uint formID, uint refID);

		[DllImport("Native", EntryPoint = "GameNetReference_GetNetworkType")]
		internal static extern uint Internal_GetNetworkType(IntPtr self);

		public override Type Implementation { get; } = typeof(NetReference);

		public override uint ObjectType { get; } = Hashing.Compute("GameNetReference");

		/// <summary>
		/// Subscribes new middleware for when a new reference is created, called directly after the managed object is initialized.
		/// </summary>
		public event Action<INetReference> OnCreateMiddleware
		{
			add { CreationSubscriptions.Add(value); }
			remove { CreationSubscriptions.Remove(value); }
		}
		internal readonly SubscriptionDelegate<Action<INetReference>> CreationSubscriptions = new SubscriptionDelegate<Action<INetReference>>();

		public override NetUnmanaged Allocate(IntPtr unmanagedAddress)
		{
			var instance = Activator.CreateInstance(Implementation);
			var reference = instance as NetReference;
			if (reference == null)
				throw new Exception("Factory created a non-reference type, is the implementation sane? It needs to inherit a concrete class");

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
        /// Constructs a new TESObjectREFR. This is not recommended for actors, use NetActor.Create as it will ensure
        /// specific actor information is networked additionally
        /// </summary>
        /// <param name="formID"></param>
        /// <param name="refID"></param>
        /// <returns></returns>
        public INetReference Create(uint formID, uint refID = uint.MinValue)
		{
			// compose the unmanaged variant, and then construct a new netreference
			var unmanaged = Internal_CreateObject(formID, refID);
			if (unmanaged != IntPtr.Zero)
			{
				var instance = Allocate(IntPtr.Zero) as NetReference;
				instance.__UnmanagedAddress = unmanaged;
				instance.MarkWeak();

				foreach (var midf in CreationSubscriptions.Subscriptions)
					midf(instance);

				instance.OnCreate();

				return instance;
			}

			return null;
		}

		public static uint GetUnmanagedNetworkType(IntPtr ptr)
		{
			return Internal_GetNetworkType(ptr);
		}
	}

	public static partial class Factory
	{
		public static NetReferenceFactory Reference = new NetReferenceFactory();
	}
}
