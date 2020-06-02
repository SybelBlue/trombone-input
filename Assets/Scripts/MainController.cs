using System.Collections.Generic;
using CustomInput;
using UnityEngine;

public class MainController : MonoBehaviour
{
    // The LayoutManager that is in charge of loading the layout
    public LayoutManager layoutManager;

    // The manager's current layout, or null if no manager exists
    private Layout layout { get => layoutManager?.currentLayout(); }

    // The main input source
    public InputFieldController inputPanel;

    // The transform of the layout display
    public RectTransform displayRect;

    // The transform of the indicator
    public RectTransform indicatorRect;

    // The place where typed guesses go
    public TextOutputController outputController;

    // True if no input is provided
    public static bool NoInput()
        => Input.touchCount == 0 && !Input.GetMouseButton(0);

    public void Start()
    {
        MinVR.VRMain.Instance.AddOnVRAnalogUpdateCallback("BlueStylusAnalog", AnalogUpdate);
        outputController.text = "";
    }

    // The most up-to-date value reported by the InputFieldController
    private int? lastReportedValue;

    public void Update()
    {
        indicatorRect.gameObject.SetActive(!NoInput());
        layout?.SetHighlightedKey(NoInput() ? null : lastReportedValue);

        if (Input.GetMouseButtonDown(1) && outputController.text.Length > 0)
        {
            outputController.text = outputController.text.Substring(0, outputController.text.Length - 1);
        }
    }

    // Callback for when the InputFieldController value changes due to user input
    public void OnInputValueChange(int value)
    {
        lastReportedValue = value;
        float width = displayRect.rect.width;
        var pos = indicatorRect.position;
        pos.x = value * width / (float)inputPanel.maxValue;
        indicatorRect.position = pos;
    }

    // Callback for when the InputFieldController register a completed gesture
    public void OnInputEnd(int value)
    {
        lastReportedValue = value;
        var (currentItem, exactItem) = layout.KeysAt(value) ?? (null, null);

        if (!currentItem)
        {
            Debug.LogWarning("Ended gesture in empty zone: " + value);
            return;
        }

        var (typed, certain) = layout.GetLetterFor(outputController.text, value) ?? ('-', false);

        Debug.Log($"Pressed [{displayData(currentItem)}] @ {displayData(exactItem)} => {(typed, certain)}");

        keypresses.Add(currentItem?.data ?? " ");

        disambiguated = SpellingAssist.Disambiguator.Disambiguated(keypresses);

        outputController.text += typed;

        lastReportedValue = null;
    }

    // TODO: put this somewhere else?
    public List<string> keypresses = new List<string>();

    // The disambiguated options for keypresses
    public List<string> disambiguated = new List<string>();

    private void AnalogUpdate(float value)
    {
        Debug.Log("From Hardware: " + value);
        OnInputValueChange(Mathf.FloorToInt(value));
    }

    // Helper function for displaying layout items in the log
    private string displayData(LayoutKey item) => item?.data ?? "<not found>";
}
