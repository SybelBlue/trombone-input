public class SimpleKeyController : AbstractSimpleKeyController
{
    [UnityEngine.SerializeField]
    private UnityEngine.UI.Text childText;

    public override string text
    {
        get => childText.text;
        set => childText.text = text;
    }
}
