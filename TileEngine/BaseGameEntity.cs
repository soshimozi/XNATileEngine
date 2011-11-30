using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TileEngine
{
    public class BaseGameEntity : IGameEntity
    {
        private float radius = 16f;
        private float _speed;
        protected readonly TriggerSystem _triggerSystem;

        public Vector2 Position = Vector2.Zero;
        public Vector2 OriginOffset = Vector2.Zero;
        
        public BaseGameEntity(int id) 
        {
            Id = id;
        }

        public int Id
        {
            get;
            private set;
        }

        public Vector2 Origin
        {
            get { return Position + OriginOffset; }
        }

        public float CollisionRadius
        {
            get { return radius; }
            set { radius = (float)Math.Max(value, 1f); }
        }

        public Vector2 Direction
        {
            get;
            set;
        }

        public float Speed
        {
            get { return _speed; }

            /* clamp speed to 1.0 minimum */
            set { _speed = (float)Math.Max(value, .1f); }
        }

    }
}
