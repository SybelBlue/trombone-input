using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RectTransform;

public class AmbiguousKeyController : KeyController
{

    public Color highlightColor;

    protected Color normalColor;
    protected bool highlighting = false;
    public RectTransform rectTransform;
    public Image background;

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

    public void AddChild(GameObject g)
    {
        g.transform.SetParent(transform);
    }

    public virtual void SetSlant(bool forward)
    {
        if (transform.childCount == 0) return;

        if (forward)
        {
            var item = transform.GetChild(0).GetComponent<SimpleKeyController>();
            item.alignment = TextAnchor.UpperCenter;

            if (transform.childCount == 1) return;

            item = transform.GetChild(1).GetComponent<SimpleKeyController>();
            item.alignment = TextAnchor.MiddleCenter;

            if (transform.childCount == 2) return;

            item = transform.GetChild(2).GetComponent<SimpleKeyController>();
            item.alignment = TextAnchor.LowerCenter;
        }
        else
        {
            var item = transform.GetChild(0).GetComponent<SimpleKeyController>();
            item.alignment = TextAnchor.LowerCenter;

            if (transform.childCount == 1) return;

            item = transform.GetChild(1).GetComponent<SimpleKeyController>();
            item.alignment = TextAnchor.MiddleCenter;

            if (transform.childCount == 2) return;

            item = transform.GetChild(2).GetComponent<SimpleKeyController>();
            item.alignment = TextAnchor.UpperCenter;
        }
    }
}
