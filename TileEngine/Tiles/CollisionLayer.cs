using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;

namespace TileEngine
{
    public class CollisionLayer
    {
        private int[,] _map;

        public CollisionLayer(int width, int height)
        {
            _map = new int[height, width];

            // initialize our map
            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    _map[h, w] = -1;
                }
            }
        }

        private enum ParseState
        {
            None,
            Texture,
            Options,
            Layout
        }

        public void Save(string filename, string[] textureNames)
        {
            using(StreamWriter writer = new StreamWriter(filename) )
            {
                writer.WriteLine("[Layout]");
                for (int y = 0; y < Height; y++)
                {
                    string line = string.Empty;
                    for (int x = 0; x < Width; x++)
                    {
                        line += _map[y, x].ToString() + " ";
                    }

                    writer.WriteLine(line);
                }

            }

        }
    
        public static CollisionLayer FromFile(string rootDirectory, string filename)
        {
            CollisionLayer CollisionLayer;
            ParseState parseState = ParseState.None;

            List<List<int>> tempLayout = new List<List<int>>();

            filename = string.Format("{0}/{1}", rootDirectory, filename);
            using (StreamReader reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();

                    if (!string.IsNullOrEmpty(line))
                    {
                        if (line.Contains("[Layout]"))
                        {
                            parseState = ParseState.Layout;
                        }
                        else
                        {
                            switch (parseState)
                            {
                                case ParseState.Layout:
                                    List<int> row = new List<int>();

                                    string[] cells = line.Split(' ');
                                    foreach (string c in cells)
                                    {
                                        if (!string.IsNullOrEmpty(c))
                                        {
                                            row.Add(int.Parse(c));
                                        }
                                    }

                                    tempLayout.Add(row);
                                    break;
                            }
                        }
                    }

                }
            }

            int width = tempLayout[0].Count;
            int height = tempLayout.Count;

            CollisionLayer = new CollisionLayer(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    CollisionLayer.SetCellIndex(x, y, tempLayout[y][x]);
                }
            }

            return CollisionLayer;
        }

        public void SetCellIndex(int x, int y, int cellIndex)
        {
            if (x < Width && y < Height)
                _map[y, x] = cellIndex;
        }

        public void SetCellIndex(Point point, int cellIndex)
        {
            if (point.X < Width && point.Y < Height)
                _map[point.Y, point.X] = cellIndex;
        }

        public int GetCellIndex(Point point)
        {
            if (point.X < Width && point.Y < Height)
                return _map[point.Y, point.X];
            else
                return -1;
        }

        public int GetCellIndex(int x, int y)
        {
            if (x < Width && y < Height)
                return _map[y, x];
            else
                return -1;
        }

        static public int TileWidth
        {
            get { return Engine.TileWidth; }
        }

        static public int TileHeight
        {
            get { return Engine.TileHeight; }
        }

        public int Width
        {

            get { return _map.GetLength(1); }
        }

        public int Height
        {
            get { return _map.GetLength(0); }
        }
            

    }
}
