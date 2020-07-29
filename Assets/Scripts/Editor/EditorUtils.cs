using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Editor
{
    public static class Utils
    {
        // get the game object with the given tag or name
        public static GameObject GetGameObject(string name, string tag = null)
        {
            var go = tag == null ? GameObject.Find(name) : GameObject.FindGameObjectWithTag(tag);

            //find does not work on inactive objects, so check there
            if (go == null)
            {
                try
                {
                    go =
                        GetAllDisabledSceneObjects()
                        .Select(t => t.gameObject)
                        .First(disabled => disabled.name == name || disabled.tag == tag);
                }
                catch (System.InvalidOperationException) { } // no inactive objects in scene
            }

            // if still null make a new one, if tag not null, set new gameobject tag
            if (go == null)
            {
                go = new GameObject(name);
                if (tag != null)
                {
                    go.tag = tag;
                }
            }

            return go;
        }
        // Force the presence of (nth many) T on go
        public static T ForceComponent<T>(ref GameObject go, int nth = 0) where T : Component
        {
            var components = go.GetComponents<T>();
            var comp = components.Length <= nth ? null : components[nth];
            return comp ?? go.AddComponent<T>();
        }

        // thanks u/tonemcbride on 
        // https://forum.unity.com/threads/how-do-i-use-gameobject-find-to-find-an-inactive-object.428277/ 
        // for this disastrous solution to a problem that should not exist, ie
        // unity's find function does not target inactive objects in a heirarchy, 
        // like say an inactive VRDevice GameObject...
        private static Transform[] GetAllDisabledSceneObjects()
        {
            var allTransforms = Resources.FindObjectsOfTypeAll(typeof(Transform));
            var previousSelection = Selection.objects;
            Selection.objects = allTransforms.Cast<Transform>()
                .Where(x => x != null)
                .Select(x => x.gameObject)
                .Where(x => x != null && !x.activeInHierarchy)
                .Cast<Object>().ToArray();

            var selectedTransforms = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);
            Selection.objects = previousSelection;

            return selectedTransforms;
        }
    }
}
