Shader "Custom/InvisibleOccluder"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry-1" }
        LOD 100

        Blend Zero One

        Pass
        {

        }
    }
}