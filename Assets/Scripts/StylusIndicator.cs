using UnityEngine;
using UnityEngine.UI;

public class StylusIndicator : MonoBehaviour
{
    public Slider xSlider, zSlider;


    public Vector2Int xEulerBounds, zEulerBounds;

    public (int, int, bool) bound1;

    public GameObject stylus;


    // Update is called once per frame
    void Update()
    {
        Vector3 euler = stylus.transform.rotation.eulerAngles;
        var x = ModIntoRange(euler.x, -180, 180);
        var z = ModIntoRange(euler.z, -180, 180);
        xSlider.value = Mathf.InverseLerp(xEulerBounds.x, xEulerBounds.y, x);
        zSlider.value = Mathf.InverseLerp(zEulerBounds.x, zEulerBounds.y, z);
    }

    private static float ModIntoRange(float value, float min, float max)
    {
        float window = max - min;
        if (min > value)
        {
            value += Mathf.FloorToInt((min - value) / window) * window + window;
        }
        return (value - min) % window + min;
    }
}
