using UnityEngine;

#pragma warning disable 649
public class StylusModelController : MonoBehaviour
{
    [SerializeField]
    private GameObject potentiometerIndicator;

    [SerializeField]
    private MeshRenderer frontButtonRenderer, backButtonRenderer;

    [SerializeField]
    private Material highlightMaterial, defaultMaterial;

    [SerializeField]
    private Vector3 minAngle, maxAngle;

    [SerializeField]
    private Vector2 sliderBounds;

    public Vector3 origin { get; private set; }

    public Vector3 direction { get; private set; }

    public (Vector3 origin, Vector3 direction) orientation => (origin, direction);

    public float normalizedX { get; private set; }
    public float normalizedY { get; private set; }
    public float normalizedZ { get; private set; }

    private bool highlightingFront, highlightingBack;

    public float? normalizedSlider
    {
        get
        {
            if (potentiometerIndicator.activeInHierarchy)
            {
                return Mathf.InverseLerp(sliderBounds.x, sliderBounds.y, potentiometerIndicator.transform.localPosition.z);
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
            pos.z = Mathf.Lerp(sliderBounds.x, sliderBounds.y, value.Value);
            potentiometerIndicator.transform.localPosition = pos;
        }
    }

    public bool frontButtonDown
    {
        get => highlightingFront;
        set
        {
            highlightingFront = value;
            frontButtonRenderer.material =
                value ?
                    highlightMaterial :
                    defaultMaterial;
        }
    }

    public bool backButtonDown
    {
        get => highlightingBack;
        set
        {
            highlightingBack = value;
            backButtonRenderer.material =
                value ?
                    highlightMaterial :
                    defaultMaterial;
        }
    }

    private void Start()
    {
        normalizedSlider = null;
        frontButtonDown = false;
        backButtonDown = false;

        UpdateOrientation();
    }

    void Update()
    {
        Vector3 euler = transform.rotation.eulerAngles;
        var x = Utils.ModIntoRange(euler.x, -180, 180);
        var y = Utils.ModIntoRange(euler.y, -180, 180);
        var z = Utils.ModIntoRange(euler.z, -180, 180);
        normalizedX = Mathf.InverseLerp(minAngle.x, maxAngle.x, x);
        normalizedY = Mathf.InverseLerp(minAngle.y, maxAngle.y, y);
        normalizedZ = Mathf.InverseLerp(minAngle.z, maxAngle.z, z);

        UpdateOrientation();
    }

    private void UpdateOrientation()
    {
        direction = transform.rotation * Vector3.forward;
        origin = transform.position;
    }
}
