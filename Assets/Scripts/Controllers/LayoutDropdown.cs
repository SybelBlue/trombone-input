using UnityEngine;
using UnityEngine.UI;

namespace Controller
{
    // The controller for the dropdown in the Testing UI
#pragma warning disable 649
    public class LayoutDropdown : Utils.IRaycastable
    {
        public Dropdown dropdown;

        [SerializeField]
        private Image image;

        [SerializeField]
        private Color highlightColor;

        private Color normalColor;


        private void Start()
            => normalColor = image.color;

        protected override void OnRaycastFocusChange(bool value)
            => image.color = value ? highlightColor : normalColor;
    }
}