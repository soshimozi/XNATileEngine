using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace TileEngine
{
    public class TriggerSystem : DrawableGameComponent
    {
        private List<Trigger> _triggers = new List<Trigger>();

        public TriggerSystem(Game game)
            : base(game)
        {
        }

        public List<Trigger> Triggers
        {
            get { return _triggers; }
        }
    

        public void AddTrigger(Trigger trigger)
        {
            _triggers.Add(trigger);
        }

        public void CheckTriggers(BaseGameEntity npc)
        {
            foreach (Trigger trigger in _triggers)
            {
                trigger.Try(npc);
            }
        }

        public override void Update(GameTime gameTime)
        {
            List<Trigger> removeList = new List<Trigger>();
            foreach (Trigger trigger in _triggers)
            {
                if (trigger.RemoveFromGame)
                {
                    removeList.Add(trigger);
                }
            }

            // now remove the list
            foreach (Trigger trigger in removeList)
            {
                _triggers.Remove(trigger);
            }
        
            // now iterate remaining and call update
            foreach (Trigger trigger in _triggers)
            {
                trigger.Update(gameTime);
            }

        }

        public override void Draw(GameTime gameTime)
        {
            //foreach (NPCTrigger trigger in _triggers)
            //{
            //    // not all triggers draw, but call virtual func anyway
            //    trigger.Draw();
            //}
        }
    }
}
