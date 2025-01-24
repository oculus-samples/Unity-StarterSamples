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
using UnityEngine;
using UnityEngine.UI;

public class EnableDisablePassthroughController : MonoBehaviour
{
    private static readonly int InvertedAlpha = Shader.PropertyToID("_InvertedAlpha");

    [SerializeField]
    [Tooltip("The passthrough layer to which we will subscribe for the resumed event")]
    private OVRPassthroughLayer passthroughLayer;

    [SerializeField]
    [Tooltip("The mesh renderer on which the fading effect is applied")]
    private MeshRenderer passthroughRenderer;

    [SerializeField]
    [Tooltip("The UI toggle that enables the event subscription")]
    private Toggle passthroughResumedToggle;

    [SerializeField]
    [Tooltip("The speed of the passthrough transition effect. Value N means the transition happens in 1/N of a sec")]
    private float passthroughFadeSpeed = 5f;

    private Material _material;
    private PassthroughTransitionType lastTransitionRequested;
    private Coroutine transitionCoroutine;

    private void Awake()
    {
        OVRManager.eyeFovPremultipliedAlphaModeEnabled = false;
        _material = passthroughRenderer.material;
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            TogglePassthrough();
        }
    }

    private void OnEnable()
    {
        if (passthroughResumedToggle.isOn)
        {
            passthroughLayer.passthroughLayerResumed.AddListener(OnPassthroughLayerResumed);
        }
    }

    private void OnDisable()
    {
        passthroughLayer.passthroughLayerResumed.RemoveListener(OnPassthroughLayerResumed);
    }

    private void TogglePassthrough()
    {
        PassthroughTransitionType transitionRequested = lastTransitionRequested == PassthroughTransitionType.MrToVr
            ? PassthroughTransitionType.VrToMr
            : PassthroughTransitionType.MrToVr;

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        transitionCoroutine = transitionRequested == PassthroughTransitionType.MrToVr
            ? StartCoroutine(TransitionToVR())
            : StartCoroutine(TransitionToMR());

        lastTransitionRequested = transitionRequested;
    }

    private IEnumerator TransitionToMR()
    {
        // Start Passthrough feature
        OVRManager.instance.isInsightPassthroughEnabled = true;
        passthroughLayer.enabled = true;
        passthroughLayer.hidden = false;

        // If we don't use PassthroughResumedEvent event, then we start the transition right here
        // Otherwise, the transition starts in the event handler.
        if (!passthroughResumedToggle.isOn)
        {
            passthroughRenderer.enabled = true;
            yield return ChangePassthroughVisibility(targetVisibility: 1f);
        }
    }

    private IEnumerator TransitionToVR()
    {
        // First, fade passthrough
        yield return ChangePassthroughVisibility(targetVisibility: 0f);

        // Disable passthrough related GameObjects and stop the feature
        passthroughRenderer.enabled = false;
        passthroughLayer.enabled = false;
        passthroughLayer.hidden = true;
        OVRManager.instance.isInsightPassthroughEnabled = false;
    }

    /// <summary>
    /// Linearly changes the transparency of the background passthrough on the scene.
    /// 1.0 means passthrough is fully visible, 0.0 means no passthrough
    /// </summary>
    private IEnumerator ChangePassthroughVisibility(float targetVisibility)
    {
        var currentAlpha = _material.GetFloat(InvertedAlpha);
        while (!Mathf.Approximately(currentAlpha, targetVisibility))
        {
            currentAlpha = Mathf.MoveTowards(currentAlpha, targetVisibility, passthroughFadeSpeed * Time.deltaTime);
            _material.SetFloat(InvertedAlpha, currentAlpha);
            yield return null;
        }

        _material.SetFloat(InvertedAlpha, targetVisibility);
    }

    private void OnPassthroughLayerResumed(OVRPassthroughLayer layer)
    {
        // Enable the passthrough renderer, and start a coroutine which will slowly reveal passthrough
        passthroughRenderer.enabled = true;
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        transitionCoroutine = StartCoroutine(ChangePassthroughVisibility(targetVisibility: 1f));
    }

    public void TogglePassthroughResumedCallback(bool shouldSubscribe)
    {
        if (shouldSubscribe)
        {
            passthroughLayer.passthroughLayerResumed.AddListener(OnPassthroughLayerResumed);
        }
        else
        {
            passthroughLayer.passthroughLayerResumed.RemoveListener(OnPassthroughLayerResumed);
        }
    }

    private enum PassthroughTransitionType
    {
        VrToMr,
        MrToVr
    }
}
