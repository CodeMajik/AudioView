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


namespace AudioGraph
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public MediaLibrary mediaLibrary;
        public SongCollection songCollection;
        public VisualizationData visualizationData;
        Texture2D mBlankTexture;
        static bool first = true;

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
           
            mediaLibrary = new MediaLibrary();
            visualizationData = new VisualizationData();
            songCollection = mediaLibrary.Songs; 
            MediaPlayer.IsVisualizationEnabled = true; 
            MediaPlayer.Play(songCollection);
            mBlankTexture = Content.Load<Texture2D>("zone_def");

            
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                MediaPlayer.MoveNext();

            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.GetVisualizationData(visualizationData);
            }

            base.Update(gameTime);
        }

        private void DrawVisualizationBarGraph()
        {
            Viewport viewport = graphics.GraphicsDevice.Viewport;
            int x, y, width, height;
         
            for (int f = 0; f < visualizationData.Frequencies.Count; f++)
            {
                x = viewport.Width * f / visualizationData.Frequencies.Count;
                y = (int)(viewport.Height / 2 - visualizationData.Frequencies[f] * viewport.Height / 2);
                width = 1;
                height = (int)(visualizationData.Frequencies[f] * viewport.Height / 2);
                
                spriteBatch.Draw(mBlankTexture, new Rectangle(x, y, width, height), Color.Red);
            }

            for (int s = 0; s < visualizationData.Samples.Count; s++)
            {
                x = viewport.Width * s / visualizationData.Samples.Count;
                width = 1;
                if (visualizationData.Samples[s] > 0.0f)
                {
                   y = (int)(0.75f * viewport.Height - visualizationData.Samples[s] * viewport.Height / 4);
                   height = (int)(visualizationData.Samples[s] * viewport.Height / 4);
                }
                else
                {
                    y = (int)(0.75f * viewport.Height);
                    height = (int)(-1.0f * visualizationData.Samples[s] * viewport.Height / 4);
                }
                spriteBatch.Draw(mBlankTexture, new Rectangle(x, y, width, height), Color.Red);
            }
        }

        public void DrawLineTo(Vector2 src, Vector2 dst, Color color)
        {
            //direction is destination - source vectors
            Vector2 direction = dst - src;
            //get the angle from 2 specified numbers (our point)
            var angle = (float)Math.Atan2(direction.Y, direction.X);
            //calculate the distance between our two vectors
            float distance;
            Vector2.Distance(ref src, ref dst, out distance);

            //draw the sprite with rotation
            spriteBatch.Draw(mBlankTexture, src, new Rectangle((int)src.X, (int)src.Y, (int)distance, 1), Color.White, angle, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
        }

        private void DrawVisualizationLineGraph()
        {
            Viewport viewport = graphics.GraphicsDevice.Viewport;
            int x, y, prevX, prevY;
            prevX = 0;
            prevY = 0;
            for (int f = 0; f < visualizationData.Frequencies.Count; f++)
            {
                x = viewport.Width * f / visualizationData.Frequencies.Count;
                y = (int)(viewport.Height / 2 - visualizationData.Frequencies[f] * viewport.Height / 2);
                if (first)
                {
                    prevX = x;
                    prevY = y;
                    first = false;
                }
                DrawLineTo(new Vector2(prevX, prevY), new Vector2(x, y), Color.Red);
                prevX = x;
                prevY = y;
                //spriteBatch.Draw(mBlankTexture, new Rectangle(x, y, width, 1), Color.Red);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.WhiteSmoke);
            spriteBatch.Begin();
            DrawVisualizationBarGraph();

            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
