// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.EventSystems;
using Meta.XR.Samples;

[MetaCodeSample("StarterSample.Core-DebugUI")]
public class HandedInputSelector : MonoBehaviour
{
    OVRCameraRig m_CameraRig;
    OVRInputModule m_InputModule;

    void Start()
    {
        m_CameraRig = FindFirstObjectByType<OVRCameraRig>();
        m_InputModule = FindFirstObjectByType<OVRInputModule>();
    }

    void Update()
    {
        if (OVRInput.GetActiveController() == OVRInput.Controller.LTouch)
        {
            SetActiveController(OVRInput.Controller.LTouch);
        }
        else
        {
            SetActiveController(OVRInput.Controller.RTouch);
        }
    }

    void SetActiveController(OVRInput.Controller c)
    {
        Transform t;
        if (c == OVRInput.Controller.LTouch)
        {
            t = m_CameraRig.leftHandAnchor;
        }
        else
        {
            t = m_CameraRig.rightHandAnchor;
        }

        m_InputModule.rayTransform = t;
    }
}
