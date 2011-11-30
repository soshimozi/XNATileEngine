using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TileEngine;
using EventBroker;

namespace TileGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private TriggerSystem npcTriggers;
        private AnimatedSprite playerSprite;
        private Dictionary<string, Script> scripts = new Dictionary<string, Script>();

        private readonly List<AnimatedSprite> renderList = new List<AnimatedSprite>();
        private readonly Dictionary<int, NPC> npcs = new Dictionary<int, NPC>();
        private readonly TileMap tileMap = new TileMap();
        private readonly Camera _camera = new Camera();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            npcTriggers = new TriggerSystem(this);
        }

        Dialog dialog;
        private NPC npc;

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            dialog = new Dialog(this, Content);
            Components.Add(dialog);
            dialog.Visible = false;

            base.Initialize();

            // TODO: Move into content loader
            // <Player>
            //   <Texture Name="blah" />
            //   <FrameHeight>32></FrameHeight>
            //   <FrameWidth>32</FrameWidth>
            //   <FrameAnimations>
            //      <FrameAnimation Key="Down" FrameCount="3" FrameCount OffsetX="0" OffsetY="0" />
            //      <FrameAnimation Key="Left" FrameCount="3" FrameCount OffsetX="96" OffsetY="0" />
            //      <FrameAnimation Key="Down" FrameCount="3" FrameCount OffsetX="192" OffsetY="0" />
            //      <FrameAnimation Key="Down" FrameCount="3" FrameCount OffsetX="288" OffsetY="0" />
            //   </FrameAnimations>
            //  </Animation>  
            FrameAnimation down = new FrameAnimation(3, 32, 32, 0, 0);
            down.FramesPerSecond = 10;
            playerSprite.Animations.Add("Down", down);

            FrameAnimation left = new FrameAnimation(3, 32, 32, 96, 0);
            left.FramesPerSecond = 10;
            playerSprite.Animations.Add("Left", left);

            FrameAnimation right = new FrameAnimation(3, 32, 32, 192, 0);
            right.FramesPerSecond = 10;
            playerSprite.Animations.Add("Right", right);

            FrameAnimation up = new FrameAnimation(3, 32, 32, 288, 0);
            up.FramesPerSecond = 10;
            playerSprite.Animations.Add("Up", up);

            Random rand = new Random();
            foreach (NPC s in npcs.Values)
            {
                s.Animations.Add("Down", (FrameAnimation)down.Clone());
                s.Animations.Add("Up", (FrameAnimation)up.Clone());
                s.Animations.Add("Left", (FrameAnimation)left.Clone());
                s.Animations.Add("Right", (FrameAnimation)right.Clone());

                int animation = rand.Next(0, 3);

                switch (animation)
                {
                    case 0:
                        s.CurrentAnimationName = "Up";
                        break;
                    case 1:
                        s.CurrentAnimationName = "Down";
                        break;

                    case 2:
                        s.CurrentAnimationName = "Left";
                        break;

                    case 3:
                        s.CurrentAnimationName = "Right";
                        break;
                }

                s.Position = new Vector2(
                    rand.Next(400),
                    rand.Next(400));
            }
        
            
            playerSprite.CurrentAnimationName = "Down";

            renderList.Add(playerSprite);
            renderList.AddRange(npcs.Values);

            // TODO: add triggers to a resource
            // <NPC>
            // <blah></blah>
            // <more></more>
            // <blah></blah>
            // <Triggers>
            //   <Trigger Type="DialogTrigger">
            //      <CreationParameters>Quest1</CreationParameters>
            //      <Regions>
            //         <Region Type="Circular" /> <!-- default radius is collisionradius * 2, specify a different one if desired
            //      </Regions>
            //   </Trigger>
            // </Triggers>
            //
            Trigger dialogTrigger = new DialogTrigger(npc, "Quest1");
            dialogTrigger.AddTriggerRegion(new ProximityRegion(npc, npc.CollisionRadius * 2)); //CircularTriggerRegion(npc.Position, npc.CollisionRadius * 2));
            npcTriggers.AddTrigger(dialogTrigger);

            npc.Target = playerSprite;

            EventService.Subscribe(
            EventNames.RemoveConversation,
            new EventHandler<GlobalEventEventArgs>(
                (sender, e) =>
                {
                    RemoveConversation(e.Message);
                }
             ));

            EventService.Subscribe(
                EventNames.EndConversationEvent,
                new EventHandler<GlobalEventEventArgs>(
                    (sender, e) =>
                    {
                        dialog.Visible = false;
                    }
                 ));

            EventService.Subscribe(
                 EventNames.StartConversationEvent, 
                new EventHandler<GlobalEventEventArgs>(
                    (sender, e) => 
                    {
                        StartConversation(e.Message);
                    }
                )
            );
        }

        private void RemoveConversation(string name)
        {
            scripts["NPC1"].RemoveConversation(name);
        }

        private void StartConversation(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                string[] parms = message.Split(':');

                // first param is name
                string conversationName = parms[0];
                int? id = SafeCast.ToInt32(parms[1]);

                if (!string.IsNullOrEmpty(conversationName) && id.HasValue && npcs.ContainsKey(id.Value) && scripts["NPC1"].HasConversation(conversationName))
                {
                    dialog.Conversation = scripts["NPC1"][conversationName];
                    dialog.Npc = npcs[id.Value];

                    dialog.ResetDialog();
                    dialog.Visible = true;
                }
            }
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            tileMap.Layers.Add(TileLayer.FromFile(Content, "Layouts/Layer1.layer"));
            tileMap.Layers.Add(TileLayer.FromFile(Content, "Layouts/Layer3.layer")); 
            tileMap.CollisionLayer = CollisionLayer.FromFile("Content", "Layouts/Collision.layer");

            scripts.Add("NPC1", Content.Load<Script>("Scripts/NPC1"));

            playerSprite = new AnimatedSprite(Content.Load<Texture2D>("Sprites/image (1)_strip"), 0);
            playerSprite.OriginOffset = new Vector2(16, 30);

            EntityManager.Instance.RegisterEntity(playerSprite);
            npc = new NPC(Content.Load<Texture2D>("Sprites/image (1)_strip"), scripts["NPC1"], 1);
            EntityManager.Instance.RegisterEntity(npc);
            npcs.Add(npc.Id, npc);

            npc = new NPC(Content.Load<Texture2D>("Sprites/image (11)_strip"), null, 2);
            EntityManager.Instance.RegisterEntity(npc);
            npcs.Add(npc.Id, npc);

            npc = new NPC(Content.Load<Texture2D>("Sprites/image (2)_strip"), null, 3);
            EntityManager.Instance.RegisterEntity(npc);
            npcs.Add(npc.Id, npc);

            npc = new NPC(Content.Load<Texture2D>("Sprites/image (7)_strip"), null, 4);
            EntityManager.Instance.RegisterEntity(npc);
            npcs.Add(npc.Id, npc);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            int screenWidth = GraphicsDevice.Viewport.Width;
            int screenHeight = GraphicsDevice.Viewport.Height;

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            InputHelper.Update();

            if (!dialog.Visible)
            {
                Vector2 motion = CheckControlState();

                if (motion != Vector2.Zero)
                {
                    motion.Normalize();

                    motion = CheckTileForCollision(playerSprite, motion);
                    playerSprite.Position += motion * playerSprite.Speed;

                    CheckForUnwalkableTile(playerSprite);

                    UpdateSpriteAnimation(motion);
                    playerSprite.IsAnimating = true;
                }
                else
                {
                    playerSprite.IsAnimating = false;
                }

                npcTriggers.Update(gameTime);

                // check triggers for our npc TODO: add for all npcs
                npcTriggers.CheckTriggers(playerSprite);
            }

            playerSprite.Update(gameTime);
            UpdateNpcs(gameTime);

            playerSprite.ClampToArea(tileMap.GetWidthInPixels(), tileMap.GetHeightInPixels());
            _camera.LockToTarget(playerSprite, screenWidth, screenHeight);

            _camera.ClampToArea(tileMap.GetWidthInPixels() - screenWidth,
                tileMap.GetWidthInPixels() - screenHeight);

            base.Update(gameTime);
        }

        private void UpdateNpcs(GameTime gameTime)
        {
            foreach (NPC s in npcs.Values)
            {
                s.Update(gameTime);

                // check for collision and react accordingly
                if (AnimatedSprite.AreColliding(playerSprite, s))
                {
                    Vector2 d = Vector2.Normalize(s.Position - playerSprite.Position);

                    playerSprite.Position
                        = s.Position - (d * (playerSprite.CollisionRadius + s.CollisionRadius));
                }
            }
        }

        private static Vector2 CheckControlState()
        {
            //KeyboardState keyState = Keyboard.GetState();

            Vector2 motion = Vector2.Zero;
            if (InputHelper.IsKeyDown(Keys.Up) || InputHelper.IsKeyDown(Keys.W))
            {
                motion.Y--;
            }

            if (InputHelper.IsKeyDown(Keys.Down) || InputHelper.IsKeyDown(Keys.S))
            {
                motion.Y++;
            }

            if (InputHelper.IsKeyDown(Keys.Left) || InputHelper.IsKeyDown(Keys.A))
            {
                motion.X--;
            }

            if (InputHelper.IsKeyDown(Keys.Right) || InputHelper.IsKeyDown(Keys.D))
            {
                motion.X++;
            }

            return motion;
        }

        private void UpdateSpriteAnimation(Vector2 motion)
        {
            float motionAngle = (float)Math.Atan2(motion.Y, motion.X);

            if (motionAngle >= -MathHelper.PiOver4 && motionAngle <= MathHelper.PiOver4)
            {
                playerSprite.CurrentAnimationName = "Right";
                motion = new Vector2(1f, 0f);
                playerSprite.Direction = Utility.AngleToVector(0.0f);
            }
            else if (motionAngle >= MathHelper.PiOver4 && motionAngle <= 3f * MathHelper.PiOver4)
            {
                playerSprite.CurrentAnimationName = "Down";
                motion = new Vector2(0f, 1f);
                playerSprite.Direction = Utility.AngleToVector(Utility.DegreesToRadians(90.0f));
            }
            else if (motionAngle <= -MathHelper.PiOver4 && motionAngle >= -3f * MathHelper.PiOver4)
            {
                playerSprite.CurrentAnimationName = "Up";
                motion = new Vector2(0f, -1f);
                playerSprite.Direction = Utility.AngleToVector(Utility.DegreesToRadians(270.0f));
            }
            else
            {
                playerSprite.CurrentAnimationName = "Left";
                motion = new Vector2(-1f, 0f);
                playerSprite.Direction = Utility.AngleToVector(Utility.DegreesToRadians(180.0f));
            }
        }

        private void CheckForUnwalkableTile(AnimatedSprite sprite)
        {
            Point spriteCell = Engine.GetPointFromCell(sprite.Center);

            Point? upLeft = null, up = null, upRight = null,
                left = null, right = null,
                downLeft = null, down = null, downRight = null;

            if (spriteCell.Y > 0)
                up = new Point(spriteCell.X, spriteCell.Y - 1);

            if (spriteCell.Y < tileMap.CollisionLayer.Height - 1)
                down = new Point(spriteCell.X, spriteCell.Y + 1);

            if (spriteCell.X > 0)
                left = new Point(spriteCell.X - 1, spriteCell.Y);

            if (spriteCell.X < tileMap.CollisionLayer.Width - 1)
                right = new Point(spriteCell.X + 1, spriteCell.Y);

            if (spriteCell.X > 0 && spriteCell.Y > 0)
                upLeft = new Point(spriteCell.X - 1, spriteCell.Y - 1);

            if (spriteCell.X < tileMap.CollisionLayer.Width - 1 && spriteCell.Y > 0)
                upRight = new Point(spriteCell.X + 1, spriteCell.Y - 1);

            if (spriteCell.X > 0 && spriteCell.Y < tileMap.CollisionLayer.Height - 1)
                downLeft = new Point(spriteCell.X - 1, spriteCell.Y + 1);

            if (spriteCell.X < tileMap.CollisionLayer.Width - 1 &&
                spriteCell.Y < tileMap.CollisionLayer.Height - 1)
                downRight = new Point(spriteCell.X + 1, spriteCell.Y + 1);

            if (up != null && tileMap.CollisionLayer.GetCellIndex(up.Value) == 1)
            {
                Rectangle cellRect = Engine.CreateRectangleForCell(up.Value);
                Rectangle spriteRect = sprite.Bounds;

                if (cellRect.Intersects(spriteRect))
                {
                    sprite.Position.Y = spriteCell.Y * Engine.TileHeight;    
                }
            }

            if (left != null && tileMap.CollisionLayer.GetCellIndex(left.Value) == 1)
            {
                Rectangle cellRect = Engine.CreateRectangleForCell(left.Value);
                Rectangle spriteRect = sprite.Bounds;

                if (cellRect.Intersects(spriteRect))
                {
                    sprite.Position.X = spriteCell.X * Engine.TileWidth;
                }

            }

            if (right != null && tileMap.CollisionLayer.GetCellIndex(right.Value) == 1)
            {
                Rectangle cellRect = Engine.CreateRectangleForCell(right.Value);
                Rectangle spriteRect = sprite.Bounds;

                if (cellRect.Intersects(spriteRect))
                {
                    sprite.Position.X = right.Value.X * Engine.TileWidth - sprite.Bounds.Width;
                }

            }

            if (down != null && tileMap.CollisionLayer.GetCellIndex(down.Value) == 1)
            {
                Rectangle cellRect = Engine.CreateRectangleForCell(down.Value);
                Rectangle spriteRect = sprite.Bounds;

                if (cellRect.Intersects(spriteRect))
                {
                    sprite.Position.Y = down.Value.Y * Engine.TileHeight - sprite.Bounds.Height;
                }
            }
        }

        private Vector2 CheckTileForCollision(AnimatedSprite sprite, Vector2 motion)
        {
            Point cell = Engine.GetPointFromCell(sprite.Origin);

            int colIndex = tileMap.CollisionLayer.GetCellIndex(cell);

            if (colIndex == 2)
                return motion * .2f;

            return motion;
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            tileMap.Draw(spriteBatch, _camera);

            // transform viewport
            spriteBatch.Begin(SpriteSortMode.Texture,
                BlendState.NonPremultiplied,
                SamplerState.PointWrap,
                DepthStencilState.Default,
                RasterizerState.CullNone,
                null, _camera.TransformMatrix);

            foreach (AnimatedSprite s in renderList)
                s.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
