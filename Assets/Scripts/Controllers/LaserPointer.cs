using UnityEngine;

namespace Controller
{
#pragma warning disable 108
#pragma warning disable 649
    public class LaserPointer : MonoBehaviour
    {
        [SerializeField]
        private Stylus modelController;

        public bool active
        {
            get => gameObject.activeInHierarchy;
            set => gameObject.SetActive(value);
        }

        void Update()
        {
            if (!transform.hasChanged) return;

            RaycastHit? hit;
            modelController.Raycast(out hit);
            float length = Mathf.Max(0.005f, hit?.distance ?? 10);

            transform.localScale = new Vector3(0.005f, length, 0.005f);

            var pos = transform.position;
            pos.z = length / 2.0f + 0.06f;
            transform.position = pos;
        }
    }
}