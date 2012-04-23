#include "GtVertex.fxh"

                    
float4 PixelShaderFunction(GtPixelInput input) : COLOR0
{
	float4 normalCol = (float4(normalize(input.normal.xyz),1) + float4(1,1,1,1)) * 0.5;
	
	return normalCol;
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