Shader "Custom/VolumetricFog"
{   
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
        _MaxDistance("Max Distance", float) = 100.0
        _StepSize("Step size", Range(0.1, 20)) = 1.0
        _DensityMultiplier("Density multiplier", Range(0, 10)) = 1.0
        _NoiseOffset("Noise offset", float) = 0
        _FogNoise("Fog noise", 3D) = "white" {}
        _NoiseTiling("Noise tiling", float) = 1
        _DensityThreshold("Density threshold", Range(0, 1)) = 0.1
    }
    
    SubShader
    {
        // HLSLINCLUDE
        // ENDHLSL

        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 100
        ZWrite Off
        Cull Off

        Pass
        {
            Name "VolumetricFog"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            float4 _Color;
            float _MaxDistance;
            float _StepSize;
            float _DensityMultiplier;
            float _NoiseOffset;
            TEXTURE3D(_FogNoise);
            float _DensityThreshold;
            float _NoiseTiling;

            float get_density(float3 worldPos)
            {
                float4 noise = _FogNoise.SampleLevel(sampler_TrilinearRepeat, worldPos * 0.01 * _NoiseTiling, 0);
                float density = dot(noise, noise);
                density = saturate(density - _DensityThreshold);
                return _DensityMultiplier;
            }

            float4 Frag(Varyings input): SV_Target
            {
                // float2 screenUV = input.texcoord;
                // float depth = SampleSceneDepth(screenUV);
                // float linear01Depth = Linear01Depth(depth, _ZBufferParams);
                // float4 foregroundColor = float4(1.0, 1.0, 1.0, 1.0);
                // float4 backgroundColor = float4(0.0, 0.0, 0.0, 0.0);
                // return lerp(foregroundColor, backgroundColor, linear01Depth);

                float4 col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord);
                float depth = SampleSceneDepth(input.texcoord);
                float3 worldPos = ComputeWorldSpacePosition(input.texcoord, depth, UNITY_MATRIX_I_VP);
                // return float4(frac(worldPos), 1.0);

                float3 entryPoint = _WorldSpaceCameraPos;
                float3 viewDir = worldPos - _WorldSpaceCameraPos;
                float viewLength = length(viewDir);
                float3 rayDir = normalize(viewDir);

                float2 pixelCoords = input.texcoord * _BlitTexture_TexelSize.zw;
                float distLimit = min(viewLength, _MaxDistance);
                float distTravelled = InterleavedGradientNoise(pixelCoords, (int)(_Time.y / max(HALF_EPS, unity_DeltaTime.x))) * _NoiseOffset;
                float transmittance = 1;
                
                while(distTravelled < distLimit)
                {
                    float3 rayPos = entryPoint + rayDir * distTravelled;
                    float density = get_density(rayPos);
                    if (density > 0)
                    {
                        transmittance *= exp(-density * _StepSize);
                    }
                    distTravelled += _StepSize;
                }

                return lerp(col, _Color, 1.0 - saturate(transmittance));
            }
            
            ENDHLSL
        }
    }
}
