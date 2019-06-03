using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AnimParameterEvent))]
public class AnimParameterEventPropertyDrawer : PropertyDrawer {

    float height = 0f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        Rect rect = position;
        rect.height = EditorGUIUtility.singleLineHeight;
        Vector2 shift_vector = (rect.height + 2) * Vector2.up;

        bool folded_out = EditorGUI.PropertyField(rect, property, false);
        rect.position += shift_vector;

        if (folded_out) {
            EditorGUI.indentLevel++;

            SerializedProperty has_animation = property.FindPropertyRelative("has_animation");
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("has_animation"));
            rect.position += shift_vector;

            if (has_animation.boolValue) {
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("parameter_name"));
                rect.position += shift_vector;

                SerializedProperty type = property.FindPropertyRelative("type");
                EditorGUI.PropertyField(rect, type);
                rect.position += shift_vector;

                if (type.enumValueIndex == (int)AnimParameterEvent.Type.Bool) {
                    EditorGUI.PropertyField(rect, property.FindPropertyRelative("bool_value"));
                    rect.position += shift_vector;
                } else if (type.enumValueIndex == (int)AnimParameterEvent.Type.Float) {
                    EditorGUI.PropertyField(rect, property.FindPropertyRelative("float_value"));
                    rect.position += shift_vector;
                }
            }



            EditorGUI.indentLevel--;
        }

        height = rect.y - position.y;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return height;
    }
}
