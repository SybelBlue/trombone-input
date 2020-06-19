using System.Collections.Generic;
using CustomInput;
using MinVR;
using UnityEngine;
using UnityEngine.UI;
using static CustomInput.VREventFactory;

public class MainController : MonoBehaviour, VREventGenerator
{

    public bool leftHanded;

    // The LayoutManager that is in charge of loading the layout
    public LayoutController layoutManager;

    public StylusModelController stylusModel;

    // The manager's current layout, or null if no manager exists
    private Layout layout
        => layoutManager?.currentLayout();

    // The simulated potentiometer input source
    public InputFieldController inputPanel;

    // The transform of the layout display
    public RectTransform displayRect;

    // The transform of the indicator
    public RectTransform indicatorRect;

    // The place where typed guesses go
    public TextOutputController outputController;

    public TextAsset trial0;

    public void Start()
    {
        Bindings.LEFT_HANDED = leftHanded;

        VRMain.Instance.AddEventGenerator(this);

        VRMain.Instance.AddOnVRAnalogUpdateCallback(_potentiometer_event_name, AnalogUpdate);

        VRMain.Instance.AddOnVRButtonDownCallback(_front_button_event_name, FrontButtonDown);
        VRMain.Instance.AddOnVRButtonUpCallback(_front_button_event_name, FrontButtonUp);

        VRMain.Instance.AddOnVRButtonDownCallback(_back_button_event_name, BackButtonDown);
        VRMain.Instance.AddOnVRButtonUpCallback(_back_button_event_name, BackButtonUp);

        outputController.text = "";

        Debug.Log($"Loaded {Testing.Utils.ReadTrialItems(trial0, false).Count} trial items");
    }

    // The most up-to-date value reported by the InputFieldController
    private int? lastReportedValue;

    private bool usingIndicator
    {
        get => indicatorRect.gameObject.activeInHierarchy;
        set => indicatorRect.gameObject.SetActive(value);
    }

    public void Update()
    {
        bool indicator = layout.usesSlider && Bindings.inputThisFrame;
        if (indicator != usingIndicator)
        {
            usingIndicator = indicator;
        }


        if (stylusModel.useLaser != layout.usesRaycasting)
        {
            stylusModel.useLaser = layout.usesRaycasting;
        }

        // TODO: Map to stylus events
        if (outputController.text.Length > 0 && Bindings.spaceDown)
        {
            outputController.text += ' ';
        }

        if (Bindings.backspaceDown)
        {
            PerformBackspace();
        }

        if (Bindings.emulatingLayoutSwitch.HasValue)
        {
            layoutManager.DropdownValueSelected(Bindings.emulatingLayoutSwitch.Value);
        }

        if (stylusModel.transform.hasChanged)
        {
            RaycastHit? hit;
            var raycastable = stylusModel.Raycast(out hit);
            if (raycastable)
            {
                raycastable.hasRaycastFocus = true;
            }
            else if (IRaycastable.last)
            {
                IRaycastable.last.hasRaycastFocus = false;
            }
        }

        layout.UpdateState(currentInputData);
    }

    // Callback for when the InputFieldController value changes due to user input
    public void OnInputValueChange(int value)
    {
        lastReportedValue = value;
        float width = displayRect.rect.width;
        var pos = indicatorRect.localPosition;

        float normalized = value / (float)inputPanel.maxValue;
        pos.x = width * (normalized - 0.5f);

        indicatorRect.localPosition = pos;

        stylusModel.normalizedSlider = normalized;
    }

