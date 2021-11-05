Shader "Hidden/Custom/ScanLines"
{
    HLSLINCLUDE

#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    float _Blend;
    float _Width;
    fixed4 _LineColor;

    float fmin = 0.7;

    float4 Frag(VaryingsDefault i) : SV_Target
    {
        float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);

        float fmod = mod(i.texcoord.y, _Width);
        float fstep = fmin + (1.0 - fmin) * fmod;

        return color;
    }

        ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

            Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}