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

        /// <summary>
        /// Sets a worldspace block. Returns a unique handle.
        /// </summary>
        /// <param name="worldspaceID"></param>
        /// <param name="pos"></param>
        /// <param name="radius"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public uint SetWorldspaceBlocked(WorldspaceType worldspaceID, Vector3 pos, float radius, BlockType type);

        /// <summary>
        /// Removes a worldspace block by its handle.
        /// </summary>
        /// <param name="handle"></param>
        public void RemoveWorldspaceBlock(uint handle);
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
        private static extern uint Internal_SetWorldspaceBlocked(uint worldspaceID, float x, float y, float z, float radius, uint type);

        [DllImport("Native", EntryPoint = "SyncBlock_RemoveWorldspaceBlock")]
        private static extern void Internal_RemoveWorldspaceBlock(uint id);

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

        public uint SetWorldspaceBlocked(WorldspaceType worldspaceID, Vector3 pos, float radius, ISyncBlockInterface.BlockType type)
        {
            return Internal_SetWorldspaceBlocked((uint)worldspaceID, pos.X, pos.Y, pos.Z, radius, (uint)type);
        }

        public void RemoveWorldspaceBlock(uint handle)
        {
            Internal_RemoveWorldspaceBlock(handle);
        }
    }

}
