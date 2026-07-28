// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Meta.XR.Samples;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[MetaCodeSample("StarterSample-Passthrough")]
public class PassthroughAtStartupController : MonoBehaviour
{
    private AsyncOperation loadingNextSceneOperation;
    private bool transitionToNextSceneStarted;
    private float loadingElapsed;
    private bool pauseTransition;

    public string nextScene;
    public float transitionMinDuration;

    [SerializeField] private SplashScreenController splashScreenController;
    [SerializeField] private OVRPassthroughLayer passthroughLayer;

    // Left controller menu UI items
    public Slider progressBar;

    public void Start()
    {
#if PLATFORM_ANDROID && !UNITY_EDITOR
        // Enable Passthrough layer only if recommended by the system. If a user comes from the VR Home -
        // the recommendation flag will be false, so we will show a black background instead of the passthrough
        // Don't do this if the app is run within Unity Editor and over Link - the Link client on the device
        // is a VR app, so OVRManager.IsPassthroughRecommended() will always return false.
        passthroughLayer.enabled = OVRManager.IsPassthroughRecommended();
#endif
        string confirmedNextScene = GetFinalSceneToRedirect(nextScene);
        if (!string.IsNullOrEmpty(confirmedNextScene))
        {
            // Trigger the async scene loading
            loadingElapsed = 0;
            loadingNextSceneOperation = SceneManager.LoadSceneAsync(confirmedNextScene);
            loadingNextSceneOperation.allowSceneActivation = false;
        }
    }

    /// <summary>
    /// Makes sure that the requested scene is present in the build. If not,
    /// applies heuristic to return some other available scene
    /// </summary>
    private string GetFinalSceneToRedirect(string proposedSceneName)
    {
        // If the build contains only one scene (the current one), then there is no more scene to redirect to
        if (SceneManager.sceneCountInBuildSettings == 1)
            return null;

        List<string> allScenesInBuild = GetAllSceneNamesFromBuild();

        // Check if proposed scene is included in the build
        if (allScenesInBuild.Any(x => String.Equals(x, proposedSceneName, StringComparison.CurrentCultureIgnoreCase)))
            return proposedSceneName;

        // If scene is not in the build, then pick the next scene after the current
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        return allScenesInBuild[(currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings];
    }

    /// <summary>
    /// A helper method to get the list of scene names included in the current build.
    /// A more intuitive solution using SceneManager.GetSceneByBuildIndex() fails when the scene hasn't been loaded yet.
    ///  </summary>
    private List<string> GetAllSceneNamesFromBuild()
    {
        var result = new List<string>();
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            result.Add(Path.GetFileNameWithoutExtension(scenePath));
        }

        return result;
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.A))
        {
            PauseTransitionPressed(!pauseTransition);
        }

        if (transitionToNextSceneStarted)
            return;

        if (!pauseTransition)
        {
            loadingElapsed += Time.deltaTime;
        }

        float sceneLoadingProgress = Mathf.Clamp01(loadingNextSceneOperation != null ? loadingNextSceneOperation.progress / 0.9f : 1f);
        float durationProgress = Mathf.Clamp01(loadingElapsed / transitionMinDuration);

        float progress = Math.Min(sceneLoadingProgress, durationProgress);
        progressBar.value = progress;

        if (progress >= .99f)
        {
            transitionToNextSceneStarted = true;
            StartTransitionToNextScene();
        }
    }

    private async void StartTransitionToNextScene()
    {
        // First wait until the splash image fades out
        await splashScreenController.FadeoutSplashImagePanel(duration: .5f);

        if (loadingNextSceneOperation != null)
        {
            // Finally trigger the scene transition
            loadingNextSceneOperation.allowSceneActivation = true;
        }
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void PauseTransitionPressed(bool value)
    {
        pauseTransition = value;
    }

    public void OnMicroGesture(OVRHand.MicrogestureType gesture)
    {
        if (gesture == OVRHand.MicrogestureType.ThumbTap)
        {
            PauseTransitionPressed(!pauseTransition);
        }
    }
}
