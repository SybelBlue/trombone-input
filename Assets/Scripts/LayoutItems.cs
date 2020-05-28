using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;
using System;

public abstract class LayoutItem : ScriptableObject
{
    public abstract int size();

    public abstract GameObject representation(Transform parent, GameObject block, GameObject basic);

    public abstract BasicLayoutItem ItemAt(int index);

    public abstract string data { get; protected set; }
}

public class BasicLayoutItem : LayoutItem
{

    private char c;

    public override string data
    {
        get => c + "";
        protected set
        {
            Assert.IsTrue(value.Length == 1);
            c = data.ToCharArray()[0];
        }
    }

    private int SIZE;

    public void init(char data, int size)
    {
        this.data = data + "";
        this.SIZE = size;
    }

    public override GameObject representation(Transform parent, GameObject block, GameObject basic)
    {
        var newItem = Instantiate(basic, parent);
        newItem.GetComponent<BasicDisplayItemController>().setSymbol(c);
        return newItem;
    }

    public override int size()
    {
        return SIZE;
    }

    public override BasicLayoutItem ItemAt(int index)
    {
        Assert.IsTrue(index < SIZE);
        return this;
    }
}

public class BlockLayoutItem : LayoutItem
{
    public override string data
    {
        get => items?.Select(i => i.data).Aggregate((a, b) => a + b);
        protected set => throw new InvalidOperationException("cannot set");
    }

    private BasicLayoutItem[] items;
    private bool slant;

    public void init(bool slant, params BasicLayoutItem[] subitems)
    {
        items = subitems;
        this.slant = slant;
    }

    public override BasicLayoutItem ItemAt(int index)
    {
        int remaining = index;
        foreach (BasicLayoutItem item in items)
        {
            remaining -= item.size();

            if (0 > remaining)
            {
                return item;
            }
        }

        throw new ArgumentException("index to large");
    }

    public override GameObject representation(Transform parent, GameObject block, GameObject basic)
    {
        var newItem = Instantiate(block, parent);
        var controller = newItem.GetComponent<BlockDisplayItemController>();
        foreach (var i in items)
        {
            var newChild = i.representation(parent, block, basic);

            newChild.GetComponent<BasicDisplayItemController>().item = i;

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
