using CustomInput;

public class RaycastQWERTY : Layout
{
    public override bool usesSlider => false;

    private bool _useAlternate;

    public override bool useAlternate
    {
        get => _useAlternate;
        set
        {
            _useAlternate = value;
            foreach (var controller in gameObject.GetComponentsInChildren<StylusKeyController>())
            {
                controller.useAlternate = value;
            }
        }
    }

    public override int ChildIndexFor(InputData data)
    {
        throw new System.NotImplementedException();
    }

    public override (char letter, bool certain)? GetSelectedLetter(InputData data)
    {
        throw new System.NotImplementedException();
    }

    public override (LayoutKey parent, SimpleKey simple)? KeysFor(InputData data)
    {
        throw new System.NotImplementedException();
    }

    public override void SetHighlightedKey(InputData data)
    {
        throw new System.NotImplementedException();
    }

    protected override LayoutKey[] FillKeys()
    {
        throw new System.NotImplementedException();
    }
}