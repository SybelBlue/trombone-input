using UnityEngine;
using UnityEngine.UI;

public class StylusIndicator : MonoBehaviour
{
    public Slider xSlider, zSlider;

    public StylusModelController modelController;

    void Update()
    {
        xSlider.value = modelController.normalizedX;
        zSlider.value = modelController.normalizedZ;
    }
}
