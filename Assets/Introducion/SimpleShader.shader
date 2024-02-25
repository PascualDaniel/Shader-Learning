Shader "Unlit/SimpleShader"
{
    Properties{
       // _MainTex ("Texture", 2D) = "white" {}
       _Color("Color",Color) = (1,1,1,0)
       _Glossiness("Glossiness",Range(1,200)) = 1
    }
    SubShader{
        Tags { "RenderType"="Opaque" }

        Pass{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            
            //Mesh data: vertex position, normal, colors, UVs, tangents, etc.
            struct appdata{
                float4 vertex : POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normal : NORMAL;
             
        
                //float4 tangent : TANGENT;         
                //float2 uv1 : TEXCOORD1;

            };
            
            // Variables from properties 
            uniform float4 _Color;
            uniform float _Glossiness;


            //Output from vertex shader to fragment shader
            struct VertexOutput{
                float2 uv0 : TEXCOORD0;
                float4 clipSpacePos : SV_POSITION;
                float3 normal : NORMAL;
                float3 worldPossition : TEXCOORD2;
       
            };

            //Vertex shader
            VertexOutput vert (appdata v){
                VertexOutput o;
                //Transform vertex position to clip space
                o.uv0 = v.uv0;
  
                o.normal = v.normal;

                o.worldPossition = mul(unity_ObjectToWorld, v.vertex);
                // Transform vertex position to clip space method
                o.clipSpacePos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (VertexOutput o) : SV_Target{
              
                float2 uv = o.uv0;
                //Direct Defuse Ligth
                float3 ligthDir = _WorldSpaceLightPos0.xyz;
                float3 ligthColor = _LightColor0.xyz;
                float ligthFallOff = max(0,dot(ligthDir,o.normal));
                float3 directDiffuseLight = ligthColor * ligthFallOff;
                //Ambient Light
                float3 ambientLight = float3(0.1,0.1,0.1);

                //Direct specualar light
                float3 cameraPosition = _WorldSpaceCameraPos ;
                float3 fragToCam = cameraPosition - o.worldPossition;
                float3 viewDir = normalize(fragToCam);

                float3 viewReflect = reflect(-viewDir,o.normal);

                float3 specularFallOff = max(0,dot(viewReflect,ligthDir));

                //modify gloss
                specularFallOff = pow(specularFallOff,_Glossiness);

                return float4(specularFallOff.xxx,0);



                //Composite Light
                float3 diffuseLight = ambientLight + directDiffuseLight;

                float3 finalSurfaceColor = diffuseLight * _Color.rgb;
   

                return float4(finalSurfaceColor,0);
            }

            ENDCG
        }
    }
}
