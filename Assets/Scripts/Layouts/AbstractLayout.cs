using Controller.Key;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace CustomInput
{
    public struct InputData
    {
        public readonly Vector3 normalizedAngles;

        public readonly float? normalizedSlider;

        public readonly uint? rawValue;

        public readonly IRaycastable stylusHit;

        public InputData(uint? rawValue, Controller.Stylus stylusModel) : this(
                rawValue,
                stylusModel.normalizedAngles,
                stylusModel.normalizedSlider
            )
        { }

        public InputData(
            uint? rawValue,
            Vector3 normalizedAngles,
            float? normalizedSlider
            )
        {
            this.rawValue = rawValue;
            this.normalizedAngles = normalizedAngles;
            this.normalizedSlider = normalizedSlider;
            
            stylusHit = IRaycastable.current;
        }
    }

    namespace Layout
    {
        using CustomInput.KeyData;
#pragma warning disable 649
        public abstract class AbstractLayout : MonoBehaviour
        {
            public RectTransform rectTransform
            { get; protected set; }

            [SerializeField]
            // Prefabs for the basic layout key and basic block key
            private GameObject simpleKeyPrefab, binnedKeyPrefab, stylusKeyPrefab, stylusBinnedPrefab, raycastKeyPrefab;

            // All of the Key in this layout
            protected AbstractData[] keys;

            // True if this layout uses the slider on the stylus
            public abstract bool usesSlider { get; }

            // True if this layout provides a key on lifting a finger from the slider
            public abstract bool keyOnFingerUp { get; }

            // True if this layout raycasts
            public abstract bool usesRaycasting { get; }

            // Determines if the LayoutKeys are displaying the alternate label
            public abstract bool useAlternate { set; get; }

            // Map of value to GameObject
            protected readonly List<GameObject> childMap = new List<GameObject>(64);

            protected void Start()
            {
                rectTransform = GetComponent<RectTransform>();

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

                    var controller = newChild.GetComponent<IKey>();

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
            }

            public void UpdateState(InputData data)
                => SetHighlightedKey(data);

            // Last width of this item
            protected float lastWidth = -1;

            private void Update()
            {
                var width = rectTransform.rect.width;

                if (lastWidth == width) return;

                lastWidth = width;
                ResizeAll();
            }

            // SetHighlight(false) on all IKeys
            protected void UnhighlightAll()
            {
                foreach (var cont in gameObject.GetComponentsInChildren<IKey>())
                {
                    cont.SetHighlight(false);
                }
            }

            // Resize all child GameObjects to fit within this and to scale
            public virtual void ResizeAll()
            {
                var width = rectTransform.rect.width;
                var height = rectTransform.rect.height;
                var unitWidth = width / 59;//(float)Bindings._slider_max_value;
                var unitHeight = height / 22.0f;

                foreach (var child in gameObject.GetComponentsInChildren<IKey>())
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

            // Boundaries for valid stylus angles
            public virtual Utils.Tuples.VBounds? StylusRotationBounds()
                  => null;

            protected abstract int ChildIndexFor(InputData data);

            // Equivalent to
            // ```ChildAt(index)?.GetComponent<LayoutKey>()```
            protected AbstractData LayoutKeyFor(InputData data) => ChildFor(data)?.GetComponent<IKey>()?.layoutKey;

            // Sets the item at index (or no item if null) to be highlighted and all others to be unhiglighted
            public abstract void SetHighlightedKey(InputData data);

            // Gets the largest key and smallest key that are situated at index, or null if the index is out of bounds
            // (if this is not a binned key, then the tuple items should be equal)
            public abstract Utils.Tuples.NestedData? KeysFor(InputData data);

            // Gets the letter for the keypress at index, given the context, and a boolean representing
            // certainty, or null if the index is out of bounds.
            // May alter state of layout and return null.
            public abstract char? GetSelectedLetter(InputData data);

            // The method to fill the Key field on this, called in Start
            protected abstract AbstractData[] FillKeys();
        }
    }
}