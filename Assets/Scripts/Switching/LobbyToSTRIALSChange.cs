using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneSwitching
{
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
                ButtonDisappear.ButtonClick();
                UnityEngine.SceneManagement.SceneManager.LoadScene(Utils._STRIALS_name, LoadSceneMode.Additive);


            }

            // SceneManager.LoadScene("_MAIN");

        }
    }
}
