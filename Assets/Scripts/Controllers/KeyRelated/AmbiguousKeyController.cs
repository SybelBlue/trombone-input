using UnityEngine;
using UnityEngine.UI;

public class AmbiguousKeyController : KeyController<CustomInput.AmbiguousKey>
{
    public Color highlightColor;

    protected Color normalColor;
    protected bool highlighting = false;
    public Image background;

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
