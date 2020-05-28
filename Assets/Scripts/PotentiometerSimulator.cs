using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Adapted from Unity's Slider.cs
/// Simulates a Potentiometer strip with adjustable noise and value range
/// </summary>
public class PotentiometerSimulator : InputFieldController
{

    public RectTransform rectTransform;

    public int minValue;

    public int maxValue;

    [SerializeField]
    private float rawValue;

    private const int axis = 0;
    private const bool reverseValue = false;

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

    private float[] noiseValues = new float[300];

    private float noise
    {
        get => simulateNoise ? noiseValues[incrNoiseIndex()] : 0;
    }

    public bool simulateNoise = true;

    public float maxAbsoluteNoise;

    private int noiseIndex = 0;

    protected override void Start()
    {
        base.Start();

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

    private int incrNoiseIndex() => noiseIndex++ % noiseValues.Length;

    int ClampValue(float input) => (int)Mathf.Round(Mathf.Clamp(input, minValue, maxValue));

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

            float val = Mathf.Clamp01(localCursor[(int)axis] / clickRect.rect.size[(int)axis]);
            normalizedValue = (reverseValue ? 1f - val : val);
        }
    }
}
