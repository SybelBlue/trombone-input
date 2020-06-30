using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainToLobbyButtonScript : MonoBehaviour
{
    public void changeToLobby()
    {
        // UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("_STRIALS");

    }
    // // Start is called before the first frame update
    // void Start()
    // {
    //
    // }
    //
    // // Update is called once per frame
    // void Update()
    // {
    //
    // }
}
