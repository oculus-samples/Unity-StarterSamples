// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Linq;
using Meta.XR.Samples;
using UnityEngine;
using UnityEngine.SceneManagement;

[MetaCodeSample("StarterSample-Passthrough")]
public class SceneSampler : MonoBehaviour
{
    private int currentSceneIndex;
    public GameObject displayText;

    void Awake()
    {
        // Make sure we have only one instance of SceneSampler game object when jumping between scene
        var samplers = FindObjectsByType<SceneSampler>(FindObjectsSortMode.None);
        if (samplers.Any(sceneSampler => sceneSampler != this))
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Log Passthrough capabilities
        OVRManager.PassthroughCapabilities passthroughCapabilities = OVRManager.GetPassthroughCapabilities();
        Debug.Log("PassthroughCapabilities.SupportsPassthrough: " + passthroughCapabilities.SupportsPassthrough);
        Debug.Log("PassthroughCapabilities.SupportsColorPassthrough: " + passthroughCapabilities.SupportsColorPassthrough);
        Debug.Log("PassthroughCapabilities.MaxColorLutResolution: " + passthroughCapabilities.MaxColorLutResolution);
        Debug.Log("IsPassthroughRecommended: " + OVRManager.IsPassthroughRecommended());
    }

    void Update()
    {
        bool controllersActive = OVRInput.GetActiveController() == OVRInput.Controller.Touch ||
          OVRInput.GetActiveController() == OVRInput.Controller.LTouch ||
          OVRInput.GetActiveController() == OVRInput.Controller.RTouch;

        displayText.SetActive(controllersActive);

        if (OVRInput.GetUp(OVRInput.Button.Start))
        {
            currentSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
            SceneManager.LoadScene(currentSceneIndex);
        }

        Vector3 menuPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch) + Vector3.up * 0.09f;
        displayText.transform.position = menuPosition;
        displayText.transform.rotation = Quaternion.LookRotation(menuPosition - Camera.main.transform.position);
    }
}
