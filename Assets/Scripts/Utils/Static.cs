using UnityEngine;
using UnityExtensions;

namespace Utils
{
    public static class Static
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
            => SignedAngle(forward.Flatten(axis), target, VectorFrom(i => i == axis ? 1 : 0));

        public static Vector3 VectorFrom(System.Func<int, float> f)
            => new Vector3(f(0), f(1), f(2));

        public static T FindTaggedComponent<T>(string name) 
            where T : Component
            => GameObject.FindGameObjectWithTag(name)?.GetComponent<T>();

        // returns true if the obj reference has been changed
        public static bool FillWithTaggedIfNull<T>(ref T obj, string name) 
            where T : Component
            => obj ? false : (obj = FindTaggedComponent<T>(name));

    }
}