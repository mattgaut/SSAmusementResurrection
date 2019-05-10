using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LootTable))]
public class LootTableEditor : Editor {
    public override void OnInspectorGUI() {

        EditorGUILayout.BeginHorizontal();

        SerializedProperty property = serializedObject.FindProperty("min_to_drop");
        GUILayout.Label("Min");
        property.intValue = EditorGUILayout.IntField(property.intValue);

        property = serializedObject.FindProperty("max_to_drop");
        GUILayout.Label("Max");
        property.intValue = EditorGUILayout.IntField(property.intValue);

        property = serializedObject.FindProperty("avg_to_drop");
        GUILayout.Label("Avg");
        property.floatValue = EditorGUILayout.FloatField(property.floatValue);

        EditorGUILayout.EndHorizontal();

        SerializedProperty possible_categories_property = serializedObject.FindProperty("possible_pickup_categories");

        EditorGUILayout.PropertyField(possible_categories_property, new GUIContent("Loot Categories"), false);
        
        if (possible_categories_property.isExpanded) {
            EditorGUI.indentLevel += 1;
            SerializedProperty array_size = possible_categories_property.Copy();
            array_size.NextVisible(true);
            EditorGUILayout.PropertyField(array_size);
            for (int i = 0; i < possible_categories_property.arraySize; i++) {
                DisplayCategory(possible_categories_property.GetArrayElementAtIndex(i));
            }
            EditorGUI.indentLevel -= 1;
        }


        serializedObject.ApplyModifiedProperties();
    }

    void DisplayCategory(SerializedProperty category) {
        
        EditorGUILayout.PropertyField(category, new GUIContent(category.displayName.Replace("Element", "Category")), false);

        if (category.isExpanded) {
            EditorGUI.indentLevel += 1;
            SerializedProperty property = category.FindPropertyRelative("_chance");        
            property.floatValue = EditorGUILayout.FloatField("Chance For Category", property.floatValue);

            property = category.FindPropertyRelative("possible_pickups");
            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, "Loot Chances", true);
            if (property.isExpanded) {
                EditorGUI.indentLevel += 1;
                property.NextVisible(true);
                EditorGUILayout.PropertyField(property);
                property = category.FindPropertyRelative("possible_pickups");
                for (int i = 0; i < property.arraySize; i++) {
                    DisplayPickupChance(property.GetArrayElementAtIndex(i));
                }
                EditorGUI.indentLevel -= 1;
            }
            EditorGUI.indentLevel -= 1;
        }

    }

    void DisplayPickupChance(SerializedProperty pickup_chance) {

        EditorGUILayout.PropertyField(pickup_chance, new GUIContent(pickup_chance.displayName.Replace("Element", "Pickup")), false);

        if (pickup_chance.isExpanded) {
            EditorGUI.indentLevel += 1;
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.label);
            rect.width = EditorGUIUtility.currentViewWidth/2f;

            SerializedProperty property = pickup_chance.FindPropertyRelative("_chance");
            property.floatValue = EditorGUI.FloatField(rect, "Pickup Chance", property.floatValue);
            rect.x += rect.width - 30;

            property = pickup_chance.FindPropertyRelative("loot");
            property.objectReferenceValue = EditorGUI.ObjectField(rect, property.objectReferenceValue, typeof(Pickup), false);
            EditorGUI.indentLevel -= 1;
        }
    }
} 
