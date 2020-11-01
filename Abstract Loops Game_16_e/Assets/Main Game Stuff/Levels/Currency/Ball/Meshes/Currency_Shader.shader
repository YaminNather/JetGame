Shader "Unlit/Currency_Shader"
{
    Properties
    {
        _ColorMap("Color Map", 2D) = "white" {}
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}


            Pass
            {
                Name "TestPass"

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
                    float4 positionHCS : SV_POSITION;
                    float2 uv : TEXCOORD0;
                };

                half _Hue0;
                TEXTURE2D(_ColorMap); SAMPLER(sampler_ColorMap);
                CBUFFER_START(UnityPerMaterial)
                half4 _ColorMap_ST;
                CBUFFER_END

                half3 HSVToRGBConvert_F(half3 In)
                {
                    half4 K = half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                    half3 P = abs(frac(In.xxx + K.xyz) * 6.0 - K.www);
                    return In.z * lerp(K.xxx, saturate(P - K.xxx), In.y);
                }

                Varyings vert(Attributes IN)
                {
                    Varyings OUT;
                    OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                    OUT.uv = TRANSFORM_TEX(IN.uv, _ColorMap);
                    //OUT.uv = IN.uv + float2(2, 2);

                    return OUT;
                }

                half4 frag(Varyings IN) : SV_Target
                {
                    half4 sampledColor = SAMPLE_TEXTURE2D(_ColorMap, sampler_ColorMap, IN.uv);
                    half3 hueColor = HSVToRGBConvert_F(half3(_Hue0, 1.0, 1.0));
                    half3 lerpedColor = lerp(hueColor, half3(1, 1, 1), sampledColor);
                    half4 colorWithAlpha = half4(lerpedColor.x, lerpedColor.y, lerpedColor.z, 1.0);
                    return sampledColor * colorWithAlpha;
                }
                ENDHLSL
            }
        }
}
