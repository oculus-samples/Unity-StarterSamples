// Copyright (c) Meta Platforms, Inc. and affiliates.

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
