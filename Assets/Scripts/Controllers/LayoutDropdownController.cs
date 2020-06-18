using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 649
public class LayoutDropdownController : IRaycastable
{
    public Dropdown dropdown;

    public Image image;

    [SerializeField]
    private Color highlightColor;

    private Color normalColor;

    private bool _highlighting;

    public override bool hasRaycastFocus
    {
        get => _highlighting;
        set
        {
            if (value)
            {
                normalColor = image.color;
                image.color = highlightColor;
            }
            else if (hasRaycastFocus)
            {
                image.color = normalColor;
            }

            _highlighting = value;
            Debug.Log(value);
        }
    }

}