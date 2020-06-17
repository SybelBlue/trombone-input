using UnityEngine;

#pragma warning disable 649
public class StylusModelController : MonoBehaviour
{
    [SerializeField]
    private GameObject potentiometerIndicator, laserPointer;

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
        get => laserPointer.activeInHierarchy;
        set => laserPointer.SetActive(value);
    }

    public Vector3 origin { get; private set; }

    public Vector3 direction { get; private set; }

    public (Vector3 origin, Vector3 direction) orientation
        => (transform.position, transform.forward);

    public Vector3 normalizedAngles { get; private set; }


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

    void Update()
    {
        normalizedAngles =
            transform
            .rotation
            .eulerAngles
            .Map(x => Utils.ModIntoRange(x, -180, 180))
            .Map((i, x) => Mathf.InverseLerp(minAngle[i], maxAngle[i], x));
    }
}
