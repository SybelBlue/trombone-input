using UnityEngine;

namespace Controller
{
    // Defines the behavior of a raycastable button
#pragma warning disable 649
    public class RaycastableButton : Utils.IRaycastable
    {
        [SerializeField]
        private UnityEngine.UI.Image background;

        private Color prev;

        private bool highlighted = false;

        protected override void OnRaycastFocusChange(bool value)
        {
            if (value && !highlighted)
            {
                highlighted = true;
                prev = background.color;
                background.color = Color.gray;
            }
            else if (!value && highlighted)
            {
                highlighted = false;
                background.color = prev;
            }
        }
    }
}