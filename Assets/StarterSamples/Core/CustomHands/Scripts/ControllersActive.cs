// Copyright (c) Meta Platforms, Inc. and affiliates.

using Meta.XR.Samples;
using UnityEngine;

namespace OVRTouchSample
{
    [MetaCodeSample("StarterSample.Core-CustomHands")]
    public class ControllersActive : MonoBehaviour
    {
        [SerializeField] private GameObject _notificationUI;
        [SerializeField] private GameObject _handLeft;
        [SerializeField] private GameObject _handRight;

        void Update()
        {
            var handTrackingActive = OVRPlugin.GetHandTrackingEnabled();
            if (handTrackingActive && !_notificationUI.activeSelf)
            {
                _notificationUI.SetActive(true);
                _handLeft.SetActive(false);
                _handRight.SetActive(false);
            }
            else if (!handTrackingActive && _notificationUI.activeSelf)
            {
                _notificationUI.SetActive(false);
                _handLeft.SetActive(true);
                _handRight.SetActive(true);
            }

        }
    }
}
