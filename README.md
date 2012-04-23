GalacTex
========

A procedural texture renderer for XNA Game Studio
--------

GalacTex is a GPU-accelerated texture renderer, inspired by the "Texture Bake" tools in Blender. With it, you can create compact procedural materials using HLSL that are rendered for your models as ordinary 2D textures at run-time.

This project lives at https://github.com/tromero/GalacTex

To use it, you simply create your models in the 3D program of your choice and load it through XNA's Content Pipeline. Your procedural materials are written in HLSL. There are some HLSL functions such as perlin noise included to help with building your effects.

The material descriptions are written in HLSL. Here's an example (taken from the Marble.fx example):

```hlsl
#include "GalacTexContent/GtVertex.fxh"
#include "GalacTexContent/GtNoise.fxh"


float4 PixelShaderFunction(GtPixelInput input) : COLOR0
{
    float noise = RidgedFractalNoise(input.position.xyz * 0.18,6) * 16;
	float marble = .5 + .5 * sin((input.position.x * 10) + noise);

	float plainNoise = (Noise(input.position.xyz) + 1) / 2;

    float4 col = (float4(marble, marble, marble, 1));
	return col;
}

technique Technique1
{
    pass Pass1
    {
        CullMode=NONE;
        
        VertexShader = compile vs_3_0 GtVertexShader();
        PixelShader = compile ps_3_0 PixelShaderFunction();
        
    }
}
```

Setting up the project
--------

- Open your game project, right-click on your solution, and select Add > Existing Project...
- Add the GalacTex project to your game.
- Add the folder GalacTexContent to the root of your game's Content Project. Certain files required by GalacTex must be in this folder.
- Select the .fxh files from GalacTexContent and set their Build Action to None in the Properties pane.


Writing your materials
--------

- Make a copy of the template file template.fx from GalacTexContent in your game's content project.
- (Optional) `#include` any .fxh files such as GtNoise.fxh that you would like to use functions from.
- Write your material description inside the pixel shader function.


Building your Models
--------

Models used with GalacTex must have non-overlapping texture coordinates which do not go outside the 0..1 range of coordinates, or the textures will not render correctly.


Using GalacTex
--------

In `Game.LoadContent()`:
```csharp
GalacTex.TextureRenderer.LoadContent(GraphicsDevice, Content);
Model model = Content.Load<Model>("modelFile");
Effect mat = Content.Load<Effect>("effectFile");

// (optional) Set up any custom effect parameters on your GalacTex Effects

GalacTex.TextureRenderer renderer = new TextureRenderer(model, mat, 1024, 1024);
renderer.RenderTexture();

// copies the texture data from the render target to an ordinary texture. 
// This is done so the contents of the rendered texture are not lost during certain
// graphics device events, such as entering fullscreen mode or minimizing the game.
Texture2D tex = renderer.GetTexture();

// Apply the texture to a model by assigning it to the model's Effect
```

In `Game.Draw()`
- Render your model normally