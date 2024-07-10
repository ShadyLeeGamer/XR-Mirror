using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Observer<>))]
public class ObserverDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, GetValueProperty(property), new GUIContent($"{label} (Observed)"), true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(GetValueProperty(property), label, true);
    }

    SerializedProperty GetValueProperty(SerializedProperty property) => property.FindPropertyRelative("value");
}
