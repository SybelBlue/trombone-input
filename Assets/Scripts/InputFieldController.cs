using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class InputFieldEvent : UnityEvent<int> { }

/// <summary>
/// Class ripped from Slider.cs and hacked to pieces, then abstracted
/// </summary>
public abstract class InputFieldController : Selectable, IDragHandler, IInitializePotentialDragHandler, ICanvasElement
{
    public abstract int value { get; set; }

    public abstract float normalizedValue { get; set; }


    [SerializeField]
    public InputFieldEvent OnValueChanged;

    protected bool MayDrag(PointerEventData eventData)
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

    protected abstract void UpdateDrag(PointerEventData eventData, Camera cam);

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
