using UnityEngine;
using UnityEditor;

public class Comment : MonoBehaviour
{ }

#if (UNITY_EDITOR) 
[CustomEditor(typeof(Comment))]
public class CommentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.TextArea("");
    }
}
#endif