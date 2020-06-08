using UnityEngine;

namespace CustomInput
{
    public class LinearABCDE : Layout
    {

        public override string layoutName => "Linear ABCDE";

        public override bool usesSlider => true;

        protected override void Start()
        {
            base.Start();

            foreach (SimpleKeyController cont in GetComponentsInChildren<SimpleKeyController>())
            {
                var newColor = cont.background.color;
                newColor.a = 1.0f;
                cont.background.color = newColor;
            }
        }

        public override (LayoutKey, SimpleKey)? KeysFor(InputData data)
        {
            var lKey = LayoutKeyFor(data);
            return (lKey, (SimpleKey)lKey);
        }

        public override void SetHighlightedKey(InputData data)
        {
            UnhighlightAll();

            if (MainController.inputThisFrame)
            {
                ChildFor(data)?.GetComponent<KeyController>()?.SetHighlight(true);
            }
        }

        public override int ChildIndexFor(InputData data)
            => data.rawValue ?? -1;

        public override (char, bool)? GetLetterFor(InputData data)
        {
            var s = CharsFor(data);
            if (s == null || s.Length != 1) return null;
            return (s.ToCharArray()[0], true);
        }

        // Auto-generated 
        protected override LayoutKey[] FillKeys()
        {
            var basicItem0 = new SimpleKey('A', 3);


            var basicItem1 = new SimpleKey('B', 2);


            var basicItem2 = new SimpleKey('C', 2);


            var basicItem3 = new SimpleKey('D', 2);


            var basicItem4 = new SimpleKey('E', 3);


            var basicItem5 = new SimpleKey('F', 2);


            var basicItem6 = new SimpleKey('G', 2);


            var basicItem7 = new SimpleKey('H', 2);


            var basicItem8 = new SimpleKey('I', 3);


            var basicItem9 = new SimpleKey('J', 2);


            var basicItem10 = new SimpleKey('K', 2);


            var basicItem11 = new SimpleKey('L', 2);


            var basicItem12 = new SimpleKey('M', 2);


            var basicItem13 = new SimpleKey('N', 3);


            var basicItem14 = new SimpleKey('O', 3);


            var basicItem15 = new SimpleKey('P', 2);


            var basicItem16 = new SimpleKey('Q', 2);


            var basicItem17 = new SimpleKey('R', 3);


            var basicItem18 = new SimpleKey('S', 2);


            var basicItem19 = new SimpleKey('T', 3);


            var basicItem20 = new SimpleKey('U', 2);


            var basicItem21 = new SimpleKey('V', 2);


            var basicItem22 = new SimpleKey('W', 2);


            var basicItem23 = new SimpleKey('X', 2);


            var basicItem24 = new SimpleKey('Y', 2);


            var basicItem25 = new SimpleKey('Z', 2);
            return new LayoutKey[] {
                        basicItem0,
                        basicItem1,
                        basicItem2,
                        basicItem3,
                        basicItem4,
                        basicItem5,
                        basicItem6,
                        basicItem7,
                        basicItem8,
                        basicItem9,
                        basicItem10,
                        basicItem11,
                        basicItem12,
                        basicItem13,
                        basicItem14,
                        basicItem15,
                        basicItem16,
                        basicItem17,
                        basicItem18,
                        basicItem19,
                        basicItem20,
                        basicItem21,
                        basicItem22,
                        basicItem23,
                        basicItem24,
                        basicItem25
                };
        }
    }
}