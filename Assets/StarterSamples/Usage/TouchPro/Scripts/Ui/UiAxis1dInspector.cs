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
using UnityEngine.UI;
using TMPro;

public class UiAxis1dInspector : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_minExtent = 0;

    [SerializeField] private float m_maxExtent = 1;

    [Header("Components")]
    [SerializeField] private TextMeshProUGUI m_nameLabel = null;

    [SerializeField] private TextMeshProUGUI m_valueLabel = null;
    [SerializeField] private Slider m_slider = null;

    public void SetExtents(float minExtent, float maxExtent)
    {
        m_minExtent = minExtent;
        m_maxExtent = maxExtent;
    }

    public void SetName(string name)
    {
        m_nameLabel.text = name;
    }

    public void SetValue(float value)
    {
        m_valueLabel.text = string.Format("[{0}]", value.ToString("f2"));

        m_slider.minValue = Mathf.Min(value, m_minExtent);
        m_slider.maxValue = Mathf.Max(value, m_maxExtent);

        m_slider.value = value;
    }
}
