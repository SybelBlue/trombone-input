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


    public static float SignedAngle(Vector3 forward, Vector3 target, Vector3 axis)
    {
        // Vector3 projectedToXYPlane = new Vector3(modelController.transform.forward.x, modelController.transform.forward.y, 0);
        float dot = Vector3.Dot(forward.normalized, target.normalized);
        float degrees = Mathf.Rad2Deg * Mathf.Acos(dot);

        Vector3 derivedAxis = Vector3.Cross(forward, target);
        float sign = Mathf.Sign(Vector3.Dot(axis, derivedAxis));

        return sign * degrees;
    }

    public static float SignedAngleFromAxis(Vector3 forward, Vector3 target, int axis)
        => Utils.SignedAngle(forward.Flatten(axis), target, VectorFrom(i => i == axis ? 1 : 0));

    public static Vector3 VectorFrom(System.Func<int, float> f)
        => new Vector3(f(0), f(1), f(2));
}

public static class Extensions
{
    #region Arrays

    // gets the last item of the list (or errs trying)
    public static T Last<T>(this T[] array)
        => array.FromEnd(0);

    public static T FromEnd<T>(this T[] array, int i)
        => array[Mathf.Max(0, array.Length - 1) - i];

    public static void SetFromEnd<T>(this T[] array, int i, T value)
        => array[Mathf.Max(0, array.Length - 1) - i] = value;

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
}