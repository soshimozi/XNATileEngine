using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace TileContent
{
	public class ScriptContent
	{
        public List<ConversationContent> Conversations
            = new List<ConversationContent>();
	}

    [ContentTypeWriter]
    public class ScriptWriter : ContentTypeWriter<ScriptContent>
    {
        protected override void Write(ContentWriter output, ScriptContent value)
        {
            // TODO: write the specified value to the output ContentWriter.
            output.Write(value.Conversations.Count);
            foreach (ConversationContent c in value.Conversations)
            {
                output.WriteObject(c);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            // TODO: change this to the name of your ContentTypeReader
            // class which will be used to load this data.
            return "TileEngine.ScriptReader, TileEngine";
        }
    }
}
