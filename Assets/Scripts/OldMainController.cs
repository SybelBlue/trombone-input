using CustomInput;
using UnityEngine;

public class OldMainController : MonoBehaviour
{
    // The LayoutManager that is in charge of loading the layout
    public LayoutManager layoutManager;

    public Layout layout
        => layoutManager?.currentLayout();

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
    {
        return Input.touchCount == 0 && !Input.GetMouseButton(0);
    }

    public void Start()
    {
        outputController.text = "";
    }

    // The most up-to-date value reported by the InputFieldController
    private int? lastReportedValue;

    public void Update()
    {
        indicatorRect.gameObject.SetActive(!NoInput());
        layout?.SetHighlightedKey(NoInput() ? null : lastReportedValue);

        if (Input.GetMouseButtonDown(1))
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

        if (currentItem != null)
        {
            Debug.LogWarning("Ended gesture in empty zone: " + value);
            return;
        }

        var (typed, certain) = layout.GetLetterFor(outputController.text, value) ?? ('-', false);

        Debug.Log($"Pressed [{displayData(currentItem)}] @ {displayData(exactItem)} => {(typed, certain)}");

        outputController.text += typed;

        lastReportedValue = null;
    }

    // Helper function for displaying layout items in the log
    private string displayData(LayoutKey item) => item?.data ?? "<not found>";
}
