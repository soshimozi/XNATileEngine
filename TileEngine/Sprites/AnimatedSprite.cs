using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

namespace TileEngine
{
    public class AnimatedSprite : BaseGameEntity
    {
        protected string currentAnimation = null;
        private bool animating = true;
        private Texture2D texture;

        public Dictionary<string, FrameAnimation> Animations =
            new Dictionary<string, FrameAnimation>();

        public Vector2 Center
        {
            get
            {
                return Position + new Vector2(
                    CurrentAnimation.CurrentRect.Width / 2,
                    CurrentAnimation.CurrentRect.Height / 2);
            }
        }

        public Rectangle Bounds
        {
            get
            {
                Rectangle rect = CurrentAnimation.CurrentRect;
                rect.X = (int)Position.X;
                rect.Y = (int)Position.Y;

                return rect;
            }
        }

        public bool IsAnimating
        {
            get { return animating; }
            set { animating = value; }
        }

        public FrameAnimation CurrentAnimation
        {
            get
            {
                if (!string.IsNullOrEmpty(currentAnimation))
                    return Animations[currentAnimation];
                else
                    return null;
            }
        
        }

        public string CurrentAnimationName
        {
            get { return currentAnimation; }
            set { if( Animations.ContainsKey(value) ) currentAnimation = value; }
        }


        public AnimatedSprite(Texture2D texture, int id)
            : base(id)
        {
            this.texture = texture;

            Speed = 3.5f;
        }

        public static bool AreColliding(AnimatedSprite a, AnimatedSprite b)
        {
            Vector2 d = b.Position - a.Position;

            return (d.Length() < b.CollisionRadius + a.CollisionRadius);
        }

        public void ClampToArea(int width, int height)
        {
            if (Position.X < 0) Position.X = 0;
            if (Position.Y < 0) Position.Y = 0;

            if (Position.X > width - CurrentAnimation.CurrentRect.Width)
                Position.X = width - CurrentAnimation.CurrentRect.Width;

            if (Position.Y > height - CurrentAnimation.CurrentRect.Height)
                Position.Y = height - CurrentAnimation.CurrentRect.Height;
        }


        public void LockToTarget(int width, int height)
        {
            if (Position.X < 0) Position.X = 0;
            if (Position.Y < 0) Position.Y = 0;

            if (Position.X > width)
                Position.X = width;

            if (Position.Y > height)
                Position.Y = width;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (IsAnimating)
            {
                FrameAnimation animation = CurrentAnimation;

                if (animation == null)
                {
                    if (Animations.Count > 0)
                    {
                        string[] keys = new string[Animations.Count];
                        Animations.Keys.CopyTo(keys, 0);

                        currentAnimation = keys[0];

                        animation = CurrentAnimation;
                    }
                    else
                    {
                        return;
                    }
                }

                animation.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            FrameAnimation animation = CurrentAnimation;

            if (animation != null)
            {
                spriteBatch.Draw(texture, Position, animation.CurrentRect, Color.White);
            }
        }

        public void SetProperty(string propertyName, object value)
        {
            PropertyInfo pi = this.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (pi == null)
                throw new ArgumentException("Specify a valid fieldname.", "fieldName");

            object convertedValue = ConvertValue(value, pi.PropertyType);

            pi.SetValue(this, convertedValue, null);
        }

        private object ConvertValue(object value, Type targetType)
        {
            object converted = null;
            try
            {
                converted = Convert.ChangeType(value, targetType);
            }
            catch
            {
                converted = false;
            }
            return converted;
        }

    }
}
