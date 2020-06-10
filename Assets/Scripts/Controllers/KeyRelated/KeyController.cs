using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RectTransform;

// The base component for all GameObjects that are meant to represent
// a key in a Layout's GUI.
//
// See Layouts/LayoutKets.cs for usage
public abstract class KeyController : MonoBehaviour
{
    // this.gameObject's RectTransform (filled in Inspector)
    [SerializeField]
    protected RectTransform rectTransform;

    // the internal data this.gameObject is meant to represent
    public CustomInput.LayoutKey data;

    // defines the behavior of this.gameObject when highlight is toggled
    public abstract void SetHighlight(bool highlight);

    // defines the behavior of this.gameObject when it recieves a new
    // sensor width in Unity-units. Note that the potentiometer is
    // 64 sensors long, so sensorWidth generally equals the width of
    // the parent container divided by 64.
    public virtual float Resize(float sensorWidth)
    {
        var width = sensorWidth * data.size;
        rectTransform.SetSizeWithCurrentAnchors(Axis.Horizontal, width);
        return width;
    }
    public virtual float ResizeHeight(float sensorHeight)
    {
      var height = sensorHeight * data.size;
      rectTransform.SetSizeWithCurrentAnchors(Axis.Vertical, height);
      return height;
    }
}

public abstract class AbstractSimpleKeyController : KeyController
{
    public Color highlightColor;

    private Color normalColor;
    private bool highlighting = false;

    public Image background;

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
