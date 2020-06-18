using UnityEngine;

#pragma warning disable 649
public class SuggestionController : IRaycastable
{
    [SerializeField]
    protected TMPro.TMP_Text text;

    [SerializeField]
    protected TextOutputController textOutputController;

    protected override void OnRaycastFocusChange(bool value)
        => text.fontStyle = value ?
            TMPro.FontStyles.Bold :
            TMPro.FontStyles.Italic;
}