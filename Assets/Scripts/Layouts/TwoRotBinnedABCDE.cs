namespace CustomInput
{
    public class TwoRotBinnedABCDE : StylusBinnedABCDE
    {
        public override bool usesSlider => false;

        protected override int? InnerIndex(InputData data, int parentSize)
            => data.normalizedX.HasValue ?
                (int?)Utils.NormalizedIntoIndex(1 - data.normalizedX.Value, parentSize) :
                null;
    }
}