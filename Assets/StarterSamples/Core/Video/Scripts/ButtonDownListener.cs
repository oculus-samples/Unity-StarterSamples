// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections;
using System.Collections.Generic;
using Meta.XR.Samples;
using UnityEngine;
using UnityEngine.EventSystems;

[MetaCodeSample("StarterSample.Core-Video")]
public class ButtonDownListener : MonoBehaviour, UnityEngine.EventSystems.IPointerDownHandler
{
    public event System.Action onButtonDown;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (onButtonDown != null)
        {
            onButtonDown.Invoke();
        }
    }
}
