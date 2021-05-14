using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
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
        Sprite2D car2;
        SpriteFont font;
        Sprite2D[] acorns = new Sprite2D[5];

        int lives = 5;
        int score = 0;
        int displayHeight, displayWidth;
        int acornArrayPos = 0;
        bool gameOver = false;

        float carSpeed = 2.5f;
        Vector3 carSpawnPos = Vector3.Zero;
        Vector3 carEndPos = Vector3.Zero;


        float car2Speed = 2.5f;
        Vector3 car2SpawnPos = Vector3.Zero;
        Vector3 car2EndPos = Vector3.Zero;

        Vector3 startingPosition = Vector3.Zero;


        public void Random()
        {
            //random number generator
            Random rand = new Random();
            int randint = rand.Next(1, 10);
        }


        //nuts array
        Vector3[] nutsPosition = new Vector3[6];
        
   

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

        
        //bounding box for water - "will kill player upon contact (yet to code function)"
        struct Water
        {                       
            public Vector3 waterPosition;
            public BoundingBox waterBBOX;           
            public float waterSize;

            public Water(ContentManager content, string filename, float sizeratio)
            {
                //water bounding box
                waterSize = sizeratio;

                waterBBOX = new BoundingBox(Vector3.Zero, Vector3.Zero);  //now we need to set the water's position in loadContent using the size of the image/screen and devide the height by 2 and add half the screen's value (making it the top half of the screen)
                waterPosition = Vector3.Zero;                

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

            //sprite variables and calling 
            spriteBatch = new SpriteBatch(GraphicsDevice);
            squirrel = new Sprite2D(Content, "squirrel", 0.4f, 5f, false);
            squirrel_right = new Sprite2D(Content, "squirrel_right", 0.4f, 5f, false);
            squirrel_left = new Sprite2D(Content, "squirrel_left", 0.4f, 5f, false);

            startingPosition = new Vector3(displayWidth / 2 - squirrel.image.Width / 2, displayHeight + 150 - squirrel.image.Height, 0);

            car = new Sprite2D(Content, "car", 0.4f, 5f, false);           
            car2 = new Sprite2D(Content, "car", 0.4f, 5f, false);

            nutsPosition[0] = new Vector3(10, 10, 0);
            nutsPosition[1] = new Vector3(20, 20, 0);
            nutsPosition[2] = new Vector3(30, 30, 0);

            for (acornArrayPos = 0; acornArrayPos < acorns.Length; acornArrayPos++)
            {
                Content.Load<Texture2D>("acorn");
                acorns[acornArrayPos] = new Sprite2D(Content, "acorn", 0.4f, 5f, false);
                acorns[acornArrayPos].position = new Vector3(displayWidth / 2 - acorns[acornArrayPos].image.Width, displayHeight / 2 - acorns[acornArrayPos].image.Height, 0);
                acorns[acornArrayPos].rect.X = (int)acorns[acornArrayPos].position.X;
                acorns[acornArrayPos].rect.Y = (int)acorns[acornArrayPos].position.Y;
                acorns[acornArrayPos].bBox = new BoundingBox(new Vector3(acorns[acornArrayPos].position.X - acorns[acornArrayPos].rect.Width / 2, acorns[acornArrayPos].position.Y - acorns[acornArrayPos].rect.Height / 2, 0), new Vector3(acorns[acornArrayPos].position.X + acorns[acornArrayPos].rect.Width / 2, acorns[acornArrayPos].position.Y + acorns[acornArrayPos].rect.Height / 2, 0));
            }
           

            //applying starting squirrel position
            squirrel.position = new Vector3(displayWidth / 2 - squirrel.image.Width / 2, displayHeight + 150 - squirrel.image.Height, 0);

           

            //Right to left for car 1
            carSpawnPos = new Vector3(displayWidth - 50, displayHeight / 2, 0); //set car spawn position
            carEndPos = new Vector3(-40, displayHeight / 2, 0); // set car end position

            //Left to right for car 2
            car2SpawnPos = new Vector3(0, displayHeight / 2 + 150, 0); //set car spawn position
            car2EndPos = new Vector3(displayWidth, displayHeight / 2 + 150, 0); // set car end position

            car.position = carSpawnPos;
            car2.position = car2SpawnPos;
            //so that when car reaches end postion, it will reset to car spawn position
            
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
            //exit game
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            //setting squirrel position
            squirrel.rect.X = (int)squirrel.position.X;
            squirrel.rect.Y = (int)squirrel.position.Y;

            //set squirrel bounding box
            squirrel.bBox = new BoundingBox(new Vector3(squirrel.position.X - squirrel.rect.Width / 2, squirrel.position.Y - squirrel.rect.Height / 2, 0), new Vector3(squirrel.position.X + squirrel.rect.Width / 2, squirrel.position.Y + squirrel.rect.Height / 2, 0));


            //setting car position
            car.rect.X = (int)car.position.X;
            car.rect.Y = (int)car.position.Y;
            Content.Load<Texture2D>("car");

            //setting car 2 position
            car2.rect.X = (int)car2.position.X;
            car2.rect.Y = (int)car2.position.Y;
            Content.Load<Texture2D>("car");

            //set car(s) bounding box
            car.bBox = new BoundingBox(new Vector3(car.position.X - car.rect.Width / 2, car.position.Y - car.rect.Height / 2, 0), new Vector3(car.position.X + car.rect.Width / 2, car.position.Y + car.rect.Height / 2, 0));
            car2.bBox = new BoundingBox(new Vector3(car2.position.X - car2.rect.Width / 2, car2.position.Y - car2.rect.Height / 2, 0), new Vector3(car2.position.X + car2.rect.Width / 2, car2.position.Y + car2.rect.Height / 2, 0));

          
           

            //custom functions
            PlayerMovement();
            CarCode();
            Car2Code();
            AcornCollision(); 

            base.Update(gameTime);
        }

        //movement function for player
        void PlayerMovement()
        {   //controls for pro gamers
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {

                squirrel.image = Content.Load<Texture2D>("Squirrel");
                squirrel.rect.Width = (int)(squirrel.image.Width * squirrel.size); 
                squirrel.rect.Height = (int)(squirrel.image.Height * squirrel.size);
                squirrel.position.Y -= squirrel.speed;
                //press w and update the sprite to the forward version of the original sprite
                

            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                squirrel.image = Content.Load<Texture2D>("squirrel_left");
                squirrel.rect.Width = (int)(squirrel.image.Width * squirrel.size);
                squirrel.rect.Height = (int)(squirrel.image.Height * squirrel.size);
                squirrel.position.X -= squirrel.speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                squirrel.image = Content.Load<Texture2D>("squirrel_right");
                squirrel.rect.Width = (int)(squirrel.image.Width * squirrel.size);
                squirrel.rect.Height = (int)(squirrel.image.Height * squirrel.size);
                squirrel.position.X += squirrel.speed;
            }

            //arrow keys for Marion
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                squirrel.image = Content.Load<Texture2D>("Squirrel");
                squirrel.rect.Width = (int)(squirrel.image.Width * squirrel.size);
                squirrel.rect.Height = (int)(squirrel.image.Height * squirrel.size);
                squirrel.position.Y -= squirrel.speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                squirrel.image = Content.Load<Texture2D>("squirrel_left");
                squirrel.rect.Width = (int)(squirrel.image.Width * squirrel.size);
                squirrel.rect.Height = (int)(squirrel.image.Height * squirrel.size);
                squirrel.position.X -= squirrel.speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                squirrel.image = Content.Load<Texture2D>("squirrel_right");
                squirrel.rect.Width = (int)(squirrel.image.Width * squirrel.size);
                squirrel.rect.Height = (int)(squirrel.image.Height * squirrel.size);
                squirrel.position.X += squirrel.speed;
            }

           

        }

        public void Lives()
        {
            if (lives <= 0)
            {
                gameOver = true;
            }               
        }

        


        public void AcornCollision()
        {
            for (int i = 0; i < acorns.Length; i++)
            {
                if (squirrel.bBox.Intersects(acorns[i].bBox) && acorns[i].visible == true)
                {
                    score++;
                    acorns[i].visible = false;
                }
            }
  
        }
       

        void CarCode()
        {
            if (squirrel.bBox.Intersects(car.bBox))
            {
                SquirrelDeath();               
            }

            car.position.X -= carSpeed; //move car to left automatically

            if(car.position.X == carEndPos.X) //if car position is equal to car end position
            {
                car.position.X = carSpawnPos.X; //car positon 'resets' to car spawn position
            }

          

        }

        void Car2Code()
        {
            if (squirrel.bBox.Intersects(car2.bBox))
            {
                SquirrelDeath();
            }

            car2.position.X += car2Speed; //move car to right automatically

            if (car2.position.X == car2EndPos.X) //if car position is equal to car end position
            {
                car2.position.X = car2SpawnPos.X; //car positon 'resets' to car spawn position
            }
        }

        void SquirrelDeath()
        {
            lives--;
            squirrel.position = startingPosition;
        }

        void GameOver()
        {
            if (gameOver == true)
            {
                //do later
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGreen);




            // TODO: Add your drawing code here
            spriteBatch.Begin();

            //draw so that we can visibly see the sprites on the screen.
            spriteBatch.Draw(squirrel.image, squirrel.rect, Color.White);
            spriteBatch.Draw(car.image, car.rect, Color.White);
            spriteBatch.Draw(car2.image, car2.rect, Color.White);

            //display font of lives and score
            spriteBatch.DrawString(font, "Score: " + score, new Vector2(25, 50), Color.White);
            spriteBatch.DrawString(font, "Lives: " + lives , new Vector2(25, 20), Color.White);

            for (int acornArrayPos = 0; acornArrayPos < acorns.Length; acornArrayPos++)
            {
                if (acorns[acornArrayPos].visible == true)
                {
                    spriteBatch.Draw(acorns[acornArrayPos].image, acorns[acornArrayPos].rect, Color.White);
                }
            }



            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
