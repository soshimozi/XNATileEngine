using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace TileEngine
{
    public class Dialog : DrawableGameComponent
    {
        public string Text = string.Empty;

        public Conversation Conversation = null;
        public NPC Npc = null;

        public Rectangle Area = new Rectangle(0, 0, 300, 200);

        private int currentHandler = 0;
        private KeyboardState lastState;

        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;

        private Texture2D background;

        private readonly ContentManager content;

        public Dialog(Game game, ContentManager content)
            : base(game)
        {
            this.content = content;
        }

        public void ResetDialog()
        {
            currentHandler = 0;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = content.Load<SpriteFont>("Fonts/Arial");

            background = new Texture2D(
                GraphicsDevice,
                1,
                1,
                false,
                SurfaceFormat.Color);

            background.SetData<Color>(new Color[] { Color.White });
                
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState newState = Keyboard.GetState();

            if (Conversation != null && Npc != null)
            {
                if (newState.IsKeyDown(Keys.Up) && lastState.IsKeyUp(Keys.Up))
                {
                    currentHandler--;
                    if (currentHandler < 0)
                        currentHandler = Conversation.Handlers.Count - 1;
                }

                if (newState.IsKeyDown(Keys.Down) && lastState.IsKeyUp(Keys.Down))
                {
                    currentHandler = (currentHandler + 1) % Conversation.Handlers.Count;
                }

                if (newState.IsKeyDown(Keys.Space) && lastState.IsKeyUp(Keys.Space))
                {
                    Conversation.Handlers[currentHandler].Invoke(Npc);
                }

            }

            lastState = newState;
        }

        public override void Draw(GameTime gameTime)
        {
            Rectangle dest = new Rectangle(
                (GraphicsDevice.Viewport.Width - Area.Width) / 2,
                (GraphicsDevice.Viewport.Height - Area.Height) / 2,
                Area.Width,
                Area.Height);

            spriteBatch.Begin();

            spriteBatch.Draw(background, dest, new Color(0, 0, 0, 100) );

            spriteBatch.End();

            if (Conversation != null)
            {

                spriteBatch.Begin();

                string fullText = WrapText(Conversation.Text);

                spriteBatch.DrawString(
                    spriteFont,
                    fullText,
                    new Vector2(dest.X, dest.Y),
                    Color.White);

                float lineHeight = spriteFont.MeasureString(" ").Y;

                float startingHeight = spriteFont.MeasureString(fullText).Y + lineHeight;

                for (int i = 0; i < Conversation.Handlers.Count; i++)
                {
                    string caption = Conversation.Handlers[i].Caption;

                    Color color = (i == currentHandler) ? Color.Yellow : Color.White;

                    spriteBatch.DrawString(
                        spriteFont,
                        caption,
                        new Vector2(dest.X, dest.Y + startingHeight + (i * lineHeight)),
                        color);

                }

                spriteBatch.End();
            }
        }

        private string WrapText(string text)
        {
            string[] words = text.Split(' ');

            StringBuilder sb = new StringBuilder();

            float lineWidth = 0f;
            float spaceWidth = spriteFont.MeasureString(" ").X;
            foreach (string word in words)
            {
                Vector2 wordSize = spriteFont.MeasureString(word);

                if (lineWidth + wordSize.X + spaceWidth < Area.Width)
                {
                    sb.Append(word + " ");
                    lineWidth += wordSize.X + spaceWidth;
                }
                else
                {
                    sb.Append("\n" + word + " ");
                    lineWidth = wordSize.X + spaceWidth;
                }
            }

            return sb.ToString();
        }
    }
}
