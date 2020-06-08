using System.Collections.Generic;
using CustomInput;
using MinVR;
using UnityEngine;

public class MainController : MonoBehaviour, VREventGenerator
{
    public const string _potentiometer_event_name = "BlueStylusAnalog";
    public const string _front_button_event_name = "BlueStylusFrontBtn";

    // The LayoutManager that is in charge of loading the layout
    public LayoutController layoutManager;

    public StylusModelController modelController;

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

    // True if no input is provided
    public static bool inputThisFrame
        => Input.touchCount > 0 || Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetKey(KeyCode.LeftControl);

    public void Start()
    {
        MinVR.VRMain.Instance.AddEventGenerator(this);
        MinVR.VRMain.Instance.AddOnVRAnalogUpdateCallback(_potentiometer_event_name, AnalogUpdate);
        MinVR.VRMain.Instance.AddOnVRButtonDownCallback(_front_button_event_name, OnBlueStylusFrontButtonDown);
        outputController.text = "";
    }

    // The most up-to-date value reported by the InputFieldController
    private int? lastReportedValue;

    public void Update()
    {
        indicatorRect.gameObject.SetActive(inputThisFrame);

        if (outputController.text.Length > 0)
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                outputController.text = outputController.text.Substring(0, outputController.text.Length - 1);
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                outputController.text += ' ';
            }
        }

        layout.UpdateState(currentInputData);
    }

    public void OnBlueStylusFrontButtonDown()
    {
        Debug.LogWarning($"Button Down from Hardware, using last reported value: ({lastReportedValue})");
        OnInputEnd(lastReportedValue ?? 0);
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

        modelController.normalizedSlider = normalized;
    }

    // Callback for when the InputFieldController register a completed gesture
    public void OnInputEnd(int value)
    {
        lastReportedValue = value;
        (LayoutKey parentKey, SimpleKey simpleKey) = layout.KeysFor(currentInputData) ?? (null, null);

        if (parentKey == null)
        {
            Debug.LogWarning("Ended gesture in empty zone: " + value);
            return;
        }

        (char typed, bool certain) = currentLetter ?? ('-', false);

        Debug.Log($"Pressed {parentKey} @ {simpleKey} => {(typed, certain)}");

        keypresses.Add(parentKey?.label ?? " ");

        disambiguated = AutoCorrect.Disambiguator.Disambiguated(keypresses);

        outputController.text += typed;

        modelController.normalizedSlider = null;

        lastReportedValue = null;
    }

    public (char letter, bool certain)? currentLetter
        => layout.GetLetterFor(currentInputData);

    private InputData currentInputData
        => new InputData
            (outputController.text
            , lastReportedValue
            , modelController.normalizedX
            , modelController.normalizedZ
            , modelController.normalizedSlider
            );

    // TODO: put this somewhere else?
    public List<string> keypresses = new List<string>();

    // The disambiguated options for keypresses
    public List<string> disambiguated = new List<string>();

    private void AnalogUpdate(float value)
        => OnInputValueChange(Mathf.FloorToInt(value));

    public void AddEventsSinceLastFrame(ref List<VREvent> eventList)
        => CaptureEmulatedPotentiometerInput(ref eventList);

    // If Right click is held and the mouse wheel is scrolled to emulate potentiometer,
    // will be less sensitive if either Shift key is held.
    // If tab is hit or Right click is released then it emulates the forward button down.
    private void CaptureEmulatedPotentiometerInput(ref List<VREvent> eventList)
    {
        if (Input.GetMouseButtonDown(1))
        {
            float value = (float)(lastReportedValue ?? inputPanel.maxValue / 2.0f);
            eventList.Add(MakeEvent(_potentiometer_event_name, "AnalogUpdate", value));
            return;
        }

        float delta = Input.mouseScrollDelta.y * 2;
        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
        {
            delta *= 4;
        }

        int rawNext = Mathf.RoundToInt(lastReportedValue + delta ?? 0);
        int next = Mathf.Clamp(rawNext, 0, inputPanel.maxValue);

        if (Input.GetMouseButton(1) && delta != 0)
        {
            eventList.Add(MakeEvent(_potentiometer_event_name, "AnalogUpdate", next));
        }

        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetMouseButtonUp(1))
        {
            eventList.Add(MakeEvent(_front_button_event_name, "ButtonDown", next));
        }
    }

    private static VREvent MakeEvent(string name, string type, float analogValue)
    {
        VREvent e = new VREvent(name);
        e.AddData("EventType", type);
        e.AddData("AnalogValue", analogValue);
        return e;
    }

}
