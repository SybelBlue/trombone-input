
using UnityEngine;

namespace CustomInput
{
    public class StylusBinnedABCDE : SquashedQWERTY
    {
        public override string layoutName => "Stylus ABCDE";

        public override void UpdateState(InputData data)
        {
            if (data.normalizedZ.HasValue)
            {
                int key = Mathf.RoundToInt(data.normalizedZ.Value * InputData.MAX_RAW_VALUE);
                SetHighlightedKey(key);
            }
            else
            {
                SetHighlightedKey(null);
            }
        }

        public override void SetHighlightedKey(int? index)
        {
            base.SetHighlightedKey(index);

            if (index.HasValue)
            {
                Debug.Log("Expanding " + Utils.DisplayKeyData(LayoutKeyAt(index.Value)));
            }
        }

        // Auto-generated 
        protected override LayoutKey[] FillKeys()
        {
            return new LayoutKey[] {
                    new StylusAmbiguousKey(true,
                            new StylusKey('A', 2),
                            new StylusKey('B', 2),
                            new StylusKey('C', 2),
                            new StylusKey('D', 2)
                            ),
                    new StylusAmbiguousKey(true,
                            new StylusKey('E', 2),
                            new StylusKey('F', 2),
                            new StylusKey('G', 2),
                            new StylusKey('H', 2)
                            ),
                    new StylusAmbiguousKey(true,
                            new StylusKey('I', 2),
                            new StylusKey('J', 2),
                            new StylusKey('K', 2),
                            new StylusKey('L', 2)
                            ),
                    new StylusAmbiguousKey(true,
                            new StylusKey('M', 2),
                            new StylusKey('N', 2),
                            new StylusKey('O', 2),
                            new StylusKey('P', 2)
                            ),
                    new StylusAmbiguousKey(true,
                            new StylusKey('Q', 2),
                            new StylusKey('R', 2),
                            new StylusKey('S', 2),
                            new StylusKey('T', 2)
                            ),
                    new StylusAmbiguousKey(true,
                            new StylusKey('U', 2),
                            new StylusKey('V', 2),
                            new StylusKey('W', 2),
                            new StylusKey('X', 2)
                            ),
                    new StylusAmbiguousKey(true,
                            new StylusKey('Y', 2),
                            new StylusKey('Z', 2)
                            )
            };
        }
    }
}