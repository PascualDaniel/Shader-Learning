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

          
            // posterize method
            float4 posterize(float steps, float value){
                return floor(value*steps)/steps;
            }



            fixed4 frag (VertexOutput o) : SV_Target{
              
                float2 uv = o.uv0;

                float3 green = float3(0,1,0);
                float3 red = float3(1,0,0);

                float3 color = lerp(green,red,uv.y);
                
                //smoothstep
                color = smoothstep(0.3,0.7,color);
                color = floor(color*10)/10;
                //return float4(color,1);


                float3 normal = normalize(o.normal); //Interpolated


                //Direct Defuse Ligth
                float3 ligthDir = _WorldSpaceLightPos0.xyz;
                float3 ligthColor = _LightColor0.xyz;
                float ligthFallOff = max(0,dot(ligthDir,normal));
                ligthFallOff = posterize(3,ligthFallOff);
                float3 directDiffuseLight = ligthColor * ligthFallOff;
                //Ambient Light
                float3 ambientLight = float3(0.1,0.1,0.1);

                //Direct specualar light
                float3 cameraPosition = _WorldSpaceCameraPos ;
                float3 fragToCam = cameraPosition - o.worldPossition;
                float3 viewDir = normalize(fragToCam);
                float3 viewReflect = reflect(-viewDir,normal);
                float3 specularFallOff = max(0,dot(viewReflect,ligthDir));
                //modify gloss
                specularFallOff = pow(specularFallOff,_Glossiness);
                specularFallOff = posterize(4,specularFallOff);
                float3 directSpecularLight = ligthColor * specularFallOff;

                //return float4(specularFallOff.xxx,0);

                //Composite Light
                float3 diffuseLight = ambientLight + directDiffuseLight;

                float3 finalSurfaceColor = diffuseLight * _Color.rgb+ directSpecularLight;
   

                return float4(finalSurfaceColor,0);
            }

            ENDCG
        }
    }
}
