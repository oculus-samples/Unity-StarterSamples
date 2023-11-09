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

public class SceneSettings : MonoBehaviour
{
    [Header("Time")]
    [SerializeField] private float m_fixedTimeStep = 0.01f;

    [Header("Physics")]
    [SerializeField] private float m_gravityScalar = 0.75f;

    [SerializeField] private float m_defaultContactOffset = 0.001f;

    private void Awake()
    {
        // Time
        Time.fixedDeltaTime = m_fixedTimeStep;

        // Physics
        Physics.gravity = Vector3.down * 9.81f * m_gravityScalar;
        Physics.defaultContactOffset = m_defaultContactOffset;
    }

    private void Start()
    {
        // Update the contact offset for all existing colliders since setting
        // Physics.defaultContactOffset only applies to newly created colliders.
        CollidersSetContactOffset(m_defaultContactOffset);
    }

    private static void CollidersSetContactOffset(float contactOffset)
    {
        // @Note: This does not find inactive objects.
        Collider[] allColliders = GameObject.FindObjectsOfType<Collider>();
        foreach (Collider collider in allColliders)
        {
            collider.contactOffset = contactOffset;
        }
    }
}
