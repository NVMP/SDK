using System;
using System.Runtime.InteropServices;

namespace NVMP.Entities
{
    internal class NetZone : NetReference, INetZone
    {
        #region Events
        internal delegate void GameILReferenceEntered
        (
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshals.NetReferenceMarshaler))]
                INetReference reference
        );
        internal delegate void GameILReferenceExited
        (
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshals.NetReferenceMarshaler))]
                INetReference reference
        );
        #endregion

        #region Natives

        [DllImport("Native", EntryPoint = "GameNetZone_SetRadius")]
        internal static extern void Internal_SetRadius(IntPtr zone, float radius);

        [DllImport("Native", EntryPoint = "GameNetZone_GetRadius")]
        internal static extern float Internal_GetRadius(IntPtr zone);

        [DllImport("Native", EntryPoint = "GameNetZone_SetReferenceEntered")]
        internal static extern void Internal_SetReferenceEntered(IntPtr zone, GameILReferenceEntered entered);

        [DllImport("Native", EntryPoint = "GameNetZone_SetReferenceExited")]
        internal static extern void Internal_SetReferenceExited(IntPtr zone, GameILReferenceExited exited);

        [DllImport("Native", EntryPoint = "GameNetZone_SetDescription", CharSet = CharSet.Unicode)]
        internal static extern void Internal_SetDescription(IntPtr zone, [MarshalAs(UnmanagedType.LPWStr)] string desc);

        [DllImport("Native", EntryPoint = "GameNetZone_GetDescription", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        internal static extern string Internal_GetDescription(IntPtr zone);
        #endregion

        public Action<INetReference> ReferenceEntered
        {
            set
            {
                ReferenceEnteredDelegate = r => value(r);
                Internal_SetReferenceEntered(__UnmanagedAddress, ReferenceEnteredDelegate);
            }
        }
        internal GameILReferenceEntered ReferenceEnteredDelegate;

        public Action<INetReference> ReferenceExited
        {
            set
            {
                ReferenceExitedDelegate = r => value(r);
                Internal_SetReferenceExited(__UnmanagedAddress, ReferenceExitedDelegate);
            }
        }
        internal GameILReferenceExited ReferenceExitedDelegate;

        public float Radius
        {
            get
            {
                return Internal_GetRadius(__UnmanagedAddress);
            }
            set
            {
                Internal_SetRadius(__UnmanagedAddress, value);
            }
        }

        public string Description
        {
            get
            {
                return Internal_GetDescription(__UnmanagedAddress);
            }
            set
            {
                Internal_SetDescription(__UnmanagedAddress, value);
            }
        }
    }
}
