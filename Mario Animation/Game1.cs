using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
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

        bool musicStarted = false;
        bool isJumping = false;
        bool hasJumped = false;
        bool marioDefeated = false;
        bool coinVisible = false;
        float coinTimer = 0f;

        //Goomba
        Vector2 goombaPosition = new Vector2(600, 500);

        //Background
        Rectangle backgroundRect;
        Texture2D backgroundTexture;

        //Title
        Rectangle titleRect;
        Texture2D titleTexture;

        //Game Over
        Rectangle gameOverRect;
        Texture2D gameOverTexture;

        //Coin 
        Texture2D coin;

        //Mario Frames
        List<Texture2D> marioFrames = new List<Texture2D>();

        //Small Mario Frames
        List<Texture2D> smallMarioFrames = new List<Texture2D>();

        //Goomba Frames
        List<Texture2D> goombaFrames = new List<Texture2D>();

        //Thumb
        Texture2D marioThumbUp;

        //Font
        SpriteFont eightBitFont;
        //Audio
        Song backgroundMusic;

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

            //Small Mario
            smallMarioFrames.Add(Content.Load<Texture2D>("small_mario_idle"));
            smallMarioFrames.Add(Content.Load<Texture2D>("small_mario_walk1"));
            smallMarioFrames.Add(Content.Load<Texture2D>("small_mario_walk2"));
            smallMarioFrames.Add(Content.Load<Texture2D>("small_mario_walk3"));
            smallMarioFrames.Add(Content.Load<Texture2D>("small_mario_death"));

            //Coin
            coin = Content.Load<Texture2D>("marioCoin");

            // Background
            backgroundTexture = Content.Load<Texture2D>("mario_background");
            backgroundRect = new Rectangle(0, 0, 800, 600);

            //Title
            titleTexture = Content.Load<Texture2D>("Super_Mario_Bros_Title");
            titleRect = new Rectangle(0, 0, 800, 600);

            //Game Over
            gameOverTexture = Content.Load<Texture2D>("SMBGameOver");
            gameOverRect = new Rectangle(0, 0, 800, 600);

            //Thumb
            marioThumbUp = Content.Load<Texture2D>("marioThumbUp");

            //Music
            backgroundMusic = Content.Load<Song>("Theme");
            deathMusic = Content.Load<Song>("Death");

            //Font
            eightBitFont = Content.Load<SpriteFont>("File");
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
                KeyboardState keyboard = Keyboard.GetState();

                if (keyboard.IsKeyDown(Keys.Enter))
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

                    marioPosition = new Vector2(232, 430);

                    goombaPosition = new Vector2(600, 475);

                    velocityY = 0f;

                    hasJumped = false;

                    frame_count_mario = 2;
                }
            }

            // End
            if ((screen == Screen.MarioCoin || screen == Screen.MarioGoomba)
                && stateTimer > 4f)
            {
              
                screen = Screen.End;

                frame_count_mario = 0;

                frame_count_goomba = 0;
            }

            //Mario Coin
            if (screen == Screen.MarioCoin)
            {
                if (!musicStarted)
                {
                    MediaPlayer.Play(backgroundMusic);
                    MediaPlayer.IsRepeating = true;
                    musicStarted = true;
                }

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

                    coinVisible = true;
                    coinTimer = 0.80f; // 1 second
                }

                //Coin Timer
                if (coinVisible)
                {
                    coinTimer -= dt;

                    if (coinTimer <= 0f)
                    {
                        coinVisible = false;
                    }
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

            if (screen == Screen.MarioGoomba)
            {
                // Only move if Mario is alive
                if (!marioDefeated)
                {
                    // Mario walks right
                    marioPosition.X += 120f * dt;

                    // Goomba walks left
                    goombaPosition.X -= 60f * dt;
                }

                // Gravity
                velocityY += 500f * dt;

                marioPosition.Y += velocityY * dt;

                // Floor collision only before death
                if (marioPosition.Y >= 430 && !marioDefeated)
                {
                    marioPosition.Y = 430;

                    velocityY = 0f;
                }

                // Mario gets defeated when touching Goomba
                if (marioPosition.X >= goombaPosition.X - 30 && !marioDefeated)
                {
                    marioDefeated = true;

                    frame_count_mario = 4;

                    velocityY = -250f;
                }

                // Goomba animation freezes after death
                if (!marioDefeated)
                {
                    if (time > frameSpeed)
                    {
                        frame_count_goomba++;

                        if (frame_count_goomba > 1)
                        {
                            frame_count_goomba = 0;
                        }
                    }
                }
            }

            //Mario Animation
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
                        if (isJumping)
                        {
                            frame_count_mario = 1; // jump frame
                        }
                        else
                        {
                            frame_count_mario = 0; // idle frame
                        }
                    }
                }

                else if (screen == Screen.MarioGoomba)
                {
                    // Walking animation
                    if (!hasJumped)
                    {
                        frame_count_mario++;

                        if (frame_count_mario > 3)
                        {
                            frame_count_mario = 1;
                        }
                    }

                    // Death frame
                    if (marioDefeated)
                    {
                        frame_count_mario = 4;
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

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            if (screen == Screen.Intro)
            {
                // Title
                _spriteBatch.Draw(titleTexture, titleRect, Color.White);
            }
            //Background
            if (screen == Screen.MarioCoin || screen == Screen.MarioGoomba)
            {
                _spriteBatch.Draw(backgroundTexture, backgroundRect, Color.White);
            }
                

            if (screen == Screen.End)
            {
                // Game Over
              
                _spriteBatch.Draw(gameOverTexture, gameOverRect, Color.White);
            }
            // Big Mario
            if (screen == Screen.MarioCoin)
            {
                // Normal Mario before coin ends
                if (coinVisible)
                {
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

                    // Coin
                    _spriteBatch.Draw(
                        coin,
                        new Rectangle(510, 320, 30, 30),
                        Color.White);
                }

                // Thumb up Mario after getting coin
                else if (hasJumped)
                {
                    _spriteBatch.Draw(
                        marioThumbUp,
                        marioPosition,
                        null,
                        Color.White,
                        0f,
                        Vector2.Zero,
                        3f,
                        SpriteEffects.None,
                        0f);
                }

                // Before jump
                else
                {
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
                }
            }

            // Small Mario
            if (screen == Screen.MarioGoomba)
            {
                Vector2 smallMarioDrawPosition =
                    new Vector2(marioPosition.X, marioPosition.Y + 48);

                _spriteBatch.Draw(
                    smallMarioFrames[frame_count_mario],
                    smallMarioDrawPosition,
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    3f,
                    SpriteEffects.None,
                    0f);
            }

            //Goomba
            if (screen == Screen.MarioGoomba)
            {
                _spriteBatch.Draw(goombaFrames[frame_count_goomba], goombaPosition, null, Color.White, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 0f);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}