using System.Runtime.InteropServices;

namespace NVMP
{
    /// <summary>
    /// Mod management. Allows you to query information about registered mods on the server.
    /// </summary>
    public static class ModManager
    {
        #region Natives
        //
        // Mods
        //
        [DllImport("Native", EntryPoint = "ModManager_FindAvailableModByName")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool Internal_FindAvailableModByName(string name, ref string filename, ref string digest, ref byte index);

        [DllImport("Native", EntryPoint = "ModManager_GetESPIndex")]
        internal static extern int Internal_NumAvailableMods(uint modIndex);

        [DllImport("Native", EntryPoint = "ModManager_NumAvailableMods")]
        internal static extern uint Internal_NumAvailableMods();

        [DllImport("Native", EntryPoint = "ModManager_GetAvailableMod")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool Internal_GetAvailableMod(uint index, ref string filename, ref string name, ref string digest);
        #endregion

        static public ModFile FindModByName(string mod)
        {
            string digest = null;
            string filePath = null;
            byte modIndex = 0;
            if (Internal_FindAvailableModByName(mod, ref filePath, ref digest, ref modIndex))
            {
                var sMod = new ModFile
                {
                    Digest = digest,
                    FilePath = filePath,
                    Name = mod,
                    Index = ((uint)modIndex << 24)
                };

                return sMod;
            }

            return null;
        }

        static public ModFile[] GetMods()
        {
            string digest = null;
            string filePath = null;
            string name = null;

            uint numMods = Internal_NumAvailableMods();

            var result = new ModFile[numMods];
            for (uint i = 0; i < numMods; ++i)
            {
                if (Internal_GetAvailableMod(i, ref filePath, ref name, ref digest))
                {
                    result[i] = new ModFile
                    {
                        Digest = digest,
                        FilePath = filePath,
                        Name = name,
                        Index = (i << 24)
                    };
                }
            }

            return result;
        }

        static public ModFile FindModUsedByReference(uint refId)
        {
            var mods = GetMods();
            foreach (var mod in mods)
            {
                if (mod.Index == ((refId >> 24) & 0xFF))
                    return mod;
            }
            return null;
        }
    }
}
