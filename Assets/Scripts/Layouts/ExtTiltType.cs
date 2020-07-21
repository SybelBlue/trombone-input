using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Utils.SystemExtensions;

namespace Extracted
{
    using Extensions;
    using UnityEngine.Assertions;
    /// <summary>
    /// The TiltType layout, compressed into a single class.
    /// 
    /// Either loadOnStart must be true at Start(), or Load() must be called before the class is usable.
    /// </summary>
#pragma warning disable 0649
    public class ExtTiltType : MonoBehaviour
    {
        [SerializeField, Tooltip("When true, automatically instantiates keys on Start().")]
        private bool loadOnStart;

        [SerializeField, Tooltip("Unbroken strings of letters form bins. Use whitespace to separate. Use backslash for control chars or to insert space literal.")]
        private string layout;

        [SerializeField, Tooltip("Pairs each char with equally-indexed char of layout, ignoring basic whitespace. Use control chars for special chars as with layout. All missing alt chars default to null.")]
        private string altLayout;

        [SerializeField, Tooltip("Prefab to Instantiate on Load().")]
        private GameObject binnedPrefab, keyPrefab;

        [SerializeField, Tooltip("Min and Max angle to normalize stylus orientation input (for left-handed users).")]
        protected Vector3 minAngle, maxAngle;

        // List of binned keys that are children to this layout
        private List<BinnedKey> keys;

        /// <summary>
        /// True after Load() completes without exception
        /// </summary>
        public bool loaded { get; private set; }

        /// <summary>
        /// holder variable, do not use
        /// </summary>
        private bool _useAlternate;

        /// <summary>
        /// Indicates wether the layout is display its alternate keybindings
        /// </summary>
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

        /// <summary>
        /// Reads layout and altLayout and Instantiates using binnedPrefab and keyPrefab to construct
        /// a matching layout to the decoded strings. Sets loaded to true if completed without exception.
        /// 
        /// Throws exception if keyPrefab or binnedPrefab are null, or if layout is null or empty string.
        /// </summary>
        public void Load()
        {
            if (loaded) return;

            Assert.IsNotNull(keyPrefab);
            Assert.IsNotNull(binnedPrefab);

            Assert.IsNotNull(layout);
            Assert.IsFalse(layout.Length == 0);

            keys = new List<BinnedKey>();
            List<List<char>> bins = layout.DecodeIntoBinnedLayout();
            char[] altChars = altLayout?.DecodeIntoBinnedLayout().SelectMany(x => x).ToArray() ?? new char[0];
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

            loaded = true;
        }

        // Checks that provided prefabs satisfy preconditions for use in this class
        private void OnValidate()
        {
            if (!binnedPrefab)
            {
                Debug.LogError("binedPrefab is null!");
            }
            else if (!binnedPrefab.GetComponent<HorizontalLayoutGroup>() && !binnedPrefab.GetComponent<VerticalLayoutGroup>())
            {
                Debug.LogWarning("binnedPrefab has no horizontal or vertical layout group! (recommended, not required)");
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
        /// <returns>char for the rotation defined by stylusForward, null if not loaded</returns>
        public char? GetSelectedLetter(Vector3 stylusForward, bool rightHandedMode = false)
            => GetSelectedLetterUnfiltered(stylusForward.NormalizeAngles(minAngle, maxAngle, rightHandedMode));

        /// <summary>
        /// Returns the character corresponding to the angles provided
        /// </summary>
        /// <param name="normalizedAngles">the normalized angles of the stylus</param>
        /// <returns>char corresponding to normalizedAngles, null if not loaded</returns>
        public char? GetSelectedLetterUnfiltered(Vector3 normalizedAngles)
            => loaded ? FetchInnerKey(normalizedAngles)?.GetChar() : null;

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
            if (!loaded) return;
            var outer = BinnedIndex(normalizedAngles);
            var binnedKey = keys[outer];

            for (int i = 0; i < keys.Count; i++)
            {
                keys[i].highlighted = (i == outer);
            }

            int? inner = null;
            if (binnedKey != null)
            {
                binnedKey.highlighted = true;
                inner = InnerIndex(normalizedAngles);
            }

            for (int i = 0; i < binnedKey.size; i++)
            {
                binnedKey.innerKeys[i].highlighted = (i == inner);
            }
        }

        // gets the index of the bin within keys that is selected by normalizedAngles
        // throws null pointer exception if called before loaded
        private int BinnedIndex(Vector3 normalizedAngles)
            => normalizedAngles.z.NormalizedIntoIndex(keys.Count);

        // gets the index of the key within the selected bin that is selected by normalizedAngles
        // throws null pointer exception if called before loaded
        private int InnerIndex(Vector3 normalizedAngles)
            => (1 - normalizedAngles.x).NormalizedIntoIndex(keys[BinnedIndex(normalizedAngles)].size);

        // gets the selected key
        // throws null pointer exception if called before loaded
        private SimpleKey FetchInnerKey(Vector3 normalizedAngles)
            => keys[BinnedIndex(normalizedAngles)].innerKeys[InnerIndex(normalizedAngles)];
    }

