using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TileEngine
{
    public class Camera
    {
        public Camera()
        {
            Position = Vector2.Zero;
            Speed = 5;
        }

        public Vector2 Position;

        private float _speed;
        public float Speed
        {
            get { return _speed; }

            /* clamp speed to 1.0 minimum */
            set { _speed = (float)Math.Max(value, 1f); }
        }

        public Matrix TransformMatrix
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3(-Position, 0f));
            }
        }

        public void LockToTarget(AnimatedSprite sprite, int screenWidth, int screenHeight)
        {
            Position.X = sprite.Position.X + (sprite.CurrentAnimation.CurrentRect.Width - (screenWidth / 2));
            Position.Y = sprite.Position.Y + (sprite.CurrentAnimation.CurrentRect.Height - (screenHeight / 2));
        }

        public void ClampToArea(int width, int height)
        {
            if (Position.X < 0) Position.X = 0;
            if (Position.Y < 0) Position.Y = 0;

            if (Position.X > width)
                Position.X = width;

            if (Position.Y > height)
                Position.Y = height;
        }

        //public void Update()
        //{
        //    KeyboardState keyState = Keyboard.GetState();

        //    Vector2 motion = Vector2.Zero;
        //    if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W))
        //    {
        //        motion.Y--;
        //    }

        //    if (keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S))
        //    {
        //        motion.Y++;
        //    }

        //    if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.A))
        //    {
        //        motion.X--;
        //    }

        //    if (keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D))
        //    {
        //        motion.X++;
        //    }

        //    if (motion != Vector2.Zero)
        //    {
        //        motion.Normalize();
        //        Position += motion * Speed;
        //    }
        //}

    }
}
