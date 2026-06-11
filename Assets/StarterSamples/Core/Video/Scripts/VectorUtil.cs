// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections;
using System.Collections.Generic;
using Meta.XR.Samples;
using UnityEngine;

[MetaCodeSample("StarterSample.Core-Video")]
public static class VectorUtil
{
    public static Vector4 ToVector(this Rect rect)
    {
        return new Vector4(rect.x, rect.y, rect.width, rect.height);
    }
}
