using System.Collections.Generic;
using AutoCorrect;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 649
public class TextOutputController : MonoBehaviour
{
    [SerializeField]
    private Text rawOutput;

    [SerializeField]
    private GameObject suggestionsContainer;

    [SerializeField]
    private Text[] suggested;


    public TextAsset dict824765, dict243342;

    public DictionarySize dictionarySize;

    public SymSpell.Verbosity verbosity;

    public string text
    {
        get => rawOutput.text;
        set
        {
            rawOutput.text = value;
            RefreshSuggestionsPanel(value);
        }
    }

    public List<string> suggestions;

    public void Start()
    {
        AutoCorrect.AutoCorrect.Instance.InitDictionary(dictionarySize, dict824765, dict243342);

        if (dictionarySize != DictionarySize.None)
        {
            AutoComplete.AutoComplete.Instance.InitDictionary(dict824765, ' ');
        }

        suggested = suggestionsContainer.GetComponentsInChildren<Text>();

        foreach (var suggestedText in suggested)
        {
            suggestedText.GetComponent<Button>().onClick.AddListener(() => text = suggestedText.text.ToUpper());
        }
    }

    private void RefreshSuggestionsPanel(string value)
    {
        suggestions = new List<string>();

        if (AutoCorrect.AutoCorrect.Instance.dictionaryLoaded)
        {
            suggestions = AutoCorrect.AutoCorrect.Instance.Suggestions(value, verbosity);
        }

        if (AutoComplete.AutoComplete.Instance.dictionaryLoaded)
        {
            string lastWord = value.Split(' ').Last();
            suggestions.AddRange(AutoComplete.AutoComplete.Instance.Completions(lastWord));
        }


        for (int i = 0; i < suggested.Length; i++)
        {
            suggested[i].text = (i >= suggestions.Count) ? "" : suggestions[i];
        }
    }

}