using Controller.Key;

namespace CustomInput
{
    public class StylusBinnedABCDE : Layout
    {
        public override bool usesSlider => true;
        public override bool usesRaycasting => false;

        private bool _useAlternate;

        public override bool useAlternate
        {
            get => _useAlternate;
            set
            {
                _useAlternate = value;
                foreach (var controller in gameObject.GetComponentsInChildren<StylusBinned>())
                {
                    controller.useAlternate = value;
                }
            }
        }

        protected virtual int? InnerIndex(InputData data, int parentSize)
            => data.normalizedSlider.HasValue ?
                (int?)Utils.Static.NormalizedIntoIndex(1 - data.normalizedSlider.Value, parentSize) :
                null;

        protected override int ChildIndexFor(InputData data)
            => Utils.Static.NormalizedIntoIndex(data.normalizedAngles.z, childMap.Count);

        private (LayoutKey, SimpleKey) FetchInnerKey(InputData data)
        {
            StylusBinnedKey parent = (StylusBinnedKey)LayoutKeyFor(data);

            var inner = InnerIndex(data, parent.size);

            if (!inner.HasValue || parent.size == 0) return (parent, null);

            return (parent, parent.ItemAt(inner.Value));
        }

        public override (LayoutKey, SimpleKey)? KeysFor(InputData data)
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
            var (parent, inner) = FetchInnerKey(data);
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
        protected override LayoutKey[] FillKeys()
        {
            return new LayoutKey[] {
                new StylusBinnedKey(true,
                        new StylusKey('A', 4, '1'),
                        new StylusKey('B', 4, '4'),
                        new StylusKey('C', 4, '7'),
                        new StylusKey('D', 4, '*')
                ),
                new StylusBinnedKey(true,
                        new StylusKey('E', 4, '2'),
                        new StylusKey('F', 4, '5'),
                        new StylusKey('G', 4, '8'),
                        new StylusKey('H', 4, '+')
                ),
                new StylusBinnedKey(true,
                        new StylusKey('I', 4, '3'),
                        new StylusKey('J', 4, '6'),
                        new StylusKey('K', 4, '9'),
                        new StylusKey('L', 4, '.')
                ),
                new StylusBinnedKey(true,
                        new StylusKey('M', 4, '/'),
                        new StylusKey('N', 4, '%'),
                        new StylusKey('O', 4, '#'),
                        new StylusKey('P', 4, '(')
                ),
                new StylusBinnedKey(true,
                        new StylusKey('Q', 4, '@'),
                        new StylusKey('R', 4, '\''),
                        new StylusKey('S', 4, '\"'),
                        new StylusKey('T', 4, ')')
                ),
                new StylusBinnedKey(true,
                        new StylusKey('U', 4, '-'),
                        new StylusKey('V', 4, '&'),
                        new StylusKey('W', 4, '?'),
                        new StylusKey('X', 4, '!')
                ),
                new StylusBinnedKey(true,
                        new StylusKey('Y', 4, ';'),
                        new StylusKey('Z', 4, ':'),
                        new StylusKey('.', 4, ','),
                        new StylusKey(' ', 4, '\b')
                ),
            };
        }
    }
}
