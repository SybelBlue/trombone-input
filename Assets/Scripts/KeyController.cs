using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RectTransform;

public abstract class KeyController : MonoBehaviour
{
    public CustomInput.LayoutKey item;

    public abstract float Resize(float unitWidth);

    public abstract void SetHighlight(bool highlight);
}

public abstract class AbstractSimpleKeyController : KeyController
{
    public Color highlightColor;

    private Color normalColor;
    private bool highlighting = false;

    public Image background;
    public RectTransform rectTransform;

    public abstract string text { get; set; }

    [SerializeField]
    private char _symbol;

    public char symbol
    {
        get => _symbol;
        set
        {
            _symbol = value;
            text = new string(new char[] { value });
        }
    }

    public override float Resize(float unitWidth)
    {
        var width = unitWidth * item.size;
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
}
