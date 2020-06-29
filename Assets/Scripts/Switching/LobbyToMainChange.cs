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
        if (CustomInput.Bindings.advanceToMain)
        {
            // SceneManager.LoadScene("_MAIN");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Stylus Trial", LoadSceneMode.Additive);

        }

        // SceneManager.LoadScene("_MAIN");

    }
}
