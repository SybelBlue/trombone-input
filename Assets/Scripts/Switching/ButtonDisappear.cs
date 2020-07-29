using UnityEngine;

namespace SceneSwitching
{
    public class ButtonDisappear : MonoBehaviour
    {
        GameObject button;

        public void ButtonClick()
        {
          // public GameObject button;
            if (!button)
            {
                // this can be an expensive call (LC)
                button = GameObject.FindWithTag("ButtonBackgroundTag");
            }
            //.. that can fail to find anything
            if (button && button.activeInHierarchy)
            {
                button.SetActive(false);
                Debug.LogWarning("Gall worked");
            }
        }
    }
}
