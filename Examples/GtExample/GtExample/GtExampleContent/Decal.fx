#include "GalacTexContent/GtVertex.fxh"


// properties 

texture decalTex0;
sampler2D decalSampler0 = sampler_state
{
	texture = <decalTex0>;
	AddressU = clamp;
	AddressV = clamp;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

float4x4 decalTransform0;

float4x4 decalUVTransform = 
{
.5,  0, 0, 0,
 0,-.5, 0, 0,
 0,  0, 1, 0,
.5, .5, 0, 1
};

float4 AlphaBlend(float4 src, float4 dst)
{
	float4 result = float4(1,1,1,1);

	result.a = src.a + (dst.a * (1-src.a));
	result.rgb = (src.rgb*src.a + dst.rgb*dst.a*(1 - src.a)) / result.a;

	return result;
}

float4 PremulAlphaBlend(float4 src, float4 dst)
{
	float4 result = float4(0,0,0,0);

	result.a = src.a + (dst.a * (1-src.a));
	result.rgb = src.rgb + (dst.rgb * (1 - src.a));

	return result;
}
                    
float4 PixelShaderFunction(GtPixelInput input) : COLOR0
{
    float4 decalCoord0 = mul(mul(input.position, decalTransform0), decalUVTransform);

	float4 decalCol0 = tex2D(decalSampler0, decalCoord0.xy);
	if (length(saturate(decalCoord0.xyz) - decalCoord0.xyz) != 0)
	{
		decalCol0 = float4(0,0,0,0);
	}
	
	float4 normalCol = (float4(normalize(input.normal.xyz),1) + float4(1,1,1,1)) * 0.5;
	
	float4 result = PremulAlphaBlend(decalCol0, normalCol);

	return result;
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