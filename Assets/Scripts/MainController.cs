using UnityEngine;

public class MainController : MonoBehaviour
{
    public DisplayController displayController;
    public InputFieldController inputPanel;
    public RectTransform displayRect, indicatorRect;

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
    public void OnSliderValueChange(int value)
    {
        float width = displayRect.rect.width;
        var pos = indicatorRect.position;
        pos.x = value * width / (float)inputPanel.maxValue;
        indicatorRect.position = pos;
    }
}
