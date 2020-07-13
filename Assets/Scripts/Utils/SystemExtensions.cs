using System;
using System.Collections.Generic;
using System.Linq;

namespace Utils.SystemExtensions
{
    public static class PrimitiveExtensions
    {
        // returns the value in the range [min, max) via modular arithmetic
        public static float ModIntoRange(this float value, float min, float max)
        {
            if (max == min) return min;
            // TTS: find (min + c) such that value = k * window + (min + c) for some integer k and 0 <= c < window
            // given:
            // - window = max - min != 0
            // - c mod window = c (derived from 0 <= c < window)
            // - int k such that value = k * window + (min + c)
            // - (r mod b) is the function that takes reals r and b and returns the remainder of r / b
            // then:
            // value = k * window + (min + c)
            // value - min = k * window + c
            // (value - min) mod window = (k * window + c) mod window
            // distributing (mod window) over (k * window + c):
            // (value - min) mod window = ((k * window) mod window + c mod window) mod window
            // (value - min) mod window = (0                       + c) mod window
            // (value - min) mod window = c
            // therefore:
            // min + ((value - min) mod window) = min + c
            // or:
            // (value - min) mod (max - min) + min = min + c
            // QED
            return (value - min) % (max - min) + min;
        }

        public static int ModIntoRange(this int value, int min, int max)
            => (value - min) % (max - min) + min;
    }

    public static class CollectionExtensions
    {
        // gets the last item of the list (or errs trying)
        public static T Last<T>(this T[] array)
            => array.FromEnd(0);

        public static int LastIndex<T>(this T[] array)
            => System.Math.Max(0, array.Length - 1);

        public static T FromEnd<T>(this T[] array, int i)
            => array[array.LastIndex() - i];

        // equivalent to array[^(i + 1)] = value indexing in later versions of C#
        public static void SetFromEnd<T>(this T[] array, int i, T value)
            => array[array.LastIndex() - i] = value;

        public static void OptionalAdd<T>(this List<T> list, T? value)
            where T : struct
        {
            if (value.HasValue)
            {
                list.Add(value.Value);
            }
        }

        public static string AsArrayString<T>(this IEnumerable<T> values)
            => $"[{string.Join(", ", values.ToArray())}]";

        public static bool IsEmpty<T>(this List<T> list)
            => list.Count == 0;
            
        public static V GetOrDefault<K, V>(this Dictionary<K, V> dictionary, K key, V def)
        {
            dictionary.TryGetValue(key, out def);
            return def;
        }

        public static V ModifyWithDefault<K, V>(this Dictionary<K, V> dictionary, K key, V def, Func<V, V> Mapper)
            => dictionary[key] = Mapper(dictionary.GetOrDefault(key, def));
    }

    public static class StringExtensions
    {
        public static string Intercalate(this string[] strings, string inner)
            => string.Join(inner, strings);

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