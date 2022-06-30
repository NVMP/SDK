using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NVMP.Extensions
{
    public static class QuaternionExtensions
    {
        #region Natives
        [DllImport("Native", EntryPoint = "Extensions_QToEulers")]
        private static extern void Internal_ToEulers(float qx, float qy, float qz, float qw, ref float x, ref float y, ref float z);
        #endregion

        public static Vector3 ToEulers(this Quaternion q)
        {
            var result = new Vector3();
            Internal_ToEulers(q.X, q.Y, q.Z, q.W, ref result.X, ref result.Y, ref result.Z);
            return result;
        }
    }
}
