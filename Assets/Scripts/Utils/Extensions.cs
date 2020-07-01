using UnityEngine;

namespace CustomExtensions
{
    public static class CollectionExtensions
    {
        // gets the last item of the list (or errs trying)
        public static T Last<T>(this T[] array)
            => array.FromEnd(0);

        public static T FromEnd<T>(this T[] array, int i)
            => array[Mathf.Max(0, array.Length - 1) - i];

        public static void SetFromEnd<T>(this T[] array, int i, T value)
            => array[Mathf.Max(0, array.Length - 1) - i] = value;

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
                => s.Substring(0, Mathf.Max(0, s.Length - 1));
     }

    public static class VectorExtensions
    {
        public static Vector3 Map(this Vector3 vec, System.Func<float, float> f)
            => new Vector3(f(vec.x), f(vec.y), f(vec.z));

        public static Vector3 Map(this Vector3 vec, System.Func<int, float, float> f)
            => new Vector3(f(0, vec[0]), f(1, vec[1]), f(2, vec[2]));

        public static Vector3 WithZ(this Vector2 vec, float z)
            => new Vector3(vec.x, vec.y, z);

        public static Vector3 WithZ(this Vector3 vec, float z)
            => new Vector3(vec.x, vec.y, z);
        public static Vector3 WithY(this Vector3 vec, float y)
            => new Vector3(vec.x, y, vec.z);

        public static Vector3 ProjectTo(this Vector3 vec, int axisMask)
            // if axis mask has a 1 in the ith place, keep ith value, else 0
            => vec.Map((i, v) => ((axisMask >> i) & 0x1) * v);

        public static Vector3 Flatten(this Vector3 vec, int axis)
            => vec.ProjectTo(~(1 << axis));

        #region Unused
        public static Vector3 ProjectTo(this Vector3 vec, bool x, bool y, bool z)
            => vec.ProjectTo(((x ? 1 << 0 : 0)) | (y ? 1 << 1 : 0) | (z ? 1 << 2 : 0));
        #endregion
    }

    public static class IOExtensions
    {
        public static System.IO.MemoryStream IntoMemoryStream(this TextAsset asset)
            => new System.IO.MemoryStream(asset.bytes);
    }
}