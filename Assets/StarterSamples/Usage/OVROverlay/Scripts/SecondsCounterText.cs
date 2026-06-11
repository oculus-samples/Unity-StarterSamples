// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SecondsCounterText : MonoBehaviour
{
    public UnityEvent<string> SetText;

    protected void Update()
    {
        SetText?.Invoke($"Seconds since start: {(int)Time.time}");
    }
}
