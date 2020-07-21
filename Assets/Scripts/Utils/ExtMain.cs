using Controller;
using UnityEngine;

namespace Extracted
{
    public class ExtMain : MonoBehaviour
    {
        public Stylus stylus;
        public ExtTiltType tiltType;
        // Update is called once per frame
        void Update()
        { 
            tiltType.UpdateHighlight(stylus.transform.forward);

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                tiltType.useAlternate = !tiltType.useAlternate;
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log($"Keypress {tiltType.GetSelectedLetter(stylus.transform.forward)}");
            }
        }
    }
}