using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TileEngine
{
    public class ProximityRegion : TriggerRegion
    {
        private readonly BaseGameEntity _entity;
        private readonly float _triggerRadius;

        public ProximityRegion(BaseGameEntity entity, float triggerRadius)
        {
            _entity = entity;
            _triggerRadius = triggerRadius;
        }

        public override bool IsTouching(Vector2 pos, float radius)
        {
            return Vector2.DistanceSquared(_entity.Position, pos) < (radius + _triggerRadius) * (radius + _triggerRadius);
        }
    }
}
