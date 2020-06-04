public class StylusKeyController : AbstractSimpleKeyController
{
    [UnityEngine.SerializeField]
    private TMPro.TextMeshPro textMeshPro;

    public override string text
    {
        get => textMeshPro.text;
        set => textMeshPro.text = value;
    }

    public override float Resize(float unitWidth)
    {
        var width = base.Resize(unitWidth);
        rectTransform.SetSizeWithCurrentAnchors(UnityEngine.RectTransform.Axis.Vertical, width);
        return width;
    }
}
