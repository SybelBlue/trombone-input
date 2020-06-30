using UnityEngine;

namespace Controllers
{
#pragma warning disable 649
    public class Suggestion : IRaycastable
    {
        [SerializeField]
        private TMPro.TMP_Text tmpText;

        [SerializeField]
        private TextOutputDisplay textOutputController;

        [SerializeField]
        public BoxCollider boxCollider;

        public string text
        {
            get => tmpText.text;
            set => tmpText.text = value;
        }

        protected override void OnRaycastFocusChange(bool value)
            => tmpText.fontStyle =
                value ?
                    TMPro.FontStyles.Bold :
                    TMPro.FontStyles.Italic;
    }
}