// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections;
using System.Collections.Generic;
using Meta.XR.Samples;
using UnityEngine;

[MetaCodeSample("StarterSample-Passthrough")]
public class SPPquad : MonoBehaviour
{
    OVRPassthroughLayer passthroughLayer;
    public MeshFilter projectionObject;
    OVRInput.Controller controllerHand;

    void Start()
    {
        passthroughLayer = GetComponent<OVRPassthroughLayer>();
        passthroughLayer.AddSurfaceGeometry(projectionObject.gameObject, false);
        if (GetComponent<GrabObject>())
        {
            GetComponent<GrabObject>().GrabbedObjectDelegate += Grab;
            GetComponent<GrabObject>().ReleasedObjectDelegate += Release;
        }
    }

    public void Grab(OVRInput.Controller grabHand)
    {
        passthroughLayer.RemoveSurfaceGeometry(projectionObject.gameObject);
        controllerHand = grabHand;
    }

    public void Release()
    {
        controllerHand = OVRInput.Controller.None;
        passthroughLayer.AddSurfaceGeometry(projectionObject.gameObject, false);
    }
}
