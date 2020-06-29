using UnityEngine;
using UnityEngine.UI;

public class StylusIndicator : MonoBehaviour
{
    public Slider xSlider, ySlider, zSlider;

    public Text display;

    public StylusModelController modelController;

    void Update()
    {
        if (modelController == null || xSlider == null || ySlider == null || zSlider == null || display == null) return;
        if (!modelController.transform.hasChanged) return;

        xSlider.value = modelController.normalizedAngles.x;
        ySlider.value = modelController.normalizedAngles.y;
        zSlider.value = modelController.normalizedAngles.z;

        display.text = "";
        for (int i = 0; i < 3; i++)
        {
            display.text +=
                string.Format("{0}: [{1,3:N0},{2,3:N0}] {3,3:N0} => {4,5:N3}\n",
                    (char)('x' + i),
                    modelController.LowerBound(i),
                    modelController.UpperBound(i),
                    modelController.eulerAngles[i],
                    modelController.normalizedAngles[i]);
        }
    }
}
