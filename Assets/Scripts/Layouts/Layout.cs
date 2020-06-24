using System.Collections.Generic;
using UnityEngine;

namespace CustomInput
{

    public readonly struct InputData
    {
        public readonly Vector3 normalizedAngles;

        public readonly float? normalizedSlider;

        public readonly int? rawValue;

        public readonly (Vector3 origin, Vector3 direction) orientation;

        public InputData(int? rawValue, StylusModelController stylusModel) : this(
                rawValue,
                stylusModel.normalizedAngles,
                stylusModel.normalizedSlider,
                stylusModel.orientation
            )
        { }

        public InputData(
            int? rawValue,
            Vector3 normalizedAngles,
            float? normalizedSlider,
            (Vector3 origin, Vector3 direction) orientation
            )
        {
            this.rawValue = rawValue;
            this.normalizedAngles = normalizedAngles;
            this.normalizedSlider = normalizedSlider;
            this.orientation = orientation;
        }
    }

    public abstract class Layout : MonoBehaviour
    {
        // Prefabs for the basic layout key and basic block key
        public GameObject simpleKeyPrefab, binnedKeyPrefab, stylusKeyPrefab, stylusBinnedPrefab, raycastKeyPrefab;

        // All of the keys in this layout
        protected LayoutKey[] keys;

        // True if this layout uses the slider on the stylus
        public abstract bool usesSlider { get; }

        // True if this layout raycasts
        public abstract bool usesRaycasting { get; }

        // Determines if the LayoutKeys are displaying the alternate label
        public abstract bool useAlternate { set; get; }

        // Map of value to GameObject
        protected readonly List<GameObject> childMap = new List<GameObject>(64);

        protected void Start()
        {
            BeforeStart();

            keys = FillKeys();

            var objectDict = new Dictionary<LayoutObjectType, GameObject>
                { { LayoutObjectType.BinnedKeyPrefab, binnedKeyPrefab }
                , { LayoutObjectType.SimpleKeyPrefab, simpleKeyPrefab }
                , { LayoutObjectType.StylusKeyPrefab, stylusKeyPrefab }
                , { LayoutObjectType.StylusBinnedPrefab, stylusBinnedPrefab }
                , { LayoutObjectType.RaycastKeyPrefab, raycastKeyPrefab }
                };

            for (int i = 0; i < keys.Length; i++)
            {
                var key = keys[i];

                var newChild = key.Representation(ParentForNthChild(i), objectDict);

                var controller = newChild.GetComponent<IKeyController>();

                if (controller)
                {
                    controller.layoutKey = key;
                }

                for (int _i = 0; _i < key.size; _i++)
                {
                    childMap.Add(newChild);
                }
            }

            ResizeAll();

            AfterStart();
        }

        protected virtual void BeforeStart()
        { }

        protected virtual Transform ParentForNthChild(int n)
            => transform;

        protected virtual void AfterStart()
        { }

        public void UpdateState(InputData data)
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
        public virtual GameObject ChildFor(InputData data)
        {
            var index = ChildIndexFor(data);
            return childMap.Count <= index || index < 0 ? null : childMap[index];
        }

        protected abstract int ChildIndexFor(InputData data);

        // Equivalent to
        // ```ChildAt(index)?.GetComponent<LayoutKey>()```
        protected LayoutKey LayoutKeyFor(InputData data) => ChildFor(data)?.GetComponent<IKeyController>().layoutKey;

        // Sets the item at index (or no item if null) to be highlighted and all others to be unhiglighted
        public abstract void SetHighlightedKey(InputData data);

        // Gets the largest key and smallest key that are situated at index, or null if the index is out of bounds
        // (if this is not a binned key, then the tuple items should be equal)
        public abstract (LayoutKey parent, SimpleKey simple)? KeysFor(InputData data);

        // Gets the letter for the keypress at index, given the context, and a boolean representing
        // certainty, or null if the index is out of bounds.
        // May alter state of layout and return null.
        public abstract char? GetSelectedLetter(InputData data);

        // The method to fill the keys field on this, called in Start
        protected abstract LayoutKey[] FillKeys();
    }
}
