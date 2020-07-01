using Controller.Key;
using UnityEngine;
using UnityEngine.UI;
using CustomInput.KeyData;
using Utils.UnityExtensions;

namespace CustomInput
{

    namespace Layout
    {
#pragma warning disable 649
        public sealed class RaycastQWERTY : AbstractLayout
        {
            public override bool usesSlider => false;
            public override bool usesRaycasting => true;

            private bool _useAlternate;

            [SerializeField]
            private GridLayoutGroup gridLayout;

            public override bool useAlternate
            {
                get => _useAlternate;
                set
                {
                    _useAlternate = value;
                    foreach (var controller in gameObject.GetComponentsInChildren<Stylus>())
                    {
                        controller.useAlternate = value;
                    }
                }
            }

            public override void ResizeAll()
            {
                base.ResizeAll();
                gridLayout.cellSize = new Vector2(rectTransform.rect.width / 10.0f, rectTransform.rect.height / 3.0f);
                foreach (var collider in GetComponentsInChildren<BoxCollider>())
                {
                    collider.size = gridLayout.cellSize.WithZ(0.2f);
                }
            }

            protected override int ChildIndexFor(InputData data)
            {
                throw new System.InvalidOperationException("Should not be used");
            }

            public override GameObject ChildFor(InputData data)
            {
                // should be UI layer
                (Vector3 origin, Vector3 direction) = data.orientation;

                foreach (RaycastHit hit in Physics.RaycastAll(origin, direction, Mathf.Infinity))
                {
                    if (hit.transform.gameObject.GetComponent<Raycast>())
                    {
                        return hit.transform.gameObject;
                    }
                    else
                    {
                        Debug.DrawLine(data.orientation.origin, hit.point, Color.red, 0.2f);
                    }
                }

                return null;
            }

            public override char? GetSelectedLetter(InputData data)
            {
                var raycastKey = RaycastKeyDataFor(data);
                if (raycastKey == null) return null;
                return raycastKey.CharWithAlternate(useAlternate);
            }

            private SimpleData RaycastKeyDataFor(InputData data)
                => ChildFor(data)?.GetComponent<Raycast>().data;

            public override (AbstractData parent, SimpleData simple)? KeysFor(InputData data)
            {
                var raycastKey = RaycastKeyDataFor(data);
                if (raycastKey == null) return null;
                return (raycastKey, raycastKey);
            }

            public override void SetHighlightedKey(InputData data)
            {
                UnhighlightAll();

                var controller = ChildFor(data)?.GetComponent<Raycast>();
                if (controller == null) return;

                controller.SetHighlight(true);
            }


            // Auto-generated 
            protected override AbstractData[] FillKeys()
            {
                return new AbstractData[] {
                new RaycastData('Q', 2, '1'),
                new RaycastData('W', 2, '2'),
                new RaycastData('E', 2, '3'),
                new RaycastData('R', 2, '4'),
                new RaycastData('T', 2, '5'),
                new RaycastData('Y', 2, '6'),
                new RaycastData('U', 2, '7'),
                new RaycastData('I', 2, '8'),
                new RaycastData('O', 2, '9'),
                new RaycastData('P', 2, '0'),
                new RaycastData('A', 2, '!'),
                new RaycastData('S', 2, '@'),
                new RaycastData('D', 2, '#'),
                new RaycastData('F', 2, '$'),
                new RaycastData('G', 2, '%'),
                new RaycastData('H', 2, '^'),
                new RaycastData('J', 2, '&'),
                new RaycastData('K', 2, '*'),
                new RaycastData('L', 2, '!'),
                new RaycastData(';', 2, ':'),
                new RaycastData('Z', 2, '('),
                new RaycastData('X', 2, ')'),
                new RaycastData('C', 2, '\\'),
                new RaycastData('V', 2, '/'),
                new RaycastData('B', 2, '\''),
                new RaycastData('N', 2, '\"'),
                new RaycastData('M', 2, '-'),
                new RaycastData(',', 2, '?'),
                new RaycastData('.', 2, '.'),
                new RaycastData(' ', 2, '\b'),
            };
            }
        }
    }
}