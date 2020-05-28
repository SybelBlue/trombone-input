using UnityEngine;

public class MainController : MonoBehaviour
{
    /// <summary>
    /// The DisplayController that is in charge of loading the layout
    /// </summary>
    public DisplayController displayController;

    /// <summary>
    /// The main input source
    /// </summary>
    public CustomInput.InputFieldController inputPanel;

    /// <summary>
    /// The transform of the layout display
    /// </summary>
    public RectTransform displayRect;

    /// <summary>
    /// The transform of the indicator
    /// </summary>
    public RectTransform indicatorRect;

    /// <summary>
    /// True if no touch input is provided
    /// </summary>
    /// <returns>no touch input</returns>
    public static bool NoTouches()
    {
        return Input.touchCount == 0;
    }

    public void Update()
    {
        indicatorRect.gameObject.SetActive(!NoTouches());
        foreach (var cont in displayController.gameObject.GetComponentsInChildren<AbstractDisplayItemController>())
        {
            cont.SetHighlight(false);
        }

        if (NoTouches()) return;

        var currentHover = displayController.ChildAt((int)inputPanel.value);
        var currentController = currentHover?.GetComponent<BlockDisplayItemController>();
        currentController?.SetHighlight(true);
    }

    /// <summary>
    /// Callback for when the InputFieldController value changes due to user input
    /// </summary>
    /// <param name="value">the new value</param>
    public void OnInputValueChange(int value)
    {
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
        Debug.Log("end");
        var currentHover = displayController.ChildAt((int)inputPanel.value);

        if (!currentHover)
        {
            Debug.Log("Ended gesture in empty zone");
            return;
        }

        LayoutItem currentItem = currentHover.GetComponent<AbstractDisplayItemController>()?.item;

        BasicLayoutItem exactItem = displayController.ExactItemAt((int)inputPanel.value);

        Debug.Log($"[{displayData(currentItem)}]: {displayData(exactItem)}");
    }

    /// <summary>
    /// Helper function for displaying layout items in the log
    /// </summary>
    /// <param name="item">LayoutItem to get data from</param>
    /// <returns>string representation of data</returns>
    private string displayData(LayoutItem item) => item?.data ?? "<not found>";
}
