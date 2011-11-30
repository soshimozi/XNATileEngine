using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace TileContent
{
    public class ConversationHandlerContent
    {
        public string Caption;
        public List<ConversationHandlerActionContent> Actions = new List<ConversationHandlerActionContent>();
    }

    [ContentTypeWriter]
    public class ConversationHandlerWriter : ContentTypeWriter<ConversationHandlerContent>
    {
        protected override void Write(ContentWriter output, ConversationHandlerContent value)
        {
            // TODO: write the specified value to the output ContentWriter.
            output.Write(value.Caption);
            output.Write(value.Actions.Count);
            foreach (ConversationHandlerActionContent a in value.Actions)
                output.WriteObject(a);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            // TODO: change this to the name of your ContentTypeReader
            // class which will be used to load this data.
            return "TileEngine.ConversationHandlerReader, TileEngine";
        }
    }
}
