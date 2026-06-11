// Copyright (c) Meta Platforms, Inc. and affiliates.

using Meta.XR.Samples;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshProUGUI))]
[MetaCodeSample("StarterSample-Passthrough")]
public class SliderValueToText : MonoBehaviour
{
    private TextMeshProUGUI textSliderValue;

    [SerializeField] private Slider sliderUI;

    private void Awake()
    {
        textSliderValue = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        ShowSliderValue();
    }

    public void ShowSliderValue()
    {
        if (textSliderValue == null || sliderUI == null)
            return;

        textSliderValue.text = $"{sliderUI.value:n2}";
    }
}
