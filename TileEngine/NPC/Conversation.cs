using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace TileEngine
{
    public class Conversation
    {
        private string _name;
        private string _text;

        public readonly List<ConversationHandler> Handlers =
            new List<ConversationHandler>();

        public Conversation(
            string name,
            string text,
            params ConversationHandler[] newHandlers)
        {
            _name = name;
            _text = text;
            foreach (ConversationHandler handler in newHandlers)
            {
                Handlers.Add(handler);
            }
        }
    
        public string Name
        {
            get { return _name; }
        }

        public string Text
        {
            get { return _text; }
        }
    }

    public class ConversationReader : ContentTypeReader<Conversation>
    {
        protected override Conversation Read(ContentReader input, Conversation existingInstance)
        {
            string name = input.ReadString();
            string text = input.ReadString();
            int handlerCount = input.ReadInt32();

            ConversationHandler[] handlers = new ConversationHandler[handlerCount];
            for (int i = 0; i < handlerCount; i++)
            {
                handlers[i] = input.ReadObject<ConversationHandler>();
            }

            return new Conversation(name, text, handlers);
        }
    }
}
