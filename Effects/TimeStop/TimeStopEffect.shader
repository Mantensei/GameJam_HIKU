Shader "GameJam_HIKU/TimeStopGlitch"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Intensity ("Effect Intensity", Range(0, 1)) = 0
        
        // 開始時エフェクト（一瞬の強いノイズ）
        _StartGlitchStrength ("Start Glitch Strength", Range(0, 1)) = 0.8
        
        // 継続中エフェクト（控えめなチラつき）
        _IdleNoiseStrength ("Idle Noise Strength", Range(0, 0.3)) = 0.02
        _IdleFlickerSpeed ("Idle Flicker Speed", Range(0, 50)) = 10
        _IdleFlickerChance ("Idle Flicker Chance", Range(0, 1)) = 0.2
        
        // 色調整（継続中）
        _TintColor ("Tint Color", Color) = (0.7, 0.85, 1.0, 1.0)
        _DesaturateAmount ("Desaturate Amount", Range(0, 1)) = 0.2
        _Contrast ("Contrast", Range(0.5, 1.5)) = 1.05
        _Brightness ("Brightness", Range(-0.5, 0.5)) = -0.02
        
        // 輪郭エフェクト
        _VignetteStrength ("Vignette Strength", Range(0, 1)) = 0.2
        _VignetteSoftness ("Vignette Softness", Range(0.1, 1)) = 0.7
        _VignetteColor ("Vignette Color", Color) = (0.3, 0.4, 0.5, 1.0)
        
        // スキャンライン（薄め）
        _ScanLineAlpha ("Scan Line Alpha", Range(0, 0.5)) = 0.05
        _ScanLineSpeed ("Scan Line Speed", Range(0, 10)) = 2
        _ScanLineFrequency ("Scan Line Frequency", Range(50, 500)) = 150
        
        // アニメーション制御（内部用）
        _AnimationTime ("Animation Time", Float) = 0
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }
        
        Pass
        {
            Name "TimeStopGlitch"
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            
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
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _Intensity;
                float _StartGlitchStrength;
                float _IdleNoiseStrength;
                float _IdleFlickerSpeed;
                float _IdleFlickerChance;
                float4 _TintColor;
                float _DesaturateAmount;
                float _Contrast;
                float _Brightness;
                float _VignetteStrength;
                float _VignetteSoftness;
                float4 _VignetteColor;
                float _ScanLineAlpha;
                float _ScanLineSpeed;
                float _ScanLineFrequency;
                float _AnimationTime;
            CBUFFER_END
            
            // 簡易ハッシュ関数
            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }
            
            float hash2(float2 p)
            {
                float3 p3 = frac(float3(p.xyx) * 0.1031);
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.x + p3.y) * p3.z);
            }
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;
                // C#から送られてくる時間を使用（TimeScale=0でも動く）
                float animTime = _AnimationTime;
                
                // エフェクト強度が0なら元画像をそのまま返す
                if (_Intensity <= 0.001)
                {
                    return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                }
                
                // エフェクトの適用強度（0.2程度を想定）
                float effectStrength = _Intensity;
                
                // 開始時の強いグリッチ（AnimationTimeで制御）
                float startGlitch = 0;
                float timeSinceStart = _AnimationTime;
                if (timeSinceStart < 0.3) // 開始から0.3秒間
                {
                    float startProgress = timeSinceStart / 0.3;
                    startGlitch = (1.0 - startProgress) * _StartGlitchStrength * effectStrength;
                    
                    // 開始時のみ強い歪み
                    float glitchLine = hash(float2(floor(uv.y * 20.0), floor(animTime * 30.0)));
                    if (glitchLine > 0.7)
                    {
                        uv.x += (hash(float2(uv.y, animTime)) - 0.5) * 0.15 * startGlitch;
                    }
                }
                
                // 継続中の控えめなチラつき（常に動く）
                float idleFlicker = 0;
                {
                    // 常にアニメーションする乱数
                    float flickerTime = animTime * _IdleFlickerSpeed;
                    float flickerSeed = floor(flickerTime);
                    float flickerRandom = hash(float2(flickerSeed, 0));
                    
                    // チラつきの発生確率
                    if (flickerRandom < _IdleFlickerChance)
                    {
                        // 画面の一部だけにノイズ（ライン単位）
                        float lineY = floor(uv.y * 40.0 + flickerTime) / 40.0;
                        float lineNoise = hash(float2(lineY, flickerSeed));
                        
                        if (lineNoise > 0.6)
                        {
                            idleFlicker = _IdleNoiseStrength * effectStrength;
                            // 横方向の微細な歪み
                            float distortion = (hash(float2(uv.y * 20.0, flickerTime)) - 0.5);
                            uv.x += distortion * idleFlicker;
                        }
                    }
                    
                    // 追加：ランダムな縦ラインのチラつき
                    float verticalLine = floor(uv.x * 60.0);
                    float verticalNoise = hash(float2(verticalLine, floor(animTime * 15.0)));
                    if (verticalNoise > 0.95)
                    {
                        uv.x += (hash(float2(animTime, verticalLine)) - 0.5) * 0.002 * effectStrength;
                    }
                }
                
                // テクスチャサンプリング
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                
                // 開始時の色収差
                if (startGlitch > 0)
                {
                    float2 rOffset = float2(startGlitch * 0.008, 0);
                    float2 bOffset = float2(-startGlitch * 0.008, 0);
                    
                    half4 colorR = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + rOffset);
                    half4 colorB = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + bOffset);
                    
                    color.r = colorR.r;
                    color.b = colorB.b;
                }
                
                // 継続中の色調整（effectStrengthで調整）
                {
                    // 彩度を少し下げる
                    float3 grayscale = dot(color.rgb, float3(0.299, 0.587, 0.114));
                    color.rgb = lerp(color.rgb, grayscale, _DesaturateAmount * effectStrength);
                    
                    // 青みがかったティント
                    color.rgb = lerp(color.rgb, color.rgb * _TintColor.rgb, effectStrength * 0.5);
                    
                    // コントラストと明度調整（控えめに）
                    color.rgb = lerp(color.rgb, 
                                   (color.rgb - 0.5) * _Contrast + 0.5 + _Brightness,
                                   effectStrength);
                    
                    // 動くスキャンライン
                    float scanLine = sin((uv.y * _ScanLineFrequency) - (animTime * _ScanLineSpeed));
                    scanLine = smoothstep(0.8, 1.0, abs(scanLine));
                    color.rgb *= 1.0 - (scanLine * _ScanLineAlpha * effectStrength);
                    
                    // 追加：微細なRGBシフト（常に動く）
                    float rgbShift = sin(animTime * 3.0 + uv.y * 10.0) * 0.001 * effectStrength;
                    color.r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(rgbShift, 0)).r;
                    
                    // ビネット効果
                    float2 vignetteUV = uv - 0.5;
                    float vignetteDist = length(vignetteUV) * 1.4142;
                    float vignette = smoothstep(1.0 - _VignetteSoftness, 1.0, vignetteDist);
                    color.rgb = lerp(color.rgb, color.rgb * _VignetteColor.rgb, 
                                   vignette * _VignetteStrength * effectStrength);
                }
                
                // チラつきによる明度変化（動的）
                if (idleFlicker > 0)
                {
                    float brightness = hash2(float2(animTime * 100.0, uv.y)) * idleFlicker;
                    color.rgb = saturate(color.rgb + brightness * 0.3);
                }
                
                // 開始時の激しいノイズ
                if (startGlitch > 0)
                {
                    float noise = hash2(uv * 100.0 + animTime * 50.0);
                    color.rgb = lerp(color.rgb, float3(noise, noise, noise), startGlitch * 0.2);
                }
                
                // 追加：微細なピクセルノイズ（常時）
                float pixelNoise = hash2(uv * 500.0 + animTime * 10.0);
                color.rgb += (pixelNoise - 0.5) * 0.01 * effectStrength;
                
                return color;
            }
            
            ENDHLSL
        }
    }
    
    FallBack "Sprites/Default"
}