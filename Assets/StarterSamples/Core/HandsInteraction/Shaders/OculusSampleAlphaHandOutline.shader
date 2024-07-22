/************************************************************************************

Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

See SampleFramework license.txt for license terms.  Unless required by applicable law
or agreed to in writing, the sample code is provided "AS IS" WITHOUT WARRANTIES OR
CONDITIONS OF ANY KIND, either express or implied.  See the license for specific
language governing permissions and limitations under the license.

************************************************************************************/

Shader "Oculus Sample/Alpha Hand Outline"
{
  Properties
  {
    _ColorPrimary ("Color Primary", Color) = (0.396078, 0.725490, 1)
    _ColorTop ("Color Top", Color) = (0.031896, 0.0343398, 0.0368894)
    _ColorBottom ("Color Bottom", Color) = (0.0137021, 0.0144438, 0.0152085)
    _RimFactor ("Rim Factor", Range(0.01, 1.0)) = 0.65
    _FresnelPower ("Fresnel Power", Range(0.01,1.0)) = 0.16

    _HandAlpha ("Hand Alpha", Range(0, 1)) = 1.0
    _MinVisibleAlpha ("Minimum Visible Alpha", Range(0,1)) = 0.15

    [HideInInspector][Toggle] _ALPHAPREMULTIPLY ("Enable Alpha", Float) = 1.0
  }

  SubShader
  {
    PackageRequirements { "com.unity.render-pipelines.universal" }

    Tags { "Queue"="Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

    HLSLINCLUDE
    half4 _ColorPrimary;
    half4 _ColorTop;
    half4 _ColorBottom;
    float _RimFactor;
    float _FresnelPower;
    float _HandAlpha;
    float _MinVisibleAlpha;
    ENDHLSL

    // Write depth values so that you only see the
    // topmost layer of this transparent material.
    Pass
    {
      ZWrite On
      ColorMask 0
    }

    Pass
    {
      Tags { "LightMode"="UniversalForward" }
      Blend SrcAlpha OneMinusSrcAlpha

      HLSLPROGRAM
      #pragma target 2.0

      #pragma vertex LitPassVertex
      #pragma fragment CustomLitPassFragment

      #define _MAIN_LIGHT_SHADOWS_CASCADE
      #define _SURFACE_TYPE_TRANSPARENT

      #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
      void CustomizeSurfaceData(InputData input, inout SurfaceData surfaceData)
      {
        // see BiRP shadow below for details
        half viewDotNormal = saturate(dot(input.viewDirectionWS, input.normalWS));
        half rim = pow(1.0 - viewDotNormal, 0.5) * (1.0 - _RimFactor) + _RimFactor;
        rim = saturate(rim);
        half3 emission = lerp(half3(0,0,0), _ColorPrimary.rgb, rim);
        emission += rim * 0.5;

        float fresnel = saturate(pow(1.0 - viewDotNormal, _FresnelPower));
        half4 color = lerp(_ColorTop, _ColorBottom, fresnel);
        half alphaValue = step(_MinVisibleAlpha, _HandAlpha) * _HandAlpha;

        surfaceData.alpha = alphaValue;
        surfaceData.emission = color.rgb * emission;
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
    LOD 100

    // Write depth values so that you see topmost layer of this transparent material.
    Pass
    {
      ZWrite On
      ColorMask 0
    }

    CGPROGRAM
    // no deferred. support lightmaps and one light. use half vector instead of
    // view vector (less accurate but faster)
    #pragma surface surf BlinnPhong alpha:fade exclude_path:prepass noforwardadd halfasview nolightmap

    struct Input {
      float2 uv_MainTex;
      float3 worldRefl;
      float3 viewDir;
      INTERNAL_DATA
    };

    fixed3 _ColorPrimary;
    fixed3 _ColorTop;
    fixed3 _ColorBottom;
    float _RimFactor;
    float _FresnelPower;

    float _HandAlpha;
    float _MinVisibleAlpha;

    float3 SafeNormalize(float3 normal) {
      float magSq = dot(normal, normal);
      if (magSq == 0) {
        return 0;
      }
      return normalize(normal);
    }

    #define ColorBlack half3(0, 0, 0)
    #define EmissionFactor (0.95)

    void surf(Input IN, inout SurfaceOutput o) {
      float3 normalDirection = SafeNormalize(o.Normal);
      float3 viewDir = SafeNormalize(IN.viewDir);
      half viewDotNormal = saturate(dot(viewDir, normalDirection));
      // the higher the rim factor, the greater the effect overall. by default,
      // it's strongest near edges
      half rim = pow(1.0 - viewDotNormal, 0.5) * (1.0 - _RimFactor) + _RimFactor;
      rim = saturate(rim);

      half3 emission = lerp(ColorBlack, _ColorPrimary, rim);
      // brighten emission a bit, multiply by factor to reign it in
      emission += rim * 0.5;
      emission *= EmissionFactor;

      // effect gets stronger toward edges. note that lerp can extrapolate! saturate first
      // potentially could schlick approx, like specColor + (1-specColor)*nDotLight^5
      // except here we woulduse nDotView instead of nDotLight
      float fresnel = saturate(pow(1.0 - viewDotNormal, _FresnelPower));
      fixed3 color = lerp(_ColorTop, _ColorBottom, fresnel);

      fixed alphaValue = step(_MinVisibleAlpha, _HandAlpha) * _HandAlpha;

      o.Albedo = 0;
      o.Gloss = 0;
      o.Specular = 0;
      o.Alpha = alphaValue;
      o.Emission = color * emission;
    }
    ENDCG
  }
}
