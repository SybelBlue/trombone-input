using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace CustomInput
{
    public class SquashedQWERTY : Layout
    {

        public override string layoutName => "Squashed QWERTY";

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

        public override (char, bool)? GetLetterFor(InputData data)
        {
            var s = CharsFor(data.rawValue);
            if (s == null) return null;
            string context = data.context;
            char last = context == null || context.Length == 0 ? context.ToCharArray()[context.Length - 1] : ' ';
            var inner = naive[last];
            var c = inner[s];
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

        // Auto-generated 
        protected override LayoutKey[] FillKeys()
        {
            return new LayoutKey[] {
                    new AmbiguousKey(true,
                            new SimpleKey('Q', 2),
                            new SimpleKey('A', 3),
                            new SimpleKey('Z', 2)
                    ),
                    new AmbiguousKey(true,
                            new SimpleKey('W', 2),
                            new SimpleKey('S', 3),
                            new SimpleKey('X', 2)
                    ),
                    new AmbiguousKey(true,
                            new SimpleKey('E', 2),
                            new SimpleKey('D', 3),
                            new SimpleKey('C', 2)
                    ),
                    new AmbiguousKey(true,
                            new SimpleKey('R', 2),
                            new SimpleKey('F', 3),
                            new SimpleKey('V', 2)
                    ),
                    new AmbiguousKey(true,
                            new SimpleKey('T', 2),
                            new SimpleKey('G', 3),
                            new SimpleKey('B', 2)
                    ),
                    new AmbiguousKey(false,
                            new SimpleKey('U', 2),
                            new SimpleKey('H', 3),
                            new SimpleKey('Y', 2)
                    ),
                    new AmbiguousKey(false,
                            new SimpleKey('N', 2),
                            new SimpleKey('J', 3),
                            new SimpleKey('I', 2)
                    ),
                    new AmbiguousKey(false,
                            new SimpleKey('M', 2),
                            new SimpleKey('K', 3),
                            new SimpleKey('O', 2)
                    ),
                    new AmbiguousKey(false,
                            new SimpleKey('L', 3),
                            new SimpleKey('P', 2)
                    )
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