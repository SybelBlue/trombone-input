using System.Collections.Generic;
using UnityEngine;

namespace CustomInput
{

    public readonly struct InputData
    {
        public const int MAX_RAW_VALUE = 64;

        public readonly string context;

        public readonly float? normalizedX, normalizedZ, normalizedPotentiometer;

        public readonly int rawValue;

        public InputData(int raw) : this(null, raw)
        { }

        public InputData(string context, int raw) : this(context, raw, null, null, null)
        { }

        public InputData(string context, int raw, float? normalizedX, float? normalizedZ, float? normalizedPotentiometer)
        {
            rawValue = raw;
            this.context = context;
            this.normalizedX = normalizedX;
            this.normalizedZ = normalizedZ;
            this.normalizedPotentiometer = normalizedPotentiometer;
        }
    }

    public abstract class Layout : MonoBehaviour
    {
        // Prefabs for the basic layout key and basic block key
        public GameObject simpleKeyPrefab, ambiguousKeyPrefab, stylusKeyPrefab, stylusAmbiguousPrefab;

        // All of the keys in this layout
        protected LayoutKey[] keys;

        // Map of value to GameObject
        protected readonly List<GameObject> childMap = new List<GameObject>(64);

        protected virtual void Start()
        {
            keys = FillKeys();

            var objectDict = new Dictionary<LayoutObjectType, GameObject>
                { { LayoutObjectType.AmbiguousKeyPrefab, ambiguousKeyPrefab }
                , { LayoutObjectType.SimpleKeyPrefab, simpleKeyPrefab }
                , { LayoutObjectType.StylusKeyPrefab, stylusKeyPrefab }
                , { LayoutObjectType.StylusAmbiguousPrefab, stylusAmbiguousPrefab }
                };

            foreach (var item in keys)
            {
                var newChild = item.Representation(transform, objectDict);

                var blockController = newChild.GetComponent<KeyController>();

                if (blockController)
                {
                    blockController.item = item;
                }

                for (int i = 0; i < item.size; i++)
                {
                    childMap.Add(newChild);
                }
            }

            ResizeAll();
        }

        public virtual void UpdateState(InputData data)
            => SetHighlightedKey(MainController.inputThisFrame ? (int?)data.rawValue : null);

        // Last width of this item
        protected float lastWidth = -1;

        private void Update()
        {
            var width = gameObject.GetComponent<RectTransform>().rect.width;

            if (lastWidth == width) return;

            ResizeAll();
        }

        // SetHighlight(false) on all AbstractDisplayItemControllers
        protected void UnhighlightAll()
        {
            foreach (var cont in gameObject.GetComponentsInChildren<KeyController>())
            {
                cont.SetHighlight(false);
            }
        }

        // Returns the GameObject at value index
        public GameObject ChildAt(int index) => childMap.Count <= index ? null : childMap[index];

        // Equivalent to 
        // ```ChildAt(index)?.GetComponent<LayoutKey>()```
        public LayoutKey LayoutKeyAt(int index) => ChildAt(index)?.GetComponent<KeyController>().item;

        // The chars for the key at index
        public string CharsFor(int index) => LayoutKeyAt(index)?.data ?? "";


        // Resize all child GameObjects to fit within this and to scale
        public abstract void ResizeAll();

        // Sets the item at index (or no item if null) to be highlighted and all others to be unhiglighted
        public abstract void SetHighlightedKey(int? index);

        // Gets the largest key and smallest key that are situated at index, or null if the index is out of bounds
        // (if this is not an ambiguous key, then the tuple items should be equal)
        public abstract (LayoutKey, SimpleKey)? KeysAt(int index);

        // Gets the letter for the keypress at index, given the context, and a boolean representing
        // certainty, or null if the index is out of bounds
        public abstract (char, bool)? GetLetterFor(InputData data);

        // The name of the layout
        public abstract string layoutName { get; }

        // The method to fill the keys field on this, called in Start
        protected abstract LayoutKey[] FillKeys();
    }
}