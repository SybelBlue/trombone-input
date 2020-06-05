using System.Collections.Generic;
using AutoCorrect;
using UnityEngine;
using UnityEngine.UI;

public class TextOutputController : MonoBehaviour
{
    public Text rawOutput;

    public Text[] suggested;


    public TextAsset dict824765, dict243342;

    public DictionarySize dictionarySize;

    public SymSpell.Verbosity verbosity;

    public string text
    {
        get => rawOutput.text;
        set
        {
            rawOutput.text = value;

            if (Autocorrect.Instance.dictionaryLoaded)
            {
                suggestions = Autocorrect.Instance.Suggestions(value, verbosity);

                for (int i = 0; i < suggested.Length; i++)
                {
                    suggested[i].text = (i >= suggestions.Count) ? "" : suggestions[i];
                }
            }
        }
    }

    public List<string> suggestions;

    public void Start()
    {
        Autocorrect.Instance.InitDictionary(dictionarySize, dict824765, dict243342);

        foreach (var suggestedText in suggested)
        {
            suggestedText.GetComponent<Button>().onClick.AddListener(() => text = suggestedText.text.ToUpper());
        }
    }

}