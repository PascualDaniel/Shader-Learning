            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            #define USE_LIGHTING

            struct MeshData {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 tangent : TEXCOORD2;
                float3 bitangent : TEXCOORD3;
                float3 wPos : TEXCOORD4;
 
                LIGHTING_COORDS(5, 6)
            };

            sampler2D _Albedo;
            float4 _Albedo_ST;
            float _Gloss;
            float4 _Color;
            sampler2D _NormalMap;
            sampler2D _HeightMap;
            sampler2D _DiffuseIBL;
            float _DisplacementStrength;
            float4 _AmbientLight;

            #define PI 3.1415926535897932384626433832795
            

            Interpolators vert (MeshData v) {
                Interpolators o;

                o.uv = TRANSFORM_TEX(v.uv, _Albedo);

                float height= tex2Dlod(_HeightMap, float4(o.uv,0,0)).x * 2 - 1;
      
                v.vertex.xyz += v.normal *(height* _DisplacementStrength);

                o.vertex = UnityObjectToClipPos(v.vertex);
                
                o.normal = UnityObjectToWorldNormal( v.normal );

                o.tangent = UnityObjectToWorldDir( v.tangent.xyz );
                o.bitangent = cross( o.normal, o.tangent );
                o.bitangent *=  (v.tangent.w * unity_WorldTransformParams.w);

                o.wPos = mul( unity_ObjectToWorld, v.vertex );
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            float2 DirToRectilinear(float3 dir){
                float x = atan2(dir.x, dir.z) / PI/2 + 0.5;
                float y = dir.y* 0.5 + 0.5;
                return float2(x,y);
            }

            float4 frag (Interpolators i) : SV_Target {

              

                
               
                float3 rock = tex2D(_Albedo, i.uv).rgb;
                float3 surfaceColor = rock * _Color.rgb;
                float3 tangentSpaceNormal = UnpackNormal( tex2D( _NormalMap, i.uv ));

                float3x3 mtxTangToWorld = { i.tangent.x, i.bitangent.x, i.normal.x,
                                            i.tangent.y, i.bitangent.y, i.normal.y,
                                            i.tangent.z, i.bitangent.z, i.normal.z };

                float3 N = mul( mtxTangToWorld, tangentSpaceNormal  );

            #ifdef USE_LIGHTING
                    // diffuse lighting
                //float3 N = normalize(i.normal);
                float3 L = normalize( UnityWorldSpaceLightDir(i.wPos));
                float attenuation = LIGHT_ATTENUATION(i);
                float3 lambert = saturate( dot( N, L ) );
                float3 diffuseLight =( lambert *attenuation ) *  _LightColor0.xyz;
                #ifdef IS_IN_BASE_PASS
                    float3 diffuseIBL =  tex2Dlod(_DiffuseIBL, float4(DirToRectilinear(N),0,0)).xyz;
                    diffuseLight += diffuseIBL;
                #endif
                // specular lighting
                float3 V = normalize( _WorldSpaceCameraPos - i.wPos );
                float3 H = normalize(L + V);
                //float3 R = reflect( -L, N ); // uses for Phong
                float3 specularLight = saturate(dot(H, N)) * (lambert > 0); // Blinn-Phong
                float specularExponent = exp2( _Gloss * 11 ) + 2;
                specularLight = pow( specularLight, specularExponent ) * _Gloss * attenuation; // specular exponent
                specularLight *= _LightColor0.xyz;

                return float4( diffuseLight * surfaceColor + specularLight, 1 );

            #else
                #ifdef IS_IN_BASE_PASS
                    
                    return surfaceColor;
                #else
                    return 0;
                #endif

            #endif

                
            }