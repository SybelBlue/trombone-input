using UnityEngine;

namespace Utils
{
    public abstract class IRaycastable : MonoBehaviour
    {
        public static IRaycastable last { get; private set; }

        public static IRaycastable current 
            => last && last.hasRaycastFocus ? last : null;

        private bool _inFocus;

        public bool hasRaycastFocus
        {
            get { return _inFocus; }
            private set
            {
                if (_inFocus != value)
                {
                    OnRaycastFocusChange(value);
                }

                if (value)
                {
                    if (last && last != this && last._inFocus)
                    {
                        last.hasRaycastFocus = false;
                    }

                    last = this;
                }

                _inFocus = value;
            }
        }

        protected abstract void OnRaycastFocusChange(bool value);

        public static IRaycastable Raycast(Vector3 origin, Vector3 direction, out RaycastHit? hit, float dist = Mathf.Infinity)
        {
            foreach (RaycastHit h in Physics.RaycastAll(origin, direction, dist))
            {
                IRaycastable raycastable = h.transform.gameObject.GetComponent<IRaycastable>();
                if (raycastable)
                {
                    raycastable.hasRaycastFocus = true;
                    hit = h;
                    return raycastable;
                }
            }

            // not found
            if (current)
            {
                current.hasRaycastFocus = false;
            }

            hit = null;
            return null;
        }
    }
}