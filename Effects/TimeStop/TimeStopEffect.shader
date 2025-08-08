Shader "GameJam_HIKU/TimeStopGlitch"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Size("Glitch Size", Float) = 1
        _Intensity("Glitch Intensity", Range(0, 1)) = 0
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }
        
        Cull Off ZWrite Off ZTest Always
 
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
 
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
            };
 
            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }
 
            float rand(float3 co)
            {
                return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
            }
 
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float _Size;
            float _Intensity;
 
            static const int blackheight = 1;
            static const float division = 768;
            static const float blackinterval = 6;
            static const float noisewidth = 0.01;
 
            half4 frag(Varyings input) : SV_Target
            {
                // IntensityÇ™0Ç»ÇÁí èÌÇÃï`âÊ
                if (_Intensity <= 0.0)
                {
                    return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                }
                
                int divisionindex = input.uv.y * division;
                int noiseindex = divisionindex / blackinterval;
 
                float3 timenoise = float3(0, int(_Time.x * 61), int(_Time.x * 83));
                float noiserate = rand(timenoise) < 0.05 ? 10 : 1;
 
                float xnoise = rand(float3(noiseindex, 0, 0) + timenoise);
                xnoise = xnoise * xnoise - 0.5;
                xnoise = xnoise * noisewidth * noiserate;
                xnoise = xnoise * (_SinTime.w / 2 + 1.1);
                xnoise = xnoise + (abs((int(_Time.x * 2000) % int(division / blackinterval)) - noiseindex) < 5 ? 0.005 : 0);
 
                float2 uv = input.uv + float2(xnoise * _Size * _Intensity, 0);
 
                half4 col1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                half4 col2 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(0.005, 0));
                half4 col3 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-0.005, 0));
                half4 col4 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(0, 0.005));
                half4 col5 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(0, -0.005));
                half4 col = (col1 * 4 + col2 + col3 + col4 + col5) / 8;
 
                // çïÇ¢ê¸ÇIntensityÇ≈êßå‰
                col.rgb = (divisionindex % blackinterval < blackheight && _Intensity > 0.5) ? half3(0, 0, 0) : col.rgb;
                
                return col;
            }
            ENDHLSL
        }
    }
}