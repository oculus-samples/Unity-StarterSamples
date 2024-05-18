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

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteInEditMode] [RequireComponent(typeof(CanvasGroup))]
public class PassthroughAtStartupInfoPanel : MonoBehaviour
{
    private readonly string infoPanelTextTemplate =
        @"This scene aims to show how to use contextual passthrough, and how to enhance that functionality within a custom startup scene.

{bullet_char}<indent=1em>{contextual_passthrough_state}</indent>
<indent=2em><size=90%>{contextual_passthrough_details}</size></indent>

{bullet_char}<indent=1em>{system_splash_screen_state}</indent>
<indent=2em><size=90%>{system_splash_screen_details}</size></indent>

{bullet_char}<indent=1em>{unity_splash_screen_state}</indent>
<indent=2em><size=90%>{unity_splash_screen_details}</size></indent>";


    public TextMeshProUGUI infoPanelText;
    public new Camera camera;
    public float panelZOffsetToCamera = .9f;
    public float panelYOffsetToCamera = -.22f;
    public float panelRotationAroundCamera = 23f;
    public SplashScreenSettings splashScreenSettings;

    private CanvasGroup canvasGroup;
    private bool isPassthroughRecommended;

    private void Awake()
    {
        isPassthroughRecommended = OVRManager.IsPassthroughRecommended();
        canvasGroup = GetComponent<CanvasGroup>();
        StartCoroutine(LoadSettingsAndPositionPanel());
    }

    // Update is called even during the Edit mode (notice [ExecuteInEditMode] at the top)
    void Update()
    {
        if (Application.isEditor)
        {
            // Check for project settings changes only when in Editor
            UpdatePanelText();
        }

        if (!Application.isEditor || Application.isPlaying)
        {
            transform.position = new Vector3(
                transform.position.x,
                camera.transform.position.y + panelYOffsetToCamera,
                transform.position.z);
        }
    }

    private IEnumerator LoadSettingsAndPositionPanel()
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            UpdatePanelText();
            yield break;
        }

        float origCanvasAlpha = canvasGroup.alpha;
        canvasGroup.alpha = 0;

        UpdatePanelText();

        // A quick way to check that the CameraRig.CenterCamera is initialized (not immediately true when running
        // in Unity Editor over Link)
        while (camera.transform.position.x == 0)
            yield return null;

        UpdatePanelPosition();
        canvasGroup.alpha = origCanvasAlpha;
    }

    // Keep track of the project settings changes, to instantly update the text once a user alters those settings
    private void UpdatePanelText()
    {
        Debug.Assert(splashScreenSettings != null, "SplashScreenSettings is required for the panel to work properly");

        string text = infoPanelTextTemplate;
        Dictionary<string, string> replacements = new Dictionary<string, string>();
        replacements["bullet_char"] = "\u2022";

        replacements["contextual_passthrough_state"] =
            splashScreenSettings.isContextualPassthroughEnabled
                ? "The app has opted in for <b>contextual passthrough</b>"
                : "The app has <b>NOT</b> opted in for <b>contextual passthrough</b>";

        replacements["contextual_passthrough_details"] =
            splashScreenSettings.isContextualPassthroughEnabled
                ? Application.isEditor
                    ? "The app supports contextual passthrough. To experience it, deploy and run the app on a Meta Quest device."
                    : isPassthroughRecommended
                        ? "Passthrough background was used on app loading"
                        : "You came from VR Home, and thus the shell-to-app transition happened on top of a black background"
                : Application.isEditor
                    ? "When running the app on a device, loading screens will be shown on black background"
                    : "Loading screens are shown on black background";

        replacements["system_splash_screen_state"] =
            splashScreenSettings.isSystemSplashScreenEnabled
                ? "<b>System splash image</b> is set"
                : "<b>System splash image</b> is not set";

        replacements["system_splash_screen_details"] =
            splashScreenSettings.isSystemSplashScreenEnabled
                ? Application.isEditor
                    ? "However, you cannot experience it in Editor either. Deploy and run the app on device to test"
                    : "It should have been shown while the app was launching"
                : Application.isEditor
                    ? "When running the app on device, a loading indicator with three dots will be shown"
                    : "A loading indicator with three dots should have been shown while the app was launching";

        replacements["unity_splash_screen_state"] =
            splashScreenSettings.isUnityVrSplashScreenEnabled
                ? "<b>Unity VR Splash Image</b> is set"
                : splashScreenSettings.isUnityLogosSplashScreenEnabled
                    ? "<b>Unity Show Splash Screen</b> toggle is on"
                    : "<b>Unity splash screen</b> is disabled";

        replacements["unity_splash_screen_details"] =
            splashScreenSettings.isUnityVrSplashScreenEnabled || splashScreenSettings.isUnityLogosSplashScreenEnabled
                ? Application.isEditor
                    ? "When running the app on device, you will always observe a period of black background"
                    : isPassthroughRecommended
                        ? "You could see a period of black background on app start"
                        : "Since the app was launched from VR Home, both Unity's and the system's loading screen were showing black background"
                : Application.isEditor
                    ? "When launching the app on device from Passthrough Home, passthrough should be visible seamlessly"
                    : isPassthroughRecommended
                        ? "Passthrough should have been visible seamlessly during launch"
                        : "Passthrough was not shown while the app was loading because it was launched from VR Home";

        foreach (string pattern in replacements.Keys)
        {
            text = text.Replace("{" + pattern + "}", replacements[pattern]);
        }

        infoPanelText.text = text;
    }

    private void UpdatePanelPosition()
    {
        if (!Application.isPlaying)
            return;

        var cameraPosition = camera.transform.position;
        var cameraForward = new Vector3(camera.transform.forward.x, 0, camera.transform.forward.z).normalized;

        // Position the panel in front of the camera
        transform.position = cameraPosition +
                             cameraForward * panelZOffsetToCamera +
                             new Vector3(0, panelYOffsetToCamera, 0); // move along the Y axis

        // Rotate the panel around the camera by some angle not to keep it at the center of FoV
        transform.RotateAround(cameraPosition, Vector3.up, panelRotationAroundCamera);

        // Orient the panel towards the camera
        transform.rotation = Quaternion.LookRotation(new Vector3(transform.position.x, cameraPosition.y, transform.position.z) - camera.transform.position);
    }
}
