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

    static void BuildDistanceGrab()
    {
        InitializeBuild("com.oculus.unitysample.distancegrab");
        Build("DistanceGrab");
    }

    static void BuildDebugUI()
    {
        InitializeBuild("com.oculus.unitysample.debugui");
        Build("DebugUI");
    }

    static void BuildHandsInteractionTrain()
    {
        InitializeBuild("com.oculus.unitysample.handsinteractiontrain");
        Build("HandsInteractionTrainScene");
    }

    static void BuildMixedRealityCapture()
    {
        InitializeBuild("com.oculus.unitysample.mixedrealitycapture");
        Build("MixedRealityCapture");
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

    // reach out to panya or brittahummel for issues regarding passthrough
    static void BuildPassthrough()
    {
        InitializeBuild("com.oculus.unitysample.passthrough");
        // TODO: enable OpenXR so Passthrough works
        Build("Passthrough");
    }

    //needs openXR backend in ovrplugin
    static void BuildBouncingBall()
    {
        InitializeBuild("com.oculus.unitysample.bouncingball");
        Build("BouncingBall");
    }

    //needs openXR backend in ovrplugin
    static void BuildShowSceneModel()
    {
        InitializeBuild("com.oculus.unitysample.scenemanager");
        Build("SceneManager");
    }

    //needs openXR backend in ovrplugin
    static void BuildVirtualFurniture()
    {
        InitializeBuild("com.oculus.unitysample.virtualfurniture");
        Build("VirtualFurniture");
    }

    static string GetFullPathForSample(string internalPath)
    {
        return Path.Combine(GetSamplesRootPath(), internalPath);
    }

    [MenuItem("Oculus/Samples/Build Starter Scene")]
    static void BuildStartScene()
    {
        InitializeBuild("com.oculus.unitysample.startscene");
        Build(
            "StartScene.apk",
            new string[]
            {
                GetFullPathForSample("Usage/StartScene.unity"),
                GetFullPathForSample("Usage/Locomotion.unity"),
                GetFullPathForSample("Usage/DistanceGrab.unity"),
                GetFullPathForSample("Usage/DebugUI.unity"),
                GetFullPathForSample("Usage/HandsInteractionTrainScene.unity"),
                GetFullPathForSample("Usage/MixedRealityCapture.unity"),
                GetFullPathForSample("Usage/OVROverlay.unity"),
                GetFullPathForSample("Usage/OVROverlayCanvas.unity"),
                GetFullPathForSample("Usage/Passthrough.unity"),
                GetFullPathForSample("Usage/SceneManager.unity")
            });
    }

    private static void InitializeBuild(string identifier)
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
    }

    private static void Build(string sceneName)
    {
        Build(sceneName + ".apk", new string[] { GetFullPathForSample($"Usage/{sceneName}.unity") });
    }


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
