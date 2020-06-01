using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;
using System;

namespace CustomInput
{
    // cannot be an assetmenu item
    public abstract class LayoutKey : ScriptableObject
    {
        public abstract int size();

        public abstract GameObject representation(Transform parent, GameObject ambiguousPrefab, GameObject simplePrefab);

        public abstract SimpleKey ItemAt(int index);

        public abstract string data { get; }
    }


    public class SimpleKey : LayoutKey
    {

        private char c;

        public override string data
        {
            get => new string(new char[] { c });
        }

        private int SIZE;

        public void init(char data, int size)
        {
            this.c = data;
            this.SIZE = size;
        }

        public override GameObject representation(Transform parent, GameObject ambiguousPrefab, GameObject simplePrefab)
        {
            var newItem = Instantiate(simplePrefab, parent);
            newItem.GetComponent<SimpleKeyController>().setSymbol(c);
            return newItem;
        }

        public override int size()
        {
            return SIZE;
        }

        public override SimpleKey ItemAt(int index)
        {
            Assert.IsTrue(index < SIZE);
            return this;
        }
    }

    public class AmbiguousKey : LayoutKey
    {
        public override string data
        {
            get => items?.Select(i => i.data).Aggregate((a, b) => a + b);
        }

        private SimpleKey[] items;
        private bool slant;

        public void init(bool slant, params SimpleKey[] subitems)
        {
            items = subitems;
            this.slant = slant;
        }

        public override SimpleKey ItemAt(int index)
        {
            int remaining = index;
            foreach (SimpleKey item in items)
            {
                remaining -= item.size();

                if (0 > remaining)
                {
                    return item;
                }
            }

            throw new ArgumentException("index to large");
        }

        public override GameObject representation(Transform parent, GameObject ambiguousPrefab, GameObject simplePrefab)
        {
            var newItem = Instantiate(ambiguousPrefab, parent);
            var controller = newItem.GetComponent<AmbiguousKeyController>();
            foreach (var i in items)
            {
                var newChild = i.representation(parent, ambiguousPrefab, simplePrefab);

                newChild.GetComponent<SimpleKeyController>().item = i;

                controller.AddChild(newChild);
            }
            controller.SetSlant(slant);
            return newItem;
        }

        public override int size()
        {
            return items?.Select(i => i.size()).Sum() ?? 0;
        }
    }
}