    /// <summary>
    /// Class for an object with an image that toggles between colors when highlighted.
    /// 
    /// Base class of SimpleKey and BinnedKey
    /// </summary>
    public class Highlighted : MonoBehaviour
    {
        // color to transition to on highlight = true
        protected Color highlightColor;

        // image component to change colors on
        protected Image image;

        // storage for previous color of image
        private Color lastColor;
        
        // holder variable, do not use
        private bool _highlighted;

        /// <summary>
        /// True when highlighting, false when not.
        /// </summary>
        public virtual bool highlighted
        {
            get => _highlighted;
            set 
            {
                // if already in state, do nothing
                if (_highlighted == value) return;
                _highlighted = value;

                image = image ?? GetComponent<Image>();

                if (!image) return;

                if (value)
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
    }

    /// <summary>
    /// The component that represents a bin of keys, with helper functions for propagating messages
    /// </summary>
    public class BinnedKey : Highlighted
    {
        /// <summary>
        /// Array of keys inside this bin
        /// </summary>
        public SimpleKey[] innerKeys = new SimpleKey[0];
        
        /// <summary>
        /// Size of this bin in keys
        /// </summary>
        public int size => innerKeys?.Length ?? 0;

        /// <summary>
        /// Highlights self and all inner keys
        /// </summary>
        public override bool highlighted 
        { 
            get => base.highlighted;
            set
            {
                base.highlighted = value;

                if (value) return;

                foreach (var key in innerKeys)
                {
                    key.highlighted = false;
                }
            }
        }

        /// <summary>
        /// Sets all inner keys.useAlternate to value
        /// </summary>
        public bool useAlternate 
        {  
            set
            {
                foreach (var key in innerKeys)
                {
                    key.useAlternate = value;
                }
            }
        }

        // sets highlighting color
        private void Start()
            => highlightColor = new Color(159, 159, 130);
    }

    /// <summary>
    /// The key component, stores one char binding and converts into a label. May also hold char for alternate binding.
    /// </summary>
    public class SimpleKey : Highlighted
    {
        // the TMP_Text on gameObject
        private TMP_Text text;

        // the main binding
        private char _sym;

        // the alt binding
        private char? _alt;

        // holder variable, do not useee
        private bool _useAlternate;

        /// <summary>
        /// When true, provides the alternate binding on GetChar().
        /// When changed, updates the label text appropriately.
        /// </summary>
        public bool useAlternate
        {
            get => _useAlternate;
            set
            {
                _useAlternate = value;
                label = GetLabel(value && alt.HasValue? alt.Value : sym);
            }
        }

        /// <summary>
        /// The main binding of this key.
        /// When changed, updates the label text appropriately.
        /// </summary>
        public char sym
        {
            get => _sym;
            set
            {
                if (_sym != value)
                {
                    _sym = value;
                    useAlternate = useAlternate;
                }
            }
        }

        /// <summary>
        /// The alternate binding of this key.
        /// When changed, updates the label text appropriately.
        /// </summary>
        public char? alt
        {
            get => _alt;
            set
            {
                if (_alt != value)
                {
                    _alt = value;
                    useAlternate = useAlternate;
                }
            }
        }

        /// <summary>
        /// The label of the key, automatically overwritten on alt.set and sym.set and useAlternate.set.
        /// </summary>
        public string label
        {
            get
            {
                text = text ?? GetComponentInChildren<TMP_Text>();
                return text.text;
            }
            set
            {
                text = text ?? GetComponentInChildren<TMP_Text>();
                text.text = value;
            }
        }

        // sets highlight color
        private void Start()
            => highlightColor = new Color(8, 161, 53, 105);

        // turns a char into a label
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

        /// <summary>
        /// Returns the active binding this key represents, accounting for useAlternate and alt == null.
        /// </summary>
        /// <returns>active char</returns>
        public char GetChar()
            => useAlternate && alt.HasValue ? alt.Value : sym;
    }

    namespace Extensions
    { 
        internal static class PrimitiveExtensions
        {
            /// <summary>
            /// Takes a well-formatted string and returns a List of Lists of chars representing the structure of the binned layout.
            /// 
            /// Rules for formatting:
            /// - bins are broken apart by whitespace(s)
            /// - empty bins (multiple whitespaces in a row, at string start) are ignored
            /// - the char '\\' followed by 't', 'n', or 'b' will create the appropriate escape char
            /// - the sequence "\\ " inserts a space bar into the layout
            /// - bins do *not* need to be equally sized
            /// </summary>
            /// <param name="s">the layout string</param>
            /// <returns>structure of the layout</returns>
            public static List<List<char>> DecodeIntoBinnedLayout(this string s)
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

            /// <summary>
            /// Turns the normalized float into an int index in the range [0, length)
            /// </summary>
            /// <param name="normalized">a float in [0.0f, 1.0f]</param>
            /// <param name="length">the length of the item to index into</param>
            /// <returns>the index into the item</returns>
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

            /// <summary>
            /// Creates a vector from a function that takes the axis index and returns a value
            /// </summary>
            /// <param name="f">the generating function</param>
            /// <returns>the vector by applying f to (0, 1, 2)</returns>
            public static Vector3 VectorFrom(System.Func<int, float> f)
                => new Vector3(f(0), f(1), f(2));

            /// <summary>
            /// Returns projected vector on 
            /// - x if axisMask & 0x001, 
            /// - y if axisMask & 0x010,
            /// - z if axismask & 0x100.
            /// </summary>
            /// <param name="vec">vector to project</param>
            /// <param name="axisMask">bit mask by axis</param>
            /// <returns>projected vec to masked axes</returns>
            public static Vector3 ProjectTo(this Vector3 vec, int axisMask)
                // if axis mask has a 1 in the ith place, keep ith value, else 0
                => vec.Map((i, v) => ((axisMask >> i) & 0x1) * v);

            /// <summary>
            /// Flattens vec on axis
            /// </summary>
            /// <param name="vec">vector to flatten</param>
            /// <param name="axis">axis to flatten</param>
            /// <returns>a new (dim(vec) - 1) vector</returns>
            public static Vector3 Flatten(this Vector3 vec, int axis)
                => vec.ProjectTo(~(1 << axis));
        
            /// <summary>
            /// Maps the enumerated components (by axis) using f and returns a new vector
            /// </summary>
            /// <param name="vec">original</param>
            /// <param name="f">mapping function</param>
            /// <returns>applies f to each (i, vec[i]) for i in [0, 1, 2]</returns>
            public static Vector3 Map(this Vector3 vec, System.Func<int, float, float> f)
                => new Vector3(f(0, vec[0]), f(1, vec[1]), f(2, vec[2]));

            /// <summary>
            /// Gets the angle from start arm around axis
            /// </summary>
            /// <param name="forward">the end of angle</param>
            /// <param name="startArm">the start of angle</param>
            /// <param name="axis">the axis of angle</param>
            /// <returns>the signed angle from startArm to forward around axis</returns>
            public static float SignedAngleFromAxis(this Vector3 forward, Vector3 startArm, int axis)
                => forward.Flatten(axis).SignedAngle(startArm, VectorFrom(i => i == axis ? 1 : 0));

            /// <summary>
            /// Returns a vector with components in [0.0f, 1.0f] based on their interpolating value between their respective min and max components.
            /// </summary>
            /// <param name="stylusForward">the direction down the tip of the stylus</param>
            /// <param name="minAngle">the min angles (for left-handed users)</param>
            /// <param name="maxAngle">the max angles (for left-handed users)</param>
            /// <param name="rightHanded">when true, flips min and max for x axis to mirror bounds</param>
            /// <returns>a vector of normalized components representing inverse interpolation of min and max angles</returns>
            public static Vector3 NormalizeAngles(this Vector3 stylusForward, Vector3 minAngle, Vector3 maxAngle, bool rightHanded = false)
                => VectorFrom(axis =>
                {
                    // if measuring angle for y (axis == 1), measure rotation around forward (0, 0, 1), else measure rotation around up (0, 1, 0)
                    Vector3 angleOrigin = axis == 1 ? Vector3.forward : Vector3.up;
                    float angle = stylusForward.SignedAngleFromAxis(angleOrigin, axis);

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
}
