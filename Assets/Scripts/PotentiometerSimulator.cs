using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.RectTransform;

namespace CustomInput
{
    // Adapted from Unity's Slider.cs
    // Simulates a Potentiometer strip with adjustable noise and value range
    public class PotentiometerSimulator : InputFieldController
    {

        // The RectTransform belonging to this.gameObject used in determining the touch boundaries
        private RectTransform rectTransform;

        // The raw floating-point value of the last input position
        [SerializeField]
        private float rawValue;

        // Potentiometer orientation
        public Axis axis;

        public bool reversed;

        public override int value
        {
            get => ClampValue(rawValue + noise);

            set
            {
                rawValue = value;
                OnValueChanged.Invoke(this.value);
            }
        }

        public override float normalizedValue
        {
            get => Mathf.InverseLerp(minValue, maxValue, value);

            set
            {
                rawValue = Mathf.Lerp(minValue, maxValue, value);
                OnValueChanged.Invoke(this.value);
            }
        }

        // Container for randomly generated noise values
        private readonly float[] noiseValues = new float[300];

        // The current level of noise (updates on read), or 0 if !simulateNoise
        private float noise
            => simulateNoise ? noiseValues[incrNoiseIndex()] : 0;

        // When true, the potentiometer will have random noise in its value up to +/- maxAbsoluteNoise
        public bool simulateNoise = true;

        // Maximum absolute value noise deviation from the input
        public float maxAbsoluteNoise;

        // Tracker for least recently used noise value
        private int noiseIndex;

        protected override void Start()
        {
            base.Start();

            rectTransform = GetComponent<RectTransform>();

            FillNoise();
        }

        // Fills noiseValues with new entries and resets noiseIndex
        private void FillNoise()
        {
            for (int i = 0; i < noiseValues.Length; i += 2)
            {
                var vec = UnityEngine.Random.insideUnitCircle;

                noiseValues[i] = maxAbsoluteNoise * vec.x;
                if (noiseValues.Length % 2 == 0)
                {
                    noiseValues[i + 1] = maxAbsoluteNoise * vec.y;
                }
            }

            noiseIndex = 0;
        }

        // Returns the current noise value index, then increments cyclicly, never exceeding noiseValues.length
        private int incrNoiseIndex() => noiseIndex++ % noiseValues.Length;

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;

            base.OnPointerDown(eventData);

            UpdateDrag(eventData, eventData.pressEventCamera);
        }


        protected override void UpdateDrag(PointerEventData eventData, Camera cam)
        {
            RectTransform clickRect = rectTransform;
            if (clickRect != null && clickRect.rect.size[(int)axis] > 0)
            {
                Vector2 position = Input.mousePosition;
                Vector2 localCursor;
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(clickRect, position, cam, out localCursor))
                    return;
                localCursor -= clickRect.rect.position;

                var ratio = Mathf.Clamp01(localCursor[(int)axis] / clickRect.rect.size[(int)axis]); ;
                normalizedValue = reversed ? 1 - ratio : ratio;
            }
        }
    }
}