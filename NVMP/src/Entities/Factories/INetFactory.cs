using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Entities
{
    /// <summary>
    /// The base implementation of an entity factory. This allows the native side of NV:MP to construct managed objects
    /// and allows you to override the implementation used.
    /// This is only ever accessed native side, and should not be used to build new instances.
    /// </summary>
    public abstract class INetFactory
    {
        public static IDictionary<uint, Func<IntPtr, NetUnmanaged>> AllocationTable = new Dictionary<uint, Func<IntPtr, NetUnmanaged>>();

        /// <summary>
        /// The concrete implementation type that will be built on factory creation calls. This should always have an inheritance
        /// of the factory's base interface.
        /// </summary>
        public virtual Type Implementation { get; }

        /// <summary>
        /// The UID native object type paired to this factory.
        /// </summary>
        public virtual uint ObjectType { get; }

        /// <summary>
        /// The unmanaged allocation helper, which wraps a native object inside the managed - and viseversa.
        /// </summary>
        /// <param name="unmanagedAddress"></param>
        /// <returns></returns>
        public abstract NetUnmanaged Allocate(IntPtr unmanagedAddress);

        public INetFactory()
        {
            if (AllocationTable.ContainsKey(ObjectType))
                throw new Exception("This factory object type has already been registered!");

            AllocationTable[ObjectType] = this.Allocate;
        }
    }

    /// <summary>
    /// All factory instances are defined inside here. The factory constructs and glues unmanaged objects with managed objects.
    /// </summary>
    public static partial class Factory
    {
        public static void Initialize()
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(Factory).TypeHandle);
        }
    }
}
