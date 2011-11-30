using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.Xml;
using System.Diagnostics;

namespace TileContent
{
    [ContentProcessor(DisplayName = "NPC Script Processor")]
    public class ScriptProcessor : ContentProcessor<XmlDocument, ScriptContent>
    {
        public override ScriptContent Process(XmlDocument input, ContentProcessorContext context)
        {
            ScriptContent script = new ScriptContent();

            XmlNodeList conversationNodes = input.GetElementsByTagName("Conversation");

            foreach (XmlNode node in conversationNodes)
            {
                ConversationContent c = new ConversationContent();

                c.Name = node.Attributes["Name"].Value;
                c.Text = node.FirstChild.InnerText;

                foreach (XmlNode handlerNode in node.LastChild.ChildNodes)
                {
                    ConversationHandlerContent h = new ConversationHandlerContent();
                    h.Caption = handlerNode.Attributes["Caption"].Value;

                    string actionString = handlerNode.Attributes["Action"].Value;

                    string[] methods = actionString.Split(';');

                    foreach (string m in methods)
                    {
                        string trimmedMethodName = m.Trim();

                        ConversationHandlerActionContent action =
                            new ConversationHandlerActionContent();

                        if (trimmedMethodName.Contains(":"))
                        {
                            string[] actionSplit = trimmedMethodName.Split(':');
                            
                            action.MethodName = actionSplit[0];
                            action.ActionParameters = (object[])actionSplit[1].Split(',');
                        }
                        else
                        {
                            action.MethodName = trimmedMethodName;
                            action.ActionParameters = null;
                        }

                        h.Actions.Add(action);
                    }

                    c.Handlers.Add(h);
                }

                script.Conversations.Add(c);
            }

            return script;
        }
    }
}