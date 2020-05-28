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

        if (!currentHover) return;

        var currentController = currentHover.GetComponent<BlockDisplayItemController>();
        LayoutItem currentItem = currentController.item;

        currentController.SetHighlight(true);

        var touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Ended)
        {

            var exactItem = displayController.ExactItemAt((int)inputPanel.value);

            string groupText = "";
            if (currentItem is BlockLayoutItem)
            {
                groupText = new string((currentItem as BlockLayoutItem).data()) + ": ";
            }

            Debug.Log(groupText + exactItem.data);
        }
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
}
