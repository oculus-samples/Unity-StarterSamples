// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Threading.Tasks;
using Meta.XR.Samples;
using UnityEngine;

[MetaCodeSample("StarterSample-Passthrough")]
public class SplashScreenController : MonoBehaviour
{
    private CameraSnapBehaviour cameraSnapBehaviour;
    private CameraFoVBehaviour cameraFoVBehavior;

    public float ImageDistance
    {
        get => cameraSnapBehaviour.distanceToCamera;
        set => cameraSnapBehaviour.distanceToCamera = value;
    }

    public float ImageFoV
    {
        get => cameraFoVBehavior.FoV;
        set => cameraFoVBehavior.FoV = value;
    }

    public float ImageFadeInDuration
    {
        get => cameraSnapBehaviour.fadeInDuration;
        set => cameraSnapBehaviour.fadeInDuration = value;
    }

    public Task FadeoutSplashImagePanel(float duration)
    {
        return cameraSnapBehaviour.FadeoutCanvas(duration);
    }

    private void Start()
    {
        cameraSnapBehaviour = gameObject.GetComponent<CameraSnapBehaviour>();
        cameraFoVBehavior = gameObject.GetComponent<CameraFoVBehaviour>();
    }
}
