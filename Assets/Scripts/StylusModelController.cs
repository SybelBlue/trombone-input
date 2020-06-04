using UnityEngine;

public class StylusModelController : MonoBehaviour
{
    [SerializeField]
    private GameObject indicatorSphere;

    [SerializeField]
    private float ymin;

    [SerializeField]
    private float ymax;

    public float xMax, xMin, zMin, zMax;

    public float normalizedX { get; private set; }
    public float normalizedZ { get; private set; }

    public float? normalizedSlider
    {
        get
        {
            if (indicatorSphere.activeInHierarchy)
            {
                return Mathf.InverseLerp(ymin, ymax, indicatorSphere.transform.position.y);
            }
            return null;
        }

        set
        {
            if (!value.HasValue)
            {
                indicatorSphere.SetActive(false);
                return;
            }

            if (!indicatorSphere.activeInHierarchy)
            {
                indicatorSphere.SetActive(true);
            }

            var pos = indicatorSphere.transform.localPosition;
            pos.y = Mathf.Lerp(ymin, ymax, value.Value);
            indicatorSphere.transform.localPosition = pos;
        }
    }

    private void Start()
        => normalizedSlider = null;

    void Update()
    {
        Vector3 euler = transform.rotation.eulerAngles;
        var x = Utils.ModIntoRange(euler.x, -180, 180);
        var z = Utils.ModIntoRange(euler.z, -180, 180);
        normalizedX = Mathf.InverseLerp(xMin, xMax, x);
        normalizedZ = Mathf.InverseLerp(zMin, zMax, z);
    }
}
