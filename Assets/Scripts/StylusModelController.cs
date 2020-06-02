using UnityEngine;

public class StylusModelController : MonoBehaviour
{
    [SerializeField]
    private GameObject indicatorSphere;

    [SerializeField]
    private float ymin;

    [SerializeField]
    private float ymax;

    public float? normalizedValue
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
                Debug.Log("valueless");
                indicatorSphere.SetActive(false);
            }
            else
            {
                if (!indicatorSphere.activeInHierarchy)
                {
                    indicatorSphere.SetActive(true);
                }

                var pos = indicatorSphere.transform.localPosition;
                pos.y = Mathf.Lerp(ymin, ymax, value.Value);
                indicatorSphere.transform.localPosition = pos;
            }
        }
    }

    private void Start()
        => normalizedValue = null;
}
