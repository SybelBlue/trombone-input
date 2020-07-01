using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneSwitching
{
    public static class Utils
    {
        public static readonly string _STRIALS_name = "_STRIALS";
        public static readonly string _LOBBY_name = "_LOBBY";
    }

    public class LobbyToSTRIALSChange : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (CustomInput.Bindings.advanceToMain)//TODO: EXPLORE THIS
            {
                // SceneManager.LoadScene("_MAIN");
                UnityEngine.SceneManagement.SceneManager.LoadScene(Utils._STRIALS_name, LoadSceneMode.Additive);
                // UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);


            }

            // SceneManager.LoadScene("_MAIN");

        }
    }
}
