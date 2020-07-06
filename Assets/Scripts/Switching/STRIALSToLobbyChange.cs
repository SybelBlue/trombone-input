using System.Collections;
using UnityEngine;

namespace SceneSwitching
{
    public class STRIALSToLobbyChange : MonoBehaviour
    {
      GameObject buttontwo;
      public Camera skybox;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (CustomInput.Bindings.returnToLobby)
            {
              ButtonReRETURN();
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(Utils._STRIALS_name);
                // UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("Trials");

                // SceneManager.LoadScene("Lobby");
            }

        }


        public void ButtonReRETURN()
        {
            var newColor = new Color(184, 158, 195, 0.0f);
            buttontwo = GameObject.FindWithTag("StartButtonTag");
            buttontwo.SetActive(true);

            // skybox = GameObject.FindWithTag("MainCamera");
            skybox = GetComponent<Camera>();
            skybox = Camera.main;
            skybox.clearFlags = CameraClearFlags.Skybox;
            skybox.backgroundColor = newColor;

            //TODO: Set skybox to EBFFFF
            // skybox = GameObject.FindWithTag("MainCamera");
            // skybox.SetBackgroundColor("EBFFFF");
        }
    }
}
