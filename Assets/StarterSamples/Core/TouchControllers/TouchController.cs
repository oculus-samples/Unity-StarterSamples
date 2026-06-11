// Copyright (c) Meta Platforms, Inc. and affiliates.


using Meta.XR.Samples;
using UnityEngine;

namespace OVRTouchSample
{
    // Animating controller that updates with the tracked controller.
    [MetaCodeSample("StarterSample.Core-TouchController")]
    public class TouchController : MonoBehaviour
    {
        [SerializeField]
        private OVRInput.Controller m_controller = OVRInput.Controller.None;

        [SerializeField]
        private Animator m_animator = null;

        private bool m_restoreOnInputAcquired = false;

        private void Update()
        {
            m_animator.SetFloat("Button 1", OVRInput.Get(OVRInput.Button.One, m_controller) ? 1.0f : 0.0f);
            m_animator.SetFloat("Button 2", OVRInput.Get(OVRInput.Button.Two, m_controller) ? 1.0f : 0.0f);
            m_animator.SetFloat("Joy X", OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, m_controller).x);
            m_animator.SetFloat("Joy Y", OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, m_controller).y);
            m_animator.SetFloat("Grip", OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller));
            m_animator.SetFloat("Trigger", OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, m_controller));

            OVRManager.InputFocusAcquired += OnInputFocusAcquired;
            OVRManager.InputFocusLost += OnInputFocusLost;
        }

        private void OnInputFocusLost()
        {
            if (gameObject.activeInHierarchy)
            {
                gameObject.SetActive(false);
                m_restoreOnInputAcquired = true;
            }
        }

        private void OnInputFocusAcquired()
        {
            if (m_restoreOnInputAcquired)
            {
                gameObject.SetActive(true);
                m_restoreOnInputAcquired = false;
            }
        }
    }
}
