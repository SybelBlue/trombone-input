using Controller.Key;
using CustomInput.KeyData;

namespace CustomInput.Layout
{
    public class SliderOnlyLayout : AbstractLayout
    {
        public override bool usesSlider => true;
        public override bool keyOnFingerUp => true;
        public override bool usesRaycasting => false;

        private bool _useAlternate;

        public override bool useAlternate
        {
            get => _useAlternate;
            set
            {
                _useAlternate = value;
                foreach (var controller in gameObject.GetComponentsInChildren<AbstractSimple>())
                {
                    controller.useAlternate = value;
                }
            }
        }

        public override (AbstractData, SimpleData)? KeysFor(InputData data)
        {
            var lKey = LayoutKeyFor(data);
            return (lKey, (SimpleData)lKey);
        }

        public override void SetHighlightedKey(InputData data)
        {
            UnhighlightAll();

            if (Bindings.inputThisFrame)
            {
                ChildFor(data)?.GetComponent<IKey>()?.SetHighlight(true);
            }
        }

        protected override int ChildIndexFor(InputData data)
            => data.normalizedSlider.HasValue ?
                Utils.Static.NormalizedIntoIndex(data.normalizedSlider.Value, 64) :
                -1;//(int?)data.rawValue ?? -1;

        public override char? GetSelectedLetter(InputData data)
        {
            var s = LayoutKeyFor(data)?.label ?? "";
            if (s == null || s.Length != 1) return null;
            return s.ToCharArray()[0];
        }

        // Auto-generated
        protected override AbstractData[] FillKeys()
        {
            return new AbstractData[] {
                new SimpleData('A', 3),
                new SimpleData('B', 2),
                new SimpleData('C', 2),
                new SimpleData('D', 2),
                new SimpleData('E', 3),
                new SimpleData('F', 2),
                new SimpleData('G', 2),
                new SimpleData('H', 2),
                new SimpleData('I', 3),
                new SimpleData('J', 2),
                new SimpleData('K', 2),
                new SimpleData('L', 2),
                new SimpleData('M', 2),
                new SimpleData('N', 3),
                new SimpleData('O', 3),
                new SimpleData('P', 2),
                new SimpleData('Q', 2),
                new SimpleData('R', 3),
                new SimpleData('S', 2),
                new SimpleData('T', 3),
                new SimpleData('U', 2),
                new SimpleData('V', 2),
                new SimpleData('W', 2),
                new SimpleData('X', 2),
                new SimpleData('Y', 2),
                new SimpleData('Z', 2),
                new SimpleData(' ', 3),
                new SimpleData('\b', 3)
            };
        }
    }
}
