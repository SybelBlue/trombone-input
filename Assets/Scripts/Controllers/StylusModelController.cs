using UnityEngine;

#pragma warning disable 649
public class StylusModelController : MonoBehaviour
{
    public bool useUnityEulerAngles = false;

    [SerializeField]
    private GameObject potentiometerIndicator;

    [SerializeField]
    private LaserController laserPointer;

    [SerializeField]
    private MeshRenderer frontButtonRenderer, backButtonRenderer;

    [SerializeField]
    private Material highlightMaterial, defaultMaterial;

    [SerializeField]
    private Vector3 minAngle, maxAngle;

    [SerializeField]
    private Vector2 sliderBounds;

    public bool useLaser
    {
        get => laserPointer.active;
        set => laserPointer.active = value;
    }

    public Vector3 origin { get; private set; }

    public Vector3 direction { get; private set; }

    public (Vector3 origin, Vector3 direction) orientation
        => (transform.position, transform.forward);

    public Vector3 normalizedAngles { get; private set; }

    public Vector3 eulerAngles
        => useUnityEulerAngles ?
            transform.eulerAngles :
            new Vector3(
            // Vector3.SignedAngle(new Vector3(0, 1, 0), transform.forward.ProjectTo(false, true, true).normalized, new Vector3(1, 0, 0)),
            // Vector3.SignedAngle(new Vector3(0, 0, 1), transform.forward.ProjectTo(true, false, true).normalized, new Vector3(0, 1, 0)),
            // Vector3.SignedAngle(new Vector3(0, 1, 0), transform.forward.ProjectTo(true, true, false).normalized, new Vector3(0, 0, 1))

            Utils.CustomSignedAngle(transform.forward.ProjectTo(false, true, true), new Vector3(0, 1, 0), new Vector3(1, 0, 0)),
            Utils.CustomSignedAngle(transform.forward.ProjectTo(true, false, true), new Vector3(0, 0, 1), new Vector3(0, 1, 0)),
            Utils.CustomSignedAngle(transform.forward.ProjectTo(true, true, false), new Vector3(0, 1, 0), new Vector3(0, 0, 1))
            );


    private bool highlightingFront, highlightingBack;

    public float? normalizedSlider
    {
        get
        {
            if (potentiometerIndicator.activeInHierarchy)
            {
                return Mathf.InverseLerp(sliderBounds.x, sliderBounds.y, -potentiometerIndicator.transform.localPosition.z);
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
            pos.z = -Mathf.Lerp(sliderBounds.x, sliderBounds.y, value.Value);
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

    public float LowerBound(int i)
    {
        return i == 0 && !CustomInput.Bindings.LEFT_HANDED ? maxAngle[i] : minAngle[i];
    }

    public float UpperBound(int i)
    {
        return i == 0 && !CustomInput.Bindings.LEFT_HANDED ? minAngle[i] : maxAngle[i];
    }

    void Update()
    {
        normalizedAngles =
            eulerAngles
            .Map(x => useUnityEulerAngles ? Utils.ModIntoRange(x, -180, 180) : x)
            .Map((i, x) => Mathf.InverseLerp(LowerBound(i), UpperBound(i), x));
    }

    public CustomInput.InputData PackageData(string context, int? lastReportedValue)
        => new CustomInput.InputData(
                context,
                lastReportedValue,
                normalizedAngles,
                normalizedSlider,
                frontButtonDown,
                backButtonDown,
                orientation
            );

    private int lastFrame = -1;
    private (RaycastHit hit, IRaycastable obj)? lastFound;

    public IRaycastable Raycast(out RaycastHit? hit)
    {
        if (lastFrame == Time.frameCount)
        {
            hit = lastFound?.hit;
            return lastFound?.obj;
        }

        lastFrame = Time.frameCount;
        foreach (RaycastHit h in Physics.RaycastAll(transform.position, transform.forward, Mathf.Infinity))
        {
            IRaycastable r = h.transform.gameObject.GetComponent<IRaycastable>();
            if (r)
            {
                lastFound = (h, r);
                hit = h;
                return r;
            }
            else
            {
                Debug.DrawLine(transform.position, h.point, Color.red, 0.2f);
            }
        }

        lastFound = null;
        hit = null;
        return null;
    }
}
