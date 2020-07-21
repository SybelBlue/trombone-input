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
            get { return _layout; }
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
        GameObject practiceEndButton;
        GameObject liveDropDownMenu;
        GameObject challengeTypeIndicatorText;

        Vector3 practiceTransForm;
        Vector3 liveDropTransForm;
        Vector3 challengeTypeIndicatorTransForm;


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

        // GameObject

        public void Start()
        {
            practiceEndButton = GameObject.FindWithTag("PracticeEndButtonTag");
            liveDropDownMenu = GameObject.FindWithTag("LiveDropDownTag");
            challengeTypeIndicatorText = GameObject.FindWithTag("ChallengeTypeIndicatorTag");


            dropdown.ClearOptions();
            dropdown.AddOptions(new List<string>(Enum.GetNames(typeof(LayoutOption))));
            dropdown.value = (int)layout;
            _memoizedLayout = _layout;
            ActivateLayout();
            //practiceTransForm = new Vector3(practiceEndButton.transform.position.x, 6.5498f, practiceEndButton.transform.position.z);
            //liveDropTransForm = new Vector3(liveDropDownMenu.transform.position.x, 6.5498f, liveDropDownMenu.transform.position.z);
           // challengeTypeIndicatorTransForm = new Vector3(challengeTypeIndicatorText.transform.position.x, 6.5498f, challengeTypeIndicatorText.transform.position.z);
        }

        public void Update()
        {
            if (_memoizedLayout != _layout)
            {
                layout = _layout;
            }

            checkForArchType();

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

        public void checkForArchType()
        {
          if (currentLayout == stylusBinnedABCDE)
          {
            // Vector3 vPos = new Vector3(-30, -30, 0);
            //Vector3 practiceTransFormNew = new Vector3(currentLayout.transform.position.x-5f, 6.5498f, currentLayout.transform.position.z);
            //Vector3 liveDropTransFormNew = new Vector3(currentLayout.transform.position.x-2.5f, 6.5498f, currentLayout.transform.position.z);
            //Vector3 challengeTypeIndicatorTransFormNew = new Vector3(currentLayout.transform.position.x+3.5f, 6.5498f, currentLayout.transform.position.z);


                //liveDropDownMenu.transform.position = liveDropTransFormNew;
                //practiceEndButton.transform.position = practiceTransFormNew;
                //challengeTypeIndicatorText.transform.position = challengeTypeIndicatorTransFormNew;

                RectTransform rt = GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, 300);

          }
            else
          {
                //liveDropDownMenu.transform.position = liveDropTransForm;
                //practiceEndButton.transform.position = practiceTransForm;
                //challengeTypeIndicatorText.transform.position = challengeTypeIndicatorTransForm;

                RectTransform rt = GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, 100);
            }
        }
    }
}
