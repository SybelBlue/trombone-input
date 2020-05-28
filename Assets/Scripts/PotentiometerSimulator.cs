using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.RectTransform;

namespace CustomInput
{
    /// <summary>
    /// Adapted from Unity's Slider.cs
    /// Simulates a Potentiometer strip with adjustable noise and value range
    /// </summary>
    public class PotentiometerSimulator : InputFieldController
    {

        /// <summary>
        /// The RectTransform belonging to this.gameObject
        /// Used in determining the touch boundaries
        /// </summary>
        private RectTransform rectTransform;

        /// <summary>
        /// The raw floating-point value of the last input position
        /// </summary>
        [SerializeField]
        private float rawValue;

        /// <summary>
        /// Potentiometer is only used horizontally
        /// </summary>
        private const Axis axis = Axis.Horizontal;

        /// <inheritdoc/>
        public override int value
        {
            get => ClampValue(rawValue + noise);

            set
            {
                rawValue = value;
                OnValueChanged.Invoke(this.value);
            }
        }

        /// <inheritdoc/>
        public override float normalizedValue
        {
            get => Mathf.InverseLerp(minValue, maxValue, value);

            set
            {
                rawValue = Mathf.Lerp(minValue, maxValue, value);
                OnValueChanged.Invoke(this.value);
            }
        }

        /// <summary>
        /// Container for randomly generated noise values
        /// </summary>
        private readonly float[] noiseValues = new float[300];

        /// <summary>
        /// The current level of noise (updates on read), or 0 if !simulateNoise
        /// </summary>
        /// <value> current noise </value>
        private float noise
        {
            get => simulateNoise ? noiseValues[incrNoiseIndex()] : 0;
        }

        /// <summary>
        /// When true, the potentiometer will have random noise in its value
        /// up to +/- maxAbsoluteNoise
        /// </summary>
        public bool simulateNoise = true;

        /// <summary>
        /// Maximum absolute value noise deviation from the input
        /// </summary>
        public float maxAbsoluteNoise;

        /// <summary>
        /// Tracker for least recently used noise value
        /// </summary>
        private int noiseIndex;

        protected override void Start()
        {
            base.Start();

            rectTransform = GetComponent<RectTransform>();

            FillNoise();
        }

        /// <summary>
        /// Fills noiseValues with new entries and resets noiseIndex
        /// </summary>
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

        /// <summary>
        /// Returns the current noise value index, then increments cyclicly, never 
        /// exceeding noiseValues.length
        /// </summary>
        /// <returns>least recently used noise value index</returns>
        private int incrNoiseIndex() => noiseIndex++ % noiseValues.Length;

        /// <inheritdoc/>
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;

            base.OnPointerDown(eventData);

            UpdateDrag(eventData, eventData.pressEventCamera);
        }


        /// <inheritdoc>
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

                normalizedValue = Mathf.Clamp01(localCursor[(int)axis] / clickRect.rect.size[(int)axis]);
            }
        }
    }
}