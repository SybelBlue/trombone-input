using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
#pragma warning disable 649
    public class Pulser : MonoBehaviour
    {
        public Color flashColor;
        public float flashDuration;

        [SerializeField]
        private Image panel;

        private int? lastFlash;

        private Color restoreColor;

        public void Flash()
        {
            if (lastFlash == null)
            {
                restoreColor = panel.color;
            }

            lastFlash = Time.frameCount;
        }

        private void Update()
        {
            if (!lastFlash.HasValue) return;

            // intercepts [-dur, dur], 1 @ t=0 => alpha = 1 - dur^{-2} t^2
            var t = (Time.frameCount - lastFlash.Value) / flashDuration;
            t = Mathf.Min(t, 1);

            panel.color = Color.Lerp(restoreColor, flashColor, 1.0f - (t * t));

            if (t == 1) lastFlash = null;
        }
    }
}