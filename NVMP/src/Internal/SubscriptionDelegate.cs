using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Internal
{
    internal class SubscriptionDelegate<T> where T : class
    {
        internal List<T> InternalSubscriptions;

        public IReadOnlyList<T> Subscriptions
        {
            get => InternalSubscriptions;
        }

        public SubscriptionDelegate()
        {
            InternalSubscriptions = new List<T>();
        }

        public void Add(T del)
        {
            lock (InternalSubscriptions)
            {
                InternalSubscriptions.Add(del);
            }
        }

        public void Remove(T del)
        {
            lock (InternalSubscriptions)
            {
                InternalSubscriptions.Remove(del);
            }
        }
    }
}
