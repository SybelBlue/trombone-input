using System.Collections.Generic;
using UnityEngine;

namespace CustomInput
{

    public abstract class Layout : MonoBehaviour
    {
        public GameObject basicItem, blockItem;

        protected LayoutKey[] items;
        protected readonly List<GameObject> childMap = new List<GameObject>(64);

        protected void Start()
        {
            items = FillItems();

            foreach (var item in items)
            {
                var newChild = item.representation(transform, blockItem, basicItem);

                var blockController = newChild.GetComponent<BlockDisplayItemController>();

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
        float lastWidth = -1;

        private void Update()
        {
            var width = gameObject.GetComponent<RectTransform>().rect.width;

            if (lastWidth == width) return;

            ResizeAll();
        }

        public abstract void ResizeAll();

        public abstract void SetHighlightedKey(int? index);

        public abstract (LayoutKey, SimpleKey)? KeysAt(int index);

        public abstract string CharsFor(int index);

        public abstract string layoutName { get; }

        protected abstract LayoutKey[] FillItems();
    }
}