// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Wireframe"
{
    Properties
    {
        _WireColor("WireColor", Color) = (1,0,0,1)
        _Color("Color", Color) = (1,1,1,1)

        _Freq("Frequency", float) = 2
        _Speed("Speed", float) = 2
    }
        SubShader
    {
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM

            #include "UnityCG.cginc"
            #pragma target 5.0
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag

            half4 _WireColor, _Color;

            float _Freq, _Speed;

            struct v2g
            {
                float4  pos : SV_POSITION;
                float2  uv : TEXCOORD0;
                float4  worldPos : TEXCOORD1;
            };

            struct g2f
            {
                float4  pos : SV_POSITION;
                float2  uv : TEXCOORD0;
                float3 dist : TEXCOORD1;
                float4  worldPos : TEXCOORD2;
            };

            v2g vert(appdata_base v)
            {
                v2g OUT;
                OUT.pos = UnityObjectToClipPos(v.vertex);
                OUT.worldPos = mul(unity_ObjectToWorld, v.vertex);
                OUT.uv = v.texcoord;
                return OUT;
            }

            [maxvertexcount(3)]
            void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream)
            {

                float2 WIN_SCALE = float2(_ScreenParams.x / 2.0, _ScreenParams.y / 2.0);

                //frag position
                float2 p0 = WIN_SCALE * IN[0].pos.xy / IN[0].pos.w;
                float2 p1 = WIN_SCALE * IN[1].pos.xy / IN[1].pos.w;
                float2 p2 = WIN_SCALE * IN[2].pos.xy / IN[2].pos.w;

                //barycentric position
                float2 v0 = p2 - p1;
                float2 v1 = p2 - p0;
                float2 v2 = p1 - p0;
                //triangles area
                float area = abs(v1.x * v2.y - v1.y * v2.x);

                g2f OUT;
                OUT.pos = IN[0].pos;
                OUT.uv = IN[0].uv;
                OUT.dist = float3(area / length(v0),0,0);
                OUT.worldPos = IN[0].worldPos;
                triStream.Append(OUT);

                OUT.pos = IN[1].pos;
                OUT.uv = IN[1].uv;
                OUT.dist = float3(0,area / length(v1),0);
                OUT.worldPos = IN[1].worldPos;
                triStream.Append(OUT);

                OUT.pos = IN[2].pos;
                OUT.uv = IN[2].uv;
                OUT.dist = float3(0,0,area / length(v2));
                OUT.worldPos = IN[2].worldPos;

                triStream.Append(OUT);
            }

            half4 frag(g2f IN) : COLOR
            {
                //distance of frag from triangles center
                float d = min(IN.dist.x, min(IN.dist.y, IN.dist.z));
                //fade based on dist from center
                float I = exp2(-4.0 * d * d);

                fixed4 color = lerp(_Color, _WireColor, I);

                float alpha = (sin(_Freq * (IN.worldPos.y + _Speed * _Time[0])) + 1) / 2;
                color.a *= alpha;

                return color;
            }
        ENDCG
        }
    }
}