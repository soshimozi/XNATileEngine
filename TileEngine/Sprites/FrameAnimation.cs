using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace TileEngine
{
    public class FrameAnimation : ICloneable
    {
        Rectangle[] frames;
        int currentFrame = 0;

        float frameLength = .5f;
        float timer = 0;

        public float FramesPerSecond
        {
            get
            {
                return 1f / frameLength;
            }

            set
            {
                // don't allow more that 1000 frames per second
                frameLength = (float)Math.Max(1f / value, .001f);
            }

        }

        public Rectangle CurrentRect
        {
            get { return frames[currentFrame]; }
        }

        public int CurrentFrame
        {
            get { return currentFrame; }
            set { currentFrame = (int)MathHelper.Clamp(value, 0, frames.Length - 1); }
        }

        public void Update(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timer >= frameLength)
            {
                timer = 0;
                currentFrame = (currentFrame + 1) % frames.Length;
            }
        }

        public FrameAnimation(
            int numberOfFrames,
            int frameWidth,
            int frameHeight,
            int xOffset,
            int yOffset)
        {
            frames = new Rectangle[numberOfFrames];

            for (int i = 0; i < numberOfFrames; i++)
            {
                Rectangle rectangle 
                    = new Rectangle(
                        xOffset + (i * frameWidth), 
                        yOffset, 
                        frameWidth, frameHeight);


                frames[i] = rectangle;
            }
        }

        private FrameAnimation()
        {
        }

        public object Clone()
        {
            FrameAnimation animation = new FrameAnimation();

            animation.frames = frames;

            animation.frameLength = frameLength;

            return animation;
        }
    }
}
