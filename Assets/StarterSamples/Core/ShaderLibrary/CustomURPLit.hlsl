#ifndef CUSTOM_URP_LIT
#define CUSTOM_URP_LIT

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

void CustomizeSurfaceData(InputData input, inout SurfaceData surfaceData);

#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Shaders/LitForwardPass.hlsl"

half4 CustomLitPassFragment(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    SurfaceData surfaceData;
    InitializeStandardLitSurfaceData(input.uv, surfaceData);

    InputData inputData;
    InitializeInputData(input, surfaceData.normalTS, inputData);

    CustomizeSurfaceData(inputData, surfaceData);

    half4 color = UniversalFragmentPBR(inputData, surfaceData);
    color.rgb = MixFog(color.rgb, inputData.fogCoord);

    return color;
}

#endif
