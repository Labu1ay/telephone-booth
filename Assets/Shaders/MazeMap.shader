Shader "Custom/URP/MazeMap"
{
    Properties
    {
        [MainTexture] _MainTex ("Map Texture", 2D) = "white" {}
        [MainColor] _BaseColor ("Tint Color", Color) = (1,1,1,1)
        _EmissionColor ("Emission (Glow in dark)", Color) = (0.5, 0.5, 0.5, 1)
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
        [Toggle(_ALPHATEST_ON)] _AlphaClip ("Use Alpha Clipping (Cutout)", Float) = 0
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline" 
            "Queue" = "AlphaTest" 
        }

        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma shader_feature_local _ALPHATEST_ON

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
                float fogFactor : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _BaseColor;
                half4 _EmissionColor;
                half _Cutoff;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            Varyings vert (Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                // Вычисляем фактор тумана для URP
                output.fogFactor = ComputeFogFactor(output.positionCS.z);
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                // Семплируем текстуру карты
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                
                // Применяем цветовой тинт
                half3 finalColor = texColor.rgb * _BaseColor.rgb;
                
                // Добавляем свечение (чтобы карту было видно в темноте хоррора)
                finalColor += texColor.rgb * _EmissionColor.rgb;

#if defined(_ALPHATEST_ON)
                // Обрезка прозрачных пикселей (для порванных краев карты)
                clip(texColor.a - _Cutoff);
#endif

                // Применяем туман URP, чтобы карта пропадала в темноте на расстоянии
                finalColor = MixFog(finalColor, input.fogFactor);

                return half4(finalColor, texColor.a * _BaseColor.a);
            }
            ENDHLSL
        }
    }
}