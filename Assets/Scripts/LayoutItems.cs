using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;
using System;

public abstract class LayoutItem : ScriptableObject
{
    public abstract int size();

    public abstract GameObject representation(Transform parent, GameObject block, GameObject basic);

    public abstract BasicLayoutItem ItemAt(int index);
}

public class BasicLayoutItem : LayoutItem
{
    public char data { get; private set; }
    private int SIZE;

    public void init(char data, int size)
    {
        this.data = data;
        this.SIZE = size;
    }

    public override GameObject representation(Transform parent, GameObject block, GameObject basic)
    {
        var newItem = Instantiate(basic, parent);
        newItem.GetComponent<BasicDisplayItemController>().setSymbol(data);
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
    private BasicLayoutItem[] items;
    private bool slant;

    public void init(bool slant, params BasicLayoutItem[] subitems)
    {
        items = subitems;
        this.slant = slant;
    }

    public char[] data()
    {
        return items?.Select(i => i.data).ToArray();
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
