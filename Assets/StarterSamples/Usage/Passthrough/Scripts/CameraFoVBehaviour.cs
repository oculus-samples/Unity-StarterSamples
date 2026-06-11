// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using Meta.XR.Samples;
using UnityEngine;

/// <summary>
/// The component responsible for the FoV occupied by the current rectTransform if rendered right in front of the camera,
/// positioning the rectangle perpendicular to the camera forward, and at the same time preserving the current distance
/// between the camera and the canvas.
/// </summary>
[RequireComponent(typeof(RectTransform))]
[MetaCodeSample("StarterSample-Passthrough")]
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
