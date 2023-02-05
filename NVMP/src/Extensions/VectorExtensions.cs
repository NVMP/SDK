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
    }
}
