public class StylusIndicator : UnityEngine.MonoBehaviour
{
    public UnityEngine.UI.Slider xSlider, ySlider, zSlider;

    public StylusModelController modelController;

    void Update()
    {
        xSlider.value = modelController.normalizedAngles.x;
        ySlider.value = modelController.normalizedAngles.y;
        zSlider.value = modelController.normalizedAngles.z;
    }
}
