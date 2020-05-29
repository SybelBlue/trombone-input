using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

using Loader = System.Func<System.IO.Stream, int, int, char[], bool>;
// using AsyncLoader = System.Func<System.IO.Stream, int, int, System.Threading.Tasks.Task<bool>>;

[Serializable]
public enum DictionarySize
{
    Dict82765,
    Dict243342,
}

public class TextOutputController : MonoBehaviour
{
    public Text outputDisplay;

    public TextAsset dict824765, dict243342;

    public DictionarySize dictionarySize;

    public SymSpell.Verbosity verbosity;

    public string text
    {
        get => outputDisplay.text;
        set
        {
            outputDisplay.text = value;

            if (dictionaryLoaded)
            {
                suggestions = Suggestions(value);
            }
        }
    }

    public List<string> suggestions;

    public void Start()
    {
        InitDictionary();
    }

    // ///////////////////////// SYMSPELL //////////////////////////////

    private const int INIT_CAPACITY = 82765;
    private const int MAX_EDIT_DISTANCE_DICT = 2;

    private readonly SymSpell symSpell = new SymSpell(INIT_CAPACITY, MAX_EDIT_DISTANCE_DICT);

    private TextAsset DictionaryTextAsset(DictionarySize size)
    {
        switch (size)
        {
            case DictionarySize.Dict82765:
                return dict824765;

            case DictionarySize.Dict243342:
                return dict243342;
        }

        throw new ArgumentException("Unknown size");
    }

    private static (int, int) DictionaryTermAndCountIndices(DictionarySize size)
    {
        switch (size)
        {
            case DictionarySize.Dict82765:
                return (0, 1);

            case DictionarySize.Dict243342:
                return (0, 2);
        }

        throw new ArgumentException("Unknown size");
    }

    private Loader DictionaryLoader(DictionarySize size)
    {
        switch (size)
        {
            case DictionarySize.Dict82765:
                return symSpell.LoadDictionary;

            case DictionarySize.Dict243342:
                return symSpell.LoadBigramDictionary;
        }

        throw new ArgumentException("Unknown size");
    }


    public bool dictionaryLoaded = false;

    private void InitDictionary()
    {
        dictionaryLoaded = false;

        Debug.Log("Starting...");

        Loader loader = DictionaryLoader(dictionarySize);

        var (termIndex, countIndex) = DictionaryTermAndCountIndices(dictionarySize);

        using (Stream corpusStream = new MemoryStream(DictionaryTextAsset(dictionarySize).bytes))
        {
            if (!loader(corpusStream, termIndex, countIndex, SymSpell.defaultSeparatorChars))
            {
                throw new Exception("Could not load dictionary!");
            }
            else
            {
                Debug.LogWarning("Dictionary Loaded!");
                dictionaryLoaded = true;
            }
        }
    }

    private List<SymSpell.SuggestItem> Lookup(string inputTerm, int maxEditDistanceLookup, SymSpell.Verbosity verbosity)
    {
        Assert.IsTrue(maxEditDistanceLookup <= MAX_EDIT_DISTANCE_DICT);
        if (inputTerm.Length == 0) return new List<SymSpell.SuggestItem>();

        // use LookupCompound?
        return symSpell.Lookup(inputTerm, verbosity, maxEditDistanceLookup);
    }

    private List<string> Suggestions(string inputTerm)
    {
        var suggestItems = Lookup(inputTerm, Mathf.Min(inputTerm.Length, MAX_EDIT_DISTANCE_DICT), verbosity);

        foreach (var item in suggestItems)
        {
            Debug.Log(item.ToString());
        }

        return suggestItems.Select(suggestion => suggestion.term).ToList();
    }

    // private static AsyncLoader AsynchronizeLoader(Loader loader) =>
    //      async (s, x, y) => await new System.Threading.Tasks.Task<bool>(() => loader(s, x, y, SymSpell.defaultSeparatorChars));

    // private AsyncLoader DictionaryAsyncLoader(DictionarySize size) => AsynchronizeLoader(DictionaryLoader(size));

    // private async void InitDictionaryAsync()
    // {
    //     dictionaryLoaded = false;

    //     Debug.Log("Starting...");

    //     AsyncLoader loader = DictionaryAsyncLoader(dictionarySize);

    //     var (termIndex, countIndex) = DictionaryTermAndCountIndices(dictionarySize);

    //     using (Stream corpusStream = new MemoryStream(DictionaryTextAsset(dictionarySize).bytes))
    //     {
    //         if (!await loader.Invoke(corpusStream, termIndex, countIndex))
    //         {
    //             throw new Exception("Could not load dictionary!");
    //         }
    //         else
    //         {
    //             Debug.LogWarning("Dictionary Loaded!");
    //             dictionaryLoaded = true;
    //         }
    //     }
    // }
}