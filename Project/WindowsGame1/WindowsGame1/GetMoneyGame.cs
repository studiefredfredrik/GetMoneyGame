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

namespace GetMoneyGame
{
    /// <summary>
    /// Main class of the game
    /// </summary>
    public class GetMoneyGame : Microsoft.Xna.Framework.Game
    {
        // Setup our games different states, starting with the menu
        public enum GameState
        {
            MainMenu,
            Intro,
            Game,
            GameOver,
            Results
        };
        public static GameState CurrentGameState = GameState.MainMenu;

        public enum MainMenuState
        {
            Menu,
            Play,
            HowTo,
            About
        };
        public static MainMenuState CurrentMenuState = MainMenuState.Menu;
        public static MainMenuState MainMenuSelectedOption = MainMenuState.Play;

        // MainMenu
        //bool MainMenuIsInit = true;
        Vector3 MainMenuCameraPosition = new Vector3(10, 3, 10);
        float MainMenuCameraHeight = 3;
        bool MainMenuGoingUp = true;
        Texture2D MainMenuLogo;
        Vector2 MainMenuLogoPosition = new Vector2(30f, 30f);
        SpriteFont MenuFont;
        SpriteFont InfoSmall;

        // Intro
        Vector3 IntroCameraPosition;
        float IntroRotateSpeed;

        // Game Over
        Texture2D GameOver;

        // The basics
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Doing some declarations
        Camera camera;
        Maze maze;
        Cube cube;
        HelpWall helpwall;
        BasicEffect effect;

        // To hold the players score and time
        float lastScoreTime = 0f;
        float lastScoreTime2 = 0f;
        int score = 0;
        SpriteFont Font1;
        Vector2 FontPos;
        float TimeStarted = 0;
        float TimeElapsed = 0;

        //Music and sfx
        bool musicIsPlaying = false;
        Song gameMusic;
        SoundEffect sfxStart;
        SoundEffect sfxGotCash;
        SoundEffect sfxGameOver;
        SoundEffectInstance sfxStartInstance;
        SoundEffectInstance sfxGotCashInstance;
        SoundEffectInstance sfxGameOverInstance;

        // Movement
        float moveScale = 1.5f;

        public GetMoneyGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.Window.Title = "Get Money Game";
            // Prevents camera from going crazy after launch
            Mouse.SetPosition(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);

            // Adding our camera and setting it up in a starting position
            float aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
            camera = new Camera(
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(0,1,0),
                GraphicsDevice.Viewport.AspectRatio,
                0.05f,
                100f);
            
            effect = new BasicEffect(GraphicsDevice);
            maze = new Maze(GraphicsDevice);
            lastMouseState = Mouse.GetState();

            base.Initialize();
        }


        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Cube 
            cube = new Cube(
                this.GraphicsDevice,
                camera.position,
                10f,
                Content.Load<Texture2D>("5_trillion"));

            // Help wall
            helpwall = new HelpWall(this.GraphicsDevice, camera.position, 10f, Content.Load<Texture2D>("helpWallTexture"));

            // Sprite font
            Font1 = Content.Load<SpriteFont>("SpriteFont1");
            InfoSmall = Content.Load<SpriteFont>("InfoSmall");

            //Main menu
            MainMenuLogo = Content.Load<Texture2D>("GetMoneyGame-menu");  // 400 * 128 px
            MenuFont = Content.Load<SpriteFont>("MenuFont"); // size 19

            // Game Over
            GameOver = Content.Load<Texture2D>("GameOver");

