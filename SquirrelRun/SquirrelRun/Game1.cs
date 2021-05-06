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


        int displayHeight, displayWidth;

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
            displayHeight = graphics.GraphicsDevice.Viewport.Height;
            displayWidth = graphics.GraphicsDevice.Viewport.Width;
            graphics.ToggleFullScreen();
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
            squirrel = new Sprite2D(Content, "squirrel", 0.4f, 5f, false);
            squirrel_right = new Sprite2D(Content, "squirrel_right", 0.4f, 5f, false);
            squirrel_left = new Sprite2D(Content, "squirrel_left", 0.4f, 5f, false);

            //applying squirrel position
            squirrel.position = new Vector3(displayWidth / 2 - squirrel.image.Width / 2, displayHeight + 150 - squirrel.image.Height, 0);
           


            // TODO: use this.Content to load your game content here
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

            /*squirrel_left.rect.X = (int)squirrel_left.position.X;
            squirrel_left.rect.Y = (int)squirrel_left.position.Y;

            squirrel_right.rect.X = (int)squirrel_right.position.X;
            squirrel_right.rect.Y = (int)squirrel_right.position.Y;*/

            //set squirrel bounding box
            squirrel.bBox = new BoundingBox(new Vector3(squirrel.position.X - squirrel.rect.Width / 2, squirrel.position.Y - squirrel.rect.Height / 2, 0), new Vector3(squirrel.position.X + squirrel.rect.Width / 2, squirrel.position.Y + squirrel.rect.Height / 2, 0));
            //squirrel_left.bBox = new BoundingBox(new Vector3(squirrel_left.position.X - squirrel_left.rect.Width / 2, squirrel_left.position.Y - squirrel_left.rect.Height / 2, 0), new Vector3(squirrel_left.position.X + squirrel_left.rect.Width / 2, squirrel_left.position.Y + squirrel_left.rect.Height / 2, 0));
           // squirrel_right.bBox = new BoundingBox(new Vector3(squirrel_right.position.X - squirrel_right.rect.Width / 2, squirrel_right.position.Y - squirrel_right.rect.Height / 2, 0), new Vector3(squirrel_right.position.X + squirrel_right.rect.Width / 2, squirrel_right.position.Y + squirrel_right.rect.Height / 2, 0));

            //player movement
            PlayerMovement();


            base.Update(gameTime);
        }


        void PlayerMovement()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {

                squirrel.image = Content.Load<Texture2D>("Squirrel");
                squirrel.rect.Width = (int)(squirrel.image.Width * squirrel.size);
                squirrel.rect.Height = (int)(squirrel.image.Height * squirrel.size);
                squirrel.position.Y -= squirrel.speed;
                //press w and update the sprite to the forward version of the original sprite
                //we will have 3 different sprites of the squireel, but each save is the same imgae, just rotated to the left/right

            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                //squirrel = new Sprite2D(Content, "squirrel_left", 0.4f, 5f, false);
                squirrel.position.X -= squirrel.speed;
                squirrel.image = Content.Load<Texture2D>("squirrel_left");
                squirrel.rect.Width = (int)(squirrel.image.Width * squirrel.size);
                squirrel.rect.Height = (int)(squirrel.image.Height * squirrel.size);

            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                squirrel.image = Content.Load<Texture2D>("squirrel_right");
                //squirrel = new Sprite2D(Content, "squirrel_right", 0.4f, 5f, false);
                squirrel.position.X += squirrel.speed;
                squirrel.rect.Width = (int)(squirrel.image.Width * squirrel.size);
                squirrel.rect.Height = (int)(squirrel.image.Height * squirrel.size);

            }

            //arrow keys for Marion
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                squirrel.image = Content.Load<Texture2D>("Squirrel");
                squirrel.position.Y -= squirrel.speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                squirrel.image = Content.Load<Texture2D>("squirrel_left");
                squirrel.position.X -= squirrel.speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                squirrel.image = Content.Load<Texture2D>("squirrel_right");
                squirrel.position.X += squirrel.speed;
            }

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

            spriteBatch.Draw(squirrel.image, squirrel.rect, Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
