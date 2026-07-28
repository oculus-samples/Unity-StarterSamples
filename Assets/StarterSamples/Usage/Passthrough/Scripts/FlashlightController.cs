// Copyright (c) Meta Platforms, Inc. and affiliates.

using Meta.XR.Samples;
using System.Collections;
using UnityEngine;

[MetaCodeSample("StarterSample-Passthrough")]
public class FlashlightController : MonoBehaviour
{
    [SerializeField]
    private OVRGrabbable grabbable;

    public Light sceneLight;
    public Transform flashlightRoot;
    Vector3 localPosition = Vector3.zero;
    Quaternion localRotation = Quaternion.identity;

    private bool isGrabbed;

    private void Start()
    {
        localRotation = flashlightRoot.localRotation;
        localPosition = flashlightRoot.localPosition;

        if (GetComponent<Flashlight>())
        {
            GetComponent<Flashlight>().EnableFlashlight(false);
        }
    }

    void Update()
    {
        if (grabbable.isGrabbed && !isGrabbed)
        {
            Grab();
        }
        else if (!grabbable.isGrabbed && isGrabbed)
        {
            Release();
        }
    }

    public void Grab()
    {
        isGrabbed = true;

        if (GetComponent<Flashlight>())
        {
            GetComponent<Flashlight>().EnableFlashlight(true);
        }

        StopAllCoroutines();
        StartCoroutine(FadeLighting(new Color(0, 0, 0, 0.95f), 0.0f, 0.25f));
    }

    public void Release()
    {
        isGrabbed = false;

        if (GetComponent<Flashlight>())
        {
            GetComponent<Flashlight>().EnableFlashlight(false);
        }

        StopAllCoroutines();
        StartCoroutine(FadeLighting(Color.clear, 1.0f, 0.25f));
    }

    IEnumerator FadeLighting(Color newColor, float sceneLightIntensity, float fadeTime)
    {
        float timer = 0.0f;
        Color currentColor = Camera.main.backgroundColor;
        float currentLight = sceneLight ? sceneLight.intensity : 0;
        while (timer <= fadeTime)
        {
            timer += Time.deltaTime;
            float normTimer = Mathf.Clamp01(timer / fadeTime);
            Camera.main.backgroundColor = Color.Lerp(currentColor, newColor, normTimer);
            if (sceneLight) sceneLight.intensity = Mathf.Lerp(currentLight, sceneLightIntensity, normTimer);
            yield return null;
        }
    }
}
