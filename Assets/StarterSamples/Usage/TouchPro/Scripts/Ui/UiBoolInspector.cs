// Copyright (c) Meta Platforms, Inc. and affiliates.

using Meta.XR.Samples;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[MetaCodeSample("StarterSample-TouchPro")]
public class UiBoolInspector : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI m_nameLabel = null;

    [SerializeField] private Toggle m_toggle = null;

    public void SetName(string name)
    {
        m_nameLabel.text = name;
    }

    public void SetValue(bool value)
    {
        m_toggle.isOn = value;
    }
}
