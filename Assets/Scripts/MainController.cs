using CustomInput;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    /// <summary>
    /// The LayoutManager that is in charge of loading the layout
    /// </summary>
    public LayoutManager layoutManager;

    public Layout layout { get => layoutManager?.currentLayout(); }

    /// <summary>
    /// The main input source
    /// </summary>
    public InputFieldController inputPanel;

    /// <summary>
    /// The transform of the layout display
    /// </summary>
    public RectTransform displayRect;

    /// <summary>
    /// The transform of the indicator
    /// </summary>
    public RectTransform indicatorRect;

    /// <summary>
    /// The place where typed guesses go
    /// </summary>
    public TextOutputController outputController;

    /// <summary>
    /// True if no input is provided
    /// </summary>
    /// <returns>no input</returns>
    public static bool NoInput()
    {
        return Input.touchCount == 0 && !Input.GetMouseButton(0);
    }

    public void Start()
    {
        outputController.text = "";
    }

    /// <summary>
    /// The most up-to-date value reported by the InputFieldController
    /// </summary>
    private int? lastReportedValue;

    public void Update()
    {
        indicatorRect.gameObject.SetActive(!NoInput());
        layout?.SetHighlightedKey(NoInput() ? null : lastReportedValue);
    }

    /// <summary>
    /// Callback for when the InputFieldController value changes due to user input
    /// </summary>
    /// <param name="value">the new value</param>
    public void OnInputValueChange(int value)
    {
        lastReportedValue = value;
        float width = displayRect.rect.width;
        var pos = indicatorRect.position;
        pos.x = value * width / (float)inputPanel.maxValue;
        indicatorRect.position = pos;
    }

    /// <summary>
    /// Callback for when the InputFieldController register a completed gesture
    /// </summary>
    /// <param name="value">the new value</param>
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

        outputController.text += typed;

        lastReportedValue = null;
    }

    /// <summary>
    /// Helper function for displaying layout items in the log
    /// </summary>
    /// <param name="item">LayoutItem to get data from</param>
    /// <returns>string representation of data</returns>
    private string displayData(LayoutKey item) => item?.data ?? "<not found>";
}
