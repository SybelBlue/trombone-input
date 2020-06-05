namespace CustomInput
{
    public class TwoRotBinnedABCDE : StylusBinnedABCDE
    {
        protected override int? InnerIndex(InputData data, int parentSize)
            => data.normalizedX.HasValue ?
                (int?)Utils.NormalizedIntoIndex(1 - data.normalizedX.Value, parentSize) :
                null;
    }
}