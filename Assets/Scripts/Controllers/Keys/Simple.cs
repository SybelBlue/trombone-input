using UnityEngine;

namespace Controller.Key
{
#pragma warning disable 649
    public class Simple : AbstractSimple
    {
        [SerializeField]
        private UnityEngine.UI.Text childText;

        public TextAnchor alignment
        {
            get { return childText.alignment; }
            set { childText.alignment = value; }
        }

        public override string text
        {
            get { return childText.text; }
            set { childText.text = value; }
        }
    }
}
