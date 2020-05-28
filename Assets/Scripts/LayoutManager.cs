using System;
using System.Linq;
using UnityEngine;

[Serializable]
public enum LayoutOption
{
    SquashedQWERTY,
    LinearABCDE,
}

public class LayoutManager : MonoBehaviour
{
    public LayoutOption layout;

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
