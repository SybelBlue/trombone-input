using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;

namespace CustomInput
{

    public enum LayoutObjectType
    {
        SimpleKeyPrefab,
        AmbiguousKeyPrefab,
        StylusKeyPrefab,
        StylusAmbiguousPrefab,
    }

    // cannot be an assetmenu item
    public abstract class LayoutKey
    {
        public abstract int size { get; }

        public abstract string data { get; }
        public abstract SimpleKey ItemAt(int index);

        public abstract GameObject Representation(Transform parent, Dictionary<LayoutObjectType, GameObject> objectDict);
    }


    public class SimpleKey : LayoutKey
    {

        private readonly char c;

        public override string data
            => new string(new char[] { c });

        private readonly int SIZE;
        public override int size => SIZE;

        public SimpleKey(char data, int size)
        {
            this.c = data;
            this.SIZE = size;
        }

        public override GameObject Representation(Transform parent, Dictionary<LayoutObjectType, GameObject> objectDict)
            => RepresentationUsing(parent, objectDict[LayoutObjectType.SimpleKeyPrefab]);

        protected GameObject RepresentationUsing(Transform parent, GameObject simpleKeyPrefab)
        {
            var newItem = GameObject.Instantiate(simpleKeyPrefab, parent);
            newItem.GetComponent<SimpleKeyController>().symbol = c;
            return newItem;
        }

        public override SimpleKey ItemAt(int index)
        {
            Assert.IsTrue(index < SIZE);
            return this;
        }
    }

    public class AmbiguousKey : LayoutKey
    {
        public override string data => DATA;
        public override int size => SIZE;

        private readonly string DATA;
        private readonly int SIZE;


        private readonly SimpleKey[] items;
        private readonly bool slant;

        public AmbiguousKey(bool slant, params SimpleKey[] subitems)
        {
            items = subitems;
            this.slant = slant;

            DATA = items?.Select(i => i.data).Aggregate((a, b) => a + b);

            SIZE = items?.Select(i => i.size).Sum() ?? 0;
        }

        public override SimpleKey ItemAt(int index)
        {
            int remaining = index;
            foreach (SimpleKey item in items)
            {
                remaining -= item.size;

                if (0 > remaining)
                {
                    return item;
                }
            }

            throw new ArgumentException("index to large");
        }

        public override GameObject Representation(Transform parent, Dictionary<LayoutObjectType, GameObject> objectDict)
        {
            var newItem = GameObject.Instantiate(objectDict[LayoutObjectType.AmbiguousKeyPrefab], parent);
            var controller = newItem.GetComponent<AmbiguousKeyController>();
            foreach (var i in items)
            {
                var newChild = i.Representation(parent, objectDict);

                newChild.GetComponent<SimpleKeyController>().item = i;

                controller.AddChild(newChild);
            }
            controller.SetSlant(slant);
            return newItem;
        }
    }

    public class StylusKey : SimpleKey
    {
        public StylusKey(char data, int size) : base(data, size)
        { }

        public override GameObject Representation(Transform parent, Dictionary<LayoutObjectType, GameObject> objectDict)
            => RepresentationUsing(parent, objectDict[LayoutObjectType.StylusKeyPrefab]);
    }
}