Shader "GameJam_HIKU/DissolveEffect"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        
        [Header(Dissolve Settings)]
        _DissolveAmount ("Dissolve Amount", Range(0, 1)) = 0
        _NoiseScale ("Noise Scale", Range(0.1, 10)) = 1
        _EdgeWidth ("Edge Width", Range(0, 0.5)) = 0.1
        _EdgeColor ("Edge Color", Color) = (0, 1, 1, 1)
        _EdgeIntensity ("Edge Intensity", Range(0, 5)) = 2
        
        [Header(Digital Glitch)]
        _GlitchStrength ("Glitch Strength", Range(0, 1)) = 0.5
        _PixelSize ("Pixel Size", Range(1, 20)) = 5
        
        [Header(UI Support)]
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }
        
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        
        Pass
        {
            Name "DissolvePass"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 worldPos : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            float4 _MainTex_ST;
            float4 _Color;
            float _DissolveAmount;
            float _NoiseScale;
            float _EdgeWidth;
            float4 _EdgeColor;
            float _EdgeIntensity;
            float _GlitchStrength;
            float _PixelSize;

            // シンプルなランダム関数
            float rand(float2 co)
            {
                return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
            }

            // 左右互い違いの段々パターン（改良版）
            float stepPattern(float2 uv, float progress)
            {
                // より大きなピクセルで明確な段々を作る
                float pixelScale = max(_PixelSize * 0.1, 1.0);
                float2 pixelPos = floor(uv * pixelScale);
                
                // 左右互い違いのオフセット（より明確に）
                float rowOffset = fmod(pixelPos.y, 2.0) * 1.0; // 0.5 → 1.0 でより明確
                float adjustedX = pixelPos.x + rowOffset;
                
                // 段々に進行する（調整済み）
                float stepProgress = progress * (pixelScale * 2 + 5);
                return step(adjustedX, stepProgress);
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.color = input.color * _Color;
                output.worldPos = mul(unity_ObjectToWorld, input.positionOS);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;
                
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) * input.color;
                
                // ピクセルパーフェクトな段々溶解
                float stepDissolve = stepPattern(input.uv, _DissolveAmount);
                
                // エッジの光る効果
                float edge = stepDissolve - stepPattern(input.uv, _DissolveAmount - _EdgeWidth);
                col.rgb += _EdgeColor.rgb * edge * _EdgeIntensity;
                
                // アルファに溶解を適用
                col.a *= stepDissolve;
                
                // 完全に透明な場合は破棄
                if (col.a < 0.01)
                    discard;
                
                return col;
            }
            ENDHLSL
        }
    }
}