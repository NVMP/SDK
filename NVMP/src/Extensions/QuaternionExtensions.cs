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

        public const float QEpsilon = 0.0001f;

        /// <summary>
        /// X-yaw, Y-pitch, Z-roll
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public static Vector3 ToEulers(this Quaternion q)
        {
            var result = new Vector3();
            Internal_ToEulers(q.X, q.Y, q.Z, q.W, ref result.X, ref result.Y, ref result.Z);
            return result;
        }

        public static float Angle(this Quaternion q, Quaternion b)
        {
            float dot = MathF.Min(1.0f, MathF.Abs(Quaternion.Dot(q, b)));
            return IsEqual(dot) ? 0.0f : MathF.Acos(dot) * 2.0f;
        }

        public static bool IsEqual(float dot, float epsilon = QEpsilon)
        {
            return dot > 1.0f - epsilon;
        }

        public static bool IsEqual(this Quaternion q, Quaternion b, float epsilon = QEpsilon)
        {
            float dot = MathF.Min(1.0f, MathF.Abs(Quaternion.Dot(q, b)));
            return IsEqual(dot, epsilon);
        }
    }
}
