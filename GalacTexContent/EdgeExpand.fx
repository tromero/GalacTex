
// Expands the edges of a texture (using magenta key) so that UV seams aren't visible, even at high mip mapping levels

sampler s0;

const float4 key = float4(1, 0, 1, 1);

bool IsKey(float4 col)
{
	return col.r == key.r &&
	       col.g == key.g &&
	       col.b == key.b &&
	       col.a == key.a;
}

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
	float4 resultCol = tex2D(s0, uv);
	bool exit = false;
	if (IsKey(resultCol))
	{
		discard;
	}

    return resultCol;
}

technique Technique1
{
    pass Pass1
    {
		PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
