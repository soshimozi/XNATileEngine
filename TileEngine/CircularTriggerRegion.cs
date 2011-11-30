using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TileEngine
{
    public class CircularTriggerRegion : TriggerRegion
    {
        private Vector2 _position;
        private float _radius;

        public CircularTriggerRegion(Vector2 pos, float radius)
        {
            _position = pos;
            _radius = radius;
        }

        public override bool IsTouching(Vector2 pos, float radius)
        {
            return Vector2.DistanceSquared(_position, pos) < (radius + _radius) * (radius + _radius);
        }
    }
}
