using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework.Content;

namespace TileEngine
{
    public class ConversationHandler
    {
        private string _caption;
        ConversationHandlerAction[] actions;

        public ConversationHandler(string caption, params ConversationHandlerAction[] actions)
        {
            _caption = caption;
            this.actions = actions;
        }

        public void Invoke(NPC npc)
        {
            foreach (ConversationHandlerAction action in actions)
                action.Invoke(npc);
        }

        public string Caption
        {
            get { return _caption; }
        }
    }

    public class ConversationHandlerReader : ContentTypeReader<ConversationHandler>
    {
        protected override ConversationHandler Read(ContentReader input, ConversationHandler existingInstance)
        {
            string caption = input.ReadString();
            int actionCount = input.ReadInt32();

            ConversationHandlerAction[] actions = new ConversationHandlerAction[actionCount];
            for (int i = 0; i < actionCount; i++)
            {
                actions[i] = input.ReadObject<ConversationHandlerAction>();
            }

            return new ConversationHandler(caption, actions);
        }
    }

}
