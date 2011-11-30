using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TileEngine
{
    public static class Engine
    {
        public const int TileWidth = 32;
        public const int TileHeight = 32;

        public static Point GetPointFromCell(Vector2 position)
        {
            return new Point(
                (int)(position.X / TileWidth),
                (int)(position.Y / TileHeight));
        }

        public static Rectangle CreateRectangleForCell(Point cell)
        {
            return new Rectangle(
                cell.X * TileWidth,
                cell.Y * TileHeight,
                TileWidth,
                TileHeight);
        }
    }
}
