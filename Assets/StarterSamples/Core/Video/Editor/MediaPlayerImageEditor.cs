// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections;
using System.Collections.Generic;
using Meta.XR.Samples;
using UnityEngine;
using UnityEditor.UI;
using UnityEditor;

[CustomEditor(typeof(MediaPlayerImage), true)]
[MetaCodeSample("StarterSample.Core-Video.Editor")]
public class MediaPlayerImageEditor : ImageEditor
{
    SerializedProperty m_ButtonType;

    protected override void OnEnable()
    {
        base.OnEnable();

        m_ButtonType = serializedObject.FindProperty("m_ButtonType");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.PropertyField(m_ButtonType);
        serializedObject.ApplyModifiedProperties();
    }
}
