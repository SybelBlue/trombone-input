namespace CustomInput
{
    public class StylusBinnedABCDE : Layout
    {
        public override string layoutName => "Stylus ABCDE";

        public override bool usesSlider => true;

        protected virtual int? InnerIndex(InputData data, int parentSize)
            => data.normalizedPotentiometer.HasValue ?
                (int?)Utils.NormalizedIntoIndex(1 - data.normalizedPotentiometer.Value, parentSize) :
                null;

        public override int ChildIndexFor(InputData data)
            => data.normalizedZ.HasValue ?
                Utils.NormalizedIntoIndex(data.normalizedZ.Value, childMap.Count) :
                -1;

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
            return inner == null ? (parent.label[0], false) : (inner.c, true);
        }

        public override void SetHighlightedKey(InputData data)
        {
            UnhighlightAll();

            var binnedKey = ChildFor(data)?.GetComponent<AmbiguousKeyController>();

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
                new AmbiguousKey(true,
                        new SimpleKey('A', 2, '1'),
                        new SimpleKey('B', 2, '4'),
                        new SimpleKey('C', 2, '7'),
                        new SimpleKey('D', 2, '*')
                ),
                new AmbiguousKey(true,
                        new SimpleKey('E', 2, '2'),
                        new SimpleKey('F', 2, '5'),
                        new SimpleKey('G', 2, '8'),
                        new SimpleKey('H', 2, '/')
                ),
                new AmbiguousKey(true,
                        new SimpleKey('I', 2, '3'),
                        new SimpleKey('J', 2, '6'),
                        new SimpleKey('K', 2, '9'),
                        new SimpleKey('L', 2, '.')
                ),
                new AmbiguousKey(true,
                        new SimpleKey('M', 2),
                        new SimpleKey('N', 2),
                        new SimpleKey('O', 2),
                        new SimpleKey('P', 2)
                ),
                new AmbiguousKey(true,
                        new SimpleKey('Q', 2),
                        new SimpleKey('R', 2),
                        new SimpleKey('S', 2),
                        new SimpleKey('T', 2)
                ),
                new AmbiguousKey(true,
                        new SimpleKey('U', 2),
                        new SimpleKey('V', 2),
                        new SimpleKey('W', 2),
                        new SimpleKey('X', 2)
                ),
                new AmbiguousKey(true,
                        new SimpleKey('Y', 2),
                        new SimpleKey('Z', 2)
                ),
        };
        }
    }
}
