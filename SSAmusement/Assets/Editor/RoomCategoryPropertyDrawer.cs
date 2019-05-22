using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Level.WeightedRoomGroup))]
public class RoomCategoryPropertyDrawer : PropertyDrawer {

    bool is_out;
    float height;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        Rect rect = position;
        rect.height = EditorGUIUtility.singleLineHeight;
        Vector2 shift_vector = (rect.height + 2) * Vector2.up;

        GUIContent override_gui = EditorGUI.BeginProperty(rect, new GUIContent(""), property);
        is_out = EditorGUI.Foldout(rect, is_out, label, true);
        EditorGUI.EndProperty();
        rect.position += shift_vector;

        if (is_out) {
            EditorGUI.indentLevel += 1;

            SerializedProperty child = property.FindPropertyRelative("use_range");
            override_gui = EditorGUI.BeginProperty(rect, new GUIContent(""), child);
            Rect toggle_rect = EditorGUI.PrefixLabel(rect, new GUIContent("Use Fixed Value"));
            child.boolValue = !GUI.Toggle(toggle_rect, !child.boolValue, override_gui);
            EditorGUI.EndProperty();
            rect.position += shift_vector;

            if (child.boolValue) {
                child = property.FindPropertyRelative("min");
                EditorGUI.PropertyField(rect, child, new GUIContent("Min"));
                rect.position += shift_vector;

                child = property.FindPropertyRelative("max");
                EditorGUI.PropertyField(rect, child, new GUIContent("Max"));
                rect.position += shift_vector;

                child = property.FindPropertyRelative("avg");
                EditorGUI.PropertyField(rect, child, new GUIContent("Average"));
                rect.position += shift_vector;
            } else {
                child = property.FindPropertyRelative("fixed_number");
                EditorGUI.PropertyField(rect, child, new GUIContent("Spawn Amount"));
                rect.position += shift_vector;
            }

            child = property.FindPropertyRelative("_rooms");
            bool expand = EditorGUI.PropertyField(rect, child);
            rect.position += shift_vector;
            if (expand) {
                EditorGUI.indentLevel += 1;
                child.NextVisible(true);
                EditorGUI.PropertyField(rect, child, new GUIContent("Size"));
                rect.position += shift_vector;

                child = property.FindPropertyRelative("_rooms");

                for (int i = 0; i < child.arraySize; i++) {
                    SerializedProperty array_child = child.GetArrayElementAtIndex(i);
                    EditorGUI.PropertyField(rect, array_child, new GUIContent("Room " + i));
                    rect.position += shift_vector;
                }

                EditorGUI.indentLevel -= 1;
            }

            EditorGUI.indentLevel -= 1;
        }

        height = rect.y - position.y;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return height;
    }
}
