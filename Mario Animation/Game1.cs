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

        int randomScene;

        List<Texture2D> marioFrames = new List<Texture2D>();
        
        Vector2 goombaPosition = new Vector2(200, 430);
        int frame_count_goomba = 0;
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

            // TODO: Add your initialization logic here
            screen = Screen.Intro;

            randomScene = randNum.Next(0,2);
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
                Exit();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            time += dt;
            stateTimer += dt;


            if (randomScene == 1)
            {


                //Screen
                //Intro
                if (screen == Screen.Intro && stateTimer > 1.5f)
                {
                    stateTimer = 0f;

                    if (randomScene == 0)
                    {
                        screen = Screen.MarioCoin;
                    }
                    else
                    {
                        screen = Screen.MarioGoomba;
                    }

                    frame_count_mario = 2;
                }

                //End
                if ((screen == Screen.MarioCoin || screen == Screen.MarioGoomba)
                    && stateTimer > 2.5f)
                {
                    screen = Screen.End;

                    frame_count_mario = 0;
                    frame_count_goomba = 0;
                }

                //Goomba
                if (screen == Screen.MarioGoomba)
                {
                    // Mario walks toward Goomba
                    marioPosition.X += 120f * dt;

                    // Goomba walks toward Mario
                    goombaPosition.X -= 60f * dt;

                    // Animate Goomba
                    if (time > frameSpeed)
                    {
                        frame_count_goomba++;

                        if (frame_count_goomba > 1)
                            frame_count_goomba = 0;
                    }
                }
                //Reset Screens
                if (randomScene == 0)
                {
                    screen = Screen.MarioCoin;

                    marioPosition = new Vector2(100, 430);
                }
                else
                {
                    screen = Screen.MarioGoomba;

                    marioPosition = new Vector2(100, 430);
                    goombaPosition = new Vector2(600, 430);
                }



                //Movement
                if (screen == Screen.MarioCoin)
                {
                    //Walking
                    if (stateTimer < 1.8f)
                    {
                        marioPosition.X += 150f * dt;
                    }

                    //Jump
                    if (stateTimer > 1.8f && !hasJumped)
                    {
                        velocityY = -250f;
                        isJumping = true;
                        hasJumped = true;
                    }

                    //Gravity
                    velocityY += 500f * dt;
                    marioPosition.Y += velocityY * dt;

                    //Floor collision
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

                    if (screen == Screen.MarioCoin)
                    {
                        if (stateTimer < 1.8f)
                        {
                            frame_count_mario++;
                            if (frame_count_mario > 4)
                                frame_count_mario = 2;
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
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            // Background
            _spriteBatch.Draw(backgroundTexture, backgroundRect, Color.White);

            //Drawing Mario
            Vector2 position = marioPosition;
            float scale = 3f;

            _spriteBatch.Draw(marioFrames[frame_count_mario], position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
