using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;
using System;

// cannot be an assetmenu item
public abstract class LayoutKey : ScriptableObject
{
    public abstract int size();

    public abstract GameObject representation(Transform parent, GameObject block, GameObject basic);

    public abstract SimpleKey ItemAt(int index);

    public abstract string data { get; }
}


// needs to be put in file with same name as class
// [CreateAssetMenu(fileName = "SimpleKey", menuName = "trombone-input/SimpleKey", order = 0)]
public class SimpleKey : LayoutKey
{

    private char c;

    public override string data
    {
        get => c + "";
    }

    private int SIZE;

    public void init(char data, int size)
    {
        this.c = data;
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

    public override SimpleKey ItemAt(int index)
    {
        Assert.IsTrue(index < SIZE);
        return this;
    }
}

// needs to be put in file with same name as class
// [CreateAssetMenu(fileName = "AmbiguousKey", menuName = "trombone-input/AmbiguousKey", order = 0)]
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
