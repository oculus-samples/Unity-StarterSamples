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
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

// Create menu of all scenes included in the build.
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
