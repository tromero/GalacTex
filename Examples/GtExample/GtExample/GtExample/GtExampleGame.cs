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

using System.Diagnostics;
using GalacTex;

namespace GtExample
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GtExampleGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        BasicEffect basicEffect;
        Model model;

        KeyboardState prevKeyboard;

        int exampleNumber = 0;

        Model teapot;
        Model teapot1;
        Model monkey;
        Model monkey1;

        public GtExampleGame()
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
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 512;
            graphics.ApplyChanges();

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

            // Initialize the texture renderer
            Stopwatch s = Stopwatch.StartNew();
            TextureRenderer.LoadContent(GraphicsDevice, Content);
            Debug.WriteLine("Setting up for texture rendering:" + s.ElapsedMilliseconds);

            teapot = Content.Load<Model>("models/teapot");
            teapot1 = Content.Load<Model>("models/teapot1");
            monkey = Content.Load<Model>("models/suzanne");
            monkey1 = Content.Load<Model>("models/suzanne1");

            TextureRenderer renderer;

            // render the marble texture for the teapot
            {
                Effect mat = Content.Load<Effect>("Marble");
                renderer = new TextureRenderer(teapot, mat, 1024, 1024);
                renderer.RenderTexture();
                Texture2D tex = renderer.GetTexture();

                SetupEffect(teapot, tex);
            }

            // render the teapot with a decal applied
            {
                Effect mat = Content.Load<Effect>("Decal");
                Texture2D logo = Content.Load<Texture2D>("GalacTexContent/Template");
                mat.Parameters["decalTex0"].SetValue(logo);
                mat.Parameters["decalTransform0"].SetValue(Matrix.CreateLookAt(new Vector3(3, 2.5f, 0), new Vector3(1f, 1.7f, 0), new Vector3(0, 1, 0)) *
                                                          Matrix.CreateOrthographic(1.2f, 1.2f, 1f, 2f));

                renderer = new TextureRenderer(teapot1, mat, 1024, 1024);
                renderer.RenderTexture();

                Texture2D tex = renderer.GetTexture();

                SetupEffect(teapot1, tex);
            }

            // render the marble monkey
            {
                Effect mat = Content.Load<Effect>("Marble");
                renderer = new TextureRenderer(monkey, mat, 1024, 1024);
                renderer.RenderTexture();
                Texture2D tex = renderer.GetTexture();
                SetupEffect(monkey, tex);
            }

            // render the monkey with a big decal applied
            {
                Effect mat = Content.Load<Effect>("Decal");
                Texture2D logo = Content.Load<Texture2D>("blender_logo");
                mat.Parameters["decalTex0"].SetValue(logo);
                mat.Parameters["decalTransform0"].SetValue(Matrix.CreateLookAt(new Vector3(1, 1, 1), new Vector3(0, 0, 0), new Vector3(0, 1, 0)) *
                                                          Matrix.CreateOrthographic(2f, 2f, 0.5f, 3f));

                renderer = new TextureRenderer(monkey1, mat, 1024, 1024);
                renderer.RenderTexture();

                Texture2D tex = renderer.GetTexture();

                SetupEffect(monkey1, tex);
            }

            basicEffect = new BasicEffect(GraphicsDevice);
        }

        void SetupEffect(Model model, Texture2D texture)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    BasicEffect b = part.Effect as BasicEffect;
                    b.TextureEnabled = true;
                    b.EnableDefaultLighting();
                    b.LightingEnabled = true;
                    b.SpecularColor = Vector3.One;
                    b.DiffuseColor = Vector3.One;

                    b.Texture = texture;
                }
            }
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
                graphics.IsFullScreen = true;
                graphics.ApplyChanges();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && prevKeyboard.IsKeyUp(Keys.Space))
            {
                exampleNumber++;
            }

            switch (exampleNumber)
            {
                case 4:
                    exampleNumber = 0;
                    break;
                case 0:
                    model = teapot;
                    break;
                case 1:
                    model = teapot1;
                    break;
                case 2:
                    model = monkey;
                    break;
                case 3:
                    model = monkey1;
                    break;
                default:
                    break;
            }

            prevKeyboard = Keyboard.GetState();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //teapotTexturizer.RenderTexture(true, gameTime);
            //tex = teapotTexturizer.GetTextureReference();

            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            // Draw the active model's texture to the screen
            spriteBatch.Draw((model.Meshes[0].Effects[0] as BasicEffect).Texture, new Rectangle(0, 0, 512, 512), Color.White);
            spriteBatch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            Matrix world =
                Matrix.CreateTranslation(0f, -1f, 0f) * 
                Matrix.CreateRotationY(Mouse.GetState().X / 100f) *
                Matrix.CreateRotationX(Mouse.GetState().Y / 100f) * 
                Matrix.CreateScale(0.4f) *
                Matrix.CreateTranslation(1f, 0.0f, 0f);
            Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 3), new Vector3(0,0,0), Vector3.Up);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(0.7f, GraphicsDevice.Viewport.AspectRatio, 0.1f, 200f);

            model.Draw(world, view, projection);


            base.Draw(gameTime);
        }



    }
}
