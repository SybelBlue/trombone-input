using Controller;  // for Stylus
using UnityEngine;

namespace Extracted
{
    public class ExtMain : MonoBehaviour
    {
        public Stylus stylus;
        public ExtTiltType tiltType;

        private void Start()
        {
            // Capture MinVR events, requires VRMain to be attached and Awake in scene.
            MinVR.VRMain.Instance.AddOnVRButtonDownCallback("BlueStylusFrontBtn", OnFrontButtonDown);
            MinVR.VRMain.Instance.AddOnVRButtonDownCallback("BlueStylusBackBtn", OnBackButtonDown);
        }

        // Update key highlighting on the layout
        private void Update()
        {
            tiltType.UpdateHighlight(stylus.transform.forward);

            CaptureEmulatedEvents();
        }

        // Print the keypress
        public void OnFrontButtonDown()
            => Debug.Log($"Keypress {tiltType.GetSelectedLetter(stylus.transform.forward)}");
        
        // toggle alternate
        public void OnBackButtonDown()
            => tiltType.useAlternate = !tiltType.useAlternate;

        // not necessary for use of keyboard
        private void CaptureEmulatedEvents()
        {
            if (Input.GetKeyDown(KeyCode.Return)) OnFrontButtonDown();
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) OnBackButtonDown();
        }
    }
}