    private bool OnInputEnd(int? value)
    {
        lastReportedValue = value;
        (LayoutKey parentKey, SimpleKey simpleKey) = layout.KeysFor(currentInputData) ?? (null, null);

        bool success = parentKey != null;

        if (success)
        {
            (char typed, bool certain) = layout.GetSelectedLetter(currentInputData) ?? ('-', false);

            if (typed == '\b' && certain)
            {
                Debug.Log("Pressed Backspace");

                PerformBackspace();
            }
            else
            {
                Debug.Log($"Pressed {parentKey} @ {simpleKey} => {(typed, certain)}");

                outputController.text += typed;
            }
        }
        else
        {
            Debug.LogWarning(value.HasValue ? $"Ended gesture in empty zone: {value}" : "Ended gesture on invalid key");
        }

        stylusModel.normalizedSlider = null;

        lastReportedValue = null;

        return success;
    }

    // Callback for when the InputFieldController register a completed gesture
    public void OnSimulatedFingerUp(int value)
        => OnInputEnd(value);

    private InputData currentInputData
        => stylusModel.PackageData(outputController.text, lastReportedValue);

    private void AnalogUpdate(float value)
        => OnInputValueChange(Mathf.RoundToInt(value));

    public void FrontButtonDown()
    {
        stylusModel.frontButtonDown = true;
        if (OnInputEnd(lastReportedValue)) return;

        RaycastHit? hit;
        var raycastable = stylusModel.Raycast(out hit);
        if (raycastable)
        {
            raycastable.GetComponent<Button>()?.onClick.Invoke();
            var dropdown = raycastable.GetComponent<Dropdown>();
            if (dropdown)
            {
                dropdown.value = (dropdown.value + 1) % dropdown.options.Count;
            }
        }
    }

    public void FrontButtonUp()
        => stylusModel.frontButtonDown = false;

    public void BackButtonDown()
    {
        stylusModel.backButtonDown = true;
        layout.useAlternate = !layout.useAlternate;
    }

    public void BackButtonUp()
        => stylusModel.backButtonDown = false;

    private void PerformBackspace()
    {
        int endIndex = Mathf.Max(0, outputController.text.Length - 1);
        outputController.text = outputController.text.Substring(0, endIndex);
    }

    public void AddEventsSinceLastFrame(ref List<VREvent> eventList)
    {
        CaptureEmulatedSliderInput(ref eventList);
        CaptureEmulatedButtonInput(ref eventList);
    }

    // If Right click is held and the mouse wheel is scrolled to emulate potentiometer,
    // will be less sensitive if either Shift key is held.
    private void CaptureEmulatedSliderInput(ref List<VREvent> eventList)
    {
        if (Bindings.beginEmulatedSlide)
        {
            int value = lastReportedValue ?? inputPanel.maxValue / 2;
            eventList.Add(MakePotentiometerEvent(value));
            return;
        }

        float delta = Bindings.emulatedSlideDelta;
        if (!Bindings.precisionMode)
        {
            delta *= 4;
        }

        int rawNext = Mathf.RoundToInt(lastReportedValue + delta ?? 0);
        int next = Mathf.Clamp(rawNext, 0, inputPanel.maxValue);

        if (delta == 0)
        {
            next = Bindings.emulatedSlideValue;
        }

        if (Bindings.emulatingSlide)
        {
            eventList.Add(MakePotentiometerEvent(next));
        }
    }


    // If BackQuote is hit or Right click is released when the layout accepts potentiometer input,
    // then it emulates the forward button down.
    // If Tab is hit then it emulates back button down
    private void CaptureEmulatedButtonInput(ref List<VREvent> eventList)
    {
        if (Bindings.emulatingFrontDown || (Bindings.endEmulatedSlide && layout.usesSlider))
        {
            eventList.Add(MakeButtonDownEvent(_front_button_event_name));
        }

        if (Bindings.emulatingFrontUp)
        {
            eventList.Add(MakeButtonUpEvent(_front_button_event_name));
        }

        if (Bindings.emulatingBackDown)
        {
            eventList.Add(MakeButtonDownEvent(_back_button_event_name));
        }

        if (Bindings.emulatingBackUp)
        {
            eventList.Add(MakeButtonUpEvent(_back_button_event_name));
        }
    }
}
