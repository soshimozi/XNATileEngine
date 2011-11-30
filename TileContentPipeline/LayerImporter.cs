using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.IO;

namespace TileContent
{
    [ContentImporter(".layer", DisplayName = "Tile Layer Importer", DefaultProcessor = "TileLayerProcessor")]
    public class LayerImporter : ContentImporter<string>
    {
        public override string Import(string filename, ContentImporterContext context)
        {
            string text = string.Empty;
            using (FileStream stream = File.OpenRead(filename))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    text = reader.ReadToEnd();
                }
            }

            return text;
        }
    }
}
