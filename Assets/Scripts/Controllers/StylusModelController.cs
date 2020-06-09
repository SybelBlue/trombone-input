using UnityEngine;

#pragma warning disable 649
public class StylusModelController : MonoBehaviour
{
    [SerializeField]
    private GameObject potentiometerIndicator, frontButtonIndicator, backButtonIndicator;

    [SerializeField]
    private Vector3 min, max;

    public float normalizedX { get; private set; }
    public float normalizedZ { get; private set; }

    public float? normalizedSlider
    {
        get
        {
            if (potentiometerIndicator.activeInHierarchy)
            {
                return Mathf.InverseLerp(min.y, max.y, potentiometerIndicator.transform.localPosition.y);
            }
            return null;
        }

        set
        {
            if (!value.HasValue)
            {
                potentiometerIndicator.SetActive(false);
                return;
            }

            if (!potentiometerIndicator.activeInHierarchy)
            {
                potentiometerIndicator.SetActive(true);
            }

            var pos = potentiometerIndicator.transform.localPosition;
            pos.y = Mathf.Lerp(min.y, max.y, value.Value);
            potentiometerIndicator.transform.localPosition = pos;
        }
    }

    public bool frontButtonDown
    {
        get => frontButtonIndicator.activeInHierarchy;
        set => frontButtonIndicator.SetActive(value);
    }

    public bool backButtonDown
    {
        get => backButtonIndicator.activeInHierarchy;
        set => backButtonIndicator.SetActive(value);
    }

    private void Start()
    {
        normalizedSlider = null;
        frontButtonDown = false;
        backButtonDown = false;
    }

    void Update()
    {
        Vector3 euler = transform.rotation.eulerAngles;
        var x = Utils.ModIntoRange(euler.x, -180, 180);
        var z = Utils.ModIntoRange(euler.z, -180, 180);
        normalizedX = Mathf.InverseLerp(min.x, max.x, x);
        normalizedZ = Mathf.InverseLerp(min.z, max.z, z);
    }
}
