Shader "Custom/GradientShader"
{
    Properties
    {
        _StartColor ("Start Color", Color) = (0,0,0,1)
        _MiddleColor ("Middle Color", Color) = (0.5,0.5,0.5,1)
        _EndColor ("End Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed4 _StartColor;
            fixed4 _MiddleColor;
            fixed4 _EndColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float t = i.uv.y;
                fixed4 col;
                
                if (t < 0.5)
                {
                    col = lerp(_StartColor, _MiddleColor, t * 2);
                }
                else
                {
                    col = lerp(_MiddleColor, _EndColor, (t - 0.5) * 2);
                }
                
                return col;
            }
            ENDCG
        }
    }
}