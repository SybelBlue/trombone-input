using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;
using Controllers.Keys;

namespace CustomInput
{
    // The full set of key prefabs, see Layout.Start for usage
    public enum LayoutObjectType
    {
        SimpleKeyPrefab,
        BinnedKeyPrefab,
        StylusKeyPrefab,
        StylusBinnedPrefab,
        RaycastKeyPrefab,
    }

    // The Base class for the data contained within a KeyController,
    // responsible for saving the concrete type name, the size in sensor widths
    // (how many of the 64 output levels correspond to this key), and the label on the key
    public abstract class LayoutKey
    {
        // the property holding name of the most concrete subclass that extends this
        public abstract string typeName { get; }

        // the property holding size in sensor widths (must be positive)
        // size is defined by how many of the 64 output levels correspond to this key, when applicable,
        // can be used as a proxy for the importance of the letter(s) on this key
        // e.g. A would usually have a bigger size than Q or V
        public abstract int size { get; }

        // the property holding character(s) this key represents when activated
        public abstract string label { get; }

        // the single character key at index widths into the key
        public abstract SimpleKey ItemAt(int index);

        // a method that defines how this key should be represented in GUI. see KeyController.cs
        public abstract GameObject Representation(Transform parent, Dictionary<LayoutObjectType, GameObject> objectDict);

        public override string ToString()
            => $"{typeName} [{label}]";
    }

    // The Base class for LayoutKeys with only a single character (like a standard QWERTY keyboard key)
    public class SimpleKey : LayoutKey
    {
        public override string typeName => "SimpleKey";

        // the char this key represents
        public readonly char c;

        public readonly char? alt;

        public override string label
            => new string(new char[] { c });

        private readonly int _size;
        public override int size => _size;

        public SimpleKey(char data, int size, char? alt = null)
        {
            this.c = data;
            this._size = size;
            this.alt = alt;
        }

        public override GameObject Representation(Transform parent, Dictionary<LayoutObjectType, GameObject> objectDict)
            => RepresentationUsing<Simple>(parent, objectDict[LayoutObjectType.SimpleKeyPrefab]);

        // Used in SimpleKey.Representation to make a default key and set the new object's T.symbol to this.c
        protected GameObject RepresentationUsing<T>(Transform parent, GameObject prefab)
            where T : AbstractSimple
        {
            var newItem = GameObject.Instantiate(prefab, parent);
            newItem.GetComponent<T>().symbol = c;
            return newItem;
        }

        public override SimpleKey ItemAt(int index)
        {
            Assert.IsTrue(0 <= index && index < _size);
            return this;
        }

        public char CharWithAlternate(bool useAlternate)
            => useAlternate && alt.HasValue ? alt.Value : c;
    }

    // The Base class for all keys with binned labeling
    public class BinnedKey : LayoutKey
    {
        public override string typeName => "BinnedKey";

        public override string label => _data;
        public override int size => _size;

        private readonly string _data;
        private readonly int _size;


        protected readonly SimpleKey[] items;
        protected readonly bool slant;

        public BinnedKey(bool slant, params SimpleKey[] subitems)
        {
            items = subitems;
            this.slant = slant;

            _data = items?.Select(i => i.label).Aggregate((a, b) => a + b) ?? "";

            _size = items?.Select(i => i.size).Sum() ?? 0;
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
            var newItem = GameObject.Instantiate(objectDict[LayoutObjectType.BinnedKeyPrefab], parent);
            var controller = newItem.GetComponent<Binned>();
            foreach (var item in items)
            {
                var newChild = item.Representation(parent, objectDict);

                newChild.GetComponent<Simple>().data = item;

                controller.AddChild(newChild);
            }
            controller.SetSlant(slant);
            return newItem;
        }
    }

    // The SimpleKey for Stylus canvases
    public class StylusKey : SimpleKey
    {
        public override string typeName => "StylusKey";

        public StylusKey(char data, int size, char? alt = null) : base(data, size, alt)
        { }

        public override GameObject Representation(Transform parent, Dictionary<LayoutObjectType, GameObject> objectDict)
            => RepresentationUsing<Stylus>(parent, objectDict[LayoutObjectType.StylusKeyPrefab]);
    }

    // The BinnedKey for Stylus canvases
    public class StylusBinnedKey : BinnedKey
    {
        public override string typeName => "StylusBinnedKey";

        public StylusBinnedKey(bool slant, params SimpleKey[] subitems) : base(slant, subitems)
        { }

        public override GameObject Representation(Transform parent, Dictionary<LayoutObjectType, GameObject> objectDict)
        {
            var newItem = GameObject.Instantiate(objectDict[LayoutObjectType.StylusBinnedPrefab], parent);
            var controller = newItem.GetComponent<StylusBinned>();
            foreach (var i in items)
            {
                var newChild = i.Representation(parent, objectDict);

                newChild.GetComponent<Stylus>().data = i;

                controller.AddChild(newChild);
            }
            return newItem;
        }
    }

    public class RaycastKey : StylusKey
    {
        public override string typeName => "RaycastKey";

        public RaycastKey(char data, int size, char? alt = null) : base(data, size, alt)
        { }

        public override GameObject Representation(Transform parent, Dictionary<LayoutObjectType, GameObject> objectDict)
            => RepresentationUsing<Raycast>(parent, objectDict[LayoutObjectType.RaycastKeyPrefab]);
    }
}