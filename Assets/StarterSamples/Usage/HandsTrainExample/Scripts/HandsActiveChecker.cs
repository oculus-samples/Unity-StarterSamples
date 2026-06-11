// Copyright (c) Meta Platforms, Inc. and affiliates.


using System.Collections;
using Meta.XR.Samples;
using UnityEngine;
using UnityEngine.Assertions;

[MetaCodeSample("StarterSample-HandsTrain")]
public class HandsActiveChecker : MonoBehaviour
{
    [SerializeField]
    private GameObject _notificationPrefab = null;

    private GameObject _notification = null;
    private OVRCameraRig _cameraRig = null;
    private Transform _centerEye = null;

    private void Awake()
    {
        Assert.IsNotNull(_notificationPrefab);
        _notification = Instantiate(_notificationPrefab);
        StartCoroutine(GetCenterEye());
    }

    private void Update()
    {
        if (OVRPlugin.GetHandTrackingEnabled())
        {
            _notification.SetActive(false);
        }
        else
        {
            _notification.SetActive(true);
            if (_centerEye)
            {
                _notification.transform.position = _centerEye.position + _centerEye.forward * 0.5f;
                _notification.transform.rotation = _centerEye.rotation;
            }
        }
    }

    private IEnumerator GetCenterEye()
    {
        if ((_cameraRig = FindFirstObjectByType<OVRCameraRig>()) != null)
        {
            while (!_centerEye)
            {
                _centerEye = _cameraRig.centerEyeAnchor;
                yield return null;
            }
        }
    }
}
