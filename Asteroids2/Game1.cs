using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Asteroids2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //screen
        int screenWidth = 0;
        int screenHeight = 0;


        Texture2D shipTexture;
        Texture2D asteroidTexture;
        Texture2D gameBackground;
        Texture2D splashBackground;
        Texture2D laserTexture;

        //player variables
        Vector2 playerPosition = new Vector2(400, 400);
        Vector2 playerOffSet = new Vector2(0, 0);
        float playerRotation = 0;
        float playerSpeed = 150.0f;
        float playerTurnSpeed = 6;
        float playerRadius = 0;
        bool playerDeath = false;

        //asteroid variables
        const int numberOfAsteroids = 5;
        Vector2[] asteroidPositions = new Vector2[numberOfAsteroids];
        Vector2[] asteroidOffSets = new Vector2[numberOfAsteroids];
        float[] asteroidRotations = new float[numberOfAsteroids];
        bool[] asteroidDeathStates = new bool[numberOfAsteroids];
        float asteroidSpeed = 150.0f;

        float asteroidRadius = 0;

        //laser variables
        Vector2 laserOffSet = new Vector2(0, 0);
        float laserRotation = 0;
        float laserSpeed = 10.0f;
        float laserRadius = 0;
        const int numberOfLasers = 50;
        Vector2[] laserPositions = new Vector2[numberOfLasers];
        Vector2[] laserVelocity = new Vector2[numberOfLasers];
        bool[] laserHit = new bool[numberOfLasers];
        float laserShootTimer = 0;
        float shootDelay = 0.3f;

        //fps
        int currentFPS = 0;
        int fpsCounter = 0;
        float fpsTimer = 0;

        //game states
        const int STATE_SPLASH = 0;
        const int STATE_GAME = 1;
        const int STATE_GAMEOVER = 2;
        int gameState = STATE_SPLASH;

        //score
        int playerScore = 0;

        SpriteFont candaraFont;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.IsFixedTimeStep = false;
            this.graphics.SynchronizeWithVerticalRetrace = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here           
            ResetGame();

            base.Initialize();
        }

        protected void ResetGame()
        {
            int halfWidth = graphics.GraphicsDevice.Viewport.Width / 2;
            int halfHeight = graphics.GraphicsDevice.Viewport.Height / 2;

            //set player 
            playerPosition = new Vector2(
                graphics.GraphicsDevice.Viewport.Width / 2.0f,
                graphics.GraphicsDevice.Viewport.Height / 2.0f);
            playerDeath = false;
            playerRotation = 0;

            //set asteroids
            asteroidPositions[0] = new Vector2(75, halfHeight);
            asteroidRotations[0] = 4.5f;
            asteroidDeathStates[0] = false;

            asteroidPositions[1] = new Vector2(screenWidth - 75, halfHeight);
            asteroidRotations[1] = 1f;
            asteroidDeathStates[1] = false;

            asteroidPositions[2] = new Vector2(halfWidth, 75);
            asteroidRotations[2] = 0.2f;
            asteroidDeathStates[2] = false;

            asteroidPositions[3] = new Vector2(500, 500);
            asteroidRotations[3] = 0.8f;
            asteroidDeathStates[3] = false;

            asteroidPositions[4] = new Vector2(300, 75);
            asteroidRotations[4] = 1.2f;
            asteroidDeathStates[4] = false;

            //score
            playerScore = 0;

            //screen
            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;

            //bullet reset
            for (int i = 0; i < numberOfLasers; i++)
            {
                laserHit[i] = true;
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

            // TODO: use this.Content to load your game content here
            shipTexture = Content.Load<Texture2D>("ship");
            candaraFont = Content.Load<SpriteFont>("Candara");
            asteroidTexture = Content.Load<Texture2D>("asteroidm");
            gameBackground = Content.Load<Texture2D>("bg");
            laserTexture = Content.Load<Texture2D>("blaser");
            splashBackground = Content.Load<Texture2D>("splash");

            playerOffSet = new Vector2(shipTexture.Width / 2, shipTexture.Height / 2);

            laserOffSet = new Vector2(laserTexture.Width / 2, laserTexture.Height / 2);

            playerRadius = shipTexture.Width / 7;
            asteroidRadius = asteroidTexture.Width / 2;
            laserRadius = laserTexture.Width / 2;

            for (int i = 0; i < numberOfAsteroids; i++)
            {
                asteroidOffSets[i] = new Vector2(asteroidTexture.Width / 2, asteroidTexture.Height / 2);
            }


        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        void ShootLaser(Vector2 position, float rotation)
        {
            int indexOfHitLaser = -1;

            for (int i = 0; i < numberOfLasers; i++)
            {
                if (laserHit[i] == true)
                {
                    indexOfHitLaser = i;
                    break;
                }
            }
            if (indexOfHitLaser == -1)
                return;

            Vector2 direction = new Vector2((float)-Math.Sin(rotation), (float)Math.Cos(rotation));

            laserRotation = playerRotation;
            direction.Normalize();
            laserVelocity[indexOfHitLaser] = direction * laserSpeed;
            laserPositions[indexOfHitLaser] = position;
            laserHit[indexOfHitLaser] = false;
        }

        void UpdateLaser(int lIndex, float deltaTime)
        {
            for (int laserIdx = 0; laserIdx < numberOfLasers; laserIdx++)
            {
                laserPositions[laserIdx] -= laserVelocity[laserIdx] * deltaTime;

                if (laserPositions[laserIdx].X < 0 ||
                    laserPositions[laserIdx].X > graphics.GraphicsDevice.Viewport.Width ||
                    laserPositions[laserIdx].Y < 0 ||
                    laserPositions[laserIdx].Y > graphics.GraphicsDevice.Viewport.Height)
                {
                    laserHit[laserIdx] = true;
                }




                for (int i = 0; i < numberOfAsteroids; i++)
                {
                    if (asteroidDeathStates[i] == false)
                    {
                        bool isColliding = IsColliding(laserPositions[laserIdx], laserRadius, asteroidPositions[i], asteroidRadius);
                        if (isColliding == true)
                        {
                            laserHit[laserIdx] = true;
                            asteroidDeathStates[i] = true;
                            playerScore = playerScore + 10;
                            break;

                        }
                    }
                }
            }
        }

        protected void UpdatePlayer(float deltaTime)
        {
            // TODO
            if (playerDeath == true)
            {
                return;
            }

            float xSpeed = 0;
            float ySpeed = 0;

            laserShootTimer += deltaTime;

            if (Keyboard.GetState().IsKeyDown(Keys.Down) == true)
            {
                ySpeed += playerSpeed * deltaTime;

            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up) == true)
            {
                ySpeed -= playerSpeed * deltaTime;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right) == true)
            {
                playerRotation += playerTurnSpeed * deltaTime;

            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left) == true)
            {
                playerRotation -= playerTurnSpeed * deltaTime;

            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space) == true)
            {
                if (laserShootTimer >= shootDelay)
                {
                    ShootLaser(playerPosition, playerRotation);
                    laserShootTimer = 0;
                }
            }

            double x = (xSpeed * Math.Cos(playerRotation)) - (ySpeed * Math.Sin(playerRotation));
            double y = (xSpeed * Math.Sin(playerRotation)) + (ySpeed * Math.Cos(playerRotation));

            playerPosition.X += (float)x;
            playerPosition.Y += (float)y;


            if (playerPosition.X < -playerOffSet.Y / 4)
            {
                playerPosition.X = graphics.GraphicsDevice.Viewport.Width - playerOffSet.Y / 4;
            }

            if (playerPosition.Y < -playerOffSet.Y / 4)
            {
                playerPosition.Y = graphics.GraphicsDevice.Viewport.Height - playerOffSet.Y / 4;
            }

            if (playerPosition.X > graphics.GraphicsDevice.Viewport.Width + playerOffSet.Y / 4)
            {
                playerPosition.X = playerOffSet.Y / 4;
            }

            if (playerPosition.Y > graphics.GraphicsDevice.Viewport.Height + playerOffSet.Y / 4)
            {
                playerPosition.Y = playerOffSet.Y / 4;
            }

            for (int i = 0; i < numberOfAsteroids; i++)
            {
                if (asteroidDeathStates[i] == false)
                {
                    bool isColliding = IsColliding(playerPosition, playerRadius / 1.5f, asteroidPositions[i], asteroidRadius / 1.5f);
                    if (isColliding == true)
                    {
                        playerDeath = true;
                        gameState = STATE_GAMEOVER;

                    }

                }
            }
        }

        protected void UpdateAsteroids(float deltaTime)
        {
            // TODO
            //call update for all asteroids
            for (int i = 0; i < numberOfAsteroids; i++)
            {
                if (asteroidDeathStates[i] == false)
                {

                    Vector2 velocity = new Vector2(
                        (float)(-asteroidSpeed * Math.Sin(asteroidRotations[i])),
                         (float)(asteroidSpeed * Math.Cos(asteroidRotations[i])));




                    if (asteroidPositions[i].X < 0)
                    {
                        asteroidPositions[i].X = 0;
                        velocity.X = -velocity.X;
                        asteroidRotations[i] = (float)Math.Atan2(velocity.Y, velocity.X) - 1.5708f;
                    }

                    if (asteroidPositions[i].Y < 0)
                    {
                        asteroidPositions[i].Y = 0;
                        velocity.Y = -velocity.Y;
                        asteroidRotations[i] = (float)Math.Atan2(velocity.Y, velocity.X) - 1.5708f;
                    }


                    if (asteroidPositions[i].X > graphics.GraphicsDevice.Viewport.Width)
                    {
                        asteroidPositions[i].X = graphics.GraphicsDevice.Viewport.Width;
                        velocity.X = -velocity.X;
                        asteroidRotations[i] = (float)Math.Atan2(velocity.Y, velocity.X) - 1.5708f;
                    }

                    if (asteroidPositions[i].Y > graphics.GraphicsDevice.Viewport.Height)
                    {
                        asteroidPositions[i].Y = graphics.GraphicsDevice.Viewport.Height;
                        velocity.Y = -velocity.Y;
                        asteroidRotations[i] = (float)Math.Atan2(velocity.Y, velocity.X) - 1.5708f;
                    }



                    asteroidPositions[i] += velocity * deltaTime;

                }

            }
        }

        protected void UpdateEnemyCollisions()
        {
            for (int i = 0; i < numberOfAsteroids; i++)
            {
                if (asteroidDeathStates[i] == true)
                    continue;
                for (int j = 1; j < numberOfAsteroids; j++)
                {
                    if (asteroidDeathStates[j] == true)
                        continue;

                    if (i == j)
                        continue;

                    if (IsColliding(asteroidPositions[i], asteroidRadius, asteroidPositions[j], asteroidRadius))
                    {
                        asteroidRotations[i] += 3.14159f;
                        asteroidRotations[j] += 3.14159f;
                        return;
                    }
                }
            }

        }

        protected void UpdateSplashState(float deltaTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) == true)
            {
                gameState = STATE_GAME;
            }
        }

        protected void UpdateGameState(float deltaTime)
        {
            UpdatePlayer(deltaTime);

            UpdateAsteroids(deltaTime);
            for (int bulletIdx = 0; bulletIdx < numberOfLasers; bulletIdx++)
            {
                UpdateLaser(bulletIdx, deltaTime);
            }


            UpdateEnemyCollisions();

            //check if asteroids are alive
            int aliveCount = 0;

            foreach (bool deathState in asteroidDeathStates)
            {
                if (deathState == false)
                    aliveCount++;
            }

            if (aliveCount == 0)
            {
                gameState = STATE_GAMEOVER;
            }
        }

        protected void UpdateGameOverState(float deltaTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) == true)
            {
                gameState = STATE_SPLASH;
                ResetGame();
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            fpsTimer += deltaTime;
            fpsCounter++;
            if (fpsTimer >= 1.0f)
            {
                currentFPS = fpsCounter;
                fpsCounter = 0;
                fpsTimer -= 1.0f;
            }
            switch (gameState)
            {
                case STATE_SPLASH:
                    UpdateSplashState(deltaTime);
                    break;

                case STATE_GAME:
                    UpdateGameState(deltaTime);
                    break;

                case STATE_GAMEOVER:
                    UpdateGameOverState(deltaTime);
                    break;
            }

            base.Update(gameTime);
        }

        protected void DrawSplashState(SpriteBatch spritebatch)
        {
            spriteBatch.Draw(splashBackground, new Rectangle(0, 0, 800, 480), Color.White);
            spriteBatch.DrawString(candaraFont, "Press Enter To Start The Game", new Vector2(250, screenHeight / 2), Color.Cyan);
        }

        protected void DrawGameState(SpriteBatch spritebatch)
        {
            //draw background
            spriteBatch.Draw(gameBackground, new Rectangle(0, 0, 800, 480), Color.White);

            //draw laser
            for (int i = 0; i < numberOfLasers; i++)
            {
                if (laserHit[i] == false)
                {
                    spriteBatch.Draw(laserTexture, laserPositions[i], null, Color.White, laserRotation, laserOffSet, 1, SpriteEffects.None, 0);
                }
            }

            //draw asteroid/s
            for (int i = 0; i < numberOfAsteroids; i++)
            {
                if (asteroidDeathStates[i] == false)
                {
                    spriteBatch.Draw(asteroidTexture, asteroidPositions[i], null, Color.White, asteroidRotations[i], asteroidOffSets[i], 1, SpriteEffects.None, 0);

                }
            }



            //draw player
            if (playerDeath == false)
            {
                spriteBatch.Draw(shipTexture, playerPosition, null, Color.White, playerRotation, playerOffSet, 0.25f, SpriteEffects.None, 0);
            }
            //draw wrapping ghost
            if (playerPosition.X < playerOffSet.Y / 4)
            {
                Vector2 wrapPos = new Vector2(graphics.GraphicsDevice.Viewport.Width + playerPosition.X, playerPosition.Y);
                spriteBatch.Draw(shipTexture, wrapPos, null, Color.White, playerRotation,
                    playerOffSet, 0.25f, SpriteEffects.None, 0);
            }

            if (playerPosition.Y < playerOffSet.Y / 4)
            {
                Vector2 wrapPos = new Vector2(playerPosition.X, graphics.GraphicsDevice.Viewport.Height + playerPosition.Y);
                spriteBatch.Draw(shipTexture, wrapPos, null, Color.White, playerRotation,
                    playerOffSet, 0.25f, SpriteEffects.None, 0);
            }

            if (playerPosition.X > graphics.GraphicsDevice.Viewport.Width - playerOffSet.Y / 4)
            {
                Vector2 wrapPos = new Vector2(-(graphics.GraphicsDevice.Viewport.Width - playerPosition.X), playerPosition.Y);
                spriteBatch.Draw(shipTexture, wrapPos, null, Color.White, playerRotation,
                    playerOffSet, 0.25f, SpriteEffects.None, 0);
            }

            if (playerPosition.Y > graphics.GraphicsDevice.Viewport.Height - playerOffSet.Y / 4)
            {

                Vector2 wrapPos = new Vector2(playerPosition.X, -(graphics.GraphicsDevice.Viewport.Height - playerPosition.Y));
                spriteBatch.Draw(shipTexture, wrapPos, null, Color.White, playerRotation,
                    playerOffSet, 0.25f, SpriteEffects.None, 0);
            }





            spriteBatch.DrawString(candaraFont, "FPS: " + currentFPS, new Vector2(graphics.GraphicsDevice.Viewport.Width - 100, 50), Color.White);
            spriteBatch.DrawString(candaraFont, "Score: " + playerScore, new Vector2(graphics.GraphicsDevice.Viewport.Width - 100, 80), Color.White);


        }

        protected void DrawGameOverState(SpriteBatch spritebatch)
        {
            spriteBatch.Draw(splashBackground, new Rectangle(0, 0, 800, 480), Color.White);
            if (playerScore >= numberOfAsteroids * 10)
            {
                spriteBatch.DrawString(candaraFont, "Congratulations, You Won! \nPress Enter To Play Again", new Vector2(275, screenHeight / 2), Color.Cyan);
            }

            if (playerScore < numberOfAsteroids * 10)
            {
                spriteBatch.DrawString(candaraFont, "Game Over \nPress Enter To Play Again", new Vector2(250, screenHeight / 2), Color.Cyan);
            }
            spriteBatch.DrawString(candaraFont, "Score: " + playerScore, new Vector2(275, screenHeight / 1.5f), Color.Cyan);
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            switch (gameState)
            {
                case STATE_SPLASH:
                    DrawSplashState(spriteBatch);
                    break;

                case STATE_GAME:
                    DrawGameState(spriteBatch);
                    break;

                case STATE_GAMEOVER:
                    DrawGameOverState(spriteBatch);
                    break;
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        // a circle-to-cirlce collision test
        protected bool IsColliding(Vector2 position1, float radius1, Vector2 position2, float radius2)
        {
            Vector2 direction = position1 - position2;
            float length = direction.Length();

            if (radius1 + radius2 < length)
            {
                return false;
            }
            return true;
        }


    }
}
