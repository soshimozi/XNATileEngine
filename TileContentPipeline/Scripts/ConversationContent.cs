using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace TileContent
{
    public class ConversationContent
    {
        public string Name;
        public string Text;

        public List<ConversationHandlerContent> Handlers =
            new List<ConversationHandlerContent>();
    }

    [ContentTypeWriter]
    public class ConversationWriter : ContentTypeWriter<ConversationContent>
    {
        protected override void Write(ContentWriter output, ConversationContent value)
        {
            // TODO: write the specified value to the output ContentWriter.
            output.Write(value.Name);
            output.Write(value.Text);

            output.Write(value.Handlers.Count);
            foreach (ConversationHandlerContent c in value.Handlers)
            {
                output.WriteObject(c);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            // TODO: change this to the name of your ContentTypeReader
            // class which will be used to load this data.
            return "TileEngine.ConversationReader, TileEngine";
        }
    }
}
