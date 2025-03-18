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

using System;
using System.Collections.Generic;
using Meta.XR.Samples;
using UnityEngine;

/// <summary>
/// Sets the visibility of an <see cref="OVRSceneAnchor"/>'s render mesh according to whether it is currently tracked.
/// </summary>
[Obsolete("OVRSceneManager and associated classes are deprecated (v65), please use MR Utility Kit instead (https://developer.oculus.com/documentation/unity/unity-mr-utility-kit-overview)")]
[MetaCodeSample("StarterSample-SceneManager")]
public class RenderMeshVisibility : MonoBehaviour
{
    List<MeshRenderer> _meshRenderers = new();

    OVRSceneAnchor _sceneAnchor;

    void Start()
    {
        if (TryGetComponent(out _sceneAnchor))
        {
            _sceneAnchor.GetComponentsInChildren(_meshRenderers);
        }
        else
        {
            enabled = false;
        }
    }

    void Update()
    {
        foreach (var meshRenderer in _meshRenderers)
        {
            // IsTracked will be false if the system cannot determine the pose of the anchor. In this case, we
            // stop rendering it so that it doesn't appear in the wrong place.
            meshRenderer.enabled = _sceneAnchor.IsTracked;
        }
    }
}
