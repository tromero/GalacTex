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