using UnityEngine;

namespace CustomInput
{
#pragma warning disable 649
    public sealed class RaycastQWERTY : Layout
    {
        public override bool usesSlider => false;

        private bool _useAlternate;

        [SerializeField]
        private RectTransform rectTransform;

        [SerializeField]
        private GameObject rowPrefab;

        public override bool useAlternate
        {
            get => _useAlternate;
            set
            {
                _useAlternate = value;
                foreach (var controller in gameObject.GetComponentsInChildren<StylusKeyController>())
                {
                    controller.useAlternate = value;
                }
            }
        }

        private GameObject[] rows;

        protected override void BeforeStart()
        {
            rows = new GameObject[3];
            for (int i = 0; i < rows.Length; i++)
            {
                rows[i] = Instantiate(rowPrefab, transform);
            }
        }

        protected override Transform ParentForNthChild(int n)
        {
            return rows[n / 10].transform;
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
                if (hit.transform.gameObject.GetComponent<RaycastKeyController>())
                {
                    Debug.Log("hit!");
                    return hit.transform.gameObject;
                }
                else
                {
                    Debug.DrawLine(data.orientation.origin, hit.point, Color.red, 0.2f);
                }
            }

            return null;
        }

        public override (char letter, bool certain)? GetSelectedLetter(InputData data)
        {
            var raycastKey = RaycastKeyFor(data);
            if (raycastKey == null) return null;
            char c = raycastKey.CharWithAlternate(useAlternate);
            return (c, true);
        }

        private SimpleKey RaycastKeyFor(InputData data)
            => ChildFor(data)?.GetComponent<RaycastKeyController>().data;

        public override (LayoutKey parent, SimpleKey simple)? KeysFor(InputData data)
        {
            var raycastKey = RaycastKeyFor(data);
            if (raycastKey == null) return null;
            return (raycastKey, raycastKey);
        }

        public override void SetHighlightedKey(InputData data)
        {
            UnhighlightAll();

            var controller = ChildFor(data)?.GetComponent<RaycastKeyController>();
            if (controller == null) return;

            controller.SetHighlight(true);
        }


        // Auto-generated 
        protected override LayoutKey[] FillKeys()
        {
            return new LayoutKey[] {
                new RaycastKey('Q', 2, '1'),
                new RaycastKey('W', 2, '2'),
                new RaycastKey('E', 2, '3'),
                new RaycastKey('R', 2, '4'),
                new RaycastKey('T', 2, '5'),
                new RaycastKey('Y', 2, '6'),
                new RaycastKey('U', 2, '7'),
                new RaycastKey('I', 2, '8'),
                new RaycastKey('O', 2, '9'),
                new RaycastKey('P', 2, '0'),
                new RaycastKey('A', 2, '!'),
                new RaycastKey('S', 2, '@'),
                new RaycastKey('D', 2, '#'),
                new RaycastKey('F', 2, '$'),
                new RaycastKey('G', 2, '%'),
                new RaycastKey('H', 2, '^'),
                new RaycastKey('J', 2, '&'),
                new RaycastKey('K', 2, '*'),
                new RaycastKey('L', 2, '!'),
                new RaycastKey(';', 2, ':'),
                new RaycastKey('Z', 2, '('),
                new RaycastKey('X', 2, ')'),
                new RaycastKey('C', 2, '\\'),
                new RaycastKey('V', 2, '/'),
                new RaycastKey('B', 2, '\''),
                new RaycastKey('N', 2, '\"'),
                new RaycastKey('M', 2, '-'),
                new RaycastKey(',', 2, '?'),
                new RaycastKey('.', 2, '.'),
                new RaycastKey(' ', 2, ' '),
            };
        }
    }
}