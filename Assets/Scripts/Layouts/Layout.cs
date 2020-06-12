using System.Collections.Generic;
using UnityEngine;

namespace CustomInput
{

    public readonly struct InputData
    {
        public const int MAX_RAW_VALUE = 64;

        public readonly string context;

        public readonly float normalizedX, normalizedZ;

        public readonly float? normalizedSlider;

        public readonly int? rawValue;

        public readonly bool frontButtonDown, backButtonDown;

        public readonly (Vector3 origin, Vector3 direction) orientation;

        public InputData(
            string context,
            int? rawValue,
            float normalizedX,
            float normalizedZ,
            float? normalizedSlider,
            bool frontButtonDown,
            bool backButtonDown,
            (Vector3 origin, Vector3 direction) orientation
            )
        {
            this.rawValue = rawValue;
            this.context = context;
            this.normalizedX = normalizedX;
            this.normalizedZ = normalizedZ;
            this.normalizedSlider = normalizedSlider;
            this.frontButtonDown = frontButtonDown;
            this.backButtonDown = backButtonDown;
            this.orientation = orientation;
        }
    }

    public abstract class Layout : MonoBehaviour
    {
        // Prefabs for the basic layout key and basic block key
        public GameObject simpleKeyPrefab, binnedKeyPrefab, stylusKeyPrefab, stylusBinnedPrefab, raycastKeyPrefab;

        // All of the keys in this layout
        protected LayoutKey[] keys;

        // Map of value to GameObject
        protected readonly List<GameObject> childMap = new List<GameObject>(64);

        protected virtual void Start()
        {
            keys = FillKeys();

            var objectDict = new Dictionary<LayoutObjectType, GameObject>
                { { LayoutObjectType.BinnedKeyPrefab, binnedKeyPrefab }
                , { LayoutObjectType.SimpleKeyPrefab, simpleKeyPrefab }
                , { LayoutObjectType.StylusKeyPrefab, stylusKeyPrefab }
                , { LayoutObjectType.StylusBinnedPrefab, stylusBinnedPrefab }
                , { LayoutObjectType.RaycastKeyPrefab, raycastKeyPrefab }
                };

            foreach (var key in keys)
            {
                var newChild = key.Representation(transform, objectDict);

                var blockController = newChild.GetComponent<IKeyController>();

                if (blockController)
                {
                    blockController.layoutKey = key;
                }

                for (int i = 0; i < key.size; i++)
                {
                    childMap.Add(newChild);
                }
            }

            ResizeAll();
        }

        public virtual void UpdateState(InputData data)
            => SetHighlightedKey(data);

        // Last width of this item
        protected float lastWidth = -1;

        private void Update()
        {
            var width = gameObject.GetComponent<RectTransform>().rect.width;

            if (lastWidth == width) return;

            lastWidth = width;
            ResizeAll();
        }

        // SetHighlight(false) on all AbstractDisplayItemControllers
        protected void UnhighlightAll()
        {
            foreach (var cont in gameObject.GetComponentsInChildren<IKeyController>())
            {
                cont.SetHighlight(false);
            }
        }

        // Resize all child GameObjects to fit within this and to scale
        public virtual void ResizeAll()
        {
            var width = gameObject.GetComponent<RectTransform>().rect.width;
            var height = gameObject.GetComponent<RectTransform>().rect.height;
            var unitWidth = width / 64.0f;
            var unitHeight = height / 22.0f;

            foreach (var child in gameObject.GetComponentsInChildren<IKeyController>())
            {
                child.Resize(unitWidth);
                child.ResizeHeight(unitHeight);
            }
        }

        // Returns the GameObject at value index
        public GameObject ChildFor(InputData data)
        {
            var index = ChildIndexFor(data);
            return childMap.Count <= index || index < 0 ? null : childMap[index];
        }

        public abstract int ChildIndexFor(InputData data);

        // Equivalent to
        // ```ChildAt(index)?.GetComponent<LayoutKey>()```
        public LayoutKey LayoutKeyFor(InputData data) => ChildFor(data)?.GetComponent<IKeyController>().layoutKey;

        // The chars for the key at index
        public string CharsFor(InputData data) => LayoutKeyFor(data)?.label ?? "";

        // Sets the item at index (or no item if null) to be highlighted and all others to be unhiglighted
        public abstract void SetHighlightedKey(InputData data);

        // Gets the largest key and smallest key that are situated at index, or null if the index is out of bounds
        // (if this is not a binned key, then the tuple items should be equal)
        public abstract (LayoutKey parent, SimpleKey simple)? KeysFor(InputData data);

        // Gets the letter for the keypress at index, given the context, and a boolean representing
        // certainty, or null if the index is out of bounds.
        // May alter state of layout and return null.
        public abstract (char letter, bool certain)? GetSelectedLetter(InputData data);

        // The method to fill the keys field on this, called in Start
        protected abstract LayoutKey[] FillKeys();

        public abstract bool usesSlider { get; }

        public abstract bool useAlternate { set; get; }
    }
}
