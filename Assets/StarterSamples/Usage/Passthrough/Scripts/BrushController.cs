// Copyright (c) Meta Platforms, Inc. and affiliates.

using Meta.XR.Samples;
using System.Collections;
using UnityEngine;

[MetaCodeSample("StarterSample-Passthrough")]
public class BrushController : MonoBehaviour
{
    [SerializeField]
    private OVRGrabbable grabbable;

    public PassthroughBrush brush;
    public MeshRenderer backgroundSphere;

    IEnumerator grabRoutine;
    IEnumerator releaseRoutine;

    private bool isGrabbed;

    void Start()
    {
        brush.controllerHand = OVRInput.Controller.None;

        if (!brush.lineContainer)
        {
            brush.lineContainer = new GameObject("LineContainer");
        }

        backgroundSphere.material.renderQueue = 3998;
        backgroundSphere.transform.parent = null;
        backgroundSphere.enabled = false;
    }

    void Update()
    {
        backgroundSphere.transform.position = Camera.main.transform.position;

        if (grabbable.isGrabbed && !isGrabbed)
        {
            Grab(OVRInput.Controller.Hands);
        }
        else if (!grabbable.isGrabbed && isGrabbed)
        {
            Release();
        }
    }

    public void GrabWithHand()
    {
        Grab(OVRInput.Controller.Hands);
    }

    public void Grab(OVRInput.Controller controllerType)
    {
        isGrabbed = true;
        brush.controllerHand = controllerType;
        brush.lineContainer.SetActive(true);
        backgroundSphere.enabled = true;

        if (grabRoutine != null) StopCoroutine(grabRoutine);
        if (releaseRoutine != null) StopCoroutine(releaseRoutine);
        grabRoutine = FadeSphere(Color.grey, 0.25f);
        StartCoroutine(grabRoutine);
    }

    public void Release()
    {
        isGrabbed = false;
        brush.controllerHand = OVRInput.Controller.None;
        brush.lineContainer.SetActive(false);

        if (grabRoutine != null) StopCoroutine(grabRoutine);
        if (releaseRoutine != null) StopCoroutine(releaseRoutine);
        releaseRoutine = FadeSphere(new Color(0.5f, 0.5f, 0.5f, 0.0f), 0.25f, true);
        StartCoroutine(releaseRoutine);
    }

    IEnumerator FadeCameraClearColor(Color newColor, float fadeTime)
    {
        float timer = 0.0f;
        Color currentColor = Camera.main.backgroundColor;
        while (timer <= fadeTime)
        {
            timer += Time.deltaTime;
            float normTimer = Mathf.Clamp01(timer / fadeTime);
            Camera.main.backgroundColor = Color.Lerp(currentColor, newColor, normTimer);
            yield return null;
        }
    }

    IEnumerator FadeSphere(Color newColor, float fadeTime, bool disableOnFinish = false)
    {
        float timer = 0.0f;
        Color currentColor = backgroundSphere.material.GetColor("_Color");
        while (timer <= fadeTime)
        {
            timer += Time.deltaTime;
            float normTimer = Mathf.Clamp01(timer / fadeTime);
            backgroundSphere.material.SetColor("_Color", Color.Lerp(currentColor, newColor, normTimer));
            if (disableOnFinish && timer >= fadeTime)
            {
                backgroundSphere.enabled = false;
            }

            yield return null;
        }
    }
}
