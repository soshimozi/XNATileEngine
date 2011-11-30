using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;

namespace TileEngine
{
    public class TileLayer
    {
        //static private int tileWidth = 64;
        //static private int tileHeight = 64;

        private List<Texture2D> _tileTextures = new List<Texture2D>();
        private int[,] _map;
        float alpha = 1f;

        public TileLayer(int width, int height)
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

        public TileLayer(int[,] existingMap)
        {
            // copy existing map into our map
            _map = (int[,])existingMap.Clone();
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
                writer.WriteLine("[Textures]");
                foreach(string t in textureNames)
                    writer.WriteLine(t);

                writer.WriteLine();

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

        public int IndexOfTexture(Texture2D texture)
        {
            if (_tileTextures.Contains(texture))
            {
                return _tileTextures.IndexOf(texture);
            }

            return -1;
        }
    
        public static TileLayer FromFile(string filename, out string [] textures)
        {
            TileLayer tileLayer;
            ParseState parseState = ParseState.None;

            List<string> textureNames = new List<string>();
            List<List<int>> tempLayout = new List<List<int>>();

            // TODO: move into options structure
            float alpha = 1f;

            using (StreamReader reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();

                    if (!string.IsNullOrEmpty(line))
                    {
                        if (line.Contains("[Textures]"))
                        {
                            parseState = ParseState.Texture;
                        }
                        else if (line.Contains("[Layout]"))
                        {
                            parseState = ParseState.Layout;
                        }
                        else if (line.Contains("[Options]"))
                        {
                            // TODO: Move into [Options] section
                            // parse options like this:  OptionName = OptionValue
                            parseState = ParseState.Options;
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

                                case ParseState.Texture:
                                    textureNames.Add(line);
                                    break;

                                case ParseState.Options:
                                    string[] nameValuePair = line.Split('=');

                                    // hopefully we have two
                                    if (nameValuePair.Length == 2)
                                    {
                                        if (nameValuePair[0].Trim().Equals("Alpha", StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            alpha = float.Parse(nameValuePair[1]);
                                        }
                                    }

                                    break;


                            }
                        }
                    }

                }
            }

            int width = tempLayout[0].Count;
            int height = tempLayout.Count;

            tileLayer = new TileLayer(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    tileLayer.SetCellIndex(x, y, tempLayout[y][x]);
                }
            }

            //tileLayer.LoadTileTextures(
            //    content,
            //    textureNames.ToArray());

            tileLayer.Alpha = alpha;


            textures = textureNames.ToArray();

            return tileLayer;
        }

        public static TileLayer FromFile(ContentManager content, string filename)
        {

            string[] textureNames;
            TileLayer tileLayer = FromFile(string.Format("{0}/{1}", content.RootDirectory, filename), out textureNames);

            tileLayer.LoadTileTextures(
                content, 
                textureNames);

            return tileLayer;
        }

        public void SetCellIndex(int x, int y, int cellIndex)
        {
            _map[y, x] = cellIndex;
        }

        public void SetCellIndex(Point point, int cellIndex)
        {
            _map[point.Y, point.X] = cellIndex;
        }

        public int GetCellIndex(Point point)
        {
            return _map[point.Y, point.X];
        }

        public int GetCellIndex(int x, int y)
        {
            return _map[y, x];
        }

        public void LoadTileTextures(ContentManager content, params string[] textureNames)
        {
            Texture2D texture;
            foreach (string textureName in textureNames)
            {
                texture = content.Load<Texture2D>(textureName);
                _tileTextures.Add(texture);
            }
        }

        public void AddTexture(Texture2D texture)
        {
            _tileTextures.Add(texture);
        }

        public void Draw(SpriteBatch batch, Camera camera)
        {
            int tileMapWidth = Width;
            int tileMapHeight = Height;

            batch.Begin(SpriteSortMode.Texture,
                BlendState.NonPremultiplied,
                SamplerState.PointWrap,
                DepthStencilState.Default,
                RasterizerState.CullNone,
                null, camera.TransformMatrix);

            for (int x = 0; x < tileMapWidth; x++)
            {
                for (int y = 0; y < tileMapHeight; y++)
                {
                    int tileIndex = _map[y, x];

                    if (tileIndex != -1)
                    {
                        Texture2D texture = _tileTextures[tileIndex];

                        batch.Draw(
                            texture,
                            new Rectangle(
                                (x * Engine.TileWidth),
                                (y * Engine.TileHeight),
                                Engine.TileWidth,
                                Engine.TileHeight),
                            new Color(new Vector4(1f, 1f, 1f, alpha )));
                    }
                }
            }

            batch.End();

        }

        static public int TileWidth
        {
            get { return Engine.TileWidth; }
        }

        static public int TileHeight
        {
            get { return Engine.TileHeight; }
        }

        public float Alpha
        {
            get { return alpha; }
            set { alpha = MathHelper.Clamp(value, 0f, 1f); }
        }

        public int WidthInPixels
        {
            get
            {
                return Width * Engine.TileWidth;
            }
        }

        public int HeightInPixels
        {
            get
            {
                return Height * Engine.TileHeight;
            }
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
