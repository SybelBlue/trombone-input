using UnityEngine;
using UnityEngine.UI;

namespace Controller
{
    public class StylusIndicator : MonoBehaviour
    {
        public Slider xSlider, ySlider, zSlider;

        public Text display;

        public Stylus stylus;

        void Update()
        {
            if (stylus == null || xSlider == null || ySlider == null || zSlider == null || display == null) return;
            if (!stylus.transform.hasChanged) return;

            xSlider.value = stylus.normalizedAngles.x;
            ySlider.value = stylus.normalizedAngles.y;
            zSlider.value = stylus.normalizedAngles.z;

            display.text = "";
            for (int i = 0; i < 3; i++)
            {
                display.text +=
                    string.Format("{0}: [{1,3:N0},{2,3:N0}] {3,3:N0} => {4,5:N3}\n",
                        (char)('x' + i),
                        stylus.LowerBound(i),
                        stylus.UpperBound(i),
                        stylus.eulerAngles[i],
                        stylus.normalizedAngles[i]);
            }

            display.text += string.Format("Slider Unfiltered: {0, 3:N3}  ", stylus.rawSlider);
            display.text += string.Format("Slider Normalized: {0, 3:N3}\n", stylus.normalizedSlider);


            display.text +=
                string.Format("travel (pos: [{0,3:N3}, {1,3:N3}, {2,3:N3}], rot: [{0,3:N3}, {1,3:N3}, {2,3:N3}])\n",
                    stylus.travel.pos[0],
                    stylus.travel.pos[1],
                    stylus.travel.pos[2],
                    stylus.travel.rot[0],
                    stylus.travel.rot[1],
                    stylus.travel.rot[2]);
        }
    }
}
