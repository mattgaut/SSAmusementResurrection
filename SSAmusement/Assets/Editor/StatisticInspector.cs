using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Statistic), true)]
public class StatisticInspector : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        Statistic stat = target as Statistic;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(stat.name, stat.string_value);
    }
}
