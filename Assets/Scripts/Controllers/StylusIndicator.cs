public class StylusIndicator : UnityEngine.MonoBehaviour
{
    public UnityEngine.UI.Slider xSlider, zSlider;

    public StylusModelController modelController;

    void Update()
    {
        xSlider.value = modelController.normalizedAngles.x;
        zSlider.value = modelController.normalizedAngles.z;
    }
}
