Shader "Unlit/HealtBar"
{
    Properties
    {
        _Health ("Health", Range(0, 1)) = 1
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Health;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv= v.uv;
 
                return o;
            }

            float InverseLerp(float a, float b, float value)
            {
                return saturate((value - a) / (b - a));
            }


            fixed4 frag (v2f i) : SV_Target
            {

                

                float3 tHealthColor = InverseLerp(0.3, 0.8, _Health) ;
                float3 healtBarColor = lerp(float3(1, 0, 0), float3(0, 1, 0), tHealthColor);
                float3 bgColor = float3(0, 0, 0);
                float healtBarMask = _Health > i.uv.x;

                clip(healtBarMask - 0.001);

                float3 col = lerp(bgColor, healtBarColor, healtBarMask);

                return float4(col, 1);
            }
            ENDCG
        }
    }
}
