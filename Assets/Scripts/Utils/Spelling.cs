using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Utils.UnityExtensions;

namespace Auto
{
    using static SymSpell.SymSpell;
    using Loader = System.Func<Stream, int, int, char[], bool>;

    public class Complete
    {
        public static readonly int _completion_count = 16;
        private static Complete _instance;

        private Complete()
        {
            _instance = this;
        }

        public static Complete Instance
            => _instance == null ? new Complete() : _instance;

        private PruningRadixTrie trie;

        public bool dictionaryLoaded => trie != null;

        public void InitDictionary(TextAsset frequencyDict, char separator)
        {
            trie = new PruningRadixTrie();
            using (Stream corpusStream = frequencyDict.IntoMemoryStream())
            {
                trie.ReadTermsFromStream(corpusStream, separator);
            }
            Debug.LogWarning($"Trie Loaded! ({trie.termCountLoaded} nodes)");
        }

        public List<string> Completions(string prefix)
        {
            long _termFreqCount;
            return 
                trie?
                    .GetTopkTermsForPrefix(prefix.ToLower(), _completion_count, out _termFreqCount)
                    .Select(t => t.term)
                    .ToList()
                ?? new List<string>();
        }

    }

    [Serializable]
    public enum DictionarySize
    {
        None,
        Dict82765,
        Dict243342,
    }

    public class Correct
    {
        private static Correct _instance;

        private Correct()
        {
            _instance = this;
        }

        public static Correct Instance
            => _instance == null ? new Correct() : _instance;

        private const int INIT_CAPACITY = 82765;
        private const int MAX_EDIT_DISTANCE_DICT = 2;

        private readonly SymSpell.SymSpell symSpell = new SymSpell.SymSpell(INIT_CAPACITY, MAX_EDIT_DISTANCE_DICT);

        private TextAsset DictionaryTextAsset(DictionarySize size, params TextAsset[] dicts)
        {
            switch (size)
            {
                case DictionarySize.Dict82765:
                    return dicts[0];

                case DictionarySize.Dict243342:
                    return dicts[1];
            }

            throw new ArgumentException("Unknown size");
        }

        private static Vector2Int DictionaryTermAndCountIndices(DictionarySize size)
        {
            switch (size)
            {
                case DictionarySize.Dict82765:
                    return new Vector2Int(0, 1);

                case DictionarySize.Dict243342:
                    return new Vector2Int(0, 2);
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

        public bool dictionaryLoaded { get; protected set; }

        public void InitDictionary(DictionarySize size, params TextAsset[] dicts)
        {
            dictionaryLoaded = false;

            if (size == DictionarySize.None) return;

            // must load basic first or else error...
            if (size != DictionarySize.Dict82765)
            {
                InitDictionary(DictionarySize.Dict82765);
            }

            Loader loader = DictionaryLoader(size);

            var pair = DictionaryTermAndCountIndices(size);
            var termIndex = pair[0];
            var countIndex = pair[1];

            using (Stream corpusStream = DictionaryTextAsset(size, dicts).IntoMemoryStream())
            {
                if (!loader(corpusStream, termIndex, countIndex, defaultSeparatorChars))
                {
                    throw new Exception("Could not load dictionary!");
                }
                else
                {
                    Debug.LogWarning($"Dictionary Loaded! ({symSpell.EntryCount} entries)");
                    dictionaryLoaded = true;
                }
            }
        }

        public static int? DictionaryValue(string word)
        {
            long value = -1;
            if (Instance.dictionaryLoaded)
            {
                Instance.symSpell.words.TryGetValue(word.ToLower(), out value);
            }

            // if dict loaded and key present
            if (value > 0) return int.MaxValue > value ? (int)value : int.MaxValue;

            return null;
        }

        public List<SuggestItem> Lookup(string inputTerm, Verbosity verbosity)
        {
            int maxEditDistanceLookup = Mathf.Min(inputTerm.Length, MAX_EDIT_DISTANCE_DICT);
            // Assert.IsTrue(maxEditDistanceLookup <= MAX_EDIT_DISTANCE_DICT);
            if (inputTerm.Length == 0) return new List<SuggestItem>();

            // use LookupCompound?
            return symSpell.LookupCompound(inputTerm, maxEditDistanceLookup).Union(symSpell.Lookup(inputTerm, verbosity, maxEditDistanceLookup).Take(5)).ToList();
            // return symSpell.Lookup(inputTerm, verbosity, maxEditDistanceLookup);
        }

        public List<string> Suggestions(string inputTerm, Verbosity verbosity)
            => Lookup(inputTerm, verbosity).Select(suggestion => suggestion.term).Where(s => !s.ToUpper().Equals(inputTerm)).ToList();
    }
}