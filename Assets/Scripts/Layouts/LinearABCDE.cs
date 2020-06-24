namespace CustomInput
{
    public class LinearABCDE : Layout
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
                foreach (var controller in gameObject.GetComponentsInChildren<AbstractSimpleKeyController>())
                {
                    controller.useAlternate = value;
                }
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

            if (Bindings.inputThisFrame)
            {
                ChildFor(data)?.GetComponent<IKeyController>()?.SetHighlight(true);
            }
        }

        protected override int ChildIndexFor(InputData data)
            => data.rawValue ?? -1;

        public override char? GetSelectedLetter(InputData data)
        {
            var s = LayoutKeyFor(data)?.label ?? "";
            if (s == null || s.Length != 1) return null;
            return s.ToCharArray()[0];
        }

        // Auto-generated 
        protected override LayoutKey[] FillKeys()
        {
            return new LayoutKey[] {
                    new SimpleKey('A', 3),
                    new SimpleKey('B', 2),
                    new SimpleKey('C', 2),
                    new SimpleKey('D', 2),
                    new SimpleKey('E', 3),
                    new SimpleKey('F', 2),
                    new SimpleKey('G', 2),
                    new SimpleKey('H', 2),
                    new SimpleKey('I', 3),
                    new SimpleKey('J', 2),
                    new SimpleKey('K', 2),
                    new SimpleKey('L', 2),
                    new SimpleKey('M', 2),
                    new SimpleKey('N', 3),
                    new SimpleKey('O', 3),
                    new SimpleKey('P', 2),
                    new SimpleKey('Q', 2),
                    new SimpleKey('R', 3),
                    new SimpleKey('S', 2),
                    new SimpleKey('T', 3),
                    new SimpleKey('U', 2),
                    new SimpleKey('V', 2),
                    new SimpleKey('W', 2),
                    new SimpleKey('X', 2),
                    new SimpleKey('Y', 2),
                    new SimpleKey('Z', 2)
                };
        }
    }
}