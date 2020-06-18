namespace CustomInput
{
    public class TwoRotationABCDE : StylusBinnedABCDE
    {
        public override bool usesSlider => false;

        protected override int? InnerIndex(InputData data, int parentSize)
            => Utils.NormalizedIntoIndex(1 - data.normalizedAngles.x, parentSize);
    }
}