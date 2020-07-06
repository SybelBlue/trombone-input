using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// using is = ButtonDisappear;

namespace SceneSwitching
{
    public class LobbyToSTRIALSChange : MonoBehaviour
    {
      // public ButtonDisappear bd;
      // GameObject button;
      // public Camera skybox;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (CustomInput.Bindings.advanceToMain)//TODO: EXPLORE THIS
            {
              // ButtonRETURN();
              // bd.ButtonClick();
                // GetComponent<StartPracticeButton>().onClick.Invoke();
                // sn = GetComponent<StartPracticeButton>().onClick.Invoke();
                UnityEngine.SceneManagement.SceneManager.LoadScene(Utils._STRIALS_name, LoadSceneMode.Additive);


            }

            // SceneManager.LoadScene("_MAIN");

        }

        // public void ButtonRETURN()
        // {
        //     var newColor = new Color(235, 255, 255, 1.0f);
        //     button = GameObject.FindWithTag("StartButtonTag");
        //     button.SetActive(false);
        //
        //     // skybox = GameObject.FindWithTag("MainCamera");
        //     skybox = GetComponent<Camera>();
        //     skybox = Camera.main;
        //     skybox.clearFlags = CameraClearFlags.Skybox;
        //     skybox.backgroundColor = newColor;
        //
        //     //TODO: Set skybox to EBFFFF
        //     // skybox = GameObject.FindWithTag("MainCamera");
        //     // skybox.SetBackgroundColor("EBFFFF");
        // }

    }
}
