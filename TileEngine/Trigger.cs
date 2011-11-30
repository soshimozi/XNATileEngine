using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TileEngine
{
    public abstract class Trigger
    {
        private TriggerRegion m_RegionOfInfluence = null;

        public bool RemoveFromGame
        {
            get;
            set;
        }

        public void AddTriggerRegion(TriggerRegion region)
        {
            m_RegionOfInfluence = region;
        }

        protected bool IsTouchingTrigger(Vector2 pos, float radius)
        {
            if (m_RegionOfInfluence != null)
            {
                return m_RegionOfInfluence.IsTouching(pos, radius);
            }

            return false;
        }

        public abstract void Try(BaseGameEntity entity);
        public abstract void Update(GameTime gameTime);
    }
}
