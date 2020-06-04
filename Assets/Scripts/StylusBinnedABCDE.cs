
using UnityEngine;

namespace CustomInput
{
    public class StylusBinnedABCDE : SquashedQWERTY
    {
        public override string layoutName => "Stylus ABCDE";

        protected override LayoutKey[] FillKeys()
        {
            return new LayoutKey[] {
                new StylusAmbiguousKey(true,
                    new StylusKey('A', 2),
                    new StylusKey('B', 2),
                    new StylusKey('C', 2),
                    new StylusKey('D', 2)
                )
            };
        }
    }
}