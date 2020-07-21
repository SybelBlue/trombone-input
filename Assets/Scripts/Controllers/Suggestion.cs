using UnityEngine;

namespace Controller
{
#pragma warning disable 649
    public class Suggestion : Utils.IRaycastable
    {
        [SerializeField]
        private TMPro.TMP_Text tmpText;

        [SerializeField]
        private TextOutputDisplay textOutputController;

        [SerializeField]
        public BoxCollider boxCollider;

        public string text
        {
            get { return tmpText.text; }
            set { tmpText.text = value; }
        }

        protected override void OnRaycastFocusChange(bool value)
            => tmpText.fontStyle =
                value ?
                    TMPro.FontStyles.Bold :
                    TMPro.FontStyles.Italic;
    }
}