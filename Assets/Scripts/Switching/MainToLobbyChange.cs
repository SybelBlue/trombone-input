using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainToLobbyChange : MonoBehaviour
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
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("Stylus Trial");

            // SceneManager.LoadScene("Lobby");
        }

    }
}
