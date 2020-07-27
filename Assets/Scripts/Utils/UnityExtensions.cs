using UnityEngine;

namespace Utils.UnityExtensions
{
    public static class VectorExtensions
    {
        // returns the mapping of all values of vec under f as a new Vector3
        public static Vector3 Map(this Vector3 vec, System.Func<float, float> f)
            => new Vector3(f(vec.x), f(vec.y), f(vec.z));

        // returns the mapping of all indexed values of vec under f as a new Vector3
        public static Vector3 Map(this Vector3 vec, System.Func<int, float, float> f)
            => new Vector3(f(0, vec[0]), f(1, vec[1]), f(2, vec[2]));

        // returns a copy of vec with z-component z
        public static Vector3 WithZ(this Vector2 vec, float z)
            => new Vector3(vec.x, vec.y, z);

        // returns a copy of vec with z-component z
        public static Vector3 WithZ(this Vector3 vec, float z)
            => new Vector3(vec.x, vec.y, z);

        // returns a copy of vec with y-component y
        public static Vector3 WithY(this Vector3 vec, float y)
            => new Vector3(vec.x, y, vec.z);

        // keeps the ith value of vec if axisMask & (1 << i)
        public static Vector3 ProjectTo(this Vector3 vec, int axisMask)
            // if axis mask has a 1 in the ith place, keep ith value, else 0
            => vec.Map((i, v) => ((axisMask >> i) & 0x1) * v);

        // flattens the ith component of vec
        public static Vector3 Flatten(this Vector3 vec, int axis)
            => vec.ProjectTo(~(1 << axis));

        // returns yaml representation of vec
        public static string AsYaml(this Vector3 vec)
            => $"[{vec.x}, {vec.y}, {vec.z}]";
    }

    public static class IOExtensions
    {
        // dereferences the bytes of the asset and creates a stream
        public static System.IO.MemoryStream IntoMemoryStream(this TextAsset asset)
            => new System.IO.MemoryStream(asset.bytes);
    }
}