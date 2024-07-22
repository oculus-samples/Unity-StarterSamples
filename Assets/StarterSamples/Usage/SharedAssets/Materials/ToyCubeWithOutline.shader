// Custom shader to draw our toy cubes and balls with an outline around them.
Shader "Custom/ToyCubeOutline"
{
    Properties
    {
        [MainColor] _BaseColor("Color", Color) = (1,1,1,1)
        [MainTexture] _BaseMap("Albedo", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline width", Range (.002, 0.03)) = .005

        [HideInInspector] _Mode ("__mode", Float) = 0.0
        [HideInInspector] _SrcBlend ("__src", Float) = 1.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
        [HideInInspector] _ZWrite ("__zw", Float) = 1.0
    }

    CGINCLUDE
    #include "UnityCG.cginc"

    struct appdata
    {
        float4 vertex : POSITION;
        float3 normal : NORMAL;

        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct v2f
    {
        float4 pos : SV_POSITION;
        fixed4 color : COLOR;

        UNITY_VERTEX_OUTPUT_STEREO
    };

    uniform float _OutlineWidth;
    uniform float4 _OutlineColor;
    uniform float4x4 _ObjectToWorldFixed;

    // Pushes the verts out a little from the object center.
    // Lets us give an outline to objects that all have normals facing away from the center.
    // If we can't assume that, we need to tweak the math of this shader.
    v2f vert(appdata v)
    {
        v2f o;

        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_OUTPUT(v2f, o);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        // MTF TODO
        // 1. Fix batching so that it actually occurs.
        // 2. See if batching causes problems,
        // if it does fix this line by adding that component that sets it.
        //float4 objectCenterWorld = mul(_ObjectToWorldFixed, float4(0.0, 0.0, 0.0, 1.0));
        float4 objectCenterWorld = mul(unity_ObjectToWorld, float4(0.0, 0.0, 0.0, 1.0));
        float4 vertWorld = mul(unity_ObjectToWorld, v.vertex);

        float3 offsetDir = vertWorld.xyz - objectCenterWorld.xyz;
        offsetDir = normalize(offsetDir) * _OutlineWidth;

        o.pos = mul(UNITY_MATRIX_VP, float4(vertWorld+offsetDir, 1.0f));

        o.color = _OutlineColor;
        return o;
    }
    ENDCG

    SubShader
    {
        PackageRequirements { "com.unity.render-pipelines.universal" }

        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline" = "UniversalPipeline"  }

        Pass
        {
            Name "OUTLINEURP"
            // To allow the cube to render entirely on top of the outline.
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            fixed4 frag(v2f i) : SV_Target
            {
                // Just draw the _OutlineColor from the vert pass above.
                return i.color;
            }
            ENDCG
        }

        Pass
        {
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma target 2.0

            // -------------------------------------
            // Use the vert/frag from Universal Render Pipeline
            #pragma vertex LitPassVertexSimple
            #pragma fragment LitPassFragmentSimple

            // -------------------------------------
            // Universal Render Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ EVALUATE_SH_MIXED
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma multi_compile_fragment _ _LIGHT_LAYERS
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            #pragma multi_compile _ _FORWARD_PLUS

            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitForwardPass.hlsl"
            ENDHLSL
        }
        UsePass "Universal Render Pipeline/Simple Lit/ShadowCaster"
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" }

        Pass
        {
            Name "OUTLINE"
            // To allow the cube to render entirely on top of the outline.
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            fixed4 frag(v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        fixed4 _BaseColor;
        sampler2D _BaseMap;
        struct Input { float2 uv_BaseMap; };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_BaseMap, IN.uv_BaseMap) * _BaseColor;
            o.Albedo = c.rgb;
            o.Metallic = 0.0f;
            o.Smoothness = 0.0f;
            o.Alpha = c.a;
        }
        ENDCG
    }

    Fallback Off
}
