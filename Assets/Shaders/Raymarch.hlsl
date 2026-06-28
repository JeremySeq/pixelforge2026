#include "Assets/Shaders/Voronoi3D.hlsl"

float HenyeyGreenstein(float g, float costh)
{
    return (1.0 - g * g) / (4.0 * PI * pow(1.0 + g * g - 2.0 * g * costh, 3.0 / 2.0));
}

void raymarch_float(float2 screenUV, float3 cameraPos, float depth, float maxDistance, float stepSize, float stepNoise, float noiseScale, float densityMultiplier, float densityThreshold, float time, float deltaTime, out float transmittance)
{
    // float depth = SHADERGRAPH_SAMPLE_SCENE_DEPTH(screenUV);
    // return depth;
    // transmittance = depth;
    float3 worldPos = ComputeWorldSpacePosition(screenUV, depth, UNITY_MATRIX_I_VP);

    float3 entryPoint = cameraPos;
    float3 viewDir = worldPos - cameraPos;
    float viewLength = length(viewDir);
    float3 rayDir = normalize(viewDir);

    float2 pixelCoords = screenUV * _ScreenParams.xy;
    float distLimit = min(viewLength, maxDistance);
    float distTravelled = InterleavedGradientNoise(pixelCoords, (int)(time / max(1e-5, deltaTime))) * stepNoise;
    float t = 1.0;

    while (distTravelled < distLimit)
    {
        float3 rayPos = entryPoint + rayDir * distTravelled;
        // float4 noise = fogNoise.SampleLevel(noiseSampler, rayPos * 0.01 * noiseScale, 0);
        // float density = dot(noise, noise);
        float density;
        float cells;
        voronoi3D_float(rayPos * 0.05, noiseScale, density, cells);
        density = (1.0 - saturate(density - densityThreshold)) * densityMultiplier;
        // density = 0.06;

        if (density > 0)
        {
            t *= exp(-density * stepSize);
        }
        distTravelled += stepSize;
    }

    transmittance = t;
    // transmittance = 0.1;
}
