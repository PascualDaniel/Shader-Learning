Shader "Unlit/LightingTest" {
    Properties {
        _Albedo ("Albedo", 2D) = "white" {}
        _Gloss ("Gloss", Range(0,1)) = 1
        _Color ("Color", Color) = (0,1,1,0)
        [NoScaleOffset]_NormalMap ("NormalMap", 2D) = "bump" {}
        [NoScaleOffset]_HeightMap ("HeightMap", 2D) = "black" {}
        _DisplacementStrength ("Displacement Strength", Range(0,0.2)) = 0
        _AmbientLight ("Ambient Light", Color) = (0,0,0,0)
        _DiffuseIBL ("Diffuse IBL", 2D) = "black" {}

    }
    SubShader {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }

        Pass {
            Tags { "LightMode" = "ForwardBase" }
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #define IS_IN_BASE_PASS
            #include "FGLighting.cginc"

          
            ENDCG
        }

        Pass {
            Tags { "LightMode" = "ForwardAdd" }
            Blend One One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd
            #include "FGLighting.cginc"

          
            ENDCG
        }
        
        
    }
}
