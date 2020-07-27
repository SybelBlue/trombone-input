using UnityEngine;

namespace SceneSwitching
{
    public class ButtonReappear : MonoBehaviour
    {
        GameObject buttonBack;

        public void ButtonReClick()
        {
            if (!buttonBack)
            {
                // can be an expensive call (LC)
                buttonBack = GameObject.FindWithTag("ButtonBackgroundTag");
            }
            // ... that can fail to find anything
            if (buttonBack && buttonBack.activeInHierarchy)
            {
                buttonBack.SetActive(true);
            }
        }
    }
}
