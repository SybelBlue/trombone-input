using UnityEngine;


namespace Controllers
{
    namespace Keys
    {
#pragma warning disable 649
        public class Simple : AbstractSimple
        {
            [SerializeField]
            private UnityEngine.UI.Text childText;

            public TextAnchor alignment
            {
                get => childText.alignment;
                set => childText.alignment = value;
            }

            public override string text
            {
                get => childText.text;
                set => childText.text = value;
            }
        }
    }
}
