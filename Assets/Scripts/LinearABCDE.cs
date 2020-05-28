using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace CustomInput
{
    public class LinearABCDE : Layout
    {

        public override string layoutName { get => "Linear ABCDE"; }

        protected override void Start()
        {
            base.Start();

            foreach (SimpleKeyController cont in GetComponentsInChildren<SimpleKeyController>())
            {
                var newColor = cont.background.color;
                newColor.a = 1.0f;
                cont.background.color = newColor;
            }
        }

        public override void ResizeAll()
        {
            var width = gameObject.GetComponent<RectTransform>().rect.width;
            var unitWidth = width / 64.0f;

            foreach (var child in gameObject.GetComponentsInChildren<AbstractKeyController>())
            {
                child.Resize(unitWidth);
            }
        }

        public override (LayoutKey, SimpleKey)? KeysAt(int index)
        {
            var lKey = LayoutKeyAt(index);
            return (lKey, (SimpleKey)lKey);
        }

        public override void SetHighlightedKey(int? index)
        {
            UnhighlightAll();

            if (index.HasValue)
            {
                ChildAt(index.Value)?.GetComponent<AbstractKeyController>()?.SetHighlight(true);
            }
        }

        // Auto-generated 
        protected override LayoutKey[] FillKeys()
        {
            var basicItem0 = ScriptableObject.CreateInstance<SimpleKey>();
            basicItem0.init('A', 3);


            var basicItem1 = ScriptableObject.CreateInstance<SimpleKey>();
            basicItem1.init('B', 2);


            var basicItem2 = ScriptableObject.CreateInstance<SimpleKey>();
            basicItem2.init('C', 2);


            var basicItem3 = ScriptableObject.CreateInstance<SimpleKey>();
            basicItem3.init('D', 2);


            var basicItem4 = ScriptableObject.CreateInstance<SimpleKey>();
            basicItem4.init('E', 3);


            var basicItem5 = ScriptableObject.CreateInstance<SimpleKey>();
            basicItem5.init('F', 2);


            var basicItem6 = ScriptableObject.CreateInstance<SimpleKey>();
            basicItem6.init('G', 2);


            var basicItem7 = ScriptableObject.CreateInstance<SimpleKey>();
            basicItem7.init('H', 2);


            var basicItem8 = ScriptableObject.CreateInstance<SimpleKey>();
            basicItem8.init('I', 3);


            var basicItem9 = ScriptableObject.CreateInstance<SimpleKey>();
            basicItem9.init('J', 2);


            var basicItem10 = ScriptableObject.CreateInstance<SimpleKey>();
            basicItem10.init('K', 2);


            var basicItem11 = ScriptableObject.CreateInstance<SimpleKey>();
            basicItem11.init('L', 2);


            var basicItem12 = ScriptableObject.CreateInstance<SimpleKey>();
            basicItem12.init('M', 2);


            var basicItem13 = ScriptableObject.CreateInstance<SimpleKey>();
            basicItem13.init('N', 3);


            var basicItem14 = ScriptableObject.CreateInstance<SimpleKey>();
            basicItem14.init('O', 3);


            var basicItem15 = ScriptableObject.CreateInstance<SimpleKey>();
            basicItem15.init('P', 2);


            var basicItem16 = ScriptableObject.CreateInstance<SimpleKey>();
            basicItem16.init('Q', 2);


            var basicItem17 = ScriptableObject.CreateInstance<SimpleKey>();
            basicItem17.init('R', 3);


            var basicItem18 = ScriptableObject.CreateInstance<SimpleKey>();
            basicItem18.init('S', 2);


            var basicItem19 = ScriptableObject.CreateInstance<SimpleKey>();
            basicItem19.init('T', 3);


            var basicItem20 = ScriptableObject.CreateInstance<SimpleKey>();
            basicItem20.init('U', 2);


            var basicItem21 = ScriptableObject.CreateInstance<SimpleKey>();
            basicItem21.init('V', 2);


            var basicItem22 = ScriptableObject.CreateInstance<SimpleKey>();
            basicItem22.init('W', 2);


            var basicItem23 = ScriptableObject.CreateInstance<SimpleKey>();
            basicItem23.init('X', 2);


            var basicItem24 = ScriptableObject.CreateInstance<SimpleKey>();
            basicItem24.init('Y', 2);


            var basicItem25 = ScriptableObject.CreateInstance<SimpleKey>();
            basicItem25.init('Z', 2);
            return new LayoutKey[] {
                        basicItem0,
                        basicItem1,
                        basicItem2,
                        basicItem3,
                        basicItem4,
                        basicItem5,
                        basicItem6,
                        basicItem7,
                        basicItem8,
                        basicItem9,
                        basicItem10,
                        basicItem11,
                        basicItem12,
                        basicItem13,
                        basicItem14,
                        basicItem15,
                        basicItem16,
                        basicItem17,
                        basicItem18,
                        basicItem19,
                        basicItem20,
                        basicItem21,
                        basicItem22,
                        basicItem23,
                        basicItem24,
                        basicItem25
                };
        }
    }
}