using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CustomInput
{
    /// <summary>
    /// A class for events issued by InputFieldControllers
    /// </summary>
    [Serializable]
    public class InputFieldEvent : UnityEvent<int> { }

    /// <summary>
    /// Class ripped from Slider.cs and hacked to pieces, then abstracted
    /// </summary>
    public abstract class InputFieldController : Selectable, IDragHandler, IInitializePotentialDragHandler, ICanvasElement
    {
        /// <summary>
        /// The output value of the input controller in [minValue, maxValue]
        /// </summary>
        /// <value>integer output</value>
        public abstract int value { get; set; }

        /// <summary>
        /// The normalized output value of the input controller (in [0.0f, 1.0f])
        /// </summary>
        /// <value>ratio output</value>
        public abstract float normalizedValue { get; set; }


        /// <summary>
        /// The minimum output for the potentiometer
        /// </summary>
        public int minValue;

        /// <summary>
        /// The maximum output for the potentiometer
        /// </summary>
        public int maxValue;

        /// <summary>
        /// Clamps the value into range [minValue, maxValue] then rounds to nearest int
        /// </summary>
        /// <returns></returns>
        protected int ClampValue(float input) => (int)Mathf.Round(Mathf.Clamp(input, minValue, maxValue));


        /// <summary>
        /// Called whenever the value is changed
        /// </summary>
        [SerializeField]
        public InputFieldEvent OnValueChanged;

        /// <summary>
        /// Called whenever the input is finished
        /// </summary>
        [SerializeField]
        public InputFieldEvent OnInputEnd;

        /// <summary>
        /// Returns wether or not this item can be dragged by the pointer event
        /// </summary>
        /// <param name="eventData">pointer event data</param>
        /// <returns>can drag</returns>
        protected bool MayDrag(PointerEventData eventData)
        {
            return IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
        }

        /// <inheritdoc/>
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;

            base.OnPointerDown(eventData);

            UpdateDrag(eventData, eventData.pressEventCamera);
        }


        /// <inheritdoc/>
        public void OnDrag(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;

            UpdateDrag(eventData, eventData.pressEventCamera);
        }

        /// <inheritdoc/>
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            if (MayDrag(eventData))
            {
                UpdateDrag(eventData, eventData.pressEventCamera);
            }

            OnInputEnd.Invoke(value);
        }

        /// <summary>
        /// Defines the behavior of the input controller when a valid drag is registered
        /// </summary>
        /// <param name="eventData">drag data</param>
        /// <param name="cam">camera the data was registered on</param>
        protected abstract void UpdateDrag(PointerEventData eventData, Camera cam);

        /// <inheritdoc/>
        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }

        /// <inheritdoc/>
        public void Rebuild(CanvasUpdate executing)
        {

#if UNITY_EDITOR
            if (executing == CanvasUpdate.Prelayout)
                OnValueChanged.Invoke(value);
#endif
        }

        /// <inheritdoc/>
        public void LayoutComplete()
        { }

        /// <inheritdoc/>
        public void GraphicUpdateComplete()
        { }
    }
}