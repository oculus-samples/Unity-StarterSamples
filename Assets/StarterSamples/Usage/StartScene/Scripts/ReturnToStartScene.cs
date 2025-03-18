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


using Meta.XR.Samples;
using UnityEngine;
using UnityEngine.SceneManagement;

[MetaCodeSample("StarterSample-StartScene")]
public class ReturnToStartScene : MonoBehaviour
{
    public bool ShowStartButtonTooltip;
    private static ReturnToStartScene _instance;
    [SerializeField] private GameObject _tooltip;
    private const float _forwardTooltipOffset = -0.05f;
    private const float _upwardTooltipOffset = -0.003f;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _tooltip.SetActive(ShowStartButtonTooltip);
            DontDestroyOnLoad(this.gameObject);
        }
        else if (_instance != this)
        {
            _instance.ToggleStartButtonTooltip(this
                .ShowStartButtonTooltip); // copy the setting from the new loaded scene
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Since this is the Start scene, we can assume it's the first index in build settings
        if (OVRInput.GetUp(OVRInput.Button.Start) && SceneManager.GetActiveScene().buildIndex != 0)
        {
            SceneManager.LoadScene(0);
        }

        if (ShowStartButtonTooltip)
        {
            var finalRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch) *
                                Quaternion.Euler(45, 0, 0);
            var forwardOffsetPosition = finalRotation * Vector3.forward * _forwardTooltipOffset;
            var upwardOffsetPosition = finalRotation * Vector3.up * _upwardTooltipOffset;
            var finalPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch) +
                                forwardOffsetPosition + upwardOffsetPosition;
            _tooltip.transform.rotation = finalRotation;
            _tooltip.transform.position = finalPosition;
        }
    }

    private void ToggleStartButtonTooltip(bool shouldShowTooltip)
    {
        ShowStartButtonTooltip = shouldShowTooltip;
        _tooltip.SetActive(ShowStartButtonTooltip);
    }
}
