using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TileEngine
{
    public abstract class TriggerRegion
    {
        public abstract bool IsTouching(Vector2 pos, float radius);
    }
}
