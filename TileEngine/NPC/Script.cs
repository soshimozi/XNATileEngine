using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace TileEngine
{
	public class Script
	{
        private Dictionary<string, Conversation> _conversations
            = new Dictionary<string, Conversation>();

        public Script(params Conversation[] conversations)
        {
            foreach (Conversation c in conversations)
            {
                _conversations.Add(c.Name, c);
            }
        }

        public bool HasConversation(string name)
        {
            return _conversations.ContainsKey(name);
        }
    
        public void RemoveConversation(string name)
        {
            _conversations.Remove(name); 
        }

        public Conversation this[string name]
        {
            get { return _conversations[name]; }
        }
	}

    public class ScriptReader : ContentTypeReader<Script>
    {
        protected override Script Read(ContentReader input, Script existingInstance)
        {
            int conversationCount = input.ReadInt32();

            Conversation[] conversations = new Conversation[conversationCount];
            for (int i = 0; i < conversationCount; i++)
            {
                conversations[i] = input.ReadObject<Conversation>();
            }

            return new Script(conversations);
        }
    }

}
