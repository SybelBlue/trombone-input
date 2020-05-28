using System.Collections.Generic;
using UnityEngine;

namespace CustomInput
{

    public abstract class Layout : MonoBehaviour
    {
        /// <summary>
        /// Prefabs for the basic layout key and basic block key
        /// </summary>
        public GameObject basicItem, blockItem;

        /// <summary>
        /// All of the keys in this layout
        /// </summary>
        protected LayoutKey[] keys;

        /// <summary>
        /// Map of value to GameObject
        /// </summary>
        /// <typeparam name="GameObject"></typeparam>
        /// <returns></returns>
        protected readonly List<GameObject> childMap = new List<GameObject>(64);

        protected virtual void Start()
        {
            keys = FillKeys();

            foreach (var item in keys)
            {
                var newChild = item.representation(transform, blockItem, basicItem);

                var blockController = newChild.GetComponent<KeyController>();

                if (blockController)
                {
                    blockController.item = item;
                }

                for (int i = 0; i < item.size(); i++)
                {
                    childMap.Add(newChild);
                }
            }

            ResizeAll();

        }

        /// <summary>
        /// Last width of this item
        /// </summary>
        protected float lastWidth = -1;

        private void Update()
        {
            var width = gameObject.GetComponent<RectTransform>().rect.width;

            if (lastWidth == width) return;

            ResizeAll();
        }

        /// <summary>
        /// SetHighlight(false) on all AbstractDisplayItemControllers
        /// </summary>
        protected void UnhighlightAll()
        {
            foreach (var cont in gameObject.GetComponentsInChildren<KeyController>())
            {
                cont.SetHighlight(false);
            }
        }

        /// <summary>
        /// Returns the GameObject at value index
        /// </summary>
        /// <param name="index">key</param>
        /// <returns>value at index</returns>
        public GameObject ChildAt(int index) => childMap.Count <= index ? null : childMap[index];

        /// <summary>
        /// <example>
        /// <code>
        /// Equivalent to ChildAt(index)?.GetComponent<LayoutKey>()
        /// </example>
        /// </summary>
        /// <typeparam name="LayoutKey"></typeparam>
        /// <returns>LayoutKey at index</returns>
        public LayoutKey LayoutKeyAt(int index) => ChildAt(index)?.GetComponent<KeyController>().item;

        /// <summary>
        /// The chars for the key at index
        /// </summary>
        /// <returns>string of key chars</returns>
        public string CharsFor(int index) => LayoutKeyAt(index)?.data ?? "";


        /// <summary>
        /// Resize all child GameObjects to fit within this and to scale
        /// </summary>
        public abstract void ResizeAll();

        /// <summary>
        /// Sets the item at index (or no item if null) to be highlighted and all others to be unhiglighted
        /// </summary>
        /// <param name="index">index of item to highlight</param>
        public abstract void SetHighlightedKey(int? index);

        /// <summary>
        /// Gets the largest key and smallest key that are situated at index
        /// </summary>
        /// <param name="index">index of the keys to find</param>
        /// <returns>The largest key containing the smallest key containing index</returns>
        public abstract (LayoutKey, SimpleKey)? KeysAt(int index);

        public abstract (char, bool)? GetLetterFor(string context, int index);

        /// <summary>
        /// The name of the layout
        /// </summary>
        /// <value></value>
        public abstract string layoutName { get; }

        /// <summary>
        /// The method to fill the keys field on this, called in Start
        /// </summary>
        /// <returns>Array of all keys in this layout</returns>
        protected abstract LayoutKey[] FillKeys();
    }
}