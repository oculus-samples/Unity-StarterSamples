// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using System.Collections;
using Meta.XR.Samples;

/// <summary>
/// This transition will move the player with no other side effects.
/// </summary>
[MetaCodeSample("StarterSample.Core-Locomotion")]
public class TeleportTransitionInstant : TeleportTransition
{
    /// <summary>
    /// When the teleport state is entered, simply move the player to the new location
    /// without any delay or other side effects.
    /// </summary>
    protected override void LocomotionTeleportOnEnterStateTeleporting()
    {
        LocomotionTeleport.DoTeleport();
    }
}
