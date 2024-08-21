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

using System.Threading.Tasks;
using UnityEngine;

public class EnableDisablePassthroughController : MonoBehaviour
{
    // with callback
    [SerializeField] private OVRPassthroughLayer layerWithPtResumedCallback;
    [SerializeField] private Renderer rendererWithPtResumedCallback;

    // without callback
    [SerializeField] private OVRPassthroughLayer layerWithoutPtResumedCallback;
    [SerializeField] private Renderer rendererWithoutPtResumedCallback;

    private void Start()
    {
        Debug.Log("Insight Passthrough supported on the device: " + OVRPlugin.IsInsightPassthroughSupported());
        OVRManager.instance.isInsightPassthroughEnabled = true;
        Debug.Log((OVRManager.instance.isInsightPassthroughEnabled ? "Enabling" : "Disabling") + " passthrough feature");

        Initialize(layerWithPtResumedCallback, rendererWithPtResumedCallback);
        Initialize(layerWithoutPtResumedCallback, rendererWithoutPtResumedCallback);

        // At start enable both Passthrough layers
        EnablePtLayerWithCallback(true);
        EnablePtLayerWithoutCallback(true);
    }

    void OnEnable()
    {
        layerWithPtResumedCallback.passthroughLayerResumed.AddListener(OnPassthroughLayerResumed);
    }

    void OnDisable()
    {
        layerWithPtResumedCallback.passthroughLayerResumed.RemoveListener(OnPassthroughLayerResumed);
    }

    private async void OnPassthroughLayerResumed(OVRPassthroughLayer layer)
    {
        Debug.Log("Received PassthroughLayerResumed event, passthrough content will be visible soon...");
        await Task.Delay(20);
        Debug.Log("... passthrough content is now visible");
        layer.enabled = true;
    }

    private void Initialize(OVRPassthroughLayer layer, Renderer layerRenderer)
    {
        layer.AddSurfaceGeometry(layerRenderer.gameObject, updateTransform: false);
    }

    private void EnablePtLayerWithCallback(bool enable)
    {
        // If enable is false, then we immediately disable the renderer.
        // If enable is true, we still disable the renderer, and wait for the PassthroughLayerResumed event,
        // which in turn enables the renderer
        rendererWithPtResumedCallback.enabled = false;

        layerWithPtResumedCallback.enabled = enable;
        layerWithPtResumedCallback.hidden = !enable;
    }

    private void EnablePtLayerWithoutCallback(bool enable)
    {
        rendererWithoutPtResumedCallback.enabled = enable;

        layerWithoutPtResumedCallback.enabled = enable;
        layerWithoutPtResumedCallback.hidden = !enable;
    }

    void Update()
    {
        // Press A button
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            EnablePtLayerWithCallback(!layerWithPtResumedCallback.enabled);
        }

        // Press B button
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            EnablePtLayerWithoutCallback(!layerWithoutPtResumedCallback.enabled);
        }
    }
}
