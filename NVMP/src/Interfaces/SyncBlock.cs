using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace NVMP
{
    public interface ISyncBlockInterface
    {
        public enum BlockType
        {
            Unblocked,

            /// <summary>
            /// Will block only encounters (dropped items, actors, etc)
            /// </summary>
            Encounters,

            /// <summary>
            /// Will block all references, including characters
            /// </summary>
            All
        }

        public void SetInteriorBlocked(uint interiorID, bool blocked, BlockType type);
        public bool IsInteriorBlocked(uint interiorID);

        public void SetRefBlocked(uint refID, bool blocked);
        public bool IsRefBlocked(uint refID);
        public void SetWorldspaceBlocked(Worldspace.WorldspaceType worldspaceID, Vector3 pos, float radius, BlockType type);
    }

    public class SyncBlockManager : ISyncBlockInterface
    {
        #region Natives

        [DllImport("Native", EntryPoint = "SyncBlock_SetInteriorBlocked")]
        private static extern void Internal_SetInteriorBlocked(uint interiorID, bool blocked, uint type);

        [DllImport("Native", EntryPoint = "SyncBlock_GetInteriorBlocked")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_GetInteriorBlocked(uint interiorID);

        [DllImport("Native", EntryPoint = "SyncBlock_SetRefBlocked")]
        private static extern void Internal_SetRefBlocked(uint interiorID, bool blocked);

        [DllImport("Native", EntryPoint = "SyncBlock_GetRefBlocked")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Internal_GetRefBlocked(uint interiorID);

        [DllImport("Native", EntryPoint = "SyncBlock_SetWorldspaceBlocked")]
        private static extern void Internal_SetWorldspaceBlocked(uint worldspaceID, float x, float y, float z, float radius, uint type);

        #endregion

        public bool IsInteriorBlocked(uint interiorID)
        {
            return Internal_GetInteriorBlocked(interiorID);
        }

        public bool IsRefBlocked(uint refID)
        {
            return Internal_GetRefBlocked(refID);
        }

        public void SetInteriorBlocked(uint interiorID, bool blocked, ISyncBlockInterface.BlockType type)
        {
            Internal_SetInteriorBlocked(interiorID, blocked, (uint)type);
        }

        public void SetRefBlocked(uint refID, bool blocked)
        {
            Internal_SetRefBlocked(refID, blocked);
        }

        public void SetWorldspaceBlocked(Worldspace.WorldspaceType worldspaceID, Vector3 pos, float radius, ISyncBlockInterface.BlockType type)
        {
            Internal_SetWorldspaceBlocked((uint)worldspaceID, pos.X, pos.Y, pos.Z, radius, (uint)type);
        }
    }

}
