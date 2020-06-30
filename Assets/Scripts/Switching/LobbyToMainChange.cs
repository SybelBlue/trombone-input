using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyToMainChange : MonoBehaviour
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
            UnityEngine.SceneManagement.SceneManager.LoadScene("_STRIALS", LoadSceneMode.Additive);

        }

        // SceneManager.LoadScene("_MAIN");

    }
}
