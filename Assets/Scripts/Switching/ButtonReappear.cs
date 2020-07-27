using System.Collections;
using UnityEngine;

namespace SceneSwitching
{
    public class ButtonReappear : MonoBehaviour
    {
        GameObject buttonBack;

        public void ButtonReClick()
        {
          buttonBack = GameObject.FindWithTag("ButtonBackgroundTag");
          buttonBack.SetActive(true);

        }
    }
}
