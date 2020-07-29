// Written by Zahara M. Spilka
// Date Created: 07/01/2020
// Date Last Updated: 07/29/2020
// TODO:CHECK TO SEE IF WE USE THIS ANYWERE IN THE PROGRAM
﻿using UnityEngine;

namespace SceneSwitching
{
    public class ButtonDisappear : MonoBehaviour
    {
        GameObject button;

        // This function is called when XYZ happens (I'm not sure if we actually
        // use this function at all).
        // When this happens, the function checks to see if the start button is active or not.
        // If it is active, the fucntion deactivates it, hiding it from the
        // user.

        public void ButtonClick()
        {
            if (!button)
            {
                // this can be an expensive call (LC)
                button = GameObject.FindWithTag("ButtonBackgroundTag");
            }
            //.. that can fail to find anything (LC)
            if (button && button.activeInHierarchy)
            {
                button.SetActive(false);
            }
        }
    }
}
