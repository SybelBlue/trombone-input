using System.Collections;
using UnityEngine;

namespace SceneSwitching
{
    public class ButtonReappear : MonoBehaviour
    {
        // public Button button;
        GameObject buttonBack;
        public Camera skybox;

        public void ButtonReClick()
        {
            var newColor = new Color(184, 158, 195, 0.0f);
            // button = GetComponent<Button>();
            // button = GameObject.FindWithTag("StartButtonTag").GetComponent<Button>();
            // button = GameObject.FindWithTag("StartButtonTag");
            // button = GameObject.FindWithTag("StartButtonTag").GetComponent<Button>();
            buttonBack = GameObject.FindWithTag("StartButtonTag");
            // buttonBack.GetComponent<Renderer>().enabled = true;
            // MeshRenderer mr = buttonBack.GetComponent<MeshRenderer>();
            // mr.enabled = true;
            // buttonBack.gameObject.SetActive(true);
            // buttonBack.GetComponent<Renderer>().enabled = true;

            // skybox = GameObject.FindWithTag("MainCamera");
            skybox = GetComponent<Camera>();
            skybox = Camera.main;
            skybox.clearFlags = CameraClearFlags.Skybox;
            skybox.backgroundColor = newColor;

            //TODO: Set skybox to EBFFFF
            // skybox = GameObject.FindWithTag("MainCamera");
            // skybox.SetBackgroundColor("EBFFFF");
        }
        // // Start is called before the first frame update
        // void Start()
        // {
        //
        // }
        //
        // // Update is called once per frame
        // void Update()
        // {
        //
        // }
    }
}
