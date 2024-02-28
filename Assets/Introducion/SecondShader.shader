Shader "Unlit/SecondSader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque"
    "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Cull Off
            ZWrite OFF
            ZTest GEQUAL
            Blend SRCCOLOR ONE

            


            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            // Tau
            #define TAU 6.28318530718

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;

                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);  
                o.normal = v.normal;
  
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                
                float xOffset = cos(i.uv.x *TAU*8)*0.01;
                float t = cos((i.uv.y + xOffset- _Time.y*0.1)*TAU*5)*0.5+0.5;

                t*= 1-i.uv.y;
                return t;

                //return float4(col.rgb * i.normal, col.a);
            }
            ENDCG
        }
    }
}
