using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Utils.SystemExtensions;

namespace Extracted
{
#pragma warning disable 0649
    public class ExtTiltType : MonoBehaviour
    {
        [SerializeField, Tooltip("When true, automatically instantiates keys on Start().")]
        private bool loadOnStart;

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

        private void Start()
        {
            if (loadOnStart) Load();
        }

        public void Load()
        {
            keys = new List<BinnedKey>();
            List<List<char>> bins = layout.DecodeIntoBinned();
            char[] altChars = altLayout.DecodeIntoBinned().SelectMany(x => x).ToArray();
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

        /// <summary>
        /// Accepts the vector along the direction stylus.transform.forward and attempts to convert this orientation into a character
        /// </summary>
        /// <param name="stylusForward">the vector pointing down the tip of the stylus model</param>
        /// <param name="rightHandedMode">if true, mirrors the bounds in the x-axis when calculating angles</param>
        /// <returns>char for the rotation defined by stylusForward</returns>
        public char? GetSelectedLetter(Vector3 stylusForward, bool rightHandedMode = false)
            => GetSelectedLetterUnfiltered(stylusForward.NormalizeAngles(minAngle, maxAngle, rightHandedMode));

        /// <summary>
        /// Returns the character corresponding to the angles provided
        /// </summary>
        /// <param name="normalizedAngles">the normalized angles of the stylus</param>
        /// <returns>char corresponding to normalizedAngles</returns>
        public char? GetSelectedLetterUnfiltered(Vector3 normalizedAngles)
            => keys == null ? null : FetchInnerKey(normalizedAngles)?.GetChar();

        /// <summary>
        /// Highlights the key corresponding to the orientation of the stylus where stylusForward is stylus.transform.forward
        /// </summary>
        /// <param name="stylusForward">the vector pointing down the tip of the stylus model</param>
        /// <param name="rightHandedMode">if true, mirrors the bounds in the x-axis when calculating angles</param>
        public void UpdateHighlight(Vector3 stylusForward, bool rightHandedMode = false)
            => UpdateHighlightUnfiltered(stylusForward.NormalizeAngles(minAngle, maxAngle, rightHandedMode));

        /// <summary>
        /// Highlights the key corresponding to the angles provided
        /// </summary>
        /// <param name="normalizedAngles">the normalized angles of the stylus</param>
        public void UpdateHighlightUnfiltered(Vector3 normalizedAngles)
        {
            if (keys == null) return;
            var outer = BinnedIndex(normalizedAngles);
            var binnedKey = keys[outer];

            for (int i = 0; i < keys.Count; i++)
            {
                keys[i].SetHighlight(i == outer);
            }

            int? inner = null;
            if (binnedKey != null)
            {
                binnedKey.SetHighlight(true);
                inner = InnerIndex(normalizedAngles);
            }

            for (int i = 0; i < binnedKey.size; i++)
            {
                binnedKey.innerKeys[i].SetHighlight(i == inner);
            }
        }

        private int BinnedIndex(Vector3 normalizedAngles)
            => normalizedAngles.z.NormalizedIntoIndex(keys.Count);

        private int InnerIndex(Vector3 normalizedAngles)
            => (1 - normalizedAngles.x).NormalizedIntoIndex(keys[BinnedIndex(normalizedAngles)].size);

        private SimpleKey FetchInnerKey(Vector3 normalizedAngles)
            => keys[BinnedIndex(normalizedAngles)].innerKeys[InnerIndex(normalizedAngles)];
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

    internal static class PrimitiveExtensions
    { 
        public static List<List<char>> DecodeIntoBinned(this string s)
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
                    if (!curr.IsEmpty())
                    {
                        ret.Add(curr);
                        curr = null;
                    }
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

    public static class VectorExtensions
    {
        public static float SignedAngle(this Vector3 forward, Vector3 target, Vector3 axis)
        {
            float dot = Vector3.Dot(forward.normalized, target.normalized);
            float degrees = Mathf.Rad2Deg * Mathf.Acos(dot);

            Vector3 derivedAxis = Vector3.Cross(forward, target);
            float sign = Mathf.Sign(Vector3.Dot(axis, derivedAxis));

            return sign * degrees;
        }

        public static Vector3 VectorFrom(System.Func<int, float> f)
            => new Vector3(f(0), f(1), f(2));

        public static Vector3 ProjectTo(this Vector3 vec, int axisMask)
            // if axis mask has a 1 in the ith place, keep ith value, else 0
            => vec.Map((i, v) => ((axisMask >> i) & 0x1) * v);

        public static Vector3 Flatten(this Vector3 vec, int axis)
            => vec.ProjectTo(~(1 << axis));

        public static Vector3 Map(this Vector3 vec, System.Func<int, float, float> f)
            => new Vector3(f(0, vec[0]), f(1, vec[1]), f(2, vec[2]));

        public static float SignedAngleFromAxis(this Vector3 forward, Vector3 target, int axis)
            => forward.Flatten(axis).SignedAngle(target, VectorFrom(i => i == axis ? 1 : 0));

        public static Vector3 NormalizeAngles(this Vector3 stylusForward, Vector3 minAngle, Vector3 maxAngle, bool rightHanded = false)
            => VectorFrom(axis => {
                // if measuring angle for y (axis == 1), measure rotation around forward (0, 0, 1), else measure rotation around up (0, 1, 0)
                Vector3 measureOrigin = axis == 1 ? Vector3.forward : Vector3.up;
                float angle = stylusForward.SignedAngleFromAxis(measureOrigin, axis);

                float low = axis == 0 && rightHanded ? maxAngle[axis] : minAngle[axis];
                float hi = axis == 0 && rightHanded ? minAngle[axis] : maxAngle[axis];

                if (angle < 0)
                {
                    angle = (180 + angle) + 180;
                }
                if (low < 0)
                {
                    low = (180 + low) + 180;
                }
                if (hi < 0)
                {
                    hi = (180 + hi) + 180;
                }
                return Mathf.Clamp01((angle - low) / (hi - low));
            });
    }
}
