// Copyright (c) Meta Platforms, Inc. and affiliates.


using UnityEngine;
using System.Collections;
using Meta.XR.Samples;

[MetaCodeSample("StarterSample.Core-Locomotion")]
public class TeleportPoint : MonoBehaviour
{
    public float dimmingSpeed = 1;
    public float fullIntensity = 1;
    public float lowIntensity = 0.5f;

    public Transform destTransform;

    private float lastLookAtTime = 0;


    // Use this for initialization
    void Start()
    {
    }

    public Transform GetDestTransform()
    {
        return destTransform;
    }


    // Update is called once per frame
    void Update()
    {
        float intensity = Mathf.SmoothStep(fullIntensity, lowIntensity, (Time.time - lastLookAtTime) * dimmingSpeed);
        GetComponent<MeshRenderer>().material.SetFloat("_Intensity", intensity);
    }

    public void OnLookAt()
    {
        lastLookAtTime = Time.time;
    }
}
