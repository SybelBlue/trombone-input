using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RectTransform;

public class SimpleKeyController : AbstractKeyController
{
    public Color highlightColor;

    private Color normalColor;
    private bool highlighting = false;

    public Image background;
    public RectTransform rectTransform;
    public Text childText;

    [SerializeField]
    private char symbol;

    public override float Resize(float unitWidth)
    {
        var width = unitWidth * item.size();
        rectTransform.SetSizeWithCurrentAnchors(Axis.Horizontal, width);
        return width;
    }

    public override void SetHighlight(bool h)
    {
        if (h)
        {
            normalColor = background.color;

            background.color = highlightColor;
        }
        else if (highlighting)
        {
            background.color = normalColor;
        }

        highlighting = h;
    }
    public void setSymbol(char c)
    {
        symbol = c;
        childText.text = c + "";
    }
}
