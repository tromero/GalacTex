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

namespace GalacTex
{
    public class TextureRenderer : IDisposable
    {
        #region constants

        // Resolution of the 3D noise texture (n*n*n resolution)
        const int NOISE_TEXTURE_RESOLUTION = 32;
        const string UNINITIALIZED_EXCEPTION = "You must call the static TextureRenderer.LoadContent method before creating any renderers.";

        // Number of cycles of the noise pattern in the texture. 
        // Lower values smoother but more repetitive
        const int NOISE_PERIOD = 5;

        #endregion

        #region instance variables
        // Stores the rendered texture for later retrieval
        RenderTarget2D renderTarget;
        // Allows targets to be swapped in order to 
        RenderTarget2D swapTarget;

        public Model Model { get; set; }

        #endregion

        #region static variables
        // The basis for noise lookups in materials. This is an intensive thing to generate,
        // so it is only done once for all materials
        private static Texture3D noiseTexture;

        // Used for rendering and postprocessing
        private static SpriteBatch spriteBatch;
        private static GraphicsDevice graphicsDevice;
        private static ContentManager content;
        private static Effect edgeExpand;

        private enum InitState { NotInitialized, InProgress, Initialized }
        private static InitState initialized = InitState.NotInitialized;

        /// <summary>
        /// Called when the TextureRenderer module finishes loading content
        /// </summary>
        public static event Action ContentLoaded;

        #endregion

        #region instance methods

        /// <summary>
        /// Used to render 2D textures which fit the given UV-unwrapped model.
        /// </summary>
        /// <param name="targetModel"></param>
        /// <param name="material"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public TextureRenderer(Model targetModel, Effect material, int width, int height)
        {
            if (initialized != InitState.Initialized)
            {
                throw new Exception(UNINITIALIZED_EXCEPTION);
            }

            renderTarget = new RenderTarget2D(graphicsDevice,
                width, height, true, SurfaceFormat.Color, DepthFormat.None);
            swapTarget = new RenderTarget2D(graphicsDevice,
                width, height, false, SurfaceFormat.Color, DepthFormat.None);

            Model = targetModel;
            Material = material;
        }

        public void Dispose()
        {
            renderTarget.Dispose();
            swapTarget.Dispose();
        }

        ~TextureRenderer()
        {
            Dispose();
        }

        static Vector2[] edgeExpandPositions = 
        { 
            new Vector2( 1, 1),
            new Vector2(-1, 1),
            new Vector2( 1,-1),
            new Vector2(-1,-1),
            new Vector2( 1, 0),
            new Vector2( 0, 1),
            new Vector2(-1, 0),
            new Vector2( 0,-1),
        };

        /// <summary>
        /// Renders the material for the supplied model and stores the result
        /// </summary>
        public void RenderTexture()
        {
            if (initialized != InitState.Initialized)
            {
                throw new Exception(UNINITIALIZED_EXCEPTION);
            }

            if (renderTarget.IsDisposed)
            {
                renderTarget = new RenderTarget2D(
                    graphicsDevice,
                    renderTarget.Width,
                    renderTarget.Height,
                    true,
                    renderTarget.Format,
                    renderTarget.DepthStencilFormat);
            }
            if (swapTarget.IsDisposed)
            {
                swapTarget = new RenderTarget2D(
                    graphicsDevice,
                    swapTarget.Width,
                    swapTarget.Height,
                    false,
                    swapTarget.Format,
                    swapTarget.DepthStencilFormat);
            }

            // Set a render target
            graphicsDevice.SetRenderTarget(swapTarget);

            // Clear the texture to something that can be easily keyed in postprocessing
            graphicsDevice.Clear(new Color(1f, 0f, 1f, 1f));

            // Set GalacTex-specific effect parameters
            EffectParameter noiseTextureParam = Material.Parameters["GtTexNoise"];
            if (noiseTextureParam != null)
            {
                noiseTextureParam.SetValue(noiseTexture);
            }
            Vector2 pixelSize = new Vector2(2f / renderTarget.Width, -2f / renderTarget.Height);
            Material.Parameters["GtPixelSize"].SetValue(pixelSize);

            Matrix[] boneTransforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            for (int i = 0; i < Model.Meshes.Count; i++)
            {
                Effect[] savedEffects = new Effect[Model.Meshes[i].MeshParts.Count];

                for (int j = 0; j < Model.Meshes[i].MeshParts.Count; j++)
                {
                    ModelMeshPart part = Model.Meshes[i].MeshParts[j];
                    savedEffects[j] = part.Effect;
                    part.Effect = Material;
                }

                // Set up and draw with the custom effect
                Material.Parameters["GtWorld"].SetValue(boneTransforms[Model.Meshes[i].ParentBone.Index]);
                Material.Parameters["GtMeshIndex"].SetValue(i);
                Model.Meshes[i].Draw();

                // Leave the model as we found it
                for (int j = 0; j < Model.Meshes[i].MeshParts.Count; j++)
                {
                    ModelMeshPart part = Model.Meshes[i].MeshParts[j];
                    part.Effect = savedEffects[j];
                }
            }


            // swap targets, redraw texture to main target
            // Draw using a custom effect that keys on pure magenta 
            // expands edges of texture outward to improve seams
            graphicsDevice.SetRenderTarget(renderTarget);
            graphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, null, null, null, edgeExpand);

