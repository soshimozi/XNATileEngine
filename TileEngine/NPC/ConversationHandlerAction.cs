using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework.Content;

namespace TileEngine
{
    public class ConversationHandlerAction
    {
        MethodInfo method;
        object[] parameters;

        public ConversationHandlerAction(string methodName, object[] parameters)
        {
            method = typeof(NPC).GetMethod(methodName);
            this.parameters = parameters;
        }

        public void Invoke(NPC npc)
        {
            method.Invoke(npc, parameters);
        }
    }

    public class ConversationHandlerActionReader : ContentTypeReader<ConversationHandlerAction>
    {
        protected override ConversationHandlerAction Read(ContentReader input, ConversationHandlerAction existingInstance)
        {
            string methodName = input.ReadString();
            object[] parameters = input.ReadObject<object[]>();

            return new ConversationHandlerAction(methodName, parameters);
        }
    }
}
