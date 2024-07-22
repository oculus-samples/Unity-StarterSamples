Shader "Oculus/Hands_Transparent" {
     Properties
     {
       _InnerColor ("Inner Color", Color) = (1.0, 1.0, 1.0, 1.0)
       _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
       _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0

       [HideInInspector][Toggle] _ALPHAPREMULTIPLY ("Enable Alpha", Float) = 1.0
     }

     SubShader
     {
         PackageRequirements { "com.unity.render-pipelines.universal" }

         Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }

         HLSLINCLUDE
         half4 _InnerColor;
         half4 _RimColor;
         half _RimPower;
         ENDHLSL

         Pass
         {
             Tags { "LightMode" = "UniversalForward" }

             Cull Back
             Blend One One

             HLSLPROGRAM
             #pragma target 2.0

             #pragma vertex LitPassVertex
             #pragma fragment CustomLitPassFragment

             #define _MAIN_LIGHT_SHADOWS_CASCADE
             #define _SHADOWS_SOFT
             #define _SURFACE_TYPE_TRANSPARENT

             #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
             void CustomizeSurfaceData(InputData input, inout SurfaceData surfaceData)
             {
                 surfaceData.albedo = _InnerColor.rgb;
                 surfaceData.alpha = _InnerColor.a;
                 half rim = 1.0 - saturate(dot(input.viewDirectionWS, input.normalWS));
                 surfaceData.emission = _RimColor.rgb * pow(rim, _RimPower);
             }
             #include "../../ShaderLibrary/CustomURPLit.hlsl"
             ENDHLSL
         }
         UsePass "Universal Render Pipeline/Lit/ShadowCaster"
         UsePass "Universal Render Pipeline/Lit/DepthOnly"
         UsePass "Universal Render Pipeline/Lit/Meta"
     }

     SubShader
     {
       Tags { "Queue" = "Transparent" }

       Cull Back
       Blend One One

       CGPROGRAM
       #pragma surface surf Lambert

       struct Input
       {
           float3 viewDir;
       };

       float4 _InnerColor;
       float4 _RimColor;
       float _RimPower;

       void surf (Input IN, inout SurfaceOutput o)
       {
           o.Albedo = _InnerColor.rgb;
           half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
           o.Emission = _RimColor.rgb * pow (rim, _RimPower);
       }
       ENDCG
     }
     Fallback "Diffuse"
   }
