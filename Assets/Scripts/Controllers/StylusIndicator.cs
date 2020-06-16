public class StylusIndicator : UnityEngine.MonoBehaviour
{
    public UnityEngine.UI.Slider xSlider, zSlider;

    public StylusModelController modelController;

    void Update()
    {
        xSlider.value = modelController.normalizedX;
        zSlider.value = modelController.normalizedY;
    }
}
