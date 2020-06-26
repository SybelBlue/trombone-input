using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyToMainButtonScript : MonoBehaviour
{
    public void changeToMain()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("_MAIN");
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
