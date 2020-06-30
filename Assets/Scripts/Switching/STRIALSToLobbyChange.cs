using System.Collections;
using UnityEngine;

namespace SceneSwitching
{
    public class STRIALSToLobbyChange : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (CustomInput.Bindings.returnToLobby)
            {
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("_STRIALS");
                // UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("Trials");

                // SceneManager.LoadScene("Lobby");
            }

        }
    }
}