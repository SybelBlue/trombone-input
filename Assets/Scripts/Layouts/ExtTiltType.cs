using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

namespace Extracted
{
#pragma warning disable 0649
    public class ExtTiltType : MonoBehaviour
    {
        public string layout 
        {
            get => _layout;
            set
            {
                if (inGame) throw new System.InvalidOperationException("Cannot change layout while running");
                _layout = value;
            }
        }

        [SerializeField, Tooltip("Unbroken strings of letters form bins. Use spaces to separate. Use backslash for control chars or to insert space literal.")]
        private string _layout;

        private string lastLayout = null;

        [SerializeField]
        private GameObject binnedPrefab, keyPrefab;

        private List<BinnedKey> keys;

        private bool inGame = false;

        public void Start()
        {
            inGame = true;
            keys = new List<BinnedKey>();
            List<List<char>> bins = layout.DecodeAsLayout();
            foreach (var bin in bins)
            {
                BinnedKey binned = Instantiate(binnedPrefab, transform).AddComponent<BinnedKey>();
                binned.innerKeys = new SimpleKey[bin.Count];
                
                for (int i = 0; i < bin.Count; i++)
                {
                    binned.innerKeys[i] = Instantiate(keyPrefab, binned.gameObject.transform).AddComponent<SimpleKey>();
                    binned.innerKeys[i].sym = bin[i];
                }

                keys.Add(binned);
            }
        }

        private void OnValidate()
        {
            if (!binnedPrefab)
            {
                Debug.LogError("binedPrefab is null!");
            }
            else if (!binnedPrefab.GetComponent<HorizontalLayoutGroup>() && !binnedPrefab.GetComponent<VerticalLayoutGroup>())
            {
                Debug.LogError("binnedPrefab has no horizontal or vertical layout group!");
            }

            if (!keyPrefab)
            {
                Debug.LogError("keyPrefab is null!");
            }
            else
            {
                if (!keyPrefab.GetComponent<TMP_Text>())
                {
                    Debug.LogError("keyPrefab has no TextMeshPro Text component!");
                }
            }

            if (lastLayout != layout)
            {
                if (layout != null)
                {
                    Debug.Log($"layout: {layout.DecodeAsLayout().PrettyPrint()}");
                }
                else
                {
                    Debug.LogError("layout is null!");
                }
                lastLayout = layout;
            }
        }
    }

    public class BinnedKey : MonoBehaviour
    {
        public SimpleKey[] innerKeys;
    }

    public class SimpleKey : MonoBehaviour
    {
        private TMP_Text text;
        
        [SerializeField]
        private char _sym;

        public char sym
        {
            get => _sym;
            set
            {
                if (_sym != value)
                {
                    _sym = value;
                    label = GetLabel(value);
                }
            }
        }

        public string label
        {
            get
            {
                text = text ? text : GetComponent<TMP_Text>();
                return text.text;
            }
            set
            {
                text = text ? text : GetComponent<TMP_Text>();
                text.text = value;
            }
        }

        private static string GetLabel(char c)
        {
            switch (c)
            {
                case '\b':
                    return "bksp";
                case ' ':
                    return "spc";
                case '\t':
                    return "tab";
                case '\n':
                    return "entr";
            }
            return $"{c}";
        }
    }

    internal static class Extensions
    { 
        public static List<List<char>> DecodeAsLayout(this string s)
        {
            var ret = new List<List<char>>();

            bool control = false;
            List<char> curr = null;
            foreach (char c in s)
            {
                curr = curr ?? new List<char>();
                if (c == '\\')
                {
                    control = true;
                    continue;
                }

                if (control)
                {
                    control = false;
                    switch(c)
                    {
                        case 't':
                            curr.Add('\t');
                            continue;
                        case 'b':
                            curr.Add('\b');
                            continue;
                        case 'n':
                            curr.Add('\n');
                            continue;
                        case ' ':
                            curr.Add(' ');
                            continue;
                    }
                    throw new System.ArgumentException($"Unkown control character \'{c}\'");
                }

                if (c == ' ')
                {
                    ret.Add(curr);
                    curr = null;
                    continue;
                }

                curr.Add(c);
            }

            ret.Add(curr ?? new List<char>());

            return ret;
        }

        public static string PrettyPrint<T>(this List<T> items)
            => $"[{string.Join(", ", items)}]";

        public static string PrettyPrint<T>(this List<List<T>> items)
            => $"[{string.Join(", ", items.Select(i => i.PrettyPrint()).ToArray())}]";
    }
}
