using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NVMP.Internal
{
    /// <summary>
    /// Please don't use this outside of NVMP.Startup. It's a C# hack that allows the CLR to query natively up to the parent process, 
    /// but it is done in a way that static runtime initialisation takes precedence, allowing future .dll's to take advantage.
    /// 
    /// It really has no purpose outside of NVMP.Startup, and will likely break things if you try to use it.
    /// </summary>
    public static class NativeResolver
    {
        public delegate void GCCollect();
        public delegate void GCHandleFree(IntPtr handle);
        public delegate bool GCIsHandleValid(IntPtr handle);
        public delegate void AllocateManagedForUnmanaged(IntPtr unmanaged);

        [DllImport("Native", EntryPoint = "BindManagedFunctions")]
        internal static extern void Internal_BindManagedFunctions(GCCollect collect, GCHandleFree handleFree, AllocateManagedForUnmanaged allocateManagedFunc, GCIsHandleValid isGCHandle);

        [DllImport("Native", EntryPoint = "GameServer_SetMainThreadRunning")]
        public static extern void SetMainThreadRunning(bool running);

        public static void BindManagedFunctions(GCCollect collect, GCHandleFree handleFree, AllocateManagedForUnmanaged allocateManagedFunc, GCIsHandleValid isGCHandle)
        {
            // dirty hack !!!
            GCHandle.Alloc(collect);
            GCHandle.Alloc(handleFree);
            GCHandle.Alloc(allocateManagedFunc);
            GCHandle.Alloc(isGCHandle);

            Internal_BindManagedFunctions(collect, handleFree, allocateManagedFunc, isGCHandle);
        }


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string l);

        // Initializes the static native resolver into assembly view
        public static void Initialize()
        { /* magic */ }

        public static IntPtr OnResolveDllImport(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            if (libraryName == "Native")
            {
                IntPtr dllHandle = GetModuleHandle(null);

                if (dllHandle == IntPtr.Zero)
                {
                    Console.WriteLine("NativeResolver failed to find native process. Please note that you cannot run assemblies standalone, they must be wrapped in a parent CLR host (the server)");
                    Console.WriteLine(" MarshalError: " + Marshal.GetLastWin32Error());
                    return IntPtr.Zero;
                }

                return dllHandle;
            }

            return IntPtr.Zero;
        }

        static NativeResolver()
        {
            NativeLibrary.SetDllImportResolver(Assembly.GetAssembly(typeof(NativeResolver)), OnResolveDllImport);
            Console.WriteLine($"[netcore] Native is bound (OS: {Environment.OSVersion}, {Environment.OSVersion.Platform}; Net: {Environment.Version})");
        }
    }
}
