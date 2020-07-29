// Written by Zahara M. Spilka
// Date Created: 07/01/2020
// Date Last Updated: 07/29/2020
﻿using UnityEngine;

namespace SceneSwitching
{
    public class ButtonReappear : MonoBehaviour
    {
        GameObject buttonBack;

        // This function is called whent he back to lobby button is invoked.
        // When this happens, the function checks to see if the start button is active or not.
        // If it is not active, the fucntion reactivates it, making it visable
        // to the user.

        public void ButtonReClick()
        {
            if (!buttonBack)
            {
                // can be an expensive call (LC)
                buttonBack = GameObject.FindWithTag("ButtonBackgroundTag");
            }
            // ... that can fail to find anything (LC)
            if (buttonBack && buttonBack.activeInHierarchy)
            {
                buttonBack.SetActive(true);
            }
        }
    }
}
