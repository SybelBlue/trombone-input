using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomInput.Layout;

namespace CustomInput
{

    [Serializable]
    public enum LayoutOption : int
    {
        SliderOnly = 0,
        ArcType = 1,
        TiltType = 2,
        Raycast = 3,
    }

#pragma warning disable 649
    public class LayoutManager : MonoBehaviour
    {
        [SerializeField]
        private LayoutOption _layout;
        private LayoutOption _memoizedLayout;
        public LayoutOption layout
        {
            get => _layout;
            set
            {
                _layout = value;
                _memoizedLayout = value;
                ActivateLayout();
            }
        }

        #region EditorSet
        [SerializeField]
        private Controller.LayoutDropdown dropdownController;

        private Dropdown dropdown
            => dropdownController.dropdown;

        [SerializeField]
        private SliderOnlyLayout linearABCDE;

        [SerializeField]
        private ArcTypeLayout stylusBinnedABCDE;
        // private GameObject circleStylus = GameObject.FindGameObjectWithTag("CircularStylus");


        [SerializeField]
        private TiltTypeLayout twoRotationABCDE;

        [SerializeField]
        private RaycastLayout raycastQWERTY;
        #endregion

        public AbstractLayout currentLayout => fromOption(layout);

        public AbstractLayout fromOption(LayoutOption option)
        {
            switch (option)
            {
                case LayoutOption.SliderOnly:
                    return linearABCDE;

                case LayoutOption.ArcType:
                    return stylusBinnedABCDE;

                case LayoutOption.TiltType:
                    return twoRotationABCDE;

                case LayoutOption.Raycast:
                    return raycastQWERTY;
            }

            throw new ArgumentException($"unknown layout option: {option.ToString()} in fromOption");
        }

        public void Start()
        {
            dropdown.ClearOptions();
            dropdown.AddOptions(new List<string>(Enum.GetNames(typeof(LayoutOption))));
            dropdown.value = (int)layout;
            _memoizedLayout = _layout;
            ActivateLayout();
        }

        public void Update()
        {
            if (_memoizedLayout != _layout)
            {
                layout = _layout;
            }
        }

        // used in editor!
        public void DropdownValueSelected(int index)
        {
            layout = (LayoutOption)index;
            dropdown.value = index;
        }

        private void ActivateLayout()
        {
            foreach (var layoutOption in Enum.GetValues(typeof(LayoutOption)))
            {
                var layout = fromOption((LayoutOption)layoutOption);
                if (layout.gameObject.activeInHierarchy)
                {
                    layout.gameObject.SetActive(false);
                }
            }

            currentLayout.gameObject.SetActive(true);
        }
    }
}
