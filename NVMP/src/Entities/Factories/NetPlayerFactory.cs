using NVMP.Internal;
using System;
using System.Runtime.InteropServices;

namespace NVMP.Entities
{
    /// <summary>
    /// INetPlayer construction and management factory. Allows you to build new label.
    /// </summary>
    public class NetPlayerFactory : INetFactory
    {
        [DllImport("Native", EntryPoint = "NetPlayer_GetNumPlayers")]
        internal static extern uint Internal_GetNumPlayers();

        [DllImport("Native", EntryPoint = "NetPlayer_GetAllPlayers")]
        internal static extern void Internal_GetAllPlayers(IntPtr[] players, uint containerSize);

        public override Type Implementation { get; } = typeof(NetPlayer);

        public override uint ObjectType { get; } = Hashing.Compute("NetPlayer");

        /// <summary>
        /// Subscribes new middleware for when a new label is created, called directly after the managed object is initialized.
        /// </summary>
        public event Action<INetPlayer> OnCreateMiddleware
        {
            add { CreationSubscriptions.Add(value); }
            remove { CreationSubscriptions.Remove(value); }
        }
        internal readonly SubscriptionDelegate<Action<INetPlayer>> CreationSubscriptions = new SubscriptionDelegate<Action<INetPlayer>>();

        public override NetUnmanaged Allocate(IntPtr unmanagedAddress)
        {
            var instance = Activator.CreateInstance(Implementation);
            var reference = instance as NetPlayer;
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
        /// Returns an array of all players allocated
        /// </summary>
        public INetPlayer[] All
        {
            get
            {
                // First grab all the native pointers
                var num = Internal_GetNumPlayers();

                var players = new IntPtr[num];
                Internal_GetAllPlayers(players, num);

                // Now "marshal" them to managed by just wrapping player objects and assigning the native address
                var marshalledPlayers = new INetPlayer[num];
                for (uint i = 0; i < num; ++i)
                {
                    IntPtr unmanagedPlayerPointer = players[i];
                    marshalledPlayers[i] = Marshals.NetPlayerMarshaler.Instance.MarshalNativeToManaged(unmanagedPlayerPointer) as INetPlayer;
                }

                return marshalledPlayers;
            }
        }
    }

    public static partial class Factory
    {
        public static NetPlayerFactory Player = new NetPlayerFactory();
    }
}