            // Music and SFX
            gameMusic = Content.Load<Song>("Soundtrack_by_Fishy");
            sfxStart = Content.Load<SoundEffect>("sfx-Start");
            sfxGotCash = Content.Load<SoundEffect>("sfx-GotCash");
            sfxGameOver = Content.Load<SoundEffect>("sfx-GameOver");
            sfxStartInstance = sfxStart.CreateInstance();
            sfxGotCashInstance = sfxGotCash.CreateInstance();
            sfxGameOverInstance = sfxGameOver.CreateInstance();
            sfxStartInstance.Volume = 0.1f;
            sfxGotCashInstance.Volume = 0.1f;
            sfxGameOverInstance.Volume = 0.1f;
        }
 
        protected override void UnloadContent()
        {
        }

        KeyboardState lastKeyboard;
        MouseState lastMouseState;
        float rotationSpeed = 0.001f;
        float leftrightRot = 0f;
        float updownRot = 0f;
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            MouseState currentMouse = Mouse.GetState();

            // Time in seconds
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float moveAmountBackForth = 0;
            float moveAmountLeftRight = 0;

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (keyState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            // Main Menu
            if(CurrentGameState == GameState.MainMenu)
            {
                // Menu selection
                    //Play
                    //HowTo
                    //About
                if (keyState != lastKeyboard) 
                {
                    if (keyState.IsKeyDown(Keys.Enter) && 
                        gameTime.TotalGameTime.TotalSeconds >= 1f) // Prevents the game from going straight to 'play' if enter opened the .exe 
                    {
                        // Enter
                        if (CurrentMenuState == MainMenuState.Menu) // Main menu
                        {
                            if (MainMenuSelectedOption == MainMenuState.Play)
                            {
                                CurrentGameState = GameState.Intro;
                                sfxStartInstance.Play();
                            }
                            if (MainMenuSelectedOption == MainMenuState.HowTo) CurrentMenuState = MainMenuState.HowTo;
                            if (MainMenuSelectedOption == MainMenuState.About) CurrentMenuState = MainMenuState.About;
                            if (MainMenuSelectedOption == MainMenuState.Menu) CurrentMenuState = MainMenuState.Menu;
                        }
                        else if (CurrentMenuState == MainMenuState.HowTo) CurrentMenuState = MainMenuState.Menu;
                        else if (CurrentMenuState == MainMenuState.About) CurrentMenuState = MainMenuState.Menu;
                    }
                    if (keyState.IsKeyDown(Keys.W) || keyState.IsKeyDown(Keys.Up))
                    {
                        // Move selection up
                        if (MainMenuSelectedOption == MainMenuState.HowTo) MainMenuSelectedOption = MainMenuState.Play;
                        if (MainMenuSelectedOption == MainMenuState.About) MainMenuSelectedOption = MainMenuState.HowTo;

                    }
                    if (keyState.IsKeyDown(Keys.S) || keyState.IsKeyDown(Keys.Down))
                    {
                        // Move selection down
                        if (MainMenuSelectedOption == MainMenuState.HowTo) MainMenuSelectedOption = MainMenuState.About;
                        if (MainMenuSelectedOption == MainMenuState.Play) MainMenuSelectedOption = MainMenuState.HowTo;
                    }
                }

                // Camera
                if(MainMenuGoingUp)
                {
                    MainMenuCameraHeight += 0.1f * elapsed;
                    MainMenuCameraPosition = new Vector3(10f, MainMenuCameraHeight, 10f);
                }
                if(!MainMenuGoingUp)
                {
                    MainMenuCameraHeight -= 0.1f * elapsed;
                    MainMenuCameraPosition = new Vector3(10f, MainMenuCameraHeight, 10f);
                }
                if(MainMenuCameraHeight >= 4)
                {
                    MainMenuGoingUp = false;
                }
                if (MainMenuCameraHeight <= 3)
                {
                    MainMenuGoingUp = true;
                }
                camera.MoveTo(MainMenuCameraPosition);
                camera.updownRot = 5.8f;
                camera.leftrightRot -= 0.1f * elapsed;

                camera.UpdateViewMatrix();

                // Setup the intro                                                                                                                                                               
                IntroCameraPosition = MainMenuCameraPosition;
                IntroRotateSpeed = camera.leftrightRot;

            }



            // Intro
            if(CurrentGameState == GameState.Intro)
            {
                Vector3 targetPosition = new Vector3(0.5f, 0.5f, 0.5f); // This is where we start
                Vector3 cameraPosition = camera.position;

                // Rotate upDown towards horisontal
                if (camera.updownRot < MathHelper.TwoPi)
                {
                    camera.updownRot += 0.1f * elapsed;
                }
                // Rotate leftRight
                if (camera.leftrightRot < 0.3f)
                {
                    // Rotate towards 0 deg horisontal
                    // The more off point we are in the beginning the faster we rotate
                    camera.leftrightRot -= IntroRotateSpeed * elapsed;
                }
                // dont move if we have reached our destination
                if(Vector3.Distance(cameraPosition, targetPosition) > 0.1f)
                {
                    Vector3 directionOfTravel = targetPosition - cameraPosition;
                    directionOfTravel.Normalize(); // We only want the direction information so we normalize
                    IntroCameraPosition += (directionOfTravel * 3.9f * elapsed);
                    camera.MoveTo(IntroCameraPosition);
                }
                if(Vector3.Distance(cameraPosition, targetPosition) < 0.1f)
                {
                    // We have reached our destination
                    // Let the game begin
                    CurrentGameState = GameState.Game;

                    // Start timer
                    TimeElapsed = 0f;
                    TimeStarted = (float)gameTime.TotalGameTime.TotalSeconds;
                    lastScoreTime = 0f; // Start from when the game starts
                    lastScoreTime2 = TimeStarted;
                    score = 0;
                }
            }

            // During game
            if (CurrentGameState == GameState.Game)
            {
                // Check time
                TimeElapsed = (float)gameTime.TotalGameTime.TotalSeconds - TimeStarted;
                if(TimeElapsed >= 120f)
                {
                    // Time's up, game over
                    sfxGameOverInstance.Play();
                    CurrentGameState = GameState.GameOver;
                }

                // Get mouse update
                if (currentMouse != lastMouseState)
                {
                    float xDifference = currentMouse.X - lastMouseState.X;
                    float yDifference = currentMouse.Y - lastMouseState.Y;
                    leftrightRot -= rotationSpeed * xDifference;
                    updownRot -= rotationSpeed * yDifference;
                    Mouse.SetPosition(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);
                    camera.updownRot = updownRot;
                    camera.leftrightRot = leftrightRot;
                    camera.UpdateViewMatrix();
                }

                // WASD - movement
                if (keyState.IsKeyDown(Keys.D)) // Right
                {
                    moveAmountLeftRight = moveScale * elapsed;
                }
                if (keyState.IsKeyDown(Keys.A)) // Left
                {
                    moveAmountLeftRight = -moveScale * elapsed;
                }
                if (keyState.IsKeyDown(Keys.W)) // Forward
                {
                    if (keyState.IsKeyDown(Keys.LeftShift)) // Run
                    {
                        moveAmountBackForth = -moveScale * elapsed * 5;
                    }
                    else
                    {
                        moveAmountBackForth = -moveScale * elapsed;
                    }
                }
                if (keyState.IsKeyDown(Keys.S)) // Backward
                {
                    moveAmountBackForth = moveScale * elapsed;
                }

                // Jump and crouch (not implemented)

                // Check the bounderies before move
                if (moveAmountBackForth != 0)
                {
                    Vector3 newLocation = camera.PreviewMove(moveAmountBackForth);
                    bool moveOk = true;
                    if (newLocation.X < 0 || newLocation.X > Maze.mazeWidth)
                        moveOk = false;
                    if (newLocation.Z < 0 || newLocation.Z > Maze.mazeHeight)
                        moveOk = false;

                    // Check if we are crashing with walls
                    foreach (BoundingBox box in
                        maze.GetBoundsForCell((int)newLocation.X, (int)newLocation.Z))
                    {
                        if (box.Contains(newLocation) == ContainmentType.Contains)
                            moveOk = false;
                    }
                    if (moveOk)
                        camera.MoveForward(moveAmountBackForth);
                }
                if (moveAmountLeftRight != 0)
                {
                    Vector3 newLocation = camera.PreviewMoveLeftRight(moveAmountLeftRight);
                    bool moveOk = true;
                    if (newLocation.X < 0 || newLocation.X > Maze.mazeWidth)
                        moveOk = false;
                    if (newLocation.Z < 0 || newLocation.Z > Maze.mazeHeight)
                        moveOk = false;

                    // Check if we are crashing with walls
                    foreach (BoundingBox box in
                        maze.GetBoundsForCell((int)newLocation.X, (int)newLocation.Z))
                    {
                        if (box.Contains(newLocation) == ContainmentType.Contains)
                            moveOk = false;
                    }

                    if (moveOk)
                        camera.MoveSide(moveAmountLeftRight);
                }

                // Check for cube collision
                if (cube.Bounds.Contains(camera.position) ==
                    ContainmentType.Contains)
                {
                    sfxGotCashInstance.Play();
                    cube.PositionCube(camera.position, 5f);
                    float thisTime = (float)gameTime.TotalGameTime.TotalSeconds;
                    float scoreTime = thisTime - lastScoreTime2;
                    score += 1000;
                    if (scoreTime < 120)
                    {
                        score += (120 - (int)scoreTime) * 100;
                    }
                    lastScoreTime2 = scoreTime;
                    lastScoreTime = thisTime - TimeStarted;
                }
            }

            // Game over
            if (CurrentGameState == GameState.GameOver)
            {
                if (keyState != lastKeyboard)
                {
                    if (keyState.IsKeyDown(Keys.Enter) || keyState.IsKeyDown(Keys.Execute))
                    {
                        // Enter
                        CurrentMenuState = MainMenuState.Menu;
                        CurrentGameState = GameState.MainMenu;
                    }
                }
            }

            // Start music if not started
            if (!musicIsPlaying)
            {
                MediaPlayer.Play(gameMusic);
                musicIsPlaying = true;
                MediaPlayer.Volume = 0.1f;
            }

            // Adjust the volume
            if (keyState != lastKeyboard)
            {
                if (keyState.IsKeyDown(Keys.Add))
                {
                    if (MediaPlayer.Volume <= 0.9f)
                    {
                        MediaPlayer.Volume += 0.05f;
                        sfxStartInstance.Volume += 0.05f;
                        sfxGotCashInstance.Volume += 0.05f;
                        sfxGameOverInstance.Volume += 0.05f;
                    }
                }
                if (keyState.IsKeyDown(Keys.Subtract))
                {
                    if (MediaPlayer.Volume >= 0.01f)
                    {
                        MediaPlayer.Volume -= 0.05f;
                        sfxStartInstance.Volume -= 0.05f;
                        sfxGotCashInstance.Volume -= 0.05f;
                        sfxGameOverInstance.Volume -= 0.05f;
                    }
                }
            }

            // Turn the cube
            cube.Update(gameTime);

            // Base?
            base.Update(gameTime);

            // To allow us to compare states
            lastKeyboard = keyState;
            currentMouse = lastMouseState;
        }

        protected override void Draw(GameTime gameTime)
        {
            // Starting spritebatch to enable writing text to screen
            spriteBatch.Begin();
            GraphicsDevice.BlendState = BlendState.Opaque; // These parameters get fucked by spritebatch default
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;  // So we change them back to avoid bad rendering

            // Background color
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Dra our stuff
            maze.Draw(camera, effect);
            cube.Draw(camera, effect);
            helpwall.Draw(camera, effect);

            if(CurrentGameState == GameState.MainMenu)
            {
                // Draw the logo with no color effect to it
                //spriteBatch.Draw(MainMenuLogo, MainMenuLogoPosition, Color.White);
                spriteBatch.Draw(MainMenuLogo, MainMenuLogoPosition, Color.Yellow);

                // The menu
                    //Play
                    //HowTo
                    //About
                if (CurrentMenuState == MainMenuState.Menu)
                {
                    spriteBatch.DrawString(MenuFont, "Play", new Vector2(600f, 400f), Color.Black, 0, new Vector2(200, 200), 1.0f, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(MenuFont, "HowTo", new Vector2(620f, 440f), Color.Black, 0, new Vector2(200, 200), 1.0f, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(MenuFont, "About", new Vector2(640f, 480f), Color.Black, 0, new Vector2(200, 200), 1.0f, SpriteEffects.None, 0.5f);
                    if(MainMenuSelectedOption == MainMenuState.Play)
                        spriteBatch.DrawString(MenuFont, "*", new Vector2(580f, 400f), Color.Black, 0, new Vector2(200, 200), 1.0f, SpriteEffects.None, 0.5f);
                    if(MainMenuSelectedOption == MainMenuState.HowTo)
                        spriteBatch.DrawString(MenuFont, "*", new Vector2(600f, 440f), Color.Black, 0, new Vector2(200, 200), 1.0f, SpriteEffects.None, 0.5f);
                    if(MainMenuSelectedOption == MainMenuState.About)
                        spriteBatch.DrawString(MenuFont, "*", new Vector2(620f, 480f), Color.Black, 0, new Vector2(200, 200), 1.0f, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(InfoSmall, "- and + to adjust volume\nESC to quit", new Vector2(820f, 640f), Color.Black, 0, new Vector2(200, 200), 1.0f, SpriteEffects.None, 0.5f);
                }
                if(CurrentMenuState == MainMenuState.HowTo)
                {
                    spriteBatch.DrawString(MenuFont, "Such a maze\n          Need moneys\n Wow spinning cube\n      wooof\n                   Get many moneyz as can haz\n    120seconds  " + 
                    "\n\n                                  *Go back nao?",
                        new Vector2(250f, 400f), Color.Black, 0, new Vector2(200, 200), 1.0f, SpriteEffects.None, 0.5f);
                }
                if (CurrentMenuState == MainMenuState.About)
                {
                    spriteBatch.DrawString(MenuFont, "Created by Fredrik\ngithub.com/studiefredfredrik\nMusic by Fishy\n\n\n\n\n                                  *Go back?"
                        , new Vector2(250f, 400f), Color.Black, 0, new Vector2(200, 200), 1.0f, SpriteEffects.None, 0.5f);
                }
            }
            if (CurrentGameState == GameState.Game)
            {
                // Score
                //spriteBatch.Begin();
                FontPos = new Vector2(40f, 25f);
                string output = "Total stacks: " + score + "\nLast time was: " + Math.Round(lastScoreTime) + "s";
                //Vector2 FontOrigin = Font1.MeasureString(output) / 2;
                Vector2 FontOrigin = FontPos;
                spriteBatch.DrawString(Font1, output, FontPos, Color.Black,
                              0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            }
            if (CurrentGameState == GameState.GameOver)
            {
                spriteBatch.Draw(GameOver, new Vector2(150f,-10f), Color.White);
                spriteBatch.DrawString(MenuFont, "You stacked a total of: " + score + " stacks!" +"\n\n\n\n\n\n                                  *Go to menu?"
                        , new Vector2(250f, 400f), Color.Black, 0, new Vector2(200, 200), 1.0f, SpriteEffects.None, 0.5f);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
