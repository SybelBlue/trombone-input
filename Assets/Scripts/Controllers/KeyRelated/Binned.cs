using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    namespace Keys
    {
        public class Binned : AbstractBinned<CustomInput.BinnedKey>
        {
            public override void SetSlant(bool forward)
            {
                var children = transform.GetComponentsInChildren<Simple>();
                for (int i = 0; i < children.Length; i++)
                {
                    var child = children[i];
                    switch (i % 3)
                    {
                        case 0:
                            child.alignment = forward ? TextAnchor.UpperCenter : TextAnchor.LowerCenter;
                            break;
                        case 1:
                            child.alignment = TextAnchor.MiddleCenter;
                            break;
                        case 2:
                            child.alignment = forward ? TextAnchor.LowerCenter : TextAnchor.UpperCenter;
                            break;
                    }
                }
            }
        }

        public abstract class AbstractBinned<T> : Key<T>
            where T : CustomInput.BinnedKey
        {
            public Color highlightColor;

            protected Color normalColor;
            protected bool highlighting = false;
            public Image background;

            public override void SetHighlight(bool h)
            {
                if (h)
                {
                    normalColor = background.color;

                    background.color = highlightColor;
                }
                else if (highlighting)
                {
                    background.color = normalColor;
                }

                highlighting = h;
            }

            public bool useAlternate
            {
                set
                {
                    foreach (var controller in gameObject.GetComponentsInChildren<AbstractSimple>())
                    {
                        controller.useAlternate = value;
                    }
                }
            }

            public void AddChild(GameObject g)
            {
                g.transform.SetParent(transform);
            }

            public abstract void SetSlant(bool forward);
        }
    }
}