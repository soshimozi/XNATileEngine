using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace TileEngine
{
    public class TileMap
    {
        public List<TileLayer> Layers = new List<TileLayer>();
        public CollisionLayer CollisionLayer;

        public int GetWidthInPixels()
        {
            return GetWidth() * TileLayer.TileWidth;
        }

        public int GetHeightInPixels()
        {
            return GetHeight() * TileLayer.TileHeight;
        }

        public int GetWidth()
        {
            int maxWidth = 0;

            foreach (TileLayer layer in Layers)
            {
                maxWidth = (int)Math.Max(maxWidth, layer.Width);
            }

            return maxWidth;
        }

        public int GetHeight()
        {
            int maxHeight = 0;

            foreach (TileLayer layer in Layers)
            {
                maxHeight = (int)Math.Max(maxHeight, layer.Height);
            }

            return maxHeight;
        }


        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            foreach (TileLayer layer in Layers)
            {
                layer.Draw(spriteBatch, camera);
            }
        }
    }
}
