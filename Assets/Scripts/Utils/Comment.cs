using UnityEngine;
using UnityEditor;

public class Comment : MonoBehaviour
{ }

[CustomEditor(typeof(Comment))]
public class CommentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.TextArea("");
    }
}