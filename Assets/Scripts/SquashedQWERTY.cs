using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace CustomInput
{
    public class SquashedQWERTY : Layout
    {

        public override string layoutName { get => "Squashed QWERTY"; }

        public override void ResizeAll()
        {
            var width = gameObject.GetComponent<RectTransform>().rect.width;
            var unitWidth = width / 64.0f;

            foreach (var child in gameObject.GetComponentsInChildren<KeyController>())
            {
                child.Resize(unitWidth);
            }
        }

        public override (LayoutKey, SimpleKey)? KeysAt(int index)
        {
            if (!gameObject.activeInHierarchy) return null;

            Assert.IsFalse(index < 0);

            int remaining = index;
            foreach (LayoutKey item in keys)
            {
                if (remaining < item.size)
                {
                    return (item, item.ItemAt(remaining));
                }
                else
                {
                    remaining -= item.size;
                }
            }

            return null;
        }

        public override (char, bool)? GetLetterFor(string context, int index)
        {
            var s = CharsFor(index);
            if (s == null) return null;
            if (context == null || context.Length == 0) context = " ";
            var c = naive[context.ToCharArray()[context.Length - 1]][s];
            return (c, false);
        }

        public override void SetHighlightedKey(int? index)
        {
            UnhighlightAll();

            if (index.HasValue)
            {
                ChildAt(index.Value)?.GetComponent<AmbiguousKeyController>()?.SetHighlight(true);
            }
        }


        protected override LayoutKey[] FillKeys()
        {
            var basicItem0 = new SimpleKey('Q', 2);
            var basicItem1 = new SimpleKey('A', 3);
            var basicItem2 = new SimpleKey('Z', 2);
            var blockItem3 = new AmbiguousKey(true, basicItem0, basicItem1, basicItem2);

            var basicItem4 = new SimpleKey('W', 2);
            var basicItem5 = new SimpleKey('S', 3);
            var basicItem6 = new SimpleKey('X', 2);
            var blockItem7 = new AmbiguousKey(true, basicItem4, basicItem5, basicItem6);

            var basicItem8 = new SimpleKey('E', 2);
            var basicItem9 = new SimpleKey('D', 3);
            var basicItem10 = new SimpleKey('C', 2);
            var blockItem11 = new AmbiguousKey(true, basicItem8, basicItem9, basicItem10);

            var basicItem12 = new SimpleKey('R', 2);
            var basicItem13 = new SimpleKey('F', 3);
            var basicItem14 = new SimpleKey('V', 2);
            var blockItem15 = new AmbiguousKey(true, basicItem12, basicItem13, basicItem14);

            var basicItem16 = new SimpleKey('T', 2);
            var basicItem17 = new SimpleKey('G', 3);
            var basicItem18 = new SimpleKey('B', 2);
            var blockItem19 = new AmbiguousKey(true, basicItem16, basicItem17, basicItem18);

            var basicItem20 = new SimpleKey('U', 2);
            var basicItem21 = new SimpleKey('H', 3);
            var basicItem22 = new SimpleKey('Y', 2);
            var blockItem23 = new AmbiguousKey(false, basicItem20, basicItem21, basicItem22);

            var basicItem24 = new SimpleKey('N', 2);
            var basicItem25 = new SimpleKey('J', 3);
            var basicItem26 = new SimpleKey('I', 2);
            var blockItem27 = new AmbiguousKey(false, basicItem24, basicItem25, basicItem26);

            var basicItem28 = new SimpleKey('M', 2);
            var basicItem29 = new SimpleKey('K', 3);
            var basicItem30 = new SimpleKey('O', 2);
            var blockItem31 = new AmbiguousKey(false, basicItem28, basicItem29, basicItem30);

            // var basicItem32 = ScriptableObject.CreateInstance<BasicLayoutItem>();
            // basicItem32.init('.', 2);
            var basicItem33 = new SimpleKey('L', 3);
            var basicItem34 = new SimpleKey('P', 2);
            var blockItem35 = new AmbiguousKey(false, /* basicItem32,*/ basicItem33, basicItem34);
            return new LayoutKey[] {
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
        private static readonly Dictionary<char, Dictionary<string, char>> naive = new Dictionary<char, Dictionary<string, char>>
    {
        {'A', new Dictionary<string, char>
            {
                {"QAZ", 'Z'},
                {"WSX", 'S'},
                {"EDC", 'C'},
                {"RFV", 'R'},
                {"TGB", 'T'},
                {"UHY", 'U'},
                {"NJI", 'N'},
                {"MKO", 'M'},
                {"LP", 'L'},
            }
        },
        {'B', new Dictionary<string, char>
            {
                {"QAZ", 'A'},
                {"WSX", 'S'},
                {"EDC", 'E'},
                {"RFV", 'R'},
                {"TGB", 'B'},
                {"UHY", 'U'},
                {"NJI", 'I'},
                {"MKO", 'O'},
                {"LP", 'L'},
            }
        },
        {'C', new Dictionary<string, char>
            {
                {"QAZ", 'A'},
                {"WSX", 'S'},
                {"EDC", 'E'},
                {"RFV", 'R'},
                {"TGB", 'T'},
                {"UHY", 'H'},
                {"NJI", 'I'},
                {"MKO", 'O'},
                {"LP", 'L'},
            }
        },
        {'D', new Dictionary<string, char>
            {
                {"QAZ", 'A'},
                {"WSX", 'S'},
                {"EDC", 'E'},
                {"RFV", 'R'},
                {"TGB", 'G'},
                {"UHY", 'U'},
                {"NJI", 'I'},
                {"MKO", 'O'},
                {"LP", 'L'},
            }
        },
        {'E', new Dictionary<string, char>
            {
                {"QAZ", 'A'},
                {"WSX", 'S'},
                {"EDC", 'D'},
                {"RFV", 'R'},
                {"TGB", 'T'},
                {"UHY", 'U'},
                {"NJI", 'N'},
                {"MKO", 'M'},
                {"LP", 'L'},
            }
        },
        {'F', new Dictionary<string, char>
            {
                {"QAZ", 'A'},
                {"WSX", 'S'},
                {"EDC", 'E'},
                {"RFV", 'F'},
                {"TGB", 'T'},
                {"UHY", 'U'},
                {"NJI", 'I'},
                {"MKO", 'O'},
                {"LP", 'L'},
            }
        },
        {'G', new Dictionary<string, char>
            {
                {"QAZ", 'A'},
                {"WSX", 'S'},
                {"EDC", 'E'},
                {"RFV", 'R'},
                {"TGB", 'G'},
                {"UHY", 'U'},
                {"NJI", 'I'},
                {"MKO", 'O'},
                {"LP", 'L'},
            }
        },
        {'H', new Dictionary<string, char>
            {
                {"QAZ", 'A'},
                {"WSX", 'S'},
                {"EDC", 'E'},
                {"RFV", 'R'},
                {"TGB", 'T'},
                {"UHY", 'Y'},
                {"NJI", 'I'},
                {"MKO", 'O'},
                {"LP", 'L'},
            }
        },
        {'I', new Dictionary<string, char>
            {
                {"QAZ", 'A'},
                {"WSX", 'S'},
                {"EDC", 'C'},
                {"RFV", 'V'},
                {"TGB", 'T'},
                {"UHY", 'U'},
                {"NJI", 'N'},
                {"MKO", 'O'},
                {"LP", 'L'},
            }
        },
        {'J', new Dictionary<string, char>
            {
                {"QAZ", 'A'},
                {"WSX", 'S'},
                {"EDC", 'E'},
                {"RFV", 'R'},
                {"TGB", 'T'},
                {"UHY", 'U'},
                {"NJI", 'I'},
                {"MKO", 'O'},
                {"LP", 'L'},
            }
        },
        {'K', new Dictionary<string, char>
            {
                {"QAZ", 'A'},
                {"WSX", 'S'},
                {"EDC", 'E'},
                {"RFV", 'R'},
                {"TGB", 'T'},
                {"UHY", 'Y'},
                {"NJI", 'I'},
                {"MKO", 'O'},
                {"LP", 'L'},
            }
        },
        {'L', new Dictionary<string, char>
            {
                {"QAZ", 'A'},
                {"WSX", 'S'},
                {"EDC", 'E'},
                {"RFV", 'V'},
                {"TGB", 'T'},
                {"UHY", 'Y'},
                {"NJI", 'I'},
                {"MKO", 'O'},
                {"LP", 'L'},
            }
        },
        {'M', new Dictionary<string, char>
            {
                {"QAZ", 'A'},
                {"WSX", 'S'},
                {"EDC", 'E'},
                {"RFV", 'F'},
                {"TGB", 'B'},
                {"UHY", 'U'},
                {"NJI", 'I'},
                {"MKO", 'O'},
                {"LP", 'P'},
            }
        },
        {'N', new Dictionary<string, char>
            {
                {"QAZ", 'A'},
                {"WSX", 'S'},
                {"EDC", 'E'},
                {"RFV", 'F'},
                {"TGB", 'G'},
                {"UHY", 'U'},
                {"NJI", 'I'},
                {"MKO", 'O'},
                {"LP", 'P'},
            }
        },
        {'O', new Dictionary<string, char>
            {
                {"QAZ", 'A'},
                {"WSX", 'S'},
                {"EDC", 'C'},
                {"RFV", 'R'},
                {"TGB", 'T'},
                {"UHY", 'U'},
                {"NJI", 'N'},
                {"MKO", 'M'},
                {"LP", 'L'},
            }
        },
        {'P', new Dictionary<string, char>
            {
                {"QAZ", 'A'},
                {"WSX", 'S'},
                {"EDC", 'E'},
                {"RFV", 'R'},
                {"TGB", 'T'},
                {"UHY", 'H'},
                {"NJI", 'I'},
                {"MKO", 'O'},
                {"LP", 'L'},
            }
        },
        {'Q', new Dictionary<string, char>
            {
                {"QAZ", 'A'},
                {"WSX", 'S'},
                {"EDC", 'E'},
                {"RFV", 'R'},
                {"TGB", 'T'},
                {"UHY", 'U'},
                {"NJI", 'I'},
                {"MKO", 'O'},
                {"LP", 'L'},
            }
        },
        {'R', new Dictionary<string, char>
            {
                {"QAZ", 'A'},
                {"WSX", 'S'},
                {"EDC", 'E'},
                {"RFV", 'R'},
                {"TGB", 'T'},
                {"UHY", 'Y'},
                {"NJI", 'I'},
                {"MKO", 'O'},
                {"LP", 'P'},
            }
        },
        {'S', new Dictionary<string, char>
            {
                {"QAZ", 'A'},
                {"WSX", 'S'},
                {"EDC", 'E'},
                {"RFV", 'F'},
                {"TGB", 'T'},
                {"UHY", 'H'},
                {"NJI", 'I'},
                {"MKO", 'O'},
                {"LP", 'P'},
            }
        },
        {'T', new Dictionary<string, char>
            {
                {"QAZ", 'A'},
                {"WSX", 'S'},
                {"EDC", 'E'},
                {"RFV", 'R'},
                {"TGB", 'T'},
                {"UHY", 'H'},
                {"NJI", 'I'},
                {"MKO", 'O'},
                {"LP", 'L'},
            }
        },
        {'U', new Dictionary<string, char>
            {
                {"QAZ", 'A'},
                {"WSX", 'S'},
                {"EDC", 'C'},
                {"RFV", 'R'},
                {"TGB", 'T'},
                {"UHY", 'Y'},
                {"NJI", 'N'},
                {"MKO", 'M'},
                {"LP", 'L'},
            }
        },
        {'V', new Dictionary<string, char>
            {
                {"QAZ", 'A'},
                {"WSX", 'S'},
                {"EDC", 'E'},
                {"RFV", 'R'},
                {"TGB", 'T'},
                {"UHY", 'U'},
                {"NJI", 'I'},
                {"MKO", 'O'},
                {"LP", 'L'},
            }
        },
        {'W', new Dictionary<string, char>
            {
                {"QAZ", 'A'},
                {"WSX", 'S'},
                {"EDC", 'E'},
                {"RFV", 'R'},
                {"TGB", 'B'},
                {"UHY", 'H'},
                {"NJI", 'I'},
                {"MKO", 'O'},
                {"LP", 'L'},
            }
        },
        {'X', new Dictionary<string, char>
            {
                {"QAZ", 'A'},
                {"WSX", 'S'},
                {"EDC", 'E'},
                {"RFV", 'F'},
                {"TGB", 'T'},
                {"UHY", 'Y'},
                {"NJI", 'I'},
                {"MKO", 'O'},
                {"LP", 'P'},
            }
        },
        {'Y', new Dictionary<string, char>
            {
                {"QAZ", 'A'},
                {"WSX", 'S'},
                {"EDC", 'C'},
                {"RFV", 'R'},
                {"TGB", 'T'},
                {"UHY", 'U'},
                {"NJI", 'N'},
                {"MKO", 'M'},
                {"LP", 'L'},
            }
        },
        {'Z', new Dictionary<string, char>
            {
                {"QAZ", 'A'},
                {"WSX", 'W'},
                {"EDC", 'E'},
                {"RFV", 'R'},
                {"TGB", 'B'},
                {"UHY", 'Y'},
                {"NJI", 'I'},
                {"MKO", 'O'},
                {"LP", 'L'},
            }
        },
        {' ', new Dictionary<string, char>
            {
                {"QAZ", 'A'},
                {"WSX", 'S'},
                {"EDC", 'C'},
                {"RFV", 'R'},
                {"TGB", 'T'},
                {"UHY", 'U'},
                {"NJI", 'N'},
                {"MKO", 'M'},
                {"LP", 'P'},
            }
        },
    };

    }
}