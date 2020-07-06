using UnityEngine;

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
            var newColor = new Color(235, 255, 255, 1.0f);
            button = GameObject.FindWithTag("StartButtonTag");
            button.SetActive(false);

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
