using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Controller
{
#pragma warning disable 649
    public class TextOutputDisplay : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TMP_Text rawOutput;

        [SerializeField]
        private GameObject suggestionsContainer;

        [SerializeField]
        private Suggestion[] suggestions;

        [SerializeField]
        private GridLayoutGroup suggestionLayoutGroup;


        public TextAsset dict824765, dict243342;

        public Auto.DictionarySize dictionarySize;

        public SymSpell.SymSpell.Verbosity verbosity;

        protected string text
        {
            get => rawOutput.text;
            set
            {
                rawOutput.text = value;
                RefreshSuggestionsPanel(suggestionSource);
            }
        }

        public virtual bool emptyText => text.Length == 0;

        public virtual string suggestionSource
        {
            get => text;
            protected set => text = value;
        }

        public virtual void Start()
        {
            Auto.Correct.Instance.InitDictionary(dictionarySize, dict824765, dict243342);

            if (dictionarySize != Auto.DictionarySize.None)
            {
                Auto.Complete.Instance.InitDictionary(dict824765, ' ');
            }

            foreach (var suggestion in suggestions)
            {
                suggestion.GetComponent<Button>().onClick.AddListener(() => OnSuggestionButtonClick(suggestion.text.ToUpper()));
            }

            RefreshSuggestionsPanel("");
        }

        protected virtual void OnSuggestionButtonClick(string suggestionText)
        {
            if (suggestionText.Length > 0)
            {
                if (suggestionSource.EndsWith(" "))
                {
                    suggestionSource += suggestionText;
                }
                else
                {
                    string[] sourceWords = suggestionSource.Split(' ');
                    string[] suggestionWords = suggestionText.Split(' ');
                    for (int i = 0; i < sourceWords.Length && i < suggestionWords.Length; i++)
                    {
                        sourceWords.SetFromEnd(i, suggestionWords.FromEnd(i));
                    }

                    suggestionSource = sourceWords.Intercalate(" ");
                }
            }
        }

        private void RefreshSuggestionsPanel(string value)
        {
            List<string> sugStrings = new List<string>();

            if (Auto.Correct.Instance.dictionaryLoaded)
            {
                sugStrings = Auto.Correct.Instance.Suggestions(value, verbosity);
            }

            if (Auto.Complete.Instance.dictionaryLoaded)
            {
                string lastWord = value.Split(' ').Last();
                sugStrings.AddRange(Auto.Complete.Instance.Completions(lastWord));
            }

            for (int i = 0; i < suggestions.Length; i++)
            {
                suggestions[i].text = (i >= sugStrings.Count) ? "" : sugStrings[i];
                suggestions[i].boxCollider.size = suggestionLayoutGroup.cellSize.WithZ(0.2f);
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
}