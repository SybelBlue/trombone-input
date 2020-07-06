using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KeyPressTest : MonoBehaviour
{
  public Button backToLobby;
  public Button toTrials;
  public Main other;
  // var return : Keyeve

    // Start is called before the first frame update
    void Start()
    {
      backToLobby = GameObject.FindGameObjectWithTag("JumbBackToLobbyTag").GetComponent<Button>();
      toTrials = GameObject.FindGameObjectWithTag("StartButtonTag").GetComponent<Button>();



    }

    // Update is called once per frame
    void Update()
    {
      Scene scene = SceneManager.GetActiveScene();
      Scene sceeneTwo = SceneManager.GetSceneByName("_STRIALS");
      Debug.Log($"Active Scene: \"{scene.name}\".");

      if(Input.GetKeyDown(KeyCode.Return))
      {
        // if(scene.name.Equals("_STRIALS").isLoaded())
        if(sceeneTwo.isLoaded)
        {
          other.OnSceneAdvance();
          Debug.LogWarning("OnSceneAdvance WORKED");
          backToLobby.onClick.Invoke();
          Debug.LogWarning("backtolobby");


        }
        else
        {
          // Main.OnSceneAdvance();
          toTrials.onClick.Invoke();
          Debug.LogWarning("toTrials WORKED");


        }
      }
      // scene = SceneManager.GetActiveScene();
      // Debug.Log($"Active Scene: \"{scene.name}\".");

    }
}
