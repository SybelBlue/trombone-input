﻿public class StylusKeyController : AbstractSimpleKeyController
{
    [UnityEngine.SerializeField]
    private TMPro.TMP_Text textMeshPro;

    public override string text
    {
        get => textMeshPro.text;
        set => textMeshPro.text = value;
    }
}
