using Controller;
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
            => tiltType.UpdateHighlight(stylus.transform.forward);

        // Print the keypress
        public void OnFrontButtonDown()
            => Debug.Log($"Keypress {tiltType.GetSelectedLetter(stylus.transform.forward)}");
        
        // toggle alternate
        public void OnBackButtonDown()
            => tiltType.useAlternate = !tiltType.useAlternate;
    }
}