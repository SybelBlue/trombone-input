using UnityEngine;

public class SimpleKeyController : AbstractSimpleKeyController
{
    [SerializeField]
    private UnityEngine.UI.Text childText;

    public TextAnchor alignment
    {
        get => childText.alignment;
        set => childText.alignment = value;
    }

    public override string text
    {
        get => childText.text;
        set => childText.text = text;
    }
}
