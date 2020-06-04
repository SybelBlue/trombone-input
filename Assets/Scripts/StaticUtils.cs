public static class Utils
{
    public static float ModIntoRange(float value, float min, float max)
    {
        float window = max - min;
        if (min > value)
        {
            value += UnityEngine.Mathf.FloorToInt((min - value) / window) * window + window;
        }
        return (value - min) % window + min;
    }

    public static string DisplayKeyData(CustomInput.LayoutKey item) => item?.data ?? "<not found>";
}