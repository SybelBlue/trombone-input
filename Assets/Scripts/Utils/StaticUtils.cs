using UnityEngine;

public static class Utils
{
    // returns the value in the range [min, max) via modular arithmetic
    public static float ModIntoRange(float value, float min, float max)
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

    // Takes float normalized in [0.0, 1.0] and maps it to an index where 0.0 is 0 and the 1.0 is the length
    public static int NormalizedIntoIndex(float normalized, int length)
        => Mathf.FloorToInt(Mathf.LerpUnclamped(0, Mathf.Max(0, length - 1), normalized));

    // gets the last item of the list (or errs trying)
    public static T Last<T>(this T[] array)
        => array.FromEnd(0);

    public static T FromEnd<T>(this T[] array, int i)
        => array[Mathf.Max(0, array.Length - 1) - i];

    public static void SetFromEnd<T>(this T[] array, int i, T value)
        => array[Mathf.Max(0, array.Length - 1) - i] = value;

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

    public static Vector3 Map(this Vector3 vec, System.Func<float, float> f)
        => new Vector3(f(vec.x), f(vec.y), f(vec.z));

    public static Vector3 Map(this Vector3 vec, System.Func<int, float, float> f)
        => new Vector3(f(0, vec[0]), f(1, vec[1]), f(2, vec[2]));

    public static Vector3 WithZ(this Vector2 vec, float z)
        => new Vector3(vec.x, vec.y, z);

    public static System.IO.MemoryStream StreamFromTextAsset(TextAsset asset)
        => new System.IO.MemoryStream(asset.bytes);
}