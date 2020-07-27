using UnityEngine;
using UnityEngine.UI;

namespace SceneSwitching
{
    public class ButtonDisappear : MonoBehaviour
    {
        GameObject button;

        public void ButtonClick()
        {
            button = GameObject.FindWithTag("ButtonBackgroundTag");
            button.SetActive(false);
        }
    }
}
