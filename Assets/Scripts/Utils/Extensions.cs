using UnityEngine;

public static class Extensions
{
    #region Collections

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

    #endregion

    #region Strings
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

    public static string Repeat(this string s, int n)
        => n > 0 ? s + s.Repeat(n - 1) : "";

    public static string Backspace(this string s)
        => s.Substring(0, Mathf.Max(0, s.Length - 1));

    #endregion

    #region Vectors

    public static Vector3 Map(this Vector3 vec, System.Func<float, float> f)
        => new Vector3(f(vec.x), f(vec.y), f(vec.z));

    public static Vector3 Map(this Vector3 vec, System.Func<int, float, float> f)
        => new Vector3(f(0, vec[0]), f(1, vec[1]), f(2, vec[2]));

    public static Vector3 WithZ(this Vector2 vec, float z)
        => new Vector3(vec.x, vec.y, z);

    public static Vector3 ProjectTo(this Vector3 vec, bool x, bool y, bool z)
        => vec.ProjectTo(((x ? 1 << 0 : 0)) | (y ? 1 << 1 : 0) | (z ? 1 << 2 : 0));

    public static Vector3 ProjectTo(this Vector3 vec, int axisMask)
        // if axis mask has a 1 in the ith place, keep ith value, else 0
        => vec.Map((i, v) => ((axisMask >> i) & 0x1) * v);

    public static Vector3 Flatten(this Vector3 vec, int axis)
        => vec.ProjectTo(~(1 << axis));

    #endregion

    public static System.IO.MemoryStream IntoMemoryStream(this TextAsset asset)
        => new System.IO.MemoryStream(asset.bytes);

    public static void AddVRButtonCallbacks(
        this MinVR.VRMain instance,
        string eventName,
        MinVR.VRMain.OnVRButtonUpEventDelegate onButtonUp,
        MinVR.VRMain.OnVRButtonDownEventDelegate onButtonDown)
    {
        instance.AddOnVRButtonUpCallback(eventName, onButtonUp);
        instance.AddOnVRButtonDownCallback(eventName, onButtonDown);
    }
}