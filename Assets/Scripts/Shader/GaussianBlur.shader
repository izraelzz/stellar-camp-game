Shader "Custom/PhotoshopBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Radius ("Blur Radius", Range(0, 20)) = 5
        _Resolution ("Resolution Scale", Range(0.1, 1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }

        Pass
        {
            ZWrite Off
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
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;
            float _Radius;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS);
                o.uv = v.uv;
                return o;
            }


            float GaussianWeight(float x, float sigma)
            {
                return exp(-(x * x) / (2.0 * sigma * sigma));
            }

            half4 frag (Varyings i) : SV_Target
            {
                float2 uv = i.uv;
                half4 color = 0;
                float totalWeight = 0;
                

                float sigma = max(_Radius, 0.01); 
                

                float2 texelSize = _MainTex_TexelSize.xy;


                for (int x = -4; x <= 4; x++)
                {
                    for (int y = -4; y <= 4; y++)
                    {
                        float2 offset = float2(x, y) * texelSize * (_Radius * 0.5);
                        float weight = GaussianWeight(length(float2(x, y)), 1.5);
                        
                        color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + offset) * weight;
                        totalWeight += weight;
                    }
                }

                return color / totalWeight;
            }
            ENDHLSL
        }
    }
}