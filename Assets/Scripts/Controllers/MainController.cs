using System.Collections.Generic;
using CustomInput;
using UnityEngine;

public class MainController : MonoBehaviour
{
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
        MinVR.VRMain.Instance.AddOnVRAnalogUpdateCallback("BlueStylusAnalog", AnalogUpdate);
        outputController.text = "";
    }

    // The most up-to-date value reported by the InputFieldController
    private int? lastReportedValue;

    public void Update()
    {
        indicatorRect.gameObject.SetActive(inputThisFrame);

        if (Input.GetKeyDown(KeyCode.Backspace) && outputController.text.Length > 0)
        {
            outputController.text = outputController.text.Substring(0, outputController.text.Length - 1);
        }

        CaptureMouseWheelInput();

        layout.UpdateState(currentInputData);
    }

    private void CaptureMouseWheelInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            OnInputValueChange(lastReportedValue ?? inputPanel.maxValue / 2);
            return;
        }

        float delta = Input.mouseScrollDelta.y * 8;
        int rawNext = Mathf.RoundToInt(lastReportedValue + delta ?? 0);
        int next = Mathf.Clamp(rawNext, 0, inputPanel.maxValue);

        if (Input.GetMouseButtonUp(1))
        {
            OnInputEnd(next);
        }
        else if (Input.GetMouseButton(1) && delta != 0)
        {
            OnInputValueChange(next);
        }
    }

    // Callback for when the InputFieldController value changes due to user input
    public void OnInputValueChange(int value)
    {
        lastReportedValue = value;
        float width = displayRect.rect.width;
        var pos = indicatorRect.position;

        var normalized = value / (float)inputPanel.maxValue;
        pos.x = width * normalized;

        indicatorRect.position = pos;

        modelController.normalizedSlider = normalized;
    }

    // Callback for when the InputFieldController register a completed gesture
    public void OnInputEnd(int value)
    {
        lastReportedValue = value;
        var (currentItem, exactItem) = layout.KeysFor(currentInputData) ?? (null, null);

        if (currentItem == null)
        {
            Debug.LogWarning("Ended gesture in empty zone: " + value);
            return;
        }

        var (typed, certain) = currentLetter ?? ('-', false);

        Debug.Log($"Pressed {currentItem} @ {exactItem} => {(typed, certain)}");

        keypresses.Add(currentItem?.label ?? " ");

        disambiguated = SpellingAssist.Disambiguator.Disambiguated(keypresses);

        outputController.text += typed;

        modelController.normalizedSlider = null;

        lastReportedValue = null;
    }

    public (char, bool)? currentLetter
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
    {
        Debug.Log("From Hardware: " + value);
        OnInputValueChange(Mathf.FloorToInt(value));
    }
}
