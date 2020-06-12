namespace CustomInput
{
    public class StylusBinnedABCDE : Layout
    {
        public override bool usesSlider => true;

        private bool _useAlternate;

        public override bool useAlternate
        {
            get => _useAlternate;
            set
            {
                _useAlternate = value;
                foreach (var controller in gameObject.GetComponentsInChildren<StylusBinnedController>())
                {
                    controller.useAlternate = value;
                }
            }
        }

        protected virtual int? InnerIndex(InputData data, int parentSize)
            => data.normalizedSlider.HasValue ?
                (int?)Utils.NormalizedIntoIndex(1 - data.normalizedSlider.Value, parentSize) :
                null;

        public override int ChildIndexFor(InputData data)
            => Utils.NormalizedIntoIndex(data.normalizedZ, childMap.Count);

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

        public override (char, bool)? GetSelectedLetter(InputData data)
        {
            var (parent, inner) = FetchInnerKey(data);
            return inner == null ? (parent.label[0], false) : (inner.CharWithAlternate(useAlternate), true);
        }

        public override void SetHighlightedKey(InputData data)
        {
            UnhighlightAll();

            var binnedKey = ChildFor(data)?.GetComponent<StylusBinnedController>();

            if (binnedKey == null) return;

            binnedKey.SetHighlight(true);
            var controllers = binnedKey.GetComponentsInChildren<StylusKeyController>();
            var inner = InnerIndex(data, binnedKey.data.size);

            if (!inner.HasValue) return;

            var highlightedData = binnedKey.data.ItemAt(inner.Value);

            foreach (var cont in controllers)
            {
                cont.SetHighlight(cont.data.label == highlightedData.label);
            }
        }

        // Auto-generated 
        protected override LayoutKey[] FillKeys()
        {
            return new LayoutKey[] {
                new StylusBinnedKey(true,
                        new StylusKey('A', 2, '1'),
                        new StylusKey('B', 2, '4'),
                        new StylusKey('C', 2, '7'),
                        new StylusKey('D', 2, '*')
                ),
                new StylusBinnedKey(true,
                        new StylusKey('E', 2, '2'),
                        new StylusKey('F', 2, '5'),
                        new StylusKey('G', 2, '8'),
                        new StylusKey('H', 2, '+')
                ),
                new StylusBinnedKey(true,
                        new StylusKey('I', 2, '3'),
                        new StylusKey('J', 2, '6'),
                        new StylusKey('K', 2, '9'),
                        new StylusKey('L', 2, '.')
                ),
                new StylusBinnedKey(true,
                        new StylusKey('M', 2, '/'),
                        new StylusKey('N', 2, '%'),
                        new StylusKey('O', 2, '#'),
                        new StylusKey('P', 2, '(')
                ),
                new StylusBinnedKey(true,
                        new StylusKey('Q', 2, '@'),
                        new StylusKey('R', 2, '\''),
                        new StylusKey('S', 2, '\"'),
                        new StylusKey('T', 2, ')')
                ),
                new StylusBinnedKey(true,
                        new StylusKey('U', 2, '-'),
                        new StylusKey('V', 2, '&'),
                        new StylusKey('W', 2, '?'),
                        new StylusKey('X', 2, '!')
                ),
                new StylusBinnedKey(true,
                        new StylusKey('Y', 2, ';'),
                        new StylusKey('Z', 2, ':'),
                        new StylusKey('.', 3, ','),
                        new StylusKey(' ', 3, '\b')
                ),
            };
        }
    }
}