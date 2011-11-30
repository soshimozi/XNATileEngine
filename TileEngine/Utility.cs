using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TileEngine
{
    public static class Utility
    {
        public static Vector2 AngleToVector(float angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public static float VectorToAngle(Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }

        public static float RadiansToDegrees(float radians)
        {
            return radians * (float)(180f / Math.PI);
        }

        public static float DegreesToRadians(float degrees)
        {
            return degrees * (float)(Math.PI / 180f);
        }

        public static float NormalizeAngle(float angle)
        {
            if (angle < 0)
                angle += 360;

            if (angle > 360)
                angle -= 360;

            return angle;
        }
    
    }
}
