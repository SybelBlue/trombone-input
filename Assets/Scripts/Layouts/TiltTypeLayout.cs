namespace CustomInput.Layout
{
    public class TiltTypeLayout : ArcTypeLayout
    {
        public override bool usesSlider => false;

        protected override int? InnerIndex(InputData data, int parentSize)
            => Utils.Static.NormalizedIntoIndex(1 - data.normalizedAngles.x, parentSize);
    }
}