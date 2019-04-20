using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System;

[CustomEditor(typeof(StateMachineController), true)]
public class StateMachineControllerEditor : Editor {

    bool parameters_foldout = false, states_foldout = false;

    public override void OnInspectorGUI() {
        SerializedObject serialized_target = new SerializedObject(target);
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), typeof(MonoScript), false);
        GUI.enabled = true;

        SerializedProperty state_machine_property = serialized_target.FindProperty("state_machine");
        state_machine_property.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent("State Machine"), state_machine_property.objectReferenceValue, typeof(StateMachine), false);

        if (state_machine_property.objectReferenceValue != null) {
            SerializedProperty parameters_property = new SerializedObject(state_machine_property.objectReferenceValue).FindProperty("parameters");
            SerializedProperty callback_list_property = serialized_target.FindProperty("parameter_callback_list");

            if (callback_list_property.arraySize != parameters_property.arraySize) {
                callback_list_property.ClearArray();
                for (int i = 0; i < parameters_property.arraySize; i++) {
                    callback_list_property.InsertArrayElementAtIndex(i);
                }
            }
            parameters_foldout = EditorGUILayout.Foldout(parameters_foldout, "Parameters");
            if (parameters_foldout) {
                EditorGUI.indentLevel += 1;
                for (int i = 0; i < callback_list_property.arraySize; i++) {
                    SerializedProperty array_element = callback_list_property.GetArrayElementAtIndex(i);
                    string parameter_name = parameters_property.GetArrayElementAtIndex(i).FindPropertyRelative("_name").stringValue;
                    SerializedProperty index = array_element.FindPropertyRelative("callback_index");

                    MemberInfo[] method_candidates = GetParameterCandidates(target.GetType());
                    string[] candidate_names = new string[method_candidates.Length];

                    for (int j = 0; j < candidate_names.Length; j++) {
                        candidate_names[j] = method_candidates[j].Name;
                    }

                    index.intValue = EditorGUILayout.Popup(parameter_name, index.intValue, candidate_names);

                    if (method_candidates.Length > index.intValue) {
                        array_element.FindPropertyRelative("callback_is_field").boolValue = (method_candidates[index.intValue].MemberType == MemberTypes.Field);
                        array_element.FindPropertyRelative("callback_name").stringValue = method_candidates[index.intValue].Name;
                        array_element.FindPropertyRelative("parameter").FindPropertyRelative("_name").stringValue = parameter_name;
                    }
                }
                EditorGUI.indentLevel -= 1;
            }

            SerializedProperty states_property = new SerializedObject(state_machine_property.objectReferenceValue).FindProperty("states");
            SerializedProperty coroutine_list_property = serialized_target.FindProperty("state_coroutine_list");

            if (coroutine_list_property.arraySize != states_property.arraySize) {
                coroutine_list_property.ClearArray();
                for (int i = 0; i < states_property.arraySize; i++) {
                    coroutine_list_property.InsertArrayElementAtIndex(i);
                }
            }
            states_foldout = EditorGUILayout.Foldout(states_foldout, "States");
            if (states_foldout) {
                EditorGUI.indentLevel += 1;
                for (int i = 0; i < coroutine_list_property.arraySize; i++) {
                    SerializedProperty array_element = coroutine_list_property.GetArrayElementAtIndex(i);
                    string state_name = states_property.GetArrayElementAtIndex(i).FindPropertyRelative("_name").stringValue;
                    SerializedProperty index = array_element.FindPropertyRelative("coroutine_index");

                    MemberInfo[] method_candidates = GetStateCandidates(target.GetType());
                    string[] candidate_names = new string[method_candidates.Length];

                    for (int j = 0; j < candidate_names.Length; j++) {
                        candidate_names[j] = method_candidates[j].Name;
                    }

                    index.intValue = EditorGUILayout.Popup(state_name, index.intValue, candidate_names);

                    if (method_candidates.Length > index.intValue) {
                        array_element.FindPropertyRelative("coroutine_name").stringValue = method_candidates[index.intValue].Name;
                        array_element.FindPropertyRelative("state").FindPropertyRelative("_name").stringValue = state_name;
                    }
                }
                EditorGUI.indentLevel -= 1;
            }
        }

        EditorGUILayout.Space();

        DrawPropertiesExcluding(serialized_target, new string[] { "state_machine", "list", "m_Script" });

        serialized_target.ApplyModifiedProperties();
    }

    MemberInfo[] GetParameterCandidates(Type type) {
        List<MemberInfo> member_info_list = new List<MemberInfo>();

        member_info_list.AddRange(type.FindMembers(MemberTypes.Method | MemberTypes.Field, BindingFlags.Instance | BindingFlags.Public, (a, b) => MatchParameterCandidate(a, b, type), null));

        return member_info_list.ToArray();
    }

    bool MatchParameterCandidate(MemberInfo member, object filter, Type type) {
        if (member.MemberType == MemberTypes.Method) {
            MethodInfo mi = type.GetMethod(member.Name, new Type[0]);
            if (type.BaseType != null && type.BaseType.GetMethod(member.Name, new Type[0]) != null) {
                return false;
            }

            return mi != null && mi.GetParameters().Length == 0 && mi.ReturnType == typeof(bool);
        }
        if (member.MemberType == MemberTypes.Field) {
            return (member as FieldInfo).FieldType == typeof(bool);
        }
        return false;
    }

    MemberInfo[] GetStateCandidates(Type type) {
        List<MemberInfo> member_info_list = new List<MemberInfo>();

        member_info_list.AddRange(type.FindMembers(MemberTypes.Method, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, (a, b) => MatchStateCandidate(a, b, type), null));

        return member_info_list.ToArray();
    }

    bool MatchStateCandidate(MemberInfo member, object filter, Type type) {
        if (member.MemberType == MemberTypes.Method) {
            MethodInfo mi = type.GetMethod(member.Name, new Type[0]);

            return mi != null && mi.GetParameters().Length == 0 && mi.ReturnType == typeof(IEnumerator);
        }
        return false;
    }
}
