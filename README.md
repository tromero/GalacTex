GalacTex
========

A procedural texture renderer for XNA Game Studio

GalacTex is a GPU-accelerated texture renderer, inspired by the "Texture Bake" tools in Blender. With it, you can create compact procedural materials using HLSL that are rendered for your models as ordinary 2D textures at run-time.

To use it, you simply create your models in the 3D program of your choice and load it through XNA's Content Pipeline. Your procedural materials are written in HLSL. There are some HLSL functions such as perlin noise included to help with building your effects.


Setting up the project

- Open your game project, right-click on your solution, and select Add > Existing Project...
- Add the GalacTex project to your game.
- Add the folder GalacTexContent to the root of your game's Content Project. Certain files required by GalacTex must be in this folder.
- Select the .fxh files from GalacTexContent and set their Build Action to None in the Properties pane.


Writing your materials

- Make a copy of the template file template.fx from GalacTexContent in your game's content project.
- (Optional) #include any .fxh files such as GtNoise.fxh that you would like to use functions from.
- Write your material in the pixel shader function.


Building your Models

Models used with GalacTex must have non-overlapping texture coordinates which do not go outside the 0..1 range of coordinates, or the textures will not render correctly.


Using GalacTex

In Game.LoadContent()
- Call GalacTex.TextureRenderer.LoadContent()
- Load your model and GalacTex Effect using the ContentManager.
- Set up any custom effect parameters on your GalacTex Effects
- Create a new TextureRenderer, passing it the Model, Effect, and texture dimensions.
- Call textureRenderer.RenderTexture() to render the model with the given effect.
- Call textureRenderer.GetTexture() to copy the texture data from the render target to an ordinary texture. This is done so the contents of the rendered texture are not lost during certain graphics device events, such as entering fullscreen mode or minimizing the game.
- Apply the texture to a model by assigning it to the model's Effect

In Game.Draw()
- Render your model normally