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

    // gets the item normalized * 100% of the way into the array
    public static T NormalizedIndex<T>(this T[] array, float normalized)
        => array[NormalizedIntoIndex(normalized, array.Length)];

    // gets the item normalized * 100% of the way into the list
    public static T GetNormalized<T>(this System.Collections.Generic.List<T> list, float normalized)
        => list[NormalizedIntoIndex(normalized, list.Count)];

    // gets the last item of the list (or errs trying)
    public static T Last<T>(this T[] array)
        => array[Mathf.Max(0, array.Length - 1)];

    public static System.IO.MemoryStream StreamFromTextAsset(TextAsset asset)
        => new System.IO.MemoryStream(asset.bytes);
}