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

    public static string DisplayKeyData(CustomInput.LayoutKey item) => item?.data ?? "<not found>";

    public static int NormalizedAsIndex(float normalized, int length)
        => Mathf.FloorToInt(Mathf.LerpUnclamped(0, length - 1, normalized));

    public static int NormalizedIndex(this System.Object[] array, float normalized)
        => NormalizedAsIndex(normalized, array.Length);
}