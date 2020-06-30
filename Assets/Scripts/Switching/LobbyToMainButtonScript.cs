using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyToMainButtonScript : MonoBehaviour
{
    public void changeToSTrial()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("_STRIALS", LoadSceneMode.Additive);
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
