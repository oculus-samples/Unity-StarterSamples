/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using UnityEngine;

public class LocalizedHaptics : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private OVRInput.Handedness m_handedness = OVRInput.Handedness.LeftHanded;

    private OVRInput.Controller m_controller;

    private void Start()
    {
        m_controller = m_handedness == OVRInput.Handedness.LeftHanded
            ? OVRInput.Controller.LTouch
            : OVRInput.Controller.RTouch;
    }

    private void Update()
    {
        // Build vibration for the frame based on device inputs
        float thumbAmp = OVRInput.Get(OVRInput.Axis1D.PrimaryThumbRestForce, m_controller) > 0.5f ? 1f : 0f;
        OVRInput.SetControllerLocalizedVibration(OVRInput.HapticsLocation.Thumb, 0f, thumbAmp, m_controller);

        float indexAmp = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, m_controller) > 0.5f ? 1f : 0f;
        OVRInput.SetControllerLocalizedVibration(OVRInput.HapticsLocation.Index, 0f, indexAmp, m_controller);

        float handAmp = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller) > 0.5f ? 1f : 0f;
        OVRInput.SetControllerLocalizedVibration(OVRInput.HapticsLocation.Hand, 0f, handAmp, m_controller);
    }
}
