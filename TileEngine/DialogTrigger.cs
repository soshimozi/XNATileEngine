using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using EventBroker;

namespace TileEngine
{
    public class DialogTrigger : Trigger
    {
        private string _conversationName;
        private ITalkable _owner;
        private bool _triggered = false;

        public DialogTrigger(ITalkable owner, string conversationName)
        {
            _conversationName = conversationName;
            _owner = owner;
        }

        public override void Try(BaseGameEntity entity)
        {
            // check if we are within the trigger distance, and if so
            // tell event broker that we need to display a dialog 
            // for this npc, or npc show dialog?
            if (IsTouchingTrigger(entity.Position, entity.CollisionRadius))
            {
                // only trigger if this is the first time npc entered region
                if (!_triggered)
                {
                    _triggered = true;
                    _owner.StartConversation(_conversationName);
                }
            }
            else
            {
                _triggered = false;
            }
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}
