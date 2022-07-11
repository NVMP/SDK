using NVMP.Internal;
using System;
using System.Runtime.InteropServices;

namespace NVMP.Entities
{
	public class NetActorFactory : INetFactory
	{
		[DllImport("Native", EntryPoint = "GameNetActor_CreateActor")]
		internal static extern IntPtr Internal_CreateActor(uint formID, uint refID);

		public override Type Implementation { get; } = typeof(NetActor);

		public override uint ObjectType { get; } = Hashing.Compute("GameNetActor");

		/// <summary>
		/// Subscribes new middleware for when a new actor is created, called directly after the managed object is initialized.
		/// </summary>
		public event Action<INetActor> OnCreateMiddleware
        {
			add { CreationSubscriptions.Add(value); }
			remove { CreationSubscriptions.Remove(value); }
        }
		internal readonly SubscriptionDelegate<Action<INetActor>> CreationSubscriptions = new SubscriptionDelegate<Action<INetActor>>();

		public override NetUnmanaged Allocate(IntPtr unmanagedAddress)
		{
			var instance = Activator.CreateInstance(Implementation);
			var reference = instance as NetActor;
			if (reference == null)
				throw new Exception("Factory created a non-actor type, is the implementation sane? It needs to inherit a concrete class");

			reference.__UnmanagedAddress = unmanagedAddress;

			if (unmanagedAddress != IntPtr.Zero)
			{
				// we will want to pin this as it comes from unmanaged creation
				reference.Pin();

				foreach (var midf in CreationSubscriptions.Subscriptions)
					midf(reference);
			}

			return reference;
		}

		/// <summary>
		/// Constructs a new actor with the unmanaged network stack, and returns an INetActor that can interface with it.
		/// </summary>
		/// <param name="formID"></param>
		/// <param name="refID"></param>
		/// <returns></returns>
		public INetActor Create(uint formID, uint refID = uint.MinValue)
		{
			var unmanaged = Internal_CreateActor(formID, refID);
			if (unmanaged != IntPtr.Zero)
            {
				var instance = Allocate(IntPtr.Zero) as NetActor;
				instance.__UnmanagedAddress = unmanaged;
				instance.MarkWeak();

				foreach (var midf in CreationSubscriptions.Subscriptions)
					midf(instance);

				return instance;
            }

			return null;
		}
    }

	public static partial class Factory
    {
		public static NetActorFactory Actor = new NetActorFactory();
	}
}
