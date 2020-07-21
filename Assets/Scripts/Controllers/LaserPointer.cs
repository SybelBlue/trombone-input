using UnityEngine;
using Utils.UnityExtensions;

namespace Controller
{
#pragma warning disable 108
#pragma warning disable 649
    public class LaserPointer : MonoBehaviour
    {
        [SerializeField]
        private Stylus modelController;

        private float lastLength = 0;

        public bool active
        {
            get { return gameObject.activeInHierarchy; }
            set { gameObject.SetActive(value); }
        }

        void Update()
        {
            if (!transform.hasChanged) return;

            RaycastHit? hit;
            modelController.Raycast(out hit);
            float length = Mathf.Max(0.005f, hit?.distance ?? 10);

            if (Mathf.Abs(lastLength - length) < 0.1f) return;
            lastLength = length;

            transform.localScale = transform.localScale.WithY(length);

            transform.localPosition = transform.localPosition.WithZ(length + 0.06f);
        }
    }
}