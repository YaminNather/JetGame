Shader "Custom/Emission_Shader_1"
{
    Properties
    {
        _WireframeTexture("Wireframe Texture", 2D) = "white" {}
        _EmissionStrength("_Emission Strength", float) = 1.0
        _SecondaryColor("_Secondary Color", Color) = (0.0, 0.0, 0.0, 1.0)
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
            
            //Global Variables
            half _Hue0;
            half3 _PrimaryColor;
            TEXTURE2D(_WireframeTexture); SAMPLER(sampler_WireframeTexture);

            //Local Variables
            CBUFFER_START(UnityPerMaterial)
            half _EmissionStrength;
            half4 _WireframeTexture_ST;
            half4 _SecondaryColor;
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
                OUT.uv = TRANSFORM_TEX(IN.uv, _WireframeTexture);
                //OUT.uv = IN.uv + float2(2, 2);

                return OUT;
            }

            half3 frag(Varyings IN) : SV_Target
            {
                half4 sampledColor = SAMPLE_TEXTURE2D(_WireframeTexture, sampler_WireframeTexture, IN.uv);
                //half3 hueColor = HSVToRGBConvert_F(half3(_Hue0, 1.0, 1.0));
                //half3 lerpedColor = lerp(_SecondaryColor, hueColor * _EmissionStrength, sampledColor);
                half3 lerpedColor = lerp(_SecondaryColor, _PrimaryColor * _EmissionStrength, sampledColor);
                return lerpedColor;
            }
            ENDHLSL
        }
    }
}
