using UnityEngine;

namespace CustomInput
{
    namespace Layout
    {
        public class TwoRotationABCDE : StylusBinnedABCDE
        {
            public override bool usesSlider => false;

            [SerializeField]
            protected Vector3 minAngle, maxAngle;

            public override (Vector3 minima, Vector3 maxima)? StylusRotationBounds()
                 => (minAngle, maxAngle);

            protected override int? InnerIndex(InputData data, int parentSize)
                => Utils.Static.NormalizedIntoIndex(1 - data.normalizedAngles.x, parentSize);
        }
    }
}