using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Entities
{
    public interface INetLabelLineStack
    {
        /// <summary>
        /// Inserts a new line and returns the reference
        /// </summary>
        /// <returns></returns>
        public INetLabelLine Push();

        /// <summary>
        /// Removes the last line
        /// </summary>
        public void Pop();

        /// <summary>
        /// Removes all labels
        /// </summary>
        public void Clear();

        /// <summary>
        /// Number of lines currently allocated
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Enumerator for the underlying native line list
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator();
    }
}
