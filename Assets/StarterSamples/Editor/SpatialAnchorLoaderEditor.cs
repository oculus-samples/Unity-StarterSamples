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

using Meta.XR.Samples;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpatialAnchorLoader))]
[MetaCodeSample("StarterSample-Editor")]
class MetaAnchorEditorMenu : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("When you save an anchor at runtime, its UUID is stored in PlayerPrefs. When " +
                                "running this sample within the Editor over Link, the number of UUIDs can become large. " +
                                "You can clear the list of previously saved anchor UUIDs either from the menu " +
                                $"\"{MenuItemPath.Replace("/", " > ")}\" or by clicking this button:", MessageType.Info);
        if (GUILayout.Button("Clear Anchor UUIDs"))
        {
            ClearAnchorUuids();
        }

        GUILayout.Space(16);

        base.OnInspectorGUI();
    }

    const string MenuItemPath = "Meta/Samples/Clear Anchor UUIDs";

    [MenuItem(MenuItemPath, isValidateFunction: false)]
    static void ClearAnchorUuids()
    {
        var count = AnchorUuidStore.Count;
        AnchorUuidStore.Uuids = new();

        var s = count == 1 ? "" : "s";
        var they = count == 1 ? "it" : "they";
        EditorUtility.DisplayDialog("Anchor UUIDs",
            $"Removed {count} UUID{s} from PlayerPrefs. Note this does not erase the anchor{s} from device, " +
            $"but {they} will no longer be loaded by the anchor samples.", "Ok");
    }

    [MenuItem(MenuItemPath, isValidateFunction: true)]
    static bool Validate() => AnchorUuidStore.Count > 0;
}
