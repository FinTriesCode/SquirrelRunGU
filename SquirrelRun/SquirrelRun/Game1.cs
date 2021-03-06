using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.IO;
using System.Diagnostics;

namespace SquirrelRun
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //variables
        Sprite2D squirrel;
        Sprite2D squirrel_left;
        Sprite2D squirrel_right;
        Sprite2D car;
        Sprite2D carTwo;
        SpriteFont font;
        Sprite2D[] acorns = new Sprite2D[9];
        Sprite2D log;
        Sprite2D nessie;
        Sprite2D river;
        Sprite2D riverTwo;
        Sprite2D road;
        Sprite2D roadTwo;
        Sprite2D castle;
        Graphic2D gameOverImage;
        Graphic2D gameWinImage;

        SoundEffect jumpSound;
        SoundEffect pickupSound;
        SoundEffect rescueSound;
        Song bgMusic;

        Random rand = new Random();


        int lives = 5;
        int score = 0;
        int squirrelsRescued = 0;
        int displayHeight, displayWidth;
        int acornArrayPos = 0;
        bool gameOver = false;
        bool isSquirrelDead = false;
        float deltaTime;
        float respawnDelayLeft;
        float respawnDelay = .5f;

        float carSpeed = 4f;
        Vector3 carSpawnPos = Vector3.Zero;
        Vector3 carEndPos = Vector3.Zero;

        float car2Speed = 4f;
        Vector3 car2SpawnPos = Vector3.Zero;
        Vector3 car2EndPos = Vector3.Zero;

        float logSpeed = 2f;
        Vector3 logSpawnPos = Vector3.Zero;
        Vector3 logEndPos = Vector3.Zero;

        float nessieSpeed = 2f;
        Vector3 nessieSpawnPos = Vector3.Zero;
        Vector3 nessieEndPos = Vector3.Zero;

        float squirrelSpeed = 3f;
        Vector3 squirrelStartPos = Vector3.Zero;
        Vector3[] nutsPosition = new Vector3[9];

        bool gameWin = false;

        struct Sprite2D
        {
            public Texture2D image;
            public Vector3 position;
            public bool visible;
            public BoundingBox bBox;
            public float speed;
            public Color colour;
            public Vector2 origin;
            public Rectangle rect;
            public bool bonus;
            public float size;

            public Sprite2D(ContentManager content, string filename, float sizeratio, float _speed, bool _bonus)
            {
                image = content.Load<Texture2D>(filename);
                size = sizeratio;
                origin.X = image.Width / 2;
                origin.Y = image.Height / 2;
                rect.Width = (int)(image.Width * size);
                rect.Height = (int)(image.Height * size);

                //initial values for rest of properties
                bBox = new BoundingBox(Vector3.Zero, Vector3.Zero);
                position = Vector3.Zero;
                rect.X = 0;
                rect.Y = 0;
                visible = true;
                speed = _speed;
                colour = Color.White;
                bonus = _bonus;
            }
        }


        struct Graphic2D
        {
            public Texture2D image;
            public Rectangle rect;

            public Graphic2D(ContentManager content, string filename, int dwidth, int dheight)
            {
                image = content.Load<Texture2D>(filename);
                float ratio = ((float)dwidth / image.Width);
                rect.Width = dwidth;
                rect.Height = (int)(image.Height * ratio);
                rect.X = 0;
                rect.Y = (dheight - rect.Height) / 2;
            }
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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

            //screen display and resolution
            displayHeight = graphics.GraphicsDevice.Viewport.Height;
            displayWidth = graphics.GraphicsDevice.Viewport.Width;
            //graphics.ToggleFullScreen();
            //font variable
            font = Content.Load<SpriteFont>("SR font");

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //sprite variables and calling 
            squirrel = new Sprite2D(Content, "squirrel_front_v2", 0.3f, squirrelSpeed, false);
            squirrel_right = new Sprite2D(Content, "squirrel_right_v2", 0.3f, squirrelSpeed, false);
            squirrel_left = new Sprite2D(Content, "squirrel_left_v2", 0.3f, squirrelSpeed, false);
            car = new Sprite2D(Content, "car_v3", 0.3f, 5f, false);
            carTwo = new Sprite2D(Content, "car_v3", 0.3f, 5f, false);
            river = new Sprite2D(Content, "river_v2", 0.4f, 5f, false);
            riverTwo = new Sprite2D(Content, "river_v2", 0.4f, 5f, false);
            road = new Sprite2D(Content, "road", 0.4f, 5f, false);
            roadTwo = new Sprite2D(Content, "road", 0.4f, 5f, false);
            log = new Sprite2D(Content, "log", 0.4f, 5f, false);
            nessie = new Sprite2D(Content, "nessie_v3", 0.5f, 5f, false);
            castle = new Sprite2D(Content, "castle", 0.15f, 5f, false);
            gameOverImage = new Graphic2D(Content, "gameOverImage", displayWidth, displayHeight);
            gameWinImage = new Graphic2D(Content, "gameWinImage", displayWidth, displayHeight);

            //Sound Effects + Music
            jumpSound = Content.Load<SoundEffect>("jump");
            bgMusic = Content.Load<Song>("bg_music");
            pickupSound = Content.Load<SoundEffect>("sfx_pickup");
            rescueSound = Content.Load<SoundEffect>("sfx_rescue");
            MediaPlayer.Play(bgMusic);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = .3f;
            NutSpawningCode();

            //applying starting squirrel position
            squirrelStartPos = new Vector3(displayWidth / 2 - squirrel.image.Width / 2, displayHeight + 20 - squirrel.image.Height / 2, 0);
            squirrel.position = squirrelStartPos;

            //Sets position for bottom river
            logSpawnPos = new Vector3(displayWidth - 50, 200, 0);
            logEndPos = new Vector3(-40, 200, 0);
            log.position = logSpawnPos;

            //Sets position for top river
            nessieSpawnPos = new Vector3(0, 80, 0);
            nessieEndPos = new Vector3(displayWidth, 80, 0);
            nessie.position = nessieSpawnPos;

            //Right to left for car 1
            carSpawnPos = new Vector3(displayWidth - 50, displayHeight / 2 + 50, 0); //set car spawn position
            carEndPos = new Vector3(-40, displayHeight / 2, 0); // set car end position

            //Left to right for car 2
            car2SpawnPos = new Vector3(0, displayHeight / 2 + 150, 0); //set car spawn position
            car2EndPos = new Vector3(displayWidth, displayHeight / 2 + 150, 0); // set car end position

            car.position = carSpawnPos;
            carTwo.position = car2SpawnPos;

            river.position.Y = log.position.Y;
            riverTwo.position.Y = nessie.position.Y;

            road.position.Y = car.position.Y;
            roadTwo.position.Y = carTwo.position.Y;

            castle.position = new Vector3(displayWidth / 2 - castle.rect.Width / 2, 0, 0);
            respawnDelayLeft = respawnDelay;

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;


            //exit game
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            //setting squirrel position
            squirrel.rect.X = (int)squirrel.position.X;
            squirrel.rect.Y = (int)squirrel.position.Y;

            //setting car position
            car.rect.X = (int)car.position.X;
            car.rect.Y = (int)car.position.Y;
            Content.Load<Texture2D>("car_v3");

            //setting car 2 position
            carTwo.rect.X = (int)carTwo.position.X;
            carTwo.rect.Y = (int)carTwo.position.Y;
            Content.Load<Texture2D>("car_v3");

            //setting logs position
            log.rect.X = (int)log.position.X;
            log.rect.Y = (int)log.position.Y;
            Content.Load<Texture2D>("log");

            //setting nessie_v3 postion
            nessie.rect.X = (int)nessie.position.X;
            nessie.rect.Y = (int)nessie.position.Y;
            Content.Load<Texture2D>("nessie_v3");

            river.rect.X = (int)river.position.X;
            river.rect.Y = (int)river.position.Y;
            Content.Load<Texture2D>("river_v2");

            riverTwo.rect.X = (int)riverTwo.position.X;
            riverTwo.rect.Y = (int)riverTwo.position.Y;

            road.rect.X = (int)road.position.X;
            road.rect.Y = (int)road.position.Y;
            Content.Load<Texture2D>("road");

            roadTwo.rect.X = (int)roadTwo.position.X;
            roadTwo.rect.Y = (int)roadTwo.position.Y;

            gameOverImage.rect.X = 0;
            gameOverImage.rect.Y = (displayHeight - gameOverImage.rect.Height) / 2;
            Content.Load<Texture2D>("GameOverImage");

            gameWinImage.rect.X = 0;
            gameWinImage.rect.Y = (displayHeight - gameOverImage.rect.Height) / 2;
            Content.Load<Texture2D>("gameWinImage");

            castle.rect.X = (int)castle.position.X;
            castle.rect.Y = (int)castle.position.Y;
            Content.Load<Texture2D>("castle");

            //set squirrel bounding box
            squirrel.bBox = new BoundingBox(new Vector3(squirrel.position.X - squirrel.rect.Width / 2, squirrel.position.Y - squirrel.rect.Height / 2, 0), new Vector3(squirrel.position.X + squirrel.rect.Width / 2, squirrel.position.Y + squirrel.rect.Height / 2, 0));

            //set car(s) bounding box
            car.bBox = new BoundingBox(new Vector3(car.position.X - car.rect.Width / 2, car.position.Y - car.rect.Height / 2, 0), new Vector3(car.position.X + car.rect.Width / 2, car.position.Y + car.rect.Height / 2, 0));
            carTwo.bBox = new BoundingBox(new Vector3(carTwo.position.X - carTwo.rect.Width / 2, carTwo.position.Y - carTwo.rect.Height / 2, 0), new Vector3(carTwo.position.X + carTwo.rect.Width / 2, carTwo.position.Y + carTwo.rect.Height / 2, 0));

            //set logs bounding box
            log.bBox = new BoundingBox(new Vector3(log.position.X - log.rect.Width / 2, log.position.Y - log.rect.Height / 2, 0), new Vector3(log.position.X + log.rect.Width / 2, log.position.Y + log.rect.Height / 2, 0));

            //set nessie_v3 bounding box
            nessie.bBox = new BoundingBox(new Vector3(nessie.position.X - nessie.rect.Width / 2, nessie.position.Y - nessie.rect.Height / 2, 0), new Vector3(nessie.position.X + nessie.rect.Width / 2, nessie.position.Y + nessie.rect.Height / 2, 0));

            //Setting bounding boxes for both rivers.
            river.bBox = new BoundingBox(new Vector3(river.position.X - river.rect.Width / 2, river.position.Y - river.rect.Height / 2, 0), new Vector3(river.position.X + river.rect.Width / 2, river.position.Y + river.rect.Height / 2, 0));
            riverTwo.bBox = new BoundingBox(new Vector3(riverTwo.position.X - riverTwo.rect.Width / 2, riverTwo.position.Y - riverTwo.rect.Height / 2, 0), new Vector3(riverTwo.position.X + riverTwo.rect.Width / 2, riverTwo.position.Y + riverTwo.rect.Height / 2, 0));

            //Setting bounding boxes for both roads.
            road.bBox = new BoundingBox(new Vector3(road.position.X - road.rect.Width / 2, road.position.Y - road.rect.Height / 2, 0), new Vector3(road.position.X + road.rect.Width / 2, road.position.Y + road.rect.Height / 2, 0));
            roadTwo.bBox = new BoundingBox(new Vector3(roadTwo.position.X - roadTwo.rect.Width / 2, roadTwo.position.Y - roadTwo.rect.Height / 2, 0), new Vector3(roadTwo.position.X + roadTwo.rect.Width / 2, roadTwo.position.Y + roadTwo.rect.Height / 2, 0));

            castle.bBox = new BoundingBox(new Vector3(castle.position.X - castle.rect.Width / 2, castle.position.Y - castle.rect.Height / 2, 0), new Vector3(castle.position.X + castle.rect.Width / 2, castle.position.Y + castle.rect.Height / 2, 0));
            castle.bBox = new BoundingBox(new Vector3(castle.position.X - castle.rect.Width / 2, castle.position.Y - castle.rect.Height / 2, 0), new Vector3(castle.position.X + castle.rect.Width / 2, castle.position.Y + castle.rect.Height / 2, 0));

            //custom functions
            if (!isSquirrelDead)
            {
                PlayerMovement();
            }
            CarLogic();
            Car2Logic();
            AcornCollision();
            LogLogic();
            NessieAI();
            RiverLogic();
            ScreenCollisions();
            DisplayGameOver();
            DisplayGameWin();
            //InstaWin();       //MARION, THIS IS TO HELP YOUR TESTING/GRADING OF THE GAME EASIER (PRESS I)
            //InstaDeath();     //MARION, THIS IS TO HELP YOUR TESTING/GRADING OF THE GAME EASIER (PRESS O)
            SquirrelRescued();
            if (isSquirrelDead)
            {
                RespawnSquirrel();
            }

            //RespawnSquirrel();

            if (gameOver == true && Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                RestartGame();
            }

            if (gameWin == true && Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                RestartGame();
            }

            //Manual restart
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                RestartGame();
            }

            //add a win version of the code above

            base.Update(gameTime);
        }

        //movement function for player
        //The else if statements prevent more than one key being pressed at a time.
        //This stops the player from moving diagonally and prevents them from moving twice as fast
        //by pressing an arrow key and a WASD key in the same direction.
        void PlayerMovement()
        {   //controls for pro gamers
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                squirrel.image = Content.Load<Texture2D>("squirrel_front_v2");
                squirrel.position.Y -= squirrel.speed;

                //press w and update the sprite to the forward version of the original sprite              
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                squirrel.image = Content.Load<Texture2D>("squirrel_left_v2");
                squirrel.position.X -= squirrel.speed;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                squirrel.image = Content.Load<Texture2D>("squirrel_right_v2");
                squirrel.position.X += squirrel.speed;
            }
            //arrow keys for Marion
            else if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                squirrel.image = Content.Load<Texture2D>("squirrel_front_v2");
                squirrel.position.Y -= squirrel.speed;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                squirrel.image = Content.Load<Texture2D>("squirrel_left_v2");
                squirrel.position.X -= squirrel.speed;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                squirrel.image = Content.Load<Texture2D>("squirrel_right_v2");
                squirrel.position.X += squirrel.speed;
            }

            squirrel.rect.Width = (int)(squirrel.image.Width * squirrel.size);
            squirrel.rect.Height = (int)(squirrel.image.Height * squirrel.size);
        }

        public void DisplayGameOver()
        {
            if (lives <= 0) gameOver = true;
        }

        //For every acorn, check if it's collided with the squirrel, if it's visible make it invisible and increase score.
        public void AcornCollision()
        {
            for (int i = 0; i < acorns.Length; i++)
            {
                if (squirrel.bBox.Intersects(acorns[i].bBox) && acorns[i].visible == true)
                {
                    score++;
                    pickupSound.Play(0.1f, 0, 0);
                    acorns[i].visible = false;
                }
            }
        }

        //For car that moves to the right (Car on second row from bottom.)
        void CarLogic()
        {
            if (squirrel.bBox.Intersects(car.bBox)) isSquirrelDead = true;

            car.position.X -= carSpeed; //move car to left automatically

            if (car.position.X <= carEndPos.X) car.position.X = carSpawnPos.X; //if car position is equal to car end position car positon 'resets' to car spawn position
        }

        //For car that moves to the left (Car on first row from bottom.)
        void Car2Logic()
        {
            if (squirrel.bBox.Intersects(carTwo.bBox)) isSquirrelDead = true;

            carTwo.position.X += car2Speed; //move car to right automatically

            if (carTwo.position.X >= car2EndPos.X) //if car position is equal to car end position
            {
                carTwo.position.X = car2SpawnPos.X; //car positon 'resets' to car spawn position
            }
        }

        void LogLogic()
        {

            log.position.X -= logSpeed; //move log to left automatically

            if (log.position.X <= logEndPos.X) //if log position is equal to log end position
            {
                log.position.X = logSpawnPos.X; //log positon 'resets' to log spawn position
            }
            if (log.bBox.Intersects(squirrel.bBox))
            {
                squirrel.position.X = log.position.X;
            }
        }

        void NessieAI()
        {
            nessie.position.X += nessieSpeed; //move nessie_v3 to right automatically

            if (nessie.position.X >= nessieEndPos.X) //if nessie_v3 position is equal to nessie_v3 end position
            {
                nessie.position.X = nessieSpawnPos.X; //nessie_v3 positon 'resets' to nessie_v3 spawn position
            }
            if (nessie.bBox.Intersects(squirrel.bBox))
            {
                squirrel.position.X = nessie.position.X;
            }
        }

        void RiverLogic()
        {
            if (squirrel.bBox.Intersects(river.bBox) && !squirrel.bBox.Intersects(log.bBox) || squirrel.bBox.Intersects(riverTwo.bBox) && !squirrel.bBox.Intersects(nessie.bBox))
            {
                RespawnSquirrel();
            }
        }

        void RespawnSquirrel()
        {
            //Delay for squirrel is decreased until it's <= 0
            //Once this delay is <= 0, then we reset the squirrel's position to it's starting position.
            //Reset respawnDelay for next time and set isSquirrelDead to false.

            respawnDelayLeft -= deltaTime;

            if (respawnDelayLeft <= 0)
            {
                squirrel.position = squirrelStartPos;
                lives--;
                respawnDelayLeft = respawnDelay;
                isSquirrelDead = false;
            }
        }

        //Old version for respawning squirrel.
        /*void RespawnSquirrel()
        {
           lives--;
            squirrel.position = squirrelStartPos;
        }*/
      
        void NutSpawningCode()
        {
            //x = 100 means in river
            nutsPosition[0] = new Vector3(displayWidth / 2, 250f, 0);
            nutsPosition[1] = new Vector3(600, 100, 0);
            nutsPosition[2] = new Vector3(100, 300, 0);
            nutsPosition[3] = new Vector3(150, 250, 0);
            nutsPosition[4] = new Vector3(600, 300, 0);
            nutsPosition[5] = new Vector3(150, 200, 0);
            nutsPosition[6] = new Vector3(200, 100, 0);
            nutsPosition[7] = new Vector3(700, 50, 0);
            nutsPosition[8] = new Vector3(250, 50, 0);
            for (acornArrayPos = 0; acornArrayPos < acorns.Length; acornArrayPos++)
            {
                Content.Load<Texture2D>("acorn");
                acorns[acornArrayPos] = new Sprite2D(Content, "acorn", 0.4f, 5f, false);
                acorns[acornArrayPos].rect.X = (int)nutsPosition[acornArrayPos].X;
                acorns[acornArrayPos].rect.Y = (int)nutsPosition[acornArrayPos].Y;
                acorns[acornArrayPos].position = new Vector3(nutsPosition[acornArrayPos].X, nutsPosition[acornArrayPos].Y, 0);
                acorns[acornArrayPos].bBox = new BoundingBox(new Vector3(acorns[acornArrayPos].position.X - acorns[acornArrayPos].rect.Width / 2, acorns[acornArrayPos].position.Y - acorns[acornArrayPos].rect.Height / 2, 0), new Vector3(acorns[acornArrayPos].position.X + acorns[acornArrayPos].rect.Width / 2, acorns[acornArrayPos].position.Y + acorns[acornArrayPos].rect.Height / 2, 0));
            }
        }

        void RestartGame()
        {
            lives = 5;
            score = 0;
            squirrel.position = squirrelStartPos;

            for (int i = 0; i < acorns.Length; i++)
            {
                acorns[i].visible = true;                
            }
            gameOver = false;
            gameWin = false;
        }

        void ScreenCollisions()
        {
            //Left
            if (squirrel.position.X <= 0)
            {
                squirrel.position.X = 0;
            }
            //Right
            if (squirrel.position.X >= displayWidth - squirrel.rect.Width)
            {
                squirrel.position.X = displayWidth - squirrel.rect.Width;
            }
            //Top
            if(squirrel.position.Y <= squirrel.rect.Height / 2)
            {
                squirrel.position.Y = squirrel.rect.Height / 2;
            }
        }

        void SquirrelRescued()
        {
            if(squirrel.bBox.Intersects(castle.bBox))
            {
                rescueSound.Play(0.3f, 0f, 0f);
                squirrel.position = squirrelStartPos;
                squirrelsRescued += 1;
            }
        }

        void DisplayGameWin()
        {
            if (squirrelsRescued >= 3)
            {
                gameWin = true;
            }
        }

        void InstaWin()
        {
           if (Keyboard.GetState().IsKeyDown(Keys.I))
            {
                squirrelsRescued = squirrelsRescued + 3; 
            }
        }

        void InstaDeath()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.O))
            {
                lives = lives - 5;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //Background colour
            GraphicsDevice.Clear(Color.LimeGreen);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            //draw so that we can visibly see the sprites on the screen.
            spriteBatch.Draw(road.image, road.rect, Color.White);
            spriteBatch.Draw(roadTwo.image, roadTwo.rect, Color.White);
            spriteBatch.Draw(river.image, river.rect, Color.DeepSkyBlue);
            spriteBatch.Draw(riverTwo.image, riverTwo.rect, Color.DeepSkyBlue);

            for (int acornArrayPos = 0; acornArrayPos < acorns.Length; acornArrayPos++)
            {
                if (acorns[acornArrayPos].visible == true)
                {
                    spriteBatch.Draw(acorns[acornArrayPos].image, acorns[acornArrayPos].rect, Color.White);
                }
            }
            spriteBatch.Draw(log.image, log.rect, Color.White);
            spriteBatch.Draw(nessie.image, nessie.rect, Color.White);
            spriteBatch.Draw(squirrel.image, squirrel.rect, Color.White);
            spriteBatch.Draw(car.image, car.rect, Color.Crimson);
            spriteBatch.Draw(carTwo.image, carTwo.rect, Color.Yellow);
            spriteBatch.Draw(castle.image, castle.rect, Color.White);

            //display font of lives and score
            spriteBatch.DrawString(font, "Lives: " + lives , new Vector2(25, 20), Color.White);
            spriteBatch.DrawString(font, "Score: " + score, new Vector2(25, 50), Color.White);
            spriteBatch.DrawString(font, "Squirrels Saved: " + squirrelsRescued + "/3", new Vector2(25, displayHeight - 50), Color.White);


            //DEBUG STATUS CODE
            //spriteBatch.DrawString(font, "Squirrel X: " + squirrel.position.X, new Vector2(25, 80), Color.White);
            //spriteBatch.DrawString(font, "Squirrel Y: " + squirrel.position.Y, new Vector2(25, 110), Color.White);
            //spriteBatch.DrawString(font, "respawn delay: " + respawnDelayLeft, new Vector2(25, 140), Color.White);
            //spriteBatch.DrawString(font, "isSquirreldead: " + isSquirrelDead, new Vector2(25, 170), Color.White);
            //spriteBatch.DrawString(font, "deltaTime: " + deltaTime, new Vector2(25, 200), Color.White);


            if (gameOver == true)
            {
                spriteBatch.Draw(gameOverImage.image, gameOverImage.rect, Color.White);
            }

            if (gameWin == true)
            {
                spriteBatch.Draw(gameWinImage.image, gameWinImage.rect, Color.White);
            }

            //DEBUG STATUS CODE
            //spriteBatch.DrawString(font, "Game Win: " + gameWin, new Vector2(25, displayHeight - 80), Color.White);
            //spriteBatch.DrawString(font, "Game Over: " + gameOver, new Vector2(25, displayHeight - 110), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
            
        }


    }
}
