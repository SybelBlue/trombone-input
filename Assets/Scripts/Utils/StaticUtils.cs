using UnityEngine;

public static class Utils
{
    public static float ModIntoRange(float value, float min, float max)
    {
        float window = max - min;
        if (min > value)
        {
            value += Mathf.FloorToInt((min - value) / window) * window + window;
        }
        return (value - min) % window + min;
    }

    public static string DisplayKeyData(CustomInput.LayoutKey item)
        => item?.label ?? "<not found>";

    public static int NormalizedIntoIndex(float normalized, int length)
        => Mathf.FloorToInt(Mathf.LerpUnclamped(0, Mathf.Max(0, length - 1), normalized));

    public static T NormalizedIndex<T>(this T[] array, float normalized)
        => array[NormalizedIntoIndex(normalized, array.Length)];

    public static T GetNormalized<T>(this System.Collections.Generic.List<T> list, float normalized)
        => list[NormalizedIntoIndex(normalized, list.Count)];

    public static T Last<T>(this T[] array)
        => array[Mathf.Max(0, array.Length - 1)];

    public static System.IO.MemoryStream StreamFromTextAsset(TextAsset asset)
        => new System.IO.MemoryStream(asset.bytes);
}