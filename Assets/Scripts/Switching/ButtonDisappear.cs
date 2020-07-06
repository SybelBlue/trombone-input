using UnityEngine;
using UnityEngine.UI;

namespace SceneSwitching
{
    public class ButtonDisappear : MonoBehaviour
    {
        GameObject button;
        public Camera skybox;
        // GameObject skybox;
        // public Button StartPracticeButton;
        // // Start is called before the first frame update
        // void Start()
        // {
        //   button = GameObject.FindWithTag("StartButtonTag");
        //   // ButtonClick();
        //
        //   // button.onClick.AddListener(Butt)
        //   // Button sButton = StartPracticeButton.GetComponent<Button>();
        //   // sButton.onClick.AddListener(TaskOnClick);
        // }

        public void ButtonClick()
        {
            var newColor = new Color(0.235f, 0.255f, 0.255f, 1.0f);
            // button = GetComponent<Button>();
            // button = GameObject.FindWithTag("StartButtonTag").GetComponent<Button>();
            // button = GameObject.FindWithTag("ButtonBackgroundTag").transform.GetChild(0).gameObject;
            // button.enabled = false;
            // button.GetComponent<Renderer>().enabled = false;
            // MeshRenderer mr = button.GetComponent<MeshRenderer>();
            // mr.enabled = false;
            button = GameObject.FindWithTag("ButtonBackgroundTag");
            button.SetActive(false);
            // buttonBack.enabled = false;

            // skybox = GameObject.FindWithTag("MainCamera");
            skybox = GetComponent<Camera>();
            skybox = Camera.main;
            skybox.clearFlags = CameraClearFlags.Skybox;
            skybox.backgroundColor = newColor;

            //TODO: Set skybox to EBFFFF
            // skybox = GameObject.FindWithTag("MainCamera");
            // skybox.SetBackgroundColor("EBFFFF");
        }

        // // Update is called once per frame
        // void Update()
        // {
        //
        // }
    }
}
