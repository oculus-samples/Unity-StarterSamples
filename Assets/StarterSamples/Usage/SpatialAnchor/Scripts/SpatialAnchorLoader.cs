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

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Demonstrates loading existing spatial anchors from storage.
/// </summary>
/// <remarks>
/// Loading existing anchors involves two asynchronous methods:
/// 1. Call <see cref="OVRSpatialAnchor.LoadUnboundAnchorsAsync"/>
/// 2. For each unbound anchor you wish to localize, invoke <see cref="OVRSpatialAnchor.UnboundAnchor.Localize"/>.
/// 3. Once localized, your callback will receive an <see cref="OVRSpatialAnchor.UnboundAnchor"/>. Instantiate an
/// <see cref="OVRSpatialAnchor"/> component and bind it to the `UnboundAnchor` by calling
/// <see cref="OVRSpatialAnchor.UnboundAnchor.BindTo"/>.
/// </remarks>
public class SpatialAnchorLoader : MonoBehaviour
{
    [SerializeField]
    OVRSpatialAnchor _anchorPrefab;

    Action<bool, OVRSpatialAnchor.UnboundAnchor> _onAnchorLocalized;

    readonly List<OVRSpatialAnchor.UnboundAnchor> _unboundAnchors = new();

    public void LoadAnchorsByUuid()
    {
        var uuids = AnchorUuidStore.Uuids.ToArray();
        if (uuids.Length == 0)
        {
            LogWarning($"There are no anchors to load.");
            return;
        }

        Log($"Attempting to load {uuids.Length} anchors by UUID: " +
            $"{string.Join($", ", uuids.Select(uuid => uuid.ToString()))}");

        OVRSpatialAnchor.LoadUnboundAnchorsAsync(uuids, _unboundAnchors)
            .ContinueWith(result =>
            {
                if (result.Success)
                {
                    ProcessUnboundAnchors(result.Value);
                }
                else
                {
                    LogError($"{nameof(OVRSpatialAnchor.LoadUnboundAnchorsAsync)} failed with error {result.Status}.");
                }
            });
    }

    private void Awake()
    {
        _onAnchorLocalized = OnLocalized;
    }

    private void ProcessUnboundAnchors(IReadOnlyList<OVRSpatialAnchor.UnboundAnchor> unboundAnchors)
    {
        Log($"{nameof(OVRSpatialAnchor.LoadUnboundAnchorsAsync)} found {unboundAnchors.Count} unbound anchors: " +
            $"[{string.Join(", ", unboundAnchors.Select(a => a.Uuid.ToString()))}]");

        foreach (var anchor in unboundAnchors)
        {
            if (anchor.Localized)
            {
                _onAnchorLocalized(true, anchor);
            }
            else if (!anchor.Localizing)
            {
                anchor.LocalizeAsync().ContinueWith(_onAnchorLocalized, anchor);
            }
        }
    }

    private void OnLocalized(bool success, OVRSpatialAnchor.UnboundAnchor unboundAnchor)
    {
        if (!success)
        {
            LogError($"{unboundAnchor} Localization failed!");
            return;
        }

        var pose = unboundAnchor.Pose;
        var spatialAnchor = Instantiate(_anchorPrefab, pose.position, pose.rotation);
        unboundAnchor.BindTo(spatialAnchor);

        if (spatialAnchor.TryGetComponent<Anchor>(out var anchor))
        {
            // We just loaded it, so we know it exists in persistent storage.
            anchor.ShowSaveIcon = true;
        }
    }

    private static void Log(LogType logType, object message)
        => Debug.unityLogger.Log(logType, "[SpatialAnchorSample]", message);

    private static void Log(object message) => Log(LogType.Log, message);

    private static void LogWarning(object message) => Log(LogType.Warning, message);

    private static void LogError(object message) => Log(LogType.Error, message);
}
