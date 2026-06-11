// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Meta.XR.Samples;

// Create menu of all scenes included in the build.
[MetaCodeSample("StarterSample-StartScene")]
public class StartMenu : MonoBehaviour
{
    public OVROverlay overlay;
    public OVROverlay text;
    public OVRCameraRig vrRig;

    void Start()
    {
        var generalScenes = new List<Tuple<int, string>>();
        var passthroughScenes = new List<Tuple<int, string>>();
        var proControllerScenes = new List<Tuple<int, string>>();

        int n = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        for (int sceneIndex = 0; sceneIndex < n; ++sceneIndex)
        {
            string path = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(sceneIndex);

            if (path.Contains("Passthrough"))
            {
                passthroughScenes.Add(new Tuple<int, string>(sceneIndex, path));
            }
            else if (path.Contains("TouchPro"))
            {
                proControllerScenes.Add(new Tuple<int, string>(sceneIndex, path));
            }
            else
            {
                generalScenes.Add(new Tuple<int, string>(sceneIndex, path));
            }
        }

        if (passthroughScenes.Count > 0)
        {
            DebugUIBuilder.instance.AddLabel("Passthrough Sample Scenes", DebugUIBuilder.DEBUG_PANE_LEFT);
            foreach (var scene in passthroughScenes)
            {
                DebugUIBuilder.instance.AddButton(Path.GetFileNameWithoutExtension(scene.Item2), () => LoadScene(scene.Item1), -1, DebugUIBuilder.DEBUG_PANE_LEFT);
            }
        }

        if (proControllerScenes.Count > 0)
        {
            DebugUIBuilder.instance.AddLabel("Pro Controller Sample Scenes", DebugUIBuilder.DEBUG_PANE_RIGHT);
            foreach (var scene in proControllerScenes)
            {
                DebugUIBuilder.instance.AddButton(Path.GetFileNameWithoutExtension(scene.Item2), () => LoadScene(scene.Item1), -1, DebugUIBuilder.DEBUG_PANE_RIGHT);
            }
        }

        DebugUIBuilder.instance.AddLabel("Press ☰ at any time to return to scene selection", DebugUIBuilder.DEBUG_PANE_CENTER);
        if (generalScenes.Count > 0)
        {
            DebugUIBuilder.instance.AddDivider(DebugUIBuilder.DEBUG_PANE_CENTER);
            DebugUIBuilder.instance.AddLabel("Sample Scenes", DebugUIBuilder.DEBUG_PANE_CENTER);
            foreach (var scene in generalScenes)
            {
                DebugUIBuilder.instance.AddButton(Path.GetFileNameWithoutExtension(scene.Item2), () => LoadScene(scene.Item1), -1, DebugUIBuilder.DEBUG_PANE_CENTER);
            }
        }

        DebugUIBuilder.instance.Show();
    }

    void LoadScene(int idx)
    {
        DebugUIBuilder.instance.Hide();
        Debug.Log("Load scene: " + idx);
        UnityEngine.SceneManagement.SceneManager.LoadScene(idx);
    }
}
