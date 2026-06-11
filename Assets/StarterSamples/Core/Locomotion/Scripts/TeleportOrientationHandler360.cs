// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using System.Collections;
using Meta.XR.Samples;

/// <summary>
/// This orientation handler doesn't actually do anything with the orientation at all; this is for users
/// who have a 360 setup and don't need to be concerned with choosing an orientation because they just
/// turn whatever direction they want.
/// </summary>
[MetaCodeSample("StarterSample.Core-Locomotion")]
public class TeleportOrientationHandler360 : TeleportOrientationHandler
{
    protected override void InitializeTeleportDestination()
    {
    }

    protected override void UpdateTeleportDestination()
    {
        LocomotionTeleport.OnUpdateTeleportDestination(AimData.TargetValid, AimData.Destination, null, null);
    }
}
