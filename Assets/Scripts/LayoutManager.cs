using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public enum LayoutOption
{
    SquashedQWERTY,
    LinearABCDE,
}

# pragma warning disable 649
public class LayoutManager : MonoBehaviour
{
    public LayoutOption layout;

    public Dropdown dropdown;

    [SerializeField]
    private CustomInput.SquashedQWERTY squashedQWERTY;

    [SerializeField]
    private CustomInput.LinearABCDE linearABCDE;

    public CustomInput.Layout currentLayout() => fromOption(layout);

    public CustomInput.Layout fromOption(LayoutOption option)
    {
        switch (option)
        {
            case LayoutOption.SquashedQWERTY:
                return squashedQWERTY;

            case LayoutOption.LinearABCDE:
                return linearABCDE;
        }

        throw new ArgumentException($"unknown layout option: {option.ToString()} in fromOption");
    }

    public void Start()
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(new List<string>(Enum.GetNames(typeof(LayoutOption))));
        dropdown.value = 0;
    }

    public void DropdownValueSelected(int index) => layout = (LayoutOption)index;

    private void Update()
    {
        foreach (var layout in GetComponentsInChildren<CustomInput.Layout>().Where(layout => layout.gameObject.activeInHierarchy))
        {
            layout.gameObject.SetActive(false);
        }

        var current = currentLayout();
        if (!current.gameObject.activeInHierarchy) current.gameObject.SetActive(true);
    }
}
