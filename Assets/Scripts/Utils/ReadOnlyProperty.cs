using System;
using UnityEditor;
using UnityEngine;

// authored by It3ertion on thread
// https://answers.unity.com/questions/489942/how-to-make-a-readonly-property-in-inspector.html

// public class ReadOnlyAttribute : PropertyAttribute
// { }

// [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
// public class ReadOnlyDrawer : PropertyDrawer
// {
//     public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//         => EditorGUI.GetPropertyHeight(property, label, true);

//     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//     {
//         GUI.enabled = false;
//         EditorGUI.PropertyField(position, property, label, true);
//         GUI.enabled = true;
//     }
// }


[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public class ReadOnlyAttribute : Attribute
{
    public int order { get; set; }
}

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyPropertyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        => EditorGUI.GetPropertyHeight(property, label, true);

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
