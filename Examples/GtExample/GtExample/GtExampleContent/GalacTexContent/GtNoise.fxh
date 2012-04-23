// Optional methods for the use of noise in GalacTex shaders

#define PI 3.141592653589793238462643383279;

texture GtTexNoise;
sampler3D GtSampNoise = sampler_state
{
	texture = <GtTexNoise>;
	AddressU = wrap;
	AddressV = wrap;
	AddressW = wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

float SimpleNoise(float3 pos)
{
    return tex3D(GtSampNoise, pos).r; 
}

// Uses the less-repetitive noise technoque described in:
// http://http.developer.nvidia.com/GPUGems2/gpugems2_chapter26.html
float Noise(float3 pos)
{
	float2 hi = tex3D(GtSampNoise, pos).rg*2-1; // High frequency noise  
	half   lo = tex3D(GtSampNoise, pos/9).r*2-1; // Low frequency noise  
	
	half  angle = lo*2.0*PI;  
	float result = hi.r * cos(angle) + hi.g * sin(angle); // Use the low frequency as a quaternion rotation of the high-frequency's real and imaginary parts.  
	return result; // done!
}

float RidgedNoise(float3 pos)
{
	return abs(Noise(pos));
}

float FractalNoise(float3 pos, int octaves)
{
	float result = 0;
    float divisor = 0;
    for (int i = 1; i <= octaves; i++)
    {
        result += Noise(pos * pow (2,i)) / i;
        divisor += 1.0 / i;
    }
    
    return result / divisor;
}

float RidgedFractalNoise(float3 pos, int octaves)
{
	float result = 0;
    float divisor = 0;
    for (int i = 1; i <= octaves; i++)
    {
        result += RidgedNoise(pos * pow (2,i)) / i;
        divisor += 1.0 / i;
    }
    
    return result / divisor;
}