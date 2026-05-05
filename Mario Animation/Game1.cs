using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Mario_Animation
{
    enum Screen
    {
        Intro,
        Mario,
        End
    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Rectangle window;

        int frame = 0;
        float time = 0f;
        float frameSpeed = 0.15f;
        Screen screen;
        float stateTimer = 0f;

        //Background
        Rectangle backgroundRect;
        Texture2D backgroundTexture;

        List<Texture2D> marioFrames = new List<Texture2D>();

        public Game1()
        {
            
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            window = new Rectangle(0, 0, 800, 600);
            _graphics.PreferredBackBufferWidth = window.Width;
            _graphics.PreferredBackBufferHeight = window.Height;
            _graphics.ApplyChanges();

            // TODO: Add your initialization logic here
            screen = Screen.Intro;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            marioFrames.Add(Content.Load<Texture2D>("mario_idle"));
            marioFrames.Add(Content.Load<Texture2D>("mario_jump"));
            marioFrames.Add(Content.Load<Texture2D>("mario_walk1"));
            marioFrames.Add(Content.Load<Texture2D>("mario_walk2"));
            marioFrames.Add(Content.Load<Texture2D>("mario_walk3"));

            // Background
            backgroundTexture = Content.Load<Texture2D>("mario_background");
            backgroundRect = new Rectangle(0, 0, 800, 600);
            

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            stateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // screen flow
            if (screen == Screen.Intro && stateTimer > 1.5f)
            {
                screen = Screen.Mario;
                stateTimer = 0f;
                frame = 2; // start running (walk1)
            }
            else if (screen == Screen.Mario && stateTimer > 2.5f)
            {
                screen = Screen.End;
                frame = 0; // back to idle
            }

            // animation timing
            if (time > frameSpeed)
            {
                time = 0f;

                if (screen == Screen.Mario)
                {
                    // first part = running
                    if (stateTimer < 1.8f)
                    {
                        frame++;
                        if (frame > 4) // loop walk1–3
                            frame = 2;
                    }
                    else
                    {
                        // jump at the end
                        frame = 1;
                    }
                }
                else if (screen == Screen.Intro || screen == Screen.End)
                {
                    frame = 0; // idle
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            _spriteBatch.Begin(SpriteSortMode.Deferred,BlendState.AlphaBlend,SamplerState.PointClamp);

            Vector2 position = new Vector2(100, 300);
            _spriteBatch.Draw(backgroundTexture, backgroundRect, Color.White);
            if (screen == Screen.Mario)
            {
               
                // move right while running
                position.X += stateTimer * 80f;

                // small jump near the end
                if (stateTimer > 1.8f)
                {
                    position.Y -= 50f;
                }
            }

            float scale = 3f; 

            _spriteBatch.Draw(
                marioFrames[frame],
                position,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f
            );

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
