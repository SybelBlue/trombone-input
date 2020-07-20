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

        [SerializeField, Tooltip("Unbroken strings of letters form bins. Use whitespace to separate. Use backslash for control chars or to insert space literal.")]
        private string layout;

        [SerializeField, Tooltip("Pairs each char with equally-indexed char of layout, ignoring basic whitespace. Use control chars for special chars as with layout. All missing alt chars default to null.")]
        private string altLayout;

        [SerializeField]
        private GameObject binnedPrefab, keyPrefab;

        [SerializeField]
        protected Vector3 minAngle, maxAngle;

        private List<BinnedKey> keys;

        private bool _useAlternate;
        public bool useAlternate
        {
            get => _useAlternate;
            set
            {
                _useAlternate = value;
                foreach (var key in keys)
                {
                    key.useAlternate = value;
                }
            }
        }

        public void Start()
        {
            keys = new List<BinnedKey>();
            List<List<char>> bins = layout.DecodeAsLayout();
            char[] altChars = altLayout.DecodeAsLayout().SelectMany(x => x).ToArray();
            int n = 0;
            foreach (var bin in bins)
            {
                BinnedKey binned = Instantiate(binnedPrefab, transform).AddComponent<BinnedKey>();
                binned.innerKeys = new SimpleKey[bin.Count];

                for (int i = 0; i < bin.Count; i++)
                {
                    binned.innerKeys[i] = Instantiate(keyPrefab, binned.gameObject.transform).AddComponent<SimpleKey>();
                    binned.innerKeys[i].sym = bin[i];
                    
                    if (n < altLayout.Length)
                    {
                        binned.innerKeys[i].alt = altChars[n];
                        n++;
                    }
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
            else if (!keyPrefab.GetComponentInChildren<TMP_Text>())
            {
                Debug.LogError("keyPrefab has no child with TextMeshPro Text component!");
            }
        }

        public char? GetSelectedLetter(Vector3 data)
        {
            var inner = FetchInnerKey(data);
            if (inner == null) return null;
            return inner.GetChar();
        }

        public void UpdateHighlight(Vector3 data)
        {
            var outer = ChildIndexFor(data);
            var binnedKey = keys[outer];

            for (int i = 0; i < keys.Count; i++)
            {
                keys[i].SetHighlight(i == outer);
            }

            int? inner = null;
            if (binnedKey != null)
            {
                binnedKey.SetHighlight(true);
                inner = InnerIndex(data, binnedKey.size);
            }

            for (int i = 0; i < binnedKey.size; i++)
            {
                binnedKey.innerKeys[i].SetHighlight(i == inner);
            }
        }

        private int ChildIndexFor(Vector3 normalizedAngles)
            => normalizedAngles.z.NormalizedIntoIndex(keys.Count);

        private int InnerIndex(Vector3 normalizedAngles, int parentSize)
            => (1 - normalizedAngles.x).NormalizedIntoIndex(parentSize);

        private SimpleKey FetchInnerKey(Vector3 data)
        {
            BinnedKey parent = keys[ChildIndexFor(data)];

            var inner = InnerIndex(data, parent.size);

            return parent.size > 0 ? parent.innerKeys[inner] : null;
        }
    }

    public class Highlighted : MonoBehaviour
    {
        protected Color highlightColor;

        protected Image image;
        protected bool isHighlighted = false;

        private Color lastColor;

        public virtual void SetHighlight(bool v)
        {
            if (isHighlighted == v) return;

            if (image == null)
            {
                image = GetComponent<Image>();
            }
            
            isHighlighted = v;

            if (v)
            {
                lastColor = image.color;
                image.color = highlightColor;
            }
            else
            {
                image.color = lastColor;
            }
        }
    }

    public class BinnedKey : Highlighted
    {
        public SimpleKey[] innerKeys;
        internal int size => innerKeys?.Length ?? 0;
        public bool useAlternate 
        {  
            set
            {
                foreach (var key in innerKeys)
                {
                    key.useAlt = value;
                }
            }
        }

        public void Start()
            => highlightColor = new Color(159, 159, 130);

        public override void SetHighlight(bool v)
        {
            base.SetHighlight(v);

            if (v) return;

            foreach (var key in innerKeys)
            {
                key.SetHighlight(false);
            }
        }
    }

    public class SimpleKey : Highlighted
    {
        private TMP_Text text;

        private char _sym;
        private char? _alt;
        private bool _useAlt;

        public bool useAlt
        {
            get => _useAlt;
            set
            {
                label = GetLabel((_useAlt = value) && alt.HasValue? alt.Value : sym);
            }
        }

        public char sym
        {
            get => _sym;
            set
            {
                if (_sym != value)
                {
                    _sym = value;
                    useAlt = useAlt;
                }
            }
        }

        public char? alt
        {
            get => _alt;
            set
            {
                if (_alt != value)
                {
                    _alt = value;
                    useAlt = useAlt;
                }
            }
        }

        public string label
        {
            get
            {
                text = text ? text : GetComponentInChildren<TMP_Text>();
                return text.text;
            }
            set
            {
                text = text ? text : GetComponentInChildren<TMP_Text>();
                text.text = value;
            }
        }

        public void Start()
            => highlightColor = new Color(8, 161, 53, 105);

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

        public char GetChar()
            => useAlt && alt.HasValue ? alt.Value : sym;
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

        public static int NormalizedIntoIndex(this float normalized, int length)
            => Mathf.FloorToInt(Mathf.LerpUnclamped(0, Mathf.Max(0, length - 1), normalized));
    }
}
