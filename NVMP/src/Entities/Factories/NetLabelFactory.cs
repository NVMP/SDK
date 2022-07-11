using NVMP.Internal;
using System;
using System.Runtime.InteropServices;

namespace NVMP.Entities
{
    /// <summary>
    /// INetLabel construction and management factory. Allows you to build new label.
    /// </summary>
    public class NetLabelFactory : INetFactory
	{
		[DllImport("Native", EntryPoint = "GameNetLabel_GetNumLabels")]
		internal static extern uint Internal_GetNumLabels();

		[DllImport("Native", EntryPoint = "GameNetLabel_GetAllLabels")]
		internal static extern void Internal_GetAllLabels(IntPtr[] labels, uint containerSize);

		[DllImport("Native", EntryPoint = "GameNetLabel_Create")]
		internal static extern IntPtr Internal_Create();

        public override Type Implementation { get; } = typeof(NetLabel);

        public override uint ObjectType { get; } = Hashing.Compute("GameNetLabel");

        /// <summary>
        /// Subscribes new middleware for when a new label is created, called directly after the managed object is initialized.
        /// </summary>
        public event Action<INetLabel> OnCreateMiddleware
        {
            add { CreationSubscriptions.Add(value); }
            remove { CreationSubscriptions.Remove(value); }
        }
        internal readonly SubscriptionDelegate<Action<INetLabel>> CreationSubscriptions = new SubscriptionDelegate<Action<INetLabel>>();

        public override NetUnmanaged Allocate(IntPtr unmanagedAddress)
        {
            var instance = Activator.CreateInstance(Implementation);
            var reference = instance as NetLabel;
            if (reference == null)
                throw new Exception("Factory created a non-label type, is the implementation sane? It needs to inherit a concrete class");

            reference.__UnmanagedAddress = unmanagedAddress;

            if (unmanagedAddress != IntPtr.Zero)
            {
                reference.Pin();

                foreach (var midf in CreationSubscriptions.Subscriptions)
                    midf(reference);
            }

            return reference;
        }

        /// <summary>
		/// Constructs a new label with the unmanaged network stack, and returns an INetLabel that can interface with it.
        /// </summary>
        /// <returns></returns>
		public INetLabel Create()
		{
			var unmanaged = Internal_Create();
			if (unmanaged != IntPtr.Zero)
            {
                var inst = Allocate(IntPtr.Zero) as NetLabel;
                inst.__UnmanagedAddress = unmanaged;
                inst.MarkWeak();

                foreach (var midf in CreationSubscriptions.Subscriptions)
                    midf(inst);

                return inst;
            }

			return null;
		}

        /// <summary>
        /// Returns an array of all labels allocated
        /// </summary>
        public INetLabel[] All
        {
            get
            {
                // First grab all the native pointers
                var numLabels = Internal_GetNumLabels();

                var labels = new IntPtr[numLabels];
                Internal_GetAllLabels(labels, numLabels);

                // Now "marshal" them to managed by just wrapping player objects and assigning the native address
                var marshalledLabels = new INetLabel[numLabels];
                for (uint i = 0; i < numLabels; ++i)
                {
                    IntPtr unmanagedPlayerPointer = labels[i];
                    marshalledLabels[i] = Marshals.NetLabelMarshaler.Instance.MarshalNativeToManaged(unmanagedPlayerPointer) as INetLabel;
                }

                return marshalledLabels;
            }
        }
    }

    public static partial class Factory
    {
        public static NetLabelFactory Label = new NetLabelFactory();
    }
}
