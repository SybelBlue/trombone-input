using UnityEngine;
using UnityEditor;

namespace Utils
{
    public class Comment : MonoBehaviour
    { }
}

#if (UNITY_EDITOR)
namespace CustomEditor
{
    [UnityEditor.CustomEditor(typeof(Utils.Comment))]
    public class CommentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.TextArea("");
        }
    }
}
#endif