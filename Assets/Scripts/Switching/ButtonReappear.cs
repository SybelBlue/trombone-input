using System.Collections;
using UnityEngine;

namespace SceneSwitching
{
    public class ButtonReappear : MonoBehaviour
    {
        // public Button button;
        GameObject buttonBack;
        public Camera skybox;

        // public void Awake()
        // {
        //   skybox = GameObject.Find("MainCamera");
        // }

        public void ButtonReClick()
        {
            var newColor = new Color(0.184f, 0.158f, 0.195f, 1.0f);
            // // button = GetComponent<Button>();
            // // button = GameObject.FindWithTag("StartButtonTag").GetComponent<Button>();
            // // button = GameObject.FindWithTag("StartButtonTag");
            // // button = GameObject.FindWithTag("StartButtonTag").GetComponent<Button>();
            // buttonBack = GameObject.FindWithTag("StartButtonTag");
            // // buttonBack.GetComponent<Renderer>().enabled = true;
            // // MeshRenderer mr = buttonBack.GetComponent<MeshRenderer>();
            // // mr.enabled = true;
            // // buttonBack.gameObject.SetActive(true);
            // // buttonBack.GetComponent<Renderer>().enabled = true;
            //
            // // skybox = GameObject.FindWithTag("MainCamera");
            skybox = GetComponent<Camera>();
            skybox = Camera.main;
            skybox.clearFlags = CameraClearFlags.Skybox;
            skybox.backgroundColor = newColor;
        }
    }
}
