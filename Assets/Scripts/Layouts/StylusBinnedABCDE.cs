namespace CustomInput
{
    public class StylusBinnedABCDE : Layout
    {
        public override string layoutName => "Stylus ABCDE";

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

        public override (char, bool)? GetLetterFor(InputData data)
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
            var controllers = binnedKey.GetComponentsInChildren<AbstractSimpleKeyController>();
            var inner = InnerIndex(data, controllers.Length);

            if (!inner.HasValue) return;

            for (int i = 0; i < controllers.Length; i++)
            {
                controllers[i].SetHighlight(i == inner);
            }
        }

        // Auto-generated
        protected override LayoutKey[] FillKeys()
        {
            return new LayoutKey[] {
                    new StylusBinnedKey(true,
                            new StylusKey('A', 2),
                            new StylusKey('B', 2),
                            new StylusKey('C', 2),
                            new StylusKey('D', 2)
                            ),
                    new StylusBinnedKey(true,
                            new StylusKey('E', 2),
                            new StylusKey('F', 2),
                            new StylusKey('G', 2),
                            new StylusKey('H', 2)
                            ),
                    new StylusBinnedKey(true,
                            new StylusKey('I', 2),
                            new StylusKey('J', 2),
                            new StylusKey('K', 2),
                            new StylusKey('L', 2)
                            ),
                    new StylusBinnedKey(true,
                            new StylusKey('M', 2),
                            new StylusKey('N', 2),
                            new StylusKey('O', 2),
                            new StylusKey('P', 2)
                            ),
                    new StylusBinnedKey(true,
                            new StylusKey('Q', 2),
                            new StylusKey('R', 2),
                            new StylusKey('S', 2),
                            new StylusKey('T', 2)
                            ),
                    new StylusBinnedKey(true,
                            new StylusKey('U', 2),
                            new StylusKey('V', 2),
                            new StylusKey('W', 2),
                            new StylusKey('X', 2)
                            ),
                    new StylusBinnedKey(true,
                            new StylusKey('Y', 2),
                            new StylusKey('Z', 2)
                            )
            };
        }
    }
}
