using UnityEngine;
using UnityEngine.UI;

namespace Controller
{
#pragma warning disable 649
    public class LayoutDropdown : IRaycastable
    {
        public Dropdown dropdown;

        [SerializeField]
        private Image image;

        [SerializeField]
        private Color highlightColor;

        private Color normalColor;


        private void Start()
        {
            normalColor = image.color;
        }

        protected override void OnRaycastFocusChange(bool value)
            => image.color = value ? highlightColor : normalColor;
    }
}