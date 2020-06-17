using UnityEngine;

#pragma warning disable 649
public class StylusModelController : MonoBehaviour
{
    [SerializeField]
    private GameObject potentiometerIndicator;

    [SerializeField]
    private MeshRenderer frontButtonRenderer, backButtonRenderer;

    private bool highlightingFront, highlightingBack;

    [SerializeField]
    private Material highlightMaterial, defaultMaterial;

    [SerializeField]
    private Vector3 min, max;

    public Vector3 origin { get; private set; }

    public Vector3 direction { get; private set; }

    public (Vector3 origin, Vector3 direction) orientation => (origin, direction);

    public float normalizedX { get; private set; }
    public float normalizedY { get; private set; }

    public float? normalizedSlider
    {
        get
        {
            if (potentiometerIndicator.activeInHierarchy)
            {
                return Mathf.InverseLerp(min.z, max.z, potentiometerIndicator.transform.localPosition.z);
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
            pos.z = Mathf.Lerp(min.z, max.z, value.Value);
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
        normalizedX = Mathf.InverseLerp(min.x, max.x, x);
        normalizedY = Mathf.InverseLerp(min.y, max.y, y);

        UpdateOrientation();
    }

    private void UpdateOrientation()
    {
        direction = transform.rotation * Vector3.forward;
        origin = transform.position;
        Debug.DrawRay(orientation.origin, orientation.direction, Color.cyan, 0.5f);
    }

    public CustomInput.InputData PackageData(string context, int? lastReportedValue)
        => new CustomInput.InputData(
                context,
                lastReportedValue,
                normalizedX,
                normalizedY,
                normalizedSlider,
                frontButtonDown,
                backButtonDown,
                orientation
            );
}
