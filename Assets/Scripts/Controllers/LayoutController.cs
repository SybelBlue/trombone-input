using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CustomInput
{

    [Serializable]
    public enum LayoutOption
    {
        LinearABCDE,
        StylusBinnedABCDE,
        TwoRotBinnedABCDE,
        RaycastQWERTY,
    }

#pragma warning disable 649
    public class LayoutController : MonoBehaviour
    {
        public LayoutOption layout;

        [SerializeField]
        private LayoutDropdownController dropdownController;

        private Dropdown dropdown
            => dropdownController.dropdown;

        [SerializeField]
        private LinearABCDE linearABCDE;

        [SerializeField]
        private StylusBinnedABCDE stylusBinnedABCDE;

        [SerializeField]
        private TwoRotationABCDE twoRotationABCDE;

        [SerializeField]
        private RaycastQWERTY raycastQWERTY;

        public Layout currentLayout() => fromOption(layout);

        public Layout fromOption(LayoutOption option)
        {
            switch (option)
            {
                case LayoutOption.LinearABCDE:
                    return linearABCDE;

                case LayoutOption.StylusBinnedABCDE:
                    return stylusBinnedABCDE;

                case LayoutOption.TwoRotBinnedABCDE:
                    return twoRotationABCDE;

                case LayoutOption.RaycastQWERTY:
                    return raycastQWERTY;
            }

            throw new ArgumentException($"unknown layout option: {option.ToString()} in fromOption");
        }

        public void Start()
        {
            dropdown.ClearOptions();
            dropdown.AddOptions(new List<string>(Enum.GetNames(typeof(LayoutOption))));
            dropdown.value = (int)layout;
        }

        public void DropdownValueSelected(int index)
            => layout = (LayoutOption)index;

        private void Update()
        {
            dropdown.value = (int)layout;

            foreach (var layoutOption in System.Enum.GetValues(typeof(LayoutOption)))
            {
                var layout = fromOption((LayoutOption)layoutOption);
                if (layout.gameObject.activeInHierarchy)
                {
                    layout.gameObject.SetActive(false);
                }
            }

            var current = currentLayout();
            if (!current.gameObject.activeInHierarchy)
            {
                current.gameObject.SetActive(true);
            }
        }
    }
}