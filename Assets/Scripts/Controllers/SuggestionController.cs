using UnityEngine;

#pragma warning disable 649
public class SuggestionController : IRaycastable
{
    [SerializeField]
    protected TMPro.TMP_Text text;

    [SerializeField]
    protected TextOutputController textOutputController;

    public override bool hasRaycastFocus
    {
        get => text.fontStyle == TMPro.FontStyles.Bold;
        set
        {
            text.fontStyle = value ?
                TMPro.FontStyles.Bold :
                TMPro.FontStyles.Italic;
            if (value)
            {
                textOutputController.SuggestionHighlighted(text.text);
            }
        }
    }
}