            // cover up seams
            {
                for (int i = 16; i > 0; i--)
                {
                    foreach (Vector2 dir in edgeExpandPositions)
                    {
                        Vector2 drawPosition = dir * i;
                        spriteBatch.Draw(swapTarget, drawPosition, Color.White);
                    }
                }
            }

            spriteBatch.Draw(swapTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            // Finished drawing to render target, reset target to backbuffer
            graphicsDevice.SetRenderTarget(null);

        }

        public Effect Material
        {
            set;
            get;
        }

        /// <summary>
        /// Use this method to retrieve a texture that has been (or will be) rendered by this material. 
        /// This texture will be modified every time RenderTexture() is called, and is volatile, so may 
        /// be replaced.
        /// </summary>
        /// <returns>A reference to the texture that this material outputs to</returns>
        public RenderTarget2D GetRenderTarget()
        {
            return renderTarget;
        }

        /// <summary>
        /// Creates a non-volatile copy of the render result
        /// </summary>
        /// <returns>A new (unmanaged) texture with the contents of the render target</returns>
        public Texture2D GetTexture()
        {
            Texture2D copy = new Texture2D(graphicsDevice,
                renderTarget.Width, renderTarget.Height, true,
                renderTarget.Format);

            // Set data for each mip map level
            for (int i = 0; i < renderTarget.LevelCount; i++)
            {
                // calculate the dimensions of the mip level.
                int width = (int)Math.Max((renderTarget.Width / Math.Pow(2, i)), 1);
                int height = (int)Math.Max((renderTarget.Height / Math.Pow(2, i)), 1);

                Color[] data = new Color[width * height];

                renderTarget.GetData<Color>(i, null, data, 0, data.Length);
                copy.SetData<Color>(i, null, data, 0, data.Length);
            }

            return copy;
        }

        #endregion

        #region static methods

        /// <summary>
        /// Generates the noise table which may be used by materials. 
        /// 
        /// Since this is time-intensive, it is recommended that this method be run in a 
        /// non-GUI thread during a loading screen or during startup.
        /// </summary>
        /// <param name="device">The device which will be used to render textures.</param>
        public static void LoadContent(GraphicsDevice device, ContentManager contentManager)
        {
            initialized = InitState.InProgress;

            graphicsDevice = device;
            spriteBatch = new SpriteBatch(graphicsDevice);
            content = contentManager;
            edgeExpand = content.Load<Effect>("GalacTexContent/EdgeExpand");

            // Generate noise volume which may be used by instances
            GenerateNoiseTexture(NOISE_TEXTURE_RESOLUTION, NOISE_PERIOD);

            initialized = InitState.Initialized;

            if (ContentLoaded != null)
            {
                ContentLoaded();
            }
        }

        /// <summary>
        /// Unloads all static assets.
        /// </summary>
        public static void Unload()
        {
            noiseTexture.Dispose();
            graphicsDevice = null;
            spriteBatch.Dispose();

            initialized = InitState.NotInitialized;
        }
        
        protected static byte SampleNoise(float x, float y, float z, int period, int offset)
        {
            // add one then divide by two to get in 0..1 range, multiply by 255 to get byte value
            return (byte)((TilingPerlinNoise.Noise(new Vector3(x,y,z), period, offset) + 1) * 127.5f );
        }

        protected static void GenerateNoiseTexture(int resolution, int period)
        {
            // Set up Texture3D for noise sampling
            noiseTexture = new Texture3D(graphicsDevice,
                resolution, resolution, resolution,
                false, SurfaceFormat.Color);

            // Fill a Resolution^3 array with values generated using smooth noise
            // Uses smooth noise instead of pure random numbers because linear filtering 
            // on random values doesn't provide a smooth or consistent enough gradient
            Color[] noise = new Color[(int)Math.Pow(resolution, 3)];
            int index = 0;
            // how many often does the pattern "cycle" over the texture?
            float inverseFrequency = (float)period / resolution;

            for (int i = 0; i < resolution; i++)
                for (int j = 0; j < resolution; j++)
                    for (int k = 0; k < resolution; k++)
                    {
                        float x = i * inverseFrequency;
                        float y = j * inverseFrequency;
                        float z = k * inverseFrequency;

                        // sample different tiling noise domains for each color channel
                        noise[index].R = SampleNoise(x, y, z, period, 0);
                        noise[index].G = SampleNoise(x, y, z, period, 50);

                        index++;
                    }

            noiseTexture.SetData<Color>(noise);
        }
        #endregion

    }
}
