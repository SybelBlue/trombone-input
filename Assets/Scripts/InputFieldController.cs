using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class InputFieldEvent : UnityEvent<int> { }

/// <summary>
/// Class ripped from Slider.cs and hacked to pieces
/// </summary>
public class InputFieldController : Selectable, IDragHandler, IInitializePotentialDragHandler, ICanvasElement
{

    public RectTransform rectTransform;

    public int minValue;

    public int maxValue;

    [SerializeField]
    private float rawValue;

    private const int axis = 0;
    private const bool reverseValue = false;

    public virtual int value
    {
        get
        {
            return (int)Mathf.Round(rawValue);
        }
        set
        {
            rawValue = value;
            OnValueChanged.Invoke(this.value);
        }
    }

    public float normalizedValue
    {
        get
        {
            return Mathf.InverseLerp(minValue, maxValue, value);
        }
        set
        {
            rawValue = Mathf.Lerp(minValue, maxValue, value);
            OnValueChanged.Invoke(this.value);
        }
    }


    [SerializeField]
    public InputFieldEvent OnValueChanged;

    int ClampValue(float input)
    {
        return (int)Mathf.Round(Mathf.Clamp(input, minValue, maxValue));
    }

    private bool MayDrag(PointerEventData eventData)
    {
        return IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!MayDrag(eventData))
            return;

        base.OnPointerDown(eventData);

        UpdateDrag(eventData, eventData.pressEventCamera);
    }


    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!MayDrag(eventData))
            return;
        UpdateDrag(eventData, eventData.pressEventCamera);
    }

    void UpdateDrag(PointerEventData eventData, Camera cam)
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

    public virtual void OnInitializePotentialDrag(PointerEventData eventData)
    {
        eventData.useDragThreshold = false;
    }

    public void Rebuild(CanvasUpdate executing)
    {

#if UNITY_EDITOR
        if (executing == CanvasUpdate.Prelayout)
            OnValueChanged.Invoke(value);
#endif
    }

    public void LayoutComplete()
    { }

    public void GraphicUpdateComplete()
    { }
}
