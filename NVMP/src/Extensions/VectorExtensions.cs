using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace NVMP.Extensions
{
    public static class V3
    {
        public static Vector3 Forward = new Vector3(0.0f, 1.0f, 0.0f);

        public static Vector3 Left = new Vector3(1.0f, 0.0f, 0.0f);

        public static Vector3 Up = new Vector3(0.0f, 0.0f, 1.0f);
    }

    public static class VectorExtensions
    {
        /// <summary>
        /// Projects euler radians to a directional vector.
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector3 EulersToVector(this Vector3 vec)
        {
            float yaw = vec.Y - ((float)Math.PI / 2);

            var result = new Vector3
            {
                X = (float)(Math.Cos(yaw) * Math.Cos(vec.X)),
                Y = (float)(Math.Sin(yaw) * Math.Cos(vec.X)),
                Z = (float)(Math.Sin(vec.X))
            };

            return result;
        }

        /// <summary>
        /// Constrains an angle and wraps it to the nearest normalization.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float ConstrainAngle(float x)
        {
            x = (x + MathF.PI) % (MathF.PI * 2.0f);
            if (x < 0.0f)
                x += (MathF.PI * 2.0f);

            return x - MathF.PI;
        }

        /// <summary>
        /// Returns the radian angle between two Euler vectors, with wrapping considered.
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static Vector3 EulerAngleDifference(this Vector3 vec, Vector3 other)
        {
            var result = new Vector3
            {
                X = ConstrainAngle(other.X - vec.X),
                Y = ConstrainAngle(other.Y - vec.Y),
                Z = ConstrainAngle(other.Z - vec.Z)
            };

            return result;
        }
    }
}
