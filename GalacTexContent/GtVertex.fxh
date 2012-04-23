// Header file required by shaders written for GalacTex
// use #include "GalacTex/GtVertex.fxh"

///////////////////////
// properties /////////
///////////////////////

float GtTime;

// Allows materials to perform conditional rendering 
// based on which part of a model is currently being drawn.
int GtMeshIndex;

float4x4 GtWorld;

// Constant transformation for UVs to fit in rendering space: 
// 2x scale, translated (-1,-1), mirrored Y axis
const float4x4 GtUVTransform = 
{
 2, 0, 0, 0,
 0,-2, 0, 0,
 0, 0, 1, 0,
-1, 1, 0, 1
};

// Used to calculate the half-pixel offset necessary to 
// ensure the sampling locations are correct
float2 GtPixelSize;


struct GtVertexInput
{
    float4 position : POSITION0;
    float4 normal : NORMAL0;
    float4 uv : TEXCOORD0;
};

struct GtPixelInput
{
	// The position of the vertex in UV space, scaled
	// and repositioned to fit within rendering coords
    float4 uvSpace : POSITION0; 
    float4 position : TEXCOORD1;
    float4 uv : TEXCOORD0; // Unmolested UV coords
    float4 normal : TEXCOORD2;
};


GtPixelInput GtVertexShader(GtVertexInput input)
{
    GtPixelInput output;
    
    output.uvSpace = mul(input.uv, GtUVTransform);
	output.uvSpace.xy -= GtPixelSize * 0.5;
    output.position = input.position;
    output.uv = input.uv;
    output.normal = input.normal;
    
    return output;
}