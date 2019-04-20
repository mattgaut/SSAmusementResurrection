using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomEditor))]
public class RoomEditorEditor : Editor {

    RoomEditor room_editor;

    private void OnEnable() {
        room_editor = (RoomEditor)serializedObject.targetObject;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (GUILayout.Button("Open Editor")) {
            RoomEditorWindow.ShowWindow(room_editor);
        }
    }

    private void OnDisable() {

    }
}
