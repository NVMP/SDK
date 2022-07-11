using NVMP.Internal;
using System;
using System.Runtime.InteropServices;

namespace NVMP.Entities
{
	/// <summary>
	/// INetCharacter construction and management factory. The SDK cannot create characters, only the unmanaged native side
	/// can call into this to allocate new objects.
	/// </summary>
	public class NetCharacterFactory : INetFactory
	{
		public override Type Implementation { get; } = typeof(NetCharacter);
		public override uint ObjectType { get; } = Hashing.Compute("GameNetCharacter");

		/// <summary>
		/// Subscribes new middleware for when a new character is created, called directly after the managed object is initialized.
		/// </summary>
		public event Action<INetCharacter> OnCreateMiddleware
		{
			add { CreationSubscriptions.Add(value); }
			remove { CreationSubscriptions.Remove(value); }
		}
		internal readonly SubscriptionDelegate<Action<INetCharacter>> CreationSubscriptions = new SubscriptionDelegate<Action<INetCharacter>>();

		public override NetUnmanaged Allocate(IntPtr unmanagedAddress)
		{
			var instance = Activator.CreateInstance(Implementation);
			var reference = instance as NetCharacter;
			if (reference == null)
				throw new Exception("Factory created a non-character type, is the implementation sane? It needs to inherit a concrete class");

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
	}

	public static partial class Factory
	{
		public static NetCharacterFactory Character = new NetCharacterFactory();
	}
}
