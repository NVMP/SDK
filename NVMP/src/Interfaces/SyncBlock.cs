using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NVMP
{
    public interface ISyncBlockInterface
    {
        public void SetInteriorBlocked(uint interiorID, bool blocked);
        public bool IsInteriorBlocked(uint interiorID);

        public void SetRefBlocked(uint refID, bool blocked);
        public bool IsRefBlocked(uint refID);
    }

    public class SyncBlockManager : ISyncBlockInterface
    {
        #region Natives

        [DllImport("Native", EntryPoint = "SyncBlock_SetInteriorBlocked")]
        private static extern void Internal_SetInteriorBlocked(uint interiorID, bool blocked);

        [DllImport("Native", EntryPoint = "SyncBlock_GetInteriorBlocked")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_GetInteriorBlocked(uint interiorID);

        [DllImport("Native", EntryPoint = "SyncBlock_SetRefBlocked")]
        private static extern void Internal_SetRefBlocked(uint interiorID, bool blocked);

        [DllImport("Native", EntryPoint = "SyncBlock_GetRefBlocked")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_GetRefBlocked(uint interiorID);

        #endregion

        public bool IsInteriorBlocked(uint interiorID)
        {
            return Internal_GetInteriorBlocked(interiorID);
        }

        public bool IsRefBlocked(uint refID)
        {
            return Internal_GetRefBlocked(refID);
        }

        public void SetInteriorBlocked(uint interiorID, bool blocked)
        {
            Internal_SetInteriorBlocked(interiorID, blocked);
        }

        public void SetRefBlocked(uint refID, bool blocked)
        {
            Internal_SetRefBlocked(refID, blocked);
        }
    }

}
