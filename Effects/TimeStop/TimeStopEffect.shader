Shader "GameJam_HIKU/TimeStopGlitch"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Intensity ("Effect Intensity", Range(0, 1)) = 0
        
        // �J�n���G�t�F�N�g�i��u�̋����m�C�Y�j
        _StartGlitchStrength ("Start Glitch Strength", Range(0, 1)) = 0.8
        
        // �p�����G�t�F�N�g�i�T���߂ȃ`�����j
        _IdleNoiseStrength ("Idle Noise Strength", Range(0, 0.3)) = 0.02
        _IdleFlickerSpeed ("Idle Flicker Speed", Range(0, 50)) = 10
        _IdleFlickerChance ("Idle Flicker Chance", Range(0, 1)) = 0.2
        
        // �F�����i�p�����j
        _TintColor ("Tint Color", Color) = (0.7, 0.85, 1.0, 1.0)
        _DesaturateAmount ("Desaturate Amount", Range(0, 1)) = 0.2
        _Contrast ("Contrast", Range(0.5, 1.5)) = 1.05
        _Brightness ("Brightness", Range(-0.5, 0.5)) = -0.02
        
        // �֊s�G�t�F�N�g
        _VignetteStrength ("Vignette Strength", Range(0, 1)) = 0.2
        _VignetteSoftness ("Vignette Softness", Range(0.1, 1)) = 0.7
        _VignetteColor ("Vignette Color", Color) = (0.3, 0.4, 0.5, 1.0)
        
        // �X�L�������C���i���߁j
        _ScanLineAlpha ("Scan Line Alpha", Range(0, 0.5)) = 0.05
        _ScanLineSpeed ("Scan Line Speed", Range(0, 10)) = 2
        _ScanLineFrequency ("Scan Line Frequency", Range(50, 500)) = 150
        
        // �A�j���[�V��������i�����p�j
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
            
            // �ȈՃn�b�V���֐�
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
                // C#���瑗���Ă��鎞�Ԃ��g�p�iTimeScale=0�ł������j
                float animTime = _AnimationTime;
                
                // �G�t�F�N�g���x��0�Ȃ猳�摜�����̂܂ܕԂ�
                if (_Intensity <= 0.001)
                {
                    return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                }
                
                // �G�t�F�N�g�̓K�p���x�i0.2���x��z��j
                float effectStrength = _Intensity;
                
                // �J�n���̋����O���b�`�iAnimationTime�Ő���j
                float startGlitch = 0;
                float timeSinceStart = _AnimationTime;
                if (timeSinceStart < 0.3) // �J�n����0.3�b��
                {
                    float startProgress = timeSinceStart / 0.3;
                    startGlitch = (1.0 - startProgress) * _StartGlitchStrength * effectStrength;
                    
                    // �J�n���̂݋����c��
                    float glitchLine = hash(float2(floor(uv.y * 20.0), floor(animTime * 30.0)));
                    if (glitchLine > 0.7)
                    {
                        uv.x += (hash(float2(uv.y, animTime)) - 0.5) * 0.15 * startGlitch;
                    }
                }
                
                // �p�����̍T���߂ȃ`�����i��ɓ����j
                float idleFlicker = 0;
                {
                    // ��ɃA�j���[�V�������闐��
                    float flickerTime = animTime * _IdleFlickerSpeed;
                    float flickerSeed = floor(flickerTime);
                    float flickerRandom = hash(float2(flickerSeed, 0));
                    
                    // �`�����̔����m��
                    if (flickerRandom < _IdleFlickerChance)
                    {
                        // ��ʂ̈ꕔ�����Ƀm�C�Y�i���C���P�ʁj
                        float lineY = floor(uv.y * 40.0 + flickerTime) / 40.0;
                        float lineNoise = hash(float2(lineY, flickerSeed));
                        
                        if (lineNoise > 0.6)
                        {
                            idleFlicker = _IdleNoiseStrength * effectStrength;
                            // �������̔��ׂȘc��
                            float distortion = (hash(float2(uv.y * 20.0, flickerTime)) - 0.5);
                            uv.x += distortion * idleFlicker;
                        }
                    }
                    
                    // �ǉ��F�����_���ȏc���C���̃`����
                    float verticalLine = floor(uv.x * 60.0);
                    float verticalNoise = hash(float2(verticalLine, floor(animTime * 15.0)));
                    if (verticalNoise > 0.95)
                    {
                        uv.x += (hash(float2(animTime, verticalLine)) - 0.5) * 0.002 * effectStrength;
                    }
                }
                
                // �e�N�X�`���T���v�����O
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                
                // �J�n���̐F����
                if (startGlitch > 0)
                {
                    float2 rOffset = float2(startGlitch * 0.008, 0);
                    float2 bOffset = float2(-startGlitch * 0.008, 0);
                    
                    half4 colorR = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + rOffset);
                    half4 colorB = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + bOffset);
                    
                    color.r = colorR.r;
                    color.b = colorB.b;
                }
                
                // �p�����̐F�����ieffectStrength�Œ����j
                {
                    // �ʓx������������
                    float3 grayscale = dot(color.rgb, float3(0.299, 0.587, 0.114));
                    color.rgb = lerp(color.rgb, grayscale, _DesaturateAmount * effectStrength);
                    
                    // �݂��������e�B���g
                    color.rgb = lerp(color.rgb, color.rgb * _TintColor.rgb, effectStrength * 0.5);
                    
                    // �R���g���X�g�Ɩ��x�����i�T���߂Ɂj
                    color.rgb = lerp(color.rgb, 
                                   (color.rgb - 0.5) * _Contrast + 0.5 + _Brightness,
                                   effectStrength);
                    
                    // �����X�L�������C��
                    float scanLine = sin((uv.y * _ScanLineFrequency) - (animTime * _ScanLineSpeed));
                    scanLine = smoothstep(0.8, 1.0, abs(scanLine));
                    color.rgb *= 1.0 - (scanLine * _ScanLineAlpha * effectStrength);
                    
                    // �ǉ��F���ׂ�RGB�V�t�g�i��ɓ����j
                    float rgbShift = sin(animTime * 3.0 + uv.y * 10.0) * 0.001 * effectStrength;
                    color.r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(rgbShift, 0)).r;
                    
                    // �r�l�b�g����
                    float2 vignetteUV = uv - 0.5;
                    float vignetteDist = length(vignetteUV) * 1.4142;
                    float vignette = smoothstep(1.0 - _VignetteSoftness, 1.0, vignetteDist);
                    color.rgb = lerp(color.rgb, color.rgb * _VignetteColor.rgb, 
                                   vignette * _VignetteStrength * effectStrength);
                }
                
                // �`�����ɂ�閾�x�ω��i���I�j
                if (idleFlicker > 0)
                {
                    float brightness = hash2(float2(animTime * 100.0, uv.y)) * idleFlicker;
                    color.rgb = saturate(color.rgb + brightness * 0.3);
                }
                
                // �J�n���̌������m�C�Y
                if (startGlitch > 0)
                {
                    float noise = hash2(uv * 100.0 + animTime * 50.0);
                    color.rgb = lerp(color.rgb, float3(noise, noise, noise), startGlitch * 0.2);
                }
                
                // �ǉ��F���ׂȃs�N�Z���m�C�Y�i�펞�j
                float pixelNoise = hash2(uv * 500.0 + animTime * 10.0);
                color.rgb += (pixelNoise - 0.5) * 0.01 * effectStrength;
                
                return color;
            }
            
            ENDHLSL
        }
    }
    
    FallBack "Sprites/Default"
}