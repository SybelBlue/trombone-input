using UnityEngine;

#pragma warning disable 649
public class StylusModelController : MonoBehaviour
{
    [SerializeField]
    private GameObject indicatorSphere;

    [SerializeField]
    private Vector3 min, max;

    public float normalizedX { get; private set; }
    public float normalizedZ { get; private set; }

    public float? normalizedSlider
    {
        get
        {
            if (indicatorSphere.activeInHierarchy)
            {
                return Mathf.InverseLerp(min.y, max.y, indicatorSphere.transform.localPosition.y);
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
            pos.y = Mathf.Lerp(min.y, max.y, value.Value);
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
        normalizedX = Mathf.InverseLerp(min.x, max.x, x);
        normalizedZ = Mathf.InverseLerp(min.z, max.z, z);
    }
}
