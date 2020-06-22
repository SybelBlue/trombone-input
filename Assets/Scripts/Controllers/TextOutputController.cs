using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 649
public class TextOutputController : MonoBehaviour
{
    [SerializeField]
    private TMPro.TMP_Text rawOutput;

    [SerializeField]
    private GameObject suggestionsContainer;

    [SerializeField]
    private TMPro.TMP_Text[] suggested;

    [SerializeField]
    private GridLayoutGroup suggestionLayoutGroup;


    public TextAsset dict824765, dict243342;

    public Auto.DictionarySize dictionarySize;

    public SymSpell.Verbosity verbosity;

    public virtual string text
    {
        get => rawOutput.text;
        protected set
        {
            rawOutput.text = value;
            RefreshSuggestionsPanel(value);
        }
    }

    public List<string> suggestions;

    public void Start()
    {
        Auto.Correct.Instance.InitDictionary(dictionarySize, dict824765, dict243342);

        if (dictionarySize != Auto.DictionarySize.None)
        {
            Auto.Complete.Instance.InitDictionary(dict824765, ' ');
        }

        suggested = suggestionsContainer.GetComponentsInChildren<TMPro.TMP_Text>();

        foreach (var suggestedText in suggested)
        {
            suggestedText.GetComponent<Button>().onClick.AddListener(() =>
            {
                var sugText = suggestedText.text.ToUpper();
                if (sugText.Length > 0)
                {
                    if (text.EndsWith(" "))
                    {
                        text += sugText;
                    }
                    else
                    {
                        string[] textWords = text.Split(' ');
                        string[] suggestionWords = sugText.Split(' ');
                        for (int i = 0; i < textWords.Length && i < suggestionWords.Length; i++)
                        {
                            textWords.SetFromEnd(i, suggestionWords.FromEnd(i));
                        }

                        text = textWords.Intercalate(" ");
                    }
                }
            });
        }

        RefreshSuggestionsPanel("");
    }

    private void RefreshSuggestionsPanel(string value)
    {
        suggestions = new List<string>();

        if (Auto.Correct.Instance.dictionaryLoaded)
        {
            suggestions = Auto.Correct.Instance.Suggestions(value, verbosity);
        }

        if (Auto.Complete.Instance.dictionaryLoaded)
        {
            string lastWord = value.Split(' ').Last();
            suggestions.AddRange(Auto.Complete.Instance.Completions(lastWord));
        }

        for (int i = 0; i < suggested.Length; i++)
        {
            suggested[i].text = (i >= suggestions.Count) ? "" : suggestions[i];
            suggested[i].GetComponent<BoxCollider>().size = suggestionLayoutGroup.cellSize.WithZ(0.2f);
        }
    }

    public virtual void ResetText()
        => text = "";

    public void TypedChar(char c)
    {
        if (c == '\b')
        {
            TypedBackspace();
        }
        else
        {
            AppendLetter(c);
        }
    }

    public virtual void AppendLetter(char c)
        => text += c;

    public virtual void TypedBackspace()
        => text = text.Backspace();
}