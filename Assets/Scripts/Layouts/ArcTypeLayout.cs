using Controller.Key;
using CustomInput.KeyData;
using UnityEngine;

namespace CustomInput.Layout
{
    public class ArcTypeLayout : AbstractLayout
    {
        public override bool usesSlider => true;
        public override bool keyOnFingerUp => true;
        public override bool usesRaycasting => false;

        [SerializeField]
        protected Vector3 minAngle, maxAngle;

        private bool _useAlternate;

        public override bool useAlternate
        {
            get { return _useAlternate; }
            set
            {
                _useAlternate = value;
                foreach (var controller in gameObject.GetComponentsInChildren<StylusBinned>())
                {
                    controller.useAlternate = value;
                }
            }
        }

        public override (Vector3 minima, Vector3 maxima)? StylusRotationBounds()
            => (minAngle, maxAngle);

        protected virtual int? InnerIndex(InputData data, int parentSize)
            => data.normalizedSlider.HasValue ?
                (int?)Utils.Static.NormalizedIntoIndex(1 - data.normalizedSlider.Value, parentSize) :
                null;

        protected override int ChildIndexFor(InputData data)
            => Utils.Static.NormalizedIntoIndex(data.normalizedAngles.z, childMap.Count);

        private (AbstractData, SimpleData) FetchInnerKey(InputData data)
        {
            StylusBinnedData parent = (StylusBinnedData)LayoutKeyFor(data);

            var inner = InnerIndex(data, parent.size);

            if (!inner.HasValue || parent.size == 0) return (parent, null);

            return (parent, parent.ItemAt(inner.Value));
        }

        public override (AbstractData, SimpleData)? KeysFor(InputData data)
        {
            var (parent, inner) = FetchInnerKey(data);
            if (inner != null)
            {
                return (parent, inner);
            }
            return null;
        }

        public override char? GetSelectedLetter(InputData data)
        {
            var (_, inner) = FetchInnerKey(data);
            if (inner == null) return null;
            return inner.CharWithAlternate(useAlternate);
        }

        public override void SetHighlightedKey(InputData data)
        {
            UnhighlightAll();

            var binnedKey = ChildFor(data)?.GetComponent<StylusBinned>();

            if (binnedKey == null) return;

            binnedKey.SetHighlight(true);
            var Controller = binnedKey.GetComponentsInChildren<Stylus>();
            var inner = InnerIndex(data, binnedKey.data.size);

            if (!inner.HasValue) return;

            var highlightedData = binnedKey.data.ItemAt(inner.Value);

            foreach (var cont in Controller)
            {
                cont.SetHighlight(cont.data.label == highlightedData.label);
            }
        }

        // Auto-generated
        protected override AbstractData[] FillKeys()
        {
            return new AbstractData[] {
            new StylusBinnedData(true,
                    new StylusData('A', 4, '1'),
                    new StylusData('B', 4, '4'),
                    new StylusData('C', 4, '7'),
                    new StylusData('D', 4, '*')
            ),
            new StylusBinnedData(true,
                    new StylusData('E', 4, '2'),
                    new StylusData('F', 4, '5'),
                    new StylusData('G', 4, '8'),
                    new StylusData('H', 4, '+')
            ),
            new StylusBinnedData(true,
                    new StylusData('I', 4, '3'),
                    new StylusData('J', 4, '6'),
                    new StylusData('K', 4, '9'),
                    new StylusData('L', 4, '0')
            ),
            new StylusBinnedData(true,
                    new StylusData('M', 4, '/'),
                    new StylusData('N', 4, '%'),
                    new StylusData('O', 4, '#'),
                    new StylusData('P', 4, '(')
            ),
            new StylusBinnedData(true,
                    new StylusData('Q', 4, '@'),
                    new StylusData('R', 4, '\''),
                    new StylusData('S', 4, '\"'),
                    new StylusData('T', 4, ')')
            ),
            new StylusBinnedData(true,
                    new StylusData('U', 4, '-'),
                    new StylusData('V', 4, '&'),
                    new StylusData('W', 4, '?'),
                    new StylusData('X', 4, '!')
            ),
            new StylusBinnedData(true,
                    new StylusData('Y', 4, ';'),
                    new StylusData('Z', 4, ':'),
                    new StylusData('.', 4, ','),
                    new StylusData(' ', 4, '\b')
            ),
        };
        }
    }
}