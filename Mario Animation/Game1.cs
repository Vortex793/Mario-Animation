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

        Vector2 marioPosition = new Vector2(100, 430);
        float velocityY = 0f;
        bool isJumping = false;
        bool hasJumped = false;

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
       || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            time += dt;
            stateTimer += dt;

            // screen flow
            if (screen == Screen.Intro && stateTimer > 1.5f)
            {
                screen = Screen.Mario;
                stateTimer = 0f;
                frame = 2;
            }
            else if (screen == Screen.Mario && stateTimer > 2.5f)
            {
                screen = Screen.End;
                frame = 0;
            }

            // ======================
            // MARIO MOVEMENT LOGIC
            // ======================
            if (screen == Screen.Mario)
            {
                // walk forward only before jump
                if (stateTimer < 1.8f)
                {
                    marioPosition.X += 150f * dt;
                }

                // jump once
                if (stateTimer > 1.8f && !hasJumped)
                {
                    velocityY = -250f;
                    isJumping = true;
                    hasJumped = true;
                }

                // gravity
                velocityY += 500f * dt;
                marioPosition.Y += velocityY * dt;

                // floor collision
                if (marioPosition.Y >= 430)
                {
                    marioPosition.Y = 430;
                    velocityY = 0f;
                    isJumping = false;
                }
            }

            //Animations
            if (time > frameSpeed)
            {
                time = 0f;

                if (screen == Screen.Mario)
                {
                    if (stateTimer < 1.8f)
                    {
                        frame++;
                        if (frame > 4)
                            frame = 2;
                    }
                    else
                    {
                        frame = 1; // jump frame
                    }
                }
                else
                {
                    frame = 0;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            // background
            _spriteBatch.Draw(backgroundTexture, backgroundRect, Color.White);

            // draw mario ONLY (no logic here)
            Vector2 position = marioPosition;
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
