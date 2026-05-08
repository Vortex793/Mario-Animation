using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Mario_Animation
{
    enum Screen
    {
        Intro,
        MarioCoin,
        MarioGoomba,
        End
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Rectangle window;

        Random randNum = new Random();

        int frame_count_mario = 0;
        int frame_count_goomba = 0;

        float time = 0f;
        float frameSpeed = 0.15f;

        Screen screen;
        float stateTimer = 0f;

        int randomScene;

        // Mario
        Vector2 marioPosition = new Vector2(100, 430);

        float velocityY = 0f;

        bool isJumping = false;
        bool hasJumped = false;

        // Goomba
        Vector2 goombaPosition = new Vector2(600, 430);

        // Background
        Rectangle backgroundRect;
        Texture2D backgroundTexture;

        // Mario Frames
        List<Texture2D> marioFrames = new List<Texture2D>();

        // Goomba Frames
        List<Texture2D> goombaFrames = new List<Texture2D>();

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

            screen = Screen.Intro;

            randomScene = randNum.Next(1, 3);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Mario
            marioFrames.Add(Content.Load<Texture2D>("mario_idle"));
            marioFrames.Add(Content.Load<Texture2D>("mario_jump"));
            marioFrames.Add(Content.Load<Texture2D>("mario_walk1"));
            marioFrames.Add(Content.Load<Texture2D>("mario_walk2"));
            marioFrames.Add(Content.Load<Texture2D>("mario_walk3"));

            // Goomba
            goombaFrames.Add(Content.Load<Texture2D>("Goomba_walk1"));
            goombaFrames.Add(Content.Load<Texture2D>("Goomba_walk2"));

            // Background
            backgroundTexture = Content.Load<Texture2D>("mario_background");

            backgroundRect = new Rectangle(0, 0, 800, 600);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            time += dt;
            stateTimer += dt;

            // Intro
            if (screen == Screen.Intro && stateTimer > 1.5f)
            {
                stateTimer = 0f;

                if (randomScene == 1)
                {
                    screen = Screen.MarioCoin;
                }
                else if (randomScene == 2)
                {
                    screen = Screen.MarioGoomba;
                }

                marioPosition = new Vector2(100, 430);

                goombaPosition = new Vector2(600, 430);

                velocityY = 0f;

                hasJumped = false;

                frame_count_mario = 2;
            }

            // End
            if ((screen == Screen.MarioCoin || screen == Screen.MarioGoomba)
                && stateTimer > 4f)
            {
                screen = Screen.End;

                frame_count_mario = 0;

                frame_count_goomba = 0;
            }

            // =========================
            // MARIO COIN SCENE
            // =========================
            if (screen == Screen.MarioCoin)
            {
                // Walking
                if (stateTimer < 1.8f)
                {
                    marioPosition.X += 150f * dt;
                }

                // Jump
                if (stateTimer > 1.8f && !hasJumped)
                {
                    velocityY = -250f;

                    isJumping = true;

                    hasJumped = true;
                }

                // Gravity
                velocityY += 500f * dt;

                marioPosition.Y += velocityY * dt;

                // Floor Collision
                if (marioPosition.Y >= 430)
                {
                    marioPosition.Y = 430;

                    velocityY = 0f;

                    isJumping = false;
                }
            }

            // =========================
            // GOOMBA SCENE
            // =========================
            if (screen == Screen.MarioGoomba)
            {
                // Mario walks right
                marioPosition.X += 120f * dt;

                // Goomba walks left
                goombaPosition.X -= 60f * dt;

                // Mario jumps too early
                if (marioPosition.X >= 320 && !hasJumped)
                {
                    velocityY = -300f;

                    hasJumped = true;
                }

                // Gravity
                velocityY += 500f * dt;

                marioPosition.Y += velocityY * dt;

                // Floor collision
                if (marioPosition.Y >= 430)
                {
                    marioPosition.Y = 430;

                    velocityY = 0f;
                }

                // Mario gets defeated
                if (hasJumped
                    && marioPosition.Y >= 430
                    && marioPosition.X >= goombaPosition.X - 30)
                {
                    marioPosition.X -= 180f * dt;

                    frame_count_mario = 0;
                }

                // Goomba animation
                if (time > frameSpeed)
                {
                    frame_count_goomba++;

                    if (frame_count_goomba > 1)
                    {
                        frame_count_goomba = 0;
                    }
                }
            }

            // =========================
            // MARIO ANIMATION
            // =========================
            if (time > frameSpeed)
            {
                time = 0f;

                if (screen == Screen.MarioCoin)
                {
                    if (stateTimer < 1.8f)
                    {
                        frame_count_mario++;

                        if (frame_count_mario > 4)
                        {
                            frame_count_mario = 2;
                        }
                    }
                    else
                    {
                        frame_count_mario = 1;
                    }
                }

                else if (screen == Screen.MarioGoomba)
                {
                    if (!hasJumped)
                    {
                        frame_count_mario++;

                        if (frame_count_mario > 4)
                        {
                            frame_count_mario = 2;
                        }
                    }
                    else
                    {
                        frame_count_mario = 1;
                    }
                }

                else
                {
                    frame_count_mario = 0;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp);

            // Background
            _spriteBatch.Draw(backgroundTexture, backgroundRect, Color.White);

            // Mario
            _spriteBatch.Draw(
                marioFrames[frame_count_mario],
                marioPosition,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                3f,
                SpriteEffects.None,
                0f);

            // Goomba
            if (screen == Screen.MarioGoomba)
            {
                _spriteBatch.Draw(
                    goombaFrames[frame_count_goomba],
                    goombaPosition,
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    3f,
                    SpriteEffects.None,
                    0f);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}