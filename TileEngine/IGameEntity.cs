using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TileEngine
{
    public interface IGameEntity
    {
        int Id
        {
            get;
        }

        Vector2 Origin
        {
            get;
        }

        float CollisionRadius
        {
            get;
            set;
        }

        float Speed
        {
            get;
            set;
        }

    }
}
