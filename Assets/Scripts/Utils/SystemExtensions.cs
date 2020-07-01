namespace Utils
{
    namespace SystemExtensions
    {
        public static class CollectionExtensions
        {
            // gets the last item of the list (or errs trying)
            public static T Last<T>(this T[] array)
                => array.FromEnd(0);

            public static T FromEnd<T>(this T[] array, int i)
                => array[System.Math.Max(0, array.Length - 1) - i];

            public static void SetFromEnd<T>(this T[] array, int i, T value)
                => array[System.Math.Max(0, array.Length - 1) - i] = value;

            public static void OptionalAdd<T>(this System.Collections.Generic.List<T> list, T? value)
                where T : struct
            {
                if (value.HasValue)
                {
                    list.Add(value.Value);
                }
            }
        }

        public static class StringExtensions
        {
            public static string Intercalate(this string[] strings, string inner)
            {
                string final = "";

                for (int i = 0; i < strings.Length - 1; i++)
                {
                    final += strings[i] + inner;
                }

                if (strings.Length > 0)
                {
                    final += strings.Last();
                }

                return final;
            }

            // O(x * log2(n)) where x is O(string.+=) method to repeat s, used in display
            public static string Repeat(this string s, int n)
            {
                if (n <= 0) return "";
                string curr = s;
                string final = "";
                while (n > 0)
                {
                    if ((n & 0x1) != 0)
                    {
                        final += curr;
                    }

                    n >>= 1;
                    curr += curr;
                }

                return final;
            }

            public static string Backspace(this string s)
                    => s.Substring(0, System.Math.Max(0, s.Length - 1));
        }
    }
}