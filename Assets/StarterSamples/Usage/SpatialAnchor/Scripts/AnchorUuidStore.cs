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
/// Manages UUIDs of persisted anchors
/// </summary>
/// <remarks>
/// In order to load anchors that were persisted in previous sessions, we need to remember their UUIDs. We could use
/// any mechanism to save these UUIDs, and Unity's
/// [PlayerPrefs](https://docs.unity3d.com/ScriptReference/PlayerPrefs.html) API provides a simple mechanism.
///
/// In a real app, you'd probably want to use a more sophisticated storage mechanism, but PlayerPrefs serves the
/// purposes required by this sample.
/// </remarks>
public static class AnchorUuidStore
{
    public const string NumUuidsPlayerPref = "numUuids";

    public static int Count => PlayerPrefs.GetInt(NumUuidsPlayerPref, 0);

    public static HashSet<Guid> Uuids
    {
        get => Enumerable
            .Range(0, Count)
            .Select(GetUuidKey)
            .Select(PlayerPrefs.GetString)
            .Select(str => Guid.TryParse(str, out var uuid) ? uuid : Guid.Empty)
            .Where(uuid => uuid != Guid.Empty)
            .ToHashSet();

        set
        {
            // Delete everything beyond the new count
            foreach (var key in Enumerable.Range(0, Count).Select(GetUuidKey))
            {
                PlayerPrefs.DeleteKey(key);
            }

            // Set the new count
            PlayerPrefs.SetInt(NumUuidsPlayerPref, value.Count);

            // Update all the uuids
            var index = 0;
            foreach (var uuid in value)
            {
                PlayerPrefs.SetString(GetUuidKey(index++), uuid.ToString());
            }
        }
    }

    public static void Add(Guid uuid)
    {
        var uuids = Uuids;
        if (uuids.Add(uuid))
        {
            Uuids = uuids;
        }
    }

    public static void Remove(Guid uuid)
    {
        var uuids = Uuids;
        if (uuids.Remove(uuid))
        {
            Uuids = uuids;
        }
    }

    static string GetUuidKey(int index) => $"uuid{index}";
}
