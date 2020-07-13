using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils.UnityExtensions
{
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

        public static string AsYaml(this Vector3 vec)
            => $"[{vec.x}, {vec.y}, {vec.z}]";

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

    public static class SceneLoadingExtensions
    {
        public static AsyncOperation UnloadAsync(this string name)
            => SceneManager.UnloadSceneAsync(name);

        public static void LoadAdditive(this string name)
            => SceneManager.LoadScene(name, LoadSceneMode.Additive);
    }
}