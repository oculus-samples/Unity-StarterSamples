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

using UnityEditor;
using UnityEngine;

/// <summary>
/// The class is responsible for keeping an instance of SplashScreenSettings scriptable object in sync
/// with the current project settings related to splash screens. It operates only in Unity Editor.
/// Outside Unity, the clients can refer to the SplashScreenSettings instance for the cached settings.
/// </summary>
[InitializeOnLoad]
public class SplashScreenSettingsSynchronizer
{
    private const string SplashScreenSettingsPath = "Assets/Resources/SplashScreenSettings.asset";

    private static readonly SplashScreenSettings SplashScreenSettings;

    static SplashScreenSettingsSynchronizer()
    {
        SplashScreenSettings = AssetDatabase.LoadAssetAtPath<SplashScreenSettings>(SplashScreenSettingsPath);
        if (SplashScreenSettings == null)
        {
            SplashScreenSettings = ScriptableObject.CreateInstance<SplashScreenSettings>();
            AssetDatabase.CreateAsset(SplashScreenSettings, SplashScreenSettingsPath);
            AssetDatabase.SaveAssets();
        }

        EditorApplication.update -= Update;
        EditorApplication.update += Update;
    }

    private static void Update()
    {
        OVRProjectConfig projectConfig = OVRProjectConfig.CachedProjectConfig;

        bool isDirty = UpdateFieldIfDiffer(
            ref SplashScreenSettings.isContextualPassthroughEnabled,
            projectConfig.systemLoadingScreenBackground == OVRProjectConfig.SystemLoadingScreenBackground.ContextualPassthrough);

        isDirty |= UpdateFieldIfDiffer(
            ref SplashScreenSettings.isSystemSplashScreenEnabled,
            projectConfig.systemSplashScreen != null);

        isDirty |= UpdateFieldIfDiffer(
            ref SplashScreenSettings.isUnityVrSplashScreenEnabled,
            PlayerSettings.virtualRealitySplashScreen != null);

        isDirty |= UpdateFieldIfDiffer(
            ref SplashScreenSettings.isUnityLogosSplashScreenEnabled,
            PlayerSettings.SplashScreen.show);

        if (isDirty)
        {
            EditorUtility.SetDirty(SplashScreenSettings);
        }
    }

    private static bool UpdateFieldIfDiffer(ref bool field, bool newValue)
    {
        if (field == newValue)
            return false;

        field = newValue;
        return true;
    }
}
