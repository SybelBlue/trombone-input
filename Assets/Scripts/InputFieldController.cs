using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CustomInput
{
    // A class for events issued by InputFieldControllers
    [Serializable]
    public class InputFieldEvent : UnityEvent<int> { }

    // Class ripped from Slider.cs and hacked to pieces, then abstracted
    public abstract class InputFieldController : Selectable, IDragHandler, IInitializePotentialDragHandler, ICanvasElement
    {
        // The output value of the input controller in [minValue, maxValue]
        public abstract int value { get; set; }

        // The normalized output value of the input controller (in [0.0f, 1.0f])
        public abstract float normalizedValue { get; set; }


        // The minimum output for the potentiometer
        public int minValue;

        // The maximum output for the potentiometer
        public int maxValue;

        // Clamps the value into range [minValue, maxValue] then rounds to nearest int
        protected int ClampValue(float input)
            => Mathf.FloorToInt(Mathf.Clamp(input, minValue, maxValue));


        // Called whenever the value is changed
        [Tooltip("Called when the value of this input field changes")]
        public InputFieldEvent OnValueChanged;

        // Called whenever the input is finished
        [Tooltip("Called when a gesture ends")]
        public InputFieldEvent OnInputEnd;

        // Returns wether or not this item can be dragged by the pointer event
        protected bool MayDrag(PointerEventData eventData)
            => IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left;

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (MayDrag(eventData))
            {
                base.OnPointerDown(eventData);

                UpdateDrag(eventData, eventData.pressEventCamera);
            }
        }


        public void OnDrag(PointerEventData eventData)
        {
            if (MayDrag(eventData))
            {
                UpdateDrag(eventData, eventData.pressEventCamera);
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            if (MayDrag(eventData))
            {
                UpdateDrag(eventData, eventData.pressEventCamera);
            }

            OnInputEnd.Invoke(value);
        }

        // Defines the behavior of the input controller when a valid drag is registered
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
}