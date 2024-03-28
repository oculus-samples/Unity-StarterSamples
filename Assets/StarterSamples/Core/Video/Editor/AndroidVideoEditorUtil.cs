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

using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;

public class AndroidVideoEditorUtil
{
    private static readonly string[] VideoPluginNames = {
        "NativeVideoPlayer", "audio360"
    };
    private const string ExoPlayerPackageName = "com.google.android.exoplayer:exoplayer";
    private const string ExoPlayerVersion = "2.18.2";
    private const string AndroidXPropertyName = "android.useAndroidX";
    private const string AndroidXPropertyValue = "true";
    private const AndroidSdkVersions MinimumTargetAndroidSdkVersion = (AndroidSdkVersions)31;

    private static void SetPluginCompatibility(bool enabled)
    {
        string[] searchFolders = { OculusBuildSamples.GetSamplesRootPath() };

        // Enable or Disable Android Video Plugins
        foreach (var pluginName in VideoPluginNames)
        {
            foreach (var guid in AssetDatabase.FindAssets(pluginName, searchFolders))
            {
                string pluginPath = AssetDatabase.GUIDToAssetPath(guid);
                PluginImporter pluginImporter = AssetImporter.GetAtPath(pluginPath) as PluginImporter;
                if (pluginImporter != null)
                {
                    Debug.Log($"Setting Android Compatibility for {pluginPath} to {enabled}");
                    pluginImporter.SetCompatibleWithPlatform(BuildTarget.Android, enabled);
                    pluginImporter.SaveAndReimport();
                }
            }
        }
    }

    [MenuItem("Oculus/Samples/Video/Enable Native Android Video Player")]
    public static void EnableNativeVideoPlayer()
    {
        // Enable video plugins
        SetPluginCompatibility(true);

        // Enable gradle build with exoplayer
        Gradle.Configuration.UseGradle();
        var template = Gradle.Configuration.OpenTemplate();
        template.AddDependency(ExoPlayerPackageName, ExoPlayerVersion);
        template.Save();

        var properties = Gradle.Configuration.OpenProperties();
        properties.SetProperty(AndroidXPropertyName, AndroidXPropertyValue);
        properties.Save();

        // Set the target sdk version to the required minimum for the NativeVideoPlayer plugin
        if (PlayerSettings.Android.targetSdkVersion < MinimumTargetAndroidSdkVersion)
        {
            PlayerSettings.Android.targetSdkVersion = MinimumTargetAndroidSdkVersion;
        }
    }

    [MenuItem("Oculus/Samples/Video/Disable Native Android Video Player")]
    public static void DisableNativeVideoPlayer()
    {
        // Disable video plugins
        SetPluginCompatibility(false);

        // remove exoplayer from gradle file (leave other changes since they are potentially shared).
        if (Gradle.Configuration.IsUsingGradle())
        {
            var template = Gradle.Configuration.OpenTemplate();
            template.RemoveDependency(ExoPlayerPackageName);
            template.Save();
        }
    }
}
