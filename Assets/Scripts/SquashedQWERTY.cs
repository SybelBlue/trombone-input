using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SquashedQWERTY : CustomInput.Layout
{

    public override string layoutName { get => "Squashed QWERTY"; }

    public GameObject basicItem, blockItem;

    readonly List<GameObject> childMap = new List<GameObject>(64);

    void Start()
    {
        fillItems();

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

    public override void ResizeAll()
    {
        var width = gameObject.GetComponent<RectTransform>().rect.width;
        var unitWidth = width / 64.0f;

        foreach (var child in gameObject.GetComponentsInChildren<AbstractDisplayItemController>())
        {
            child.Resize(unitWidth);
        }
    }

    public override (LayoutKey, SimpleKey)? KeysAt(int index)
    {
        Assert.IsFalse(index < 0);

        int remaining = index;
        foreach (LayoutKey item in items)
        {
            if (remaining < item.size())
            {
                return (item, item.ItemAt(remaining));
            }
            else
            {
                remaining -= item.size();
            }
        }

        return null;
    }

    public override string CharsFor(int index) => ChildAt(index)?.GetComponent<LayoutKey>()?.data ?? "";

    public GameObject ChildAt(int index) => childMap.Count <= index ? null : childMap[index];

    public override void SetHighlightedKey(int? index)
    {
        foreach (var cont in gameObject.GetComponentsInChildren<AbstractDisplayItemController>())
        {
            cont.SetHighlight(false);
        }

        if (index.HasValue)
        {
            ChildAt(index.Value)?.GetComponent<BlockDisplayItemController>()?.SetHighlight(true);
        }
    }

    // Auto-generated 
    private LayoutKey[] items;
    private void fillItems()
    {
        var basicItem0 = ScriptableObject.CreateInstance<SimpleKey>();
        basicItem0.init('Q', 2);
        var basicItem1 = ScriptableObject.CreateInstance<SimpleKey>();
        basicItem1.init('A', 3);
        var basicItem2 = ScriptableObject.CreateInstance<SimpleKey>();
        basicItem2.init('Z', 2);
        var blockItem3 = ScriptableObject.CreateInstance<AmbiguousKey>();
        blockItem3.init(true, basicItem0, basicItem1, basicItem2);

        var basicItem4 = ScriptableObject.CreateInstance<SimpleKey>();
        basicItem4.init('W', 2);
        var basicItem5 = ScriptableObject.CreateInstance<SimpleKey>();
        basicItem5.init('S', 3);
        var basicItem6 = ScriptableObject.CreateInstance<SimpleKey>();
        basicItem6.init('X', 2);
        var blockItem7 = ScriptableObject.CreateInstance<AmbiguousKey>();
        blockItem7.init(true, basicItem4, basicItem5, basicItem6);

        var basicItem8 = ScriptableObject.CreateInstance<SimpleKey>();
        basicItem8.init('E', 2);
        var basicItem9 = ScriptableObject.CreateInstance<SimpleKey>();
        basicItem9.init('D', 3);
        var basicItem10 = ScriptableObject.CreateInstance<SimpleKey>();
        basicItem10.init('C', 2);
        var blockItem11 = ScriptableObject.CreateInstance<AmbiguousKey>();
        blockItem11.init(true, basicItem8, basicItem9, basicItem10);

        var basicItem12 = ScriptableObject.CreateInstance<SimpleKey>();
        basicItem12.init('R', 2);
        var basicItem13 = ScriptableObject.CreateInstance<SimpleKey>();
        basicItem13.init('F', 3);
        var basicItem14 = ScriptableObject.CreateInstance<SimpleKey>();
        basicItem14.init('V', 2);
        var blockItem15 = ScriptableObject.CreateInstance<AmbiguousKey>();
        blockItem15.init(true, basicItem12, basicItem13, basicItem14);

        var basicItem16 = ScriptableObject.CreateInstance<SimpleKey>();
        basicItem16.init('T', 2);
        var basicItem17 = ScriptableObject.CreateInstance<SimpleKey>();
        basicItem17.init('G', 3);
        var basicItem18 = ScriptableObject.CreateInstance<SimpleKey>();
        basicItem18.init('B', 2);
        var blockItem19 = ScriptableObject.CreateInstance<AmbiguousKey>();
        blockItem19.init(true, basicItem16, basicItem17, basicItem18);

        var basicItem20 = ScriptableObject.CreateInstance<SimpleKey>();
        basicItem20.init('U', 2);
        var basicItem21 = ScriptableObject.CreateInstance<SimpleKey>();
        basicItem21.init('H', 3);
        var basicItem22 = ScriptableObject.CreateInstance<SimpleKey>();
        basicItem22.init('Y', 2);
        var blockItem23 = ScriptableObject.CreateInstance<AmbiguousKey>();
        blockItem23.init(false, basicItem20, basicItem21, basicItem22);

        var basicItem24 = ScriptableObject.CreateInstance<SimpleKey>();
        basicItem24.init('N', 2);
        var basicItem25 = ScriptableObject.CreateInstance<SimpleKey>();
        basicItem25.init('J', 3);
        var basicItem26 = ScriptableObject.CreateInstance<SimpleKey>();
        basicItem26.init('I', 2);
        var blockItem27 = ScriptableObject.CreateInstance<AmbiguousKey>();
        blockItem27.init(false, basicItem24, basicItem25, basicItem26);

        var basicItem28 = ScriptableObject.CreateInstance<SimpleKey>();
        basicItem28.init('M', 2);
        var basicItem29 = ScriptableObject.CreateInstance<SimpleKey>();
        basicItem29.init('K', 3);
        var basicItem30 = ScriptableObject.CreateInstance<SimpleKey>();
        basicItem30.init('O', 2);
        var blockItem31 = ScriptableObject.CreateInstance<AmbiguousKey>();
        blockItem31.init(false, basicItem28, basicItem29, basicItem30);

        // var basicItem32 = ScriptableObject.CreateInstance<BasicLayoutItem>();
        // basicItem32.init('.', 2);
        var basicItem33 = ScriptableObject.CreateInstance<SimpleKey>();
        basicItem33.init('L', 3);
        var basicItem34 = ScriptableObject.CreateInstance<SimpleKey>();
        basicItem34.init('P', 2);
        var blockItem35 = ScriptableObject.CreateInstance<AmbiguousKey>();
        blockItem35.init(false, /* basicItem32,*/ basicItem33, basicItem34);
        items = new LayoutKey[] {
            blockItem3,
            blockItem7,
            blockItem11,
            blockItem15,
            blockItem19,
            blockItem23,
            blockItem27,
            blockItem31,
            blockItem35
        };
    }

}