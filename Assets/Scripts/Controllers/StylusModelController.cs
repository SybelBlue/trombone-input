using UnityEngine;

#pragma warning disable 649
public class StylusModelController : MonoBehaviour
{
    #region EditorSet
    public bool recordSliderData;
    public bool useUnityEulerAngles;

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
    #endregion

    private (int? frame, RaycastHit? hit, IRaycastable obj) lastFound;
    private (bool front, bool back) highlighting;
    private (string path, float? last) saveData = (null, null);


    #region Properties
    public bool useLaser
    {
        get => laserPointer.active;
        set => laserPointer.active = value;
    }

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

            // Utils.SignedAngle(transform.forward.ProjectTo(false, true, true), new Vector3(0, 1, 0), new Vector3(1, 0, 0)),
            // Utils.SignedAngle(transform.forward.ProjectTo(true, false, true), new Vector3(0, 0, 1), new Vector3(0, 1, 0)),
            // Utils.SignedAngle(transform.forward.ProjectTo(true, true, false), new Vector3(0, 1, 0), new Vector3(0, 0, 1))

            Utils.SignedAngleFromAxis(transform.forward, new Vector3(0, 1, 0), 0),
            Utils.SignedAngleFromAxis(transform.forward, new Vector3(0, 0, 1), 1),
            Utils.SignedAngleFromAxis(transform.forward, new Vector3(0, 1, 0), 2)
            );

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
        get => highlighting.front;
        set
        {
            highlighting.front = value;
            frontButtonRenderer.material =
                value ?
                    highlightMaterial :
                    defaultMaterial;
        }
    }

    public bool backButtonDown
    {
        get => highlighting.back;
        set
        {
            highlighting.back = value;
            backButtonRenderer.material =
                value ?
                    highlightMaterial :
                    defaultMaterial;
        }
    }
    #endregion

    private void Start()
    {
        if (!recordSliderData) return;

        saveData = ($"{Application.persistentDataPath}/{Testing.Utils.UniqueYamlName("stylus-output")}", null);
    }

    private void Update()
    {
        if (!transform.hasChanged) return;

        normalizedAngles =
            eulerAngles
            .Map(x => useUnityEulerAngles ? Utils.ModIntoRange(x, -180, 180) : x)
            .Map((i, x) => Mathf.InverseLerp(LowerBound(i), UpperBound(i), x));

        if (!recordSliderData) return;

        if (saveData.last == normalizedSlider) return;

        Testing.Utils.UsingStream(saveData.path, WriteSliderData);

        saveData.last = normalizedSlider;
    }

    private void OnDestroy()
    {
        if (recordSliderData)
        {
            Debug.LogWarning($"Saved data to:\n{saveData.path}");
        }
        else if (saveData.path != null)
        {
            System.IO.File.Delete(saveData.path);
        }
    }

    public float LowerBound(int axis)
        => axis == 0 && !CustomInput.Bindings.LEFT_HANDED ? maxAngle[axis] : minAngle[axis];

    public float UpperBound(int axis)
        => axis == 0 && !CustomInput.Bindings.LEFT_HANDED ? minAngle[axis] : maxAngle[axis];

    public IRaycastable Raycast(out RaycastHit? hit)
    {
        if (lastFound.frame == Time.frameCount)
        {
            hit = lastFound.hit;
            return lastFound.obj;
        }

        foreach (RaycastHit h in Physics.RaycastAll(transform.position, transform.forward, Mathf.Infinity))
        {
            IRaycastable r = h.transform.gameObject.GetComponent<IRaycastable>();
            if (r)
            {
                lastFound = (Time.frameCount, h, r);
                hit = h;
                return r;
            }
            else
            {
                Debug.DrawLine(transform.position, h.point, Color.red, 0.2f);
            }
        }

        lastFound = (Time.frameCount, null, null);
        hit = null;
        return null;
    }

    private void WriteSliderData(System.IO.StreamWriter writer)
        => writer.WriteLine($"{Time.time}: {normalizedSlider}");
}
