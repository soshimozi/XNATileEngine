using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TileEngine
{
    static class SteeringBehaviors
    {
        public static Vector2 Seek(Vector2 target, Vector2 position, float speed, Vector2 direction)
        {
            Vector2 desired_D = Vector2.Normalize(Vector2.Subtract(target, position)) * speed;
            return Vector2.Normalize(Vector2.Subtract(desired_D, direction));
        }

        public static Vector2 Arrive(Vector2 target, Vector2 position, float currentSpeed, Vector2 direction, double arriveRadius)
        {
            Vector2 toTarget = Vector2.Subtract(target, position);
            double distance = toTarget.Length();
            if (distance > 0)
            {
                double speed = 1.0f * (distance / arriveRadius);
                speed = Math.Min(speed, 1.0f);
                return toTarget * (float)(speed / distance);
            }
            return new Vector2(0, 0);
        }
    }
}
