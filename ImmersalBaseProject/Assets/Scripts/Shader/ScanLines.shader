Shader "Hidden/Custom/Scanlines"
{
    HLSLINCLUDE

#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    int _Width;
    int _RefScreenHeight;
    float4 _LineColor;

    //float fmin = 0.0;

    float4 Frag(VaryingsDefault i) : SV_Target
    {
        float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);

        int y = round(i.texcoord.y * _RefScreenHeight);
        float fstep = (y % _Width) / float(_Width - 1);
        //float fmod = (i.texcoord.y % _Width);
        //float fstep = fmin + (1.0 - fmin) * fmod;

        color.rgb = lerp(color.rgb, _LineColor.rgb, fstep * _LineColor.a);

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