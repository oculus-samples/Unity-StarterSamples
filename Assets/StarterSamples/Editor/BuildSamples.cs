/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System.IO;
using System.Linq;

//-------------------------------------------------------------------------------------
// ***** OculusBuildSamples
//
// Provides scripts to build Samples scene APKs.
//
partial class OculusBuildSamples
{
    // Update const if the file changes name or is relocated
    private const string CLASS_FILE_NAME = "BuildSamples.cs";
    private const string CLASS_FILE_INTERNAL_LOCATION = "Editor/" + CLASS_FILE_NAME;

    private static string samplesRootPath = "";

    public static string GetSamplesRootPath()
    {
        // we ensure the path exists otherwise we update it. This is in case the root of the samples is renamed or moved
        if (string.IsNullOrEmpty(samplesRootPath) || !Directory.Exists(samplesRootPath))
        {
            UpdateRootPath();
        }

        return samplesRootPath;
    }

    static void ImportSamplesFramework()
    {
        AssetDatabase.ImportPackage("OculusIntegration-release.unitypackage", false);
    }

    static void BuildLocomotion()
    {
        InitializeBuild("com.oculus.unitysample.locomotion");
        Build("Locomotion");
    }


    static void BuildHandsInteractionTrain()
    {
        InitializeBuild("com.oculus.unitysample.handsinteractiontrain");
        Build("HandsInteractionTrainScene");
    }

    static void BuildMixedRealityCapture()
    {
        InitializeBuild("com.oculus.unitysample.mixedrealitycapture");
        Build("MixedRealityCapture.apk",
            new[] { GetFullPathForSample("Usage/MixedRealityCapture/MixedRealityCapture.unity") });
    }

    static void BuildOVROverlay()
    {
        InitializeBuild("com.oculus.unitysample.ovroverlay");
        Build("OVROverlay");
    }

    static void BuildOVROverlayCanvas()
    {
        InitializeBuild("com.oculus.unitysample.ovroverlaycanvas");
        Build("OVROverlayCanvas");
    }

    static void BuildPassthrough()
    {
        InitializeBuild("com.oculus.unitysample.passthrough");
        Build("Passthrough");
    }

    static string GetFullPathForSample(string internalPath)
    {
        return Path.Combine(GetSamplesRootPath(), internalPath);
    }

    [MenuItem("Meta/Samples/Build Starter Scene")]
    static void BuildStartScene()
    {
        InitializeBuild("com.oculus.unitysample.startscene", "Meta XR SDK Samples");

        var projectSettings = OVRProjectConfig.CachedProjectConfig;
        projectSettings.insightPassthroughSupport = OVRProjectConfig.FeatureSupport.Supported;
        projectSettings.anchorSupport = OVRProjectConfig.AnchorSupport.Enabled;
        projectSettings.sceneSupport = OVRProjectConfig.FeatureSupport.Supported;
        projectSettings.handTrackingSupport = OVRProjectConfig.HandTrackingSupport.ControllersAndHands;
        Build(
            "StartScene.apk",
            new string[]
            {
                GetFullPathForSample("Usage/StartScene.unity"),
                GetFullPathForSample("Usage/CustomControllers.unity"),
                GetFullPathForSample("Usage/CustomHands.unity"),
                GetFullPathForSample("Usage/Tools/Firebase.unity"),
                GetFullPathForSample("Usage/HandsInteractionTrainScene.unity"),
                GetFullPathForSample("Usage/Locomotion.unity"),
                GetFullPathForSample("Usage/MixedRealityCapture/MixedRealityCapture.unity"),
                GetFullPathForSample("Usage/OVROverlay.unity"),
                GetFullPathForSample("Usage/OVROverlayCanvas.unity"),
                GetFullPathForSample("Usage/OVROverlayCanvas_Text.unity"),
                GetFullPathForSample("Usage/Passthrough.unity"),
                GetFullPathForSample("Usage/SceneManager.unity"),
                GetFullPathForSample("Usage/Stereo180Video.unity"),
                GetFullPathForSample("Usage/SpatialAnchor.unity"),
                GetFullPathForSample("Usage/WidevineVideo.unity"),
                GetFullPathForSample("Usage/Passthrough/Scenes/AugmentedObjects.unity"),
                GetFullPathForSample("Usage/Passthrough/Scenes/Lighting.unity"),
                GetFullPathForSample("Usage/Passthrough/Scenes/OverlayPassthrough.unity"),
                GetFullPathForSample("Usage/Passthrough/Scenes/PassthroughHands.unity"),
                GetFullPathForSample("Usage/Passthrough/Scenes/PassthroughStyles.unity"),
                GetFullPathForSample("Usage/Passthrough/Scenes/SelectivePassthrough.unity"),
                GetFullPathForSample("Usage/Passthrough/Scenes/SurfaceProjectedPassthrough.unity"),
                GetFullPathForSample("Usage/TouchPro/TouchProSample.unity"),
            });
    }


    private static void InitializeBuild(string identifier, string productName = null)
    {
        PlayerSettings.stereoRenderingPath = StereoRenderingPath.SinglePass;
        GraphicsDeviceType[] graphicsApis = new GraphicsDeviceType[1];
        graphicsApis[0] = GraphicsDeviceType.Vulkan;
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, graphicsApis);
        PlayerSettings.colorSpace = ColorSpace.Linear;
        //Set ARM64 Requirements
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetArchitecture(BuildTargetGroup.Android, 1); //0 - None, 1 - ARM64, 2 - Universal
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
        QualitySettings.antiAliasing = 4;
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, identifier);
        if (!string.IsNullOrEmpty(productName))
        {
            PlayerSettings.productName = productName;
        }
    }

    static void Build(string sceneName) => Build("Usage", sceneName);

    static void Build(string relativeSamplePath, string sceneName) =>
        Build($"{sceneName}.apk", new[]
        {
            GetFullPathForSample(Path.Combine(relativeSamplePath, $"{sceneName}.unity"))
        });


    private static void Build(string apkName, string[] scenes)
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.locationPathName = apkName;
        buildPlayerOptions.scenes = scenes;
        BuildReport buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
        if (!Application.isBatchMode && buildReport.summary.result == BuildResult.Succeeded)
        {
            EditorUtility.RevealInFinder(apkName);
        }
    }

    private static void AddSplashScreen(string path)
    {
        Texture2D companyLogo = Resources.Load<Texture2D>(path);
        PlayerSettings.virtualRealitySplashScreen = companyLogo;

        var logos = new PlayerSettings.SplashScreenLogo[2];

        // Company logo
        Sprite companyLogoSprite = (Sprite)AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));
        logos[0] = PlayerSettings.SplashScreenLogo.Create(2.5f, companyLogoSprite);

        // Set the Unity logo to be drawn after the company logo.
        logos[1] = PlayerSettings.SplashScreenLogo.CreateWithUnityLogo();

        PlayerSettings.SplashScreen.logos = logos;
    }

    private static void SetAppDetails(string companyName, string productName)
    {
        PlayerSettings.companyName = companyName;
        PlayerSettings.productName = productName;
    }

    private static void UpdateRootPath()
    {
        var path = AssetDatabase.GetAllAssetPaths()
            .FirstOrDefault(path => path.EndsWith(CLASS_FILE_NAME, StringComparison.OrdinalIgnoreCase));

        if (path == null)
        {
            throw new Exception($"Can't find path for {CLASS_FILE_NAME}");
        }

        var rootPath = path.Replace(CLASS_FILE_INTERNAL_LOCATION, string.Empty);
        samplesRootPath = rootPath;
    }
}
