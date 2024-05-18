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
using UnityEngine;

/// <summary>
/// The component responsible for the FoV occupied by the current rectTransform if rendered right in front of the camera,
/// positioning the rectangle perpendicular to the camera forward, and at the same time preserving the current distance
/// between the camera and the canvas.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class CameraFoVBehaviour : MonoBehaviour
{
    private const float IpdMeter = .065f; // use fixed IPD

    [SerializeField] private new Camera camera;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
    }

    /// <summary>
    /// FoV occupied by the current rectTransform if rendered right in front of the camera,
    /// positioning the canvas perpendicular to the camera forward, and at the same time preserving the current distance
    /// between the camera and the canvas. The result is returned in degrees.
    /// </summary>
    public float FoV
    {
        get
        {
            float rectWidthMeter = rectTransform.rect.width * rectTransform.transform.localScale.x;
            float distanceMeter = Vector3.Distance(rectTransform.transform.position, camera.transform.position);

            double fovDegree = Math.Atan((rectWidthMeter - IpdMeter) / 2 / distanceMeter) * 2 / Math.PI * 180;
            return (float)fovDegree;
        }
        set
        {
            float fovDegree = value;
            float distanceMeter = Vector3.Distance(rectTransform.transform.position, camera.transform.position);
            float rectWidthMeter = (float)(2 * distanceMeter * Math.Tan(fovDegree / 2 / 180 * Math.PI)) + IpdMeter;
            float newRectCanvasWidth = rectWidthMeter / rectTransform.transform.localScale.x;
            rectTransform.sizeDelta = new Vector2(newRectCanvasWidth, newRectCanvasWidth);
        }
    }
}
