using UnityEngine;

namespace MinVR
{
    public class FPS : MonoBehaviour
    {
        public Color textColor = Color.white;

        [Tooltip("Prefab of textmesh to use to display fps.")]
        public GameObject labelPrefab;

        [Tooltip("Label Position in world space")]
        public Vector3 labelPosition;

        private TextMesh label;

        void Start()
        {
            if (labelPrefab)
            {
                label = Instantiate(labelPrefab).GetComponent<TextMesh>();
            }
            else
            {
                label = gameObject.GetComponentInChildren<TextMesh>();
            }

            if (label)
            {
                label.gameObject.transform.position = labelPosition;
                label.color = textColor;
            }
            else
            {
                Debug.LogError("FPS script requires a prefab or a child with a TextMesh component to render to.");
                Destroy(this);
            }
        }

        void Update()
        {
            if (label != null)
            {
                float fps = 1.0f / Time.smoothDeltaTime;
                string text = string.Format("{0:0.} fps", fps);
                label.text = text;
            }
        }

        void OnGUI()
        {
            int w = Screen.width;
            int h = Screen.height;
            GUIStyle style = new GUIStyle();
            Rect rect = new Rect(0, 0, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = textColor;
            float fps = 1.0f / Time.smoothDeltaTime;
            string text = string.Format("{0:0.} fps", fps);
            GUI.Label(rect, text, style);
        }
    }
}