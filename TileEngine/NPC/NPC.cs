using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using EventBroker;

namespace TileEngine
{
    public class NPC : AnimatedSprite, ITalkable
    {
        private Script _script;
        private float reactionRadius = 12;

        public NPC(Texture2D texture, Script script, int id) 
            : base(texture, id)
        {
            _script = script;
            Direction = Vector2.Zero;
        }

        public AnimatedSprite Target { get; set; }

        public override void Update(GameTime gameTime)
        {
            if (Target != null && Following)
            {
                float targetFacing = Utility.RadiansToDegrees(Utility.VectorToAngle(Target.Direction));

                // now subract 180 from that
                targetFacing = Utility.NormalizeAngle(targetFacing - 180f);

                // get point 180 degrees behind target direction
                Vector2 targetPosition = Utility.AngleToVector(Utility.DegreesToRadians(targetFacing)) * 40f; // TODO: replace with proper bounding radius or whatever
                targetPosition += Target.Position;

                Speed = 3.5f;


                if (Math.Abs(Position.X - Target.Position.X) > 5)
                {
                    if (Position.X > Target.Position.X)
                    {
                        // target is to the left of us
                        CurrentAnimationName = "Left";
                    }
                    else
                    {
                        // target is to the right of us
                        CurrentAnimationName = "Right";
                    }
                }
                else
                {
                    if (Position.Y > Target.Position.Y)
                    {
                        CurrentAnimationName = "Up";
                    }
                    else
                    {
                        CurrentAnimationName = "Down";
                    }

                }

                Direction = GetSteeringVector(targetPosition, 40f);
                if (Direction != Vector2.Zero && (Math.Abs(Direction.X) > .001 || Math.Abs(Direction.Y) > .001))
                {
                    Position += Direction * Speed;
                    IsAnimating = true;
                }
                else
                {
                    CurrentAnimationName = Target.CurrentAnimationName;
                    IsAnimating = false;
                }
            }

            base.Update(gameTime);
        }


        private Vector2 GetSteeringVector(Vector2 targetPosition, float minDistance)
        {
            Vector2 steering = SteeringBehaviors.Arrive(targetPosition, Position, Speed, Direction, 10.0f);

            Vector2 toTarget = Target.Position - Position;
            float distanceToTarget = Math.Abs(toTarget.Length());
            if (distanceToTarget < minDistance)
            {
                return Vector2.Zero;
            }

            return steering;

        }

        // this algorithm is very dumb
        // it basically calculates the sprites position
        // based on the targets center minus the collision radius of the target and the sprite
        private void FollowTarget()
        {
            Position = (Target.Center) + new Vector2(Target.CollisionRadius + CollisionRadius, 0);
            CurrentAnimationName = Target.CurrentAnimationName;
        }


        public void RemoveConversation(string conversationName)
        {
            EventService.FireEvent(
                EventNames.RemoveConversation,
                this, new GlobalEventEventArgs(conversationName));
        }

        public void StartConversation(string conversationName)
        {
            EventService.FireEvent(
                EventNames.StartConversationEvent,
                this, new GlobalEventEventArgs(string.Format("{0}:{1}", conversationName, Id)));
        }

        public void EndConversation()
        {
            EventService.FireEvent(
                EventNames.EndConversationEvent,
                this, new GlobalEventEventArgs(string.Empty));
        }

        public bool InSpeakingRange(AnimatedSprite sprite)
        {
            Vector2 d = Origin - sprite.Origin;
            return (d.Length() < ReactionRadius);
        }

        public float ReactionRadius
        {
            get { return reactionRadius; }
            set { reactionRadius = (float)Math.Max(value, CollisionRadius * 2); }
        }

        public bool Following
        {
            get;
            set;
        }

    }
}
