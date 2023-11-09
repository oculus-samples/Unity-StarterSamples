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

using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UiSceneMenu : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private VerticalLayoutGroup m_layoutGroup = null;

    [SerializeField] private TextMeshProUGUI m_labelPf = null;

    private static Vector2 s_lastThumbstickL;
    private static Vector2 s_lastThumbstickR;

    private Scene m_activeScene;

    private void Awake()
    {
        m_activeScene = SceneManager.GetActiveScene();

        // Build labels
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; ++i)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            CreateLabel(i, scenePath);
        }
    }

    private void Update()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        if (InputPrevScene())
        {
            ChangeScene((m_activeScene.buildIndex - 1 + sceneCount) % sceneCount);
        }
        else if (InputNextScene())
        {
            ChangeScene((m_activeScene.buildIndex + 1) % sceneCount);
        }

        s_lastThumbstickL = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
        s_lastThumbstickR = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
    }

    private bool InputPrevScene()
    {
        return KeyboardPrevScene() || ThumbstickPrevScene(OVRInput.Controller.LTouch) ||
               ThumbstickPrevScene(OVRInput.Controller.RTouch);
    }

    private bool InputNextScene()
    {
        return KeyboardNextScene() || ThumbstickNextScene(OVRInput.Controller.LTouch) ||
               ThumbstickNextScene(OVRInput.Controller.RTouch);
    }

    private bool KeyboardPrevScene()
    {
        return Input.GetKeyDown(KeyCode.UpArrow);
    }

    private bool KeyboardNextScene()
    {
        return Input.GetKeyDown(KeyCode.DownArrow);
    }

    private bool ThumbstickPrevScene(OVRInput.Controller controller)
    {
        return (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, controller).y >= 0.9f) &&
               (GetLastThumbstickValue(controller).y < 0.9f);
    }

    private bool ThumbstickNextScene(OVRInput.Controller controller)
    {
        return (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, controller).y <= -0.9f) &&
               (GetLastThumbstickValue(controller).y > -0.9f);
    }

    private Vector2 GetLastThumbstickValue(OVRInput.Controller controller)
    {
        return controller == OVRInput.Controller.LTouch ? s_lastThumbstickL : s_lastThumbstickR;
    }

    private void ChangeScene(int nextScene)
    {
        SceneManager.LoadScene(nextScene);
    }

    private void CreateLabel(int sceneIndex, string scenePath)
    {
        // Get the scene name
        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

        // Add spaces after capital letters
        sceneName = Regex.Replace(sceneName, "[A-Z]", " $0").Trim();

        // Call attention to the active scene
        bool isActiveScene = m_activeScene.buildIndex == sceneIndex;
        if (isActiveScene)
        {
            sceneName = $"Open: {sceneName}";
        }

        // Create and set the label
        TextMeshProUGUI label = GameObject.Instantiate(m_labelPf);
        label.SetText($"{sceneIndex + 1}. {sceneName}");
        label.transform.SetParent(m_layoutGroup.transform, false);
    }
}
