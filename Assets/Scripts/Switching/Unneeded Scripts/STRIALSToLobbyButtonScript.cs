using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainToLobbyButtonScript : UnityEngine.MonoBehaviour
{
    public void ChangeToLobby()
    {
        // UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("_STRIALS");

    }
}
