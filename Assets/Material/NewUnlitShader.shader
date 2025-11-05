Shader "Unlit/NewUnlitShader"
{
     Properties
    {
        _MainTex ("Noise Texture", 2D) = "white" {}
        _Tint ("Electric Color", Color) = (0.2, 0.9, 1, 1)
        _Intensity ("Glow Intensity", Float) = 3
        _Speed ("Scroll Speed", Float) = 1.5
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend One One        
        ZWrite Off          
        Cull Off

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;

            float4 _Tint;
            float _Intensity;
            float _Speed;

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

            Varyings vert (Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                float2 uv = TRANSFORM_TEX(input.uv, _MainTex);
                uv += float2(_Time.y * _Speed, _Time.y * _Speed * 0.5); 
                output.uv = uv;
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                half4 noise = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                half glow = noise.r * _Intensity;
                return half4(_Tint.rgb * glow, glow); 
            }

            ENDHLSL
        }
    }
}
