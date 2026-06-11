// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using Meta.XR.Samples;
using UnityEngine.Assertions;
#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif

/// <summary>
/// Simply aggregates accessors.
/// </summary>
[MetaCodeSample("StarterSample.Core-Locomotion")]
public class LocomotionController : MonoBehaviour
{
    public OVRCameraRig CameraRig;

    //public CharacterController CharacterController;
    public CapsuleCollider CharacterController;

    //public OVRPlayerController PlayerController;
    public SimpleCapsuleWithStickMovement PlayerController;

    void Start()
    {
        /*
        if (CharacterController == null)
        {
            CharacterController = GetComponentInParent<CharacterController>();
        }
        Assert.IsNotNull(CharacterController);
        */
        //if (PlayerController == null)
        //{
        //PlayerController = GetComponentInParent<OVRPlayerController>();
        //}
        //Assert.IsNotNull(PlayerController);
        if (CameraRig == null)
        {
            CameraRig = FindFirstObjectByType<OVRCameraRig>();
        }

        Assert.IsNotNull(CameraRig);
    }
}
