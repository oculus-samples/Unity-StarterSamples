// Copyright (c) Meta Platforms, Inc. and affiliates.

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
